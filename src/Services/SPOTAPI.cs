using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using GoldenForCongress.Data;
using GoldenForCongress.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;

namespace GoldenForCongress.Services
{
    public class SPOTAPI
    {
        private static readonly TimeSpan WaitTime = TimeSpan.FromMinutes(2.5);
        private static readonly TimeSpan WaitInterval = TimeSpan.FromMilliseconds(1000);

        private readonly Func<DB> _newDB;
        private readonly HttpClient _httpClient;
        private readonly IHostingEnvironment _env;
        private readonly string _url;
        private DateTime _lastChecked;
        public bool Running { get; private set; }

        public SPOTAPI(Func<DB> dbFac, HttpClient httpClient, IConfiguration config, IHostingEnvironment env)
        {
            _newDB = dbFac;
            _httpClient = httpClient;
            _env = env;
            _url = $"https://api.findmespot.com/spot-main-web/consumer/rest-api/2.0/public/feed/{config["SPOT_FEED_ID"]}/message.json?feedPassword={config["SPOT_KEY"]}";

#pragma warning disable 4014
            Start();
#pragma warning restore 4014
        }

        public async Task Start()
        {
            if (Running)
                return;

            Running = true;
            while (Running)
            {
                try
                {
                    var now = DateTime.Now;
                    if (now - _lastChecked <= WaitTime)
                        continue;

                    var db = _newDB();
                    var result = await _httpClient.GetStringAsync(_url);
                    _lastChecked = now;

                    var message = JObject.Parse(result)["response"]["feedMessageResponse"]["messages"]["message"][0];
                    var location = new Location
                    {
                        ID = Guid.NewGuid(),
                        Time = DateTime.Parse(message["dateTime"].Value<string>()),
                        Position = $"{{ lat: {message["latitude"]}, lng: {message["longitude"]} }}"
                    };
                    await db.AddAsync(location);
                    await db.SaveChangesAsync();

                    var json = JObject.FromObject(location, Startup.SnakeCase);
                    json["position"] = JObject.Parse(json["position"].Value<string>());
                    File.WriteAllText(Path.Combine(_env.WebRootPath, "ian.json"), json.ToString());
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
                finally
                {
                    await Task.Delay(WaitInterval);
                }
            }
        }

        public void Stop()
        {
            Running = false;
            _lastChecked = new DateTime();
        }
    }
}
