using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FAP.Common.Domain.Entities;

namespace FAP.Common.Application.Interfaces
{
    public interface IUniversityDbContext
    {
        DbSet<SinhVien> SinhViens { get; }
        
        DbSet<MonHoc> MonHocs { get; }
        DbSet<DangKyMon> DangKyMons { get; }
        DbSet<Term> Terms { get; }
        DbSet<User> Users { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
