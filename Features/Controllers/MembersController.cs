using GokstadFriidrettsforeningAPI.Features.Services.Interfaces;
using GokstadFriidrettsforeningAPI.ModelResponses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GokstadFriidrettsforeningAPI.Features.Controllers;

[ApiController]
[AllowAnonymous]
[Route("api/v1/[controller]")]
public class MembersController(
    ILogger<MembersController> logger,
    IMemberService memberService) : ControllerBase
{
    [HttpGet(Name = "GetMembers")]
    public async Task<IActionResult> GetMembers([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        IEnumerable<MemberResponse> memberResponses = await memberService.GetPagedAsync(page, pageSize);
        return Ok(memberResponses);
    }
}