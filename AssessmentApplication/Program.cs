using Assessment.Domain;
using Assessment.Infrastructure;
using Newtonsoft;
using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Assessment.Infrastructure.Messengers;

namespace AssessmentApplication
{
    class Program
    {
        const string _url = @"https://eohmc-acme-api.azurewebsites.net/api";
        public static List<Employee> GetBirthday(IList<Employee> employees, DateTime today)
        {

            return employees.Where(e => e.DateOfBirth.Day == today.Day && e.DateOfBirth.Month == today.Month && e.EmploymentStartDate.Date <= today.Date && !e.EmploymentEndDate.HasValue).ToList();
            
        }

        public static IList<Employee> GetWorkAnniverseries(IList<Employee> employees, DateTime today)
        {

            // return employees.Where(e => e.DateOfBirth.Month == DateTime.Now.Day && e.DateOfBirth.Month == DateTime.Now.Month).ToList();
            return employees.Where(e => e.EmploymentStartDate.Day == today.Day && e.EmploymentStartDate.Month == today.Month && e.EmploymentStartDate.Year <= today.Date.AddYears(-1).Year && !e.EmploymentEndDate.HasValue).ToList();
        }

        public static async Task<string> GetApiData(string endpoint)
        {
            var client = new ApiRestClient(_url);
            var response = await client.GetAsync(endpoint);
            return response;

        }
       
        static void Main(string[] args)
        {
            IMessenger messenger = new SendGridMessenger(null);
            var response = GetApiData("Employees");
            List<Employee> employees = JsonConvert.DeserializeObject<List<Employee>>(response.Result);
            var birthdays = GetBirthday(employees, new DateTime(2020, 1, 6));
            var anniversaries = GetWorkAnniverseries(employees, new DateTime(2020, 4, 1));
            birthdays.ForEach(e => messenger.SendMessageAsync("rafaelg.email@gmail.com", "noreply@test.com", "Happy Birthday", $"Happy Birthday {e.Name}, { e.Lastname}"));
            response = GetApiData("BirthdayWishExclusions");
            var exclusions = JsonConvert.DeserializeObject<List<int>>(response.Result);
            Console.ReadLine();

        }
    }
}
