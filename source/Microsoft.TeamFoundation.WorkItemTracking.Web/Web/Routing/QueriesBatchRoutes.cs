// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Routing.QueriesBatchRoutes
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.Azure.Boards.WebApi.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using System.Web.Http;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Routing
{
  public class QueriesBatchRoutes : IVssApiResourceProvider
  {
    public void RegisterResources(
      IVssRequestContext requestContext,
      ResourceAreaCollection areas,
      HttpRouteCollection routes)
    {
      routes.MapResourceRoute(TfsApiResourceScope.Project, WorkItemTrackingLocationIds.QueriesBatch, "wit", "queriesBatch", "{area}/{resource}", VssRestApiVersion.v5_0, VssRestApiReleaseState.Released);
    }
  }
}
