using MassTransit;
using Microsoft.Extensions.Options;
using PaymentService.Controller;
using PaymentService.Extensions;
using PaymentService.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddSwaggerGen();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers();
builder.Services.AddRabbitMq(builder.Configuration);

builder.Services.Configure<ServiceUrlsOptions>(builder.Configuration.GetSection("ServiceUrls"));

builder.Services.AddHttpClient<IOrderServiceClient, OrderServiceClient>((serviceProvider, client) =>
{
    var options = serviceProvider.GetRequiredService<IOptions<ServiceUrlsOptions>>();
    client.BaseAddress = new Uri(options.Value.OrderService);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
//     app.MapOpenApi();
// }

app.MapOpenApi();
app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();

app.UseHttpsRedirection();

app.Run();