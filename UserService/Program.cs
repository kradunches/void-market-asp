using MongoDB.Driver;
using UserService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

var mongoConnectionString = builder.Environment.IsDevelopment()
    ? builder.Configuration["MongoDB:ConnectionString"]
    : builder.Configuration["MongoDB:DockerConnectionString"];

builder.Services.AddSingleton<IMongoClient>(sp => new MongoClient(mongoConnectionString));

builder.Services.AddTransient<UserRepository>();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddOpenApi();
    builder.Services.AddSwaggerGen();
    builder.Services.AddEndpointsApiExplorer();
}

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
//     app.MapOpenApi();
// }

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();