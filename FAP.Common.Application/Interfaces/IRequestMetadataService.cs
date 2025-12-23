using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FAP.Common.Application.Interfaces
{
    public interface IRequestMetadataService
    {
        string UserId { get; }
        string CampusCode { get; }
        string CorrelationId { get; }
        string IpAddress { get; }
        string UserAgent { get; }   
        string Method { get; }  
        string Path { get; }
        int StatusCode { get; }
    }

}
