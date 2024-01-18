namespace Client.Models
{
    public class UserCartDetails
    {

        public AspUsersTable? aspUsersTable;
        public List<ProductTable>? productTable { get; set; }
        //public List<ProductImgTable>? productImageTables { get; set; }
        public List<UserCartTable>? userCartTables { get; set; }

    }
}

