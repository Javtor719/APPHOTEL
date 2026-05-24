using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DesktopApp.Models
{
    public class DashboardStats
    {
        [JsonPropertyName("totalRooms")]
        public int TotalRooms { get; set; }

        [JsonPropertyName("todayCheckIns")]
        public int TodayCheckIns { get; set; }

        [JsonPropertyName("todayCheckOuts")]
        public int TodayCheckOuts { get; set; }

        [JsonPropertyName("activeReservations")]
        public int ActiveReservations { get; set; }
    }
}
