using System.Data;
using Npgsql;
using PetFamily.Core.Database;

namespace Infrastructure.Postgres;

public class SqlConnectionFactory(string connectionString) : ISqlConnectionFactory
{
    public IDbConnection Create()
        => new NpgsqlConnection(connectionString);
}