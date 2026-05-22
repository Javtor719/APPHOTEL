using DesktopApp.Commands;
using DesktopApp.Models;
using DesktopApp.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Mail;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace DesktopApp.ViewModels
{
    public class SendInvoiceEmailViewModel : INotifyPropertyChanged
    {
        private readonly ApiClient _api = ApiClient.Instance;
        private readonly Reservations _reservation;

        private InvoiceData _customInvoiceData;

        private string _emailDestino;
        public string EmailDestino
        {
            get => _emailDestino;
            set
            {
                _emailDestino = value;
                OnPropertyChanged();
                ErrorMessage = "";
                CommandManager.InvalidateRequerySuggested();
            }
        }

        private string _errorMessage;
        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                _errorMessage = value;
                OnPropertyChanged();
            }
        }

        public ICommand SendCommand { get; }
        public ICommand CancelCommand { get; }

        public SendInvoiceEmailViewModel(Reservations reservation, InvoiceData customInvoiceData)
        {
            _reservation = reservation;
            _customInvoiceData = customInvoiceData;

            SendCommand = new RelayCommand(async w => await SendAsync(w as Window), _ => CanSend());
            CancelCommand = new RelayCommand(w => (w as Window)?.Close());

            _ = LoadClientEmailAsync();
        }

        private async Task LoadClientEmailAsync()
        {
            try
            {
                var user = await _api.GetUserByIdOrDniAsync(_reservation.User, "id");
                EmailDestino = user?.Email ?? "";
            }
            catch (Exception ex)
            {
                ErrorMessage = "No se pudo cargar el email del cliente: " + ex.Message;
            }
        }

        private bool CanSend()
        {
            return IsValidEmail(EmailDestino);
        }

        private async Task SendAsync(Window window)
        {
            if (!IsValidEmail(EmailDestino))
            {
                ErrorMessage = "Introduce un email válido.";
                return;
            }

            try
            {
                await _api.SendInvoiceEmailAsync(_reservation.Id, EmailDestino, _customInvoiceData);
                MessageBox.Show("Factura enviada correctamente.");
                window?.Close();
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
        }

        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                var mail = new MailAddress(email);
                return mail.Address == email;
            }
            catch
            {
                return false;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
