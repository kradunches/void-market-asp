using MassTransit;
using OrderService.Consumers;

namespace OrderService.Extensions;

public static class RabbitMqExtensions
{
    public static IServiceCollection AddRabbitMq(this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
    {
        services.AddMassTransit(x =>
        {
            x.AddConsumer<OrderStatusUpdatedConsumer>();

            x.UsingRabbitMq((context, cfg) =>
            {
                var host = environment.IsDevelopment()
                    ? configuration["RabbitMq:LocalHost"]
                    : configuration["RabbitMq:DockerHost"];
                cfg.Host(host, "/", h =>
                {
                    h.Username(configuration["RabbitMq:Username"]);
                    h.Password(configuration["RabbitMq:Password"]);
                });

                // Настройка очереди order-status
                cfg.ReceiveEndpoint("order-status", e =>
                {
                    e.ConfigureConsumer<OrderStatusUpdatedConsumer>(context);
                });
            });
        });
        
        return services;
    }
}