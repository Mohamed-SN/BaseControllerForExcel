using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using ReadFromExcelSheet.BLL.Helper;
using ReadFromExcelSheet.BLL.Implementation;
using ReadFromExcelSheet.BLL.Interface;
using ReadFromExcelSheet.Configurations;
using ReadFromExcelSheet.DAL.Database;
using ReadFromExcelSheet.DAL.Extends;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddAutoMapper(typeof(DomainProfile).Assembly);
ExcelPackage.LicenseContext = LicenseContext.NonCommercial;




builder.Services.AddLocalizationConfig();
builder.Services.AddScoped(typeof(IBaseRepo<BaseEntity<int>,EntitySC,int>), typeof(BaseRepo<BaseEntity<int>, EntitySC, int>));





builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseLazyLoadingProxies().UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseStaticFiles();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
