using DesktopApp.Commands;
using DesktopApp.Models;
using DesktopApp.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using static DesktopApp.Models.Rooms;

namespace DesktopApp.ViewModels
{
    public class FormInvoiceViewModel : INotifyPropertyChanged
    {
        private readonly ApiClient _api =  ApiClient.Instance;

        public event Action<InvoiceData> InvoiceGenerated;

        private readonly string _reservationId;

        public event PropertyChangedEventHandler? PropertyChanged;

        private readonly Dictionary<string, List<string>> _errors = new();

        public bool HasErrors => _errors.Any();

        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

        public ICommand SaveCommand { get; }
        public ICommand LimpiarCommand { get; }
        public ICommand CancelCommand { get; }

        public string CanSaveMessage
        {
            get
            {
                if (string.IsNullOrWhiteSpace(DirrecionHotel)) return "La dirección del hotel es obligatoria.";
                if (string.IsNullOrWhiteSpace(CIFHotel)) return "El CIF del hotel es obligatorio.";
                if (string.IsNullOrWhiteSpace(PhoneHotel)) return "El teléfono del hotel es obligatorio.";
                if (string.IsNullOrWhiteSpace(EmailHotel)) return "El email del hotel es obligatorio.";

                if (string.IsNullOrWhiteSpace(NameCliente)) return "El nombre del cliente es obligatorio.";
                if (string.IsNullOrWhiteSpace(DirrecionCliente)) return "La dirección del cliente es obligatoria.";
                if (string.IsNullOrWhiteSpace(CiudadCliente)) return "La ciudad del cliente es obligatoria.";
                if (string.IsNullOrWhiteSpace(DNICliente)) return "El DNI/CIF del cliente es obligatorio.";
                if (string.IsNullOrWhiteSpace(EmailCliente)) return "El email del cliente es obligatorio.";

                if (HasErrors) return "Corrige los campos marcados en rojo.";
                return "Todo correcto.";
            }
        }

        private string _dirrecionHotel;
        public string DirrecionHotel
        {
            get => _dirrecionHotel;
            set
            {
                _dirrecionHotel = value;
                OnPropertyChanged();

                if (string.IsNullOrWhiteSpace(value))
                    SetErrors(nameof(DirrecionHotel), "La dirección del hotel es obligatoria.");
                else
                    SetErrors(nameof(DirrecionHotel));

                RefreshCanSave();
            }
        }

        private string _cifHotel;
        public string CIFHotel
        {
            get => _cifHotel;
            set
            {
                _cifHotel = value;
                OnPropertyChanged();

                if (string.IsNullOrWhiteSpace(value))
                    SetErrors(nameof(CIFHotel), "El CIF es obligatorio.");
                else
                    SetErrors(nameof(CIFHotel));

                RefreshCanSave();
            }
        }

        private string _phoneHotel;
        public string PhoneHotel
        {
            get => _phoneHotel;
            set
            {
                _phoneHotel = value;
                OnPropertyChanged();

                if (string.IsNullOrWhiteSpace(value))
                    SetErrors(nameof(PhoneHotel), "El teléfono es obligatorio.");
                else if (!value.All(char.IsDigit))
                    SetErrors(nameof(PhoneHotel), "El teléfono solo puede contener números.");
                else
                    SetErrors(nameof(PhoneHotel));

                RefreshCanSave();
            }
        }

        private string _emailHotel;
        public string EmailHotel
        {
            get => _emailHotel;
            set
            {
                _emailHotel = value;
                OnPropertyChanged();

                ValidateEmail(nameof(EmailHotel), value);
                RefreshCanSave();
            }
        }

        private string _nameCliente;
        public string NameCliente
        {
            get => _nameCliente;
            set
            {
                _nameCliente = value;
                OnPropertyChanged();

                if (string.IsNullOrWhiteSpace(value))
                    SetErrors(nameof(NameCliente), "El nombre del cliente es obligatorio.");
                else
                    SetErrors(nameof(NameCliente));

                RefreshCanSave();
            }
        }

        private string _emailCliente;
        public string EmailCliente
        {
            get => _emailCliente;
            set
            {
                _emailCliente = value;
                OnPropertyChanged();

                ValidateEmail(nameof(EmailCliente), value);
                RefreshCanSave();
            }
        }

        private string _ciudadCliente;
        public string CiudadCliente
        {
            get => _ciudadCliente;
            set
            {
                _ciudadCliente = value;
                OnPropertyChanged();

                if (string.IsNullOrWhiteSpace(value))
                    SetErrors(nameof(CiudadCliente), "La ciudad es obligatoria.");
                else
                    SetErrors(nameof(CiudadCliente));

                RefreshCanSave();
            }
        }

        private string _dniCliente;
        public string DNICliente
        {
            get => _dniCliente;
            set
            {
                _dniCliente = value;
                OnPropertyChanged();

                if (string.IsNullOrWhiteSpace(value))
                    SetErrors(nameof(DNICliente), "El DNI/CIF es obligatorio.");
                else
                    SetErrors(nameof(DNICliente));

                RefreshCanSave();
            }
        }

        private string _dirrecionCliente;
        public string DirrecionCliente
        {
            get => _dirrecionCliente;
            set
            {
                _dirrecionCliente = value;
                OnPropertyChanged();

                if (string.IsNullOrWhiteSpace(value))
                    SetErrors(nameof(DirrecionCliente), "La dirección del cliente es obligatoria.");
                else
                    SetErrors(nameof(DirrecionCliente));

                RefreshCanSave();
            }
        }

        private Rooms? _selectedRoom;
        public Rooms? SelectedRoom
        {
            get => _selectedRoom;
            set { _selectedRoom = value; OnPropertyChanged(); }
        }

        public FormInvoiceViewModel(string reservationId)
        {
            _reservationId = reservationId;

            SaveCommand = new RelayCommand(_ => GenerateInvoice(), _ => CanSave());
            LimpiarCommand = new RelayCommand(_ => Clean());
            CancelCommand = new RelayCommand(w => CloseWindow(w as Window));

            _ = LoadInvoiceData();
        }


        private async Task LoadInvoiceData()
        {
            try
            {
                var data = await _api.GetInvoiceDataAsync(_reservationId);

                DirrecionHotel = data.Hotel?.Address ?? "";
                CIFHotel = data.Hotel?.TaxId ?? "";
                PhoneHotel = data.Hotel?.Phone ?? "";
                EmailHotel = data.Hotel?.Email ?? "";

                NameCliente = data.Client?.Name ?? "";
                EmailCliente = data.Client?.Email ?? "";
                CiudadCliente = data.Client?.City ?? "";
                DNICliente = data.Client?.Dni ?? "";
                DirrecionCliente = data.Client?.Address ?? "";

                RefreshCanSave();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error cargando datos de factura");
            }
        }

        private bool CanSave()
        {
            return !string.IsNullOrWhiteSpace(DirrecionHotel)
                && !string.IsNullOrWhiteSpace(CIFHotel)
                && !string.IsNullOrWhiteSpace(PhoneHotel)
                && !string.IsNullOrWhiteSpace(EmailHotel)
                && !string.IsNullOrWhiteSpace(NameCliente)
                && !string.IsNullOrWhiteSpace(DirrecionCliente)
                && !string.IsNullOrWhiteSpace(CiudadCliente)
                && !string.IsNullOrWhiteSpace(DNICliente)
                && !string.IsNullOrWhiteSpace(EmailCliente)
                && !HasErrors;
        }

        private void GenerateInvoice()
        {
            var data = new InvoiceData
            {
                Hotel = new InvoiceHotel
                {
                    Address = DirrecionHotel,
                    TaxId = CIFHotel,
                    Phone = PhoneHotel,
                    Email = EmailHotel
                },
                Client = new InvoiceClient
                {
                    Name = NameCliente,
                    Email = EmailCliente,
                    City = CiudadCliente,
                    Dni = DNICliente,
                    Address = DirrecionCliente
                }
            };

            InvoiceGenerated?.Invoke(data);
        }

        private void Clean()
        {
            NameCliente = "";
            EmailCliente = "";
            CiudadCliente = "";
            DNICliente = "";
            DirrecionCliente = "";

            DirrecionHotel = "";
            CIFHotel = "";
            PhoneHotel = "";
            EmailHotel = "";

            RefreshCanSave();
        }

        private void ValidateEmail(string propertyName, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                SetErrors(propertyName, "El email es obligatorio.");
            else if (!value.Contains("@") || !value.Contains("."))
                SetErrors(propertyName, "El email no tiene un formato válido.");
            else
                SetErrors(propertyName);
        }

        public System.Collections.IEnumerable GetErrors(string? propertyName)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
                return _errors.SelectMany(e => e.Value);

            return _errors.TryGetValue(propertyName, out var list)
                ? list
                : Enumerable.Empty<string>();
        }

        private void SetErrors(string propertyName, params string[] errors)
        {
            if (errors == null || errors.Length == 0)
            {
                if (_errors.Remove(propertyName))
                    ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
            }
            else
            {
                _errors[propertyName] = errors.ToList();
                ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
            }

            OnPropertyChanged(nameof(HasErrors));
            OnPropertyChanged(nameof(CanSaveMessage));
            CommandManager.InvalidateRequerySuggested();
        }

        private void RefreshCanSave()
        {
            OnPropertyChanged(nameof(CanSaveMessage));
            CommandManager.InvalidateRequerySuggested();
        }

        private void CloseWindow(Window? w)
        {
            w?.Close();
        }
        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }


}
