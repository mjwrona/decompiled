// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.Internal.FeedMetricsController
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi.Internal;
using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Feed.Server.Internal
{
  [ControllerApiVersion(1.0)]
  [ClientGroupByResource("Artifact Details")]
  [VersionedApiControllerCustomName(Area = "Packaging", ResourceName = "Metrics")]
  public class FeedMetricsController : FeedApiController
  {
    [HttpPost]
    [ClientInternalUseOnly(true)]
    public Task<HttpStatusCode> PublishMetrics(List<PackageMetricsData> packageMetricsUpdates)
    {
      this.TfsRequestContext.Trace(10019107, TraceLevel.Info, FeedApiController.Area, "FeedMetrics", "Received package metrics updates for  " + (packageMetricsUpdates != null ? packageMetricsUpdates.Count.ToString() : "0") + " packages");
      this.FeedMetricsService.IngestRawMetrics(this.TfsRequestContext, packageMetricsUpdates);
      return Task.FromResult<HttpStatusCode>(HttpStatusCode.Accepted);
    }
  }
}
