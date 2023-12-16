using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text;
using Talabat.Core.Services.Contract;
namespace Talabat.API.Helpers
{
    public class CachedAttribute : Attribute, IAsyncActionFilter
    {
        private readonly int _timeToLiveInSeconds;

        public CachedAttribute(int timeToLiveInSeconds)//hna mesh l clr l 48al nta l 48al(depndacnce injecation)
        {
            _timeToLiveInSeconds = timeToLiveInSeconds;
        }
        //bthsl calling m3 l action l ast5dmth m3 l attribute
        //(abl l end point and after ma yhsl pinding mn query from query string is complete)
        //dh l by3ml filter bya2ol l action hytnfz wlh l2a
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            //RequestServices:depndance injection contoner (allow injection services)
            //Ask CLR Creating object from "ResponseCacheServices" explicitly
           var responseCacheServices= context.HttpContext.RequestServices.GetRequiredService<IResponseCacheServices>();
            var cacheKey = GenerateCacheKeyFromRequest(context.HttpContext.Request);
            var response = await responseCacheServices.GetCacheResponseAsync(cacheKey);
            if (!string.IsNullOrEmpty(response))
            {
                var result = new ContentResult()
                {
                    Content = response,
                    ContentType = "application/json",
                    StatusCode = 200,
                };
                context.Result = result;
                return;
            }

            var executedActionContext= await next.Invoke();//will execute the next action filter or itself
            if (executedActionContext.Result is OkObjectResult okObjectResult && okObjectResult.Value is not null)
            {
                await responseCacheServices.CacheResponseAsync(cacheKey, okObjectResult.Value, TimeSpan.FromSeconds(_timeToLiveInSeconds));
            }
        }
        //mo3br 3n l response hykon eh
        private string GenerateCacheKeyFromRequest(HttpRequest request)
        {
            //{URL} api/products?pageIndex=1&pagesize=5&sort=name
            var KeyBuilder = new StringBuilder();
            KeyBuilder.Append(request.Path);// api/products
            //pageIndex=1
            //PageSize=5
            //sort=name
            foreach(var (key,value)in request.Query.OrderBy(x=>x.Key))
            {
                KeyBuilder.Append($"|{key}-{value}");
                // /api/products/pageIndex-1 (by3ml pinding)
                // /api/products/pageIndex-1|pageSize-5
                // /api/products/pageIndex-1|pageSize-5/sort-name
            }
            return KeyBuilder.ToString();
        }
    }
}
