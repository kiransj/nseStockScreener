using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace nseScreenerUI
{
    public class Program
    {
        private static string configFilepath;

        public static string configFile {
            get {
                return configFilepath;
            }
        }
        public static void Main(string[] args)
        {
            if(args.Length != 1) 
            {
                Console.WriteLine($"Pass the path to config file");
                Environment.Exit(1);
            }
            configFilepath = args[0];
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
