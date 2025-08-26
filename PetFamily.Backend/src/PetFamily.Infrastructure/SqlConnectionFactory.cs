using System.Data;
using Microsoft.Extensions.Configuration;
using Npgsql;
using PetFamily.Application.Database;

namespace PetFamily.Infrastructure;

public class SqlConnectionFactory(string connectionString) : ISqlConnectionFactory
{
    public IDbConnection Create()
        => new NpgsqlConnection(connectionString);
}