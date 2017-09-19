using System;

namespace GoldenForCongress.Models
{
    public class Section
    {
        public Guid ID { get; set; }
        public int Day { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public string Color { get; set; }
        public string Path { get; set; }
    }
}
