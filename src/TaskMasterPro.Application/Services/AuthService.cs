using TaskManagerPro.TaskManagerPro.Interfaces;
using TaskManagerPro.TaskMasterPro.Application.DTOs.Auth;
using TaskManagerPro.TaskMasterPro.Domain;
using TaskManagerPro.TaskMasterPro.Infrastructure.Auth;
using TaskManagerPro.TaskMasterPro.Infrastructure.Services;
using Task = System.Threading.Tasks.Task;

namespace TaskManagerPro.TaskMasterPro.Application.Services;

public class AuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly GenerateTokenService _generateTokenService;

    public AuthService(IUserRepository userRepository, IPasswordHasher passwordHasher,
        GenerateTokenService generateTokenService)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _generateTokenService = generateTokenService;
    }
    /*
    public async Task<User?> Login(string email, string password)
    {
        var user = await _userRepository.GetByEmailAsync(email);
        if (user == null) return null;
        var isValid=_passwordHasher.Verify(password, user.Password);
        return isValid ? user : null;

    }
    */

    public async Task Register(string email, string password)
    {
        try
        {
            var user = new User
            {
                Email = email,
                Password = _passwordHasher.Hash(password)
            };
            await _userRepository.AddAsync(user);
        }
        catch (Exception exception)
        {
            throw new Exception("Error al insertar el usuario: " + exception.Message);
        }
    }

    public Task Logout()
    {
        throw new NotImplementedException();
    }

    public async Task<AuthRecordDto?> Login(LoginRequestDto loginRequestDto)
    {
        var user = await _userRepository.GetByEmailAsync(loginRequestDto.Email);
#pragma warning disable CS8603 // Possible null reference return.
        if (user == null) return null;
#pragma warning restore CS8603 // Possible null reference return.

        var isPasswordValid = _passwordHasher.Verify(loginRequestDto.Password, user.Password);

        if (isPasswordValid)
        {
            var authResponse = await _generateTokenService.GenerateTokensAsync(user);
            return new AuthRecordDto(user.Id, user.Email, authResponse.AccessToken);
        }

        return null;
    }
}