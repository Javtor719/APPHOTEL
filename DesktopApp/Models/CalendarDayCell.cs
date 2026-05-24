using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace DesktopApp.Models
{
    public class CalendarDayCell : INotifyPropertyChanged
    {
        public DateTime Date { get; set; }
        public int DayNumber => Date.Day;
        private string _status;
        public string Status
        {
            get => _status;
            set
            {
                _status = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Background));
                OnPropertyChanged(nameof(Foreground));
            }
        }
        private string _info;
        public string Info
        {
            get => _info;
            set
            {
                _info = value;
                OnPropertyChanged();
            }
        }
        public string BlockId { get; set; }

        public Brush Background
        {
            get
            {
                return Status switch
                {
                    "booked" => (Brush)Application.Current.Resources["UpdateBrush"],
                    "blocked" => (Brush)Application.Current.Resources["CancelBrush"],
                    "selected" => (Brush)Application.Current.Resources["AccentBrush"],
                    _ => (Brush)Application.Current.Resources["CalendarBrush"]
                };
            }
        }

        public Brush Foreground => Brushes.White;

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
