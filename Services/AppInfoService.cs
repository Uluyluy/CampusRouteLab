namespace CampusRouteLab.Services;

public class AppInfoService : IAppInfoService
{
    public AppInfoService()
    {
        AppInstanceId = Guid.NewGuid();
        StartedAt = DateTime.UtcNow;
        
    }

    public Guid AppInstanceId { get; }
    public DateTime StartedAt { get; }
}