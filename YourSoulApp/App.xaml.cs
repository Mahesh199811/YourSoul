using System.Diagnostics;
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
			Debug.WriteLine("Starting app initialization...");

			// Set up global unhandled exception handlers
			AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
			{
				Exception ex = (Exception)args.ExceptionObject;
				Debug.WriteLine($"UNHANDLED EXCEPTION: {ex.Message}\n{ex.StackTrace}");
				// Log to file if possible
				LogExceptionToFile(ex, "AppDomain.UnhandledException");
			};

			TaskScheduler.UnobservedTaskException += (sender, args) =>
			{
				Debug.WriteLine($"UNOBSERVED TASK EXCEPTION: {args.Exception.Message}\n{args.Exception.StackTrace}");
				// Log to file if possible
				LogExceptionToFile(args.Exception, "TaskScheduler.UnobservedTaskException");
				args.SetObserved(); // Prevent the app from crashing
			};

			// Add a small delay to ensure the UI is fully loaded
			await Task.Delay(500);

			// Wait for database initialization to complete with a timeout
			var timeoutTask = Task.Delay(15000); // 15 second timeout
			var initTask = _databaseService.WaitForInitializationAsync();

			Debug.WriteLine("Waiting for database initialization...");
			var completedTask = await Task.WhenAny(initTask, timeoutTask);

			if (completedTask == timeoutTask)
			{
				Debug.WriteLine("Database initialization timed out!");
				// Instead of throwing an exception, try to continue
				Debug.WriteLine("Attempting to continue despite timeout...");
			}
			else
			{
				Debug.WriteLine("Database initialization completed successfully");
			}

			// Check if user is already logged in
			if (!_authService.IsLoggedIn())
			{
				Debug.WriteLine("No user logged in, navigating to login page");
				MainThread.BeginInvokeOnMainThread(() => {
					try {
						GoToLoginPage();
					} catch (Exception ex) {
						Debug.WriteLine($"Error navigating to login page: {ex.Message}\n{ex.StackTrace}");
						LogExceptionToFile(ex, "Navigation to login page failed");
					}
				});
			}
			else
			{
				Debug.WriteLine("User already logged in, refreshing user data");
				try {
					// Make sure we have the latest user data
					await _authService.UpdateCurrentUserAsync();
					Debug.WriteLine($"User data refreshed: {AuthService.CurrentUser?.Name}");

					MainThread.BeginInvokeOnMainThread(() => {
						try {
							GoToMainPage();
						} catch (Exception ex) {
							Debug.WriteLine($"Error navigating to main page: {ex.Message}\n{ex.StackTrace}");
							LogExceptionToFile(ex, "Navigation to main page failed");
							// Try to go to login as fallback
							GoToLoginPage();
						}
					});
				} catch (Exception ex) {
					Debug.WriteLine($"Error refreshing user data: {ex.Message}\n{ex.StackTrace}");
					LogExceptionToFile(ex, "User data refresh failed");
					// Go to login page as fallback
					MainThread.BeginInvokeOnMainThread(() => GoToLoginPage());
				}
			}
		}
		catch (Exception ex)
		{
			// If initialization fails, go to login page
			Debug.WriteLine($"Initialization error: {ex.Message}\n{ex.StackTrace}");
			LogExceptionToFile(ex, "App initialization failed");

			// Add a small delay before navigating to ensure the UI is ready
			await Task.Delay(500);
			MainThread.BeginInvokeOnMainThread(() => {
				try {
					GoToLoginPage();
				} catch (Exception navEx) {
					Debug.WriteLine($"Failed to navigate to login page after error: {navEx.Message}");
					LogExceptionToFile(navEx, "Navigation after error failed");
				}
			});
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
		try
		{
			Shell.Current.GoToAsync("//login");
		}
		catch (Exception ex)
		{
			Debug.WriteLine($"Error in GoToLoginPage: {ex.Message}\n{ex.StackTrace}");
			LogExceptionToFile(ex, "GoToLoginPage failed");
		}
	}

	private void GoToMainPage()
	{
		try
		{
			Shell.Current.GoToAsync("//main");
		}
		catch (Exception ex)
		{
			Debug.WriteLine($"Error in GoToMainPage: {ex.Message}\n{ex.StackTrace}");
			LogExceptionToFile(ex, "GoToMainPage failed");
		}
	}

	private void LogExceptionToFile(Exception ex, string context)
	{
		try
		{
			string logPath = Path.Combine(FileSystem.AppDataDirectory, "error_log.txt");
			string logMessage = $"[{DateTime.Now}] {context}: {ex.Message}\n{ex.StackTrace}\n\n";
			File.AppendAllText(logPath, logMessage);
		}
		catch
		{
			// Silently fail if logging itself fails
		}
	}
}
