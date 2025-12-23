using System;
using System.Collections.Generic;

namespace FAP.Common.Domain.Entities
{
  
    public class RoomType
    {
        public short TypeID { get; set; }
        public string TypeName { get; set; }
        public string Color { get; set; }
        public ICollection<Room> Rooms { get; set; } = new List<Room>();
    }
}