using ShoesApi.DbContextFile.DBFiles;

namespace ShoesApi.Models
{
    public class CategoriesModel
    {
        public IEnumerable<AddProductTable>? addProductTables { get; set; }
        public double MaxPageSize { get; set; }
    }
}
