using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DesktopApp.Models
{
    public class BookingAuditState
    {
        [JsonPropertyName("rooms")]
        public List<BookingAuditRoom> Rooms { get; set; } = new();

        [JsonPropertyName("comentario")]
        public string Comentario { get; set; }

        [JsonPropertyName("checkIn")]
        public DateTime CheckIn { get; set; }

        [JsonPropertyName("checkOut")]
        public DateTime CheckOut { get; set; }

        [JsonPropertyName("guest")]
        public int Guest { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonIgnore]
        public string RoomNumbers =>
           Rooms != null && Rooms.Any()
               ? string.Join(", ", Rooms.Select(r => r.NumRoom))
               : "";

    }
}
