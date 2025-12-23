using System;
using System.Collections.Generic;

namespace FAP.Common.Domain.Entities
{
    public class Room
    {
        public short AreaID { get; set; }
        public string RoomNo { get; set; }
        public short Capacity { get; set; }
        public short TypeID { get; set; }
        public string Description { get; set; }
        public bool IsDisable { get; set; }
        public Area Area { get; set; }
        public RoomType RoomType { get; set; }
    }
}