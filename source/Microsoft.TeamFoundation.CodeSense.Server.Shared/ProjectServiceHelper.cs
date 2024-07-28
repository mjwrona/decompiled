// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.CodeSense.Server.ProjectServiceHelper
// Assembly: Microsoft.TeamFoundation.CodeSense.Server.Shared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 548902A5-AE61-4BC7-8D52-315B40AB5900
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.CodeSense.Server.Shared.dll

using Microsoft.TeamFoundation.CodeSense.Platform.Abstraction;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.CodeSense.Server
{
  public static class ProjectServiceHelper
  {
    public static void CreateProjectDataspacesForCollection(IVssRequestContext requestContext)
    {
      IProjectService service = requestContext.GetService<IProjectService>();
      List<ProjectMapEntity> projectMapEntityList = new List<ProjectMapEntity>();
      List<Guid> guidList = new List<Guid>();
      try
      {
        IEnumerable<ProjectInfo> projects = service.GetProjects(requestContext);
        if (projects == null || !projects.Any<ProjectInfo>())
          return;
        List<Guid> list = projects.Select<ProjectInfo, Guid>((Func<ProjectInfo, Guid>) (project => project.Id)).ToList<Guid>();
        ProjectServiceHelper.CreateProjectDataspacesForGivenProjects(requestContext, list);
        string format = string.Format("CreateProjectDataspacesForCollection() succeeded for the collection : {0}", (object) requestContext.ServiceHost.InstanceId);
        requestContext.Trace(1024425, TraceLayer.Job, format);
      }
      catch (Exception ex)
      {
        string message = string.Format("CreateProjectDataspacesForCollection() failed for the collection : {0} with the following exception : {1}", (object) requestContext.ServiceHost.InstanceId, (object) ex.ToString());
        requestContext.Trace(1024430, TraceLevel.Error, "CodeSense", TraceLayer.Job, message);
        throw;
      }
    }

    public static void CreateProjectDataspaceForProject(
      IVssRequestContext requestContext,
      string projectName,
      Guid projectIdentifier)
    {
      if (!string.IsNullOrEmpty(projectName))
      {
        if (!(projectIdentifier == Guid.Empty))
        {
          try
          {
            if (DataspaceServiceHelper.QueryDataspace(requestContext, "Default", projectIdentifier) != null)
              return;
            ProjectServiceHelper.CreateProjectDataspacesForGivenProjects(requestContext, ((IEnumerable<Guid>) new Guid[1]
            {
              projectIdentifier
            }).ToList<Guid>());
            string format = string.Format("CreateProjectDataspaceForProject() succeeded for the collection : {0} and project : {1}", (object) requestContext.ServiceHost.InstanceId, (object) projectIdentifier);
            requestContext.Trace(1024440, TraceLayer.Job, format);
            return;
          }
          catch (Exception ex)
          {
            string message = string.Format("CreateProjectDataspaceForProject() failed for project guid {0} with the following exception : {1}", (object) projectIdentifier, (object) ex.ToString());
            requestContext.Trace(1024445, TraceLevel.Error, "CodeSense", TraceLayer.Job, message);
            throw;
          }
        }
      }
      string message1 = string.Format("CreateProjectDataspaceForProject() failed due incorrect parameters. projectName : {0}, projectGuid : {1}", (object) projectName, (object) projectIdentifier);
      requestContext.Trace(1024435, TraceLevel.Error, "CodeSense", TraceLayer.Job, message1);
    }

    public static Guid GetProjectId(
      IVssRequestContext requestContext,
      string projectName,
      ProjectMapCache projectMapCache = null)
    {
      if (string.IsNullOrEmpty(projectName) || char.ToUpper(projectName[0]) == 'D' && Guid.TryParse(projectName.Substring(1), out Guid _))
        return Guid.Empty;
      IProjectService service = requestContext.GetService<IProjectService>();
      Guid projectGuid = Guid.Empty;
      if (projectMapCache != null && projectMapCache.TryGetValue(projectName, out projectGuid))
        return projectGuid;
      try
      {
        projectGuid = service.GetProjectId(requestContext.Elevate(), projectName, true);
        projectMapCache?.AddByName(projectName, projectGuid);
        return projectGuid;
      }
      catch (ProjectDoesNotExistWithNameException ex)
      {
        string message = string.Format("GetProjectId() could not find the projectId for the project name {0} and failed with the following exception : {1}", (object) projectName, (object) ex.ToString());
        requestContext.Trace(1024470, TraceLevel.Error, "CodeSense", TraceLayer.Job, message);
        return Guid.Empty;
      }
      catch (Exception ex)
      {
        string message = string.Format("GetProjectId() failed for project name {0} with the following exception : {1}", (object) projectName, (object) ex.ToString());
        requestContext.Trace(1024450, TraceLevel.Error, "CodeSense", TraceLayer.Job, message);
        throw;
      }
    }

    public static string GetProjectName(
      IVssRequestContext requestContext,
      Guid projectGuid,
      ProjectMapCache projectMapCache = null)
    {
      if (projectGuid.Equals(Guid.Empty))
        return string.Empty;
      IProjectService service = requestContext.GetService<IProjectService>();
      string projectName1 = string.Empty;
      if (projectMapCache != null && projectMapCache.TryGetValue(projectGuid, out projectName1))
        return projectName1;
      try
      {
        string projectName2 = service.GetProjectName(requestContext.Elevate(), projectGuid);
        projectMapCache?.AddByGuid(projectGuid, projectName2);
        return projectName2;
      }
      catch (ProjectDoesNotExistException ex)
      {
        string message = string.Format("GetProjectName() could not find the project name for the project id {0} and failed with the following exception : {1}", (object) projectGuid, (object) ex.ToString());
        requestContext.Trace(1024470, TraceLevel.Error, "CodeSense", TraceLayer.Job, message);
        return string.Empty;
      }
      catch (Exception ex)
      {
        string message = string.Format("GetProjectName() failed for project Guid {0} with the following exception : {1}", (object) projectGuid, (object) ex.ToString());
        requestContext.Trace(1024455, TraceLevel.Error, "CodeSense", TraceLayer.Job, message);
        throw;
      }
    }

    private static void CreateProjectDataspacesForGivenProjects(
      IVssRequestContext requestContext,
      List<Guid> projectIds)
    {
      DataspaceServiceHelper.CreateDataspaces(requestContext, "Default", projectIds, DataspaceState.Active);
    }
  }
}
