using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Assessment.Application.Interfaces;
using Assessment.Infrastructure.Messengers;
using Assessment.Application.Processors;
using Assessment.Domain.Entity;
using Assessment.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;

namespace Assessment.Application.CorrespondenceService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //Add Serilog as the logger of choice
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft",
                LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .WriteTo.File(@"C:\Temp\WorkerService\LogFile.txt")
                .CreateLogger();

            try 
            {
                Log.Information("Correspondence Service is starting.");
                CreateHostBuilder(args).Build().Run();
                return;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Correspondence Service failed to start");
                return;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    IConfiguration configuration = hostContext.Configuration;
   
                    services.AddHostedService<Worker>();

                    ///Register the various services and processors for injection
                    services.AddTransient<IMessageRepository, EmployeeMessageRepository>();
                    //You can add additional correspondence classes here, like salary paid. Could have used reflection to configure the classes by looping through classes that implement ICorrespondenceProcessor
                 
                    services.AddTransient<ICorrespondenceProcessor, BirthdayCorrespondenceProcessor>();
                    services.AddTransient<ICorrespondenceProcessor,WorkAnniversaryCorrespondenceProcessor>();
                    services.AddTransient<IMessenger, SendGridMessenger>();

                }).UseSerilog();
    }
}
