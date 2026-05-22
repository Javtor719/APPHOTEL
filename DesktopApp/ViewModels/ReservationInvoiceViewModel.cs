using DesktopApp.Commands;
using DesktopApp.Models;
using DesktopApp.Services;
using DesktopApp.Views;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace DesktopApp.ViewModels
{
    public class ReservationInvoiceViewModel : INotifyPropertyChanged
    {
        private readonly ApiClient _apiClient = ApiClient.Instance;

        private InvoiceData _customInvoiceData;

        private Reservations _selectedReservation;
        public Reservations SelectedReservation
        {
            get => _selectedReservation;
            set { _selectedReservation = value; OnPropertyChanged(); }
        }

        private string _pdfPath;

        public string PdfPath
        {
            get => _pdfPath;
            set
            {
                _pdfPath = value;
                OnPropertyChanged();
            }
        }

        public ICommand DownloadCommand { get; }
        public ICommand SendEmailCommand { get; }
        public ICommand ConfigHeaderCommand { get; }
        public ReservationInvoiceViewModel(ReservationListViewModel vm)
        {
            DownloadCommand = new RelayCommand(_ => DownloadPdf());

            ConfigHeaderCommand = new RelayCommand(_ => OpenHeaderConfig());

            SendEmailCommand = new RelayCommand(_ => SendEmailToClient());
        }

        public async Task LoadInvoice(Reservations reservation, ReservationListViewModel vm)
        {
            try
            {
                SelectedReservation = reservation;

                byte[] pdfBytes = await _apiClient.GetInvoicePdfAsync(reservation.Id);

                await vm.CargarReservasAsync();

                var updated = vm.Reservas.FirstOrDefault(r => r.Id == reservation.Id);
                if (updated != null)
                    SelectedReservation = updated;

                var invoiceNumber = string.IsNullOrWhiteSpace(SelectedReservation.InvoiceNumber)
                    ? SelectedReservation.Id
                    : SelectedReservation.InvoiceNumber;

                string path = Path.Combine(
                    Path.GetTempPath(),
                    $"factura-{invoiceNumber}.pdf"
                );

                await File.WriteAllBytesAsync(path, pdfBytes);

                PdfPath = path;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error");
            }
        }

        private void DownloadPdf()
        {
            if (string.IsNullOrEmpty(PdfPath) || !File.Exists(PdfPath))
            {
                MessageBox.Show("La factura todavía no está cargada.");
                return;
            }

            SaveFileDialog dialog = new SaveFileDialog
            {
                FileName =  Path.GetFileName(PdfPath),
                Filter = "PDF (*.pdf)|*.pdf"
            };

            if (dialog.ShowDialog() == true)
            {
                File.Copy(PdfPath, dialog.FileName, true);
                MessageBox.Show("Factura descargada correctamente.");
            }
        }

        private async void OpenHeaderConfig()
        {
            var win = new DesktopApp.Views.Reservation.InvoiceHeaderConfigWindow(SelectedReservation);
            win.Owner = Application.Current.MainWindow;

            if (win.ShowDialog() == true && win.InvoiceDataResult != null)
            {
                byte[] pdfBytes = await _apiClient.PostInvoicePdfAsync(
                    SelectedReservation.Id,
                    _customInvoiceData = win.InvoiceDataResult
                );

                string path = Path.Combine(
                    Path.GetTempPath(),
                    $"factura-preview-{SelectedReservation.InvoiceNumber}.pdf"
                );

                await File.WriteAllBytesAsync(path, pdfBytes);
                PdfPath = path;
            }
        }

        private void SendEmailToClient()
        {
            if (SelectedReservation == null)
            {
                MessageBox.Show("No hay reserva seleccionada.");
                return;
            }

            var win = new DesktopApp.Views.SendInvoiceEmailWindow(SelectedReservation, _customInvoiceData);
            win.Owner = Application.Current.MainWindow;
            win.ShowDialog();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
