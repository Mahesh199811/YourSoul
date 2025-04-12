using System.Diagnostics;

namespace YourSoulApp.Views;

public partial class MatchesPage : ContentPage
{
    private readonly ViewModels.MatchesViewModel _viewModel;
    private bool _isFirstAppearance = true;

    public MatchesPage(ViewModels.MatchesViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        try
        {
            Debug.WriteLine("MatchesPage OnAppearing");
            await _viewModel.LoadMatchesAsync();

            // Force a refresh on first appearance to ensure clean data
            if (_isFirstAppearance)
            {
                _isFirstAppearance = false;
                await Task.Delay(100); // Small delay to ensure UI is ready
                await _viewModel.LoadMatchesAsync();
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error in MatchesPage.OnAppearing: {ex.Message}\n{ex.StackTrace}");
        }
    }
}
