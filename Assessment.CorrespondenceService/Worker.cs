using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Assessment.Application.Interfaces;
using Assessment.Domain;
using Assessment.Domain.Entity;
using Assessment.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Assessment.Application.CorrespondenceService
{


    /// <summary>
    /// This worker can be converted to either a windows service a a Linux deamon process
    /// To convert to a windows service include the nuget package Microsoft.Extensions.Hosting.WindowsService
    /// </summary>
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IEnumerable<ICorrespondenceProcessor> _correspondenceProcessors;
        private HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private DateTime _lastRun;
        //Poll interval, stored in appSettings to control 
        private double _pollInterval;
        private async Task<IEnumerable<Employee>> GetEmployees()
        {
            var response = await GetApiDataAsync(_configuration["EmployeesEndPoint"]);
            return JsonConvert.DeserializeObject<List<Employee>>(response);
        }

        public async Task<string> GetApiDataAsync(string endpoint)
        {
            var apiBaseUrl = _configuration["ApiBaseUrl"];
            var response = await _httpClient.GetStringAsync($"{apiBaseUrl}\\{endpoint}");
            return response;
        }

        private async Task<IEnumerable<int>> GetExclusions()
        {
            var response = await GetApiDataAsync(_configuration["ExclusionsEndpoint"]);
            return JsonConvert.DeserializeObject<List<int>>(response);
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _httpClient = new HttpClient();
            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _httpClient.Dispose();
            return base.StopAsync(cancellationToken);
        }

        public Worker(ILogger<Worker> logger, IConfiguration configuration, IEnumerable<ICorrespondenceProcessor> correspondenceProcessors)
        {
            _logger = logger;
            _correspondenceProcessors = correspondenceProcessors;
            _configuration = configuration;
            _pollInterval = double.Parse(_configuration["PollIntervalMinutes"]);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {

                _logger.LogInformation("Worker service started running at: {time}", DateTimeOffset.Now);
 
                TimeSpan lastrunTime = DateTime.Now.Subtract(_lastRun);
                if (lastrunTime.TotalMinutes >= _pollInterval)
                {
                    _lastRun = DateTime.Now;
                    try
                    {
                        //Call the Acme API to get employees and exclusions 
                        var employees = await GetEmployees();
                        var exclusions = await GetExclusions();
                        //Loop through all correspondence processors and call process. More can be added in the startup class
                        foreach (ICorrespondenceProcessor processor in _correspondenceProcessors)
                        {
                            await processor.Process(employees, exclusions);
                        }
                    }
                    catch(Exception ex)
                    {
                        //Log the entire erroincluding callstack and inner exceptions
                        _logger.LogError(ex.ToString());
                    }
                 
                }
                await Task.Delay(5000, stoppingToken);
              
            }
        }

        public override void Dispose()
        {
            if (_httpClient != null)
                _httpClient.Dispose();
            base.Dispose();
        }
    }
}
