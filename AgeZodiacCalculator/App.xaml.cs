﻿using System;
using System.Windows;
using AgeZodiacCalculator.Model.UI;
using AgeZodiacCalculator.Model.UI.Impl;
using AgeZodiacCalculator.View;
using AgeZodiacCalculator.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Shared.Tool.View;
using Shared.Tool.View.Container;
using Shared.Tool.View.Navigator;
using Shared.Tool.View.Visualizer;

namespace AgeZodiacCalculator
{
    public partial class AgeZodiacCalculatorApp
    {
        private void OnStartup(object sender, StartupEventArgs e)
        {
            IHost host = Host.CreateDefaultBuilder()
                .ConfigureLogging(loggingBuilder =>
                {
                    loggingBuilder
                        .ClearProviders()
                        .AddConsole(options => { options.LogToStandardErrorThreshold = LogLevel.Information; });
                })
                .ConfigureServices(serviceCollection =>
                {
                    serviceCollection.AddSingleton<ContentWindow, ContentWindow>();
                    serviceCollection.AddSingleton<IViewVisualizer>(provider =>
                        new WindowViewVisualizer(provider.GetRequiredService<ContentWindow>())
                    );
                    serviceCollection.AddSingleton<IMutableViewContainer, DefaultViewContainer>();
                    serviceCollection.AddSingleton<IViewContainer>(provider => provider.GetRequiredService<IMutableViewContainer>());
                    serviceCollection.AddSingleton<IViewNavigator, ViewContainerBasedNavigator>();

                    serviceCollection.AddSingleton<IPickDateModel, ConverterBasedPickDateModel>();
                    serviceCollection.AddSingleton<PickDateViewModel>();
                    serviceCollection.AddSingleton<PickDateView>();
                })
                .Build();

            IServiceProvider serviceProvider = host.Services;
            var container = serviceProvider.GetRequiredService<IMutableViewContainer>();
            var pickDateView = serviceProvider.GetRequiredService<PickDateView>();
            container.AddViewModelAware(new DefaultViewModelAware<PickDateView, PickDateViewModel>(pickDateView));

            var navigator = serviceProvider.GetRequiredService<IViewNavigator>();
            var window = serviceProvider.GetRequiredService<ContentWindow>();
            navigator.Navigate<PickDateView>();
            window.Show();
        }
    }
}