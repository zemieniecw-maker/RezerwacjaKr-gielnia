using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // Potrzebne do pobrania listy torów
using RezerwacjaKręgielnia.Data;
using RezerwacjaKręgielnia.Models;
using RezerwacjaKręgielnia.Services;
using RezerwacjaKręgielnia.Entities; // Dodaj to, jeśli podkreśla UserEntity

namespace RezerwacjaKręgielnia.Controllers
{
    [Authorize] // Tylko zalogowani mogą tu wejść!
    public class ReservationsController : Controller
    {
        private readonly ReservationService _reservationService;
        private readonly UserManager<UserEntity> _userManager;
        private readonly BowlingDbContext _context; // Potrzebne tylko do listy torów w formularzu

        public ReservationsController(ReservationService reservationService, UserManager<UserEntity> userManager, BowlingDbContext context)
        {
            _reservationService = reservationService;
            _userManager = userManager;
            _context = context;
        }

        // 1. Widok listy moich rezerwacji
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var reservations = await _reservationService.GetMyReservationsAsync(userId);
            return View(reservations);
        }

        // 2. Formularz rezerwacji (GET - wyświetl pusty)
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = new BookLaneViewModel
            {
                // Pobieramy listę torów do dropdowna
                AvailableLanes = await _context.Lanes.ToListAsync()
            };
            return View(model);
        }

        // 3. Obsługa wysłania formularza (POST - zapisz)
        [HttpPost]
        public async Task<IActionResult> Create(BookLaneViewModel model)
        {
            // Jeśli walidacja modelu nie przeszła (np. brak liczby graczy), wróć
            // Usuwamy walidację "Date" z ModelState, bo będziemy ją modyfikować ręcznie
            ModelState.Remove("AvailableLanes"); 

            if (!ModelState.IsValid)
            {
                // Musimy załadować tory ponownie, jeśli wracamy z błędem
                model.AvailableLanes = await _context.Lanes.ToListAsync();
                return View(model);
            }

            var userId = _userManager.GetUserId(User);

            // --- TU JEST MAGIA ŁĄCZENIA ---
            // 1. Bierzemy samą datę (np. 2024-01-20)
            DateTime finalDate = model.Date.Date; 
            
            // 2. Bierzemy godzinę z stringa (np. "15:30") i dodajemy do daty
            if (TimeSpan.TryParse(model.SelectedTime, out TimeSpan timeSpan))
            {
                finalDate = finalDate.Add(timeSpan);
            }

            // 3. Przekazujemy sklejoną datę do serwisu
            string result = await _reservationService.BookLaneAsync(
    model.LaneId, 
    finalDate, 
    model.NumberOfPlayers, 
    userId, 
    model.Duration // <--- Dodaj ten parametr
);

            if (result == "OK")
            {
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError("", result);
                model.AvailableLanes = await _context.Lanes.ToListAsync();
                return View(model);
            }
        }
    }
}