using ElectronNET.API;
using ElectronNET.API.Entities;

namespace ElectronProcessManager;

public class Program
{
    private static bool IsElectron => true;

    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        if (IsElectron)
        {
            builder.WebHost.UseElectron(args);
        }

        // Add services to the container.
        builder.Services.AddRazorPages();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        //app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthorization();

        app.MapRazorPages();

        if (IsElectron)
        {
            await app.StartAsync();

            CreateMenu();

            // Open the Electron-Window here
            BrowserWindowOptions options = new()
            {
                DarkTheme = true,
            };
            await Electron.WindowManager.CreateWindowAsync(options);

            app.WaitForShutdown();
        }
        else
        {
            app.Run();
        }
    }

    private static void CreateMenu()
    {
        MenuItem[] fileMenu = new MenuItem[]
        {
            new MenuItem
            {
                Label = "Save As...",
                Type = MenuType.normal,
                Click = async () =>
                {
                    var mainWindow = Electron.WindowManager.BrowserWindows.First();
                    var options = new SaveDialogOptions()
                    {
                        Filters = new FileFilter[] { new FileFilter { Name = "CSV Files", Extensions = new string[] { "csv" } } }
                    };
                    string result = await Electron.Dialog.ShowSaveDialogAsync(mainWindow, options);
                    if (!string.IsNullOrEmpty(result))
                    {
                        string url = $"http://localhost:{BridgeSettings.WebPort}/SaveAs?path={result}";
                        mainWindow.LoadURL(url);
                    }
                }
            },
            new MenuItem { Type = MenuType.separator },
            new MenuItem { Role = OperatingSystem.IsMacOS() ? MenuRole.close : MenuRole.quit }
        };

        MenuItem[] editMenu = new MenuItem[]
        {
            new MenuItem { Role = MenuRole.undo, Accelerator = "CmdOrCtrl+Z" },
            new MenuItem { Role = MenuRole.redo, Accelerator = "Shift+CmdOrCtrl+Z" },
            new MenuItem { Type = MenuType.separator },
            new MenuItem { Role = MenuRole.cut, Accelerator = "CmdOrCtrl+X" },
            new MenuItem { Role = MenuRole.copy, Accelerator = "CmdOrCtrl+C" },
            new MenuItem { Role = MenuRole.paste, Accelerator = "CmdOrCtrl+V" },
            new MenuItem { Role = MenuRole.delete },
            new MenuItem { Type = MenuType.separator },
            new MenuItem { Role = MenuRole.selectall, Accelerator = "CmdOrCtrl+A" }
        };

        MenuItem[] viewMenu = new MenuItem[]
        {
            new MenuItem { Role = MenuRole.reload },
            new MenuItem { Role = MenuRole.forcereload },
            new MenuItem { Role = MenuRole.toggledevtools },
            new MenuItem { Type = MenuType.separator },
            new MenuItem { Role = MenuRole.togglefullscreen }
        };

        MenuItem[] windowMenu = new MenuItem[]
        {
            new MenuItem { Role = MenuRole.minimize, Accelerator = "CmdOrCtrl+M" },
            new MenuItem { Role = MenuRole.close, Accelerator = "CmdOrCtrl+W" }
        };

        MenuItem[] menu;
        if (OperatingSystem.IsMacOS())
        {
            MenuItem[] appMenu = new MenuItem[]
            {
                new MenuItem { Role = MenuRole.about },
                new MenuItem { Type = MenuType.separator },
                new MenuItem { Role = MenuRole.services },
                new MenuItem { Type = MenuType.separator },
                new MenuItem { Role = MenuRole.hide },
                new MenuItem { Role = MenuRole.hideothers },
                new MenuItem { Role = MenuRole.unhide },
                new MenuItem { Type = MenuType.separator },
                new MenuItem { Role = MenuRole.quit }
            };

            menu = new MenuItem[]
            {
                new MenuItem { Label = "Electron", Type = MenuType.submenu, Submenu = appMenu },
                new MenuItem { Label = "File", Type = MenuType.submenu, Submenu = fileMenu },
                new MenuItem { Role = MenuRole.editMenu, Submenu = editMenu },
                new MenuItem { Label = "View", Type = MenuType.submenu, Submenu = viewMenu },
                new MenuItem { Role = MenuRole.windowMenu, Submenu = windowMenu },
            };
        }
        else
        {
            menu = new MenuItem[]
            {
                new MenuItem { Label = "File", Type = MenuType.submenu, Submenu = fileMenu },
                new MenuItem { Role = MenuRole.editMenu, Submenu = editMenu },
                new MenuItem { Label = "View", Type = MenuType.submenu, Submenu = viewMenu },
                new MenuItem { Role = MenuRole.windowMenu, Submenu = windowMenu },
            };
        }

        Electron.Menu.SetApplicationMenu(menu);
    }
}
