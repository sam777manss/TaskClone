using ShoesApi.Models;

namespace ShoesApi.Interfaces
{
    public interface IRoles
    {
        public Task<List<Roles>> Index();
        public Task<RoleEdit> Update(string Id);
        public Task<bool> Update(RoleModification roleModification);
        public Task<bool> Create(string name);
        public Task<bool> Delete(string Id);
    }
}
