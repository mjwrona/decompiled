// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Analytics.WebApi.AnalyticsServiceStateController
// Assembly: Microsoft.TeamFoundation.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7A426D2C-9BEF-4A84-9FA2-D9A32F46BD7E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Analytics.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Analytics.WebApi
{
  [VersionedApiControllerCustomName(Area = "TfsAnalytics", ResourceName = "State")]
  public class AnalyticsServiceStateController : TfsApiController
  {
    [HttpPatch]
    [ClientResponseType(typeof (void), null, null)]
    [ClientInclude(RestClientLanguages.CSharp)]
    public HttpResponseMessage UpdateAnalyticsState(AnalyticsState state) => this.Request.CreateResponse<HttpStatusCode>(HttpStatusCode.OK);
  }
}
