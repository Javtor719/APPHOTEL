using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DesktopApp.Models
{
    public class RoomCalendarResponse
    {
        public string RoomId { get; set; }
        public int NumRoom { get; set; }
        public string Month { get; set; }
        public string Availability { get; set; }

        public Dictionary<string, RoomCalendarDay> Calendar { get; set; } = new();
    }

    public class RoomCalendarDay
    {
        public string Status { get; set; }
        public List<RoomCalendarReservation> Reservations { get; set; } = new();
        public List<RoomCalendarBlock> Blocks { get; set; } = new();
    }

    public class RoomCalendarReservation
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        public string ReservationNumber { get; set; }
        public string Status { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
    }

    public class RoomCalendarBlock
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        public string Type { get; set; }
        public string Reason { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
