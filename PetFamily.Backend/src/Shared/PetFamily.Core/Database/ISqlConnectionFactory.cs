using System.Data;

namespace PetFamily.Core.Database;

public interface ISqlConnectionFactory
{
    IDbConnection Create();
}