// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Graph.GraphCachePoliciesController
// Assembly: Microsoft.VisualStudio.Services.Graph, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 00390AA0-D8BB-45EB-AEF5-70DC8BFC765D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Graph.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Graph.Client;
using Microsoft.VisualStudio.Services.WebApi;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Graph
{
  [ControllerApiVersion(3.1)]
  [ClientInternalUseOnly(true)]
  [RestrictInternalGraphEndpoints]
  [VersionedApiControllerCustomName(Area = "Graph", ResourceName = "CachePolicies")]
  public class GraphCachePoliciesController : GraphControllerBase
  {
    private const string TraceLayer = "GraphCachePoliciesController";

    [HttpGet]
    [TraceFilter(10008101, 10008102)]
    public GraphCachePolicies GetCachePolicies()
    {
      this.CheckPermission(this.TfsRequestContext);
      return this.TfsRequestContext.GetService<IGraphCachePolicyService>().GetCachePolicies(this.TfsRequestContext);
    }

    private void CheckPermission(IVssRequestContext requestContext)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      vssRequestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(vssRequestContext, FrameworkSecurity.FrameworkNamespaceId).CheckPermission(vssRequestContext, FrameworkSecurity.FrameworkNamespaceToken, 4, false);
    }

    public override string TraceArea => "Graph";

    public override string ActivityLogArea => "Graph";
  }
}
