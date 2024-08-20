using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArpScan.Desktop.WPF.ViewModel
{
    public partial class MainWindowsViewModel : ViewModelBase
    {
        public MainWindow? window;

        [ObservableProperty]
        private ObservableCollection<string> _scanIpList = [];

        [ObservableProperty]
        private string _ipStart = "";

        [ObservableProperty]
        private string _ipEnd = "";

        [ObservableProperty]
        private int _ipRange = 100;

        [ObservableProperty]
        private int _scanProgressValue = 0;

        [ObservableProperty]
        private bool _scanNotRunning = true;


        public MainWindowsViewModel()
        {

        }

        [RelayCommand]
        private void DoScan()
        {
            ScanIpList.Clear();
            ScanProgressValue = 0;
            IpRange = ArpScanCore.GetIpRange(IpStart, IpEnd);
            Task.Run(() =>
            {
                ScanNotRunning = false;
                ArpScanCore.Scan(IpStart, IpEnd, (string ip, bool success) =>
                {
                    window?.Dispatcher.Invoke(new Action(() =>
                    {
                        ScanProgressValue++;
                    }));
                    if (success)
                    {
                        window?.Dispatcher.Invoke(new Action(() =>
                        {
                            ScanIpList.Add(ip);
                            var scanIpList_list = ScanIpList.ToList();
                            scanIpList_list.Sort((x, y) =>
                            {
                                return (int)ArpScanCore.IpStr2Uint(x) - (int)ArpScanCore.IpStr2Uint(y);
                            });
                            ScanIpList = new ObservableCollection<string>(scanIpList_list);
                        }));
                    }
                });
                ScanNotRunning = true;
            });
        }
    }
}
