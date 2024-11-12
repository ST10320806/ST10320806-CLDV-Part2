using ST10320806_Part1.Models;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ST10320806_Part1.Services
{
    public class OrderService
    {
        private readonly SqlConnection _sqlConnection;
        private readonly ILogger<OrderService> _logger;

        public OrderService(IConfiguration configuration, ILogger<OrderService> logger)
        {
            _sqlConnection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
            _logger = logger;
        }

        // Code for inserting data into OrderTable
        public async Task InsertOrderAsync(OrderProfile order)
        {
            const string query = @"
            INSERT INTO OrderTable (OrderNumber)
            VALUES (@OrderNumber)";

            try
            {
                using var command = new SqlCommand(query, _sqlConnection);
                command.Parameters.AddWithValue("@OrderNumber", order.OrderNumber);

                await _sqlConnection.OpenAsync();
                await command.ExecuteNonQueryAsync();
                await _sqlConnection.CloseAsync();

                _logger.LogInformation("Order data inserted successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error inserting order: {ex.Message}");
                throw; // Re-throwing the exception to handle it in the controller or upper layer
            }
        }

        // Method to retrieve all orders
        public async Task<IEnumerable<OrderProfile>> GetOrdersAsync()
        {
            const string query = "SELECT OrderID, OrderNumber FROM OrderTable";
            var orders = new List<OrderProfile>();

            try
            {
                using var command = new SqlCommand(query, _sqlConnection);
                await _sqlConnection.OpenAsync();
                using var reader = await command.ExecuteReaderAsync();

                while (reader.Read())
                {
                    orders.Add(new OrderProfile
                    {
                        OrderID = reader.GetInt32(0),
                        OrderNumber = reader.GetString(1)
                    });
                }

                await _sqlConnection.CloseAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving orders: {ex.Message}");
            }

            return orders;
        }
    }
}


