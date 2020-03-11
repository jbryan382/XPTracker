using System;
using System.Collections.Generic;

namespace XPTracker.Models
{
    public class SessionTrackerModel
    {
        public int Id { get; set; }
        public string SessionDescription { get; set; }
        public int SocialInteractionXP { get; set; }
        public int ExplorationXP { get; set; }
        public int CombatXP { get; set; }
        public int XPId { get; set; }
        public XPTrackerModel XPTrackerModel { get; set; }
    }
}