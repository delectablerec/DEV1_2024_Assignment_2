using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

public class HomeController : Controller
{
    private readonly UserManager<Cliente> _userManager;
    private CarrelloService _carrelloService;
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger, UserManager<Cliente> userManager, CarrelloService carrelloService)
    {
        _logger = logger;
        _userManager = userManager;
        _carrelloService = carrelloService;
    }

    public IActionResult Index()
    {
        ViewData["CartItemCount"] = ItemsInCart();
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

        public int ItemsInCart()
    {
        var userId = _userManager.GetUserId(User);
        if (string.IsNullOrEmpty(userId))
        {
            _logger.LogError("User ID is null or empty.");
            return 0;
        }

        var carrello = _carrelloService.CaricaCarrello(userId);

        if (carrello == null || carrello.Carrello.Count == 0)
        {
            _logger.LogWarning("Carrello vuoto per UserId: {UserId}", userId);
            return 0;
        }
        else
        {
            return carrello.Carrello.Count;
        }
    }
}
