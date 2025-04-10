namespace YourSoulApp.Views;

public partial class ChatDetailPage : ContentPage
{
    private readonly ViewModels.ChatDetailViewModel _viewModel;
    
    public ChatDetailPage(ViewModels.ChatDetailViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        _viewModel = viewModel;
    }
    
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadChatAsync();
    }
}
