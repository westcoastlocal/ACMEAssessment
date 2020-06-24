using Assessment.Domain.Entity;
using Microsoft.Extensions.Configuration;
using System;
using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Net.WebSockets;
using System.Data.SqlClient;

namespace Assessment.Infrastructure
{
    //Repository that access the database to fetch and store messages sent to employees.
    //I used the Micro ORM, Dapper for simplicity as the database only has one table.
    public class EmployeeMessageRepository : IMessageRepository
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionstring;
        public EmployeeMessageRepository(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionstring = configuration.GetConnectionString("DefaultConnection");
        }

        /// <summary>
        /// Gets all messages sent to employees on a specific day and for a message type
        /// </summary>
        /// <param name="dateSent"></param>
        /// <param name="messageType"></param>
        /// <returns></returns>
        public IEnumerable<EmployeeMessage> GetEmployeeMessages(DateTime dateSent, string messageType)
        {
            IEnumerable<EmployeeMessage> messages = null;
            using (var connection = new SqlConnection(_connectionstring))
            {
                connection.Open();
                messages = connection.Query<EmployeeMessage>(@"SELECT * FROM [EmployeeMessage] WHERE MessageDate = @MessageDate AND MessageType = @MessageType", new { MessageDate = dateSent.Date, MessageType = messageType } );
            }
            return messages;
        }

        /// <summary>
        /// Saves employee messages sent in bulk and in a transaction
        /// </summary>
        /// <param name="employeeMessages"></param>
        public void SaveEmployeeMessages(IEnumerable<EmployeeMessage> employeeMessages)
        {
            using (var connection = new SqlConnection(_connectionstring))
            {
                connection.Open();
                var transaction = connection.BeginTransaction();
                try
                {
                    foreach (var message in employeeMessages)
                        connection.Execute("INSERT INTO [EmployeeMessage] ([EmployeeId] ,[MessageDate] ,[MessageType]) VALUES (@EmployeeId, @MessageDate, @MessageType)", message);
                    transaction.Commit();
                }
                catch (Exception)
                {
                    try
                    {
                        // Attempt to roll back the transaction.
                        transaction.Rollback();
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// Saves a message that was sent to an employee
        /// </summary>
        /// <param name="employeeMessage"></param>
        /// <returns></returns>
        public int SaveEmployeeMessage(EmployeeMessage employeeMessage)
        {
            using (var connection = new SqlConnection(_connectionstring))
            {
                connection.Open();
                return connection.Execute("INSERT INTO [EmployeeMessage] ([EmployeeId] ,[MessageDate] ,[MessageType]) VALUES (@EmployeeId, @MessageDate, @MessageType)", employeeMessage);
            }
        }
    }
}
