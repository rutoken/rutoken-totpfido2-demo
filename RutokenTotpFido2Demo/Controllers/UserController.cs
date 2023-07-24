using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RutokenTotpFido2Demo.Extensions;
using RutokenTotpFido2Demo.Models;
using RutokenTotpFido2Demo.Services;

namespace RutokenTotpFido2Demo.Controllers;

[ApiController]
[Route("user")]
public class UserController : ControllerBase
{
    private readonly UserService _userService;

    public UserController(UserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    [Route("loginstate")]
    [Authorize]
    public IActionResult LoginState()
    {
        return Ok();
    }

    [HttpGet]
    [Route("logout")]
    [Authorize]
    public async Task<IActionResult> SignOut()
    {
        await HttpContext.SignOutAsync();
        return Ok();
    }

    [HttpGet]
    [Route("info")]
    [Authorize]
    public async Task<IActionResult> Info()
    {
        var userInfo = await _userService.GetUserInfo(User.UserId());

        return Ok(userInfo);
    }


    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> Register([FromBody] UserRegisterDto model)
    {
        await _userService.Register(model);
        return Ok();
    }

    [HttpGet]
    [Route("registertotp")]
    public async Task<IActionResult> RegisterTotp()
    {
        await _userService.RegisterTotp(User.UserId());
        return Ok();
    }

    [HttpGet]
    [Route("removetotp/{id}")]
    public async Task<IActionResult> RemoveTotp([FromRoute] int id)
    {
        await _userService.RemoveTotp(User.UserId(), id);
        return Ok();
    }

    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> Login([FromBody] UserRegisterDto model)
    {
        var claims = new List<Claim>();

        var user = await _userService.Login(model);

        claims.AddRange(new List<Claim>
        {
            new("Id", user.Id.ToString()),
        });

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

        await
            HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity));

        return Ok();
    }
}