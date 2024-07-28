// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.EndpointsController
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.VisualStudio.Services.Security;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [ControllerApiVersion(1.0)]
  [VersionedApiControllerCustomName(Area = "Auditing", ResourceName = "Endpoints")]
  public class EndpointsController : TfsApiController
  {
    [HttpGet]
    public IQueryable<EndpointDescription> GetEndpoints()
    {
      this.TfsRequestContext.CheckDeploymentRequestContext();
      if (!this.TfsRequestContext.ExecutionEnvironment.IsDevFabricDeployment)
        this.TfsRequestContext.GetService<TeamFoundationSecurityService>().GetSecurityNamespace(this.TfsRequestContext, FrameworkSecurity.FrameworkNamespaceId).CheckPermission(this.TfsRequestContext, FrameworkSecurity.FrameworkNamespaceToken, 2);
      List<EndpointDescription> source = new List<EndpointDescription>();
      using (IDisposableReadOnlyList<ITfsRouteAuditingProvider> extensions = this.TfsRequestContext.GetExtensions<ITfsRouteAuditingProvider>())
      {
        foreach (ITfsRouteAuditingProvider auditingProvider in (IEnumerable<ITfsRouteAuditingProvider>) extensions)
          source.AddRange(auditingProvider.GetEndpoints(this.TfsRequestContext));
      }
      return source.AsQueryable<EndpointDescription>();
    }

    public override string TraceArea => "AuditingService";

    public override string ActivityLogArea => "Framework";

    protected override void InitializeExceptionMap(ApiExceptionMapping exceptionMap)
    {
      base.InitializeExceptionMap(exceptionMap);
      exceptionMap.AddStatusCode<AccessCheckException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<UnexpectedHostTypeException>(HttpStatusCode.NotFound);
    }
  }
}
