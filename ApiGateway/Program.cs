using ApiGateway.Services;

var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();

// HttpClients
builder.Services.AddHttpClient("orders", c =>
    c.BaseAddress = new Uri(builder.Configuration["Services:OrderBaseAddress"]!));
builder.Services.AddHttpClient("users", c =>
    c.BaseAddress = new Uri(builder.Configuration["Services:UserBaseAddress"]!));
builder.Services.AddHttpClient("payments", c =>
    c.BaseAddress = new Uri("http://asp.payment-svc:80"));


// Services
builder.Services.AddScoped<IOrderAggregateService, OrderAggregateService>();

// Swagger для dev
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
}

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run("http://0.0.0.0:80");
