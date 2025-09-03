using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QuanLyTroDaiLoi.Data;
using QuanLyTroDaiLoi.Models;
using System.Linq;

namespace QuanLyTroDaiLoi.Pages.CauHinhsThue
{
    public class EditModel : PageModel
    {
        private readonly TroDbContext _context;
        public EditModel(TroDbContext context) => _context = context;

        [BindProperty]
        public CauHinhThue CauHinh { get; set; } = new();

        public void OnGet()
        {
            CauHinh = _context.CauHinhThues.FirstOrDefault();
            if (CauHinh == null)
            {
                CauHinh = new CauHinhThue();
                _context.CauHinhThues.Add(CauHinh);
                _context.SaveChanges();
            }
        }

        public IActionResult OnPost()
        {
            var existing = _context.CauHinhThues.FirstOrDefault();

            if (existing != null)
            {
                // Cập nhật giá từ form
                existing.GiaThuePhong = CauHinh.GiaThuePhong;
                existing.GiaThueNha = CauHinh.GiaThueNha;

                _context.SaveChanges(); // chỉ cần SaveChanges, không cần _context.Update()
            }
            else
            {
                // Nếu chưa có, tạo record mới với Id = 1
                CauHinh.Id = 1;
                _context.Add(CauHinh);
                _context.SaveChanges();
            }

            TempData["Message"] = "Cập nhật giá thuê thành công!";
            return RedirectToPage("/CauHinhThues/Edit");
        }

    }
}
