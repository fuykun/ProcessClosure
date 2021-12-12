using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.System.Diagnostics;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace ProcessClosure
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        private static List<ProcessDiagnosticInfo> processes;
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private async void processList_Loading(FrameworkElement sender, object args)
        {
            var aa = await AppDiagnosticInfo.RequestAccessAsync();
            DiagnosticAccessStatus diagnosticAccessStatus = aa;

            switch (diagnosticAccessStatus)
            {
                case DiagnosticAccessStatus.Allowed:
                    processes = ProcessDiagnosticInfo.GetForProcesses().ToList();

                    processes.ForEach(p => processList.Items.Add(p.ExecutableFileName));
                    break;
                case DiagnosticAccessStatus.Limited:
                    break;
            }
        }

        private void addToSelected_Click(object sender, RoutedEventArgs e)
        {
            var selectedProcesses = processList.SelectedItems.ToList();

            foreach (var process in selectedProcesses)
            {
                selectedProcessList.Items.Add(process);
                processList.Items.Remove(process);
            }
        }

    }
}
