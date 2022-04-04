using System;

namespace UMLEditor.ViewModels
{
    /// <summary>
    /// MainViewModel.cs
    /// </summary>
    public class MainViewModel : ViewModelBase
    {

        // Command for key combos
        private void OnKeyComboSent(string commandParam) => KeyComboIssued?.Invoke(this, commandParam.ToLower());
        
        /// <summary>
        /// Event for when a key combo is issued
        /// </summary>
        public static EventHandler<string>? KeyComboIssued;

    }
    
}