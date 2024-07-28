// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.BaseServicingJobsController
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Net;

namespace Microsoft.VisualStudio.Services.Cloud
{
  public class BaseServicingJobsController : TfsApiController
  {
    private static readonly Dictionary<Type, HttpStatusCode> s_httpExceptions = new Dictionary<Type, HttpStatusCode>()
    {
      {
        typeof (UnexpectedHostTypeException),
        HttpStatusCode.NotFound
      }
    };

    public override string TraceArea => "ServicingJobs";

    public override string ActivityLogArea => "Framework";

    public override IDictionary<Type, HttpStatusCode> HttpExceptions => (IDictionary<Type, HttpStatusCode>) BaseServicingJobsController.s_httpExceptions;

    protected virtual void CheckHostType()
    {
      if (!this.TfsRequestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        throw new UnexpectedHostTypeException(this.TfsRequestContext.ServiceHost.HostType);
    }

    protected virtual void CheckDiagnosticPermission(IVssRequestContext requestContext)
    {
      TeamFoundationSecurityService service = requestContext.GetService<TeamFoundationSecurityService>();
      IVssSecurityNamespace securityNamespace = service.GetSecurityNamespace(requestContext, FrameworkSecurity.FrameworkNamespaceId);
      if (service.GetSecurityNamespace(requestContext, FrameworkSecurity.DiagnosticNamespaceId).HasPermission(requestContext, FrameworkSecurity.DiagnosticNamespaceToken, 4, false))
        return;
      securityNamespace.CheckPermission(requestContext, FrameworkSecurity.FrameworkNamespaceToken, 2, false);
    }
  }
}
