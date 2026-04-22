using CampusRouterLab.Models;
using Microsoft.AspNetCore.Http.HttpResults;

namespace CampusRouterLab.Services;

public class DiagnosticsReportService
{
    private readonly IAppInfoService _appInfo;
    private readonly IRequestContextService _requestContext;
    private readonly ITransientMarkerService _transientMarker;
    private readonly IStudentCatalogService _studentCatalog;


    // Внедрение зависимостей через конструктор
    public DiagnosticsReportService(
        IAppInfoService appInfo,
        IRequestContextService requestContext,
        ITransientMarkerService transientMarker,
        IStudentCatalogService studentCatalog)
    {
        _appInfo = appInfo;
        _requestContext = requestContext;
        _transientMarker = transientMarker;
        _studentCatalog = studentCatalog;
    }


     // Метод для получения отчета
    public object GetReport()
    {
        // Возвращает object (анонимный тип)
        return new
        {
            AppInstance = new
            {
                Id = _appInfo.AppInstanceId,
                StartedAt = _appInfo.StartedAt
            },
            RequestContext = new
            {
                RequestId = _requestContext.RequestId,
                CreatedAt = _requestContext.CreatedAt
            },
            TransientMarker = new
            {
                MarkerId = _transientMarker.MarkerId
            },
            StudentCatalog = _studentCatalog.GetAllGroups().Count()
        };
    }
}