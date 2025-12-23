using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FAP.Common.Domain.Entities
{
    public class Area
    {
        public short AreaID { get; set; }          // PK
        public string AreaName { get; set; }       // Tên khu vực
        public bool IsHeadStart { get; set; }      // Có phải HeadStart
        public bool IsActive { get; set; }         // Trạng thái
        public int Sort { get; set; }              // Thứ tự

        // Navigation: 1 Area có nhiều Room
        public ICollection<Room> Rooms { get; set; } = new List<Room>();
    }

}