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

    // 1. El tipo de retorno debe ser Task<User> porque vas a devolver al usuario si todo sale bien
    public async Task<UserEntity> Login(string email, string password)
    {
        // 2. Buscamos al usuario por email
        var user = _context.User.FirstOrDefault(u => u.Email == email);
        // 3. PRIMER FILTRO: ¿Existe el usuario? 
        // Si no existe, nos salimos de inmediato para no chocar
#pragma warning disable CS8603 // Possible null reference return.
        if (user == null) return null;
#pragma warning restore CS8603 // Possible null reference return.
        // 4. SEGUNDO FILTRO: ¿La contraseña es correcta?
        // Ya estamos seguros de que 'user' no es nulo, así que podemos leer 'user.Password'
        var isPasswordValid = _passwordHasher.Verify(password, user.Password);
        if (isPasswordValid) return user;
        // Si la clave estuvo mal, regresamos nulo
#pragma warning disable CS8603 // Possible null reference return.
        return null;
#pragma warning restore CS8603 // Possible null reference return.
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