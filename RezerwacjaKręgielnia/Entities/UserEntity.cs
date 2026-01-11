using Microsoft.AspNetCore.Identity;

namespace RezerwacjaKręgielnia.Entities
{
    public class UserEntity : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        // Relacja: Jeden użytkownik ma wiele rezerwacji
        public virtual ICollection<ReservationEntity> Reservations { get; set; }
    }
}