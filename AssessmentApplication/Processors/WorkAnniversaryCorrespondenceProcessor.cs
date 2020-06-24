using Assessment.Application.Interfaces;
using Assessment.Domain;
using Assessment.Domain.Entity;
using Assessment.Infrastructure;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assessment.Application.Processors
{
    /// <summary>
    /// Send work anniversary wishes to employees provided they are not in the exclusions list and have been sent an email already
    /// </summary>
    public class WorkAnniversaryCorrespondenceProcessor : ICorrespondenceProcessor
    {
        public string CorrespondenceType => "workanniversary";
        private readonly IConfiguration _config;
        private readonly IMessenger _messenger;
        private readonly IMessageRepository _repository;

        public WorkAnniversaryCorrespondenceProcessor(IConfiguration configuration, IMessenger messenger, IMessageRepository repository)
        {
            _config = configuration;
            _messenger = messenger;
            _repository = repository;
        }

        private IEnumerable<int> GetEmployeesThatReceivedMessagesToday()
        {
            return _repository.GetEmployeeMessages(DateTime.Now.Date, CorrespondenceType).Select(e => e.EmployeeId);
        }

        private void SaveEmployeeMessageSent(int employeeId)
        {
            _repository.SaveEmployeeMessage(new EmployeeMessage { EmployeeId = employeeId, MessageType = CorrespondenceType, MessageDate = DateTime.Now.Date });
        }


        /// <summary>
        /// Works out who has work anniversaries and are not excluded or have already been sent a message today and saves message to the repo 
        /// </summary>
        /// <param name="employees"></param>
        /// <param name="exclusions"></param>
        /// <returns></returns>
        public async Task<bool> Process(IEnumerable<Employee> employees, IEnumerable<int> exclusions)
        {
            var today = DateTime.Now;
            var employeeMessagesSent = GetEmployeesThatReceivedMessagesToday();
            var anniversaries = employees.Where(e => e.EmploymentStartDate.Day == today.Day && e.EmploymentStartDate.Month == today.Month && e.EmploymentStartDate.Year <= today.Date.AddYears(-1).Year && !e.EmploymentEndDate.HasValue).ToList();
            var anniversariesWithoutExclusions = anniversaries.Where(e => !exclusions.ToList().Contains(e.Id)).ToList();
            var anniversariesToSend = anniversariesWithoutExclusions.Where(e => !employeeMessagesSent.ToList().Contains(e.Id)).ToList();

            foreach (var employee in anniversariesToSend)
            {
                await _messenger.SendMessageAsync(_config["ToEmailAddress"], _config["FromEmailAddress"], "Work Anniversary", $"Happy Work Anniversary {employee.Name}, { employee.Lastname}");
                SaveEmployeeMessageSent(employee.Id);
            }
            return true;
        }
    }
}
