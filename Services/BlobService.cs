using System.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ST10320806_Part1.Services
{
    public class BlobService
    {
        private readonly string _connectionString;
        private readonly ILogger<BlobService> _logger;

        public BlobService(IConfiguration configuration, ILogger<BlobService> logger)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _logger = logger;
        }

        // Code for inserting data into the BlobTable
        public async Task<bool> InsertBlobAsync(byte[] blobImage)
        {
            const string query = @"INSERT INTO BlobTable (BlobImage) VALUES (@BlobImage)";

            try
            {
                await using var connection = new SqlConnection(_connectionString);
                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@BlobImage", blobImage);

                await connection.OpenAsync();
                await command.ExecuteNonQueryAsync();
                _logger.LogInformation("Blob data inserted successfully");

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error inserting blob data: {Message}", ex.Message);
                return false;
            }
        }
    }
}
