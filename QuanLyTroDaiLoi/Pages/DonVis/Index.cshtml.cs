using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QuanLyTroDaiLoi.Data;
using QuanLyTroDaiLoi.Models;

namespace QuanLyTroDaiLoi.Pages.DonVis
{
    public class IndexModel : PageModel
    {
        private readonly TroDbContext _context;
        public IndexModel(TroDbContext context) => _context = context;

        public List<DonVi> DanhSachDonVi { get; set; } = new();
        public string LoaiDonVi { get; set; } = "Phong";

        public async Task OnGetAsync(string? loai)
        {
            LoaiDonVi = loai ?? "Phong";
            DanhSachDonVi = await _context.DonVis
                .Where(d => d.LoaiDonVi == LoaiDonVi)
                .ToListAsync();
        }
    }
}
