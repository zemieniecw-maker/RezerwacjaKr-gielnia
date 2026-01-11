using RezerwacjaKręgielnia.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace RezerwacjaKręgielnia.Data
{
    // Dziedziczymy z IdentityDbContext, żeby obsłużyć logowanie
    public class BowlingDbContext : IdentityDbContext<UserEntity>
    {
        public BowlingDbContext(DbContextOptions<BowlingDbContext> options) : base(options)
        {
        }

        public DbSet<LaneEntity> Lanes { get; set; }
        public DbSet<ReservationEntity> Reservations { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Seedowanie danych: Automatycznie dodamy 5 torów do bazy przy starcie
            builder.Entity<LaneEntity>().HasData(
                new LaneEntity { Id = 1, LaneNumber = 1, PricePerHour = 50 },
                new LaneEntity { Id = 2, LaneNumber = 2, PricePerHour = 50 },
                new LaneEntity { Id = 3, LaneNumber = 3, PricePerHour = 50 },
                new LaneEntity { Id = 4, LaneNumber = 4, PricePerHour = 50 },
                new LaneEntity { Id = 5, LaneNumber = 5, PricePerHour = 80 } // Tor VIP
            );
        }
    }
}