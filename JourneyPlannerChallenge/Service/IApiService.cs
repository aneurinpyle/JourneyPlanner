using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JourneyPlannerClient.Service
{
    public interface IApiService
    {
        Task<HttpResponseMessage> GetAsync(string targetUrl, Dictionary<string, string> queryParams);
    }
}
