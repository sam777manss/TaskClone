using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ShoesApi.DbContextFile;
using ShoesApi.Interfaces;
using ShoesApi.Models;
using ShoesApi.Repositories;
using AutoMapper;
using ShoesApi.Models.ProductModel;
using ShoesApi.DbContextFile.DBFiles;

var builder = WebApplication.CreateBuilder(args);

// mapper start
var mapperConfiguration = new MapperConfiguration(cfg =>
{
    cfg.CreateMap<AppUser, AspUsersTable>();
    cfg.CreateMap<ProductTable, AddProductTable>();
    cfg.CreateMap<UserCartTable, UserCart>();
});

IMapper mapper = mapperConfiguration.CreateMapper();
builder.Services.AddSingleton(mapper);
// mapper ends

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("ConnStr")));

// For Identity
builder.Services.AddIdentity<AppUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IProduct, ProductRepositories>();
builder.Services.AddScoped<IUser, UserRepositories>();
builder.Services.AddScoped<IAdmin, AdminRepositories>();
builder.Services.AddScoped<IRoles, RolesRepositories>();

builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{
    options.AddPolicy("MyAllowPolicy",
        build =>
        {
            build.WithOrigins("https://localhost:7109", "https://localhost:0000").AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

// --- set session time out starts --- //
//builder.Services.ConfigureApplicationCookie(options =>
//{
//    options.Cookie.Name = ".AspNetCore.Identity.Application";
//    options.ExpireTimeSpan = TimeSpan.FromMinutes(10);
//    options.SlidingExpiration = true;
//});


// --- set session time out Ends --- //
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseCors("MyAllowPolicy");


app.MapControllers();

app.Run();
