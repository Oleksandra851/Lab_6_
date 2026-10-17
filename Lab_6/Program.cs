using Lab_6.Data;
using Lab_6.Hubs;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Реєстрація DbContext у контейнері залежностей
builder.Services.AddDbContext<UniversityDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("UniversityDb")));

// 2. Реєстрація Razor Pages та сервісу SignalR
builder.Services.AddRazorPages();
builder.Services.AddSignalR();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

// 3. Мапінг сторінок та конекшн-ендпоінту SignalR
app.MapRazorPages();
app.MapHub<TeacherHub>("/teacherHub");

app.Run();