// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.ProjectWebContext
// Assembly: Microsoft.TeamFoundation.WebPlatform.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: BDF91478-A3ED-4B5B-AA51-9473C7AE6182
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WebPlatform.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.WebPlatform.Server;
using System;
using System.Web.Routing;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  public class ProjectWebContext : WebContext
  {
    protected IProjectAccessChecker m_projectAccessChecker;

    public ProjectWebContext(RequestContext requestContext)
      : base(requestContext)
    {
    }

    public ProjectWebContext()
    {
    }

    protected override void InitializeContext()
    {
      base.InitializeContext();
      string project = this.NavigationContext.Project;
      if (string.IsNullOrEmpty(project))
        return;
      this.Project = TfsProjectHelpers.GetProjectFromRequestName(this.TfsRequestContext, project, this.ProjectAccessChecker);
      if (this.Project == null)
        return;
      if (!this.Project.Name.Equals(this.NavigationContext.Project, StringComparison.OrdinalIgnoreCase) && !Guid.TryParseExact(project, "D", out Guid _))
      {
        this.NavigationContext.SetProjectName(this.Project.Name);
        this.HandleProjectRename(project, this.Project);
      }
      ProjectUtility.SetupProjectContext(this.TfsRequestContext, this.Project);
    }

    public virtual ProjectInfo Project { get; private set; }

    protected override ContextIdentifier CreateProjectContext()
    {
      if (this.Project == null)
        return (ContextIdentifier) null;
      return new ContextIdentifier()
      {
        Name = this.Project.Name,
        Id = this.Project.Id
      };
    }

    protected IProjectAccessChecker ProjectAccessChecker
    {
      get
      {
        if (this.m_projectAccessChecker == null)
          this.m_projectAccessChecker = (IProjectAccessChecker) new Microsoft.TeamFoundation.WebPlatform.Server.ProjectAccessChecker();
        return this.m_projectAccessChecker;
      }
    }

    protected virtual void HandleProjectRename(string requestProjectName, ProjectInfo project)
    {
    }
  }
}
