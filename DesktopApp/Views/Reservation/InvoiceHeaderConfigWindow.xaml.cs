using DesktopApp.Models;
using DesktopApp.ViewModels;
using System;
using System.Collections.Generic;
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
    /// Lógica de interacción para InvoiceHeaderConfigWindow.xaml
    /// </summary>
    public partial class InvoiceHeaderConfigWindow : Window
    {
        public InvoiceData InvoiceDataResult { get; private set; }
        public InvoiceHeaderConfigWindow(Reservations reservation)
        {
            InitializeComponent();
            var vm = new FormInvoiceViewModel(reservation.Id);
            vm.InvoiceGenerated += data =>
            {
                InvoiceDataResult = data;
                DialogResult = true;
                Close();
            };

            DataContext = vm;
        }
    }
}
