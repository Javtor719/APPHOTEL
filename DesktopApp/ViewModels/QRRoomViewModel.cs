using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using DesktopApp.Commands;
using DesktopApp.Models;
using DesktopApp.Services;
using Microsoft.Win32;

namespace DesktopApp.ViewModels
{
    public class QRRoomViewModel : INotifyPropertyChanged
    {
        private readonly ApiClient _api = ApiClient.Instance;

        private readonly Rooms _room;

        private byte[] _qrBytes;
        public ObservableCollection<RoomQrScanLog> Logs { get; } = new();

        private BitmapImage _qrImage;
        public BitmapImage QrImage
        {
            get => _qrImage;
            set
            {
                _qrImage = value;
                OnPropertyChanged();
            }
        }

        public string Title => $"Habitación {RoomNumber}";
        public string RoomNumber => _room.numRoom.ToString();

        public ICommand GenerateQrCommand { get; }
        public ICommand DownloadQrCommand { get; }
        public ICommand PrintQrCommand { get; }
        public ICommand RegenerateQrCommand { get; }
        public ICommand RefreshLogsCommand { get; }

        public QRRoomViewModel(Rooms room)
        {
            _room = room;

            GenerateQrCommand = new RelayCommand(async _ => await LoadQrAsync());
            DownloadQrCommand = new RelayCommand(_ => DownloadQr(), _ => _qrBytes != null);
            PrintQrCommand = new RelayCommand(_ => PrintQr(), _ => QrImage != null);
            RegenerateQrCommand = new RelayCommand(async _ => await RegenerateQrAsync());
            RefreshLogsCommand = new RelayCommand(async _ => await LoadLogsAsync());

            _ = LoadQrAsync();
            _ = LoadLogsAsync();
        }

        private async Task LoadQrAsync()
        {
            try
            {
                _qrBytes = await _api.GetRoomQrAsync(_room.Id);
                QrImage = ByteArrayToBitmapImage(_qrBytes);

                CommandManager.InvalidateRequerySuggested();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error generando QR: " + ex.Message);
            }
        }

        private async Task RegenerateQrAsync()
        {
            var result = MessageBox.Show(
                "¿Seguro que quieres regenerar el QR? El anterior dejará de funcionar.",
                "Regenerar QR",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning
            );

            if (result != MessageBoxResult.Yes)
                return;

            try
            {
                await _api.RegenerateRoomQrAsync(_room.Id);
                await LoadQrAsync();
                await LoadLogsAsync();

                MessageBox.Show("QR regenerado correctamente.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error regenerando QR: " + ex.Message);
            }
        }

        private async Task LoadLogsAsync()
        {
            try
            {
                var logs = await _api.GetRoomQrLogsAsync(_room.Id);

                Logs.Clear();

                foreach (var log in logs)
                {
                    var user = await _api.GetUserByIdOrDniAsync(log.ActorId,"id");

                    log.FirstName = user?.FirstName ?? "Desconocido";
                    log.LastName = user?.LastName ?? "Desconocido";

                    Logs.Add(log);

                }
                    
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error cargando logs: " + ex.Message);
            }
        }

        private void DownloadQr()
        {
            if (_qrBytes == null)
            {
                MessageBox.Show("Primero genera el QR.");
                return;
            }

            var dialog = new SaveFileDialog
            {
                Filter = "PNG (*.png)|*.png",
                FileName = $"qr-habitacion-{_room.numRoom}.png"
            };

            if (dialog.ShowDialog() == true)
            {
                File.WriteAllBytes(dialog.FileName, _qrBytes);
                MessageBox.Show("QR descargado correctamente.");
            }
        }

        private void PrintQr()
        {
            if (QrImage == null)
            {
                MessageBox.Show("Primero genera el QR.");
                return;
            }

            var image = new System.Windows.Controls.Image
            {
                Source = QrImage,
                Width = 320,
                Height = 320,
                Margin = new Thickness(40)
            };

            var printDialog = new PrintDialog();

            if (printDialog.ShowDialog() == true)
            {
                printDialog.PrintVisual(image, $"QR Habitación {_room.numRoom}");
            }
        }

        private BitmapImage ByteArrayToBitmapImage(byte[] bytes)
        {
            using var stream = new MemoryStream(bytes);

            var image = new BitmapImage();
            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.StreamSource = stream;
            image.EndInit();
            image.Freeze();

            return image;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
