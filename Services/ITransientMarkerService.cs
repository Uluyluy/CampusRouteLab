using Microsoft.AspNetCore.SignalR;

namespace CampusRouteLab.Services;

public interface ITransientMarkerService
{
    // уникальный маркер, который генерируется каждый раз при создании нового экземпляра сервиса
    Guid MarkerId{get;}
}