using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using DesktopApp.Commands;
using DesktopApp.Models;
using DesktopApp.Services;
using DesktopApp.Views;

namespace DesktopApp.ViewModels
{
    public class RoomCalendarViewModel : INotifyPropertyChanged
    {
        private readonly ApiClient _api = ApiClient.Instance;

        public ObservableCollection<Rooms> Rooms { get; } = new();
        public ObservableCollection<CalendarDayCell> Days { get; } = new();

        private Rooms _selectedRoom;
        public Rooms SelectedRoom
        {
            get => _selectedRoom;
            set
            {
                _selectedRoom = value;
                OnPropertyChanged();
                _ = LoadCalendarAsync();
            }
        }

        private DateTime _currentMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        public string MonthTitle => _currentMonth.ToString("MMMM yyyy", new CultureInfo("es-ES")).ToUpper();

        private DateTime? _rangeStart;

        public ICommand PreviousMonthCommand { get; }
        public ICommand NextMonthCommand { get; }
        public ICommand DayClickCommand { get; }
        public ICommand DeleteBlockCommand { get; }

        public RoomCalendarViewModel(Rooms selectedRoom, IEnumerable<Rooms> rooms)
        {
            foreach (var room in rooms)
                Rooms.Add(room);

            _selectedRoom = selectedRoom;

            PreviousMonthCommand = new RelayCommand(async _ => await ChangeMonthAsync(-1));
            NextMonthCommand = new RelayCommand(async _ => await ChangeMonthAsync(1));
            DayClickCommand = new RelayCommand(async day => await SelectDayAsync(day as CalendarDayCell));
            DeleteBlockCommand = new RelayCommand(async day => await DeleteBlockAsync(day as CalendarDayCell));

            _ = LoadCalendarAsync();
        }

        private async Task ChangeMonthAsync(int months)
        {
            _currentMonth = _currentMonth.AddMonths(months);
            _rangeStart = null;
            OnPropertyChanged(nameof(MonthTitle));
            await LoadCalendarAsync();
        }

        private async Task LoadCalendarAsync()
        {
            if (SelectedRoom == null)
                return;

            try
            {
                Days.Clear();

                string month = _currentMonth.ToString("yyyy-MM");
                var response = await _api.GetRoomCalendarAsync(SelectedRoom.Id, month);

                foreach (var item in response.Calendar.OrderBy(c => c.Key))
                {
                    var date = DateTime.Parse(item.Key);
                    var data = item.Value;

                    var block = data.Blocks.FirstOrDefault(b => !string.IsNullOrEmpty(b.Id));
                    var reservation = data.Reservations.FirstOrDefault();

                    Days.Add(new CalendarDayCell
                    {
                        Date = date,
                        Status = data.Status,
                        BlockId = block?.Id,
                        Info = data.Status switch
                        {
                            "booked" => reservation != null ? $"Reserva {reservation.ReservationNumber}" : "Ocupado",
                            "blocked" => block != null ? block.Reason : data.Blocks.FirstOrDefault()?.Reason ?? "Bloqueado",
                            _ => "Libre"
                        }
                    });
                }

                OnPropertyChanged(nameof(MonthTitle));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error cargando calendario: " + ex.Message);
            }
        }

        private async Task SelectDayAsync(CalendarDayCell day)
        {
            if (day == null)
                return;

            if (day.Status == "booked")
            {
                MessageBox.Show("No puedes bloquear un día con reserva.");
                return;
            }

            if (day.Status == "blocked")
            {
                MessageBox.Show("Este día ya está bloqueado. Usa eliminar bloqueo si es manual.");
                return;
            }

            if (_rangeStart == null)
            {
                _rangeStart = day.Date;
                day.Status = "selected";
                day.Info = "Seleccionado";
                return;
            }

            var start = _rangeStart.Value <= day.Date ? _rangeStart.Value : day.Date;
            var end = _rangeStart.Value <= day.Date ? day.Date : _rangeStart.Value;

            // Backend trabaja con fecha fin exclusiva, así que sumamos 1 día.
            var endExclusive = end.AddDays(1);

            var reasonWindow = new RoomBlockReasonWindow
            {
                Owner = Application.Current.MainWindow
            };

            if (reasonWindow.ShowDialog() == true)
            {
                try
                {
                    await _api.CreateRoomBlockAsync(SelectedRoom.Id, start, endExclusive, reasonWindow.Reason);
                    MessageBox.Show("Bloqueo creado correctamente.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error creando bloqueo: " + ex.Message);
                }
            }

            _rangeStart = null;
            await LoadCalendarAsync();
        }

        private async Task DeleteBlockAsync(CalendarDayCell day)
        {
            if (day == null || string.IsNullOrWhiteSpace(day.BlockId))
            {
                MessageBox.Show("Este día no tiene un bloqueo manual eliminable.");
                return;
            }

            var ok = MessageBox.Show(
                $"¿Eliminar bloqueo del día {day.Date:dd/MM/yyyy}?",
                "Eliminar bloqueo",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning
            );

            if (ok != MessageBoxResult.Yes)
                return;

            try
            {
                await _api.DeleteRoomBlockAsync(SelectedRoom.Id, day.BlockId);
                MessageBox.Show("Bloqueo eliminado correctamente.");
                await LoadCalendarAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error eliminando bloqueo: " + ex.Message);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
