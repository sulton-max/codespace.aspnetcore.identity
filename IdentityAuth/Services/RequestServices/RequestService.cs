using IdentityAuth.Services.RequestServices.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Routing;

namespace IdentityAuth.Services.RequestServices;

public class RequestService : IRequestService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUrlHelper _urlHelper;

    public RequestService(IHttpContextAccessor httpContextAccessor, IUrlHelperFactory urlHelperFactory)
    {
        _httpContextAccessor = httpContextAccessor;

        if (_httpContextAccessor.HttpContext is null)
            throw new HttpRequestException("Can't access http context");

        var actionContext = new ActionContext(_httpContextAccessor.HttpContext, _httpContextAccessor.HttpContext.GetRouteData(),
            new ActionDescriptor());
        _urlHelper = urlHelperFactory.GetUrlHelper(actionContext);
    }

    public string GetActionUrl(string actionName, string controllerName, object? routeValues = null)
    {
        if (_httpContextAccessor.HttpContext is null)
            throw new HttpRequestException("Can't access http context");

        return _urlHelper.Action(actionName, controllerName, routeValues, _httpContextAccessor.HttpContext.Request.Scheme) ??
               throw new RouteCreationException("Can't get action url");
    }
}