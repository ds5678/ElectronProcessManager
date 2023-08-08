using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics;

namespace ElectronProcessManager.Pages;
public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    public List<Process> Processes { get; set; }

    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {
        Processes = Process.GetProcesses().Where(p => !string.IsNullOrEmpty(p.ProcessName)).ToList();
        _logger.Log(LogLevel.Information, "Found {Count} processes.", Processes.Count);
    }
}
