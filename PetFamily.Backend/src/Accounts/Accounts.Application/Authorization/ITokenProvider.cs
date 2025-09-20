using Accounts.Domain;

namespace Accounts.Application.Authorization;

public interface ITokenProvider
{
    string GenerateAccessToken(User user);
}