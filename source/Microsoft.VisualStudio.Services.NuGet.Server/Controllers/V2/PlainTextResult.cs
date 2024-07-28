// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.Controllers.V2.PlainTextResult
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.NuGet.Server.Controllers.V2
{
  public class PlainTextResult : IHttpActionResult
  {
    private readonly HttpRequestMessage request;
    private readonly ISecuredObject securedObject;

    public PlainTextResult(
      string content,
      HttpRequestMessage request,
      ISecuredObject securedObject)
    {
      this.request = request;
      this.Content = content;
      this.securedObject = securedObject;
    }

    public string Content { get; private set; }

    public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken) => Task.FromResult<HttpResponseMessage>(new HttpResponseMessage()
    {
      Content = (HttpContent) new VssServerStringContent(this.Content, Encoding.UTF8, "text/plain", (object) this.securedObject),
      RequestMessage = this.request
    });
  }
}
