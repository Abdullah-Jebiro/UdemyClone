using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Dtos;
using Models.Dtos.Account;
using Models.ResponseModels;
using Services.Account;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "AdminOnly")] // Requires admin permission to access this controller

    public class AdminController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly IMapper _mapper;
        public AdminController(IAccountService accountService, IMapper mapper)
        {
            _accountService = accountService;
            _mapper = mapper;
        }


        [HttpGet("AllUser")]
        public async Task<IActionResult> GetAllUser()
        {
            var userList = await _accountService.GetUsersAsync();
            var data = _mapper.Map<IReadOnlyList<UserwithoutRoleDto>>(userList);

            return Ok(new BaseResponse<IReadOnlyList<UserwithoutRoleDto>>(data, $"User List"));
        }



        [HttpGet("AllUserWithRoles")]
        public async Task<IActionResult> GetAllUserWithRoles()
        {
            var userList = await _accountService.GetUsersAsync();

            var result = userList.Select(x => new UserDto
            {
                Email = x.Email,
                UserName = x.UserName,
                Roles = x.UserRoles.ToList().Select(y => y.Role.Name.ToString()).ToList()
            });

            return Ok(new BaseResponse<IEnumerable<UserDto>>(result, $"User List"));
        }

        [HttpPost("addRole")]
        public async Task<IActionResult> AddRoleForUser(AddRoleRequest request)
        {
            var response = await _accountService.AddRoleForUser(request.UserId, request.roleId);

            if (response.IsSuccess)
            {
                return Ok(response.Message);
            }
            else
            {
                return BadRequest(response.Message);
            }
        }

    }
}

