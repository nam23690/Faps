using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FAP.Common.Application.Interfaces
{
    public interface ICampusDbContextFactory
    {
        IUniversityDbContext Create();
        IUniversityDbContext Create(string CampusCode);
    }


}
