using System.Threading.Tasks;
using TaskManagerPro.TaskMasterPro.Domain;

namespace TaskManagerPro.TaskManagerPro.Interfaces;

public interface IUserRepository
{
    Task<UserEntity?> GetByEmailAsync(string email);
    Task AddAsync(UserEntity user);
    Task<UserEntity> Login(string email, string password);
    Task Logout();
    Task Register(string email, string password);

}