﻿using System;
using System.ComponentModel;
using System.Windows;
using AgeZodiacCalculator.Content;
using AgeZodiacCalculator.Info;
using AgeZodiacCalculator.Model;
using AgeZodiacCalculator.Model.Impl;
using AgeZodiacCalculator.ViewModel;
using Shared.View;
using Shared.View.Container;
using Shared.View.Navigator;

namespace AgeZodiacCalculator
{
    public partial class AgeZodiacCalculatorApp
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            var window = new ContentWindow();

            TypeConverter ageInfoConverter = TypeDescriptor.GetConverter(typeof(AgeInfo));
            TypeConverter chineseSignConverter = TypeDescriptor.GetConverter(typeof(ChineseSign));
            TypeConverter westernSignConverter = TypeDescriptor.GetConverter(typeof(WesternSign));

            IPickDateModel pickDateModel = new ConverterBasedPickDateModel(DateTime.Now, chineseSignConverter, westernSignConverter);
            var pickDateViewModel = new PickDateViewModel(pickDateModel, ageInfoConverter, chineseSignConverter, westernSignConverter);
            var pickDateContent = new PickDateContent(pickDateViewModel);
            var pickDateView = new View("Age/Zodiac Calculator", 300, 300, pickDateContent);

            IViewNavigator<Type> navigator = new ViewProviderBasedNavigator<Type>(window, new ContentTypeBasedViewContainer(pickDateView));
            navigator.Navigate(typeof(PickDateContent));

            window.Show();
        }
    }
}