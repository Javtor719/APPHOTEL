using DesktopApp.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DesktopApp.ViewModels
{
    public class DashboardViewModel : INotifyPropertyChanged
    {
        private readonly ApiClient _api = ApiClient.Instance;

        private int _totalRooms;
        public int TotalRooms
        {
            get => _totalRooms;
            set { _totalRooms = value; OnPropertyChanged(); }
        }

        private int _todayCheckIns;
        public int TodayCheckIns
        {
            get => _todayCheckIns;
            set { _todayCheckIns = value; OnPropertyChanged(); }
        }

        private int _todayCheckOuts;
        public int TodayCheckOuts
        {
            get => _todayCheckOuts;
            set { _todayCheckOuts = value; OnPropertyChanged(); }
        }

        private int _activeReservations;
        public int ActiveReservations
        {
            get => _activeReservations;
            set { _activeReservations = value; OnPropertyChanged(); }
        }

        public DashboardViewModel()
        {
            _ = LoadStatsAsync();
        }

        private async Task LoadStatsAsync()
        {
            try
            {
                var stats = await _api.GetDashboardStatsAsync();

                TotalRooms = stats.TotalRooms;
                TodayCheckIns = stats.TodayCheckIns;
                TodayCheckOuts = stats.TodayCheckOuts;
                ActiveReservations = stats.ActiveReservations;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error cargando dashboard: " + ex.Message);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
