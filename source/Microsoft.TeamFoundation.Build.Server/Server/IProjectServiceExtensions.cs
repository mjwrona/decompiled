// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.IProjectServiceExtensions
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.TeamFoundation.Build.Server
{
  internal static class IProjectServiceExtensions
  {
    public static TeamProject GetTeamProjectFromGuidOrName(
      this IProjectService projectService,
      IVssRequestContext requestContext,
      string parameter)
    {
      ProjectInfo projectInfo = (ProjectInfo) null;
      Guid result = Guid.Empty;
      if (Guid.TryParse(parameter, out result))
      {
        try
        {
          projectInfo = projectService.GetProject(requestContext.Elevate(), result);
        }
        catch (ProjectDoesNotExistException ex)
        {
        }
      }
      if (projectInfo == null)
        projectInfo = projectService.GetProject(requestContext.Elevate(), parameter);
      return new TeamProject(projectInfo.Uri, projectInfo.Name);
    }

    public static TeamProject GetTeamProjectFromUri(
      this IProjectService projectService,
      IVssRequestContext requestContext,
      string uri)
    {
      return projectService.GetTeamProjectFromGuidOrName(requestContext, LinkingUtilities.DecodeUri(uri).ToolSpecificId);
    }

    public static TeamProject GetTeamProjectFromGuid(
      this IProjectService projectService,
      IVssRequestContext requestContext,
      Guid projectId)
    {
      ProjectInfo project = projectService.GetProject(requestContext.Elevate(), projectId);
      return new TeamProject(project.Uri, project.Name);
    }
  }
}
