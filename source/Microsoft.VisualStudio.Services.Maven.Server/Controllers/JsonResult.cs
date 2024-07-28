// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.Controllers.JsonResult
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Maven.Server.Controllers
{
  public class JsonResult : IHttpActionResult
  {
    private readonly HttpRequestMessage request;
    private readonly ISecuredObject securedObject;

    public JsonResult(string content, HttpRequestMessage request, ISecuredObject securedObject)
    {
      this.request = request;
      this.Content = content;
      this.securedObject = securedObject;
    }

    public string Content { get; private set; }

    public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken) => Task.FromResult<HttpResponseMessage>(new HttpResponseMessage()
    {
      Content = (HttpContent) new VssServerStringContent(this.Content, Encoding.UTF8, "application/json", (object) this.securedObject),
      RequestMessage = this.request
    });
  }
}
