using YourSoulApp.Views;

namespace YourSoulApp;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

		// Register routes for navigation
		Routing.RegisterRoute("chatdetail", typeof(ChatDetailPage));
		Routing.RegisterRoute("userprofile", typeof(UserProfilePage));
	}
}
