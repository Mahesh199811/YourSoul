namespace YourSoulApp.Views;

public partial class DiscoverPage : ContentPage
{
    private readonly ViewModels.DiscoverViewModel _viewModel;
    
    public DiscoverPage(ViewModels.DiscoverViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        _viewModel = viewModel;
    }
    
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadPotentialMatchesAsync();
    }
}
