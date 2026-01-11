namespace RezerwacjaKręgielnia.Entities
{
    public class ReservationEntity
    {
        public int Id { get; set; }
        public DateTime Date { get; set; } // Data i godzina rezerwacji
        public int NumberOfPlayers { get; set; }

        public int Duration { get; set; } = 1;

        // Relacja do Toru
        public int LaneId { get; set; }
        public virtual LaneEntity Lane { get; set; }

        // Relacja do Użytkownika
        public string UserId { get; set; }
        public virtual UserEntity User { get; set; }
    }
}