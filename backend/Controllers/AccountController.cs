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

        public AccountsController(
            IAccountRepository accountRepository,
            AppDbContext context)
        {
            _accountRepository = accountRepository;
            _context = context;
        }

        // =========================================================
        // GET /api/accounts
        // Unified list for the admin User Management table.
        // Joins Accounts with Parents / Users depending on AccountType.
        // =========================================================

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
                        firstName = user.FirstName,
                        middleName = user.MiddleName,
                        lastName = user.LastName,
                        username = account.Username,
                        email = user.Email,
                        contactNo = user.ContactNo,
                        role = user.UserType,
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

        [HttpPost("personnel")]
public async Task<IActionResult> CreatePersonnelAccount(
    [FromBody] CreatePersonnelAccountDto dto)
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
        UserType = dto.Role == "Staff"
        ? "Staff"
        : "Healthcare",
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

        temporaryPassword
    });
}

         

           

        // =========================================================
        // PATCH /api/accounts/{id}/status
        // Active / Inactive only.
        // =========================================================

        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateStatus(
            Guid id,
            [FromBody] UpdateAccountStatusDto dto)
        {
            var account = await _accountRepository.GetByIdAsync(id);

            if (account == null)
                return NotFound(new { message = "Account not found." });

            account.Status = dto.Status;
            account.UpdatedAt = DateTime.Now;

            await _accountRepository.UpdateAsync(account);

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

        [HttpPost("{id}/reset-password")]
        public async Task<IActionResult> ResetPassword(Guid id)
        {
            var account = await _accountRepository.GetByIdAsync(id);

            if (account == null)
                return NotFound(new { message = "Account not found." });

            string temporaryPassword = GenerateTemporaryPassword();

            account.PasswordHash = BCrypt.Net.BCrypt.HashPassword(temporaryPassword);
            account.MustChangePassword = true;
            account.UpdatedAt = DateTime.Now;

            await _accountRepository.UpdateAsync(account);

            return Ok(new
            {
                message = "Password reset successfully.",
                accountID = account.AccountID,
                temporaryPassword
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