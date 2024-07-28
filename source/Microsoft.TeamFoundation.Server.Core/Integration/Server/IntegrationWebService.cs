// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Integration.Server.IntegrationWebService
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core.WebServices;
using System;
using System.Web.Services.Protocols;

namespace Microsoft.TeamFoundation.Integration.Server
{
  public abstract class IntegrationWebService : TeamFoundationWebService
  {
    internal IntegrationHost m_integrationHost;

    protected IntegrationWebService()
    {
      if (!this.RequestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        throw new UnexpectedHostTypeException(this.RequestContext.ServiceHost.HostType);
      this.RequestContext.ServiceName = "Integration";
      this.m_integrationHost = this.RequestContext.GetService<IntegrationHost>();
    }

    protected override Exception HandleException(Exception exception)
    {
      if (this.RequestContext.Status == null)
        this.RequestContext.Status = exception;
      if (!(exception is SoapException))
        exception = SoapExceptionServerUtilities.LogAndFilter(exception, this.RequestContext.DomainUserName);
      return exception;
    }
  }
}
