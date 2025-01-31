using ErrorOr;
using FluentValidation;
using Homemap.ApplicationCore.Interfaces.Repositories;
using Homemap.ApplicationCore.Interfaces.Services;
using Homemap.ApplicationCore.Models;
using Homemap.ApplicationCore.Models.Auth;
using Homemap.WebAPI.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace Homemap.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(
        IAuthService authService,
        IUserRepository userRepository,
        IPasswordHasher<User> passwordHasher) : ControllerBase
    {
        private readonly IAuthService _authService = authService;
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IPasswordHasher<User> _passwordHasher = passwordHasher;

        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _authService.ValidateCredentialsAsync(request.Email, request.Password);
            if (user == null)
            {
                return Unauthorized();
            }

            var (accessToken, refreshToken) = await _authService.GenerateTokensAsync(user);

            SetRefreshTokenCookie(refreshToken);

            return Ok(new
            {
                accessToken,
                user = new { user.Id, user.Email, user.Name }
            });
        }

        [HttpPost("refresh-token")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> RefreshToken()
        {
            var refreshToken = Request.Cookies["refresh_token"];

            if (string.IsNullOrEmpty(refreshToken) ||
                !await _authService.ValidateRefreshTokenAsync(refreshToken))
            {
                return Unauthorized();
            }

            var session = await _userRepository.GetSessionByRefreshTokenAsync(refreshToken);
            var user = await _userRepository.GetUserByIdAsync(session.UserId);

            var (newAccessToken, newRefreshToken) = await _authService.GenerateTokensAsync(user);

            SetRefreshTokenCookie(newRefreshToken);

            return Ok(new
            {
                accessToken = newAccessToken
            });
        }

        // TODO: Logout api, implement InvalidateToken machanics


        [HttpGet("check-session")]
        public async Task<IActionResult> CheckSession()
        {
            var refreshToken = Request.Cookies["refresh_token"];
            if (string.IsNullOrEmpty(refreshToken) || !await _authService.ValidateRefreshTokenAsync(refreshToken))
            {
                return Unauthorized();
            }

            return Ok();
        }

        private bool ValidateRefreshToken(string token)
        {
            // Logic to validate the refresh token
            return true; // Replace with actual validation logic
        }

        private void SetRefreshTokenCookie(string refreshToken)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = false, // Always use true in production
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(7), // Refresh token typically lives longer
                Path = "/",
                Domain = null, // Set your domain in production
            };

            Response.Cookies.Append("refresh_token", refreshToken, cookieOptions);
        }

        [HttpPost("google-login")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequest request)
        {
            if (request == null)
            {
                return BadRequest(new
                {
                    message = "Login request is missing.",
                    errorCode = "INVALID_REQUEST"
                });
            }

            if (string.IsNullOrWhiteSpace(request.IdToken))
            {
                return BadRequest(new
                {
                    message = "ID Token is required.",
                    errorCode = "MISSING_TOKEN"
                });
            }

            try
            {
                var payload = await _authService.VerifyGoogleToken(request.IdToken);

                if (payload == null)
                {
                    return Unauthorized(new
                    {
                        message = "Invalid or expired Google token.",
                        errorCode = "INVALID_TOKEN"
                    });
                }

                var (user, message) = await _authService.HandleGoogleUserAsync(payload);
                var (accessToken, refreshToken) = await _authService.GenerateTokensAsync(user);

                SetRefreshTokenCookie(refreshToken);

                Response.Cookies.Append("access_token", accessToken, new CookieOptions
                {
                    HttpOnly = false,
                    Secure = true,
                    SameSite = SameSiteMode.Lax
                });

                return Ok(new
                {
                    message,
                    accessToken,
                    refreshToken,
                    user = new { user.Id, user.Email, user.Name }
                });
            }
            catch (Exception ex)
            {

                return StatusCode(500, new
                {
                    message = "An unexpected error occurred during login.",
                    errorCode = "LOGIN_ERROR"
                });
            }
        }
    }
}
