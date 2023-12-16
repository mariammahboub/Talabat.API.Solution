using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Talabat.Core.Services.Contract
{
    public interface IResponseCacheServices
    {
        //object hykon afdl bdel in this case(Key,Value,Time Expire)
        Task CacheResponseAsync(string cacheKey,object response,TimeSpan timeToLive);
        //Get
        Task<string?> GetCacheResponseAsync(string cacheKey);
    }
}
