using DesktopApp.Commands;
using DesktopApp.Models;
using DesktopApp.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace DesktopApp.ViewModels
{
    public class ListRoomsViewModel : INotifyPropertyChanged
    {
        private readonly ApiClient _api =  ApiClient.Instance;
        public event PropertyChangedEventHandler? PropertyChanged;

        public ObservableCollection<Rooms> Rooms { get; set; } = new();
        public ObservableCollection<Reviews> Reviews { get; set; } = new();

        private Rooms? _selectedRoom;
        public Rooms? SelectedRoom
        {
            get => _selectedRoom;
            set { _selectedRoom = value; OnPropertyChanged(); }
        }

        public ICommand DeleteRoomCommand { get; }
        public ListRoomsViewModel()
        {
            _ = LoadRoomsAsync();
            DeleteRoomCommand = new RelayCommand(async room => await DeleteDataRooms(room as Rooms), room => room is Rooms);
        }
        public async Task LoadRoomsAsync()
        {
            try
            {
                var list = await _api.GetRooms();

                Rooms.Clear();

                string month = DateTime.Today.ToString("yyyy-MM");
                string todayKey = DateTime.Today.ToString("yyyy-MM-dd");

                foreach (var r in list)
                {
                    r.EffectiveAvailability = r.availability.ToString().ToLower();

                    try
                    {
                        var calendar = await _api.GetRoomCalendarAsync(r.Id, month);

                        if (calendar.Calendar.TryGetValue(todayKey, out var today))
                        {
                            if (today.Status == "blocked")
                                r.EffectiveAvailability = "block";
                            else if (today.Status == "booked")
                                r.EffectiveAvailability = "unavailable";
                            else
                                r.EffectiveAvailability = "available";
                        }
                    }
                    catch
                    {
                        r.EffectiveAvailability = r.availability.ToString().ToLower();
                    }

                    Rooms.Add(r);
                }

                OnPropertyChanged(nameof(Rooms));
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
        private async Task DeleteDataRooms(Rooms? room)
        {
            try
            {
                if (room is null)
                {
                    MessageBox.Show("Selecciona una habitación para eliminar.");
                    return;
                }

                var ok = MessageBox.Show(
                    $"¿Eliminar la habitación '{room.numRoom}'?",
                    "Confirmar eliminación",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning) == MessageBoxResult.Yes;

                if (!ok) return;

                await _api.DeleteIdRoom(room.Id);

                MessageBox.Show(
                    "Habitación Eliminada\n\n" +
                    $"Número habitación: {room.numRoom}\n" +
                    $"Planta: {room.numFloor}",
                    "Éxito",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );

                await LoadRoomsAsync();

                SelectedRoom = null;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
        private void OnPropertyChanged([CallerMemberName] string? n = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(n));
    }
}
