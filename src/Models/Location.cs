using System;

namespace GoldenForCongress.Models
{
    public class Location
    {
        public Guid ID { get; set; }
        public DateTime Time { get; set; }
        public string Position { get; set; }
    }
}