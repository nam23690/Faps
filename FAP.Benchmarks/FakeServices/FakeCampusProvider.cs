using FAP.Common.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FAP.Benchmarks.FakeServices
{
    public class FakeCampusProvider : ICampusProvider
    {
        public string GetCampusConnectionString()
        {
            // Bạn chỉnh lại connection string tùy DB local của bạn
            string str = "Server=DESKTOP-GPIO766;Database=AP_HL;Persist Security Info=True;Trusted_Connection=True;MultipleActiveResultSets=True;TrustServerCertificate=True";
            str = "Server=DESKTOP-GPIO766;Database=AP_HCM;Persist Security Info=True;Trusted_Connection=True;MultipleActiveResultSets=True;TrustServerCertificate=True";
            return str;
        }
        public string GetCampusCode()=>"HCM";


    }

}
