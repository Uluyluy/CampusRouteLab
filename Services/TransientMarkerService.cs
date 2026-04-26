namespace CampusRouteLab.Services;

public class TransientMarkerService : ITransientMarkerService
{
    public TransientMarkerService()
    {
        MarkerId = Guid.NewGuid();
    }

    public Guid MarkerId{get;}
}