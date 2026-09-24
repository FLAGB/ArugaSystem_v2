using AndroidWebAPI.Models;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;

namespace AndroidWebAPI.Data
{
    public class VaccineRepository
    {
        private readonly string _connectionString;

        public VaccineRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        // ========================================
        // GET ALL
        // ========================================
        public async Task<IEnumerable<Vaccine>> GetAllAsync()
        {
            using IDbConnection connection = new SqlConnection(_connectionString);

            string sql = @"
                SELECT *
                FROM Vaccines
                ORDER BY VaccineName";

            return await connection.QueryAsync<Vaccine>(sql);
        }

        // ========================================
        // GET BY ID
        // ========================================
        public async Task<Vaccine?> GetByIdAsync(int vaccineId)
        {
            using IDbConnection connection = new SqlConnection(_connectionString);

            string sql = @"
                SELECT *
                FROM Vaccines
                WHERE VaccineID = @VaccineID";

            return await connection.QueryFirstOrDefaultAsync<Vaccine>(
                sql,
                new { VaccineID = vaccineId });
        }

        // ========================================
        // CREATE
        // ========================================
        public async Task<Vaccine> CreateAsync(Vaccine vaccine)
        {
            using IDbConnection connection = new SqlConnection(_connectionString);

            vaccine.CreatedAt = DateTime.Now;

            string sql = @"
                INSERT INTO Vaccines
                (
                    VaccineName,
                    Abbreviation,
                    Description,
                    TargetDisease,
                    RecommendedAge,
                    AgeCategory,
                    NumberOfRequiredDoses,
                    DoseInterval,
                    AdministrationRoute,
                    Status,
                    CreatedAt
                )
                VALUES
                (
                    @VaccineName,
                    @Abbreviation,
                    @Description,
                    @TargetDisease,
                    @RecommendedAge,
                    @AgeCategory,
                    @NumberOfRequiredDoses,
                    @DoseInterval,
                    @AdministrationRoute,
                    @Status,
                    @CreatedAt
                );

                SELECT CAST(SCOPE_IDENTITY() AS INT);";

            vaccine.VaccineID = await connection.QuerySingleAsync<int>(sql, vaccine);

            return vaccine;
        }

        // ========================================
        // UPDATE
        // ========================================
        public async Task<Vaccine> UpdateAsync(Vaccine vaccine)
        {
            using IDbConnection connection = new SqlConnection(_connectionString);

            vaccine.UpdatedAt = DateTime.Now;

            string sql = @"
                UPDATE Vaccines
                SET
                    VaccineName = @VaccineName,
                    Abbreviation = @Abbreviation,
                    Description = @Description,
                    TargetDisease = @TargetDisease,
                    RecommendedAge = @RecommendedAge,
                    AgeCategory = @AgeCategory,
                    NumberOfRequiredDoses = @NumberOfRequiredDoses,
                    DoseInterval = @DoseInterval,
                    AdministrationRoute = @AdministrationRoute,
                    Status = @Status,
                    UpdatedAt = @UpdatedAt
                WHERE VaccineID = @VaccineID";

            await connection.ExecuteAsync(sql, vaccine);

            return vaccine;
        }

        // ========================================
        // DELETE
        // ========================================
        public async Task<bool> DeleteAsync(int vaccineId)
        {
            using IDbConnection connection = new SqlConnection(_connectionString);

            string sql = @"
                DELETE FROM Vaccines
                WHERE VaccineID = @VaccineID";

            int rows = await connection.ExecuteAsync(sql, new
            {
                VaccineID = vaccineId
            });

            return rows > 0;
        }
    }
}