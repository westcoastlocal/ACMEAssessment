using Assessment.Application.Interfaces;
using Assessment.Domain;
using Assessment.Domain.Entity;
using Assessment.Infrastructure;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Assessment.Application.Processors
{
    /// <summary>
    /// Send birthday wishes to employees provided they are not in the exclusions list and have been sent an email already
    /// </summary>
    public class BirthdayCorrespondenceProcessor : ICorrespondenceProcessor

    {
        public string CorrespondenceType => "birthday";
        private readonly IConfiguration _config;
        private readonly IMessenger _messenger;
        private readonly IMessageRepository _repository;

        public BirthdayCorrespondenceProcessor(IConfiguration configuration, IMessenger messenger, IMessageRepository repository)
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
        /// Works out who has birthdays and are not excluded or have already been sent a message today and saves message to the repo 
        /// </summary>
        /// <param name="employees"></param>
        /// <param name="exclusions"></param>
        /// <returns></returns>
        public async Task<bool> Process(IEnumerable<Employee> employees, IEnumerable<int> exclusions)
        {

            var today = DateTime.Now;
            //This calculates who to send messages to. It could be one long LINQ query
            var employeeMessagesSent = GetEmployeesThatReceivedMessagesToday();
            var employeeBirthdays = employees.Where(e => e.DateOfBirth.Day == today.Day && e.DateOfBirth.Month == today.Month && e.EmploymentStartDate.Date <= today.Date && !e.EmploymentEndDate.HasValue).ToList();
            var employeesWithoutExclusions = employeeBirthdays.Where(e => !exclusions.ToList().Contains(e.Id)).ToList();
            var employeesToSend = employeesWithoutExclusions.Where(e => !employeeMessagesSent.ToList().Contains(e.Id)).ToList();

            foreach (var employee in employeesToSend)
            {
                await _messenger.SendMessageAsync(_config["ToEmailAddress"], _config["FromEmailAddress"], "Happy Birthday", $"Happy Birthday {employee.Name}, { employee.Lastname}");
                SaveEmployeeMessageSent(employee.Id);
            }
            return true;
        }
    }
}
