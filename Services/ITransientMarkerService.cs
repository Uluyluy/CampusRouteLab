using Microsoft.AspNetCore.SignalR;

namespace CampusRouterLab.Services;

public interface ITransientMarkerService
{
    // уникальный маркер, который генерируется каждый раз при создании нового экземпляра сервиса
    Guid MarkerId{get;}
}