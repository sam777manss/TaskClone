using Microsoft.AspNetCore.Mvc;
using ShoesApi.Interfaces;
using ShoesApi.Models;

namespace ShoesApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {
        // Need To be included in constructor all the interfaces and other dbcontext manager
        private readonly IUser _user;
        // Need To be included in constructor all the interfaces
        public AuthenticateController(IUser user)
        {
            _user = user;
        }

        [HttpGet]
        [Route("Index")]
        public IActionResult Index()
        {
            return Ok();
        }

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register(Register register)
        {
            if (ModelState.IsValid)
            {
                bool successful = await _user.RegisterUser(register);
                if (successful)
                {
                    return Ok();
                }
                return BadRequest();
            }
            return Unauthorized();
        }

        [HttpPost]
        [Route("RegisterAdmin")]
        public async Task<IActionResult> RegisterAdmin(Register registerAdmin)
        {
            if (ModelState.IsValid)
            {
                Response success = await _user.RegisterAdmin(registerAdmin);

                if (success.Message != null)
                {
                    return Ok(success);
                }
                return BadRequest();
            }
            return Unauthorized();
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login(Login login)
        {
            if (ModelState.IsValid)
            {
                CommonIndex successful = await _user.LoginUser(login);
                return Ok(successful);
            }
            return Unauthorized();
        }
        //[HttpGet]
        //[Route("LogOut")]
        //public async Task<bool> LogOut()
        //{
        //    AppUser user = await userManager.GetUserAsync(HttpContext.User);

        //    if (ModelState.IsValid)
        //    {
        //        bool successful = await _user.LogOut();
        //        return true;
        //    }
        //    return false;
        //}
    }
}
