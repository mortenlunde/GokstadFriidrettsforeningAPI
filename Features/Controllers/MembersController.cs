using GokstadFriidrettsforeningAPI.Features.Services.Interfaces;
using GokstadFriidrettsforeningAPI.ModelResponses;
using GokstadFriidrettsforeningAPI.Models;
using GokstadFriidrettsforeningAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace GokstadFriidrettsforeningAPI.Features.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class MembersController(
    ILogger<MembersController> logger,
    IMemberService memberService,
    ITokenService tokenService) : ControllerBase
{
    [Authorize]
    [HttpGet(Name = "GetMembers")]
    public async Task<IActionResult> GetMembers([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        IEnumerable<MemberResponse> memberResponses = await memberService.GetPagedAsync(page, pageSize);
        return Ok(memberResponses);
    }

    [AllowAnonymous]
    [HttpPost("Register", Name = "RegisterMember")]
    public async Task<ActionResult<MemberResponse>> RegisterMemberAsync(MemberRegistration memberRegistration)
    {
        MemberResponse? member = await memberService.RegisterAsync(memberRegistration);
        return member is null
            ? BadRequest("Failed to register new user")
            : Ok(member);
    }
    
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        Member? user = await memberService.AuthenticateUserAsync(request.Email, request.Password);
        
        if (user == null)
            return Unauthorized(new { Message = "Invalid username or password" });

        string token = tokenService.GenerateToken(user.MemberId, user.Email);
        return Ok(new { Token = token });
    }
}