// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.TfvcProjectInfoController
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  public class TfvcProjectInfoController : TfvcApiController
  {
    [Obsolete("Use the Projects API instead")]
    [HttpGet]
    [ClientLocationId("252D9C40-0643-41CF-85B2-044D80F9B675")]
    [ClientInclude(RestClientLanguages.TypeScript)]
    public IList<VersionControlProjectInfo> GetProjectInfos()
    {
      List<VersionControlProjectInfo> projectInfos1 = new List<VersionControlProjectInfo>();
      IProjectService service = this.TfsRequestContext.GetService<IProjectService>();
      IEnumerable<ProjectInfo> projectInfos2;
      if (this.ProjectId == Guid.Empty)
        projectInfos2 = service.GetProjects(this.TfsRequestContext, ProjectState.WellFormed);
      else
        projectInfos2 = (IEnumerable<ProjectInfo>) new ProjectInfo[1]
        {
          this.ProjectInfo
        };
      foreach (ProjectInfo projectInfo in projectInfos2)
        projectInfos1.Add(TeamProjectVersionControlInfoUtil.GetProjectInfo(this.TfsRequestContext, projectInfo.Id));
      return (IList<VersionControlProjectInfo>) projectInfos1;
    }

    [Obsolete("Use the Projects API instead")]
    [HttpGet]
    [ClientLocationId("252D9C40-0643-41CF-85B2-044D80F9B675")]
    [ClientInclude(RestClientLanguages.TypeScript)]
    public VersionControlProjectInfo GetProjectInfo([ClientParameterType(typeof (Guid), false)] string projectId)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(projectId, nameof (projectId), this.TfsRequestContext.ServiceName);
      if (this.ProjectId == Guid.Empty)
        return TeamProjectVersionControlInfoUtil.GetProjectInfo(this.TfsRequestContext, GitServerUtils.GetProjectInfo(this.TfsRequestContext, projectId, true).Id);
      Guid result;
      if (Guid.TryParse(projectId, out result))
      {
        if (this.ProjectId != result)
          throw new ArgumentException(Resources.Format("RequestProjectMismatch", (object) projectId, (object) this.ProjectId));
      }
      else if (!projectId.Equals(this.ProjectInfo.Name))
        throw new ArgumentException(Resources.Format("RequestProjectMismatch", (object) projectId, (object) this.ProjectInfo.Name));
      return TeamProjectVersionControlInfoUtil.GetProjectInfo(this.TfsRequestContext, this.ProjectId);
    }
  }
}
