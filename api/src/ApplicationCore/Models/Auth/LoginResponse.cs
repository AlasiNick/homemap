﻿namespace Homemap.ApplicationCore.Models.Auth;

public class LoginResponse
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public UserDto User { get; set; }
}
