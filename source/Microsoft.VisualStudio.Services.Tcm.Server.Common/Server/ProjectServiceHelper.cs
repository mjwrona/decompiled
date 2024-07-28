// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.ProjectServiceHelper
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class ProjectServiceHelper : IProjectServiceHelper
  {
    private IVssRequestContext m_requestContext;
    private IProjectService m_projectService;

    public ProjectServiceHelper(IVssRequestContext requestContext) => this.m_requestContext = requestContext;

    public virtual Guid GetProjectGuid(string projectName) => this.GetProjectFromName(projectName).Id;

    public virtual string GetProjectUri(string projectName) => this.GetProjectFromName(projectName).Uri;

    public virtual GuidAndString GetProjectGuidAndUri(string projectName)
    {
      ProjectInfo projectFromName = this.GetProjectFromName(projectName);
      return new GuidAndString(projectFromName.Uri, projectFromName.Id);
    }

    public virtual bool TryGetProjectName(Guid projectId, out string projectName)
    {
      try
      {
        projectName = this.ProjectService.GetProjectName(this.m_requestContext, projectId);
        return true;
      }
      catch (ProjectDoesNotExistException ex)
      {
        this.m_requestContext.TraceException("Cache", (Exception) ex);
        projectName = (string) null;
        return false;
      }
    }

    public virtual string GetProjectName(Guid projectId) => this.ProjectService.GetProjectName(this.m_requestContext, projectId);

    public virtual ProjectInfo GetProjectFromGuid(Guid projectId) => this.ProjectService.GetProject(this.m_requestContext, projectId);

    public virtual ProjectInfo GetProjectFromName(string projectName) => this.ProjectService.GetProject(this.m_requestContext, projectName);

    public virtual IProjectService ProjectService
    {
      get
      {
        if (this.m_projectService == null)
          this.m_projectService = this.m_requestContext.GetService<IProjectService>();
        return this.m_projectService;
      }
    }

    public ShallowReference GetProjectRepresentation(string projectName) => new ShallowReference()
    {
      Name = projectName,
      Url = UrlBuildHelper.GetResourceUrl(this.m_requestContext, ServiceInstanceTypes.TFS, "core", CoreConstants.ProjectsLocationId, (object) new
      {
        projectId = projectName
      })
    };

    public ShallowReference GetProjectRepresentation(TeamProjectReference projectReference)
    {
      ShallowReference projectRepresentation = this.GetProjectRepresentation(projectReference.Name);
      projectRepresentation.Id = projectReference.Id != Guid.Empty ? projectReference.Id.ToString() : string.Empty;
      return projectRepresentation;
    }

    public virtual IEnumerable<ProjectInfo> GetProjects(ProjectState projectState) => this.ProjectService.GetProjects(this.m_requestContext, projectState);
  }
}
