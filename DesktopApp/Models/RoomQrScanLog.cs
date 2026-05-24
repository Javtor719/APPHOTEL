using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DesktopApp.Models
{
    public  class RoomQrScanLog
    {
        [JsonPropertyName("_id")]
        public string Id { get; set; }

        [JsonPropertyName("roomId")]
        public string RoomId { get; set; }

        [JsonPropertyName("numRoom")]
        public int? NumRoom { get; set; }

        [JsonPropertyName("actorId")]
        public string ActorId { get; set; }

        [JsonPropertyName("actorRole")]
        public string ActorRole { get; set; }

        [JsonPropertyName("result")]
        public string Result { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("ip")]
        public string Ip { get; set; }

        [JsonPropertyName("userAgent")]
        public string UserAgent { get; set; }

        [JsonPropertyName("scannedAt")]
        public DateTime ScannedAt { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string NombreCompleto => $"{FirstName} {LastName}";

    }
}
