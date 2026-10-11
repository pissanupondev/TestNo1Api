using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Add Database Context
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Register Business Logic Services
builder.Services.AddScoped<IUserService, UserService>();

// --- เพิ่มบริการ Swagger Gen ---
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 3. Add SignalR and CORS
builder.Services.AddSignalR();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials()
              .SetIsOriginAllowed(_ => true));
});

var app = builder.Build();

// --- เปิดใช้งาน Swagger Middleware ---
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "User API v1");
    c.RoutePrefix = string.Empty; // ตั้งค่าให้หน้า Swagger เปิดขึ้นมาทันทีเมื่อเข้า Root URL (เช่น http://localhost:5108/)
});

// 4. Configure Middleware Pipeline
app.UseCors("AllowAll");
app.UseStaticFiles();
app.MapHub<NotificationHub>("/notificationHub");
app.MapUserEndpoints();

app.Run();