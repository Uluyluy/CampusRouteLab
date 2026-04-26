namespace CampusRouteLab.Services;

// Хранит информацию о текущем HTTP-запросе (один на запрос)
public interface IRequestContextService 
{ 
    // уникальный ID текущего запроса
    Guid RequestId { get; } 
    // время создания контекста (начала обработки запроса)
    DateTime CreatedAt { get; } 
} 