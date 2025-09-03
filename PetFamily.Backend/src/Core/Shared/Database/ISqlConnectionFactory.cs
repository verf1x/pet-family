using System.Data;

namespace PetFamily.Framework.Database;

public interface ISqlConnectionFactory
{
    IDbConnection Create();
}