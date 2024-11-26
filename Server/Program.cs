using ListenerDatabase;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// ??????????? AppDbContext ?? ListenerDatabase
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer("Data Source=IHORNOTEBOOK;Initial Catalog=ListenerDatabase;Integrated Security=True;Persist Security Info=False;Pooling=False;Multiple Active Result Sets=False;Connect Timeout=60;Encrypt=False;Trust Server Certificate=False;Command Timeout=0"));

// ??????? Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Listener API",
        Version = "v1",
        Description = "API for ListenerServer"
    });
});

// ??????? ??????????
builder.Services.AddControllers();

var app = builder.Build();

// ??????? ????????????? ??? ???????????
app.MapControllers();

// ?????????????? Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Listener API v1");
        c.RoutePrefix = string.Empty; // ?????? ?? Swagger UI ?? ?????????? URL
    });
}

app.Run();
