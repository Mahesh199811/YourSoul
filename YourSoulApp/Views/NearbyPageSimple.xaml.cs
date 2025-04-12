using System.Diagnostics;

namespace YourSoulApp.Views
{
    public partial class NearbyPageSimple : ContentPage
    {
        private readonly ViewModels.NearbyViewModel _viewModel = null!;

        public NearbyPageSimple(ViewModels.NearbyViewModel viewModel)
        {
            try
            {
                Debug.WriteLine("NearbyPageSimple constructor called");
                InitializeComponent();
                BindingContext = viewModel;
                _viewModel = viewModel;
                Debug.WriteLine("NearbyPageSimple constructor completed successfully");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in NearbyPageSimple constructor: {ex}");
                Debug.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }

        protected override async void OnAppearing()
        {
            try
            {
                Debug.WriteLine("NearbyPageSimple OnAppearing called");
                base.OnAppearing();

                // Check if user is logged in before loading data
                if (!_viewModel.IsUserLoggedIn())
                {
                    Debug.WriteLine("User not logged in, cannot load nearby users");
                    await DisplayAlert("Not Logged In", "Please log in to see nearby users", "OK");
                    return;
                }

                // Load nearby users
                Debug.WriteLine("Loading nearby users...");
                await _viewModel.LoadNearbyUsersAsync();
                Debug.WriteLine($"Loaded {_viewModel.NearbyUsers.Count} nearby users");
                Debug.WriteLine("OnAppearing completed successfully");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in OnAppearing: {ex}");
                Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
            }
        }
    }
}
