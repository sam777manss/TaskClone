using log4net;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ShoesApi.Interfaces;
using ShoesApi.Models;

namespace ShoesApi.Repositories
{
    public class RolesRepositories : IRoles
    {
        private RoleManager<IdentityRole> roleManager;
        private UserManager<AppUser> userManager;
        private static readonly ILog log = LogManager.GetLogger(typeof(UserRepositories));

        public RolesRepositories(RoleManager<IdentityRole> roleManager, UserManager<AppUser> userManager)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
        }

        public async Task<List<Roles>> Index()
        {
            List<Roles> rolesData = new List<Roles>();
            try
            {
                foreach (var role in roleManager.Roles)
                {
                    Roles roles = new Roles()
                    {
                        Id = role.Id,
                        Name = role.Name,
                    };
                    rolesData.Add(roles);
                }
                return (rolesData);
            }
            catch (Exception ex)
            {
                log.Error(ex.InnerException != null ? string.Format("Inner Exception: {0} --- Exception: {1}", ex.InnerException.Message, ex.Message) : ex.Message, ex);
            }
            return rolesData;
        }
        public async Task<RoleEdit> Update(string Id)
        {
            try
            {
                IdentityRole role = await roleManager.FindByIdAsync(Id);
                List<AppUser> members = new List<AppUser>();
                List<AppUser> nonMembers = new List<AppUser>();

                List<AppUser> users = await userManager.Users.ToListAsync();


                foreach (AppUser user in users)
                {
                    var list = await userManager.IsInRoleAsync(user, role.Name) ? members : nonMembers;
                    list.Add(user);
                }
                return new RoleEdit
                {
                    Role = role,
                    Members = members,
                    NonMembers = nonMembers
                };
            }
            catch (Exception ex)
            {
                log.Error(ex.InnerException != null ? string.Format("Inner Exception: {0} --- Exception: {1}", ex.InnerException.Message, ex.Message) : ex.Message, ex);
            }
            return new RoleEdit();
        }
        public async Task<bool> Update(RoleModification roleModification)
        {
            try
            {
                IdentityResult result;
                foreach (string userId in roleModification.AddIds ?? new string[] { })
                {
                    AppUser user = await userManager.FindByIdAsync(userId);
                    if (user != null)
                    {
                        result = await userManager.AddToRoleAsync(user, roleModification.RoleName);
                        //if (!result.Succeeded)
                    }
                }
                foreach (string userId in roleModification.DeleteIds ?? new string[] { })
                {
                    AppUser user = await userManager.FindByIdAsync(userId);
                    if (user != null)
                    {
                        result = await userManager.RemoveFromRoleAsync(user, roleModification.RoleName);
                        //if (!result.Succeeded)
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                log.Error(ex.InnerException != null ? string.Format("Inner Exception: {0} --- Exception: {1}", ex.InnerException.Message, ex.Message) : ex.Message, ex);

            }
            return false;
        }

        public async Task<bool> Create(string name)
        {
            try
            {
                IdentityResult result = await roleManager.CreateAsync(new IdentityRole(name));
                if (result.Succeeded)
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.InnerException != null ? string.Format("Inner Exception: {0} --- Exception: {1}", ex.InnerException.Message, ex.Message) : ex.Message, ex);

            }
            return false;
        }

        public async Task<bool> Delete(string Id)
        {
            try
            {
                IdentityRole role = await roleManager.FindByIdAsync(Id);
                if (role != null)
                {
                    IdentityResult result = await roleManager.DeleteAsync(role);
                    if (result.Succeeded)
                    {
                        return true;
                    }
                    else { return false; }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.InnerException != null ? string.Format("Inner Exception: {0} --- Exception: {1}", ex.InnerException.Message, ex.Message) : ex.Message, ex);

            }
            return false;
        }
    }
}
