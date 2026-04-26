using CampusRouteLab.Middleware;
using CampusRouteLab.Services;
using Microsoft.AspNetCore.Routing;

var builder = WebApplication.CreateBuilder(args);

// 1. Регистрируем все наши сервисы
builder.Services.AddCampusServices();

var app = builder.Build();

// 2. Подключаем наш custom middleware
app.UseRequestAudit();

// 3. БИЗНЕС-МАРШРУТЫ

// Главная страница
app.MapGet("/", () => 
    "CampusRouteLab - Учебный диагностический веб-сервис\n\n" +
    "Основные разделы:\n" +
    "- /students - список групп\n" +
    "- /reports - отчеты\n" +
    "- /routes - все маршруты\n" +
    "- /diag/lifetimes - диагностика DI");

// /students - все группы
app.MapGet("/students", (IStudentCatalogService catalog) =>
{
    var groups = catalog.GetAllGroups().Select(g => new 
    { 
        GroupName = g.Name, 
        StudentCount = g.Students.Count 
    });
    return Results.Ok(groups);
});

// /students/{group} - одна группа
app.MapGet("/students/{group}", (string group, IStudentCatalogService catalog) =>
{
    try
    {
        var groupData = catalog.GetGroup(group);
        return Results.Ok(new 
        { 
            GroupName = groupData.Name,
            Students = groupData.Students.Select(s => new 
            { 
                s.Id, 
                s.Name, 
                s.Email 
            })
        });
    }
    catch (KeyNotFoundException)
    {
        return Results.NotFound(new { error = $"Группа '{group}' не найдена" });
    }
});

// /students/{group}/{id} - конкретный студент
app.MapGet("/students/{group}/{id}", (string group, Guid id, IStudentCatalogService catalog) =>
{
    try
    {
        var student = catalog.GetStudent(group, id);
        return Results.Ok(new 
        { 
            student.Id, 
            student.Name, 
            student.Email, 
            student.GroupName 
        });
    }
    catch (KeyNotFoundException ex)
    {
        return Results.NotFound(new { error = ex.Message });
    }
});

// /reports/{section?} - необязательный параметр
app.MapGet("/reports/{section?}", (string? section) =>
{
    section ??= "overview"; // значение по умолчанию
    return Results.Ok(new 
    { 
        Section = section,
        Message = $"Отчет: {section}",
        GeneratedAt = DateTime.UtcNow
    });
});

// /portal/{module=home}/{page=index}/{id?} - значения по умолчанию
app.Map("/portal/{module=home}/{page=index}/{id?}", 
    (string module, string page, string? id, HttpContext context) =>
    {
        var routeValues = context.Request.RouteValues;
        
        // Проверяем, был ли параметр в исходном запросе
        var hasModule = routeValues.ContainsKey("module");
        var hasPage = routeValues.ContainsKey("page");
        
        return $@"
        Module: {module} ({(hasModule && module != "home" ? "from URL" : "default value")})
        Page: {page} ({(hasPage && page != "index" ? "from URL" : "default value")})
        Id: {id ?? "not provided"} ({(id != null ? "from URL" : "optional")})";
    });

// Catch-all маршрут
app.MapGet("/files/{**path}", (string? path) =>
{
    if (string.IsNullOrEmpty(path))
    {
        return "Files root - provide a path like /files/css/style.css";
    }
    return $"Captured path: {path}";
});

// 4. ДИАГНОСТИЧЕСКИЕ МАРШРУТЫ

// /routes - все зарегистрированные конечные точки
app.MapGet("/routes", (IEnumerable<EndpointDataSource> endpointSources) =>
{
    var endpoints = endpointSources.SelectMany(source => source.Endpoints);
    var result = new System.Text.StringBuilder();
    
    foreach (var endpoint in endpoints)
    {
        if (endpoint is RouteEndpoint routeEndpoint)
        {
            result.AppendLine($"{routeEndpoint.RoutePattern.RawText}");
        }
    }
    
    return Results.Text(result.ToString());
});

// /diag/lifetimes - демонстрация жизненных циклов
app.MapGet("/diag/lifetimes", (
    IAppInfoService appInfo,
    IRequestContextService requestContext,
    ITransientMarkerService transientMarker,
    DiagnosticsReportService reportService) =>
{
    return Results.Ok(new
    {
        DirectAccess = new
        {
            Singleton = new { AppInstanceId = appInfo.AppInstanceId },
            Scoped = new { RequestId = requestContext.RequestId },
            Transient = new { MarkerId = transientMarker.MarkerId }
        },
        FromDiagnosticsService = reportService.GetReport()
    });
});

// /diag/lifetimes/check - проверка transient, scoped, singleton
app.MapGet("/diag/lifetimes/check", (
    IAppInfoService singleton1, 
    IAppInfoService singleton2,
    IRequestContextService scoped1,
    IRequestContextService scoped2,
    ITransientMarkerService transient1, 
    ITransientMarkerService transient2) =>
{
    return Results.Ok(new
    {
        Singleton = new
        {
            FirstResolution = singleton1.AppInstanceId,
            SecondResolution = singleton2.AppInstanceId,
            AreSame = singleton1.AppInstanceId == singleton2.AppInstanceId,
            Explanation = "Singleton создаётся один раз для всего приложения"
        },
        Scoped = new
        {
            FirstResolution = scoped1.RequestId,
            SecondResolution = scoped2.RequestId,
            AreSame = scoped1.RequestId == scoped2.RequestId,
            Explanation = "Scoped создаётся один раз в рамках запроса"
        },
        Transient = new
        {
            FirstResolution = transient1.MarkerId,
            SecondResolution = transient2.MarkerId,
            AreSame = transient1.MarkerId == transient2.MarkerId,
            Explanation = "Transient создаётся заново при каждом обращении"
        },
        Summary = new
        {
            SingletonMatches = singleton1.AppInstanceId == singleton2.AppInstanceId,
            ScopedMatches = scoped1.RequestId == scoped2.RequestId,
            TransientMatches = transient1.MarkerId == transient2.MarkerId,
            Message = "Только Transient сервисы различаются в рамках одного запроса!"
        }
    });
});

// /diag/request-services - получение через HttpContext.RequestServices
app.MapGet("/diag/request-services", (HttpContext context) =>
{
    var appInfo = context.RequestServices.GetRequiredService<IAppInfoService>();
    return Results.Ok(new
    {
        AppInstanceId = appInfo.AppInstanceId,
        StartedAt = appInfo.StartedAt,
        Method = "HttpContext.RequestServices.GetRequiredService"
    });
});

// /diag/app-services - получение через app.Services (только singleton!)
app.MapGet("/diag/app-services", () =>
{
    // Получаем сервис из корневого провайдера
    var appInfo = app.Services.GetRequiredService<IAppInfoService>();
    
    return Results.Ok(new
    {
        AppInstanceId = appInfo.AppInstanceId,
        StartedAt = appInfo.StartedAt,
        Method = "app.Services.GetRequiredService",
        Note = "Доступно только для Singleton-сервисов"
    });
});

// 5. ЗАПУСК ПРИЛОЖЕНИЯ
app.Run();