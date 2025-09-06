using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLyTroDaiLoi.Data;
using QuanLyTroDaiLoi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuanLyTroDaiLoi.Pages
{
    public class DanhSachDongTienModel : PageModel
    {
        private readonly TroDbContext _context;

        public DanhSachDongTienModel(TroDbContext context)
        {
            _context = context;
        }

        public List<HoaDon> HoaDons { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public int Thang { get; set; }
        [BindProperty(SupportsGet = true)]
        public int Nam { get; set; }

        public SelectList ThangList { get; set; }
        public SelectList NamList { get; set; }

        public IList<DonVi> DonVis { get; set; }

        public async Task<IActionResult> OnGetAsync(int? thang, int? nam)
        {
            Thang = thang ?? DateTime.Now.Month;
            Nam = nam ?? DateTime.Now.Year;

            int startYear = 2025;
            int endYear = DateTime.Now.Year + 2;

            ThangList = new SelectList(Enumerable.Range(1, 12));
            NamList = new SelectList(Enumerable.Range(startYear, endYear - startYear + 1));

            // Lấy hóa đơn tháng/năm
            HoaDons = await _context.HoaDons
                .Include(h => h.DonVi)
                .Where(h => h.Thang == Thang && h.Nam == Nam)
                .ToListAsync();

            // Lấy tất cả đơn vị
            var allDonVis = await _context.DonVis.ToListAsync();

            var nhaOrder = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 15, 16 };
            var phongOrder = new List<int> { 1, 2, 3, 4, 6, 7, 8, 9, 10, 12, 16, 17, 18, 19, 20, 22, 24, 25, 26, 27 };

            DonVis = allDonVis
                .OrderBy(d =>
                {
                    var so = ExtractNumber(d.TenDonVi); // lấy số từ "Phòng 12" hoặc "Nhà 3"
                    if (d.LoaiDonVi == "Nha")
                        return nhaOrder.IndexOf(so); // theo thứ tự nhà
                    else
                        return 1000 + phongOrder.IndexOf(so); // theo thứ tự phòng
                })
                .ToList();

            return Page();
        }
        private int ExtractNumber(string input)
        {
            if (string.IsNullOrEmpty(input)) return -1;
            var digits = new string(input.Where(char.IsDigit).ToArray());
            return int.TryParse(digits, out int number) ? number : -1;
        }

    }

}