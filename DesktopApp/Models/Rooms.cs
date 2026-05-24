using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DesktopApp.Models
{
    public class Rooms: INotifyPropertyChanged
    {
        [JsonPropertyName("_id")]
        public string Id { get; set; }
        public int numRoom { get; set; }
        public int numFloor { get; set; }
        public enum RoomType { Single, Double, Triple, Fourfold }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public RoomType roomType { get; set; }
        public string description { get; set; }
        public List<string> image { get; set; } = new();

        public float pricePerNight { get; set; }
        public int maxOccupancy { get; set; }
        public enum Availability { Available, Unavailable, Block }

        public List<string> services { get; set; } = new();

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Availability availability { get; set; }

        [JsonPropertyName("qrVersion")]
        public int QrVersion { get; set; }

        private string _effectiveAvailability;
        public string EffectiveAvailability
        {
            get => _effectiveAvailability;
            set
            {
                _effectiveAvailability = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}