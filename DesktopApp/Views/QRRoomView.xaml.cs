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
using DesktopApp.Models;
using DesktopApp.ViewModels;

namespace DesktopApp.Views
{
    /// <summary>
    /// Lógica de interacción para QRRoomView.xaml
    /// </summary>
    public partial class QRRoomView : Window
    {
        public QRRoomView(Rooms room)
        {
            InitializeComponent();
            DataContext = new QRRoomViewModel(room);
        }
    }
}
