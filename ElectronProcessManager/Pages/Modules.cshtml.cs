using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics;

namespace ElectronProcessManager.Pages;

public class ModulesModel : PageModel
{
    public Process? Process { get; set; }

    public int? Id => Process?.Id;

    public bool TryGetModules(out IEnumerable<string> modules, out Exception? error)
    {
        if (Process == null)
        {
            modules = Enumerable.Empty<string>();
            error = null;
            return false;
        }

        try
        {
            modules = Process.Modules
                .Cast<ProcessModule>()
                .Select(m => m.ModuleName)
                .Order()
                .ToList();
            error = null;
            return true;
        }
        catch (Exception ex)
        {
            modules = Enumerable.Empty<string>();
            error = ex;
            return false;
        }
    }

    public string Name
    {
        get
        {
            string? processName = Process?.ProcessName;
            return string.IsNullOrEmpty(processName) ? "Modules" : $"Modules: {processName}";
        }
    }

    public void OnGet(int? id)
    {
        if (id.HasValue)
        {
            Process = Process.GetProcessById(id.Value);
        }

        if (Process == null)
        {
            NotFound();
        }
    }
}
