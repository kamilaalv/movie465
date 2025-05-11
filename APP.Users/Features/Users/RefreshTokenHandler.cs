using APP.Users.Domain;
using CORE.APP.Features;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace APP.Users.Features.Users
{
    public class RefreshTokenRequest : Request<TokenResponse>
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }

    public class RefreshTokenHandler : Handler<RefreshTokenRequest, TokenResponse>
    {
        private readonly UsersDbHandler _dbHandler;
        private readonly AppSettings _appSettings;

        public RefreshTokenHandler(UsersDbHandler dbHandler, AppSettings appSettings)
        {
            _dbHandler = dbHandler;
            _appSettings = appSettings;
        }

        public override async Task<TokenResponse> Handle(RefreshTokenRequest request, CancellationToken cancellationToken)
        {
            // Validate the expired access token
            ClaimsPrincipal principal;
            try
            {
                principal = _dbHandler.GetPrincipalFromExpiredToken(request.AccessToken);
            }
            catch (Exception)
            {
                return new TokenResponse { IsSuccessful = false, Message = "Invalid access token" };
            }

            // Get user id from the claims
            var userIdClaim = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
                return new TokenResponse { IsSuccessful = false, Message = "Invalid access token" };

            // Find user with the given refresh token
            var user = await _dbHandler.GetUserByRefreshToken(request.RefreshToken);
            if (user == null || user.Id != userId)
                return new TokenResponse { IsSuccessful = false, Message = "Invalid refresh token" };

            // Generate new tokens
            var newAccessToken = _dbHandler.CreateAccessToken(user);
            var newRefreshToken = _dbHandler.CreateRefreshToken();

            // Set token expiration times
            var accessTokenExpiration = DateTime.UtcNow.AddMinutes(_appSettings.TokenExpirationInMinutes);
            var refreshTokenExpiration = DateTime.UtcNow.AddDays(_appSettings.RefreshTokenExpirationInDays);

            // Update user with new refresh token
            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiration = refreshTokenExpiration;

            await _dbHandler.Update(user);

            return new TokenResponse
            {
                IsSuccessful = true,
                Message = "Token refresh successful",
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
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