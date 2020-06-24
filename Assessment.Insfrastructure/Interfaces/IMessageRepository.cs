using Assessment.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Assessment.Infrastructure
{
    public interface IMessageRepository
    {
        IEnumerable<EmployeeMessage> GetEmployeeMessages(DateTime dateSent, string messageType);
        void SaveEmployeeMessages(IEnumerable<EmployeeMessage> employeeMessages);
        int SaveEmployeeMessage(EmployeeMessage employeeMessage);
    }
}
