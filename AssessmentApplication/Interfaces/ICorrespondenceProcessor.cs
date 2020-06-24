using Assessment.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Assessment.Application.Interfaces
{
    public interface ICorrespondenceProcessor
    {
        string CorrespondenceType { get; }

        Task<bool> Process(IEnumerable<Employee> employees, IEnumerable<int> exclusions);
    }
}
