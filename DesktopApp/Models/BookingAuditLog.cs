using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DesktopApp.Models
{
    public class BookingAuditLog
    {
        [JsonPropertyName("_id")]
        public string Id { get; set; }

        [JsonPropertyName("bookingId")]
        public string BookingId { get; set; }

        [JsonPropertyName("action")]
        public string Action { get; set; }

        [JsonPropertyName("actorId")]
        public string ActorId { get; set; }

        [JsonPropertyName("actorType")]
        public string ActorType { get; set; }

        [JsonPropertyName("previousState")]
        public BookingAuditState PreviousState { get; set; }

        [JsonPropertyName("newState")]
        public BookingAuditState NewState { get; set; }

        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; }

        [JsonIgnore]
        public string UserDNI { get; set; }

        [JsonIgnore]
        public string UserNombre { get; set; }
        
    }
}
