using ECommerce.Application.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text;

namespace ECommerce.API.Attributes
{
    public class RedisCacheAttribute : ActionFilterAttribute
    {
        private readonly int _durationInMinutes;
        public RedisCacheAttribute(int durationInMinutes)
        {
            _durationInMinutes = durationInMinutes;
        }
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var CacheService = context.HttpContext.RequestServices.GetRequiredService<ICacheServices>();
            var CacheKey = CreateCacheKey(context.HttpContext.Request);

            var Cached = await CacheService.GetAsync(CacheKey);

            if (!string.IsNullOrEmpty(Cached))
            {
                context.Result = new ContentResult()
                {
                    Content = Cached,
                    ContentType = "application/json",
                    StatusCode = StatusCodes.Status200OK
                };
                return;
            }
            var Executed = await next.Invoke();
            if (Executed.Result is OkObjectResult { Value: not null } ok)
                await CacheService.SetAsync(CacheKey, ok.Value, TimeSpan.FromMinutes(_durationInMinutes));
            return;
        }

        private static string CreateCacheKey(HttpRequest request)
        {
            var key = new StringBuilder();
            key.Append(request.Path).Append("?");

            foreach (var (k, v) in request.Query.OrderBy(q => q.Key))
            {
                key.Append(k).Append("=").Append(v).Append("&");
            }
            return key.ToString();
        }
    }
}
