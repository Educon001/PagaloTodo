using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using UCABPagaloTodoWeb.Models;
using UCABPagaloTodoWeb.Models.CurrentUser;

namespace UCABPagaloTodoWeb.Controllers;

public class LoginController : Controller
{
    private readonly ILogger<LoginController> _logger;
    private readonly IHttpClientFactory _httpClientFactory;

    public LoginController(IHttpClientFactory httpClientFactory, ILogger<LoginController> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public IActionResult Consumer()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Consumer(LoginRequest request)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("PagaloTodoApi");
            var response = await client.PostAsJsonAsync("/api/login", request);
            response.EnsureSuccessStatusCode();
            //return RedirectToAction("Index2", "Home");
            var items = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };
            LoginModel login = JsonSerializer.Deserialize<LoginModel>(items, options)!;
            CurrentUser.SetUser(login);
            var aa = CurrentUser.GetUser();
            return RedirectToAction("Index2", "Home");
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.BadRequest)
        {
            _logger.LogError("Se ha producido un error en la solicitud. Exception: " + ex);
            ViewBag.Error = "Las credenciales de inicio de sesión son inválidas. Por favor, inténtalo de nuevo.";
        }
        catch (Exception ex)
        {
            _logger.LogError("Se ha producido un error inesperado. Exception: " + ex);
            ViewBag.Error = "Se ha producido un error inesperado. Por favor, inténtalo de nuevo más tarde.";
        }

        return View();
    }

    public IActionResult Provider()
    {
        return View();
    }
    
    [HttpPost]
    public async Task<IActionResult> Provider(LoginRequest request)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("PagaloTodoApi");
            var response = await client.PostAsJsonAsync("/api/login", request);
            response.EnsureSuccessStatusCode();
            //return RedirectToAction("Index2", "Home");
            var items = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };
            LoginModel login = JsonSerializer.Deserialize<LoginModel>(items, options)!;
            CurrentUser.SetUser(login);
            return RedirectToAction("Index2", "Home");
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.BadRequest)
        {
            _logger.LogError("Se ha producido un error en la solicitud. Exception: " + ex);
            ViewBag.Error = "Las credenciales de inicio de sesión son inválidas. Por favor, inténtalo de nuevo.";
        }
        catch (Exception ex)
        {
            _logger.LogError("Se ha producido un error inesperado. Exception: " + ex);
            ViewBag.Error = "Se ha producido un error inesperado. Por favor, inténtalo de nuevo más tarde.";
        }

        return View();
    }
    
    public IActionResult Admin()
    {
        return View();
    }
    
    [HttpPost]
    public async Task<IActionResult> Admin(LoginRequest request)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("PagaloTodoApi");
            var response = await client.PostAsJsonAsync("/api/login", request);
            response.EnsureSuccessStatusCode();
            //return RedirectToAction("Index2", "Home");
            var items = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };
            LoginModel login = JsonSerializer.Deserialize<LoginModel>(items, options)!;
            CurrentUser.SetUser(login);
            return RedirectToAction("Index2", "Home");
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.BadRequest)
        {
            _logger.LogError("Se ha producido un error en la solicitud. Exception: " + ex);
            ViewBag.Error = "Las credenciales de inicio de sesión son inválidas. Por favor, inténtalo de nuevo.";
        }
        catch (Exception ex)
        {
            _logger.LogError("Se ha producido un error inesperado. Exception: " + ex);
            ViewBag.Error = "Se ha producido un error inesperado. Por favor, inténtalo de nuevo más tarde.";
        }

        return View();
    }
    
    public IActionResult ForgotPassword()
    {
        return View();
    }
    
    [ValidateAntiForgeryToken]
    [HttpPost]
    public async Task<IActionResult> ForgotPassword(string request)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("PagaloTodoApi");
            var response = await client.PostAsJsonAsync("/api/login/forgotpassword", request);
            var result = await response.Content.ReadAsStringAsync();
            return RedirectToAction("Index2", "Home");
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine(e);
            return NotFound();
        }
    }
    
}