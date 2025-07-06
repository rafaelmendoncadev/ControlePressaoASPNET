using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ControlePressao.Models;
using ControlePressao.Services;

namespace ControlePressao.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ISaudeService _saudeService;

    public HomeController(ILogger<HomeController> logger, ISaudeService saudeService)
    {
        _logger = logger;
        _saudeService = saudeService;
    }

    public async Task<IActionResult> Index()
    {
        var dashboardData = await _saudeService.ObterDadosDashboardAsync();
        return View(dashboardData);
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
