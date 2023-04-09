namespace IdentityAuth.Services.RequestServices.Interface;

public interface IRequestService
{
    string GetActionUrl(string actionName, string controllerName, object? routeValues = null);
}