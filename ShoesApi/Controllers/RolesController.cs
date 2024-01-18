using Microsoft.AspNetCore.Mvc;
using ShoesApi.Interfaces;
using ShoesApi.Models;

namespace ShoesApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly IRoles rolesData;
        public RolesController(IRoles rolesData)
        {
            this.rolesData = rolesData;
        }

        [HttpGet]
        [Route("Index")]
        public async Task<IActionResult> Index()
        {
            List<Roles> roles = await rolesData.Index();
            return Ok(roles);
        }

        [HttpGet]
        [Route("Update")]
        public async Task<IActionResult> Update(string Id)
        {
            RoleEdit Editroles = await rolesData.Update(Id);
            return Ok(Editroles);
        }
        [HttpPost]
        [Route("Update")]
        public async Task<IActionResult> Update(RoleModification model)
        {
            bool flag = await rolesData.Update(model);
            return Ok(flag);
        }
        [HttpGet]
        [Route("Create")]
        public async Task<IActionResult> Create(string rolename)
        {
            bool flag = await rolesData.Create(rolename);
            return Ok(flag);
        }

        [HttpGet]
        [Route("Delete")]
        public async Task<IActionResult> Delete(string Id)
        {
            bool flag = await rolesData.Delete(Id);
            return Ok(flag);
        }
    }
}
