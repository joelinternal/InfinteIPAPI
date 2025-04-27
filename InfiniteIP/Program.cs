using InfiniteIP.DbUtils;
using InfiniteIP.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<InfiniteContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddScoped<IGmSheet, GmSheetServices>();
string[] orgins = { "http://localhost:3000", "https://3n7r3kwf-3000.inc1.devtunnels.ms" };
builder.Services.AddCors(o => o.AddPolicy("alloworgin", builder =>
{
    builder.WithOrigins(orgins)
    .AllowAnyHeader().AllowAnyMethod();
}));
var app = builder.Build();
app.UseCors("alloworgin");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("AllowReactApp");

app.UseAuthorization();

app.MapControllers();

app.Run();
