using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RezerwacjaKręgielnia.Models;
using RezerwacjaKręgielnia.Services;

namespace RezerwacjaKręgielnia.Controllers
{
    [Authorize(Roles = "Admin")] // TYLKO ADMIN MA WSTĘP!
    public class AdminController : Controller
    {
        private readonly ReservationService _reservationService;

        public AdminController(ReservationService reservationService)
        {
            _reservationService = reservationService;
        }

        public async Task<IActionResult> Index()
        {
            var model = new AdminDashboardViewModel
            {
                TotalReservations = await _reservationService.GetTotalReservationsCountAsync(),
                TotalRevenue = await _reservationService.GetTotalRevenueAsync(),
                Reservations = await _reservationService.GetAllReservationsAsync(),
                LaneStats = await _reservationService.GetLanePopularityAsync()
            };

            return View(model);
        }
    }
}
