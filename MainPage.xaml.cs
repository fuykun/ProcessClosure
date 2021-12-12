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
                    processes = ProcessDiagnosticInfo.GetForProcesses().OrderBy(x => x.ExecutableFileName).ToList();

                    processes.Where(x => x.IsPackaged).ToList().ForEach(p => processList.Items.Add(p.ExecutableFileName));
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
        private void removeToSelected_Click(object sender, RoutedEventArgs e)
        {
            var selectedProcesses = selectedProcessList.SelectedItems.ToList();

            foreach (var process in selectedProcesses)
            {
                processList.Items.Add(process);
                selectedProcessList.Items.Remove(process);
            }
        }

        private async void btnKillAll_Click(object sender, RoutedEventArgs e)
        {
            var selectedProcesses = selectedProcessList.Items.ToList();

            var aa = processes.Where(x => selectedProcesses.Contains(x.ExecutableFileName)).ToList();

            foreach (var process in aa)
            {
                var infos = process.GetAppDiagnosticInfos();
                if (infos != null)
                {
                    foreach (var info in infos)
                    {
                        IList<AppResourceGroupInfo> groups = info.GetResourceGroups();
                        foreach (AppResourceGroupInfo group in groups)
                        {
                            AppExecutionStateChangeResult result = await group.StartTerminateAsync();
                            if (result != null && result.ExtendedError != null)
                            {
                                var dialog = new MessageDialog(result.ExtendedError?.ToString());
                                await dialog.ShowAsync();
                            }
                            else
                            {
                                var dialog = new MessageDialog("success");
                                await dialog.ShowAsync();
                            }
                        }
                    }
                    selectedProcessList.Items.Remove(process.ExecutableFileName);
                    processList.Items.Remove(process.ExecutableFileName);
                }
            }
        }

        private async void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            var aa = await AppDiagnosticInfo.RequestAccessAsync();
            DiagnosticAccessStatus diagnosticAccessStatus = aa;

            switch (diagnosticAccessStatus)
            {
                case DiagnosticAccessStatus.Allowed:
                    processList.Items.Clear();
                    processes = ProcessDiagnosticInfo.GetForProcesses().OrderBy(x => x.ExecutableFileName).ToList();

                    processes.Where(x => x.IsPackaged).ToList().ForEach(p => processList.Items.Add(p.ExecutableFileName));
                    break;
                case DiagnosticAccessStatus.Limited:
                    break;
            }
        }

        private async void btnClear_Click(object sender, RoutedEventArgs e)
        {
            selectedProcessList.Items.Clear();
            btnRefresh_Click(sender, e);
        }
    }
}
