// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.ContentSecurityPolicy.CspReportController
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server;
using System.Net;
using System.Net.Http;

namespace Microsoft.TeamFoundation.Server.WebAccess.ContentSecurityPolicy
{
  [RequestContentTypeRestriction(AllowCspReport = true, AllowJson = true, AllowJsonPatch = false)]
  [ControllerApiVersion(1.0)]
  [VersionedApiControllerCustomName(Area = "CspReport", ResourceName = "CspReport")]
  public class CspReportController : TfsApiController
  {
    private const string c_traceArea = "ContentSecurityPolicy";
    private const string c_traceLayer = "CspReport";

    public HttpResponseMessage Post()
    {
      if (!this.TfsRequestContext.IsFeatureEnabled(ContentSecurityPolicyFeatureFlags.ContentSecurityPolicyCspReportEndpointFeatureFlag))
        return this.Request.CreateResponse(HttpStatusCode.NotFound);
      string result = this.Request.Content.ReadAsStringAsync().Result;
      this.TfsRequestContext.GetService<CspReportLoggingService>().LogCspReport(this.TfsRequestContext, result);
      return this.Request.CreateResponse(HttpStatusCode.OK);
    }

    public override string TraceArea => "ContentSecurityPolicy";

    public override string ActivityLogArea => "Framework";
  }
}
