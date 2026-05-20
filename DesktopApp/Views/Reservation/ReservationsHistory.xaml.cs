using DesktopApp.Models;
using DesktopApp.ViewModels;
using System;
using System.Collections.Generic;
using System.IO.Packaging;
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
    /// Lógica de interacción para ReservationsHistory.xaml
    /// </summary>
    public partial class ReservationsHistory : Window
    {
        private readonly ReservationListViewModel _vm = new();
        public ReservationsHistory(string reservationNumber, string reservationId)
        {
            InitializeComponent();
            DataContext = _vm;
            Title = $"Reviews Habitación {reservationNumber}";
            Loaded += async (_, __) => await _vm.LoadHistory(reservationId);
        }
    }
}
