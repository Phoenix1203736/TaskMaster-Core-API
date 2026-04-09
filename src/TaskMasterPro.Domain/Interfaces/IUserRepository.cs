using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.Data;
using TaskManagerPro.TaskMasterPro.Application.DTOs.Auth;
using TaskManagerPro.TaskMasterPro.Domain;
using Task = System.Threading.Tasks.Task;

namespace TaskManagerPro.TaskManagerPro.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);
    Task AddAsync(User user);
    
}