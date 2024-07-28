// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Devops.Teams.Service.TeamProjectUtil
// Assembly: Microsoft.Azure.Devops.Teams.Service, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 643BB522-27FA-4CE7-8A1E-31D95F8AC409
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Devops.Teams.Service.dll

using Microsoft.Azure.Boards.ProcessTemplates;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace Microsoft.Azure.Devops.Teams.Service
{
  internal static class TeamProjectUtil
  {
    private static readonly RegistryQuery s_waitTimeRegistry = (RegistryQuery) "/Service/ProjectWorkflowService/QueueDeleteProjectWaitTime";
    private static readonly string s_area = nameof (TeamProjectUtil);
    private static readonly string s_layer = nameof (TeamProjectUtil);

    public static bool IsTpcOptedOutOfPromote(IVssRequestContext requestContext)
    {
      requestContext.TraceEnter(1000002, TeamProjectUtil.s_area, TeamProjectUtil.s_layer, nameof (IsTpcOptedOutOfPromote));
      ArtifactSpec artifactSpec = new ArtifactSpec(ArtifactKinds.ProcessTemplate, 0, 0);
      using (TeamFoundationDataReader properties = requestContext.GetService<TeamFoundationPropertyService>().GetProperties(requestContext, artifactSpec, (IEnumerable<string>) new string[1]
      {
        ProcessTemplateVersionPropertyNames.IsOptOutOfPromote
      }))
      {
        IEnumerator enumerator1 = properties.GetEnumerator();
        try
        {
          if (enumerator1.MoveNext())
          {
            using (IEnumerator<PropertyValue> enumerator2 = ((ArtifactPropertyValue) enumerator1.Current).PropertyValues.GetEnumerator())
            {
              if (enumerator2.MoveNext())
              {
                PropertyValue current = enumerator2.Current;
                bool result = false;
                bool.TryParse(current.Value as string, out result);
                requestContext.TraceLeave(1000003, TeamProjectUtil.s_area, TeamProjectUtil.s_layer, nameof (IsTpcOptedOutOfPromote));
                return result;
              }
            }
          }
        }
        finally
        {
          if (enumerator1 is IDisposable disposable)
            disposable.Dispose();
        }
      }
      requestContext.TraceLeave(1000003, TeamProjectUtil.s_area, TeamProjectUtil.s_layer, nameof (IsTpcOptedOutOfPromote));
      return false;
    }

    public static void UpdateProjectProcessTemplate(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid processTemplateTypeId)
    {
      IProjectService service = requestContext.GetService<IProjectService>();
      IEnumerable<ProjectProperty> projectProperties = service.GetProjectProperties(requestContext, projectId, (IEnumerable<string>) new string[2]
      {
        ProcessTemplateIdPropertyNames.OriginalProcessTemplateId,
        ProcessTemplateIdPropertyNames.ProcessTemplateType
      });
      List<ProjectProperty> projectPropertyList = new List<ProjectProperty>();
      if (!projectProperties.Any<ProjectProperty>((Func<ProjectProperty, bool>) (prop => prop.Name == ProcessTemplateIdPropertyNames.ProcessTemplateType)) || (string) projectProperties.First<ProjectProperty>((Func<ProjectProperty, bool>) (prop => prop.Name == ProcessTemplateIdPropertyNames.ProcessTemplateType)).Value != processTemplateTypeId.ToString("D"))
        projectPropertyList.Add(new ProjectProperty()
        {
          Name = ProcessTemplateIdPropertyNames.ProcessTemplateType,
          Value = (object) processTemplateTypeId.ToString("D")
        });
      ProcessDescriptor descriptor;
      requestContext.GetService<ITeamFoundationProcessService>().TryGetProcessDescriptor(requestContext, processTemplateTypeId, out descriptor);
      if (descriptor != null)
      {
        if (!projectProperties.Any<ProjectProperty>((Func<ProjectProperty, bool>) (prop => prop.Name == ProcessTemplateIdPropertyNames.OriginalProcessTemplateId)))
          projectPropertyList.Add(new ProjectProperty()
          {
            Name = ProcessTemplateIdPropertyNames.OriginalProcessTemplateId,
            Value = (object) descriptor.RowId.ToString("D")
          });
        projectPropertyList.Add(new ProjectProperty()
        {
          Name = ProcessTemplateIdPropertyNames.CurrentProcessTemplateId,
          Value = (object) descriptor.RowId.ToString("D")
        });
      }
      service.CheckProjectPermission(requestContext, ProjectInfo.GetProjectUri(projectId), TeamProjectPermissions.GenericWrite);
      service.SetProjectProperties(requestContext.Elevate(), projectId, (IEnumerable<ProjectProperty>) projectPropertyList);
    }

    public static void FixProjectProcessTemplateDescriptorsForOOBProjects(
      IVssRequestContext requestContext,
      Guid projectId)
    {
      IProjectService service1 = requestContext.GetService<IProjectService>();
      ITeamFoundationProcessService service2 = requestContext.GetService<ITeamFoundationProcessService>();
      IEnumerable<ProjectProperty> projectProperties = service1.GetProjectProperties(requestContext, projectId, (IEnumerable<string>) new string[2]
      {
        ProcessTemplateIdPropertyNames.CurrentProcessTemplateId,
        ProcessTemplateIdPropertyNames.ProcessTemplateType
      });
      Guid result1 = Guid.Empty;
      Guid result2 = Guid.Empty;
      foreach (ProjectProperty projectProperty in projectProperties)
      {
        if (projectProperty.Name.Equals(ProcessTemplateIdPropertyNames.CurrentProcessTemplateId))
          Guid.TryParse(projectProperty.Value.ToString(), out result1);
        else if (projectProperty.Name.Equals(ProcessTemplateIdPropertyNames.ProcessTemplateType))
          Guid.TryParse(projectProperty.Value.ToString(), out result2);
      }
      ProcessDescriptor descriptor;
      if (!service2.TryGetProcessDescriptor(requestContext, result2, out descriptor) || !descriptor.IsSystem || !(descriptor.RowId != result1))
        return;
      ProjectProperty projectProperty1 = new ProjectProperty(ProcessTemplateIdPropertyNames.CurrentProcessTemplateId, (object) descriptor.RowId);
      ProjectProperty projectProperty2 = new ProjectProperty(ProcessTemplateIdPropertyNames.OriginalProcessTemplateId, (object) descriptor.RowId);
      service1.SetProjectProperties(requestContext.Elevate(), projectId, (IEnumerable<ProjectProperty>) new ProjectProperty[2]
      {
        projectProperty1,
        projectProperty2
      });
    }

    public static ProcessDescriptor GetProjectProcessTemplateDescriptor(
      IVssRequestContext requestContext,
      Guid projectId)
    {
      IProjectService service1 = requestContext.GetService<IProjectService>();
      ITeamFoundationProcessService service2 = requestContext.GetService<ITeamFoundationProcessService>();
      IVssRequestContext requestContext1 = requestContext;
      Guid projectId1 = projectId;
      string[] strArray = new string[1]
      {
        ProcessTemplateIdPropertyNames.CurrentProcessTemplateId
      };
      IEnumerable<ProjectProperty> projectProperties = service1.GetProjectProperties(requestContext1, projectId1, strArray);
      Guid result = Guid.Empty;
      ProjectProperty projectProperty = projectProperties.FirstOrDefault<ProjectProperty>((Func<ProjectProperty, bool>) (property => property.Name == ProcessTemplateIdPropertyNames.CurrentProcessTemplateId));
      if (projectProperty == null)
        return (ProcessDescriptor) null;
      if (!Guid.TryParse((string) projectProperty.Value, out result))
      {
        requestContext.Trace(1000042, TraceLevel.Error, TeamProjectUtil.s_area, TeamProjectUtil.s_layer, "Found project {0} with process rowId {1}, but this guid could not be parsed.", (object) projectId.ToString(), (object) (string) projectProperty.Value);
        return (ProcessDescriptor) null;
      }
      ProcessDescriptor descriptor;
      if (service2.TryGetSpecificProcessDescriptor(requestContext, result, out descriptor))
        return descriptor;
      requestContext.Trace(1000041, TraceLevel.Error, TeamProjectUtil.s_area, TeamProjectUtil.s_layer, "Found project {0} with process rowId {1}, but that process could not be found.", (object) projectId.ToString(), (object) result.ToString());
      return (ProcessDescriptor) null;
    }

    public static ArtifactSpec GetProjectPropertySpec(Guid projectId) => new ArtifactSpec(TeamProjectPropertyConstants.ArtifactKindId, TeamProjectPropertyConstants.ArtifactId, 0, projectId);

    public static ILeaseInfo AcquireProjectLease(
      IVssRequestContext requestContext,
      Guid projectId,
      TimeSpan leaseTime,
      TimeSpan timeout)
    {
      requestContext.CheckProjectCollectionRequestContext();
      return requestContext.GetService<ILeaseService>().AcquireLease(requestContext, TeamProjectUtil.GetLeaseName(projectId), leaseTime, timeout);
    }

    public static void ReleaseProjectLease(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid leaseOwner)
    {
      requestContext.CheckProjectCollectionRequestContext();
      ArgumentUtility.CheckForEmptyGuid(leaseOwner, nameof (leaseOwner));
      requestContext.GetService<IInternalLeaseService>().ReleaseLease(requestContext, TeamProjectUtil.GetLeaseName(projectId), leaseOwner);
    }

    public static void PrepareProjectCreation(
      IVssRequestContext requestContext,
      string projectName,
      string projectDescription,
      Guid processTemplateTypeId,
      ProjectVisibility projectVisibility,
      bool isPreCreate,
      out Guid pendingProjectGuid,
      out ILeaseInfo lease,
      out ProcessDescriptor processTemplateDescriptor)
    {
      ITeamFoundationProcessService service1 = requestContext.GetService<ITeamFoundationProcessService>();
      processTemplateDescriptor = service1.GetProcessDescriptor(requestContext, processTemplateTypeId);
      if (service1.GetDisabledProcessTypeIds(requestContext).Contains(processTemplateTypeId))
        throw new CannotCreateProjectsWithDisableProcessException(processTemplateDescriptor.Name);
      if (processTemplateDescriptor.ProcessStatus != ProcessStatus.Ready)
        throw new ProcessStatusNotReadyException(processTemplateDescriptor.Name);
      projectName = projectName.Trim();
      IProjectService service2 = requestContext.GetService<IProjectService>();
      pendingProjectGuid = service2.ReserveProject(requestContext, projectName);
      lease = TeamProjectUtil.AcquireProjectLease(requestContext, pendingProjectGuid, TimeSpan.FromMinutes(5.0), TimeSpan.Zero);
      requestContext.RequestContextInternal().RemoveLease(lease.LeaseName);
      requestContext.GetService<IDataspaceService>().CreateDataspaces(requestContext, new string[1]
      {
        "Default"
      }, pendingProjectGuid);
      if (!requestContext.IsFeatureEnabled("WebAccess.Process.XmlTemplateProcess") || requestContext.IsFeatureEnabled("WebAccess.Process.Hierarchy"))
        TeamProjectUtil.UpdateProjectProcessTemplate(requestContext.Elevate(), pendingProjectGuid, processTemplateTypeId);
      else
        service2.SetProjectProperties(requestContext.Elevate(), pendingProjectGuid, (IEnumerable<ProjectProperty>) new List<ProjectProperty>()
        {
          new ProjectProperty()
          {
            Name = ProcessTemplateIdPropertyNames.ProcessTemplateTypeAtCreation,
            Value = (object) processTemplateTypeId
          }
        });
      if (isPreCreate)
        service2.SetProjectProperties(requestContext, pendingProjectGuid, (IEnumerable<ProjectProperty>) new ProjectProperty[1]
        {
          new ProjectProperty("System.ProjectPreCreated", (object) true)
        });
      processTemplateDescriptor = service1.GetProcessDescriptor(requestContext, processTemplateTypeId);
      if (processTemplateDescriptor.ProcessStatus != ProcessStatus.Ready)
        throw new ProcessStatusNotReadyException(processTemplateDescriptor.Name);
    }

    public static void CheckAcquireProjectLease(IVssRequestContext requestContext, Guid projectId)
    {
      TimeSpan timeout = requestContext.GetService<IVssRegistryService>().GetValue<TimeSpan>(requestContext, in TeamProjectUtil.s_waitTimeRegistry, true, TimeSpan.FromSeconds(10.0));
      try
      {
        using (TeamProjectUtil.AcquireProjectLease(requestContext, projectId, TimeSpan.FromSeconds(5.0), timeout))
          ;
      }
      catch (AcquireLeaseTimeoutException ex)
      {
        throw new ProjectWorkPendingException(projectId.ToString("D"));
      }
    }

    private static ArtifactSpec GetProcessTemplateVersionSpec(string projectUri)
    {
      Guid projectId = ProjectInfo.GetProjectId(projectUri);
      return new ArtifactSpec(ArtifactKinds.ProcessTemplate, projectId.ToByteArray(), 0);
    }

    private static string GetLeaseName(Guid projectId)
    {
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Project-{0}", (object) projectId.ToString("D"));
    }
  }
}
