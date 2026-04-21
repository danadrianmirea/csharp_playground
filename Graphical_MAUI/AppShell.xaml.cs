using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;

namespace HelloMauiApp;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
	}

	private async void OnFileExitClicked(object? sender, EventArgs e)
	{
		Application.Current?.Quit();
	}

	private async void OnHelpAboutClicked(object? sender, EventArgs e)
	{
		await DisplayAlertAsync("About Hello MAUI App", 
			"This is a simple .NET MAUI Hello World application with menus.\n\n" +
			"Created as a demonstration of MAUI development.", 
			"OK");
	}
}
