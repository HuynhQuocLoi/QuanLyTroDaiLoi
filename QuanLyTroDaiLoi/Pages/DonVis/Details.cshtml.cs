using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuanLyTroDaiLoi.Data;
using QuanLyTroDaiLoi.Models;
using QuestPDF.Helpers;
using System;
using System.Linq;
using System.Threading.Tasks;
using SkiaSharp;

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
            int startYear = 2025;
            int endYear = DateTime.Now.Year + 2;
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

            NamList = new SelectList(
    Enumerable.Range(startYear, endYear - startYear + 1).Select(x => new SelectListItem
    {
        Value = x.ToString(),
        Text = x.ToString(),
        Selected = (nam == 0 ? x == DateTime.Now.Year : x == nam)
    }),
    "Value", "Text"
);

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
                    // Chỉ khi hóa đơn chưa có tiền phòng thì mới set theo cấu hình
                    if (CurrentMonth.TienPhong <= 0)
                    {
                        CurrentMonth.TienPhong = CurrentMonth.DonVi?.LoaiDonVi?.ToLower() == "nha"
                            ? cauHinhThue.GiaThueNha
                            : cauHinhThue.GiaThuePhong;

                        await _context.SaveChangesAsync(); // lưu snapshot giá thuê
                    }

                    UpdateTongTien(CurrentMonth); // cập nhật tổng tiền
                }


            }
            UpdateTongTien(CurrentMonth);

            return Page();
        }

        // ------------------------ INIT MONTH ------------------------
        // ------------------------ INIT MONTH ------------------------
        public async Task<IActionResult> OnPostInitMonthAsync(int id)
        {
            if (Thang <= 0 || Nam <= 0) return RedirectToPage(new { id });

            // Lấy cấu hình giá điện, nước (mới nhất)
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

                // Lấy thông tin đơn vị
                var donVi = await _context.DonVis.FirstOrDefaultAsync(d => d.DonViId == id);
                if (donVi == null) return NotFound();

                // Chọn giá thuê theo loại đơn vị
                decimal tienPhong = cauHinhThue.GiaThuePhong;
                if (!string.IsNullOrEmpty(donVi.LoaiDonVi) && donVi.LoaiDonVi.ToLower() == "nha")
                {
                    tienPhong = cauHinhThue.GiaThueNha;
                }

                // 🔥 Khi tạo kỳ mới -> lấy đơn giá điện nước từ cấu hình hiện tại
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
            // 🔹 Xóa toàn bộ người thuê của phòng
            var nguoiThues = await _context.NguoiThues.Where(n => n.DonViId == id).ToListAsync();
            _context.NguoiThues.RemoveRange(nguoiThues);

            // 🔹 Xóa toàn bộ phụ phí liên quan phòng này
            var phiKhacs = await _context.PhiKhacs.Where(p => p.DonViId == id).ToListAsync();
            _context.PhiKhacs.RemoveRange(phiKhacs);

            // 🔹 Xóa toàn bộ kỳ của phòng (bao gồm cả số điện, nước cũ/mới)
            var thangs = await _context.DonViThangs.Where(t => t.DonViId == id).ToListAsync();
            _context.DonViThangs.RemoveRange(thangs);

            // 🔹 Xóa luôn hóa đơn của phòng nếu có
            var hoaDons = await _context.HoaDons.Where(h => h.DonViId == id).ToListAsync();
            _context.HoaDons.RemoveRange(hoaDons);

            // Lưu thay đổi
            await _context.SaveChangesAsync();

            return RedirectToPage("/DonVis/Index"); // Quay về danh sách phòng
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
            // Xóa tất cả người thuê
            var nguoi = await _context.NguoiThues.Where(n => n.DonViId == id).ToListAsync();
            _context.NguoiThues.RemoveRange(nguoi);

            // Xóa tất cả phụ phí
            var phis = await _context.PhiKhacs.Where(p => p.DonViId == id).ToListAsync();
            _context.PhiKhacs.RemoveRange(phis);

            // Xóa tất cả hóa đơn của phòng
            var bills = await _context.HoaDons.Where(h => h.DonViId == id).ToListAsync();
            _context.HoaDons.RemoveRange(bills);

            await _context.SaveChangesAsync();

            // Quay về danh sách hoặc trang chi tiết
            return RedirectToPage("/DonVis/Index");
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
        public async Task<IActionResult> OnPostExportInvoiceAsync(int id, int Thang, int Nam)
        {
            var hd = await _context.HoaDons
                .Include(h => h.DonVi)
                .Include(h => h.PhiKhacs)
                .FirstOrDefaultAsync(h => h.DonViId == id && h.Thang == Thang && h.Nam == Nam);

            if (hd == null) return NotFound();

            var tienDien = (hd.DienMoi - hd.DienCu) * hd.DonGiaDien;
            var tienNuoc = (hd.NuocMoi - hd.NuocCu) * hd.DonGiaNuoc;
            var tienPhiKhac = hd.PhiKhacs?.Sum(p => p.ThanhTien) ?? 0;
            var tongTien = hd.TienPhong + tienDien + tienNuoc + tienPhiKhac;

            var soTaiKhoan = "0909678666"; // cố định

            QuestPDF.Settings.License = LicenseType.Community;

            QuestPDF.Settings.License = LicenseType.Community;

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A5);
                    page.Margin(20);

                    page.Header().Text("HÓA ĐƠN THANH TOÁN THUÊ TRỌ")
                        .SemiBold().FontSize(18).FontColor(Colors.Blue.Medium).AlignCenter();

                    page.Content().Column(col =>
                    {
                        col.Spacing(10);

                        col.Item().Text($"{hd.DonVi.TenDonVi} - Tháng {Thang}/{Nam}")
                            .FontSize(12).AlignCenter();

                        // Bảng chi tiết (có thêm cột Tiêu thụ)
                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(c =>
                            {
                                c.RelativeColumn(3);  // Hạng mục
                                c.RelativeColumn(3);  // Chỉ số
                                c.RelativeColumn(2);  // Tiêu thụ
                                c.RelativeColumn(3);  // Đơn giá
                                c.RelativeColumn(4);  // Thành tiền
                            });

                            // Header
                            table.Header(header =>
                            {
                                header.Cell().Border(1).PaddingVertical(6).PaddingHorizontal(4)
                                    .Background(Colors.Grey.Lighten2).AlignCenter().Text("Hạng mục").SemiBold();
                                header.Cell().Border(1).PaddingVertical(6).PaddingHorizontal(4)
                                    .Background(Colors.Grey.Lighten2).AlignCenter().Text("Chỉ số").SemiBold();
                                header.Cell().Border(1).PaddingVertical(6).PaddingHorizontal(4)
                                    .Background(Colors.Grey.Lighten2).AlignCenter().Text("Tiêu thụ").SemiBold();
                                header.Cell().Border(1).PaddingVertical(6).PaddingHorizontal(4)
                                    .Background(Colors.Grey.Lighten2).AlignCenter().Text("Đơn giá").SemiBold();
                                header.Cell().Border(1).PaddingVertical(6).PaddingHorizontal(4)
                                    .Background(Colors.Grey.Lighten2).AlignCenter().Text("Thành tiền").SemiBold();
                            });

                            // Row helper (lưu ý: .Text(...).SemiBold() là hợp lệ)
                            // Row helper: tất cả AlignLeft
                            void Row(string hangMuc, string chiSo, string tieuThu, string donGia, string thanhTien)
                            {
                                table.Cell().Border(1).Padding(6).AlignLeft().Text(hangMuc);
                                table.Cell().Border(1).Padding(6).AlignLeft().Text(chiSo);
                                table.Cell().Border(1).Padding(6).AlignLeft().Text(tieuThu);
                                table.Cell().Border(1).Padding(6).AlignLeft().Text(donGia);
                                table.Cell().Border(1).Padding(6).AlignLeft().Text(thanhTien);
                            }

                            // Dữ liệu
                            Row("Tiền phòng", "-", "-", "-", $"{hd.TienPhong:N0}");
                            Row("Điện", $"{hd.DienCu} → {hd.DienMoi}", $"{hd.DienMoi - hd.DienCu}", $"{hd.DonGiaDien:N0}", $"{tienDien:N0}");
                            Row("Nước", $"{hd.NuocCu} → {hd.NuocMoi}", $"{hd.NuocMoi - hd.NuocCu}", $"{hd.DonGiaNuoc:N0}", $"{tienNuoc:N0}");

                            if (hd.PhiKhacs != null && hd.PhiKhacs.Any())
                            {
                                foreach (var phi in hd.PhiKhacs)
                                {
                                    Row(phi.TenPhi, "-", "-", $"{phi.DonGia:N0}", $"{phi.ThanhTien:N0}");
                                }
                            }

                            // Tổng cộng
                            table.Cell().ColumnSpan(4).Border(1).Padding(8).AlignLeft()
                                .Text("TỔNG CỘNG").SemiBold();

                            table.Cell().Border(1).Padding(8).AlignLeft()
                                .Text($"{tongTien:N0} ₫").SemiBold().FontColor(Colors.Red.Medium);

                        });

                        // Footer text dưới bảng
                        col.Item().PaddingTop(15).Column(c =>
                        {
                            c.Item().Text("Người nhận: Huỳnh Công Lý").AlignCenter();
                            c.Item().Text($"STK: {soTaiKhoan} (Sacombank)").AlignCenter();
                            c.Item().Text("Xin cảm ơn quý khách!").Italic().FontSize(11).AlignCenter();
                        });
                    });
                });
            });


            // Xuất ra ảnh PNG
            var images = document.GenerateImages();
            var firstImageBytes = images.First();
            var imgStream = new MemoryStream(firstImageBytes);

            return File(imgStream, "image/png", $"HoaDon_{hd.DonVi.TenDonVi}_{Thang}_{Nam}.png");
        }
        public async Task<IActionResult> OnPostTogglePaidAsync(int hoaDonId)
        {
            var hoaDon = await _context.HoaDons.FindAsync(hoaDonId);
            if (hoaDon == null) return NotFound();

            hoaDon.DaDongTien = !hoaDon.DaDongTien;
            await _context.SaveChangesAsync();

            return RedirectToPage(new { id = hoaDon.DonViId, thang = hoaDon.Thang, nam = hoaDon.Nam });
        }



    }
}
