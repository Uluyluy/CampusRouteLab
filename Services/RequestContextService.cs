namespace CampusRouteLab.Services;

public class RequestContextService : IRequestContextService
{
    public RequestContextService()
    {
        RequestId = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
    }

    public Guid RequestId { get; } 
    public DateTime CreatedAt { get; } 
}