namespace CampusRouterLab.Services;

// Хранит информацию о самом приложении (одна на все приложение)

public interface IAppInfoService 
{ 
    // уникальный ID экземпляра приложения
    Guid AppInstanceId { get; } 
    // время запуска приложения
    DateTime StartedAt { get; } 
} 