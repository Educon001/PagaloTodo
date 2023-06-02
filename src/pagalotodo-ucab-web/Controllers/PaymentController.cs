using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using UCABPagaloTodoWeb.Models;
using UCABPagaloTodoWeb.Models.CurrentUser;

namespace UCABPagaloTodoWeb.Controllers;

public class PaymentController : Controller
{
    private readonly ILogger<PaymentController> _logger;
    private readonly IHttpClientFactory _httpClientFactory;

    public PaymentController(IHttpClientFactory httpClientFactory, ILogger<PaymentController> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }
    
    [Route("/admin/payments", Name="paymentsIndex")]
    public async Task<IActionResult> Index()
    {
        try
        {
            var client = _httpClientFactory.CreateClient("PagaloTodoApi");
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", CurrentUser.GetUser().Token);
            var response = await client.GetAsync("/payments");
            response.EnsureSuccessStatusCode();
            var items = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };
            IEnumerable<PaymentModel> payments = JsonSerializer.Deserialize<IEnumerable<PaymentModel>>(items, options)!;
            return View(payments);
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine(e);
            TempData["error"] = "Error getting payments. Try again.";
            return NotFound();
        }
    }
}