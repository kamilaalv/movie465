using APP.Users.Domain;
using CORE.APP.Features;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace APP.Users.Features.Users
{
    public class TokenRequest : Request<TokenResponse>
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }

    public class TokenResponse : Response
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime AccessTokenExpiration { get; set; }
        public DateTime RefreshTokenExpiration { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Role { get; set; }
    }

    public class TokenHandler : Handler<TokenRequest, TokenResponse>
    {
        private readonly UsersDbHandler _dbHandler;
        private readonly AppSettings _appSettings;

        public TokenHandler(UsersDbHandler dbHandler, AppSettings appSettings)
        {
            _dbHandler = dbHandler;
            _appSettings = appSettings;
        }

        public override async Task<TokenResponse> Handle(TokenRequest request, CancellationToken cancellationToken)
        {
            // Find user by username
            var user = await _dbHandler.Query<User>(u => u.UserName == request.UserName)
                .Include(u => u.Role)
                .FirstOrDefaultAsync(cancellationToken);

            if (user == null)
                return new TokenResponse { IsSuccessful = false, Message = "Invalid username or password" };

            // Verify password
            if (user.Password != request.Password) // In a real app, use password hashing
                return new TokenResponse { IsSuccessful = false, Message = "Invalid username or password" };

            if (!user.IsActive)
                return new TokenResponse { IsSuccessful = false, Message = "User account is inactive" };

            // Generate JWT token
            var accessToken = _dbHandler.CreateAccessToken(user);
            var refreshToken = _dbHandler.CreateRefreshToken();

            // Set token expiration times
            var accessTokenExpiration = DateTime.UtcNow.AddMinutes(_appSettings.TokenExpirationInMinutes);
            var refreshTokenExpiration = DateTime.UtcNow.AddDays(_appSettings.RefreshTokenExpirationInDays);

            // Update user with refresh token
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiration = refreshTokenExpiration;

            await _dbHandler.Update(user);

            return new TokenResponse
            {
                IsSuccessful = true,
                Message = "Authentication successful",
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                AccessTokenExpiration = accessTokenExpiration,
                RefreshTokenExpiration = refreshTokenExpiration,
                UserId = user.Id,
                UserName = user.UserName,
                FullName = $"{user.Name} {user.Surname}",
                Role = user.Role.Name
            };
        }
    }
}