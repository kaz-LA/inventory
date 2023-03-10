using Aine.Inventory.Core.Interfaces;
using Aine.Inventory.SharedKernel;
using Aine.Inventory.SharedKernel.Interfaces;
using Aine.Inventory.SharedKernel.Security;
using Ardalis.Result;

namespace Aine.Inventory.Infrastructure;

[Inject]
internal class DefaultUserAuthenticator  : IUserAuthenticator
{
  public Task<Result<IUser>> AuthenticateUserAsync(UserModel user)
  {
    return Task.FromResult(Result<IUser>.Success(User.Create(0, user.UserName, user.CorpName)));
  }
}
