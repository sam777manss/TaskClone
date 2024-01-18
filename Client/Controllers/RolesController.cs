using Client.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Client.Controllers
{
    [Authorize(Roles = "Admin")]
    public class RolesController : Controller
    {
        public static string URL = "https://localhost:7257/";

        public RolesController()
        {
        }
        public async Task<IActionResult> Index()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(URL);
                var response = await client.GetAsync("Roles/Index"); // using concatenation
                var responseContent = await response.Content.ReadAsStringAsync();
                List<Roles> user = JsonConvert.DeserializeObject<List<Roles>>(responseContent);
                if (response.IsSuccessStatusCode)
                {
                    return View(user);
                }
            }
            return View();
        }

        public async Task<IActionResult> Update(string Id)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(URL);
                var response = await client.GetAsync("Roles/Update?Id=" + Id); // using concatenation
                var responseContent = await response.Content.ReadAsStringAsync();
                RoleEdit user = JsonConvert.DeserializeObject<RoleEdit>(responseContent);
                if (response.IsSuccessStatusCode)
                {
                    return View(new RoleEdit
                    {
                        Role = user.Role,
                        Members = user.Members,
                        NonMembers = user.NonMembers
                    });
                }
            }
            return View(new RoleEdit());
            //IdentityRole role = await roleManager.FindByIdAsync(id);
            //List<AppUser> members = new List<AppUser>();
            //List<AppUser> nonMembers = new List<AppUser>();

            //foreach (AppUser user in userManager.Users)
            //{
            //    var list = await userManager.IsInRoleAsync(user, role.Name) ? members : nonMembers;
            //    list.Add(user);
            //}
            //return View(new RoleEdit
            //{
            //    Role = role,
            //    Members = members,
            //    NonMembers = nonMembers
            //});
        }
        [HttpPost]
        public async Task<IActionResult> Update(RoleModification model)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(URL + "Roles/Update");
                var response = await client.PostAsJsonAsync("", model);
            }
            return await Update(model.RoleId);
        }

        public async Task<IActionResult> Create() => View();

        [HttpPost]
        public async Task<bool> Create(string rolename)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(URL);
                var response = await client.GetAsync("Roles/Create?rolename=" + rolename);
                var responseContent = await response.Content.ReadAsStringAsync();
                bool user = JsonConvert.DeserializeObject<bool>(responseContent);
                return user;
            }
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string Id)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(URL);
                var response = await client.GetAsync("Roles/Delete?Id=" + Id);
                var responseContent = await response.Content.ReadAsStringAsync();
                bool user = JsonConvert.DeserializeObject<bool>(responseContent);
                return RedirectToAction("Index", "Roles");
            }
        }
    }
}
