using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using SectionSteelCalculationTool.ViewModels;
using Serilog;
using Serilog.Core;

namespace SectionSteelCalculationTool {
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application {
        public IServiceProvider Services { get; }

        public static new App Current => (App) Application.Current;

        private IServiceProvider ConfigureServices() {
            var services = new ServiceCollection();

            services.AddSingleton<Logger>(provider =>
                new LoggerConfiguration().
                WriteTo.File("log.txt")
                .CreateLogger());
            services.AddSingleton<MainWindow>();
            services.AddSingleton<MainWindowViewModel>();

            return services.BuildServiceProvider();
        }

        public App() {
            Services = ConfigureServices();
        }

        private void Application_Startup(object sender, StartupEventArgs e) {
            var mainWindow = Services.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }
    }
}
