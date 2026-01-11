namespace RezerwacjaKręgielnia.Entities
{
    public class LaneEntity
    {
        public int Id { get; set; }
        public int LaneNumber { get; set; } // Numer toru (np. 1, 2, 3)
        public decimal PricePerHour { get; set; } // Cena

        // Relacja: Jeden tor ma historię wielu rezerwacji
        public virtual ICollection<ReservationEntity> Reservations { get; set; }
    }
}