// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Graph.RequestAccessController
// Assembly: Microsoft.VisualStudio.Services.Graph, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 00390AA0-D8BB-45EB-AEF5-70DC8BFC765D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Graph.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Graph.Client;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Graph
{
  [ControllerApiVersion(5.1)]
  [VersionedApiControllerCustomName(Area = "Graph", ResourceName = "RequestAccess")]
  public class RequestAccessController : GraphControllerBase
  {
    private const string TraceLayer = "RequestAccessController";

    [HttpPost]
    [ClientResponseType(typeof (void), null, null)]
    [RequestRestrictions(RequiredAuthentication.Authenticated, false, true, AuthenticationMechanisms.All, "", UserAgentFilterType.None, null)]
    [TraceFilter(21981, 21982)]
    public HttpResponseMessage RequestAccess([FromBody] JToken jsondocument)
    {
      this.TfsRequestContext.Trace(21983, TraceLevel.Info, this.TraceArea, nameof (RequestAccessController), "Received access request for organization " + this.TfsRequestContext.ServiceHost.Name);
      string json = jsondocument.ToString();
      RequestAccessPayLoad requestAccessPayLoad = (RequestAccessPayLoad) null;
      if (!JsonUtilities.TryDeserialize<RequestAccessPayLoad>(json, out requestAccessPayLoad))
        requestAccessPayLoad = new RequestAccessPayLoad()
        {
          Message = json,
          UrlRequested = (string) null,
          ProjectUri = (string) null
        };
      return this.TfsRequestContext.GetService<IPlatformRequestAccessService>().SendRequestAccessEmail(this.TfsRequestContext, requestAccessPayLoad.Message, requestAccessPayLoad.UrlRequested, requestAccessPayLoad.ProjectUri) ? this.Request.CreateResponse(HttpStatusCode.OK) : this.Request.CreateResponse(HttpStatusCode.BadRequest);
    }
  }
}
