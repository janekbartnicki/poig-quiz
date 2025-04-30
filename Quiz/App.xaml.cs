using Quiz.Stores;
using Quiz.ViewModels;
using System.Windows;

namespace Quiz;
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        NavigationStore navigationStore = new NavigationStore();

        navigationStore.CurrentViewModel = new HomeViewModel(navigationStore);

        MainWindow = new MainWindow()
        {
            DataContext = new MainViewModel(navigationStore)
        };
        MainWindow.Show();
           

        base.OnStartup(e);
    }
}

