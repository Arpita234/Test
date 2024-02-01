using ConfigFileManagementApp.Interface;
using ConfigFileManagementApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace ConfigFileManagementApp.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly IUserService _userService;
        private readonly IConfiguration _iConfig;

        public AuthController(IUserService userService, IConfiguration iConfig)
        {
            _userService = userService;
            _iConfig = iConfig;
        }

        // POST api/auth/login
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public IActionResult Post(UserLogin loginDetails)
        {
            try
            {
                if (string.IsNullOrEmpty(loginDetails.Username) || string.IsNullOrEmpty(loginDetails.Password))
                {
                    return BadRequest("Invalid user credentials");
                }

                var user = _userService.Authenticate(loginDetails.Username, loginDetails.Password);

                if (user == null)
                {
                    return Unauthorized();
                }

                var token = _userService.GenerateJwtToken(user, _iConfig);

                return Ok(new { Token = token });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

    }
}
