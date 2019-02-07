using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;

namespace PleskEmailAliasManager.Utilities
{
    public class CaliWPFUtilities
    {
        public static UIElement GetBindedUIElement<T>()
            where T : new() => GetBindedUIElement(new T());

        public static UIElement GetBindedUIElement(object viewModel)
        {
            var uiElement = ViewLocator.LocateForModel(viewModel, null, null);
            ViewModelBinder.Bind(viewModel, uiElement, null);

            return uiElement;
        }
    }
}
