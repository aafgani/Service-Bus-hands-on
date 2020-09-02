using MessageSender.Model;
using Microsoft.Extensions.Options;
using System.Net.Http;

namespace MessageSender.Service
{
    public class FunctionClientService 
    {
        protected HttpClient httpClient;
        protected AppConfig appConfig;

        public FunctionClientService(HttpClient httpClient, IOptions<AppConfig> options)
        {
            this.httpClient = httpClient;
            this.appConfig = options.Value;
        }
    }
}
