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

                ushort port = environment.IsDevelopment()
                    ? Convert.ToUInt16(configuration["RabbitMq:LocalPort"])
                    : Convert.ToUInt16(configuration["RabbitMq:DockerPort"]);

                cfg.Host(host, port, "/", h =>
                {
                    h.Username(configuration["RabbitMq:Username"]);
                    h.Password(configuration["RabbitMq:Password"]);
                });

                cfg.ReceiveEndpoint(configuration["RabbitMq:QUEUE"], e =>
                {
                    e.ConfigureConsumer<OrderStatusUpdatedConsumer>(context);
                });
            });
        });

        return services;
    }
}