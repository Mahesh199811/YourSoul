namespace YourSoulApp.Views;

public partial class UserProfilePage : ContentPage
{
    private readonly ViewModels.UserProfileViewModel _viewModel;
    
    public UserProfilePage(ViewModels.UserProfileViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        _viewModel = viewModel;
    }
    
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadUserProfileAsync();
    }
}
