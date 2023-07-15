using System.Net;
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

    /// <summary>
    /// Autentica un usuario mediante una solicitud de inicio de sesión.
    /// </summary>
    /// <param name="request">La solicitud de inicio de sesión del usuario.</param>
    /// <param name="userType">El tipo de usuario que está iniciando sesión.</param>
    /// <returns>El modelo de inicio de sesión del usuario autenticado, o null si no se pudo autenticar.</returns>
    private async Task<LoginModel?> AuthenticateUserAsync(LoginRequest request, string userType)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("PagaloTodoApi");
            var response = await client.PostAsJsonAsync("/api/login", request);
            response.EnsureSuccessStatusCode();
            var items = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };
            LoginModel login = JsonSerializer.Deserialize<LoginModel>(items, options)!;
            CurrentUser.SetUser(login);
            return login;
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.BadRequest)
        {
            _logger.LogError($"Se ha producido un error en la solicitud de inicio de sesión de {userType}. Exception: {ex}");
            ViewBag.Error = "Las credenciales de inicio de sesión son inválidas. Por favor, inténtalo de nuevo.";
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.Unauthorized)
        {
            _logger.LogError($"La cuenta del usuario {userType} está inactiva. Exception: {ex}");
            ViewBag.Error = "La cuenta del usuario está inactiva. Por favor, contacta con el administrador.";
        }
        catch (Exception ex)
        {
            _logger.LogError($"Se ha producido un error inesperado durante el inicio de sesión de {userType}. Exception: {ex}");
            ViewBag.Error = "Ha ocurrido un error inesperado. Por favor, inténtalo de nuevo más tarde.";
            throw new Exception("Ocurrió un error inesperado durante el inicio de sesión.");
        }
        return null;
    }

    /// <summary>
    /// Muestra la vista Consumer.
    /// </summary>
    /// <returns>La vista Consumer.</returns>
    public IActionResult Consumer()
    {
        return View();
    }

    /// <summary>
    /// Autentica un usuario Consumer mediante una solicitud de inicio de sesión.
    /// </summary>
    /// <param name="request">La solicitud de inicio de sesión del usuario Consumer.</param>
    /// <returns>La vista Index2 de Home si el usuario Consumer se autentica correctamente, de lo contrario, la vista Consumer.</returns>
    [HttpPost]
    public async Task<IActionResult> Consumer(LoginRequest request)
    {
        var login = await AuthenticateUserAsync(request, "consumer");
        if (login?.Token != null)
        {
            return RedirectToAction("Index2", "Home");
        }
        return View();
    }

    /// <summary>
    /// Muestra la vista Provider.
    /// </summary>
    /// <returns>La vista Provider.</returns>
    public IActionResult Provider()
    {
        return View();
    }

    /// <summary>
    /// Autentica un usuario Provider mediante una solicitud de inicio de sesión.
    /// </summary>
    /// <param name="request">La solicitud de inicio de sesión del usuario Provider.</param>
    /// <returns>La vista Index2 de Home si el usuario Provider se autentica correctamente, de lo contrario, la vista Provider.</returns>
    [HttpPost]
    public async Task<IActionResult> Provider(LoginRequest request)
    {
        var login = await AuthenticateUserAsync(request, "provider");
        if (login?.Token != null)
        {
            return RedirectToAction("Index2", "Home");
        }
        return View();
    }

    /// <summary>
    /// Muestra la vista Admin.
    /// </summary>
    /// <returns>La vista Admin.</returns>
    public IActionResult Admin()
    {
        return View();
    }

    /// <summary>
    /// Autentica un usuario Admin mediante una solicitud de inicio de sesión.
    /// </summary>
    /// <param name="request">La solicitud de inicio de sesión del usuario Admin.</param>
    /// <returns>La vista Index2 de Home si el usuario Admin se autentica correctamente, de lo contrario, la vista Admin.</returns>
    [HttpPost]
    public async Task<IActionResult> Admin(LoginRequest request)
    {
        var login = await AuthenticateUserAsync(request, "admin");
        if (login?.Token != null)
        {
            return RedirectToAction("Index2", "Home");
        }
        return View();
    }

    /// <summary>
    /// Muestra la vista ForgotPassword.
    /// </summary>
    /// <returns>La vista ForgotPassword.</returns>
    public IActionResult ForgotPassword()
    {
        return View();
    }

    /// <summary>
    /// Envía una solicitud de cambio de contraseña a través del correo electrónico del usuario.
    /// </summary>
    /// <param name="request">La solicitud de cambio de contraseña.</param>
    /// <returns>La vista Index de Home si la solicitud se envía correctamente, de lo contrario, NotFound.</returns>
    [ValidateAntiForgeryToken]
    [HttpPost]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest request)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("PagaloTodoApi");
            var response = await client.PostAsJsonAsync("/api/login/forgotpassword", request.Email);
            var unused = await response.Content.ReadAsStringAsync();
            TempData["success"] = $"An email was sent to {request.Email} in order to change password.";
            return RedirectToAction("Index", "Home");
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine(e);
            return NotFound();
        }
    }
}