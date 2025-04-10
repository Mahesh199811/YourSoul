using Microsoft.Extensions.Logging;
using Plugin.LocalNotification;
using YourSoulApp.Services;
using YourSoulApp.ViewModels;
using YourSoulApp.Views;

namespace YourSoulApp;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.UseLocalNotification()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

#if DEBUG
		builder.Logging.AddDebug();
#endif

		// Register services
		builder.Services.AddSingleton<DatabaseService>();
		builder.Services.AddSingleton<AuthService>();
		builder.Services.AddSingleton<NotificationService>();

		// Register view models
		builder.Services.AddTransient<LoginViewModel>();
		builder.Services.AddTransient<RegisterViewModel>();
		builder.Services.AddTransient<DiscoverViewModel>();
		builder.Services.AddTransient<MatchesViewModel>();
		builder.Services.AddTransient<ChatsViewModel>();
		builder.Services.AddTransient<ChatDetailViewModel>();
		builder.Services.AddTransient<ProfileViewModel>();
		builder.Services.AddTransient<UserProfileViewModel>();

		// Register views
		builder.Services.AddTransient<LoginPage>();
		builder.Services.AddTransient<RegisterPage>();
		builder.Services.AddTransient<DiscoverPage>();
		builder.Services.AddTransient<MatchesPage>();
		builder.Services.AddTransient<ChatsPage>();
		builder.Services.AddTransient<ChatDetailPage>();
		builder.Services.AddTransient<ProfilePage>();
		builder.Services.AddTransient<UserProfilePage>();

		return builder.Build();
	}
}
