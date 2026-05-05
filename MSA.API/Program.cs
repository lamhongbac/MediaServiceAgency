using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.FileProviders;
using MSA.BLL;
using MSA.DAL.Repos;
using System.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IDbConnection>(sp =>
    new SqlConnection(builder.Configuration.GetConnectionString("MSADbConnection")));
builder.Services.AddScoped<IAppRepository, AppRepositoryDapper>();
builder.Services.AddScoped<IMediaBusiness, MediaBusiness>();
var app = builder.Build();


var provider = new FileExtensionContentTypeProvider();
// Chỉ cho phép các định dạng an toàn
provider.Mappings[".exe"] = "application/x-msdownload-blocked"; // Chặn thực thi

var storagePath = builder.Configuration["MediaConfig:StorageRoot"];

// Đảm bảo thư mục tồn tại để tránh lỗi khởi động
if (!Directory.Exists(storagePath)) Directory.CreateDirectory(storagePath);

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(storagePath),
    RequestPath = "/cdn", // URL sẽ có dạng: domain.com/cdn/appcode/...

    OnPrepareResponse = ctx =>
    {
        // 1. Bảo mật: Chống trình duyệt tự ý thực thi file sai định dạng
        ctx.Context.Response.Headers.Append("X-Content-Type-Options", "nosniff");

        // 2. Tối ưu: Cấu hình Cache cho trình duyệt (Ví dụ: 1 năm)
        // Giúp "CDN nội bộ" của bạn chạy cực nhanh cho người dùng cuối
        ctx.Context.Response.Headers.Append("Cache-Control", "public, max-age=31536000");

        // 3. Tùy chọn: Cho phép gọi ảnh từ các domain khác (CORS)
        ctx.Context.Response.Headers.Append("Access-Control-Allow-Origin", "*");
    }
});

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
