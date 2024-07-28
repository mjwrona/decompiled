// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.ProjectHelper
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Artifact.Extensions, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AA75D202-9F5E-426B-B40F-64BEE45B1703
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement2.Artifact.Extensions.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions
{
  public static class ProjectHelper
  {
    public static string GetProjectName(IVssRequestContext context, Guid projectId) => context != null ? context.GetService<IProjectService>().GetProjectName(context, projectId) : throw new ArgumentNullException(nameof (context));

    public static IList<ProjectInfo> GetProjects(IVssRequestContext context) => context != null ? (IList<ProjectInfo>) context.GetService<IProjectService>().GetProjects(context, ProjectState.WellFormed).ToList<ProjectInfo>() : throw new ArgumentNullException(nameof (context));

    public static ProjectInfo GetProject(IVssRequestContext context, Guid projectId) => context != null ? context.GetService<IProjectService>().GetProject(context, projectId) : throw new ArgumentNullException(nameof (context));

    public static IList<ProjectInfo> FilterProjects(
      IList<ProjectInfo> projects,
      string propertyName)
    {
      return projects == null ? (IList<ProjectInfo>) new List<ProjectInfo>() : (IList<ProjectInfo>) projects.Where<ProjectInfo>((Func<ProjectInfo, bool>) (project => ProjectHelper.HasTrueProjectProperty(project, propertyName))).ToList<ProjectInfo>();
    }

    public static bool HasTrueProjectProperty(ProjectInfo projectInfo, string propertyName)
    {
      ProjectProperty projectProperty = projectInfo != null ? ProjectHelper.GetProjectProperty(projectInfo, propertyName) : throw new ArgumentNullException(nameof (projectInfo));
      return projectProperty != null && string.Equals((string) projectProperty.Value, bool.TrueString);
    }

    public static IList<Guid> FilterSoftDeletedProjects(
      IVssRequestContext context,
      IList<Guid> projectIds)
    {
      if (projectIds == null)
        return (IList<Guid>) new List<Guid>();
      IList<TeamProjectReference> softDeletedProjects = ProjectHelper.GetSoftDeletedProjects(context);
      if (softDeletedProjects.IsNullOrEmpty<TeamProjectReference>())
        return projectIds;
      Dictionary<Guid, bool> dictionary = softDeletedProjects.Select<TeamProjectReference, Guid>((Func<TeamProjectReference, Guid>) (x => x.Id)).ToDictionary<Guid, Guid, bool>((Func<Guid, Guid>) (x => x), (Func<Guid, bool>) (x => true));
      List<Guid> guidList = new List<Guid>();
      foreach (Guid projectId in (IEnumerable<Guid>) projectIds)
      {
        if (!dictionary.ContainsKey(projectId))
          guidList.Add(projectId);
      }
      return (IList<Guid>) guidList;
    }

    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Should catch all exceptions")]
    private static IList<TeamProjectReference> GetSoftDeletedProjects(IVssRequestContext context)
    {
      IList<TeamProjectReference> softDeletedProjects = (IList<TeamProjectReference>) null;
      try
      {
        softDeletedProjects = (IList<TeamProjectReference>) context.GetClient<ProjectHttpClient>().GetProjects(new ProjectState?(ProjectState.Deleted)).Result;
      }
      catch (Exception ex)
      {
        context.TraceException(1980007, "ReleaseManagementService", "JobLayer", ex);
      }
      return softDeletedProjects;
    }

    private static ProjectProperty GetProjectProperty(ProjectInfo project, string propertyName) => project.Properties.FirstOrDefault<ProjectProperty>((Func<ProjectProperty, bool>) (x => x.Name.Equals(propertyName)));
  }
}
