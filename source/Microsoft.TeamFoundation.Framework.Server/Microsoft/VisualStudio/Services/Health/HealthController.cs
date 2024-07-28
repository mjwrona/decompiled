// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Health.HealthController
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Health
{
  [ControllerApiVersion(1.0)]
  [VersionedApiControllerCustomName(Area = "Health", ResourceName = "Health")]
  public class HealthController : TfsApiController
  {
    public override string TraceArea => "Health";

    public override string ActivityLogArea => "Framework";

    [HttpGet]
    public HttpResponseMessage GetHealth()
    {
      HealthService service = this.TfsRequestContext.GetService<HealthService>();
      bool isHealthy;
      HealthData healthData = service.InitializeAndGetHealthData(this.TfsRequestContext, out isHealthy);
      HttpResponseMessage response = this.Request.CreateResponse<HealthData>(isHealthy ? HttpStatusCode.OK : HttpStatusCode.ServiceUnavailable, healthData);
      string qualityOfResponse = service.GetAfdOutgoingQualityOfResponse(this.TfsRequestContext);
      if (qualityOfResponse != null)
        response.Headers.Add("X-AS-QualityOfResponse", qualityOfResponse);
      return response;
    }
  }
}
