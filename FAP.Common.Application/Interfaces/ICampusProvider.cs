using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FAP.Common.Application.Interfaces
{
    public interface ICampusProvider
    {
        string GetCampusCode();
        string GetCampusConnectionString();
    }

}
