namespace PaymentService.Controller;

public interface IOrderServiceClient
{
    Task<bool> OrderExistsAsync(int orderId);
}

public class OrderServiceClient : IOrderServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<OrderServiceClient> _logger;
    
    public OrderServiceClient(HttpClient httpClient, ILogger<OrderServiceClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<bool> OrderExistsAsync(int orderId)
    {
        var url = $"api/orders/GetOrder/{orderId}";
        _logger.LogInformation("Проверяем заказ: {BaseAddress}{Url}", _httpClient.BaseAddress, url);

        var response = await _httpClient.GetAsync(url);

        var content = await response.Content.ReadAsStringAsync();
        _logger.LogInformation("Ответ OrderService: StatusCode={StatusCode}, Content={Content}",
            response.StatusCode, content);

        return response.IsSuccessStatusCode;
    }
}