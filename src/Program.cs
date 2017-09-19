using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace GoldenForCongress
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .CaptureStartupErrors(true)
                .Build();

            host.Run();
        }
    }
}
