using Assessment.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Assessment.Infrastructure
{ 
    public interface IMessenger
    {

        Task<bool> SendMessageAsync(string to, string from, string subject, string message);

    }
}
