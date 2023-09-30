using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors();

var app = builder.Build();



app.UseCors(x=>x.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod());
var dir = Path.Combine(Directory.GetCurrentDirectory(), "Images");
if (!Directory.Exists(dir))
{
    Directory.CreateDirectory(dir);
}
app.UseStaticFiles(new StaticFileOptions {FileProvider=new PhysicalFileProvider(dir),RequestPath="/images" });





// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
