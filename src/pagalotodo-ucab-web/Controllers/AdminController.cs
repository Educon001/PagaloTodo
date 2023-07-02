using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using UCABPagaloTodoWeb.Models;
using UCABPagaloTodoWeb.Models.CurrentUser;

namespace UCABPagaloTodoWeb.Controllers;

public class AdminController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;

    public AdminController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    [Route("admin/updatePassword/{token:guid?}", Name="updatePswdAdmin")]
    public IActionResult UpdatePassword()
    {
        return View();
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("admin/updatePswd", Name="updatePswdPostAdmin")]
    public async Task<IActionResult> UpdatePassword(UpdatePasswordRequest request)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("PagaloTodoApi");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", CurrentUser.GetUser().Token);
            var id = CurrentUser.GetUser().Id;
            var response = await client.PutAsJsonAsync($"/admins/{id}/password", request);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsStringAsync();
            TempData["success"] = "Password changed successfully";
            if (CurrentUser.GetUser().UserType == "admin")
            {
                return RedirectToAction("Index2", "Home");
            }
            return RedirectToAction("Index", "Home");
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.BadRequest)
        {
            ModelState.AddModelError(string.Empty, "La contraseña no cumple con los requisitos. Por favor, inténtalo de nuevo.");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, "Se ha producido un error inesperado. Por favor, inténtalo de nuevo más tarde."+ex.Message);
        }

        return View(request);
    }
    
    [HttpGet]
    private async Task<List<ProviderModel>> GetProvidersAsync()
    {
        var client = _httpClientFactory.CreateClient("PagaloTodoApi");
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", CurrentUser.GetUser().Token);
        var response = await client.GetAsync("/providers");
        response.EnsureSuccessStatusCode();
        var items = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
        List<ProviderModel> providers = JsonSerializer.Deserialize<List<ProviderModel>>(items, options)!;
        return providers;
    }
    
    [HttpGet]
    private async Task<List<ConsumerModel>> GetConsumersAsync()
    {
        var client = _httpClientFactory.CreateClient("PagaloTodoApi");
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", CurrentUser.GetUser().Token);
        var response = await client.GetAsync("/consumers");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
        List<ConsumerModel> consumers = JsonSerializer.Deserialize<List<ConsumerModel>>(content, options)!;
        return consumers;
    }

    
    [Route("admin/reports/")]
    public async Task<IActionResult> SeleccionarReporte()
    {
        List<ProviderModel> providers = await GetProvidersAsync();
        List<ConsumerModel> consumers = await GetConsumersAsync();

        var model = new ReportSelectionModel
        {
            Providers = providers,
            Consumers = consumers
        };

        return View(model);
    }
    
}