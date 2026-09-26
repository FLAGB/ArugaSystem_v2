using AndroidWebAPI.Data;
using AndroidWebAPI.DTOs;
using AndroidWebAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AndroidWebAPI.Controllers
{
    [ApiController]
    [Route("api/accounts")]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountRepository _accountRepository;
        private readonly AppDbContext _context;
        private readonly AndroidWebAPI.Services.AuditService _audit;

        public AccountsController(
            IAccountRepository accountRepository,
            AppDbContext context,
            AndroidWebAPI.Services.AuditService audit)
        {
            _accountRepository = accountRepository;
            _context = context;
            _audit = audit;
        }

        // =========================================================
        // GET /api/accounts
        // Unified list for the admin User Management table.
        // Joins Accounts with Parents / Users depending on AccountType.
        // =========================================================

        [Microsoft.AspNetCore.Authorization.Authorize(Roles = AndroidWebAPI.Services.Roles.Admin)]
        [HttpGet]
        public async Task<IActionResult> GetAllAccounts()
        {
            var accounts = await _context.Accounts
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();

            var parents = await _context.Parents.ToDictionaryAsync(p => p.ParentID);
            var users = await _context.Users.ToDictionaryAsync(u => u.UserID);

            var result = new List<object>();

            foreach (var account in accounts)
            {
                if (account.AccountType == "Parent" &&
                    parents.TryGetValue(account.ReferenceID, out var parent))
                {
                    result.Add(new
                    {
                        accountID = account.AccountID,
                        referenceID = account.ReferenceID,
                        firstName = parent.FirstName,
                        middleName = parent.MiddleName,
                        lastName = parent.LastName,
                        username = account.Username,
                        email = parent.Email,
                        contactNo = parent.ContactNo,
                        role = "Parent",
                        status = account.Status ? "Active" : "Inactive",
                        mustChangePassword = account.MustChangePassword,
                        lastLogin = account.LastLogin,
                        createdAt = account.CreatedAt
                    });
                }
                else if (account.AccountType == "Personnel" &&
                         users.TryGetValue(account.ReferenceID, out var user))
                {
                    result.Add(new
                    {
                        accountID = account.AccountID,
                        referenceID = account.ReferenceID,
                        firstName = user.FirstName,
                        middleName = user.MiddleName,
                        lastName = user.LastName,
                        username = account.Username,
                        email = user.Email,
                        contactNo = user.ContactNo,
                        // Position is the real role (Doctor / Nurse / Staff /
                        // Administrator); UserType is a legacy column whose
                        // values ("Admission", ...) don't match the UI's roles.
                        role = string.IsNullOrWhiteSpace(user.Position) ? user.UserType : user.Position,
                        status = account.Status ? "Active" : "Inactive",
                        mustChangePassword = account.MustChangePassword,
                        lastLogin = account.LastLogin,
                        createdAt = account.CreatedAt
                    });
                }
                // Accounts whose Parent/User record is missing (orphaned) are skipped
                // rather than crashing the whole list.
            }

            return Ok(result);
        }

        // =========================================================
        // POST /api/accounts/personnel
        // =========================================================

        [Microsoft.AspNetCore.Authorization.Authorize(Roles = AndroidWebAPI.Services.Roles.Admin)]
        [HttpPost("personnel")]
public async Task<IActionResult> CreatePersonnelAccount(
    [FromBody] CreatePersonnelAccountDto dto,
    [FromServices] AndroidWebAPI.Services.MessageSender sender)
{
    if (string.IsNullOrWhiteSpace(dto.FirstName))
        return BadRequest(new { message = "First name is required." });

    if (string.IsNullOrWhiteSpace(dto.LastName))
        return BadRequest(new { message = "Last name is required." });

    if (string.IsNullOrWhiteSpace(dto.Role))
        return BadRequest(new { message = "Role is required." });

    var validRoles = new[] { "Doctor", "Nurse", "Staff" };

    if (!validRoles.Contains(dto.Role))
    {
        return BadRequest(new
        {
            message = "Role must be Doctor, Nurse, or Staff."
        });
    }

    // =========================================================
    // GENERATE USERNAME AUTOMATICALLY
    // =========================================================

    string username = await GenerateUniqueUsernameAsync(
        dto.FirstName,
        dto.LastName
    );

    // Generate temporary password
    string temporaryPassword = GenerateTemporaryPassword();

    string passwordHash =
        BCrypt.Net.BCrypt.HashPassword(temporaryPassword);

    // =========================================================
    // 1. CREATE USER / PERSONNEL RECORD
    // =========================================================

    var user = new User
    {
        UserID = Guid.NewGuid(),
        FirstName = dto.FirstName,
        MiddleName = dto.MiddleName,
        LastName = dto.LastName,
        Username = username,
        PasswordHash = passwordHash,
        Email = dto.Email,
        ContactNo = dto.ContactNo,
        Address = dto.Address,
        // dbo.Users has a CHECK constraint allowing only
        // 'Doctor' | 'Nurse' | 'Admission' | 'Request' here — writing
        // "Healthcare"/"Staff" made every account creation fail. Position
        // (below) is what login actually uses to pick the portal.
        UserType = dto.Role == "Staff"
        ? "Admission"
        : dto.Role,
Position = dto.Role,
        PRCNo = dto.LicenseNumber,
        AccountStatus = "Active"
    };

    _context.Users.Add(user);

    await _context.SaveChangesAsync();

    // =========================================================
    // 2. CREATE ACCOUNT RECORD
    // =========================================================

    var account = new Account
    {
        AccountID = Guid.NewGuid(),
        Username = username,
        PasswordHash = passwordHash,
        AccountType = "Personnel",
        ReferenceID = user.UserID,
        Status = true,
        MustChangePassword = true,
        FailedLoginAttempts = 0,
        LockedUntil = null,
        LastLogin = null,
        CreatedAt = DateTime.Now,
        UpdatedAt = null
    };

    await _accountRepository.CreateAsync(account);

    await _audit.LogAsync("User Management", "Create",
        $"{dto.Role} Account – {user.FirstName} {user.LastName}",
        $"Created a new {dto.Role} account (username {username}).",
        newValue: $"Status: Active, Role: {dto.Role}");

    // Sign-in details to the new staff member (the admin also sees them)
    bool emailed = await sender.SendEmailAsync(dto.Email, "Your Aruga staff account",
        $"Hi {dto.FirstName},\n\nAn Aruga account was created for you at Leveriza Health Center " +
        $"({(dto.Role == "Staff" ? "Admission Staff" : dto.Role)}).\n\n" +
        $"Username: {username}\nTemporary password: {temporaryPassword}\n\n" +
        "You'll be asked to choose your own password the first time you sign in.");

    // =========================================================
    // RETURN GENERATED CREDENTIALS
    // =========================================================

    return Ok(new
    {
        message = "Personnel account created successfully.",

        account = new
        {
            account.AccountID,
            account.Username,
            account.AccountType,
            account.ReferenceID,

            user.UserID,
            user.FirstName,
            user.MiddleName,
            user.LastName,
            user.UserType,
            user.PRCNo,
            user.ContactNo,
            user.Email,
            user.Address
        },

        temporaryPassword,
        emailed
    });
}

         

           

        // =========================================================
        // PATCH /api/accounts/{id}/status
        // Active / Inactive only.
        // =========================================================

        [Microsoft.AspNetCore.Authorization.Authorize(Roles = AndroidWebAPI.Services.Roles.Admin)]
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateStatus(
            Guid id,
            [FromBody] UpdateAccountStatusDto dto)
        {
            var account = await _accountRepository.GetByIdAsync(id);

            if (account == null)
                return NotFound(new { message = "Account not found." });

            bool previous = account.Status;
            account.Status = dto.Status;
            account.UpdatedAt = DateTime.Now;

            await _accountRepository.UpdateAsync(account);

            await _audit.LogAsync("User Management", dto.Status ? "Activate" : "Deactivate",
                $"Account – {account.Username}",
                dto.Status ? "Activated a user account." : "Deactivated a user account.",
                oldValue: $"Status: {(previous ? "Active" : "Inactive")}",
                newValue: $"Status: {(dto.Status ? "Active" : "Inactive")}");

            return Ok(new
            {
                message = dto.Status ? "Account activated." : "Account deactivated.",
                accountID = account.AccountID,
                status = account.Status ? "Active" : "Inactive"
            });
        }

        // =========================================================
        // POST /api/accounts/{id}/reset-password
        // Admin-triggered reset. Forces MustChangePassword back to true.
        // =========================================================

        [Microsoft.AspNetCore.Authorization.Authorize(Roles = AndroidWebAPI.Services.Roles.Admin)]
        [HttpPost("{id}/reset-password")]
        public async Task<IActionResult> ResetPassword(Guid id, [FromServices] AndroidWebAPI.Services.MessageSender sender)
        {
            var account = await _accountRepository.GetByIdAsync(id);

            if (account == null)
                return NotFound(new { message = "Account not found." });

            string temporaryPassword = GenerateTemporaryPassword();

            account.PasswordHash = BCrypt.Net.BCrypt.HashPassword(temporaryPassword);
            account.MustChangePassword = true;
            account.UpdatedAt = DateTime.Now;

            await _accountRepository.UpdateAsync(account);

            await _audit.LogAsync("User Management", "Reset Password",
                $"Account – {account.Username}",
                "Reset the account password to a temporary one (must be changed on next login).");

            // Tell the owner their temporary password, by email
            string? email = account.AccountType == "Parent"
                ? await _context.Parents.Where(p => p.ParentID == account.ReferenceID).Select(p => p.Email).FirstOrDefaultAsync()
                : await _context.Users.Where(u => u.UserID == account.ReferenceID).Select(u => u.Email).FirstOrDefaultAsync();
            bool emailed = await sender.SendEmailAsync(email, "Your Aruga password was reset",
                $"The administrator reset the password for your Aruga account ({account.Username}).\n\n" +
                $"Temporary password: {temporaryPassword}\n\n" +
                "You'll be asked to choose your own password the next time you sign in. " +
                "If you didn't ask for this, please contact Leveriza Health Center.");

            return Ok(new
            {
                message = "Password reset successfully.",
                accountID = account.AccountID,
                temporaryPassword,
                emailed
            });
        }

        private async Task<string> GenerateUniqueUsernameAsync(
    string firstName,
    string lastName)
{
    string first = NormalizeName(firstName);
    string last = NormalizeName(lastName);

    string baseUsername = $"{first}_{last}";

    string username = baseUsername;
    int counter = 1;

    while (await _context.Accounts.AnyAsync(a => a.Username == username))
    {
        username = $"{baseUsername}{counter:00}";
        counter++;
    }

    return username;
}

private static string NormalizeName(string name)
{
    return new string(
        name
            .Trim()
            .ToLowerInvariant()
            .Where(c => char.IsLetterOrDigit(c))
            .ToArray()
    );
}

        private static string GenerateTemporaryPassword()
        {
            const string characters =
                "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz23456789";

            var random = new Random();

            return new string(
                Enumerable
                    .Range(0, 10)
                    .Select(_ => characters[random.Next(characters.Length)])
                    .ToArray()
            );
        }
    }

    public class UpdateAccountStatusDto
    {
        public bool Status { get; set; }
    }
}