using YourSoulApp.Services;

namespace YourSoulApp;

public partial class App : Application
{
	private readonly AuthService _authService;
	private readonly DatabaseService _databaseService;

	public App(AuthService authService, DatabaseService databaseService)
	{
		InitializeComponent();

		_authService = authService;
		_databaseService = databaseService;
		_authService.UserLoggedIn += OnUserLoggedIn;
		_authService.UserLoggedOut += OnUserLoggedOut;

		MainPage = new AppShell();

		// Initialize the app asynchronously
		InitializeAppAsync();
	}

	private async void InitializeAppAsync()
	{
		try
		{
			System.Diagnostics.Debug.WriteLine("Starting app initialization...");

			// Add a small delay to ensure the UI is fully loaded
			await Task.Delay(500);

			// Wait for database initialization to complete with a timeout
			var timeoutTask = Task.Delay(10000); // 10 second timeout
			var initTask = _databaseService.WaitForInitializationAsync();

			var completedTask = await Task.WhenAny(initTask, timeoutTask);

			if (completedTask == timeoutTask)
			{
				System.Diagnostics.Debug.WriteLine("Database initialization timed out!");
				throw new TimeoutException("Database initialization timed out");
			}

			System.Diagnostics.Debug.WriteLine("Database initialization completed successfully");

			// Check if user is already logged in
			if (!_authService.IsLoggedIn())
			{
				System.Diagnostics.Debug.WriteLine("No user logged in, navigating to login page");
				GoToLoginPage();
			}
			else
			{
				System.Diagnostics.Debug.WriteLine("User already logged in, refreshing user data");
				// Make sure we have the latest user data
				await _authService.UpdateCurrentUserAsync();
				System.Diagnostics.Debug.WriteLine($"User data refreshed: {AuthService.CurrentUser?.Name}");
				GoToMainPage();
			}
		}
		catch (Exception ex)
		{
			// If initialization fails, go to login page
			System.Diagnostics.Debug.WriteLine($"Initialization error: {ex}");

			// Add a small delay before navigating to ensure the UI is ready
			await Task.Delay(500);
			GoToLoginPage();
		}
	}

	private void OnUserLoggedIn(object? sender, Models.User user)
	{
		MainThread.BeginInvokeOnMainThread(() =>
		{
			GoToMainPage();
		});
	}

	private void OnUserLoggedOut(object? sender, EventArgs e)
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
