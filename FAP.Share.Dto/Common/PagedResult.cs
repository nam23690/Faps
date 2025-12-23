using System;
using System.Collections.Generic;
using System.Text;

namespace FAP.Share.Dtos
{
    public class PagedResult<T> : PagedResultBase
    {
        public List<T> Items { set; get; }
    }
}