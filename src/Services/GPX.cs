using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using GoldenForCongress.Models;
using Newtonsoft.Json.Linq;

namespace GoldenForCongress.Services
{
    public static class GPX
    {
        public static XElement Transform(Section section)
        {
            var path = JArray.Parse(section.Path);
            return new XElement("gpx",
                new XAttribute("version", "1.0"),
                new XElement("name", section.Date.ToString("yyyy-MM-dd")),
                new XElement("trk",
                    new XElement("name", section.Description),
                    new XElement("number", "1"),
                    new XElement("trkseg", path.Select(j => new XElement("trkpt", new XAttribute("lat", j["lat"]), new XAttribute("lon", j["lng"]))))
                )
            );
        }
    }
}