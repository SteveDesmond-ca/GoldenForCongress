using System;

namespace GoldenForCongress.Models
{
    public class Event
    {
        public Guid ID { get; set; }
        public DateTime Date { get; set; }
        public string Location { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Link { get; set; }
    }
}