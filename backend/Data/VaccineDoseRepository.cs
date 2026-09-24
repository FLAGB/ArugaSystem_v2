using AndroidWebAPI.Models;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;

namespace AndroidWebAPI.Data
{
    public class VaccineDoseRepository
    {
        private readonly string _connectionString;

        public VaccineDoseRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        // ========================================
        // GET ALL
        // ========================================
        public async Task<IEnumerable<VaccineDose>> GetAllAsync()
        {
            using IDbConnection connection = new SqlConnection(_connectionString);

            string sql = @"
                SELECT *
                FROM VaccineDoses
                ORDER BY VaccineID, DoseNumber";

            return await connection.QueryAsync<VaccineDose>(sql);
        }

        public async Task<IEnumerable<VaccineDose>> GetByVaccineIdAsync(int vaccineId)
{
    using IDbConnection connection = new SqlConnection(_connectionString);

    string sql = @"
        SELECT *
        FROM VaccineDoses
        WHERE VaccineID = @VaccineID
        ORDER BY DoseNumber";

    return await connection.QueryAsync<VaccineDose>(
        sql,
        new { VaccineID = vaccineId });
}

        // ========================================
        // GET BY ID
        // ========================================
        public async Task<VaccineDose?> GetByIdAsync(int doseId)
        {
            using IDbConnection connection = new SqlConnection(_connectionString);

            string sql = @"
                SELECT *
                FROM VaccineDoses
                WHERE DoseID = @DoseID";

            return await connection.QueryFirstOrDefaultAsync<VaccineDose>(
                sql,
                new { DoseID = doseId });
        }

        // ========================================
        // CREATE
        // ========================================
        public async Task<VaccineDose> CreateAsync(VaccineDose dose)
        {
            using IDbConnection connection = new SqlConnection(_connectionString);

            string sql = @"
                INSERT INTO VaccineDoses
                (
                    VaccineID,
                    DoseNumber,
                    MinIntervalDays
                )
                VALUES
                (
                    @VaccineID,
                    @DoseNumber,
                    @MinIntervalDays
                );

                SELECT CAST(SCOPE_IDENTITY() AS INT);";

            dose.DoseID = await connection.QuerySingleAsync<int>(sql, dose);

            return dose;
        }

        // ========================================
        // UPDATE
        // ========================================
        public async Task<VaccineDose> UpdateAsync(VaccineDose dose)
        {
            using IDbConnection connection = new SqlConnection(_connectionString);

            string sql = @"
                UPDATE VaccineDoses
                SET
                    VaccineID = @VaccineID,
                    DoseNumber = @DoseNumber,
                    MinIntervalDays = @MinIntervalDays
                WHERE DoseID = @DoseID";

            await connection.ExecuteAsync(sql, dose);

            return dose;
        }

        // ========================================
        // DELETE
        // ========================================
        public async Task<bool> DeleteAsync(int doseId)
        {
            using IDbConnection connection = new SqlConnection(_connectionString);

            string sql = @"
                DELETE FROM VaccineDoses
                WHERE DoseID = @DoseID";

            int rows = await connection.ExecuteAsync(sql, new
            {
                DoseID = doseId
            });

            return rows > 0;
        }

    }
}