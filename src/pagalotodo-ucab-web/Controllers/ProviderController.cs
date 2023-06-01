using System.Net;
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
            TempData["success"] = "Provider Created Successfully";
            return RedirectToAction("Index");
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine(e);
            TempData["error"] = "There was an error creating the service";
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
            TempData["success"] = "Provider Deleted Successfully";
            return RedirectToAction("Index");
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine(e);
            TempData["error"] = "There was an error deleting the provider";
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
            TempData["success"] = "Provider Updated Successfully";
            if (CurrentUser.GetUser().UserType == "provider")
            {
                return RedirectToAction("Index2", "Home");
            }
            return RedirectToAction("Index");
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine(e);
            TempData["error"] = "There was an error updating the provider";
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
            if (provider.Username == username && provider.Id != CurrentUser.GetUser().Id)
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

    [Route("provider/updatePassword/{token:guid?}")]
    public IActionResult UpdatePassword()
    {
        return View();
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("provider/updatePswd/{token:guid?}", Name="updatePswdPostProvider")]
    public async Task<IActionResult> UpdatePassword(UpdatePasswordRequest request)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("PagaloTodoApi");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", CurrentUser.GetUser().Token);
            var id = CurrentUser.GetUser().Id;
            var response = await client.PutAsJsonAsync($"/providers/{id}/password", request);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsStringAsync();
            if (CurrentUser.GetUser().UserType == "provider")
            {
                return RedirectToAction("Index2", "Home");
            }
            return RedirectToAction("Index");
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.BadRequest)
        {
            ModelState.AddModelError(string.Empty, "La contraseña no cumple con los requisitos. Por favor, inténtalo de nuevo.");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, "Se ha producido un error inesperado. Por favor, inténtalo de nuevo más tarde.");
        }
        return View(request);
    }
    
    //TODO: Lista de confirmacion
}