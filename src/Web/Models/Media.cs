using System;
using System.ComponentModel;

namespace GoldenForCongress.Models
{
    public class Media
    {
        public Guid ID { get; set; }
        public DateTime Date { get; set; }
        public string Location { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Type MediaType { get; set; }
        public string EmbeddedContent { get; set; }

        public enum Type
        {
            [Description("Audio")] Audio,
            [Description("Video")] Video,
            [Description("Image")] Image,
            [Description("Text")] Text
        }
    }
}