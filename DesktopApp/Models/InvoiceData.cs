using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopApp.Models
{
    public class InvoiceData
    {
        public InvoiceHotel Hotel { get; set; }
        public InvoiceClient Client { get; set; }
        public Reservations Reservation { get; set; }
    }

    public class InvoiceHotel
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string TaxId { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
    }

    public class InvoiceClient
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public string Dni { get; set; }
    }
}
