// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.TfvcFileStatsUtility
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.TeamFoundation.VersionControl.Common;
using Microsoft.TeamFoundation.VersionControl.Server;
using System.Web.Http.Routing;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  internal class TfvcFileStatsUtility
  {
    public static TfvcStatistics QueryTfvcStatistics(
      IVssRequestContext requestContext,
      UrlHelper urlHelper,
      string scopePath)
    {
      scopePath = VersionControlPath.GetFullPath(scopePath);
      TeamFoundationVersionControlService service = requestContext.GetService<TeamFoundationVersionControlService>();
      ItemSpec itemSpec = new ItemSpec(scopePath, RecursionType.Full);
      IVssRequestContext requestContext1 = requestContext;
      ItemSpec scopeItem = itemSpec;
      return service.QueryTfvcFileStats(requestContext1, scopeItem).ToWebApiTfvcFileStats();
    }
  }
}
