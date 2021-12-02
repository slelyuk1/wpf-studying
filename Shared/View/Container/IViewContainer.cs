﻿using Shared.Tool.View;

namespace Shared.View.Container
{
    public interface IViewContainer
    {
        IViewModelAware<TView, TViewModel> GetRequiredViewModelAware<TView, TViewModel>();
        IViewModelAware<TView, TViewModel>? GetViewModelAware<TView, TViewModel>();
    }
}