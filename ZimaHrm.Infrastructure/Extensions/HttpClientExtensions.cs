using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ZimaHrm.Core.Infrastructure.Extensions
{
  public static class HttpClientExtensions
  {
    public static Task<HttpResponseMessage> PostAsJsonAsync(this HttpClient httpClient, string url, string json, CancellationToken cancellationToken = default(CancellationToken))
    {
      var message = new HttpRequestMessage(HttpMethod.Post, new Uri(url))
      {
        Content = new StringContent(json, Encoding.UTF8, "application/json")
      };

      return httpClient.SendAsync(message, cancellationToken);
    }
  }
}
