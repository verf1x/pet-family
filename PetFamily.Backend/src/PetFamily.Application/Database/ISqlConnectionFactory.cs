using System.Data;

namespace PetFamily.Infrastructure;

public interface ISqlConnectionFactory
{
    IDbConnection Create();
}