using FAP.Common.Domain.Base;

namespace FAP.Common.Domain.Entities
{
    public class SinhVien : BaseEntity
    {
        public string MaSinhVien { get; set; } = default!;
        public string HoTen { get; set; } = default!;
        public DateTime NgaySinh { get; set; }
        public string Email { get; set; } = default!;
        public ICollection<DangKyMon> DangKyMons { get; set; } = new List<DangKyMon>();
    }

    public class MonHoc : BaseEntity
    {
        public string MaMon { get; set; } = default!;
        public string TenMon { get; set; } = default!;
        public int SoTinChi { get; set; }

        public ICollection<DangKyMon> DangKyMons { get; set; } = new List<DangKyMon>();
    }

    public class DangKyMon : BaseEntity
    {
        public int SinhVienId { get; set; }
        public int MonHocId { get; set; }

        public SinhVien SinhVien { get; set; } = default!;
        public MonHoc MonHoc { get; set; } = default!;
        public DateTime NgayDangKy { get; set; }
    }
}
