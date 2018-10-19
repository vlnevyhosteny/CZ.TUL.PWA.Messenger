using System;
using System.Threading.Tasks;
using CZ.TUL.PWA.Messenger.Server.Model;

namespace CZ.TUL.PWA.Messenger.Server.Services
{
    public interface ITokenService
    {
        Task SetRefreshToken(User user, RefreshToken refreshToken);

        Task<RefreshToken> GetRefreshToken(User user);

        Task RevokeRefreshToken(User user);

        Task<User> ValidateUser(string userName, string password);

        string GenerateJwtToken(string userName);

        string GenerateRefreshToken();

        string GetUserNameFromJwtToken(string token);

        Task<User> ValidateRefreshToken(string userName, string refreshToken);
    }
}