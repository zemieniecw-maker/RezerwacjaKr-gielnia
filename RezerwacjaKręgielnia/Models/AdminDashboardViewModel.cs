using RezerwacjaKręgielnia.Entities;

namespace RezerwacjaKręgielnia.Models
{
    public class AdminDashboardViewModel
    {
        public int TotalReservations { get; set; }
        public decimal TotalRevenue { get; set; }
        public List<ReservationEntity> Reservations { get; set; } = new();
        public Dictionary<int, int> LaneStats { get; set; } = new();
    }
}