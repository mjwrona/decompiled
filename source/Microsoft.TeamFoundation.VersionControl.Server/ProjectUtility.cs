// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.ProjectUtility
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.VersionControl.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  public static class ProjectUtility
  {
    private static string ProjectUtilityIdNameMapKey = "VersionControl.ProjectUtility.IdNameMap";
    private static string ProjectUtilityNameIdMapKey = "VersionControl.ProjectUtility.NameIdMap";

    private static void AddProjectIdNameMappingToRequestContext(
      IVssRequestContext requestContext,
      Guid projectId,
      string projectName)
    {
      object obj1;
      if (requestContext.Items.TryGetValue(ProjectUtility.ProjectUtilityIdNameMapKey, out obj1))
      {
        ((IDictionary<Guid, string>) obj1)[projectId] = projectName;
      }
      else
      {
        object obj2 = (object) new Dictionary<Guid, string>()
        {
          {
            projectId,
            projectName
          }
        };
        requestContext.Items.Add(ProjectUtility.ProjectUtilityIdNameMapKey, obj2);
      }
    }

    private static void AddProjectNameIdMappingToRequestContext(
      IVssRequestContext requestContext,
      Guid projectId,
      string projectName)
    {
      object obj1;
      if (requestContext.Items.TryGetValue(ProjectUtility.ProjectUtilityNameIdMapKey, out obj1))
      {
        ((IDictionary<string, Guid>) obj1)[projectName] = projectId;
      }
      else
      {
        object obj2 = (object) new Dictionary<string, Guid>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
        {
          {
            projectName,
            projectId
          }
        };
        requestContext.Items.Add(ProjectUtility.ProjectUtilityNameIdMapKey, obj2);
      }
    }

    internal static void AddProjectMappingsToRequestContext(
      IVssRequestContext requestContext,
      Guid projectId,
      string projectName)
    {
      ProjectUtility.AddProjectIdNameMappingToRequestContext(requestContext, projectId, projectName);
      ProjectUtility.AddProjectNameIdMappingToRequestContext(requestContext, projectId, projectName);
    }

    public static bool TryGetProjectId(
      IVssRequestContext requestContext,
      string projectName,
      out Guid projectId)
    {
      try
      {
        projectId = ProjectUtility.GetProjectId(requestContext, projectName);
        return true;
      }
      catch (ProjectDoesNotExistWithNameException ex)
      {
        projectId = Guid.Empty;
        return false;
      }
    }

    public static Guid GetProjectId(
      IVssRequestContext requestContext,
      string projectName,
      bool requireWellFormedProjects = true)
    {
      Guid empty = Guid.Empty;
      object obj;
      if (requestContext.Items.TryGetValue(ProjectUtility.ProjectUtilityNameIdMapKey, out obj) && ((IDictionary<string, Guid>) obj).TryGetValue(projectName, out empty))
        return empty;
      ProjectInfo project;
      using (requestContext.AcquireExemptionLock())
        project = requestContext.GetService<IProjectService>().GetProject(requestContext.Elevate(), projectName, true);
      if (requireWellFormedProjects && project.State != ProjectState.WellFormed && project.State != ProjectState.Deleted && project.State != ProjectState.Deleting)
        throw new ProjectDoesNotExistWithNameException(projectName);
      Guid id = project.Id;
      projectName = ProjectInfo.NormalizeProjectName(projectName, nameof (projectName), true);
      projectName = project.KnownNames.Single<string>((Func<string, bool>) (n => n.Equals(projectName, StringComparison.OrdinalIgnoreCase)));
      ProjectUtility.AddProjectMappingsToRequestContext(requestContext, id, projectName);
      return id;
    }

    private static bool TryGetProjectName(
      IVssRequestContext requestContext,
      Guid projectId,
      out string projectName)
    {
      projectName = string.Empty;
      object obj;
      if (requestContext.Items.TryGetValue(ProjectUtility.ProjectUtilityIdNameMapKey, out obj) && ((IDictionary<Guid, string>) obj).TryGetValue(projectId, out projectName))
        return true;
      using (requestContext.AcquireExemptionLock())
      {
        IProjectService service = requestContext.GetService<IProjectService>();
        try
        {
          projectName = service.GetProjectName(requestContext.Elevate(), projectId);
          ProjectUtility.AddProjectMappingsToRequestContext(requestContext, projectId, projectName);
          return true;
        }
        catch (ProjectDoesNotExistException ex)
        {
          IList<ProjectInfo> projectHistory = service.GetProjectHistory(requestContext.Elevate(), projectId);
          if (projectHistory != null)
          {
            projectName = projectHistory.Last<ProjectInfo>().Name;
            ProjectUtility.AddProjectIdNameMappingToRequestContext(requestContext, projectId, projectName);
            return true;
          }
        }
      }
      return false;
    }

    private static bool TryGetProjectIdFromPath(
      IVssRequestContext requestContext,
      string path,
      out Guid projectId,
      out string projectName,
      out bool conversionRequired,
      bool checkCanonicalization)
    {
      projectId = Guid.Empty;
      projectName = string.Empty;
      conversionRequired = false;
      if (path != null)
      {
        try
        {
          projectName = VersionControlPath.GetTeamProjectName(path, checkCanonicalization: checkCanonicalization);
          if (!string.IsNullOrEmpty(projectName))
          {
            if (!projectName.Equals("*"))
            {
              if (Wildcard.IsWildcard(projectName))
                return false;
              if (Guid.TryParse(projectName, out projectId))
                return ProjectUtility.TryGetProjectName(requestContext, projectId, out projectName);
              conversionRequired = true;
              return ProjectUtility.TryGetProjectId(requestContext, projectName, out projectId);
            }
          }
        }
        catch (Exception ex)
        {
          return false;
        }
      }
      return true;
    }

    private static bool TryGetProjectNameFromPath(
      IVssRequestContext requestContext,
      string path,
      out Guid projectId,
      out string projectName,
      bool checkCanonicalization)
    {
      projectId = Guid.Empty;
      projectName = string.Empty;
      if (path != null)
      {
        string teamProjectName = VersionControlPath.GetTeamProjectName(path, true, checkCanonicalization);
        if (!string.IsNullOrEmpty(teamProjectName))
          return Guid.TryParse(teamProjectName, out projectId) && ProjectUtility.TryGetProjectName(requestContext, projectId, out projectName);
      }
      return true;
    }

    public static bool TryConvertToPathWithProjectId(
      IVssRequestContext requestContext,
      string path,
      out string convertedPath,
      out Guid projectId,
      out string projectName,
      bool checkCanonicalization = false)
    {
      convertedPath = path;
      bool conversionRequired;
      if (!ProjectUtility.TryGetProjectIdFromPath(requestContext, path, out projectId, out projectName, out conversionRequired, checkCanonicalization))
        return false;
      if (conversionRequired && projectId != Guid.Empty)
        convertedPath = "$/" + projectId.ToString() + path.Substring(projectName.Length + "$/".Length);
      return true;
    }

    public static bool TryConvertToPathWithProjectName(
      IVssRequestContext requestContext,
      string path,
      out string convertedPath,
      out Guid projectId,
      out string projectName,
      bool checkCanonicalization = false)
    {
      convertedPath = path;
      projectId = Guid.Empty;
      projectName = string.Empty;
      if (string.IsNullOrEmpty(path) || !VersionControlPath.IsServerItem(path) || VersionControlPath.IsRootFolder(path))
        return true;
      if (!ProjectUtility.TryGetProjectNameFromPath(requestContext, path, out projectId, out projectName, checkCanonicalization) || string.IsNullOrEmpty(projectName))
        return false;
      convertedPath = ProjectUtility.ReplaceGuidPathWithName(path, projectName);
      return true;
    }

    internal static Guid GetProjectIdFromPathPair(
      IVssRequestContext requestContext,
      ItemPathPair pathPair)
    {
      return ProjectUtility.GetProjectIdFromPath(requestContext, pathPair.ProjectGuidPath ?? pathPair.ProjectNamePath);
    }

    public static Guid GetProjectIdFromPath(IVssRequestContext requestContext, string path)
    {
      Guid projectId;
      ProjectUtility.ConvertToPathWithProjectId(requestContext, path, out projectId, out string _);
      return projectId;
    }

    public static string ReplaceGuidPathWithName(string path, string projectName) => projectName != null ? "$/" + projectName + path.Substring(36 + "$/".Length) : path;

    public static string ConvertToPathWithProjectId(
      IVssRequestContext requestContext,
      string path,
      out Guid projectId,
      out string projectName,
      bool checkCanonicalization = true)
    {
      string convertedPath;
      if (ProjectUtility.TryConvertToPathWithProjectId(requestContext, path, out convertedPath, out projectId, out projectName))
        return convertedPath;
      if (Wildcard.IsWildcard(projectName))
        throw new WildcardNotAllowedException("WildcardNotAllowedInProjectName", Array.Empty<object>());
      throw new TeamProjectNotFoundException(projectName);
    }

    public static string ConvertToPathWithProjectName(
      IVssRequestContext requestContext,
      string path,
      bool checkCanonicalization = true)
    {
      string convertedPath;
      Guid projectId;
      if (ProjectUtility.TryConvertToPathWithProjectName(requestContext, path, out convertedPath, out projectId, out string _, checkCanonicalization))
        return convertedPath;
      if (projectId == Guid.Empty)
        throw new IllegalServerItemException(path);
      throw new TeamProjectNotFoundException(projectId.ToString());
    }

    public static List<ProjectInfo> GetProjectsWithTfvcProperty(IVssRequestContext requestContext)
    {
      List<ProjectInfo> withTfvcProperty = new List<ProjectInfo>();
      using (requestContext.AcquireExemptionLock())
      {
        IProjectService service = requestContext.GetService<IProjectService>();
        string[] projectPropertyFilters = new string[1]
        {
          "System.SourceControlTfvcEnabled"
        };
        foreach (ProjectInfo project in service.GetProjects(requestContext))
        {
          if (project.State == ProjectState.WellFormed || project.State == ProjectState.Deleting)
          {
            try
            {
              ProjectProperty projectProperty = service.GetProjectProperties(requestContext, project.Id, (IEnumerable<string>) projectPropertyFilters).FirstOrDefault<ProjectProperty>();
              if (projectProperty != null)
              {
                if (object.Equals(projectProperty.Value, (object) bool.TrueString))
                  withTfvcProperty.Add(project);
              }
            }
            catch (ProjectDoesNotExistException ex)
            {
              requestContext.Trace(700354, TraceLevel.Error, TraceArea.General, TraceLayer.BusinessLogic, "Project does not exist exception usually caused by race condition.  Project Name:   " + project.Name);
            }
          }
        }
      }
      return withTfvcProperty;
    }
  }
}
