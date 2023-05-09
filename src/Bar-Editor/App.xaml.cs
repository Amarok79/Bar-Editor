// Copyright (c) 2022, Olaf Kober <olaf.kober@outlook.com>

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using WinUIEx;


namespace Bar;


public partial class App : Application
{
    private Window? mWindow;


    public new static App Current => (App)Application.Current;


    public Window MainWindow => mWindow!;

    public IConfigurationRoot Configuration { get; }

    public IServiceProvider Services { get; }


    public App()
    {
        Configuration = _BuildConfiguration();
        Services = _BuildServices(Configuration);

        InitializeComponent();
    }

    protected override void OnLaunched(
        LaunchActivatedEventArgs args
    )
    {
        mWindow = new MainWindow();
        mWindow.Activate();
        mWindow.Title = "Bar Editor";
        mWindow.Maximize();
    }


    public TViewModel CreateViewModel<TViewModel>()
        where TViewModel : class
    {
        return ActivatorUtilities.CreateInstance<TViewModel>(Services);
    }


    private static IConfigurationRoot _BuildConfiguration()
    {
        var builder = new ConfigurationBuilder();

        var root = builder.AddJsonFile("appsettings.json", true)
            .AddJsonFile("appsettings.Development.json", true)
            .AddJsonFile("appsettings.Production.json", true)
            .AddEnvironmentVariables("BAR_")
            .AddCommandLine(Environment.GetCommandLineArgs())
            .Build();

        return root;
    }

    private static IServiceProvider _BuildServices(
        IConfigurationRoot configuration
    )
    {
        var services = new ServiceCollection();

        services.AddSingleton<IConfiguration>(configuration);
        services.AddSingleton(configuration);

        return services.BuildServiceProvider();
    }
}
