using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using ElectronNET.API;
using ElectronNET.API.Entities;

namespace ElectronProcessManager.Pages;

public class ViewModel : PageModel
{
    private readonly ILogger<ViewModel> _logger;

    public ViewModel(ILogger<ViewModel> logger)
    {
        _logger = logger;
    }

    public Process? Process { get; set; }

    public IEnumerable<KeyValuePair<string, object>> Properties
    {
        get
        {
            if (Process == null)
            {
                yield break;
            }

            yield return new KeyValuePair<string, object>(nameof(Process.Id), Process.Id);
            yield return new KeyValuePair<string, object>(nameof(Process.ProcessName), Process.ProcessName);
            yield return new KeyValuePair<string, object>(nameof(Process.WorkingSet64), Process.WorkingSet64);
        }
    }

    public string Name
    {
        get
        {
            string? processName = Process?.ProcessName;
            if (string.IsNullOrEmpty(processName))
            {
                return "Process View";
            }
            else
            {
                return $"Process View: {processName}";
            }
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

    public async Task<IActionResult> OnPostAsync(int? id)
    {
        if (id.HasValue)
        {
            Process = Process.GetProcessById(id.Value);
        }

        if (Process == null)
        {
            return NotFound();
        }

        MessageBoxResult result;
        {
            MessageBoxOptions options = new("Are you sure you want to kill this process?")
            {
                Type = MessageBoxType.question,
                Buttons = new string[] { "No", "Yes" },
                DefaultId = 1,
                CancelId = 0
            };
            result = await Electron.Dialog.ShowMessageBoxAsync(options);
        }

        if (result.Response == 1)
        {
            try
            {
                Process.Kill();
            }
            catch
            {
                MessageBoxOptions options = new("This application is unable to kill processes.")
                {
                    Type = MessageBoxType.info,
                };
                await Electron.Dialog.ShowMessageBoxAsync(options);
                return Page();
            }
            return RedirectToPage("Index");
        }

        return Page();
    }
}
