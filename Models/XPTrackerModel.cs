using System;
using System.Collections.Generic;

namespace XPTracker.Models
{
    public class XPTrackerModel
    {
        public int Id { get; set; }
        public int TotalXP { get; set; }
        public int TotalLevel { get; set; }
        public List<SessionTrackerModel> Sessions { get; set; } = new List<SessionTrackerModel>();
    }
}