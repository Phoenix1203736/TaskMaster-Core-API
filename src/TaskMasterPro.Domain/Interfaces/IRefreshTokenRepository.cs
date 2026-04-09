namespace TaskManagerPro.TaskMasterPro.Domain.Interfaces;

public interface IRefreshTokenRepository
{
    System.Threading.Tasks.Task SaveAsync(RefreshToken refreshToken);
    Task<RefreshToken?> GetByTokenAsync(string token);
}