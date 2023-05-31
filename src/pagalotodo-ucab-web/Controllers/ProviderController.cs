using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using UCABPagaloTodoWeb.Models;
using UCABPagaloTodoWeb.Models.CurrentUser;

namespace UCABPagaloTodoWeb.Controllers;

public class ProviderController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;

    public ProviderController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }
    
    public async Task<IActionResult> Index()
    {
        try
        {
            var client = _httpClientFactory.CreateClient("PagaloTodoApi");
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", CurrentUser.GetUser().Token);
            var response = await client.GetAsync("/providers");
            response.EnsureSuccessStatusCode();
            var items = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };
            IEnumerable<ProviderModel> providers = JsonSerializer.Deserialize<IEnumerable<ProviderModel>>(items, options)!;
            return View(providers);
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine(e);
            return NotFound();
        }
    }

    public IActionResult Create()
    {
        return View();
    }
    
    [ValidateAntiForgeryToken]
    [HttpPost]
    public async Task<IActionResult> Create(ProviderRequest provider)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("PagaloTodoApi");
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", CurrentUser.GetUser().Token);
            var response = await client.PostAsJsonAsync("/providers", provider);
            var result = await response.Content.ReadAsStringAsync();
            return RedirectToAction("Index");
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine(e);
            return NotFound();
        }
    }

    [HttpGet]
    [Route("showProvider/{id:Guid}", Name = "showProvider")]
    public async Task<IActionResult> Show(Guid id)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("PagaloTodoApi");
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", CurrentUser.GetUser().Token);
            var response = await client.GetAsync($"/providers/{id}");
            response.EnsureSuccessStatusCode();
            var items = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };
            ProviderModel provider = JsonSerializer.Deserialize<ProviderModel>(items, options)!;
            return View(provider);
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine(e);
            return NotFound();
        }
    }
    
    public async Task<ActionResult> Delete(Guid id)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("PagaloTodoApi");
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", CurrentUser.GetUser().Token);
            var response = await client.DeleteAsync($"/providers/{id}");
            response.EnsureSuccessStatusCode();
            var items = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };
            Guid provider = JsonSerializer.Deserialize<Guid>(items, options)!;
            return RedirectToAction("Index");
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine(e);
            return NotFound();
        }
    }
    
    [HttpGet]
    [Route("updateProvider/{id:Guid}", Name = "updateProvider")]
    public async Task<IActionResult> Update(Guid id)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("PagaloTodoApi");
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", CurrentUser.GetUser().Token);
            var providersResponse = await client.GetAsync($"/providers/{id}");
            providersResponse.EnsureSuccessStatusCode();
            var providerItem = await providersResponse.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };
            ProviderModel provider = JsonSerializer.Deserialize<ProviderModel>(providerItem, options)!;
            ViewBag.Id = id;
            ProviderRequest providerRequest = new ProviderRequest()
            {
                Username = provider.Username,
                Email = provider.Email,
                Name = provider.Name,
                Status = provider.Status,
                Rif = provider.Rif,
                AccountNumber = provider.AccountNumber
            };
            return View(providerRequest);
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine(e);
            return NotFound();
        }
    }
    
    [HttpPost]
    [Route("putProvider/{id:Guid}", Name = "putProvider")]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Update(ProviderRequest provider, Guid id)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("PagaloTodoApi");
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", CurrentUser.GetUser().Token);
            // var stringContent = new StringContent(JsonSerializer.Serialize(Provider));
            var response = await client.PutAsJsonAsync($"/providers/{id}", provider);
            var result = await response.Content.ReadAsStringAsync();
            if (CurrentUser.GetUser().UserType == "provider")
            {
                return RedirectToAction("Index2", "Home");
            }
            return RedirectToAction("Index");
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine(e);
            return NotFound();
        }
    }

    [HttpGet]
    //TODO: No traer todo de la bdd sino definir una ruta para obtener true/false
    //TODO: Email check
    public async Task<JsonResult> CheckUsername(string username)
    {
        var client = _httpClientFactory.CreateClient("PagaloTodoApi");
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", CurrentUser.GetUser().Token);
        var response = await client.GetAsync("/providers");
        response.EnsureSuccessStatusCode();
        var items = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
        IEnumerable<ProviderModel> providers = JsonSerializer.Deserialize<IEnumerable<ProviderModel>>(items, options)!;
        bool isUnique = true;
        foreach (ProviderModel provider in providers)
        {
            if (provider.Username == username)
            {
                isUnique = false;
                break;
            }
        }
        return Json(isUnique);
    }
    
    [Route("/paymentsProvider/{id:Guid}", Name = "paymentsProvider")]
    public async Task<IActionResult> GetPayments(Guid id)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("PagaloTodoApi");
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", CurrentUser.GetUser().Token);
            var response = await client.GetAsync($"/payments/provider/{id}");
            response.EnsureSuccessStatusCode();
            var items = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions()
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
            return NotFound();
        }
    }

    //TODO: Lista de confirmacion
}