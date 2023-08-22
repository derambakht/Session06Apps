using System.Security.Claims;
using Core;
using Infrastructure.Common;
using Infrastructure.Models.Dto.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace SampleAPI.CustomAttributes;

public class CheckPermissoinAttribute : ActionFilterAttribute
{
  public override void OnActionExecuting(ActionExecutingContext context)
  {
    var userId = Guid.Parse(context.HttpContext.User.Claims
    .FirstOrDefault(q => q.Type == ClaimTypes.Name).Value);

    var cacheKey = string.Format(CacheKeys.UserPermission, userId);

    var routes = context.ActionDescriptor.RouteValues;
    var controllerName = routes["controller"].ToLower();
    var actionName = routes["action"].ToLower();

    var cache = (IMemoryCache)context.HttpContext.RequestServices
  .GetService(typeof(IMemoryCache));



    var userPermissions = new List<UserPermissionCacheDto>();

    if (!cache.TryGetValue(cacheKey, out userPermissions))
    {
      // Set cache options.
      var cacheEntryOptions = new MemoryCacheEntryOptions()
          // Keep in cache for this time, reset time if accessed.
          .SetSlidingExpiration(TimeSpan.FromSeconds(120));

      var dbContext = (SampleDbContext)context.HttpContext.RequestServices
      .GetService(typeof(SampleDbContext));

      userPermissions = dbContext.UserPermission.Include(q => q.Permission)
         .Where(q => q.UserId == userId)
         .Select(q => new UserPermissionCacheDto
         {
           ControllerName = q.Permission.ControllerName,
           ActionName = q.Permission.ActionName
         }).ToList();

      // Save data in cache.
      cache.Set(cacheKey, userPermissions, cacheEntryOptions);
    
    }


    var permission = userPermissions.Any(q => q.ControllerName.ToLower() == controllerName.ToLower()
    && q.ActionName.ToLower() == actionName.ToLower());

    if (!permission)
    {
      context.Result = new BadRequestObjectResult(context.ModelState);
    }
    else
    {
      base.OnActionExecuting(context);
    }
  }
}