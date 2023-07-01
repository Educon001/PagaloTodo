using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;
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
    
    [Route("admin/reports/")]
    public IActionResult Reporte1()
    {
        return View();
    }

    [HttpPost]
    [Route("admin/reports/generarreporte")]
    public async Task<IActionResult> GenerarReporte(string consumerId, string startDate, string endDate)
    {
        try
        {
            // Construir la URL para llamar al servidor de informes
            var url = $"http://localhost/Reports/report/pagalotodo/PagosConsumidorTiempo?ConsumerId={consumerId}&StartDate={startDate}&EndDate={endDate}&rs:Format=PDF";

            // Crear un cliente HTTP
            using (var httpClient = new HttpClient())
            {
                // Descargar el contenido del archivo PDF en un MemoryStream
                var response = await httpClient.GetAsync(url);
                var content = await response.Content.ReadAsByteArrayAsync();
                var stream = new MemoryStream(content);

                // Devolver el archivo PDF como un archivo para descargar
                return File(stream, "application/pdf", "report.pdf");
            }
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, "Se ha producido un error inesperado. Por favor, inténtalo de nuevo más tarde." + ex.Message);
        }

        return View("Reporte1");
    }
}