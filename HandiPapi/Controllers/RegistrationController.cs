using AutoMapper;
using HandiPapi.DataAccess;
using HandiPapi.Models;
using HandiPapi.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HandiPapi.Controllers
{
    [Route("api/[controller]")]
    // [ApiController]
    public class RegistrationController : ControllerBase
    {
        private readonly UserManager<ApiUser> userManager;
        private readonly ILogger<RegistrationController> logger;
        private readonly IMapper mapper;
        private readonly IAuthManager authManager;

        public RegistrationController(UserManager<ApiUser> userManager, ILogger<RegistrationController> logger, IMapper mapper, IAuthManager authManager)
        {
            this.userManager = userManager;
            this.logger = logger;
            this.mapper = mapper;
            this.authManager = authManager;
        }

        [HttpPost]
        [Route("register")]

        public async Task<IActionResult> Register([FromBody] UserDto userDto)
        {
            logger.LogInformation($"Registration attempt for {userDto.Email}");
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var user = mapper.Map<ApiUser>(userDto);
                user.UserName = userDto.Email;
                var result = await userManager.CreateAsync(user);

                if (!result.Succeeded)
                {
                    foreach(var error in result.Errors)
                    {
                        ModelState.AddModelError(error.Code, error.Description);
                    }
                    return BadRequest(ModelState);
                }
                return Accepted();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Something Went Wrong in the {nameof(Register)}");
                return Problem($"Something Went wrong in the{nameof(Register)}", statusCode: 500);
            }
        }
    }
}

