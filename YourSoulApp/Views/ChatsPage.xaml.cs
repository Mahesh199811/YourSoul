namespace YourSoulApp.Views;

public partial class ChatsPage : ContentPage
{
    private readonly ViewModels.ChatsViewModel _viewModel;
    
    public ChatsPage(ViewModels.ChatsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        _viewModel = viewModel;
    }
    
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadConversationsAsync();
    }
}
