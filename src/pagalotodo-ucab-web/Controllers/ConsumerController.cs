using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using UCABPagaloTodoWeb.Models;

namespace UCABPagaloTodoWeb.Controllers;

public class ConsumerController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;

    public ConsumerController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }
    
    public async Task<IActionResult> Index()
    {
        try
        {
            var client = _httpClientFactory.CreateClient("PagaloTodoApi");
            var response = await client.GetAsync("/consumers");
            response.EnsureSuccessStatusCode();
            var items = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };
            IEnumerable<ConsumerModel> consumers = JsonSerializer.Deserialize<IEnumerable<ConsumerModel>>(items, options)!;
            return View(consumers);
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
    public async Task<IActionResult> Create(ConsumerRequest consumer)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("PagaloTodoApi");
            var response = await client.PostAsJsonAsync("/consumers", consumer);
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
    [Route("showConsumer/{id:Guid}", Name = "showConsumer")]
    public async Task<IActionResult> Show(Guid id)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("PagaloTodoApi");
            var response = await client.GetAsync($"/consumers/{id}");
            response.EnsureSuccessStatusCode();
            var items = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };
            ConsumerModel consumer = JsonSerializer.Deserialize<ConsumerModel>(items, options)!;
            return View(consumer);
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
            var response = await client.DeleteAsync($"/consumers/{id}");
            response.EnsureSuccessStatusCode();
            var items = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };
            Guid consumer = JsonSerializer.Deserialize<Guid>(items, options)!;
            return RedirectToAction("Index");
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine(e);
            return NotFound();
        }
    }
    
    [HttpGet]
    [Route("updateConsumer/{id:Guid}", Name = "updateConsumer")]
    public async Task<IActionResult> Update(Guid id)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("PagaloTodoApi");
            var consumersResponse = await client.GetAsync($"/consumers/{id}");
            consumersResponse.EnsureSuccessStatusCode();
            var consumerItem = await consumersResponse.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };
            ConsumerModel consumer = JsonSerializer.Deserialize<ConsumerModel>(consumerItem, options)!;
            ViewBag.Id = id;
            ConsumerRequest consumerRequest = new ConsumerRequest()
            {
                Username = consumer.Username,
                Email = consumer.Email,
                ConsumerId = consumer.ConsumerId,
                Name = consumer.Name,
                LastName = consumer.LastName,
                Status = consumer.Status
            };
            return View(consumerRequest);
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine(e);
            return NotFound();
        }
    }
    
    [HttpPost]
    [Route("putConsumer/{id:Guid}", Name = "putConsumer")]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Update(ConsumerRequest consumer, Guid id)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("PagaloTodoApi");
            // var stringContent = new StringContent(JsonSerializer.Serialize(Consumer));
            var response = await client.PutAsJsonAsync($"/consumers/{id}", consumer);
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
    //TODO: No traer todo de la bdd sino definir una ruta para obtener true/false
    public async Task<JsonResult> CheckUsername(string username)
    {
        var client = _httpClientFactory.CreateClient("PagaloTodoApi");
        var response = await client.GetAsync("/consumers");
        response.EnsureSuccessStatusCode();
        var items = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
        IEnumerable<ConsumerModel> consumers = JsonSerializer.Deserialize<IEnumerable<ConsumerModel>>(items, options)!;
        bool isUnique = true;
        foreach (ConsumerModel consumer in consumers)
        {
            if (consumer.Username == username)
            {
                isUnique = false;
                break;
            }
        }
        return Json(isUnique);
    }
}