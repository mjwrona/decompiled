// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WebContext
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.WebPlatform;
using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server;
using System;
using System.Runtime.Serialization;
using System.Web.Mvc;
using System.Web.Routing;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  [DataContract]
  public class WebContext : WebSdkMetadata
  {
    private UrlHelper m_urlHelper;
    private Microsoft.VisualStudio.Services.Identity.Identity m_currentIdentity;
    private bool m_readCurrentIdentity;
    private ContextIdentifier m_projectContext;
    private TeamContext m_teamContext;
    private bool m_projectInitialized;
    private bool m_teamInitialized;
    private UserContext m_user;
    private HostContext m_collection;
    private HostContext m_account;
    private ExtendedHostContext m_host;
    private DiagnosticsContext m_diagnostics;
    private NavigationContext m_navigation;
    private GlobalizationContext m_globalizationContext;

    public WebContext(RequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<RequestContext>(requestContext, nameof (requestContext));
      this.RequestContext = requestContext;
      this.TfsRequestContext = requestContext.TfsRequestContext(true);
    }

    public WebContext()
    {
    }

    internal void Initialize()
    {
      this.TfsRequestContext.TraceEnter(520004, "WebAccess", TfsTraceLayers.Controller, "WebContext.Initialize()");
      this.InitializeContext();
      NavigationHelpers.SetRequestNavigationProperties(this);
      if (this.NavigationContext != null)
      {
        this.NavigationContext.UpdateResolvedRoute();
        if (this.Diagnostics.TracePointCollectionEnabled && string.Equals(this.NavigationContext.CurrentController, "ContributedPage", StringComparison.OrdinalIgnoreCase))
        {
          string str = "A legacy platform WebContext object was instantiated during a request for a new web platform page. The stack trace is available in a debug build.";
          this.TfsRequestContext.GetService<IPerformanceInsightsService>().AddPerformanceInsight(this.TfsRequestContext, new PerformanceInsight()
          {
            GroupName = "Legacy Server API",
            Title = "Used Legacy WebContext API in a new web platform page",
            Description = str,
            Level = Level.Error
          });
        }
      }
      this.TfsRequestContext.TraceLeave(520006, "WebAccess", TfsTraceLayers.Controller, "WebContext.Initialize()");
    }

    protected virtual void InitializeContext()
    {
    }

    public virtual RequestContext RequestContext { get; private set; }

    public virtual IVssRequestContext TfsRequestContext { get; private set; }

    public virtual UrlHelper Url
    {
      get
      {
        if (this.m_urlHelper == null)
          this.m_urlHelper = new UrlHelper(this.RequestContext);
        return this.m_urlHelper;
      }
    }

    public virtual bool IsHosted => this.TfsRequestContext.ExecutionEnvironment.IsHostedDeployment;

    public Microsoft.VisualStudio.Services.Identity.Identity CurrentIdentity
    {
      get
      {
        if (!this.m_readCurrentIdentity)
        {
          if (this.TfsRequestContext.UserContext != (IdentityDescriptor) null)
            this.m_currentIdentity = this.TfsRequestContext.GetUserIdentity();
          this.m_readCurrentIdentity = true;
        }
        return this.m_currentIdentity;
      }
    }

    public virtual string CurrentUserDisplayName => this.CurrentIdentity?.DisplayName;

    public DiagnosticsContext Diagnostics
    {
      get
      {
        if (this.m_diagnostics == null)
          this.m_diagnostics = this.CreateDiagnosticsContext();
        return this.m_diagnostics;
      }
    }

    public NavigationContext NavigationContext
    {
      get
      {
        if (this.m_navigation == null)
          this.m_navigation = this.CreateNavigationContext();
        return this.m_navigation;
      }
    }

    public GlobalizationContext Globalization
    {
      get
      {
        if (this.m_globalizationContext == null)
          this.m_globalizationContext = this.CreateGlobalizationContext();
        return this.m_globalizationContext;
      }
    }

    [DataMember(EmitDefaultValue = false)]
    public UserContext User
    {
      get
      {
        if (this.m_user == null && this.CurrentIdentity != null)
        {
          using (PerformanceTimer.StartMeasure(this.TfsRequestContext, "WebContext.CreateUserContext"))
            this.m_user = this.CreateUserContext(this.CurrentIdentity);
        }
        return this.m_user;
      }
    }

    [DataMember(Name = "team", EmitDefaultValue = false)]
    public TeamContext TeamContext
    {
      get
      {
        if (!this.m_teamInitialized)
        {
          using (PerformanceTimer.StartMeasure(this.TfsRequestContext, "WebContext.CreateTeamContext"))
            this.m_teamContext = this.CreateTeamContext();
          this.m_teamInitialized = true;
        }
        return this.m_teamContext;
      }
    }

    [DataMember(Name = "project", EmitDefaultValue = false)]
    public ContextIdentifier ProjectContext
    {
      get
      {
        if (!this.m_projectInitialized)
        {
          using (PerformanceTimer.StartMeasure(this.TfsRequestContext, "WebContext.CreateProjectContext"))
            this.m_projectContext = this.CreateProjectContext();
          this.m_projectInitialized = true;
        }
        return this.m_projectContext;
      }
    }

    [DataMember(EmitDefaultValue = false)]
    public HostContext Collection
    {
      get
      {
        if (this.m_collection == null && this.TfsRequestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
          this.m_collection = new HostContext(this.TfsRequestContext);
        return this.m_collection;
      }
    }

    [DataMember(EmitDefaultValue = false)]
    public HostContext Account
    {
      get
      {
        if (this.m_account == null)
        {
          if (this.IsHosted && this.NavigationContext.TopMostLevel == NavigationContextLevels.Deployment)
            return (HostContext) null;
          if (this.TfsRequestContext.ServiceHost.Is(TeamFoundationHostType.Application))
            this.m_account = new HostContext(this.TfsRequestContext);
          else if (this.IsHosted)
          {
            this.m_account = new HostContext(this.TfsRequestContext);
            this.m_account.Id = this.TfsRequestContext.To(TeamFoundationHostType.Application).ServiceHost.InstanceId;
            ILocationService service = this.TfsRequestContext.GetService<ILocationService>();
            AccessMapping accessMapping = service.DetermineAccessMapping(this.TfsRequestContext);
            if (accessMapping.Moniker == "LegacyCollectionAccessMapping" || accessMapping.Moniker == AccessMappingConstants.VstsAccessMapping && string.Equals(this.m_account.RelativeUri, "/DefaultCollection/", StringComparison.OrdinalIgnoreCase))
            {
              accessMapping.VirtualDirectory = "";
              this.m_account.Uri = service.GetSelfReferenceUrl(this.TfsRequestContext, accessMapping);
              this.m_account.RelativeUri = this.TfsRequestContext.RemoveVirtualDirectory(this.m_account.RelativeUri);
            }
          }
          else
            this.m_account = new HostContext(this.TfsRequestContext.To(TeamFoundationHostType.Application));
          if (!this.IsHosted)
            this.m_account.Name = WACommonResources.NavigationContentMenuItem_Instance;
        }
        return this.m_account;
      }
    }

    [DataMember(EmitDefaultValue = false)]
    public ExtendedHostContext Host
    {
      get
      {
        if (this.m_host == null)
          this.m_host = new ExtendedHostContext(this.TfsRequestContext);
        return this.m_host;
      }
    }

    protected virtual ContextIdentifier CreateProjectContext()
    {
      if (string.IsNullOrEmpty(this.NavigationContext.Project))
        return (ContextIdentifier) null;
      return new ContextIdentifier()
      {
        Name = this.NavigationContext.Project
      };
    }

    protected virtual TeamContext CreateTeamContext()
    {
      if (CommonUtility.ShouldIgnoreTeamContext(this.TfsRequestContext))
        return (TeamContext) null;
      if (string.IsNullOrEmpty(this.NavigationContext.Team))
        return (TeamContext) null;
      TeamContext teamContext = new TeamContext();
      teamContext.Name = this.NavigationContext.Team;
      return teamContext;
    }

    protected virtual UserContext CreateUserContext(Microsoft.VisualStudio.Services.Identity.Identity userIdentity) => new UserContext(this.TfsRequestContext, userIdentity);

    protected virtual NavigationContext CreateNavigationContext() => new NavigationContext(this.TfsRequestContext, this.RequestContext);

    protected virtual DiagnosticsContext CreateDiagnosticsContext() => new DiagnosticsContext(this.TfsRequestContext, this.RequestContext);

    protected virtual GlobalizationContext CreateGlobalizationContext() => new GlobalizationContext(this.TfsRequestContext, this.RequestContext);

    public string GetNavigationDisplayText()
    {
      string navigationDisplayText = string.Empty;
      if (this.NavigationContext.TopMostLevel == NavigationContextLevels.Team)
        navigationDisplayText = this.ProjectContext.Name + " / " + this.TeamContext.Name;
      else if (this.ProjectContext != null)
        navigationDisplayText = this.ProjectContext.Name;
      else if (this.Collection != null)
        navigationDisplayText = this.Collection.Name;
      else if (this.Account != null)
        navigationDisplayText = this.Account.Name;
      return navigationDisplayText;
    }
  }
}
