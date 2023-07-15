using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
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
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine(e);
            TempData["error"] = "There was an error creating the service";
        }
        return RedirectToAction("Index");
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
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine(e);
            TempData["error"] = "There was an error deleting the provider";
        }
        return RedirectToAction("Index");
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
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine(e);
            TempData["error"] = "There was an error updating the provider";
        }
        if (CurrentUser.GetUser().UserType == "provider")
        {
            return RedirectToAction("Index2", "Home");
        }
        return RedirectToAction("Index");
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
            if (provider.Username == username)
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

    [Route("provider/updatePassword/{token:guid?}", Name="updatePswdProvider")]
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
            TempData["success"] = "Password changed successfully";
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
    
    public IActionResult UploadConciliation()
    {
        var model = new UploadConciliationViewModel
        {
            SuccessMessage = TempData["success"] as string ?? string.Empty,
            ErrorMessage = TempData["error"] as string ?? string.Empty
        };

        return View(model);
    }
    
    [ValidateAntiForgeryToken]
    [HttpPost]
    public async Task<IActionResult> UploadConciliationPost(List<IFormFile> files)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("PagaloTodoApi");
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", CurrentUser.GetUser().Token);

            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    // Leer el contenido del archivo y enviarlo a la API
                    using (var stream = new MemoryStream())
                    {
                        await file.CopyToAsync(stream);
                        stream.Seek(0, SeekOrigin.Begin);
                        var content = new StreamContent(stream);
                        content.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                        {
                            Name = "files",
                            FileName = file.FileName
                        };

                        var multipartContent = new MultipartFormDataContent();
                        multipartContent.Add(content);
                        
                        // Realizar una solicitud POST a la API para cargar el archivo de conciliación
                        var response = await client.PostAsync("/providers/uploadconciliation", multipartContent);
                        response.EnsureSuccessStatusCode();
                        
                        TempData["success"] = "File(s) uploaded successfully";
                    }
                }
            }

        }
        catch (HttpRequestException e)
        {
            Console.WriteLine(e);
            TempData["error"] = "An error occurred while uploading the file(s)";
        }
        
        // Espera 5 segundos antes de redirigir al usuario.
        await Task.Delay(1500);

        // Redirige al usuario a la página de inicio.
        return RedirectToAction("Index2", "Home");
    }
    
    public async Task<IActionResult> ShowMyServices(Guid id)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("PagaloTodoApi");
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", CurrentUser.GetUser().Token);
            var response = await client.GetAsync($"/providers/{id}");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };
            var services = JsonSerializer.Deserialize<IEnumerable<ServiceModel>>(JObject.Parse(content)["services"].ToString(), options);
            return View(services);
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine(e);
            return NotFound();
        }
    }
    
    
    
    [Route("/{id:Guid}/debtors", Name="UploadConfirmationList")]
    public IActionResult UploadConfirmationList(Guid id)
    {
        ViewBag.ServiceId = id;
        return View();
    }
    
    [ValidateAntiForgeryToken]
    [HttpPost]
    [Route("/{id:Guid}/debtors", Name = "UploadConfirmationList")]
    public async Task<IActionResult> UploadConfirmationList(ConfirmationList request, Guid id)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("PagaloTodoApi");
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", CurrentUser.GetUser().Token);
    
            if (request.File.Length > 0)
            {
                // Leer el contenido del archivo y enviarlo a la API
                using (var stream = new MemoryStream())
                {
                    await request.File.CopyToAsync(stream);
                    stream.Seek(0, SeekOrigin.Begin);
                    var content = new StreamContent(stream);
                    content.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                    {
                        Name = "file",
                        FileName = request.File.FileName
                    };
    
                    var multipartContent = new MultipartFormDataContent();
                    multipartContent.Add(content);
    
                    // Realizar una solicitud POST a la API para cargar el archivo deudores
                    var response = await client.PostAsync($"/services/{id}/debtors", multipartContent);
                    var result = await response.Content.ReadAsStringAsync();
                    TempData["success"] = $"Debtors file uploaded successfully. Result: {result}";
                }
            }
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine(e);
            TempData["error"] = $"There was an error creating the conciliation format: {e.Message}";
        }
        await Task.Delay(3000); 
        return RedirectToRoute("showService", new { id });
    }
    
    //TODO: Lista de confirmacion
}