// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.VersionControlRequestContext
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server;
using Microsoft.TeamFoundation.VersionControl.Common;
using Microsoft.VisualStudio.Services.Common;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class VersionControlRequestContext
  {
    private IVssSecurityNamespace m_repositorySecurity;
    private IVssSecurityNamespace m_repositorySecurity2;
    private IVssSecurityNamespace m_privilegeSecurity;
    private IVssSecurityNamespace m_workspaceSecurity;
    private IVssSecurityNamespace m_authorizationGlobalSecurity;
    private IVssSecurityNamespace m_authorizationProjectSecurity;
    private VersionControlRequestContext m_systemContext;
    private PathLength m_maxClientSupportedServerPathLength = PathLength.Length399;

    internal VersionControlRequestContext(
      IVssRequestContext requestContext,
      TeamFoundationVersionControlService versionControlService)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<TeamFoundationVersionControlService>(versionControlService, nameof (versionControlService));
      this.RequestContext = requestContext;
      this.VersionControlService = versionControlService;
      this.Validation = new Validation(this);
    }

    internal VersionControlRequestContext(IVssRequestContext requestContext)
      : this(requestContext, VersionControlRequestContext.GetVersionControlService(requestContext))
    {
    }

    public PathLength MaxSupportedServerPathLength
    {
      get
      {
        PathLength serverPathLength = this.VersionControlService.GetMaxAllowedServerPathLength(this);
        return serverPathLength < this.m_maxClientSupportedServerPathLength ? serverPathLength : this.m_maxClientSupportedServerPathLength;
      }
      internal set
      {
        this.m_maxClientSupportedServerPathLength = value;
        if (this.m_maxClientSupportedServerPathLength != PathLength.Unspecified)
          return;
        this.m_maxClientSupportedServerPathLength = PathLength.Length399;
      }
    }

    public IVssRequestContext RequestContext { get; private set; }

    public TeamFoundationVersionControlService VersionControlService { get; private set; }

    public IVssServiceHost Host => this.RequestContext.ServiceHost;

    internal Validation Validation { get; private set; }

    internal HashSet<WorkspaceInternal> VerifiedWorkspaces => this.RequestContext.GetVerifiedWorkspaces();

    internal int LatestChangeset { get; set; }

    public VersionControlRequestContext Elevate()
    {
      if (this.RequestContext.IsSystemContext)
        return this;
      if (this.m_systemContext == null)
        this.m_systemContext = new VersionControlRequestContext(this.RequestContext.Elevate());
      return this.m_systemContext;
    }

    private static TeamFoundationVersionControlService GetVersionControlService(
      IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IVssServiceHost>(requestContext.ServiceHost, "requestContext.ServiceHost");
      return requestContext.GetService<TeamFoundationVersionControlService>();
    }

    internal IVssSecurityNamespace GetAuthorizationProjectSecurity()
    {
      if (this.m_authorizationProjectSecurity == null)
        this.m_authorizationProjectSecurity = this.RequestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(this.RequestContext, AuthorizationSecurityConstants.ProjectSecurityGuid);
      return this.m_authorizationProjectSecurity;
    }

    internal IVssSecurityNamespace GetAuthorizationGlobalSecurity()
    {
      if (this.m_authorizationGlobalSecurity == null)
        this.m_authorizationGlobalSecurity = this.RequestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(this.RequestContext, AuthorizationSecurityConstants.NamespaceSecurityGuid);
      return this.m_authorizationGlobalSecurity;
    }

    internal IVssSecurityNamespace GetWorkspaceSecurity()
    {
      if (this.m_workspaceSecurity == null)
        this.m_workspaceSecurity = this.RequestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(this.RequestContext, SecurityConstants.WorkspaceSecurityNamespaceGuid);
      return this.m_workspaceSecurity;
    }

    internal IVssSecurityNamespace GetPrivilegeSecurity()
    {
      if (this.m_privilegeSecurity == null)
        this.m_privilegeSecurity = this.RequestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(this.RequestContext, SecurityConstants.PrivilegeSecurityNamespaceGuid);
      return this.m_privilegeSecurity;
    }

    internal IVssSecurityNamespace GetRepositorySecurity()
    {
      if (this.m_repositorySecurity == null)
        this.m_repositorySecurity = this.RequestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(this.RequestContext, SecurityConstants.RepositorySecurityNamespaceGuid);
      return this.m_repositorySecurity;
    }

    internal IVssSecurityNamespace GetRepositorySecurity2()
    {
      if (this.m_repositorySecurity2 == null)
        this.m_repositorySecurity2 = this.RequestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(this.RequestContext, SecurityConstants.RepositorySecurity2NamespaceGuid);
      return this.m_repositorySecurity2;
    }
  }
}
