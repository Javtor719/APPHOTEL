using DesktopApp.Models;
using DesktopApp.Services;
using DesktopApp.ViewModels;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace DesktopApp.Views.Reservation
{
    public partial class ListReservationView : UserControl
    {
        private readonly ApiClient _apiClient;
        private readonly ReservationListViewModel _vm = new ReservationListViewModel();


        public ListReservationView()
        {
            InitializeComponent();
            DataContext =  _vm;

        }

        private async void Eliminar_Reserva(object sender, RoutedEventArgs e)
        {
            var form = new DeleteCancelledReservationsView();
            form.Owner = Application.Current.MainWindow;
            var ok = form.ShowDialog();
            await _vm.CargarReservasAsync();
        }


        private async void Nueva_Reserva(object sender, RoutedEventArgs e)
        {

            var form = new AddReservationView();
            form.Owner = Application.Current.MainWindow;
            var ok = form.ShowDialog();
            await _vm.CargarReservasAsync();
        }

        private void History_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button)?.CommandParameter is not Reservations reservation)
            {
                MessageBox.Show("Selecciona una reserva.");
                return;
            }

            var win = new ReservationsHistory(reservation.ReservationNumber, reservation.Id);
            win.Owner = Application.Current.MainWindow;
            win.ShowDialog();
        }
        private async void PDF_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button)?.CommandParameter is not Reservations reservation)
            {
                MessageBox.Show("Selecciona una reserva.");
                return;
            }

            var win = new ReservationInvoicePDF(reservation, _vm);
            win.Owner = Application.Current.MainWindow;
            win.ShowDialog();

            await _vm.CargarReservasAsync();
        }
        
    }
}
