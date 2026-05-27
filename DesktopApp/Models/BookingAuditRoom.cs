using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DesktopApp.Models
{
    public class BookingAuditRoom
    {
        [JsonPropertyName("roomId")]
        public string RoomId { get; set; }

        [JsonPropertyName("numRoom")]
        public int NumRoom { get; set; }

        [JsonPropertyName("roomType")]
        public string RoomType { get; set; }
    }
}
