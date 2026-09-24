using AndroidWebAPI.Models;
using Dapper;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;
namespace AndroidWebAPI.Data
{
    public class StaffPatientRecordsRepository
    {
        private readonly string _connectionString;

        public StaffPatientRecordsRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<IEnumerable<FamilyRecordDto>> GetAllFamiliesAsync()
{
    using IDbConnection connection = new SqlConnection(_connectionString);

    string sql = @"  
        SELECT

            p.ParentID,

            CONCAT(
                p.FirstName,' ',
                ISNULL(p.MiddleName + ' ',''),
                p.LastName
            ) AS ParentFullName,

            p.ContactNo,

            c.ChildID,

            CONCAT(
                c.FirstName,' ',
                ISNULL(c.MiddleName + ' ',''),
                c.LastName
            ) AS ChildFullName,

            c.BirthDate,
            c.Sex,
            c.Barangay,

            r.RelationshipID,
            r.RelationshipType,
            r.IsPrimaryContact,
            r.CanReceiveNotifications

        FROM ChildParentRelationship r

        INNER JOIN Parents p
            ON r.ParentID = p.ParentID

        INNER JOIN Children c
            ON r.ChildID = c.ChildID

        WHERE r.Status = 'Active'

        ORDER BY
            p.LastName,
            c.LastName";

    return await connection.QueryAsync<FamilyRecordDto>(sql);
}
    }
}