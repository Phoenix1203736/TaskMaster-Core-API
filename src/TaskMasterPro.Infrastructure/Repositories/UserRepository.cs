using TaskManagerPro.TaskManagerPro.Interfaces;
using TaskManagerPro.TaskMasterPro.Domain;

namespace TaskManagerPro.TaskMasterPro.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<UserEntity?> GetByEmailAsync(string email) => await _context.User.FindAsync(email);

    public async Task addAsync(UserEntity user)
    {
        await _context.User.AddAsync(user);
        await _context.SaveChangesAsync();
    }
}