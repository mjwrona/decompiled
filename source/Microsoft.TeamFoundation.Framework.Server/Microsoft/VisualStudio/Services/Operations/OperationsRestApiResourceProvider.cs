// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Operations.OperationsRestApiResourceProvider
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Operations
{
  public sealed class OperationsRestApiResourceProvider : IVssApiResourceProvider
  {
    public void RegisterResources(
      IVssRequestContext requestContext,
      ResourceAreaCollection areas,
      HttpRouteCollection routes)
    {
      routes.MapResourceRoute(TeamFoundationHostType.All, OperationsResourceIds.OperationsPluginLocationId, "operations", "operations", "{resource}/{pluginId}/{operationId}", VssRestApiVersion.v4_0, routeName: "OperationsPlugin");
      routes.MapResourceRoute(TeamFoundationHostType.All, OperationsResourceIds.OperationsLocationId, "operations", "operations", "{resource}/{operationId}", VssRestApiVersion.v2_0, VssRestApiReleaseState.Released, routeName: "Operations");
    }
  }
}
