using AutoMapper;
using log4net;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ShoesApi.DbContextFile;
using ShoesApi.DbContextFile.DBFiles;
using ShoesApi.Interfaces;
using ShoesApi.Models;
using ShoesApi.Models.ProductModel;
using System.Text.Json;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using System.Collections.Generic;
using CommonModel;

namespace ShoesApi.Repositories
{
    public class ProductRepositories : IProduct
    {
        private readonly IMapper _mapper;

        private readonly ApplicationDbContext context;
        private static readonly ILog log = LogManager.GetLogger(typeof(UserRepositories));
        private UserManager<AppUser> userManager;

        public ProductRepositories(ApplicationDbContext context, UserManager<AppUser> userManager
            , IServiceProvider serviceProvider)
        {
            _mapper = serviceProvider.GetRequiredService<IMapper>();
            this.userManager = userManager;
            this.context = context;
        }

        public async Task<CategoriesModel> Categories(string category, int pageNumber, int pageSize)
        {
            try
            {
                var MaxPageNumber = Math.Ceiling((double)(context.AddProductTable.Where(c => c.ProductCategory == category || c.ProductName == category || c.ProductType == category || c.ProductCategoryDescription == category || c.ProductCategoryType == category).Count()) /pageSize);
                //int skipping = pageNumber - 1;
                //if (MaxPageNumber > pageNumber)
                //{
                //    skipping = pageNumber;
                //}
                List<AddProductTable> product = context.AddProductTable.Where(c => c.ProductCategory == category || c.ProductName == category || c.ProductType == category || c.ProductCategoryDescription == category || c.ProductCategoryType == category)
                .OrderBy(p => p.ProductId)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

                CategoriesModel categoriesModel = new CategoriesModel();
                categoriesModel.addProductTables = product;
                categoriesModel.MaxPageSize = MaxPageNumber;
                return categoriesModel;

            }
            catch (Exception ex)
            {
                log.Error(ex.InnerException != null ? string.Format("Inner Exception: {0} --- Exception: {1}", ex.InnerException.Message, ex.Message) : ex.Message, ex);
            }
            return new CategoriesModel();
        }

        public async Task<int> GetCartCounter(string Uid)
        {
            try {
                int CartItems = context.UserCart.Where(cart => cart.UserId == Guid.Parse(Uid)).Count();
                return CartItems;
            }
            catch (Exception ex)
            {
                log.Error(ex.InnerException != null ? string.Format("Inner Exception: {0} --- Exception: {1}", ex.InnerException.Message, ex.Message) : ex.Message, ex);
            }
            return 0;
        }
        public async Task<ProductInfo> InfoById(string ProductId)
        {
            try
            {
                //QuickUrlLinks = (from c in _Context.CategoryMapping
                //                 join cat in _Context.Category on c.CategoryID equals cat.CategoryID
                //                 join user in _Context.tblUser on cat.UserID equals user.UserID
                //                 where user.UserID == CurrentAdminSession.UserID && c.isQuickLink == true
                //                 orderby c.SortOrder descending
                //                 select new QuickLinkViewModel { Name = c.Name, UrlId = c.CategoryMappingID, Url = c.CategoryURL, IsQuickLink = c.isQuickLink, sortOrder = c.SortOrder, isShared = false, CategoryId = c.CategoryID }).ToList();
                var data = context.AddProductTable.Where(c => c.ProductId.ToString() == ProductId).FirstOrDefault();

                ProductInfo Productinfo = new ProductInfo()
                {
                    ProductDetails = data,
                    images = new List<string>()
                };

                var info = (from image in context.ProductImageTable
                            where image.ProductId == data.ProductId
                            select new ProductImageTable
                            {
                                ProductImgId = image.ProductImgId,
                                ProductId = image.ProductId,
                                ImageUrl = image.ImageUrl
                            }).ToList();
                int i = 1;
                foreach (var image in info)
                {
                    if (i == 1)
                    {
                        Productinfo.ProductImgId = image.ProductImgId.ToString();
                    }
                    Productinfo.images.Add(image.ImageUrl.ToString());
                    i--;
                }

                return Productinfo;
            }
            catch (Exception ex)
            {
                log.Error(ex.InnerException != null ? string.Format("Inner Exception: {0} --- Exception: {1}", ex.InnerException.Message, ex.Message) : ex.Message, ex);
            }
            return new ProductInfo();
        }

        public async Task<IActionResult> ProductDetail(UserCart cart)
        {
            try
            {
                AppUser user = await userManager.FindByIdAsync(cart.UserId.ToString());
                if(!string.IsNullOrEmpty(user.State) && !string.IsNullOrEmpty(user.Zip_Code) 
                   && !string.IsNullOrEmpty(user.Country) && !string.IsNullOrEmpty(user.Address))
                {
                    context.UserCart.Add(cart);
                    await context.SaveChangesAsync();
                    return new StatusCodeResult(200);
                }
                else
                {
                    return new StatusCodeResult(406);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.InnerException != null ? string.Format("Inner Exception: {0} --- Exception: {1}", ex.InnerException.Message, ex.Message) : ex.Message, ex);
            }
            return new StatusCodeResult(500);
        }

        public async Task<List<AddProductTable>> functionSearchResults(string input)
        {
            try
            {
                List<AddProductTable> product = context.AddProductTable.Where(c => c.ProductCategory == input || c.ProductName == input || c.ProductType == input || c.ProductCategoryDescription == input || c.ProductCategoryType == input).ToList();
                return product;
            }
            catch (Exception ex)
            {
                log.Error(ex.InnerException != null ? string.Format("Inner Exception: {0} --- Exception: {1}", ex.InnerException.Message, ex.Message) : ex.Message, ex);
            }
            return new List<AddProductTable>();
        }
        public async Task<bool> ProductQuantity(string ProductId, string Quantity, string UserId)
        {
            try
            {
                UserCart? userCart = context.UserCart.Where(product => product.ProductId == Guid.Parse(ProductId) && product.UserId == Guid.Parse(UserId)).FirstOrDefault();
                userCart.Quantity = Quantity;
                 context.UserCart.Update(userCart);
                await context.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                log.Error(ex.InnerException != null ? string.Format("Inner Exception: {0} --- Exception: {1}", ex.InnerException.Message, ex.Message) : ex.Message, ex);
            }
            return true;
        }

        public async Task<bool> DeleteProduct(string UserCartTableId)
        {
            UserCart? userCart = context.UserCart.Where(product => product.Id == Guid.Parse(UserCartTableId)).FirstOrDefault();
            context.UserCart.Remove(userCart);
            context.SaveChanges();
            return true;
        }

        public async Task<IActionResult> AddProduct( AddProduct product)
        {
            try
            {
                // Create a new instance of Product using the data from the AddProduct object
                AddProductTable newProduct = new AddProductTable
                {
                    ProductName = product.ProductName,
                    ProductDescription = product.ProductDescription,
                    ProductType = product.ProductType,
                    ProductCategory = product.ProductCategory,
                    ProductCategoryType = product.ProductCategoryType,
                    ProductCategoryDescription = product.ProductCategoryDescription,
                    VendorEmail = product.VendorEmail,
                    Price = product.Price,
                    Small = product.Small,
                    Medium = product.Medium,
                    Large = product.Large,
                    XL = product.XL,
                    XXL = product.XXL,
                    ProductId = product.ProductId,
                    MainImage = product.ImageUrl[0] // fetch first main image
                };
                context.AddProductTable.Add(newProduct);
                foreach (var file in product.ImageUrl)
                {
                    var productImage = new ProductImageTable
                    {
                        ImageUrl = file,
                        AddProductTables = newProduct
                    };
                    context.ProductImageTable.Add(productImage);
                }
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                log.Error(ex.InnerException != null ? string.Format("Inner Exception: {0} --- Exception: {1}", ex.InnerException.Message, ex.Message) : ex.Message, ex);
            }
            return new StatusCodeResult(500);
        }

        public async Task<ApiResponseModel> UpdateProduct(AddProduct addProduct)
        {
            var response = new ApiResponseModel();
            try
            {
                var data = context.AddProductTable.Where(x => x.ProductId == addProduct.ProductId).FirstOrDefault();

                if (data != null)
                {
                    data.ProductName = addProduct.ProductName;
                    data.Price = addProduct.Price;
                    data.VendorEmail = addProduct.VendorEmail;
                    data.ProductType = addProduct.ProductType;
                    data.MainImage = addProduct.ImageUrl[0];

                    var productImageTables = context.ProductImageTable.Where(x => x.ProductId == addProduct.ProductId).ToList();
                    ImagesModel files = new ImagesModel();
                    foreach (var productImageTable in productImageTables)
                    {

                        files.Images.Add(productImageTable.ImageUrl);
                        context.ProductImageTable.Remove(productImageTable);
                    }
                    response.Data = files;
                    //context.ProductImageTable.RemoveRange(context.ProductImageTable.Where(x => x.ProductId == addProduct.ProductId).ToList());
                    foreach (var file in addProduct.ImageUrl)
                    {
                        var productImage = new ProductImageTable
                        {
                            ImageUrl = file,
                            AddProductTables = data
                        };
                        context.ProductImageTable.Add(productImage);
                        //context.ProductImageTable.Add(productImage);
                    }
                    await context.SaveChangesAsync();

                    return response;
                }
                return response;
            }
            catch (Exception ex)
            {
                log.Error(ex.InnerException != null ? string.Format("Inner Exception: {0} --- Exception: {1}", ex.InnerException.Message, ex.Message) : ex.Message, ex);
            }
            return response;
        }
        public async Task<Object> UserCartDetails(string Uid)
        {
            try
            {
                AppUser userData = await userManager.FindByIdAsync(Uid);
                UserCardModel userCardModel = new UserCardModel();
                List<ProductTable> productTables = new List<ProductTable>(); // all
                ProductTable insproductTables = new ProductTable(); // individual

                List<UserCartTable>? userCartTables = new List<UserCartTable>();

                //userCardModel.productTable.ProductImgTable 
                var options = new JsonSerializerOptions
                {
                    ReferenceHandler = ReferenceHandler.Preserve
                };

                AspUsersTable aspUsersTable = new AspUsersTable();
                userCardModel.aspUsersTable = _mapper.Map<AspUsersTable>(userData); // user data
                //var sss = context.UserCart.Where(c => c.UserId == Guid.Parse(userData.Id)).ToList();
                var data = (from ucart in context.UserCart
                            join product in context.AddProductTable on ucart.ProductId equals product.ProductId
                            //join images in context.ProductImageTable on product.ProductId equals images.ProductId
                            where ucart.UserId == Guid.Parse(Uid)
                            select new { 
                                Product = new
                                {
                                    product.ProductId,
                                    product.ProductName,
                                    product.ProductDescription,
                                    product.ProductType,
                                    product.ProductCategory,
                                    product.ProductCategoryDescription,
                                    product.ProductCategoryType,
                                    product.VendorEmail,
                                    product.MainImage,
                                    product.Price,
                                    product.Small,
                                    product.Medium,
                                    product.Large,
                                    product.XL,
                                    product.XXL,
                                    ucart.Id,
                                    ucart.Quantity
                                }, 
                                UserCart = new
                                {
                                    ucart.Id,
                                    ucart.UserId,
                                    ucart.ProductId,
                                    ucart.ProuctColor,
                                    ucart.ProductSize,
                                    ucart.ProductSum,
                                    ucart.Quantity
                                    
                                }, 
                                //Pro = new
                                //{
                                //    images.ProductId,
                                //    images.ProductImgId,
                                //    images.ImageUrl
                                //}
                            }).ToList();

                foreach ( var ucart in data )
                {
                    string con = JsonConvert.SerializeObject(ucart.Product);
                    string userCarts = JsonConvert.SerializeObject(ucart.UserCart);


                    ProductTable productTable = JsonConvert.DeserializeObject<ProductTable>(con);
                    UserCartTable userCart = JsonConvert.DeserializeObject<UserCartTable>(userCarts);
                    productTable.UserCartTableId = userCart.Id;
                    productTable.Quantity = userCart.Quantity;

                    userCartTables.Add(JsonConvert.DeserializeObject<UserCartTable>(userCarts));
                    

                    //productTable.productImgTables = new List<ProductImgTable>();

                    //userCardModel.productTable.Add(productTable);
                    insproductTables = productTable;
                    var info = (from image in context.ProductImageTable
                                where image.ProductId == Guid.Parse(productTable.ProductId)
                                select new 
                                {
                                    image.ProductId,
                                    image.ProductImgId,
                                    image.ImageUrl,
                                }).ToList();
                    foreach(var image in info)
                    {
                        string img = JsonConvert.SerializeObject(image);
                        ProductImgTable? productImageTable = JsonConvert.DeserializeObject<ProductImgTable>(img);
                        insproductTables.productImgTables.Add(productImageTable);
                    }
                    string s = "dff";
                    productTables.Add(insproductTables);
                }
                userCardModel.productTable.AddRange(productTables);
                userCardModel.userCartTables.AddRange(userCartTables);
                string jsonData = JsonConvert.SerializeObject(userCardModel);
                return jsonData;
            }
            catch (Exception ex)
            {
                log.Error(ex.InnerException != null ? string.Format("Inner Exception: {0} --- Exception: {1}", ex.InnerException.Message, ex.Message) : ex.Message, ex);
            }
            return new UserCardModel();
        }

        public async Task<Object> Checkout(string Uid)
        {
            try
            {
                AppUser userData = await userManager.FindByIdAsync(Uid);
                UserCardModel userCardModel = new UserCardModel();
                List<ProductTable> productTables = new List<ProductTable>(); // all
                ProductTable insproductTables = new ProductTable(); // individual

                List<UserCartTable>? userCartTables = new List<UserCartTable>();

                //userCardModel.productTable.ProductImgTable 
                var options = new JsonSerializerOptions
                {
                    ReferenceHandler = ReferenceHandler.Preserve
                };

                AspUsersTable aspUsersTable = new AspUsersTable();
                userCardModel.aspUsersTable = _mapper.Map<AspUsersTable>(userData); // user data
                //var sss = context.UserCart.Where(c => c.UserId == Guid.Parse(userData.Id)).ToList();
                var data = (from ucart in context.UserCart
                            join product in context.AddProductTable on ucart.ProductId equals product.ProductId
                            //join images in context.ProductImageTable on product.ProductId equals images.ProductId
                            where ucart.UserId == Guid.Parse(Uid)
                            select new
                            {
                                Product = new
                                {
                                    product.ProductId,
                                    product.ProductName,
                                    product.ProductDescription,
                                    product.ProductType,
                                    product.ProductCategory,
                                    product.ProductCategoryDescription,
                                    product.ProductCategoryType,
                                    product.VendorEmail,
                                    product.MainImage,
                                    product.Price,
                                    product.Small,
                                    product.Medium,
                                    product.Large,
                                    product.XL,
                                    product.XXL,
                                    ucart.Id,
                                    ucart.Quantity
                                },
                                UserCart = new
                                {
                                    ucart.Id,
                                    ucart.UserId,
                                    ucart.ProductId,
                                    ucart.ProuctColor,
                                    ucart.ProductSize,
                                    ucart.ProductSum,
                                    ucart.Quantity

                                },
                                //Pro = new
                                //{
                                //    images.ProductId,
                                //    images.ProductImgId,
                                //    images.ImageUrl
                                //}
                            }).ToList();

                foreach (var ucart in data)
                {
                    string con = JsonConvert.SerializeObject(ucart.Product);
                    string userCarts = JsonConvert.SerializeObject(ucart.UserCart);


                    ProductTable productTable = JsonConvert.DeserializeObject<ProductTable>(con);
                    UserCartTable userCart = JsonConvert.DeserializeObject<UserCartTable>(userCarts);
                    productTable.UserCartTableId = userCart.Id;
                    productTable.Quantity = userCart.Quantity;

                    userCartTables.Add(JsonConvert.DeserializeObject<UserCartTable>(userCarts));


                    //productTable.productImgTables = new List<ProductImgTable>();

                    //userCardModel.productTable.Add(productTable);
                    insproductTables = productTable;
                    var info = (from image in context.ProductImageTable
                                where image.ProductId == Guid.Parse(productTable.ProductId)
                                select new
                                {
                                    image.ProductId,
                                    image.ProductImgId,
                                    image.ImageUrl,
                                }).ToList();
                    foreach (var image in info)
                    {
                        string img = JsonConvert.SerializeObject(image);
                        ProductImgTable? productImageTable = JsonConvert.DeserializeObject<ProductImgTable>(img);
                        insproductTables.productImgTables.Add(productImageTable);
                    }
                    string s = "dff";
                    productTables.Add(insproductTables);
                }
                userCardModel.productTable.AddRange(productTables);
                userCardModel.userCartTables.AddRange(userCartTables);
                string jsonData = JsonConvert.SerializeObject(userCardModel);
                return jsonData;
            }
            catch (Exception ex)
            {
                log.Error(ex.InnerException != null ? string.Format("Inner Exception: {0} --- Exception: {1}", ex.InnerException.Message, ex.Message) : ex.Message, ex);
            }
            return new UserCardModel();
        }
    }
}
