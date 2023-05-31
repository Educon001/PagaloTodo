using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using UCABPagaloTodoWeb.Models;
using UCABPagaloTodoWeb.Models.CurrentUser;

namespace UCABPagaloTodoWeb.Controllers;

public class ProfileController : Controller 
{
    private readonly ILogger<ProfileController> _logger;
    private readonly IHttpClientFactory _httpClientFactory;

    public ProfileController(IHttpClientFactory httpClientFactory, ILogger<ProfileController> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    [Route("showProfile/{id:guid}", Name = "showProfile")]
    public async Task<IActionResult> ShowProfile(Guid id)
    {
        try
        {
            if (CurrentUser.GetUser().UserType == "admin")
            {
                return View(new UserModel());
            }
            var client = _httpClientFactory.CreateClient("PagaloTodoApi");
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", CurrentUser.GetUser().Token);
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };

            string ruta = "";
            
            if (CurrentUser.GetUser().UserType == "consumer")
            {
                //Usuario Consumidor
                ruta = "consumers";
            }
            else if (CurrentUser.GetUser().UserType == "provider")
            {
                ruta = "providers";
                //Usuario Proveedor
            }
            var response = await client.GetAsync($"/{ruta}/{id}");
            response.EnsureSuccessStatusCode();
            var items = await response.Content.ReadAsStringAsync();
            var user = new UserModel();
            if (CurrentUser.GetUser().UserType == "consumer")
            { 
                user = JsonSerializer.Deserialize<ConsumerModel>(items, options)!;
            }
            else if (CurrentUser.GetUser().UserType == "provider")
            {
                user = JsonSerializer.Deserialize<ProviderModel>(items, options);
            }
            
            return View(user);
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine(e);
            return NoContent();
        }
    }
    
    [Route("changePassword/{id:guid}", Name = "changePassword")]
    public async Task<IActionResult> ChangePassword(Guid id)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("PagaloTodoApi");
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", CurrentUser.GetUser().Token);
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };

            string ruta = "";
            
            if (CurrentUser.GetUser().UserType == "consumer")
            {
                //Usuario Consumidor
                ruta = "consumers";
            }
            else if (CurrentUser.GetUser().UserType == "provider")
            {
                ruta = "providers";
                //Usuario Proveedor
            }
            var response = await client.GetAsync($"/{ruta}/{id}");
            response.EnsureSuccessStatusCode();
            var items = await response.Content.ReadAsStringAsync();
            var user = new UserModel();
            if (CurrentUser.GetUser().UserType == "consumer")
            { 
                user = JsonSerializer.Deserialize<ConsumerModel>(items, options)!;
            }
            else if (CurrentUser.GetUser().UserType == "provider")
            {
                user = JsonSerializer.Deserialize<ProviderModel>(items, options);
            }
            
            return View(new UpdatePasswordRequest());
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine(e);
            return NoContent();
        }
    }
}