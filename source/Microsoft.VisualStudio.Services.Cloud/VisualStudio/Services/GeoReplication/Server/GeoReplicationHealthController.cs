// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.GeoReplication.Server.GeoReplicationHealthController
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Health;
using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.GeoReplication.Server
{
  [ControllerApiVersion(3.1)]
  [VersionedApiControllerCustomName(Area = "GeoReplicationHealth", ResourceName = "GeoReplicationHealth")]
  public class GeoReplicationHealthController : TfsApiController
  {
    private static string s_tenantName;

    [HttpGet]
    public HttpResponseMessage GetGeoReplicationHealth()
    {
      if (GeoReplicationHealthController.s_tenantName == null)
        GeoReplicationHealthController.s_tenantName = this.TfsRequestContext.GetService<IHostedTenantService>().TenantName;
      bool flag = this.TfsRequestContext.GetService<IGeoReplicationHealthService>().IsPrimary(this.TfsRequestContext);
      HttpResponseMessage response = this.Request.CreateResponse<GeoReplicationHealthData>(HttpStatusCode.OK, new GeoReplicationHealthData()
      {
        TimeStamp = DateTime.UtcNow,
        TenantName = GeoReplicationHealthController.s_tenantName,
        IsPrimary = flag
      });
      response.Headers.Add("X-AS-QualityOfResponse", flag ? "90" : "0");
      return response;
    }

    public override string TraceArea => "GeoReplicationHealth";

    public override string ActivityLogArea => "Framework";
  }
}
