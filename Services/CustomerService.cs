using System.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ST10320806_Part1.Models;

namespace ST10320806_Part1.Services
{
    public class CustomerService
    {
        private readonly string _connectionString;
        private readonly ILogger<CustomerService> _logger;

        public CustomerService(IConfiguration configuration, ILogger<CustomerService> logger)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _logger = logger;
        }

        public async Task<bool> InsertCustomerAsync(CustomerProfile profile)
        {
            var query = @"INSERT INTO CustomerTable (FirstName, SecondName, Email, PhoneNumber)
                          VALUES (@FirstName, @SecondName, @Email, @PhoneNumber)";

            try
            {
                await using var connection = new SqlConnection(_connectionString);
                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@FirstName", profile.FirstName);
                command.Parameters.AddWithValue("@SecondName", profile.LastName);
                command.Parameters.AddWithValue("@Email", profile.Email);
                command.Parameters.AddWithValue("@PhoneNumber", profile.PhoneNumber);

                await connection.OpenAsync();
                await command.ExecuteNonQueryAsync();
                _logger.LogInformation("Successfully inserted customer {FirstName} {LastName}", profile.FirstName, profile.LastName);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error inserting customer: {Message}", ex.Message);
                return false;
            }
        }
    }
}
