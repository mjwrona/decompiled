// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.VersionControl.VersionControlAreaController
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.VersionControl, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AEC6FD7F-E72C-4C65-8428-206D27D3BF89
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.VersionControl.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.Routing;
using Microsoft.TeamFoundation.SourceControl.WebServer;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.VersionControl
{
  public abstract class VersionControlAreaController : TfsAreaController
  {
    private const string c_reviewModeRoutePathPart = "review";
    private VersionControlRepositoryInfoFactory m_repositoryInfoFactory;

    public string RepositoryName { get; protected set; }

    public override string AreaName => "VersionControl";

    public override string TraceArea => VersionControlConstants.TraceArea;

    public string GetVersionFromRouteParameters(out bool reviewMode)
    {
      reviewMode = false;
      string fromRouteParameters = this.RouteData.GetRouteValue<string>("parameters", (string) null);
      if (!string.IsNullOrEmpty(fromRouteParameters))
      {
        int length = fromRouteParameters.LastIndexOf('/');
        if ((length < 0 ? fromRouteParameters : fromRouteParameters.Substring(length + 1)).Equals("review", StringComparison.OrdinalIgnoreCase))
        {
          reviewMode = true;
          fromRouteParameters = length < 0 ? string.Empty : fromRouteParameters.Substring(0, length);
        }
      }
      return fromRouteParameters;
    }

    protected override void OnActionExecuted(ActionExecutedContext filterContext)
    {
      if (this.RepositoryName != null)
        this.SaveDefaultRepository(this.RepositoryName);
      base.OnActionExecuted(filterContext);
    }

    protected void SaveDefaultRepository(string repositoryName)
    {
      if (string.IsNullOrEmpty(repositoryName))
        return;
      this.TfsWebContext.SetProjectSetting(SettingsUserScope.User, "Git/DefaultRepository", (object) repositoryName);
    }

    public bool IsSearchEnabled()
    {
      if (this.TfsRequestContext.IsFeatureEnabled("WebAccess.SearchShell"))
        return true;
      return this.TfsRequestContext.GetService<IContributionService>().QueryContributions(this.TfsRequestContext, (IEnumerable<string>) new string[1]
      {
        "ms.vss-code-search.code-entity-type"
      }).Any<Contribution>();
    }

    protected override void Dispose(bool disposing)
    {
      if (this.m_repositoryInfoFactory != null)
      {
        this.m_repositoryInfoFactory.Dispose();
        this.m_repositoryInfoFactory = (VersionControlRepositoryInfoFactory) null;
      }
      base.Dispose(disposing);
    }

    internal VersionControlRepositoryInfoFactory RepositoryInfoFactory
    {
      get
      {
        if (this.m_repositoryInfoFactory == null)
          this.m_repositoryInfoFactory = new VersionControlRepositoryInfoFactory(this.TfsWebContext);
        return this.m_repositoryInfoFactory;
      }
    }

    protected VersionControlProvider GetVcProvider(string repositoryName) => this.GetVcProvider(repositoryName, new VersionControlRepositoryType?());

    protected VersionControlProvider GetVcProvider(Guid? repositoryId)
    {
      Guid valueOrDefault = repositoryId.GetValueOrDefault(Guid.Empty);
      return valueOrDefault == Guid.Empty ? (VersionControlProvider) this.GetTfsVcProvider() : (VersionControlProvider) this.GetGitVcProviderById(valueOrDefault);
    }

    private VersionControlProvider GetVcProvider(
      string repositoryName,
      VersionControlRepositoryType? requiredType)
    {
      return this.RepositoryInfoFactory.GetRepositoryInfo(repositoryName, requiredType).Provider;
    }

    protected TfsVersionControlProvider GetTfsVcProvider() => (TfsVersionControlProvider) this.GetVcProvider((string) null, new VersionControlRepositoryType?(VersionControlRepositoryType.TFS));

    protected GitVersionControlProvider GetGitVcProviderById(Guid repositoryId) => this.RepositoryInfoFactory.GetGitRepositoryById(repositoryId).GitProvider;
  }
}
