using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;

namespace DesktopApp.Models
{
    public class Reservations
    {
        [JsonPropertyName("reservationNumber")]
        public string ReservationNumber { get; set; }

        [JsonPropertyName("_id")]
        public string Id { get; set; }

        [JsonPropertyName("userId")]
        public string User { get; set; }

        [JsonIgnore] // No se envía al backend
        public List<Rooms> Rooms { get; set; } = new List<Rooms>();

        [JsonPropertyName("roomIds")]
        public List<string> RoomIds { get; set; } = new List<string>();

        [JsonPropertyName("checkIn")]
        public DateTime CheckIn { get; set; }

        [JsonPropertyName("checkOut")]
        public DateTime CheckOut { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("totalPrice")]
        public float TotalPrice { get; set; }

        [JsonPropertyName("numGuests")] 
        public int NumGuests { get; set; }

        [JsonPropertyName("invoiceNumber")]
        public string InvoiceNumber { get; set; }

        [JsonIgnore]
        public string UserDNI { get; set; }

        [JsonIgnore]
        public string UserNombre { get; set; }

        public string RoomNumbers
        {
            get
            {
                return Rooms != null && Rooms.Any()
                    ? string.Join(", ", Rooms.Select(r => r.numRoom))
                    : "";
            }
        }

        public bool CanInvoice =>
            Status == "checkIn" ||
            Status == "checkOut" ||
            Status == "facturada";

        public double InvoiceOpacity =>
            CanInvoice ? 1.0 : 0.3;
    }
}
