using DesktopApp.Commands;
using DesktopApp.Models;
using DesktopApp.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DesktopApp.ViewModels
{
    public class ReservationListViewModel : INotifyPropertyChanged
    {
        private readonly ApiClient _apiClient;

        public ReservationListViewModel()
        {
            _apiClient = new ApiClient();

            LimpiarCommand = new RelayCommand(_ => LimpiarFiltro());
            LimpiarLogCommand = new RelayCommand(_ => LimpiarFiltroLog());
            NuevaReservaCommand = new RelayCommand(_ => NuevaReserva());
            EliminarReservaCommand = new RelayCommand(_ => EliminarReserva());
            CancelarReservaCommand = new RelayCommand(async _ => await CancelarReservaAsync());

            _ = CargarReservasAsync();
        }

        public ObservableCollection<Reservations> Reservas { get; } = new();

        private ObservableCollection<Reservations> _todas;
        public ObservableCollection<BookingAuditLog> AuditLog { get; } = new();

        private ObservableCollection<BookingAuditLog> _todasAuditLog;        

        private BookingAuditLog _reservaAuditLog;
        public BookingAuditLog ReservaAuditLog
        {
            get => _reservaAuditLog;
            set { _reservaAuditLog = value; OnPropertyChanged(); }
        }

        private Reservations _selectedReservation;
        public Reservations SelectedReservation
        {
            get => _selectedReservation;
            set { _selectedReservation = value; OnPropertyChanged(); }
        }

        private Reservations _reservaSeleccionada;
        public Reservations ReservaSeleccionada
        {
            get => _reservaSeleccionada;
            set
            {
                _reservaSeleccionada = value;
                OnPropertyChanged();
                CommandManager.InvalidateRequerySuggested();
            }
        }

        private string _textoBusqueda;
        public string TextoBusqueda
        {
            get => _textoBusqueda;
            set
            {
                _textoBusqueda = value;
                OnPropertyChanged();
                AplicarFiltro();
            }
        }

        private bool _ocultarCanceladas = true;
        public bool OcultarCanceladas
        {
            get => _ocultarCanceladas;
            set
            {
                _ocultarCanceladas = value;
                OnPropertyChanged();
                AplicarFiltro();
            }
        }

        private bool _ocultarTerminadas = true;
        public bool OcultarTerminadas
        {
            get => _ocultarTerminadas;
            set
            {
                _ocultarTerminadas = value;
                OnPropertyChanged();
                AplicarFiltro();
            }
        }

        private bool _mostrarConfirmadas = false;
        public bool MostrarConfirmadas
        {
            get => _mostrarConfirmadas;
            set
            {
                _mostrarConfirmadas = value;
                OnPropertyChanged();
                AplicarFiltroAuditLog();
            }
        }

        private bool _mostrarCanceladas = false;
        public bool MostrarCanceladas
        {
            get => _mostrarCanceladas;
            set
            {
                _mostrarCanceladas = value;
                OnPropertyChanged();
                AplicarFiltroAuditLog();
            }
        }

        private bool _mostrarCheckIn = false;
        public bool MostrarCheckIn
        {
            get => _mostrarCheckIn;
            set
            {
                _mostrarCheckIn = value;
                OnPropertyChanged();
                AplicarFiltroAuditLog();
            }
        }

        private bool _mostrarCheckOut = false;
        public bool MostrarCheckOut
        {
            get => _mostrarCheckOut;
            set
            {
                _mostrarCheckOut = value;
                OnPropertyChanged();
                AplicarFiltroAuditLog();
            }
        }

        public ICommand NuevaReservaCommand { get; }
        public ICommand CancelarReservaCommand { get; }
        public ICommand LimpiarCommand { get; }

        public ICommand LimpiarLogCommand { get; }
        public ICommand EliminarReservaCommand { get; }

        public async Task CargarReservasAsync()
        {
            try
            {
                var reservations = await _apiClient.GetReservasAsync();
                var usuarios = await _apiClient.GetUsersByRolAsync("Usuario");

                foreach (var reservation in reservations)
                {
                    var rooms = await Task.WhenAll(
                        reservation.RoomIds.Select(id => _apiClient.GetRoomsId(id))
                    );

                    reservation.Rooms = rooms.Where(r => r != null).ToList();

                    var user = usuarios.FirstOrDefault(u => u.Id == reservation.User);
                    if (user != null)
                    {
                        reservation.UserDNI = user.DNI;
                        reservation.UserNombre = user.NombreCompleto;
                    }
                }

                _todas = new ObservableCollection<Reservations>(reservations);
                AplicarFiltro();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar reservas: " + ex.Message);
            }
        }

        public async Task LoadHistory(string reservationId)
        {
            try
            {
                var historial = await _apiClient.GetHistoryReservation(reservationId);

                AuditLog.Clear();
                
                foreach (var item in historial)
                {
                    var user = await _apiClient.GetUserByIdOrDniAsync(item.ActorId, "id");

                    if (user != null)
                    {
                        item.UserDNI = user.DNI;
                        item.UserNombre = user.NombreCompleto;
                    }
                    else
                    {
                        item.UserDNI = item.ActorId;
                        item.UserNombre = item.ActorType;
                    }
                        

                    AuditLog.Add(item);
                    _todasAuditLog = new ObservableCollection<BookingAuditLog>(historial);
                    AplicarFiltroAuditLog();
                }
                    
            }
            catch (System.Exception e)
            {
                MessageBox.Show(e.Message, "Error");
            }
        }
        private void AplicarFiltroAuditLog()
        {
            if (_todasAuditLog == null) return;

            var filtradasLog = _todasAuditLog.AsEnumerable();

            if (!MostrarConfirmadas &&
                !MostrarCanceladas &&
                !MostrarCheckIn &&
                !MostrarCheckOut)
            {
                AuditLog.Clear();

                foreach (var item in _todasAuditLog)
                    AuditLog.Add(item);

                return;
            }

            filtradasLog = filtradasLog.Where(r =>
                (MostrarConfirmadas && r.Action.Equals("confirmada", StringComparison.OrdinalIgnoreCase)) ||

                (MostrarCanceladas && r.Action.Equals("cancelada", StringComparison.OrdinalIgnoreCase)) ||

                (MostrarCheckIn && r.Action.Equals("checkIn", StringComparison.OrdinalIgnoreCase)) ||

                (MostrarCheckOut && r.Action.Equals("checkOut", StringComparison.OrdinalIgnoreCase))
            );

            AuditLog.Clear();

            foreach (var r in filtradasLog)
                AuditLog.Add(r);
        }
        private void AplicarFiltro()
        {
            if (_todas == null) return;

            var filtradas = _todas.AsEnumerable();

            //Buscador Global (Habitación, Nombre o DNI)
            if (!string.IsNullOrWhiteSpace(TextoBusqueda))
            {
                string busqueda = TextoBusqueda.ToLower().Trim();

                filtradas = filtradas.Where(r =>
                    // Buscar en Habitaciones
                    (r.Rooms != null && r.Rooms.Any(h => h.numRoom.ToString().Contains(busqueda))) ||

                    // Buscar en el Nombre del Usuario
                    (!string.IsNullOrEmpty(r.UserNombre) && r.UserNombre.ToLower().Contains(busqueda)) ||

                    // Buscar por DNI
                    (!string.IsNullOrEmpty(r.UserDNI) && r.UserDNI.ToLower().Contains(busqueda)) ||

                    // Buscar por Nº de Reserva
                    (!string.IsNullOrEmpty(r.ReservationNumber) && r.ReservationNumber.ToLower().Contains(busqueda))
                );
            }

            // Filtro de Canceladas
            if (OcultarCanceladas)
            {
                filtradas = filtradas.Where(r =>
                    !string.Equals(r.Status, "cancelada", StringComparison.OrdinalIgnoreCase)
                );
            }

            if (OcultarTerminadas)
            {
                filtradas = filtradas.Where(r =>
                    !string.Equals(r.Status, "terminada", StringComparison.OrdinalIgnoreCase)
                );
            }

            // Actualizar la colección de la UI
            // Usamos una lista temporal para evitar múltiples refrescos visuales si la lista es muy grande
            var listaFinal = filtradas.ToList();

            Reservas.Clear();
            foreach (var r in listaFinal)
            {
                Reservas.Add(r);
            }
        }

        private void LimpiarFiltro()
        {
            TextoBusqueda = "";

            OcultarCanceladas = true;

            OcultarTerminadas = true;
            
            AplicarFiltro();
        }

        private void LimpiarFiltroLog()
        {

            MostrarConfirmadas = false;

            MostrarCanceladas = false;

            MostrarCheckIn = false;

            MostrarCheckOut = false;

            AplicarFiltroAuditLog();
        }

        private void NuevaReserva()
        {
            var ventana = new Views.Reservation.AddReservationView();
            ventana.Owner = Application.Current.MainWindow;
            ventana.ShowDialog();
        }

        private void EliminarReserva()
        {
            var ventana = new Views.Reservation.DeleteCancelledReservationsView();
            ventana.Owner = Application.Current.MainWindow;
            ventana.ShowDialog();
        }

        private async Task CancelarReservaAsync()
        {
            if (ReservaSeleccionada == null) return;

            try
            {
                MessageBoxResult messageBoxResult = MessageBox.Show($"¿Seguro que quieres cancelar la reserva Nº: {ReservaSeleccionada.ReservationNumber}?", "Cancelar reserva", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    bool exito = await _apiClient.CancelReservationAsync(ReservaSeleccionada.Id);

                    if (exito)
                    {
                        ReservaSeleccionada.Status = "cancelada";
                        MessageBox.Show($"Reserva Nº: {ReservaSeleccionada.ReservationNumber} cancelada correctamente.");
                        AplicarFiltro();
                    }
                    else
                    {
                        MessageBox.Show($"No se pudo cancelar la reserva Nº: {ReservaSeleccionada.ReservationNumber}.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cancelar reserva: " + ex.Message);
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}

