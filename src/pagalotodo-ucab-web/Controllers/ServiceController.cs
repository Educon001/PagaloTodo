using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using UCABPagaloTodoMS.Core.Enums;
using UCABPagaloTodoWeb.Models;

namespace UCABPagaloTodoWeb.Controllers;

public class ServiceController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;

    public ServiceController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }
    
    public async Task<IActionResult> Index()
    {
        try
        {
            var client = _httpClientFactory.CreateClient("PagaloTodoApi");
            var response = await client.GetAsync("/services");
            response.EnsureSuccessStatusCode();
            var items = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };
            IEnumerable<ServiceModel> services = JsonSerializer.Deserialize<IEnumerable<ServiceModel>>(items, options)!;
            return View(services);
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine(e);
            return NotFound();
        }
    }

    public async Task<IActionResult> Create()
    {
        try
        {
            var client = _httpClientFactory.CreateClient("PagaloTodoApi");
            var response = await client.GetAsync("/providers");
            response.EnsureSuccessStatusCode();
            var items = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };
            IEnumerable<ProviderModel> providers = JsonSerializer.Deserialize<IEnumerable<ProviderModel>>(items, options)!;
            ViewBag.Message = providers;
            return View();
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine(e);
            return NotFound();
        }
    }
    
    [ValidateAntiForgeryToken]
    [HttpPost]
    public async Task<IActionResult> Create(ServiceRequest service)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("PagaloTodoApi");
            var response = await client.PostAsJsonAsync("/services", service);
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
    [Route("show/{id:Guid}", Name = "show")]
    // [HttpGet("{id:Guid}")]
    public async Task<IActionResult> Show(Guid id)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("PagaloTodoApi");
            var response = await client.GetAsync($"/services/{id}");
            response.EnsureSuccessStatusCode();
            var items = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };
            ServiceModel service = JsonSerializer.Deserialize<ServiceModel>(items, options)!;
            return View(service);
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
            var response = await client.DeleteAsync($"/services/{id}");
            response.EnsureSuccessStatusCode();
            var items = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };
            Guid service = JsonSerializer.Deserialize<Guid>(items, options)!;
            return RedirectToAction("Index");
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine(e);
            return NotFound();
        }
    }
    
    [HttpGet]
    [Route("update/{id:Guid}", Name = "update")]
    public async Task<IActionResult> Update(Guid id)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("PagaloTodoApi");
            var providersResponse = await client.GetAsync("/providers");
            providersResponse.EnsureSuccessStatusCode();
            var providersItems = await providersResponse.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };
            IEnumerable<ProviderModel> providers = JsonSerializer.Deserialize<IEnumerable<ProviderModel>>(providersItems, options)!;
            ViewBag.Message = providers;
            var servicesResponse = await client.GetAsync($"/services/{id}");
            servicesResponse.EnsureSuccessStatusCode();
            var serviceItem = await servicesResponse.Content.ReadAsStringAsync();
            ServiceModel service = JsonSerializer.Deserialize<ServiceModel>(serviceItem, options)!;
            ViewBag.Id = id;
            ServiceRequest serviceRequest = new ServiceRequest()
            {
                Name = service.Name,
                Description = service.Description,
                ServiceStatus = (ServiceStatusEnum)Enum.Parse(typeof(ServiceStatusEnum), service.ServiceStatus!),
                ServiceType = (ServiceTypeEnum)Enum.Parse(typeof(ServiceTypeEnum), service.ServiceType!),
                Provider = service.Provider!.Id
            };
            return View(serviceRequest);
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine(e);
            return NotFound();
        }
    }
    
    [HttpPost]
    [Route("update/{id:Guid}", Name = "put")]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Update(ServiceRequest service, Guid id)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("PagaloTodoApi");
            // var stringContent = new StringContent(JsonSerializer.Serialize(service));
            var response = await client.PutAsJsonAsync($"/services/{id}", service);
            var result = await response.Content.ReadAsStringAsync();
            return RedirectToAction("Index");
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine(e);
            return NotFound();
        }

    }
}