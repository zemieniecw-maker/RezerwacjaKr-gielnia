using Microsoft.EntityFrameworkCore;
using RezerwacjaKręgielnia.Data;
using RezerwacjaKręgielnia.Entities;

namespace RezerwacjaKręgielnia.Services
{
    public class ReservationService
    {
        private readonly BowlingDbContext _context;

        // Wstrzykiwanie zależności (Dependency Injection) - Wymóg architektury
        public ReservationService(BowlingDbContext context)
        {
            _context = context;
        }

        // --- FUNKCJA 1: Logika Rezerwacji (Walidacja biznesowa - 5 pkt) ---
        public async Task<string> BookLaneAsync(int laneId, DateTime date, int players, string userId, int duration)
        {
            if (date < DateTime.Now) return "Nie można rezerwować wstecz.";

            // Obliczamy kiedy kończy się nowa rezerwacja
            DateTime newReservationEnd = date.AddHours(duration);

            // LOGIKA KONFLIKTÓW (OVERLAP):
            // Sprawdzamy, czy jakakolwiek istniejąca rezerwacja nakłada się na nową.
            // Wzór: (StartA < KoniecB) ORAZ (KoniecB > StartA)
            bool isTaken = await _context.Reservations
                .AnyAsync(r => r.LaneId == laneId &&
                               r.Date < newReservationEnd &&
                               r.Date.AddHours(r.Duration) > date);

            if (isTaken) return "W tym czasie tor jest zajęty przez inną rezerwację.";

            // Zapisz z czasem trwania
            var reservation = new ReservationEntity
            {
                LaneId = laneId,
                Date = date,
                NumberOfPlayers = players,
                UserId = userId,
                Duration = duration // Zapisujemy czas
            };

            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();

            return "OK";
        }

        // --- FUNKCJA 2: Pobieranie rezerwacji zalogowanego użytkownika ---
        public async Task<List<ReservationEntity>> GetMyReservationsAsync(string userId)
        {
            return await _context.Reservations
                .Include(r => r.Lane) // Dołączamy dane toru, żeby wyświetlić numer
                .Where(r => r.UserId == userId) // FILTROWANIE po ID użytkownika (Bezpieczeństwo)
                .OrderByDescending(r => r.Date)
                .ToListAsync();
        }

        // --- FUNKCJA 3: Statystyki (Zaawansowany ORM - 5 pkt) ---
        // Używamy funkcji agregujących SumAsync i CountAsync
        public async Task<int> GetTotalReservationsCountAsync()
        {
            return await _context.Reservations.CountAsync();
        }

        public async Task<decimal> GetTotalRevenueAsync()
        {
            // POPRAWKA DLA SQLite:
            // Najpierw pobieramy dane do pamięci (.ToListAsync),
            // a potem sumujemy w C# (.Sum), bo SQLite nie umie sumować Decimali.

            var reservations = await _context.Reservations
                .Include(r => r.Lane)
                .ToListAsync();

            return reservations.Sum(r => r.Lane.PricePerHour);
        }

        // --- FUNKCJA 4: Dla Administratora (Pobierz wszystko) ---
        public async Task<List<ReservationEntity>> GetAllReservationsAsync()
        {
            return await _context.Reservations
                .Include(r => r.Lane)
                .Include(r => r.User) // Admin musi widzieć KTO zarezerwował
                .OrderByDescending(r => r.Date)
                .ToListAsync();
        }

        // --- FUNKCJA 5: Grupowanie (Wymóg na 5 pkt za ORM) ---
        public async Task<Dictionary<int, int>> GetLanePopularityAsync()
        {
            // Grupujemy rezerwacje po ID toru i liczymy ile razy każdy był wybrany
            return await _context.Reservations
                .GroupBy(r => r.Lane.LaneNumber)
                .Select(g => new { LaneNumber = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.LaneNumber, x => x.Count);
        }
    }
}