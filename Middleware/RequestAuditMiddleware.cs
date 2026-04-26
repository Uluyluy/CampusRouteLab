using CampusRouteLab.Services;

namespace CampusRouteLab.Middleware;

public class RequestAuditMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IAppInfoService _appInfo;

    // В конструктор внедряем только Singleton-сервисы и RequestDelegate
    public RequestAuditMiddleware(RequestDelegate next, IAppInfoService appInfo)
    {
        _next = next;
        _appInfo = appInfo;
    }

    // В InvokeAsync внедряем Scoped и Transient сервисы
    public async Task InvokeAsync(HttpContext context, IRequestContextService requestContext, ITransientMarkerService transientMarker)
    {
        // 1. Фиксируем начало обработки
        Console.WriteLine($"[Audit] Start processing request: {requestContext.RequestId}");

        // 2. Вызываем следующий middleware в конвейере (или конечную точку)
        await _next(context);

        // 3. После обработки запроса добавляем диагностические заголовки в ответ
        context.Response.Headers.Append("X-App-Instance", _appInfo.AppInstanceId.ToString());
        context.Response.Headers.Append("X-Request-Id", requestContext.RequestId.ToString());
        context.Response.Headers.Append("X-Transient-Id", transientMarker.MarkerId.ToString());

        // 4. Дополнительная информация для раздела /diag
        if (context.Request.Path.StartsWithSegments("/diag"))
        {
            Console.WriteLine($"[Audit] Diag request detected. App: {_appInfo.AppInstanceId}, Req: {requestContext.RequestId}, Trans: {transientMarker.MarkerId}");
        }
    }
}