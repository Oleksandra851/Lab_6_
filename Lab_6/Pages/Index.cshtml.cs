using System.Collections.Generic;
using System.Threading.Tasks;
using Lab_6.Data;
using Lab_6.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Lab_6.Pages
{
    public class IndexModel : PageModel
    {
        private readonly UniversityDbContext _context;

        public IndexModel(UniversityDbContext context)
        {
            _context = context;
        }

        public List<Position> Positions { get; set; } = new List<Position>();
        public List<Subject> Subjects { get; set; } = new List<Subject>();

        public async Task OnGetAsync()
        {
            Positions = await _context.Positions.ToListAsync();
            Subjects = await _context.Subjects.ToListAsync();
        }
    }
}