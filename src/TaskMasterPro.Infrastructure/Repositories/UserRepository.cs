using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using TaskManagerPro.TaskManagerPro.Interfaces;
using TaskManagerPro.TaskMasterPro.Domain;

namespace TaskManagerPro.TaskMasterPro.Infrastructure.Repositories;

public class UserRepository(AppDbContext context, IPasswordHasher passwordHasher) : IUserRepository
{
    private readonly AppDbContext _context = context;
    private IPasswordHasher _passwordHasher = passwordHasher;

    // Buscamos usando una expresión Lambda porque el Email no es la Llave Primaria
    public async Task<UserEntity?> GetByEmailAsync(string email)
    {
        return await _context.User
            .FirstOrDefaultAsync(u => u.Email == email);
    }


    public async Task AddAsync(UserEntity user)
    {
        await _context.User.AddAsync(user);
        await _context.SaveChangesAsync();
    }

    public Task Login(string email, string password)
    {
        return null;
    }

    public Task Logout()
    {
        throw new NotImplementedException();
    }

    public async Task Register(string email, string password)
    {
        try
        {
            var user = new UserEntity();
            user.Email = email;
            user.Password = _passwordHasher.Hash(password);
            await _context.User.AddAsync(user);
            await _context.SaveChangesAsync();
        }
        catch (Exception exception)
        {
            throw new Exception(exception.Message + " Error inserting a new user ");
        }
    }
}