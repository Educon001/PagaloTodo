using Microsoft.AspNetCore.Mvc;

namespace UCABPagaloTodoWeb.Controllers;

public class LoginController : Controller
{
    private readonly ILogger<LoginController> _logger;

    public LoginController(ILogger<LoginController> logger)
    {
        _logger = logger;
    }
    
    public IActionResult Consumer()
    {
        return View();
    }
    
    public IActionResult Provider()
    {
        return View();
    }
    
    public IActionResult Admin()
    {
        return View();
    }
}