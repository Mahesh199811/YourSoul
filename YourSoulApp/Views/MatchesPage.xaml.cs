namespace YourSoulApp.Views;

public partial class MatchesPage : ContentPage
{
    private readonly ViewModels.MatchesViewModel _viewModel;
    
    public MatchesPage(ViewModels.MatchesViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        _viewModel = viewModel;
    }
    
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadMatchesAsync();
    }
}
