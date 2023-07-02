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

    // <SUMMARY> Muestra la vista para actualizar la contraseña del administrador.
    [Route("admin/updatePassword/{token:guid?}", Name="updatePswdAdmin")]
    public IActionResult UpdatePassword()
    {
        return View();
    }
    
    // <SUMMARY> Maneja la solicitud HTTP POST para actualizar la contraseña del administrador.
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
            
            // Envía una solicitud PUT a la API para actualizar la contraseña del administrador.
            var response = await client.PutAsJsonAsync($"/admins/{id}/password", request);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsStringAsync();
            TempData["success"] = "Contraseña cambiada exitosamente";
            
            // Redirige a diferentes vistas según el tipo de usuario.
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
            ModelState.AddModelError(string.Empty, "Se ha producido un error inesperado. Por favor, inténtalo de nuevo más tarde. " + ex.Message);
        }

        return View(request);
    }
    
    // <SUMMARY> Realiza una solicitud HTTP GET para obtener la lista de proveedores.
    [HttpGet]
    private async Task<List<ProviderModel>> GetProvidersAsync()
    {
        try
        {
            var client = _httpClientFactory.CreateClient("PagaloTodoApi");
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", CurrentUser.GetUser().Token);
            
            // Envía una solicitud GET a la API para obtener la lista de proveedores.
            var response = await client.GetAsync("/providers");
            response.EnsureSuccessStatusCode();
            var items = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };
            
            // Deserializa la respuesta JSON en una lista de objetos ProviderModel.
            List<ProviderModel> providers = JsonSerializer.Deserialize<List<ProviderModel>>(items, options)!;
            return providers;
        }
        catch (Exception ex)
        {
            // Maneja cualquier excepción y la agrega al ModelState para mostrar un mensaje de error.
            ModelState.AddModelError(string.Empty, "Se ha producido un error al obtener la lista de proveedores. Por favor, inténtalo de nuevo más tarde. " + ex.Message);
            return new List<ProviderModel>();
        }
    }
    
    // <SUMMARY> Realiza una solicitud HTTP GET para obtener la lista de consumidores.
    [HttpGet]
    private async Task<List<ConsumerModel>> GetConsumersAsync()
    {
        try
        {
            var client = _httpClientFactory.CreateClient("PagaloTodoApi");
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", CurrentUser.GetUser().Token);
            
            // Envía una solicitud GET a la API para obtener la lista de consumidores.
            var response = await client.GetAsync("/consumers");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };
            
            // Deserializa la respuesta JSON en una lista de objetos ConsumerModel.
            List<ConsumerModel> consumers = JsonSerializer.Deserialize<List<ConsumerModel>>(content, options)!;
            return consumers;
        }
        catch (Exception ex)
        {
            // Maneja cualquier excepción y la agrega al ModelState para mostrar un mensaje de error.
            ModelState.AddModelError(string.Empty, "Se ha producido un error al obtener la lista de consumidores. Por favor, inténtalo de nuevo más tarde. " + ex.Message);
            return new List<ConsumerModel>();
        }
    }

    // <SUMMARY> Muestra la vista para seleccionar un informe.
    [Route("admin/reports/")]
    public async Task<IActionResult> SeleccionarReporte()
    {
        try
        {
            // Obtiene la lista de proveedores y consumidores mediante las solicitudes HTTP GET.
            List<ProviderModel> providers = await GetProvidersAsync();
            List<ConsumerModel> consumers = await GetConsumersAsync();

            // Crea un modelo de selección de informe y lo pasa a la vista.
            var model = new ReportSelectionModel
            {
                Providers = providers,
                Consumers = consumers
            };

            return View(model);
        }
        catch (Exception ex)
        {
            // Maneja cualquier excepción y muestra un mensaje de error genérico.
            ModelState.AddModelError(string.Empty, "Se ha producido un error al cargar la página de selección de informe. Por favor, inténtalo de nuevo más tarde. " + ex.Message);
            return View(new ReportSelectionModel());
        }
    }   
    
    [Route("admin/conciliation/")]
    /// <summary>
    /// Acción para mostrar la vista de "AccountingClose".
    /// </summary>
    public IActionResult AccountingClose()
    {
        var model = new AccountingCloseViewModel
        {
            SuccessMessage = TempData["success"] as string ?? string.Empty,
            ErrorMessage = TempData["error"] as string ?? string.Empty
        };

        return View(model);
    }

    /// <summary>
    /// Acción para realizar la emisión de archivos de conciliación.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AccountingClosePost()
    {
        try
        {
            var client = _httpClientFactory.CreateClient("PagaloTodoApi");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", CurrentUser.GetUser().Token);
            var id = CurrentUser.GetUser().Id;

            // Envía una solicitud GET a la API para emitir los archivos de conciliación.
            var response = await client.GetAsync($"/admins/cierre");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();

            TempData["success"] = content;
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine(e);

            TempData["error"] = "Ocurrió un error emitiendo el archivo: " + e.Message;
        }

        // Espera 5 segundos antes de redirigir al usuario.
        await Task.Delay(1500);

        // Redirige al usuario a la página de inicio.
        return RedirectToAction("Index2", "Home");
    }


}