// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.TeamProjectVersionControlInfoUtil
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using System;
using System.Linq;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  public static class TeamProjectVersionControlInfoUtil
  {
    public static bool HasGitVersionControlSupport(
      IVssRequestContext requestContext,
      ProjectInfo projectInfo)
    {
      return TeamProjectVersionControlInfoUtil.HasGitVersionControlSupport(requestContext, projectInfo.Id);
    }

    public static bool HasGitVersionControlSupport(
      IVssRequestContext requestContext,
      Guid projectId)
    {
      return TeamProjectVersionControlInfoUtil.HasTrueProjectProperty(TeamProjectVersionControlInfoUtil.ReadProjectInfo(requestContext, projectId), "System.SourceControlGitEnabled");
    }

    public static bool HasTfvcVersionControlSupport(
      IVssRequestContext requestContext,
      ProjectInfo projectInfo)
    {
      return TeamProjectVersionControlInfoUtil.HasTfvcVersionControlSupport(requestContext, projectInfo.Id);
    }

    public static bool HasTfvcVersionControlSupport(
      IVssRequestContext requestContext,
      Guid projectId)
    {
      return TeamProjectVersionControlInfoUtil.ProjectSupportsTfvc(TeamProjectVersionControlInfoUtil.ReadProjectInfo(requestContext, projectId));
    }

    public static VersionControlProjectInfo GetProjectInfo(
      IVssRequestContext requestContext,
      Guid projectId)
    {
      ProjectInfo projectInfo = TeamProjectVersionControlInfoUtil.ReadProjectInfo(requestContext, projectId);
      return new VersionControlProjectInfo()
      {
        DefaultSourceControlType = TeamProjectVersionControlInfoUtil.GetDefaultSourceControlType(projectInfo),
        SupportsGit = TeamProjectVersionControlInfoUtil.HasTrueProjectProperty(projectInfo, "System.SourceControlGitEnabled"),
        SupportsTFVC = TeamProjectVersionControlInfoUtil.ProjectSupportsTfvc(projectInfo),
        Project = projectInfo.ToTeamProjectReference(requestContext)
      };
    }

    private static ProjectInfo ReadProjectInfo(IVssRequestContext requestContext, Guid projectId)
    {
      ProjectInfo project = requestContext.GetService<IProjectService>().GetProject(requestContext, projectId);
      project.PopulateProperties(requestContext, "System.SourceControlGitEnabled", "System.SourceControlTfvcEnabled", "System.SourceControlCapabilityFlags");
      return project;
    }

    private static bool HasTrueProjectProperty(ProjectInfo project, string propertyName)
    {
      ProjectProperty projectProperty = TeamProjectVersionControlInfoUtil.GetProjectProperty(project, propertyName);
      return projectProperty != null && string.Equals((string) projectProperty.Value, bool.TrueString);
    }

    private static SourceControlTypes GetDefaultSourceControlType(ProjectInfo project)
    {
      ProjectProperty projectProperty = TeamProjectVersionControlInfoUtil.GetProjectProperty(project, "System.SourceControlCapabilityFlags");
      SourceControlTypes result;
      return projectProperty != null && Enum.TryParse<SourceControlTypes>((string) projectProperty.Value, out result) ? result : SourceControlTypes.Tfvc;
    }

    private static bool ProjectSupportsTfvc(ProjectInfo project) => TeamProjectVersionControlInfoUtil.HasTrueProjectProperty(project, "System.SourceControlTfvcEnabled") || TeamProjectVersionControlInfoUtil.GetDefaultSourceControlType(project) == SourceControlTypes.Tfvc;

    private static ProjectProperty GetProjectProperty(ProjectInfo project, string propertyName) => project.Properties.FirstOrDefault<ProjectProperty>((Func<ProjectProperty, bool>) (x => x.Name.Equals(propertyName)));
  }
}
