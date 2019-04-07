using System;
using System.Windows;

namespace Lab_05_Lopukhina.Managers
{
    internal static class StationManager
    {
        public static event Action StopThreads;

        internal static void CloseApp()
        {
            MessageBox.Show("Closing the app...");
            StopThreads?.Invoke();
            Environment.Exit(1);
        }
    }
}