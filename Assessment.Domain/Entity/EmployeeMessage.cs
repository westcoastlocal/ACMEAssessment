using System;
using System.Collections.Generic;
using System.Text;

namespace Assessment.Domain.Entity
{
    public class EmployeeMessage
    {
        public int EmployeeMessageId { get; set; }
        public int EmployeeId { get; set; }
        public DateTime MessageDate { get; set; }
        public string MessageType { get; set; }
    }
}
