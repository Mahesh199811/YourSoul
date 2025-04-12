namespace YourSoulApp.Views;

public partial class ProfilePage : ContentPage
{
    private readonly ViewModels.ProfileViewModel _viewModel;

    public ProfilePage(ViewModels.ProfileViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Always force a refresh of the profile data when the page appears
        System.Diagnostics.Debug.WriteLine("ProfilePage appeared - refreshing data");
        await _viewModel.LoadProfileAsync(true);
    }
}
