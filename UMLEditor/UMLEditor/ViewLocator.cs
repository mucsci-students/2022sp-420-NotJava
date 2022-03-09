using System;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using UMLEditor.ViewModels;

namespace UMLEditor
{
    /// <summary>
    /// View Locator
    /// </summary>
    public class ViewLocator : IDataTemplate
    {
        /// <summary>
        /// Build
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public IControl Build(object data)
        {
            var name = data.GetType().FullName!.Replace("ViewModel", "View");
            var type = Type.GetType(name);

            if (type != null)
            {
                return (Control) Activator.CreateInstance(type)!;
            }
            else
            {
                return new TextBlock {Text = "Not Found: " + name};
            }
        }

        /// <summary>
        /// Match
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool Match(object data)
        {
            return data is ViewModelBase;
        }
    }
}