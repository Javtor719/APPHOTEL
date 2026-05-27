using DesktopApp.Models;
using DesktopApp.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DesktopApp.Views.Reservation
{
    /// <summary>
    /// Lógica de interacción para ReservationInvoicePDF.xaml
    /// </summary>
    public partial class ReservationInvoicePDF : Window
    {
        private readonly ReservationInvoiceViewModel _vm;
        public ReservationInvoicePDF(Reservations reservation, ReservationListViewModel vm)
        {
            InitializeComponent();

            _vm = new ReservationInvoiceViewModel(vm);

            DataContext = _vm;

            Title = $"Factura Reserva {reservation.ReservationNumber}";

            _vm.PropertyChanged += Vm_PropertyChanged;

            Loaded += async (_, __) => await _vm.LoadInvoice(reservation,vm);
        }

        private async void Vm_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(_vm.PdfPath) &&
                !string.IsNullOrEmpty(_vm.PdfPath))
            {
                await PdfBrowser.EnsureCoreWebView2Async();

                PdfBrowser.CoreWebView2.Navigate(
                    new Uri(_vm.PdfPath).AbsoluteUri
                );
            }
        }
    }
}
