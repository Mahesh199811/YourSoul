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
        await _viewModel.LoadProfileAsync();
    }
}
