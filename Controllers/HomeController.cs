using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

public class HomeController : Controller
{
    private CarrelloService _carrelloService;
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger, CarrelloService carrelloService)
    {
        _logger = logger;
        _carrelloService = carrelloService;
    }

    public IActionResult Index()
    {
        ViewData["CartItemCount"] = _carrelloService.ItemsInCart();
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

