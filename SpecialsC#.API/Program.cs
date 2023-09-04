var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Ensure default route (i.e. homepage) returns static text.
app.MapGet("/", async context =>
{
  await context.Response.WriteAsync("Future port of Grocery Specials web app to .NET based tech stack. See https://github.com/cagross/csharp-specials. See working Swagger page at https://specialscapi20230823213803.azurewebsites.net/swagger/index.html");
});

app.Run();
