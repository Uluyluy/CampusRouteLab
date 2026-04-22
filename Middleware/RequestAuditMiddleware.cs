public class RequestAuditMiddleware 
{ 
public RequestAuditMiddleware(RequestDelegate next, IAppInfoService 
appInfo) { ... } 
public Task InvokeAsync(HttpContext context, IRequestContextService 
requestContext, 
ITransientMarkerService transientMarker) { ... } 
} 