using RezerwacjaKręgielnia.Entities;
using System.ComponentModel.DataAnnotations; // Dodaj to dla [Required]

namespace RezerwacjaKręgielnia.Models
{
    public class BookLaneViewModel
    {
        public int LaneId { get; set; }

        // Zmieniamy typ na samą datę (bez godziny) w formularzu
        [DataType(DataType.Date)]
        public DateTime Date { get; set; } = DateTime.Now;

        // NOWE POLE: Tutaj trafi "15:00" lub "15:30"
        public string SelectedTime { get; set; }

        [Display(Name = "Czas gry (godziny)")]
        public int Duration { get; set; } = 1;
        public int NumberOfPlayers { get; set; } = 2;
        public List<LaneEntity>? AvailableLanes { get; set; }
    }
}