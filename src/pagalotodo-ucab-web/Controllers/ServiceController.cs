using System.Security.Cryptography;
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
            IEnumerable<ProviderModel> providers =
                JsonSerializer.Deserialize<IEnumerable<ProviderModel>>(items, options)!;
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
            Guid id = JsonSerializer.Deserialize<Guid>(result);
            return RedirectToRoute("CreateFormat", new { id });
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine(e);
            return NotFound();
        }
    }

    [HttpGet]
    [Route("showService/{id:Guid}", Name = "showService")]
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
    [Route("updateService/{id:Guid}", Name = "updateService")]
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
            IEnumerable<ProviderModel> providers =
                JsonSerializer.Deserialize<IEnumerable<ProviderModel>>(providersItems, options)!;
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
    [Route("putService/{id:Guid}", Name = "putService")]
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

    [Route("/{id:Guid}", Name="createFormat")]
    public IActionResult CreateFormat(Guid id)
    {
        ViewBag.ServiceId = id;
        return View();
    }
    
    [ValidateAntiForgeryToken]
    [HttpPost]
    [Route("/{id:Guid}", Name="createFormatR")]
    public async Task<IActionResult> CreateFormat(List<FieldRequest> fieldRequests, Guid id)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("PagaloTodoApi");
            foreach (FieldRequest field in fieldRequests)
            {
                field.Service = id;
            }
            var response = await client.PostAsJsonAsync("/services/format", fieldRequests);
            var result = await response.Content.ReadAsStringAsync();
            // List<Guid> id = JsonSerializer.Deserialize<List<Guid>>(result)!;
            return RedirectToAction("Index");
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine(e);
            return NotFound();
        }
        // try
        // {
        //     var client = _httpClientFactory.CreateClient("PagaloTodoApi");
        //     var response = await client.PostAsJsonAsync("/services", service);
        //     var result = await response.Content.ReadAsStringAsync();
        //     Guid id = JsonSerializer.Deserialize<Guid>(result);
        //     return RedirectToAction("CreateFormat", id);
        // }
        // catch (HttpRequestException e)
        // {
        //     Console.WriteLine(e);
        //     return NotFound();
        // }
    }
    
    // [HttpGet]
    // [Route("createFormat/{id:Guid}", Name = "createFormat")]
    // public async Task<IActionResult> ConciliationFormat(List<FieldRequest> fieldRequests)
    // public IActionResult ConciliationFormat(List<FieldRequest> fieldRequests)
    // {
        // RedirectToAction("Index");
        // return View();
        // try
        // {
        //     var client = _httpClientFactory.CreateClient("PagaloTodoApi");
        //     var providersResponse = await client.GetAsync("/providers");
        //     providersResponse.EnsureSuccessStatusCode();
        //     var providersItems = await providersResponse.Content.ReadAsStringAsync();
        //     var options = new JsonSerializerOptions()
        //     {
        //         PropertyNameCaseInsensitive = true,
        //         DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        //     };
        //     IEnumerable<ProviderModel> providers = JsonSerializer.Deserialize<IEnumerable<ProviderModel>>(providersItems, options)!;
        //     ViewBag.Message = providers;
        //     var servicesResponse = await client.GetAsync($"/services/{id}");
        //     servicesResponse.EnsureSuccessStatusCode();
        //     var serviceItem = await servicesResponse.Content.ReadAsStringAsync();
        //     ServiceModel service = JsonSerializer.Deserialize<ServiceModel>(serviceItem, options)!;
        //     ViewBag.Id = id;
        //     ServiceRequest serviceRequest = new ServiceRequest()
        //     {
        //         Name = service.Name,
        //         Description = service.Description,
        //         ServiceStatus = (ServiceStatusEnum)Enum.Parse(typeof(ServiceStatusEnum), service.ServiceStatus!),
        //         ServiceType = (ServiceTypeEnum)Enum.Parse(typeof(ServiceTypeEnum), service.ServiceType!),
        //         Provider = service.Provider!.Id
        //     };
        //     return View(serviceRequest);
        // }
        // catch (HttpRequestException e)
        // {
        //     Console.WriteLine(e);
        //     return NotFound();
        // }
    // }
    
    [Route("updateField/{serviceId:guid}/{id:guid}", Name = "updateField")]
    public async Task<IActionResult> UpdateField(Guid id, Guid serviceId)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("PagaloTodoApi");
            var fieldResponse = await client.GetAsync($"/services/fields/{id}");
            fieldResponse.EnsureSuccessStatusCode();
            var field = await fieldResponse.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };
            FieldModel fieldFinal =
                JsonSerializer.Deserialize<FieldModel>(field, options)!;
            ViewBag.Message = fieldFinal;
            ViewBag.Id = id;
            ViewBag.serviceId = serviceId;
            FieldRequest fieldRequest = new FieldRequest
            {
                Name = fieldFinal.Name,
                Format = fieldFinal.Format,
                Length = fieldFinal.Length,
                AttrReference = fieldFinal.AttrReference,
                // Service = serviceId
            };
            return View(fieldRequest);
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine(e);
            return NotFound();
        }
    }
    
    [HttpPost]
    [Route("putField/{serviceId:Guid}/{id:Guid}", Name = "putField")]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> UpdateField(FieldRequest field, Guid id, Guid serviceId)
    {
        try
        {
            field.Service = serviceId;
            var client = _httpClientFactory.CreateClient("PagaloTodoApi");
            var response = await client.PutAsJsonAsync($"/services/fields/{id}", field);
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