using TaskManagerPro.TaskMasterPro.Domain;

namespace TaskManagerPro.TaskManagerPro.Interfaces;

public interface ITokenService
{
    string GenerateToken(UserEntity userEntity);
}