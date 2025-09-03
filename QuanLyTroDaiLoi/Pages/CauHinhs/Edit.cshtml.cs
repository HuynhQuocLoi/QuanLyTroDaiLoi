using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QuanLyTroDaiLoi.Data;
using QuanLyTroDaiLoi.Models;
using System.Linq;

namespace QuanLyTroDaiLoi.Pages.CauHinhs
{
    public class EditModel : PageModel
    {
        private readonly TroDbContext _context;
        public EditModel(TroDbContext context) => _context = context;

        [BindProperty]
        public CauHinh CauHinh { get; set; } = new();

        public void OnGet()
        {
            // Lấy record duy nhất, nếu chưa có thì tạo mới
            CauHinh = _context.CauHinhs.FirstOrDefault();
            if (CauHinh == null)
            {
                CauHinh = new CauHinh { DonGiaDien = 3500, DonGiaNuoc = 10000 };
                _context.CauHinhs.Add(CauHinh);
                _context.SaveChanges();
            }
        }

        public IActionResult OnPost()
        {
            var existing = _context.CauHinhs.FirstOrDefault();
            if (existing != null)
            {
                existing.DonGiaDien = CauHinh.DonGiaDien;
                existing.DonGiaNuoc = CauHinh.DonGiaNuoc;
                _context.Update(existing);
            }
            else
            {
                _context.Add(CauHinh);
            }

            _context.SaveChanges();
            TempData["Message"] = "Cập nhật giá điện nước thành công!";
            return RedirectToPage("/CauHinhs/Edit");
        }
    }
}
