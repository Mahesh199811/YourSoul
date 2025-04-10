using YourSoulApp.Services;

namespace YourSoulApp;

public partial class App : Application
{
	private readonly AuthService _authService;

	public App(AuthService authService)
	{
		InitializeComponent();

		_authService = authService;
		_authService.UserLoggedIn += OnUserLoggedIn;
		_authService.UserLoggedOut += OnUserLoggedOut;

		MainPage = new AppShell();

		// Check if user is already logged in
		if (!_authService.IsLoggedIn())
		{
			GoToLoginPage();
		}
		else
		{
			GoToMainPage();
		}
	}

	private void OnUserLoggedIn(object sender, Models.User user)
	{
		MainThread.BeginInvokeOnMainThread(() =>
		{
			GoToMainPage();
		});
	}

	private void OnUserLoggedOut(object sender, EventArgs e)
	{
		MainThread.BeginInvokeOnMainThread(() =>
		{
			GoToLoginPage();
		});
	}

	private void GoToLoginPage()
	{
		Shell.Current.GoToAsync("//login");
	}

	private void GoToMainPage()
	{
		Shell.Current.GoToAsync("//main");
	}
}
