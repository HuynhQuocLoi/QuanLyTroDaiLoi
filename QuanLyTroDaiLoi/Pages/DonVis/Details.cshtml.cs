using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLyTroDaiLoi.Data;
using QuanLyTroDaiLoi.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace QuanLyTroDaiLoi.Pages.DonVis
{
    public class DetailsModel : PageModel
    {
        private readonly TroDbContext _context;

        public DetailsModel(TroDbContext context)
        {
            _context = context;
        }

        [BindProperty] public DonVi DonVi { get; set; } = null!;
        [BindProperty] public HoaDon CurrentMonth { get; set; } = null!;
        [BindProperty(SupportsGet = true)] public int Thang { get; set; }
        [BindProperty(SupportsGet = true)] public int Nam { get; set; }

        public SelectList ThangList { get; set; } = null!;
        public SelectList NamList { get; set; } = null!;

        // ------------------------ GET ------------------------
        public async Task<IActionResult> OnGetAsync(int id, int thang = 0, int nam = 0)
        {
            DonVi = await _context.DonVis
                .Include(d => d.NguoiThues)
                .FirstOrDefaultAsync(d => d.DonViId == id);

            if (DonVi == null) return NotFound();
            var cauHinhThue = await _context.CauHinhThues.FirstOrDefaultAsync() ?? new CauHinhThue();

            ThangList = new SelectList(Enumerable.Range(1, 12).Select(x => new SelectListItem
            {
                Value = x.ToString(),
                Text = x.ToString(),
                Selected = x == thang
            }), "Value", "Text");

            NamList = new SelectList(Enumerable.Range(2020, 10).Select(x => new SelectListItem
            {
                Value = x.ToString(),
                Text = x.ToString(),
                Selected = x == nam
            }), "Value", "Text");

            Thang = thang;
            Nam = nam;

            if (Thang > 0 && Nam > 0)
            {
                CurrentMonth = await _context.HoaDons
                    .Include(h => h.PhiKhacs)
                    .Include(h => h.DonVi)
                    .FirstOrDefaultAsync(h => h.DonViId == id && h.Thang == Thang && h.Nam == Nam);

                if (CurrentMonth != null)
                {
                    // Cập nhật tiền phòng theo loại đơn vị
                    CurrentMonth.TienPhong = CurrentMonth.DonVi?.LoaiDonVi?.ToLower() == "nha"
                        ? cauHinhThue.GiaThueNha
                        : cauHinhThue.GiaThuePhong;

                    UpdateTongTien(CurrentMonth); // cập nhật tổng tiền
                }

            }
            UpdateTongTien(CurrentMonth);

            return Page();
        }

        // ------------------------ INIT MONTH ------------------------
        public async Task<IActionResult> OnPostInitMonthAsync(int id)
        {
            if (Thang <= 0 || Nam <= 0) return RedirectToPage(new { id });

            // Lấy cấu hình giá điện, nước
            var cauHinh = await _context.CauHinhs.FirstOrDefaultAsync()
                           ?? new CauHinh { DonGiaDien = 3500, DonGiaNuoc = 10000 };

            // Lấy cấu hình giá thuê
            var cauHinhThue = await _context.CauHinhThues.FirstOrDefaultAsync() ?? new CauHinhThue();

            // Kiểm tra hóa đơn đã tồn tại cho kỳ này chưa
            var existed = await _context.HoaDons.AnyAsync(h => h.DonViId == id && h.Thang == Thang && h.Nam == Nam);
            if (!existed)
            {
                // Lấy kỳ trước gần nhất
                var prev = await GetPrevBillAsync(id, Thang, Nam);
                var (dCu, nCu) = prev == null ? (0, 0) : (prev.DienMoi, prev.NuocMoi);

                // Lấy thông tin đơn vị (để lấy loại đơn vị nếu cần)
                var donVi = await _context.DonVis.FirstOrDefaultAsync(d => d.DonViId == id);
                if (donVi == null) return NotFound();

                // Chọn giá thuê theo loại đơn vị (ví dụ "Nha" hay "Phong")
                decimal tienPhong = cauHinhThue.GiaThuePhong; // mặc định phòng
                if (!string.IsNullOrEmpty(donVi.LoaiDonVi) && donVi.LoaiDonVi.ToLower() == "nha")
                {
                    tienPhong = cauHinhThue.GiaThueNha;
                }

                var bill = new HoaDon
                {
                    DonViId = id,
                    Thang = Thang,
                    Nam = Nam,
                    DienCu = dCu,
                    DienMoi = dCu,
                    NuocCu = nCu,
                    NuocMoi = nCu,
                    DonGiaDien = cauHinh.DonGiaDien,
                    DonGiaNuoc = cauHinh.DonGiaNuoc,
                    TienPhong = tienPhong,
                    NgayTao = DateTime.Now,
                    IsClosed = false
                };

                _context.HoaDons.Add(bill);
                await _context.SaveChangesAsync();

                // Copy phụ phí từ kỳ trước nếu có
                if (prev != null && prev.PhiKhacs.Any())
                {
                    foreach (var pk in prev.PhiKhacs)
                    {
                        var newPk = new PhiKhac
                        {
                            TenPhi = pk.TenPhi,
                            DonGia = pk.DonGia,
                            SoLuong = pk.SoLuong,
                            ThanhTien = pk.ThanhTien,
                            DonViId = id,
                            HoaDonId = bill.HoaDonId
                        };
                        _context.PhiKhacs.Add(newPk);
                    }
                    await _context.SaveChangesAsync();
                }
            }

            return RedirectToPage(new { id, Thang, Nam });
        }



        // ------------------------ SAVE / CHOT HOA DON ------------------------
        public async Task<IActionResult> OnPostSaveAsync(HoaDon CurrentBill)
        {
            var bill = await _context.HoaDons
                .Include(h => h.PhiKhacs)
                .Include(h => h.DonVi)
                .FirstOrDefaultAsync(h => h.HoaDonId == CurrentBill.HoaDonId);

            if (bill == null) return NotFound();

            bill.DienCu = CurrentBill.DienCu;
            bill.DienMoi = CurrentBill.DienMoi;
            bill.NuocCu = CurrentBill.NuocCu;
            bill.NuocMoi = CurrentBill.NuocMoi;
            bill.DonGiaDien = CurrentBill.DonGiaDien;
            bill.DonGiaNuoc = CurrentBill.DonGiaNuoc;

            UpdateTongTien(bill);

            _context.Update(bill);
            await _context.SaveChangesAsync();

            return RedirectToPage(new { id = bill.DonViId, Thang = bill.Thang, Nam = bill.Nam });
        }

        private void UpdateTongTien(HoaDon bill)
        {
            if (bill == null) return;

            var cauHinhThue = _context.CauHinhThues.FirstOrDefault() ?? new CauHinhThue();

            decimal tienPhong = bill.DonVi?.LoaiDonVi?.ToLower() == "nha"
                ? cauHinhThue.GiaThueNha
                : cauHinhThue.GiaThuePhong;

            var tienDien = (bill.DienMoi - bill.DienCu) * bill.DonGiaDien;
            var tienNuoc = (bill.NuocMoi - bill.NuocCu) * bill.DonGiaNuoc;
            var tienPhiKhac = bill.PhiKhacs?.Sum(p => p.ThanhTien) ?? 0;

            bill.TongTien = tienPhong + tienDien + tienNuoc + tienPhiKhac;
        }



        // ------------------------ TOGGLE CLOSE ------------------------
        public async Task<IActionResult> OnPostToggleCloseAsync(int id)
        {
            var bill = await _context.HoaDons
                .FirstOrDefaultAsync(h => h.DonViId == id && h.Thang == Thang && h.Nam == Nam);

            if (bill == null) return NotFound();

            bill.IsClosed = !bill.IsClosed;
            await _context.SaveChangesAsync();

            return RedirectToPage(new { id, Thang, Nam });
        }

        // ------------------------ NGUOI THUE ------------------------
        public async Task<IActionResult> OnPostAddNguoiThueAsync(int id, string HoTen, string CCCD, string SDT, string DiaChi, DateTime? NgayVao)
        {
            var donvi = await _context.DonVis.Include(d => d.NguoiThues).FirstOrDefaultAsync(d => d.DonViId == id);
            if (donvi == null) return NotFound();

            donvi.NguoiThues.Add(new NguoiThue
            {
                DonViId = id,
                HoTen = HoTen,
                CCCD = CCCD,
                SDT = SDT,
                DiaChi = DiaChi,
                NgayVao = NgayVao ?? DateTime.Now
            });

            await _context.SaveChangesAsync();
            return RedirectToPage(new { id, Thang, Nam });
        }

        public async Task<IActionResult> OnPostRemoveNguoiThueAsync(int id, int nguoiId)
        {
            var nt = await _context.NguoiThues.FindAsync(nguoiId);
            if (nt != null)
            {
                _context.NguoiThues.Remove(nt);
                await _context.SaveChangesAsync();
            }
            return RedirectToPage(new { id, Thang, Nam });
        }

        // ------------------------ PHI KHAC ------------------------
        public async Task<IActionResult> OnPostAddPhiKhacAsync(int id, string TenPhi, string DonGiaInput, int SoLuong)
        {
            var bill = await _context.HoaDons
                .Include(h => h.PhiKhacs)
                .Include(h => h.DonVi)
                .FirstOrDefaultAsync(h => h.DonViId == id && h.Thang == Thang && h.Nam == Nam);

            if (bill == null) return NotFound();

            // Loại bỏ dấu chấm trước khi convert
            var donGia = decimal.Parse(DonGiaInput.Replace(".", ""));

            var pk = new PhiKhac
            {
                TenPhi = TenPhi,
                DonGia = donGia,
                SoLuong = SoLuong,
                ThanhTien = donGia * SoLuong,
                DonViId = id,
                HoaDonId = bill.HoaDonId
            };

            _context.PhiKhacs.Add(pk);
            await _context.SaveChangesAsync();

            // Cập nhật lại tổng tiền
            UpdateTongTien(bill);
            _context.Update(bill);
            await _context.SaveChangesAsync();

            return RedirectToPage(new { id, Thang, Nam });
        }


        // Hàm tiện ích
        private decimal ParseVNCurrency(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return 0;
            var cleaned = input.Replace(".", "").Replace(",", "");
            return decimal.TryParse(cleaned, out var val) ? val : 0;
        }


        public async Task<IActionResult> OnPostDeletePhiKhacAsync(int id, int phiId)
        {
            var phi = await _context.PhiKhacs.FindAsync(phiId);
            if (phi != null)
            {
                _context.PhiKhacs.Remove(phi);
                await _context.SaveChangesAsync();
            }
            return RedirectToPage(new { id, Thang, Nam });
        }

        // ------------------------ DELETE ALL DATA ------------------------
        public async Task<IActionResult> OnPostDeleteAllDataAsync(int id)
        {
            var donVi = await _context.DonVis
                .Include(d => d.NguoiThues)
                .Include(d => d.DonViThangs)
                    .ThenInclude(m => m.PhiKhacs)
                .FirstOrDefaultAsync(d => d.DonViId == id);

            if (donVi != null)
            {
                // Xóa tất cả người thuê
                _context.NguoiThues.RemoveRange(donVi.NguoiThues);

                // Xóa tất cả phụ phí trong từng kỳ
                foreach (var month in donVi.DonViThangs)
                {
                    _context.PhiKhacs.RemoveRange(month.PhiKhacs);
                }

                // Xóa tất cả kỳ (DonViThang)
                _context.DonViThangs.RemoveRange(donVi.DonViThangs);

                await _context.SaveChangesAsync();
            }

            return RedirectToPage("/DonVis/Index"); // quay về danh sách phòng
        }





        // ------------------------ PRIVATE ------------------------
        private async Task<HoaDon?> GetPrevBillAsync(int donViId, int thang, int nam)
        {
            var (pt, pn) = thang == 1 ? (12, nam - 1) : (thang - 1, nam);
            return await _context.HoaDons
                .Include(h => h.PhiKhacs)
                .Include(h => h.DonVi)
                .FirstOrDefaultAsync(h => h.DonViId == donViId && h.Thang == pt && h.Nam == pn);
        }
    
    //-----------------------------XoaALL------------------------
    public async Task<IActionResult> OnPostResetRoomDataAsync(int id)
        {
            var bill = await _context.HoaDons
                .Where(h => h.DonViId == id)
                .OrderByDescending(h => h.Nam)
                .ThenByDescending(h => h.Thang)
                .FirstOrDefaultAsync();

            int dienMoi = bill?.DienMoi ?? 0;
            int nuocMoi = bill?.NuocMoi ?? 0;

            // Xóa người thuê
            var nguoi = await _context.NguoiThues.Where(n => n.DonViId == id).ToListAsync();
            _context.NguoiThues.RemoveRange(nguoi);

            // Xóa các phí khác
            var phis = await _context.PhiKhacs.Where(p => p.DonViId == id).ToListAsync();
            _context.PhiKhacs.RemoveRange(phis);

            await _context.SaveChangesAsync();

            // Cập nhật lại số điện nước mới nhất thành cũ của kỳ tiếp theo
            if (bill != null)
            {
                bill.DienCu = dienMoi;
                bill.NuocCu = nuocMoi;
                bill.DienMoi = dienMoi;
                bill.NuocMoi = nuocMoi;
                UpdateTongTien(bill);
                _context.Update(bill);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage(new { id });
        }
        //---------------------Luu dien nuoc---------------
        public async Task<IActionResult> OnPostSaveWaterElectricAsync(int id, int DienCu, int DienMoi, int NuocCu, int NuocMoi)
        {
            var bill = await _context.HoaDons.FirstOrDefaultAsync(h => h.DonViId == id && h.Thang == Thang && h.Nam == Nam);
            if (bill == null) return NotFound();

            bill.DienCu = DienCu;
            bill.DienMoi = DienMoi;
            bill.NuocCu = NuocCu;
            bill.NuocMoi = NuocMoi;

            UpdateTongTien(bill);

            _context.Update(bill);
            await _context.SaveChangesAsync();

            return RedirectToPage(new { id, Thang, Nam });
        }
        

    }
}
