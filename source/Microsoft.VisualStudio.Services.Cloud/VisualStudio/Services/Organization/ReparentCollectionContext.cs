// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Organization.ReparentCollectionContext
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Cloud.WebApi;
using System;

namespace Microsoft.VisualStudio.Services.Organization
{
  public class ReparentCollectionContext : IDisposable
  {
    private readonly Guid m_sourceOrganizationId;
    private readonly Guid m_targetOrganizationId;
    private readonly IVssRequestContext m_sourceOrganizationContext;
    private readonly IVssRequestContext m_targetOrganizationContext;
    private readonly IVssRequestContext[] m_requestContexts;
    private readonly IServicingContext m_servicingContext;

    public ReparentCollectionContext(IServicingContext servicingContext)
      : this(servicingContext, servicingContext.GetRequiredToken<Guid>(ServicingTokenConstants.SourceParentHostId), servicingContext.GetRequiredToken<Guid>(ServicingTokenConstants.TargetParentHostId))
    {
    }

    public ReparentCollectionContext(
      IServicingContext servicingContext,
      Guid sourceOrganizationId,
      Guid targetOrganizationId)
    {
      this.m_servicingContext = servicingContext;
      servicingContext.DisposeTargetRequestContext();
      this.DeploymentContext = servicingContext.DeploymentRequestContext;
      this.m_sourceOrganizationId = sourceOrganizationId;
      this.m_targetOrganizationId = targetOrganizationId;
      this.m_requestContexts = new IVssRequestContext[2];
      try
      {
        if (this.m_sourceOrganizationId.CompareTo(this.m_targetOrganizationId) < 0)
        {
          this.m_sourceOrganizationContext = this.m_requestContexts[0] = this.CreateRequestContext(servicingContext.DeploymentRequestContext, this.m_sourceOrganizationId);
          this.m_targetOrganizationContext = this.m_requestContexts[1] = this.CreateRequestContext(servicingContext.DeploymentRequestContext, this.m_targetOrganizationId);
        }
        else
        {
          this.m_targetOrganizationContext = this.m_requestContexts[0] = this.CreateRequestContext(servicingContext.DeploymentRequestContext, this.m_targetOrganizationId);
          this.m_sourceOrganizationContext = this.m_requestContexts[1] = this.CreateRequestContext(servicingContext.DeploymentRequestContext, this.m_sourceOrganizationId);
        }
        if (servicingContext.DeploymentRequestContext.GetReparentCollectionHostAction(this.m_targetOrganizationId) != ReparentCollectionHostAction.SwitchParent)
          this.m_targetOrganizationContext = (IVssRequestContext) null;
        this.CollectionContext = servicingContext.GetTargetRequestContext();
      }
      catch
      {
        this.Dispose();
        throw;
      }
    }

    public void Dispose()
    {
      this.m_servicingContext.DisposeTargetRequestContext();
      for (int index = this.m_requestContexts.Length - 1; index >= 0; --index)
        this.m_requestContexts[index]?.Dispose();
    }

    public IVssRequestContext DeploymentContext { get; private set; }

    public IVssRequestContext SourceOrganizationContext => this.m_sourceOrganizationContext ?? throw new HostDoesNotExistException(this.m_sourceOrganizationId);

    public IVssRequestContext TargetOrganizationContext => this.m_targetOrganizationContext ?? throw new NotSupportedException(string.Format("Creating request context targeting organization host {0} is not supported for services with lightweight collections", (object) this.m_targetOrganizationId));

    public IVssRequestContext CollectionContext { get; private set; }

    private IVssRequestContext CreateRequestContext(
      IVssRequestContext deploymentContext,
      Guid hostId)
    {
      ITeamFoundationHostManagementService service = deploymentContext.GetService<ITeamFoundationHostManagementService>();
      return service.QueryServiceHostPropertiesCached(deploymentContext, hostId) == null ? (IVssRequestContext) null : service.BeginRequest(deploymentContext, hostId, RequestContextType.ServicingContext);
    }
  }
}
