// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.TfvcQueryTfvcStatisticsController
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System.Web.Http;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  [ControllerApiVersion(1.0)]
  [VersionedApiControllerCustomName(Area = "tfvc", ResourceName = "stats", ResourceVersion = 1)]
  [ClientGroupByResource("stats")]
  [ClientInternalUseOnly(true)]
  public class TfvcQueryTfvcStatisticsController : TfvcApiController
  {
    [HttpGet]
    [ClientLocationId("E15C74C0-3605-40E0-AED4-4CC61E549ED8")]
    public TfvcStatistics GetTfvcStatistics(string scopePath = "$\\") => TfvcFileStatsUtility.QueryTfvcStatistics(this.TfsRequestContext, this.Url, scopePath);
  }
}
