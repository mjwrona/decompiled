// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Types.IProjectService
// Assembly: Microsoft.TeamFoundation.Server.Types, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FC707FA3-32BF-41E4-BD8A-1BB971125382
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Types.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Server.Types
{
  [DefaultServiceImplementation("Microsoft.TeamFoundation.Server.Core.PlatformProjectService, Microsoft.TeamFoundation.Server.Core")]
  public interface IProjectService : IVssFrameworkService
  {
    ProjectInfo CreateProject(
      IVssRequestContext requestContext,
      Guid projectId,
      string projectName,
      ProjectVisibility projectVisibility,
      string projectAbbreviation = null,
      string projectDescription = null);

    void DeleteProject(IVssRequestContext requestContext, Guid projectId);

    void DeleteReservedProject(IVssRequestContext requestContext, Guid pendingProjectGuid);

    ProjectInfo GetProject(IVssRequestContext requestContext, Guid projectId);

    ProjectInfo GetProject(
      IVssRequestContext requestContext,
      string projectName,
      bool includeHistory = false);

    bool TryGetProject(
      IVssRequestContext requestContext,
      Guid projectId,
      out ProjectInfo projectInfo);

    bool TryGetProject(
      IVssRequestContext requestContext,
      string projectName,
      out ProjectInfo projectInfo,
      bool includeHistory = false);

    IList<ProjectInfo> GetProjectHistory(
      IVssRequestContext requestContext,
      Guid projectId,
      long minRevision = 0);

    IList<ProjectInfo> GetProjectHistory(IVssRequestContext requestContext, long minRevision = 0);

    Guid GetProjectId(IVssRequestContext requestContext, string projectName, bool includeHistory = false);

    bool TryGetProjectId(
      IVssRequestContext requestContext,
      string projectName,
      out Guid projectId,
      bool includeHistory = false);

    string GetProjectName(IVssRequestContext requestContext, Guid projectId);

    bool TryGetProjectName(
      IVssRequestContext requestContext,
      Guid projectId,
      out string projectName);

    IEnumerable<ProjectInfo> GetProjects(IVssRequestContext requestContext, ProjectState state = ProjectState.All);

    string GetSecurityToken(IVssRequestContext requestContext, string projectUri);

    Guid ReserveProject(
      IVssRequestContext requestContext,
      string projectName,
      Guid? desiredProjectGuid = null);

    Guid UpdateProject(
      IVssRequestContext requestContext,
      ProjectInfo project,
      out ProjectInfo updatedProject);

    IEnumerable<ProjectProperty> GetProjectProperties(
      IVssRequestContext requestContext,
      Guid projectId,
      IEnumerable<string> projectPropertyFilters);

    IEnumerable<ProjectProperties> GetProjectsProperties(
      IVssRequestContext requestContext,
      IEnumerable<Guid> projectIds,
      IEnumerable<string> projectPropertyFilters);

    void SetProjectProperties(
      IVssRequestContext requestContext,
      Guid projectId,
      IEnumerable<ProjectProperty> projectProperties);

    ProjectVisibility GetProjectVisibility(IVssRequestContext requestContext, Guid projectId);
  }
}
