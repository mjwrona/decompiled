// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.WebServer.BuildMetrics5Controller
// Assembly: Microsoft.TeamFoundation.Build2.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FDDA87C8-3548-4A75-AA18-4FB488450659
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.WebServer.dll

using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Build2.WebServer
{
  [ControllerApiVersion(5.0)]
  [VersionedApiControllerCustomName(Area = "build", ResourceName = "Metrics", ResourceVersion = 1)]
  public class BuildMetrics5Controller : BuildMetrics2Controller
  {
    [HttpGet]
    [ClientLocationId("7433FAE7-A6BC-41DC-A6E2-EEF9005CE41A")]
    [PublicProjectRequestRestrictions]
    public override List<BuildMetric> GetProjectMetrics(
      string metricAggregationType = "Daily",
      DateTime? minMetricsTime = null)
    {
      return base.GetProjectMetrics(metricAggregationType, minMetricsTime);
    }
  }
}
