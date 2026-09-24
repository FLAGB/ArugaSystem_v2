using AndroidWebAPI.Models;

namespace AndroidWebAPI.Data
{
    public interface IAccountRepository
    {
      Task<Account?> LoginAsync(
    string identifier,
    string password,
    string accountType);

        Task<Account?> GetByIdAsync(Guid accountId);

        Task<Account?> GetByEmailAsync(string email);

        Task<Account?> GetByUsernameAsync(string username);

        Task<Account> CreateAsync(Account account);

        Task<Account> UpdateAsync(Account account);

        Task<bool> ChangePasswordAsync(Guid accountId, string newPassword);

        Task<bool> UpdateLastLoginAsync(Guid accountId);

        Task<bool> DeleteAsync(Guid accountId);
    }
}