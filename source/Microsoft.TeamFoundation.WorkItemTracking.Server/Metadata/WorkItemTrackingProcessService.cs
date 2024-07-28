// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemTrackingProcessService
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.Azure.Boards.ProcessTemplates;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.AuditLog;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Process;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Rules;
using Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessComparer.Declarations;
using Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessComparer.Packaging;
using Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessComparer.Repository;
using Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessMetadata;
using Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessMetadata.XmlToInherited;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata
{
  public class WorkItemTrackingProcessService : IWorkItemTrackingProcessService, IVssFrameworkService
  {
    private Guid? m_notificationAuthor;

    public void ServiceStart(IVssRequestContext requestContext) => this.m_notificationAuthor = new Guid?(requestContext.GetService<ITeamFoundationSqlNotificationService>().Author);

    public void ServiceEnd(IVssRequestContext requestContext)
    {
    }

    public ProcessDescriptor CreateInheritedProcess(
      IVssRequestContext requestContext,
      string name,
      string referenceName,
      string description,
      Guid parentTypeId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<string>(name, nameof (name));
      ArgumentUtility.CheckForEmptyGuid(parentTypeId, nameof (parentTypeId));
      return requestContext.TraceBlock<ProcessDescriptor>(10005100, 10005101, "ProcessTemplate", nameof (WorkItemTrackingProcessService), nameof (CreateInheritedProcess), (Func<ProcessDescriptor>) (() =>
      {
        ITeamFoundationProcessService service = requestContext.GetService<ITeamFoundationProcessService>();
        name = name.Trim();
        service.CheckValidProcessName(name);
        Guid guid = Guid.NewGuid();
        referenceName = referenceName == null ? this.GenerateReferenceName(guid) : referenceName.Trim();
        WorkItemTrackingProcessService.CheckValidProcessReferenceName(referenceName);
        WorkItemTrackingProcessService.CheckDescriptionText(description);
        ProcessDescriptor processDescriptor = service.GetProcessDescriptor(requestContext, parentTypeId);
        if (!processDescriptor.IsSystem)
          throw new ProcessInvalidParentException();
        if (processDescriptor.IsDerived)
          throw new ProcessInvalidParentException();
        this.CheckDeploymentLevelNameCollisions(requestContext, name, referenceName);
        service.CheckProcessPermission(requestContext, processDescriptor, 4, false);
        this.CheckMaxProcessLimitRespected(requestContext);
        Guid identityIdInternal = this.GetUserIdentityIdInternal(requestContext);
        ProcessDescriptor descriptor;
        using (ProcessTemplateComponent component = this.CreateComponent(requestContext))
          descriptor = (ProcessDescriptor) new ProcessDescriptorImpl(component.CreateInheritedProcess(processDescriptor, guid, name, referenceName, description, identityIdInternal));
        this.PublishSqlNotification(requestContext, ProcessConstants.Notifications.ProcessChanged, TeamFoundationSerializationUtility.SerializeToString<ProcessDescriptorNotificationRecord[]>(new ProcessDescriptorNotificationRecord[1]
        {
          new ProcessDescriptorNotificationRecord()
          {
            SpecificId = descriptor.RowId,
            TypeId = descriptor.TypeId
          }
        }, new XmlRootAttribute("descriptors")));
        service.AddProcessPermission(requestContext, descriptor);
        requestContext.GetService<ProcessDescriptorCacheService>().Set(requestContext, descriptor);
        CustomerIntelligenceData properties = new CustomerIntelligenceData();
        properties.Add("ProcessId", (object) descriptor.TypeId);
        properties.Add("ProcessName", descriptor.Name);
        properties.Add("ParentId", (object) parentTypeId);
        properties.Add("ParentName", processDescriptor.Name);
        requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, "ProcessTemplate", "AddInheritedProcess", properties);
        return descriptor;
      }));
    }

    public IReadOnlyCollection<ProcessMigrationResult> MigrateProjectsProcess(
      IVssRequestContext requestContext,
      IEnumerable<ProcessMigrationModel> migrationRequests)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IEnumerable<ProcessMigrationModel>>(migrationRequests, nameof (migrationRequests));
      if (migrationRequests.Contains<ProcessMigrationModel>((ProcessMigrationModel) null))
        throw new ArgumentException(Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.InvalidProjectProvidedInMigratingProjectList(), nameof (migrationRequests));
      return (IReadOnlyCollection<ProcessMigrationResult>) requestContext.TraceBlock<List<ProcessMigrationResult>>(10005104, 10005105, "ProcessTemplate", "TeamFoundationProcessService", nameof (MigrateProjectsProcess), (Func<List<ProcessMigrationResult>>) (() =>
      {
        IProcessTemplateExtensionService service1 = requestContext.GetService<IProcessTemplateExtensionService>();
        List<ProcessMigrationResult> processMigrationResultList = new List<ProcessMigrationResult>();
        List<ProjectInfo> projectInfoList = new List<ProjectInfo>();
        ITeamFoundationProcessService service2 = requestContext.GetService<ITeamFoundationProcessService>();
        ISet<Guid> disabledProcessTypeIds = service2.GetDisabledProcessTypeIds(requestContext);
        foreach (ProcessMigrationModel migrationRequest in migrationRequests)
        {
          ProjectInfo project = (ProjectInfo) null;
          try
          {
            ProcessDescriptor processDescriptor;
            this.TryGetProjectAndProcessDescriptor(requestContext, migrationRequest.ProjectId, out project, out processDescriptor);
            ProcessDescriptor descriptor;
            if (!service2.TryGetProcessDescriptor(requestContext, migrationRequest.NewProcessTypeId, out descriptor))
            {
              ProcessNotFoundByTypeIdException byTypeIdException = new ProcessNotFoundByTypeIdException(migrationRequest.NewProcessTypeId);
              processMigrationResultList.Add(new ProcessMigrationResult()
              {
                ProjectId = migrationRequest.ProjectId,
                Project = project,
                MigrationException = (Exception) byTypeIdException
              });
            }
            else if (disabledProcessTypeIds.Contains(descriptor.TypeId))
            {
              CannotMigrateToDisabledProcessException processException = new CannotMigrateToDisabledProcessException();
              processMigrationResultList.Add(new ProcessMigrationResult()
              {
                ProjectId = migrationRequest.ProjectId,
                Project = project,
                MigrationException = (Exception) processException
              });
            }
            else
            {
              if (processDescriptor != null)
              {
                bool flag1 = !processDescriptor.IsDerived && descriptor.Inherits == processDescriptor.TypeId;
                bool flag2 = !descriptor.IsDerived && processDescriptor.Inherits == descriptor.TypeId;
                bool flag3 = processDescriptor.IsDerived && descriptor.IsDerived && processDescriptor.Inherits == descriptor.Inherits;
                bool flag4 = processDescriptor.IsCustom || descriptor.IsCustom;
                bool flag5 = WorkItemTrackingFeatureFlags.IsProjectChangeProcessEnabled(requestContext);
                if (flag5 & flag4 || !flag5 && !flag1 && !flag2 && !flag3)
                {
                  ProcessInvalidProjectMigrationException migrationException = new ProcessInvalidProjectMigrationException(project.Name, processDescriptor.Name, descriptor.Name);
                  processMigrationResultList.Add(new ProcessMigrationResult()
                  {
                    ProjectId = migrationRequest.ProjectId,
                    Project = project,
                    MigrationException = (Exception) migrationException
                  });
                  continue;
                }
                if (!this.HasPermissionToChangeProcessOfProject(requestContext, project))
                  throw new ProjectProcessChangeAccessDeniedException(Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.InsufficientPermissionToChangeProcessOfProjectExceptionMessage((object) project.Name));
                service1.RunProjectProcessMigrationReadinessChecks(requestContext, project, processDescriptor, descriptor);
              }
              service2.UpdateProjectProcess(requestContext, project, descriptor);
              projectInfoList.Add(project);
              if (processDescriptor == null || processDescriptor.Name.IsNullOrEmpty<char>())
              {
                IVssRequestContext requestContext1 = requestContext;
                string withoutOldProcess = ProjectAuditConstants.ProjectProcessModifiedWithoutOldProcess;
                Dictionary<string, object> data = new Dictionary<string, object>();
                data.Add("ProcessName", (object) descriptor.Name);
                Guid id = project.Id;
                Guid targetHostId = new Guid();
                Guid projectId = id;
                requestContext1.LogAuditEvent(withoutOldProcess, data, targetHostId, projectId);
              }
              else
              {
                IVssRequestContext requestContext2 = requestContext;
                string projectProcessModified = ProjectAuditConstants.ProjectProcessModified;
                Dictionary<string, object> data = new Dictionary<string, object>();
                data.Add("ProcessName", (object) descriptor.Name);
                data.Add("OldProcessName", (object) processDescriptor?.Name);
                Guid id = project.Id;
                Guid targetHostId = new Guid();
                Guid projectId = id;
                requestContext2.LogAuditEvent(projectProcessModified, data, targetHostId, projectId);
              }
              CustomerIntelligenceData properties = new CustomerIntelligenceData();
              properties.Add("ProjectId", (object) project.Id);
              properties.Add("ProjectName", project.Name);
              properties.Add("OldTemplateId", (object) processDescriptor?.TypeId);
              properties.Add("OldTemplateName", processDescriptor?.Name);
              properties.Add("OldTemplateType", this.GetProcessTemplateTypeNameForCI(processDescriptor));
              properties.Add("NewTemplateId", (object) descriptor.TypeId);
              properties.Add("NewTemplateName", descriptor.Name);
              properties.Add("NewTemplateType", this.GetProcessTemplateTypeNameForCI(descriptor));
              requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, "ProcessTemplate", "MigratedProject", properties);
            }
          }
          catch (Exception ex)
          {
            processMigrationResultList.Add(new ProcessMigrationResult()
            {
              ProjectId = migrationRequest.ProjectId,
              Project = project,
              MigrationException = ex
            });
          }
        }
        if (projectInfoList.Any<ProjectInfo>())
          service1.QueueMigrateProjectsProcessOperations(requestContext, (IEnumerable<ProjectInfo>) projectInfoList);
        return processMigrationResultList;
      }));
    }

    public void EnableDisableProcess(
      IVssRequestContext requestContext,
      Guid processTypeId,
      bool isEnabled)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(processTypeId, nameof (processTypeId));
      ITeamFoundationProcessService service = requestContext.GetService<ITeamFoundationProcessService>();
      ProcessDescriptor processDescriptor = service.GetProcessDescriptor(requestContext, processTypeId);
      service.CheckProcessPermission(requestContext, processDescriptor, 1, false);
      if (!isEnabled && processDescriptor.TypeId == service.GetDefaultProcessTypeId(requestContext))
        throw new ProcessInvalidDisableOnDefaultException();
      Guid identityIdInternal = this.GetUserIdentityIdInternal(requestContext);
      using (ProcessTemplateComponent component = this.CreateComponent(requestContext))
        component.EnableDisableProcess(processTypeId, isEnabled, identityIdInternal);
      requestContext.GetService<ProcessDescriptorCacheService>().SetDisabledProcessId(requestContext, processTypeId, isEnabled);
      requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, "ProcessTemplate", "UpdateEnabledProcessProperty", isEnabled ? "Enabled" : "Disabled", processDescriptor.ReferenceName);
    }

    public virtual bool HasDeleteFieldPermission(IVssRequestContext requestContext) => this.HasPermission(requestContext, FrameworkSecurity.TeamProjectCollectionNamespaceId, FrameworkSecurity.TeamProjectCollectionNamespaceToken, TeamProjectCollectionPermissions.DeleteField);

    public virtual bool HasSetFieldLockPermission(IVssRequestContext requestContext) => this.HasPermission(requestContext, FrameworkSecurity.TeamProjectCollectionNamespaceId, FrameworkSecurity.TeamProjectCollectionNamespaceToken, TeamProjectCollectionPermissions.DeleteField);

    private bool HasPermissionToChangeProcessOfProject(
      IVssRequestContext requestContext,
      ProjectInfo projectInfo)
    {
      string token = projectInfo != null ? TeamProjectSecurityConstants.GetToken(projectInfo.Uri) : throw new ArgumentNullException(nameof (projectInfo));
      return this.HasPermission(requestContext, FrameworkSecurity.TeamProjectNamespaceId, token, TeamProjectPermissions.ChangeProjectsProcess, true);
    }

    private bool HasPermission(
      IVssRequestContext requestContext,
      Guid namespaceId,
      string token,
      int permission,
      bool checkActionDefined = false)
    {
      ITeamFoundationSecurityService service = requestContext.GetService<ITeamFoundationSecurityService>();
      return (service.GetSecurityNamespace(requestContext, namespaceId) ?? service.GetSecurityNamespace(requestContext, namespaceId)).HasPermission(requestContext, token, permission);
    }

    public ProcessDescriptor GetProjectProcessDescriptor(
      IVssRequestContext requestContext,
      Guid projectId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      ProjectInfo project;
      ProcessDescriptor processDescriptor;
      if (this.TryGetProjectAndProcessDescriptor(requestContext, projectId, out project, out processDescriptor))
        return processDescriptor;
      throw new ProcessProjectWithInvalidProcessException(string.IsNullOrEmpty(project.Name) ? projectId.ToString() : project.Name);
    }

    public bool TryGetProjectProcessDescriptor(
      IVssRequestContext requestContext,
      Guid projectId,
      out ProcessDescriptor processDescriptor)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      if (!(projectId == Guid.Empty))
        return this.TryGetProjectAndProcessDescriptor(requestContext, projectId, out ProjectInfo _, out processDescriptor);
      processDescriptor = (ProcessDescriptor) null;
      return false;
    }

    public virtual bool TryGetLatestProjectProcessDescriptor(
      IVssRequestContext requestContext,
      Guid projectId,
      out ProcessDescriptor processDescriptor)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      if (!(projectId == Guid.Empty))
        return this.TryGetProjectAndProcessDescriptor(requestContext, projectId, out ProjectInfo _, out processDescriptor);
      processDescriptor = (ProcessDescriptor) null;
      return false;
    }

    private bool TryGetProjectAndProcessDescriptor(
      IVssRequestContext requestContext,
      Guid projectId,
      out ProjectInfo project,
      out ProcessDescriptor processDescriptor)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      processDescriptor = (ProcessDescriptor) null;
      project = (ProjectInfo) null;
      try
      {
        requestContext.TraceEnter(10005113, "ProcessTemplate", nameof (WorkItemTrackingProcessService), nameof (TryGetProjectAndProcessDescriptor));
        if (!requestContext.IsFeatureEnabled("WebAccess.Process.Hierarchy") && !requestContext.IsFeatureEnabled("WebAccess.Process.ProcessUpload"))
          return false;
        IProjectService service = requestContext.GetService<IProjectService>();
        project = service.GetProject(requestContext, projectId);
        project.PopulateProperties(requestContext, ProcessTemplateIdPropertyNames.ProcessTemplateType);
        IList<ProjectProperty> properties = project.Properties;
        string input = properties != null ? (string) properties.FirstOrDefault<ProjectProperty>((Func<ProjectProperty, bool>) (p => StringComparer.OrdinalIgnoreCase.Equals(p.Name, ProcessTemplateIdPropertyNames.ProcessTemplateType)))?.Value : (string) (object) null;
        Guid result;
        if (!string.IsNullOrEmpty(input) && Guid.TryParse(input, out result))
        {
          int num = requestContext.GetService<ITeamFoundationProcessService>().TryGetProcessDescriptor(requestContext, result, out processDescriptor) ? 1 : 0;
          if (num == 0)
            requestContext.Trace(10005116, TraceLevel.Error, "ProcessTemplate", "TeamFoundationProcessService", string.Format("Couldn't found process descriptor from process typeid. processid: {0}, projectid: {1}", (object) result, (object) projectId));
          return num != 0;
        }
        requestContext.Trace(10005106, TraceLevel.Error, "ProcessTemplate", "TeamFoundationProcessService", "Couldn't find anything to identify the process.");
      }
      finally
      {
        requestContext.TraceLeave(10005114, "ProcessTemplate", nameof (WorkItemTrackingProcessService), nameof (TryGetProjectAndProcessDescriptor));
      }
      return false;
    }

    public ProjectProcessDescriptorMapping GetProjectProcessDescriptorMapping(
      IVssRequestContext requestContext,
      Guid projectId,
      bool expectUnmappedProject = false)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      ProjectInfo project;
      ProcessDescriptor processDescriptor;
      if (!this.TryGetProjectAndProcessDescriptor(requestContext, projectId, out project, out processDescriptor))
      {
        if (!expectUnmappedProject)
          throw new ProcessProjectWithInvalidProcessException(project.Name);
        processDescriptor = (ProcessDescriptor) null;
      }
      return new ProjectProcessDescriptorMapping()
      {
        Project = project,
        Descriptor = processDescriptor
      };
    }

    public IReadOnlyCollection<ProjectProcessDescriptorMapping> GetProjectProcessDescriptorMappings(
      IVssRequestContext requestContext,
      IEnumerable<Guid> projectIds = null,
      bool expectUnmappedProjects = false,
      bool expectInvalidProcessIds = false,
      ProjectState projectStateFilter = ProjectState.WellFormed)
    {
      return requestContext.GetService<ITeamFoundationProcessService>().GetProjectProcessDescriptorMappings(requestContext, projectIds, expectUnmappedProjects, expectInvalidProcessIds, projectStateFilter);
    }

    public ProcessDescriptor UpdateProcessNameAndDescription(
      IVssRequestContext requestContext,
      Guid processTypeId,
      string name,
      string description)
    {
      return requestContext.GetService<ITeamFoundationProcessService>().UpdateProcessNameAndDescription(requestContext, processTypeId, name, description);
    }

    public void UpdateProjectProcess(
      IVssRequestContext requestContext,
      ProjectInfo project,
      ProcessDescriptor targetProcess)
    {
      requestContext.GetService<ITeamFoundationProcessService>().UpdateProjectProcess(requestContext, project, targetProcess);
    }

    public virtual bool TryGetAllowedValues(
      IVssRequestContext requestContext,
      Guid processId,
      int fieldId,
      IEnumerable<string> workItemTypeNames,
      out IReadOnlyCollection<string> allowedValues,
      bool bypassCustomProcessCache = true)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      bool allowedValues1 = false;
      ProcessDescriptor descriptor = (ProcessDescriptor) null;
      FieldEntry fieldById = requestContext.GetService<WorkItemTrackingFieldService>().GetFieldById(requestContext, fieldId);
      allowedValues = (IReadOnlyCollection<string>) null;
      if (fieldById == null)
        return false;
      bool flag = WorkItemTrackingFeatureFlags.IsProcessCustomizationEnabled(requestContext);
      WorkItemPickList picklist;
      if (flag && fieldById.IsPicklist && requestContext.GetService<IWorkItemPickListService>().TryGetList(requestContext, fieldById.PickListId.Value, out picklist))
      {
        allowedValues = (IReadOnlyCollection<string>) picklist.Items.Select<WorkItemPickListMember, string>((Func<WorkItemPickListMember, string>) (i => i.Value)).ToList<string>();
        return true;
      }
      if (flag)
      {
        IProcessWorkItemTypeService service1 = requestContext.GetService<IProcessWorkItemTypeService>();
        ITeamFoundationProcessService processService = requestContext.GetService<ITeamFoundationProcessService>();
        switch (fieldId)
        {
          case 2:
            IWorkItemStateDefinitionService service2 = requestContext.GetService<IWorkItemStateDefinitionService>();
            if (processId == Guid.Empty)
            {
              List<string> list = service2.GetStateNames(requestContext).OrderBy<string, string>((Func<string, string>) (s => s)).ToList<string>();
              allowedValues = (IReadOnlyCollection<string>) list;
              return true;
            }
            IReadOnlyCollection<ComposedWorkItemType> source1 = service1.GetAllWorkItemTypes(requestContext, processId);
            if (workItemTypeNames != null && workItemTypeNames.Any<string>())
              source1 = (IReadOnlyCollection<ComposedWorkItemType>) source1.Where<ComposedWorkItemType>((Func<ComposedWorkItemType, bool>) (t => workItemTypeNames.Contains<string>(t.Name, (IEqualityComparer<string>) TFStringComparer.WorkItemTypeName))).ToList<ComposedWorkItemType>();
            HashSet<string> source2 = new HashSet<string>((IEqualityComparer<string>) TFStringComparer.WorkItemStateName);
            foreach (ComposedWorkItemType composedWorkItemType in (IEnumerable<ComposedWorkItemType>) source1)
            {
              IReadOnlyCollection<WorkItemStateDefinition> stateDefinitions = service2.GetCombinedStateDefinitions(requestContext, composedWorkItemType.ProcessId, composedWorkItemType.ReferenceName);
              source2.UnionWith(stateDefinitions.Select<WorkItemStateDefinition, string>((Func<WorkItemStateDefinition, string>) (s => s.Name)));
            }
            allowedValues = (IReadOnlyCollection<string>) source2.OrderBy<string, string>((Func<string, string>) (s => s)).ToList<string>();
            return true;
          case 25:
            if (processId != Guid.Empty)
            {
              List<string> list = service1.GetAllWorkItemTypes(requestContext, processId).Select<ComposedWorkItemType, string>((Func<ComposedWorkItemType, string>) (t => t.Name)).OrderBy<string, string>((Func<string, string>) (t => t)).ToList<string>();
              allowedValues = (IReadOnlyCollection<string>) list;
            }
            else
            {
              IReadOnlyCollection<ProcessDescriptor> processDescriptors = processService.GetProcessDescriptors(requestContext);
              HashSet<string> source3 = new HashSet<string>((IEqualityComparer<string>) TFStringComparer.WorkItemTypeName);
              foreach (ProcessDescriptor processDescriptor in (IEnumerable<ProcessDescriptor>) processDescriptors)
                source3.UnionWith(service1.GetAllWorkItemTypes(requestContext, processDescriptor.TypeId).Select<ComposedWorkItemType, string>((Func<ComposedWorkItemType, string>) (t => t.Name)));
              allowedValues = (IReadOnlyCollection<string>) source3.OrderBy<string, string>((Func<string, string>) (t => t)).ToList<string>();
            }
            return true;
          default:
            if (WorkItemTrackingFeatureFlags.IsProcessCustomizationEnabled(requestContext))
            {
              if (processId != Guid.Empty && processService.TryGetProcessDescriptor(requestContext, processId, out descriptor) && !descriptor.IsCustom)
              {
                allowedValues1 = this.GetAllowedAndSuggestedValuesFromOOBTypelets(requestContext, descriptor, fieldId, workItemTypeNames, out allowedValues, bypassCustomProcessCache);
                break;
              }
              if (processId == Guid.Empty)
              {
                IEnumerable<ProcessDescriptor> processDescriptors = processService.GetProcessDescriptors(requestContext).Where<ProcessDescriptor>((Func<ProcessDescriptor, bool>) (desc => !desc.IsCustom)).Select<ProcessDescriptor, ProcessDescriptor>((Func<ProcessDescriptor, ProcessDescriptor>) (desc => !desc.IsDerived ? desc : processService.GetProcessDescriptor(requestContext, desc.Inherits))).Distinct<ProcessDescriptor>();
                allowedValues = (IReadOnlyCollection<string>) new List<string>();
                bool includeDefaultValues = !WorkItemTrackingFeatureFlags.IsOobPicklistFixEnabled(requestContext);
                using (IEnumerator<ProcessDescriptor> enumerator = processDescriptors.GetEnumerator())
                {
                  while (enumerator.MoveNext())
                  {
                    ProcessDescriptor current = enumerator.Current;
                    IReadOnlyCollection<string> allowedValues2;
                    this.GetAllowedValuesFromOOBRules(requestContext, current, fieldId, workItemTypeNames, out allowedValues2, includeDefaultValues);
                    if (allowedValues2 != null)
                    {
                      allowedValues1 = true;
                      allowedValues = (IReadOnlyCollection<string>) allowedValues.Union<string>((IEnumerable<string>) allowedValues2).ToList<string>();
                    }
                  }
                  break;
                }
              }
              else
                break;
            }
            else
              break;
        }
      }
      return allowedValues1;
    }

    private bool GetAllowedAndSuggestedValuesFromOOBTypelets(
      IVssRequestContext requestContext,
      ProcessDescriptor processDescriptor,
      int fieldId,
      IEnumerable<string> workItemTypeNames,
      out IReadOnlyCollection<string> allowedValues,
      bool bypassCache = true)
    {
      HashSet<string> source = new HashSet<string>();
      IProcessWorkItemTypeService service = requestContext.GetService<IProcessWorkItemTypeService>();
      requestContext.GetService<ITeamFoundationProcessService>();
      IReadOnlyCollection<ComposedWorkItemType> allWorkItemTypes = service.GetAllWorkItemTypes(requestContext, processDescriptor.TypeId, bypassCache);
      List<ComposedWorkItemType> composedWorkItemTypeList = new List<ComposedWorkItemType>();
      List<ComposedWorkItemType> list;
      if (workItemTypeNames != null && workItemTypeNames.Any<string>())
      {
        list = allWorkItemTypes.Where<ComposedWorkItemType>((Func<ComposedWorkItemType, bool>) (t => workItemTypeNames.Contains<string>(t.ReferenceName, (IEqualityComparer<string>) TFStringComparer.WorkItemTypeName))).ToList<ComposedWorkItemType>();
        if (list == null || list.Count == 0)
          list = allWorkItemTypes.Where<ComposedWorkItemType>((Func<ComposedWorkItemType, bool>) (t => workItemTypeNames.Contains<string>(t.Name, (IEqualityComparer<string>) TFStringComparer.WorkItemTypeName))).ToList<ComposedWorkItemType>();
      }
      else
        list = allWorkItemTypes != null ? allWorkItemTypes.ToList<ComposedWorkItemType>() : (List<ComposedWorkItemType>) null;
      bool flag = !WorkItemTrackingFeatureFlags.IsOobPicklistFixEnabled(requestContext);
      foreach (ComposedWorkItemType composedWorkItemType in list)
      {
        List<string> other1 = (List<string>) null;
        ProcessWorkItemType processTypelet;
        if (service.TryGetTypelet<ProcessWorkItemType>(requestContext, processDescriptor.TypeId, composedWorkItemType.ReferenceName, bypassCache, out processTypelet) && processTypelet != null)
        {
          WorkItemFieldRule rule;
          if (flag)
          {
            IEnumerable<WorkItemFieldRule> fieldRules = processTypelet.FieldRules;
            rule = fieldRules != null ? fieldRules.FirstOrDefault<WorkItemFieldRule>((Func<WorkItemFieldRule, bool>) (fr => TFStringComparer.WorkItemFieldReferenceName.Equals((object) fr.FieldId, (object) fieldId))) : (WorkItemFieldRule) null;
          }
          else
          {
            IReadOnlyCollection<WorkItemFieldRule> combinedFieldRules = processTypelet.GetCombinedFieldRules(requestContext);
            rule = combinedFieldRules != null ? combinedFieldRules.FirstOrDefault<WorkItemFieldRule>((Func<WorkItemFieldRule, bool>) (fr => TFStringComparer.WorkItemFieldReferenceName.Equals((object) fr.FieldId, (object) fieldId))) : (WorkItemFieldRule) null;
          }
          IEnumerable<string> workItemFieldRule = WorkItemTrackingProcessService.GetAllowedAndSuggestedValuesFromWorkItemFieldRule((WorkItemRule) rule);
          if (workItemFieldRule != null)
          {
            other1 = workItemFieldRule.ToList<string>();
            source.UnionWith((IEnumerable<string>) other1);
          }
        }
        if (other1 == null)
        {
          string str = composedWorkItemType.IsDerived ? composedWorkItemType.ParentTypeRefName : composedWorkItemType.ReferenceName;
          IVssRequestContext requestContext1 = requestContext;
          ProcessDescriptor processDescriptor1 = processDescriptor;
          int fieldId1 = fieldId;
          List<string> workItemTypeNames1 = new List<string>();
          workItemTypeNames1.Add(str);
          IReadOnlyCollection<string> other2;
          ref IReadOnlyCollection<string> local = ref other2;
          int num = flag ? 1 : 0;
          this.GetAllowedValuesFromOOBRules(requestContext1, processDescriptor1, fieldId1, (IEnumerable<string>) workItemTypeNames1, out local, num != 0);
          if (other2 != null && other2.Count > 0)
            source.UnionWith((IEnumerable<string>) other2);
        }
      }
      allowedValues = (IReadOnlyCollection<string>) source.ToList<string>();
      return allowedValues.Count > 0;
    }

    private bool GetAllowedValuesFromOOBRules(
      IVssRequestContext requestContext,
      ProcessDescriptor processDescriptor,
      int fieldId,
      IEnumerable<string> workItemTypeNames,
      out IReadOnlyCollection<string> allowedValues,
      bool includeDefaultValues)
    {
      bool valuesFromOobRules = false;
      allowedValues = (IReadOnlyCollection<string>) new List<string>();
      HashSet<string> source = new HashSet<string>();
      IProcessWorkItemTypeService service1 = requestContext.GetService<IProcessWorkItemTypeService>();
      ITeamFoundationProcessService service2 = requestContext.GetService<ITeamFoundationProcessService>();
      WorkItemTrackingOutOfBoxRulesCache service3 = requestContext.GetService<WorkItemTrackingOutOfBoxRulesCache>();
      ProcessDescriptor systemDescriptor = processDescriptor;
      if (processDescriptor.IsDerived)
        systemDescriptor = service2.GetProcessDescriptor(requestContext, processDescriptor.Inherits);
      if (systemDescriptor != null)
      {
        IReadOnlyCollection<ComposedWorkItemType> allWorkItemTypes = service1.GetAllWorkItemTypes(requestContext, systemDescriptor.TypeId);
        List<ComposedWorkItemType> composedWorkItemTypeList = new List<ComposedWorkItemType>();
        List<ComposedWorkItemType> list;
        if (workItemTypeNames != null && workItemTypeNames.Any<string>())
        {
          list = allWorkItemTypes.Where<ComposedWorkItemType>((Func<ComposedWorkItemType, bool>) (t => workItemTypeNames.Contains<string>(t.ReferenceName, (IEqualityComparer<string>) TFStringComparer.WorkItemTypeName))).ToList<ComposedWorkItemType>();
          if (list == null || list.Count == 0)
            list = allWorkItemTypes.Where<ComposedWorkItemType>((Func<ComposedWorkItemType, bool>) (t => workItemTypeNames.Contains<string>(t.Name, (IEqualityComparer<string>) TFStringComparer.WorkItemTypeName))).ToList<ComposedWorkItemType>();
        }
        else
          list = allWorkItemTypes != null ? allWorkItemTypes.ToList<ComposedWorkItemType>() : (List<ComposedWorkItemType>) null;
        foreach (ComposedWorkItemType composedWorkItemType in list)
        {
          IReadOnlyCollection<WorkItemFieldRule> rules;
          if (service3.TryGetOutOfBoxRules(requestContext, systemDescriptor, composedWorkItemType.ReferenceName, out rules))
          {
            foreach (WorkItemRule rule in rules.Where<WorkItemFieldRule>((Func<WorkItemFieldRule, bool>) (r => r.FieldId == fieldId)))
            {
              IEnumerable<string> workItemFieldRule = WorkItemTrackingProcessService.GetAllowedAndSuggestedValuesFromWorkItemFieldRule(rule);
              if (workItemFieldRule != null)
                source.UnionWith(workItemFieldRule);
            }
          }
        }
        valuesFromOobRules = true;
      }
      if (source != null && source.Count > 0)
      {
        allowedValues = (IReadOnlyCollection<string>) source.ToList<string>();
      }
      else
      {
        IReadOnlyCollection<string> values;
        if (includeDefaultValues && OOBFieldValues.TryGetAllowedOrSuggestedValues(requestContext, fieldId, out values) && values != null)
          allowedValues = values;
      }
      if (this.IsNoAllowedValuesForActivity(requestContext, (IEnumerable<string>) allowedValues, fieldId))
      {
        string str1 = systemDescriptor != null ? systemDescriptor.TypeId.ToString() : "null";
        string str2 = systemDescriptor != null ? systemDescriptor.RowId.ToString() : "null";
        requestContext.Trace(911327, TraceLevel.Warning, nameof (WorkItemTrackingProcessService), "TryGetAllowedValues", string.Format("GetAllowedValues() for Microsoft.VSTS.Common.Activity returns nothing! ProcessId: {0}, SystemProcessTypeId: {1}, SystemProcessId: {2}, FieldId: {3}", (object) processDescriptor.TypeId, (object) str1, (object) str2, (object) fieldId));
      }
      return valuesFromOobRules;
    }

    public static bool TryGetAllowedValuesFromRules(
      IVssRequestContext requestContext,
      int fieldId,
      WorkItemFieldRule fieldRule,
      out IReadOnlyCollection<string> allowedValues)
    {
      HashSet<string> source = new HashSet<string>();
      if (fieldRule != null && fieldRule.SubRules != null)
      {
        foreach (WorkItemRule subRule in fieldRule.SubRules)
        {
          IEnumerable<string> workItemFieldRule = WorkItemTrackingProcessService.GetAllowedAndSuggestedValuesFromWorkItemFieldRule(subRule);
          if (workItemFieldRule != null)
            source.UnionWith(workItemFieldRule);
        }
      }
      allowedValues = (IReadOnlyCollection<string>) source.ToList<string>();
      if (allowedValues.Count == 0)
        OOBFieldValues.TryGetAllowedValues(requestContext, fieldId, out allowedValues);
      IReadOnlyCollection<string> strings = allowedValues;
      return strings != null && strings.Count > 0;
    }

    public void DeleteProcess(IVssRequestContext requestContext, Guid processId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(processId, nameof (processId));
      Guid id = requestContext.WitContext().RequestIdentity.Id;
      using (WorkItemTrackingMetadataComponent component = requestContext.CreateComponent<WorkItemTrackingMetadataComponent>())
        component.DeleteProcess(processId, id, false);
      requestContext.ResetMetadataDbStamps();
    }

    public bool IsInheritedStateCustomizationAllowed(
      IVssRequestContext requestContext,
      Guid processId)
    {
      ITeamFoundationProcessService service = requestContext.GetService<ITeamFoundationProcessService>();
      ProcessDescriptor processDescriptor = service.GetProcessDescriptor(requestContext, processId);
      return processDescriptor.IsDerived && service.GetProcessDescriptor(requestContext, processDescriptor.Inherits).IsSystem;
    }

    public Guid CloneHostedXmlProcessToInherited(
      IVssRequestContext requestContext,
      Guid sourceTemplateId,
      string processName,
      Guid TargetProcessType,
      string processDescription = null,
      Dictionary<string, Dictionary<string, string>> workItemTypeNameToStateNameToStateColors = null)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(sourceTemplateId, nameof (sourceTemplateId));
      if (!OutOfBoxProcessTemplateIds.IsOOBTypeProcess(TargetProcessType))
        TargetProcessType = OutOfBoxProcessTemplateIds.Agile;
      ITeamFoundationProcessService service = requestContext.GetService<ITeamFoundationProcessService>();
      ProcessDescriptor processDescriptor = service.GetProcessDescriptor(requestContext, sourceTemplateId);
      if (processDescription == null)
        processDescription = processDescriptor.Description;
      ProcessDescriptor inheritedProcess = this.CreateInheritedProcess(requestContext, processName, "Inherited." + Guid.NewGuid().ToString("N"), processDescription, TargetProcessType);
      Guid typeId = inheritedProcess.TypeId;
      try
      {
        this.MergeProcessDataForHostedXmlClone(requestContext, sourceTemplateId, typeId, TargetProcessType, workItemTypeNameToStateNameToStateColors);
        this.ValidateCompletedCategory(requestContext, typeId);
      }
      catch (Exception ex)
      {
        service.DeleteProcess(requestContext, inheritedProcess.TypeId);
        throw;
      }
      requestContext.ResetMetadataDbStamps();
      requestContext.GetService<LegacyWorkItemTrackingProcessWorkDefinitionCache>().Clear(requestContext);
      requestContext.GetService<ITeamFoundationSqlNotificationService>().SendNotification(requestContext, SqlNotificationEventClasses.LegacyProcessFieldDefinitionChanged, TeamFoundationSerializationUtility.SerializeToString<string>(""));
      return typeId;
    }

    private void ValidateCompletedCategory(IVssRequestContext requestContext, Guid clonedProcessId)
    {
      IWorkItemStateDefinitionService service = requestContext.GetService<IWorkItemStateDefinitionService>();
      foreach (ComposedWorkItemType allWorkItemType in (IEnumerable<ComposedWorkItemType>) requestContext.GetService<ProcessWorkItemTypeService>().GetAllWorkItemTypes(requestContext, clonedProcessId, true, false))
      {
        IEnumerable<WorkItemStateDefinition> source = service.GetCombinedStateDefinitions(requestContext, clonedProcessId, allWorkItemType.ReferenceName).Where<WorkItemStateDefinition>((Func<WorkItemStateDefinition, bool>) (def => def.StateCategory == WorkItemStateCategory.Completed));
        if (source.Count<WorkItemStateDefinition>() != 1)
        {
          string stateNames = "";
          if (source.Count<WorkItemStateDefinition>() > 1)
          {
            List<string> values = new List<string>();
            foreach (WorkItemStateDefinition itemStateDefinition in source)
              values.Add(itemStateDefinition.Name);
            stateNames = string.Join(", ", (IEnumerable<string>) values);
          }
          throw new CompletedCategoryNotHasOneStateException(allWorkItemType.Name, stateNames);
        }
      }
    }

    public void RestoreProcess(IVssRequestContext requestContext, Guid processTypeId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(processTypeId, nameof (processTypeId));
      ITeamFoundationProcessService service = requestContext.GetService<ITeamFoundationProcessService>();
      ProcessDescriptor restoredProcessDescriptor = (ProcessDescriptor) null;
      if (service.TryGetProcessDescriptor(requestContext, processTypeId, out restoredProcessDescriptor))
        return;
      using (ProcessTemplateComponent component = this.CreateComponent(requestContext))
      {
        ProcessTemplateDescriptorEntry entry = component.RestoreProcess(processTypeId);
        restoredProcessDescriptor = entry != null ? (ProcessDescriptor) new ProcessDescriptorImpl(entry) : (ProcessDescriptor) null;
      }
      if (restoredProcessDescriptor == null)
        throw new ProcessNotFoundByIdException(processTypeId);
      this.PublishSqlNotification(requestContext, ProcessConstants.Notifications.Reset, string.Empty);
      if (restoredProcessDescriptor.IsCustom || !service.GetProcessDescriptors(requestContext).Where<ProcessDescriptor>((Func<ProcessDescriptor, bool>) (desc => TFStringComparer.ProcessName.Equals(restoredProcessDescriptor.Name, desc.Name) && !object.Equals((object) restoredProcessDescriptor.TypeId, (object) desc.TypeId))).Any<ProcessDescriptor>())
        return;
      string uniqueProcessName = this.GenerateUniqueProcessName(restoredProcessDescriptor.Name);
      service.UpdateProcessNameAndDescription(requestContext, processTypeId, uniqueProcessName, (string) null);
    }

    internal static void CheckEditPermission(IVssRequestContext requestContext, Guid processId) => requestContext.TraceBlock(909912, 909913, "WorkItemType", nameof (WorkItemTrackingProcessService), "CheckWritePermission", (Action) (() =>
    {
      ITeamFoundationProcessService service = requestContext.GetService<ITeamFoundationProcessService>();
      ProcessDescriptor processDescriptor = service.GetProcessDescriptor(requestContext, processId);
      service.CheckProcessPermission(requestContext, processDescriptor, 1);
      if (!processDescriptor.IsDerived)
        throw new InvalidOperationException(Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.ProcessCannotBeCustomized());
    }));

    internal string GenerateUniqueProcessName(string oldName)
    {
      string str = "_" + Guid.NewGuid().ToString("N");
      oldName = oldName.Length + str.Length > ProcessConstants.MaxNameLength ? oldName.Substring(0, ProcessConstants.MaxNameLength - str.Length) : oldName;
      return oldName + str;
    }

    private bool IsNoAllowedValuesForActivity(
      IVssRequestContext requestContext,
      IEnumerable<string> values,
      int fieldId)
    {
      if (values == null || values.Count<string>() == 0)
      {
        WorkItemTrackingRequestContext trackingRequestContext = requestContext.WitContext();
        if (trackingRequestContext != null)
        {
          IFieldTypeDictionary fieldDictionary = trackingRequestContext.FieldDictionary;
          FieldEntry field;
          if (fieldDictionary != null && fieldDictionary.TryGetField(fieldId, out field) && field.ReferenceName == "Microsoft.VSTS.Common.Activity")
            return false;
        }
      }
      return true;
    }

    private Dictionary<string, string> GetWitCategoryRefNameByWitName(
      IReadOnlyCollection<WorkItemTypeCategoryDeclaration> witCategories,
      Dictionary<string, Dictionary<string, WorkItemStateCategory>> stateCategoryMappingByCategoryRefName)
    {
      Dictionary<string, string> refNameByWitName = new Dictionary<string, string>();
      foreach (WorkItemTypeCategoryDeclaration categoryDeclaration in witCategories.Where<WorkItemTypeCategoryDeclaration>((Func<WorkItemTypeCategoryDeclaration, bool>) (c => !TFStringComparer.WorkItemCategoryReferenceName.Equals(c.ReferenceName, "Microsoft.HiddenCategory"))))
      {
        Dictionary<string, WorkItemStateCategory> dictionary;
        if (stateCategoryMappingByCategoryRefName.TryGetValue(categoryDeclaration.ReferenceName, out dictionary) && dictionary.Count != 0)
        {
          foreach (string workItemTypeName in (IEnumerable<string>) categoryDeclaration.WorkItemTypeNames)
          {
            if (!refNameByWitName.ContainsKey(workItemTypeName))
              refNameByWitName.Add(workItemTypeName, categoryDeclaration.ReferenceName);
          }
        }
      }
      return refNameByWitName;
    }

    private WorkItemStateCategory ConvertStateTypeEnumToStateCategory(StateTypeEnum stateType)
    {
      switch (stateType)
      {
        case StateTypeEnum.Proposed:
          return WorkItemStateCategory.Proposed;
        case StateTypeEnum.InProgress:
          return WorkItemStateCategory.InProgress;
        case StateTypeEnum.Complete:
          return WorkItemStateCategory.Completed;
        case StateTypeEnum.Resolved:
          return WorkItemStateCategory.Resolved;
        case StateTypeEnum.Removed:
          return WorkItemStateCategory.Removed;
        default:
          throw new Exception(string.Format("Unsupported state type enum encountered: {0}", (object) stateType.ToString()));
      }
    }

    private Dictionary<string, Dictionary<string, WorkItemStateCategory>> GetStateCategoryMappingByCategoryRefName(
      Dictionary<string, string> witCategoryRefNameByWitName,
      ProcessConfigurationDeclaration processConfig)
    {
      Dictionary<string, Dictionary<string, WorkItemStateCategory>> byCategoryRefName = new Dictionary<string, Dictionary<string, WorkItemStateCategory>>((IEqualityComparer<string>) TFStringComparer.WorkItemCategoryReferenceName);
      List<CategoryConfiguration> source = new List<CategoryConfiguration>();
      if (processConfig.PortfolioBacklogs != null)
        source.AddRange((IEnumerable<CategoryConfiguration>) processConfig.PortfolioBacklogs);
      source.Add((CategoryConfiguration) processConfig.RequirementBacklog);
      source.Add((CategoryConfiguration) processConfig.TaskBacklog);
      source.Add(processConfig.FeedbackRequestWorkItems);
      source.Add(processConfig.FeedbackResponseWorkItems);
      source.Add(processConfig.FeedbackWorkItems);
      source.Add(processConfig.TestPlanWorkItems);
      source.Add(processConfig.TestSuiteWorkItems);
      source.Add(processConfig.BugWorkItems);
      source.Add(processConfig.ReleaseWorkItems);
      source.Add(processConfig.ReleaseStageWorkItems);
      source.Add(processConfig.StageSignoffTaskWorkItems);
      foreach (CategoryConfiguration categoryConfiguration in source.Where<CategoryConfiguration>((Func<CategoryConfiguration, bool>) (c => c != null)))
        byCategoryRefName.Add(categoryConfiguration.CategoryReferenceName, ((IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessComparer.Declarations.State>) categoryConfiguration.States).ToDictionary<Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessComparer.Declarations.State, string, WorkItemStateCategory>((Func<Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessComparer.Declarations.State, string>) (s => s.Value), (Func<Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessComparer.Declarations.State, WorkItemStateCategory>) (s => this.ConvertStateTypeEnumToStateCategory(s.Type)), (IEqualityComparer<string>) TFStringComparer.WorkItemStateName));
      return byCategoryRefName;
    }

    private void MergeProcessDataForHostedXmlClone(
      IVssRequestContext requestContext,
      Guid sourceTemplateId,
      Guid targetTemplateId,
      Guid OobTargetProcessType,
      Dictionary<string, Dictionary<string, string>> workItemTypeNameToStateNameToStateColors)
    {
      IProcessWorkItemTypeService service1 = requestContext.GetService<IProcessWorkItemTypeService>();
      Dictionary<string, ComposedWorkItemType> dictionary1 = service1.GetAllWorkItemTypes(requestContext, sourceTemplateId).ToDictionary<ComposedWorkItemType, string, ComposedWorkItemType>((Func<ComposedWorkItemType, string>) (w => w.Name), (Func<ComposedWorkItemType, ComposedWorkItemType>) (w => w), (IEqualityComparer<string>) TFStringComparer.WorkItemTypeName);
      Dictionary<string, ComposedWorkItemType> workItemTypesInTargetProcess = service1.GetAllWorkItemTypes(requestContext, targetTemplateId).ToDictionary<ComposedWorkItemType, string, ComposedWorkItemType>((Func<ComposedWorkItemType, string>) (w => w.Name), (Func<ComposedWorkItemType, ComposedWorkItemType>) (w => w), (IEqualityComparer<string>) TFStringComparer.WorkItemTypeName);
      List<ProcessWorkItemType> customWorkItemTypesInTarget;
      this.MergeWorkItemTypes(requestContext, sourceTemplateId, targetTemplateId, dictionary1, workItemTypesInTargetProcess, out customWorkItemTypesInTarget);
      ILegacyWorkItemTrackingProcessService service2 = requestContext.GetService<ILegacyWorkItemTrackingProcessService>();
      ProcessWorkDefinition processWorkDefinition = service2.GetProcessWorkDefinition(requestContext, sourceTemplateId);
      Action<string> logError = (Action<string>) (message =>
      {
        throw new Exception(message);
      });
      IReadOnlyCollection<WorkItemTypeCategoryDeclaration> witCategories = (IReadOnlyCollection<WorkItemTypeCategoryDeclaration>) new ReadOnlyCollection<WorkItemTypeCategoryDeclaration>((IList<WorkItemTypeCategoryDeclaration>) new List<WorkItemTypeCategoryDeclaration>());
      Dictionary<string, Dictionary<string, WorkItemStateCategory>> dictionary2 = new Dictionary<string, Dictionary<string, WorkItemStateCategory>>((IEqualityComparer<string>) TFStringComparer.WorkItemCategoryReferenceName);
      Dictionary<string, string> witCategoryRefNameByWitName = new Dictionary<string, string>((IEqualityComparer<string>) TFStringComparer.WorkItemTypeName);
      IReadOnlyCollection<WorkItemTypeDeclaration> workItemTypeDeclarations;
      ProcessConfigurationDeclaration processConfig;
      using (ZipFileXmlDocumentRepository documentRepository = new ZipFileXmlDocumentRepository(requestContext.GetService<ITeamFoundationProcessService>().GetLegacyProcessPackageContent(requestContext, sourceTemplateId)))
      {
        WorkItemTrackingProcessTemplatePackage processTemplatePackage = new WorkItemTrackingProcessTemplatePackage(new ProcessTemplatePackage((IXmlDocumentRepository) documentRepository, logError), logError);
        workItemTypeDeclarations = processTemplatePackage.ReadWorkItemTypes((Action<string>) null);
        witCategories = processTemplatePackage.ReadWorkItemTypeCategories(logError);
        processConfig = processTemplatePackage.ReadProcessConfiguration(logError);
      }
      Dictionary<string, Dictionary<string, WorkItemStateCategory>> byCategoryRefName = this.GetStateCategoryMappingByCategoryRefName(witCategoryRefNameByWitName, processConfig);
      Dictionary<string, string> refNameByWitName = this.GetWitCategoryRefNameByWitName(witCategories, byCategoryRefName);
      workItemTypesInTargetProcess = service1.GetAllWorkItemTypes(requestContext, targetTemplateId, true).Where<ComposedWorkItemType>((Func<ComposedWorkItemType, bool>) (t => (t.IsDerived || t.IsCustomType) && !t.IsDisabled)).ToDictionary<ComposedWorkItemType, string, ComposedWorkItemType>((Func<ComposedWorkItemType, string>) (w => w.Name), (Func<ComposedWorkItemType, ComposedWorkItemType>) (w => w), (IEqualityComparer<string>) TFStringComparer.WorkItemTypeName);
      Dictionary<string, ProcessWorkItemTypeDefinition> dictionary3 = processWorkDefinition.WorkItemTypeDefinitions.Where<ProcessWorkItemTypeDefinition>((Func<ProcessWorkItemTypeDefinition, bool>) (w => workItemTypesInTargetProcess.ContainsKey(w.Name))).ToDictionary<ProcessWorkItemTypeDefinition, string, ProcessWorkItemTypeDefinition>((Func<ProcessWorkItemTypeDefinition, string>) (w => w.Name), (Func<ProcessWorkItemTypeDefinition, ProcessWorkItemTypeDefinition>) (w => w), (IEqualityComparer<string>) TFStringComparer.WorkItemTypeName);
      this.MergeFields(requestContext, targetTemplateId, dictionary3, workItemTypesInTargetProcess, workItemTypeDeclarations);
      this.MergeForms(requestContext, targetTemplateId, workItemTypesInTargetProcess, dictionary1);
      Dictionary<string, IList<string>> dedupedDictionary1 = processWorkDefinition.WorkItemTypeDefinitions.ToDedupedDictionary<ProcessWorkItemTypeDefinition, string, IList<string>>((Func<ProcessWorkItemTypeDefinition, string>) (witDef => witDef.Name), (Func<ProcessWorkItemTypeDefinition, IList<string>>) (witDef => witDef.StateNames), (IEqualityComparer<string>) TFStringComparer.WorkItemTypeName);
      Dictionary<string, string> dedupedDictionary2 = processWorkDefinition.WorkItemTypeDefinitions.ToDedupedDictionary<ProcessWorkItemTypeDefinition, string, string>((Func<ProcessWorkItemTypeDefinition, string>) (witDef => witDef.Name), (Func<ProcessWorkItemTypeDefinition, string>) (witDef => witDef.InitialState), (IEqualityComparer<string>) TFStringComparer.WorkItemTypeName);
      this.MergeStates(requestContext, targetTemplateId, dedupedDictionary1, dedupedDictionary2, workItemTypesInTargetProcess, refNameByWitName, byCategoryRefName, workItemTypeNameToStateNameToStateColors);
      this.MergeBacklogLevels(requestContext, targetTemplateId, customWorkItemTypesInTarget, service2.GetProcessWorkDefinition(requestContext, OobTargetProcessType).ProcessBacklogs, processWorkDefinition.ProcessBacklogs);
    }

    private void MergeForms(
      IVssRequestContext requestContext,
      Guid targetTemplateId,
      Dictionary<string, ComposedWorkItemType> workItemTypesInTargetProcess,
      Dictionary<string, ComposedWorkItemType> workItemTypesInSourceProcess)
    {
      requestContext.GetService<IFormLayoutService>();
      requestContext.GetService<IProcessWorkItemTypeService>();
      foreach (KeyValuePair<string, ComposedWorkItemType> keyValuePair in workItemTypesInTargetProcess)
      {
        Layout inheritedLayout = keyValuePair.Value.GetFormLayoutInfo(requestContext).BaseLayout.Clone();
        Layout formLayout = workItemTypesInSourceProcess[keyValuePair.Key].GetFormLayout(requestContext);
        XmlToInheritedFormHelper.MergeForms(requestContext, targetTemplateId, keyValuePair.Value.ReferenceName, inheritedLayout, formLayout);
      }
    }

    internal void MergeFields(
      IVssRequestContext requestContext,
      Guid targetTemplateId,
      Dictionary<string, ProcessWorkItemTypeDefinition> baseWorkItemTypesInCurrentProcess,
      Dictionary<string, ComposedWorkItemType> workItemTypesInTargetProcess,
      IReadOnlyCollection<WorkItemTypeDeclaration> workItemTypeDeclarations)
    {
      IProcessWorkItemTypeService service1 = requestContext.GetService<IProcessWorkItemTypeService>();
      WorkItemTrackingFieldService service2 = requestContext.GetService<WorkItemTrackingFieldService>();
      ProcessFieldService service3 = requestContext.GetService<ProcessFieldService>();
      IEnumerable<ProcessFieldDefinition> allFieldDefinitions = baseWorkItemTypesInCurrentProcess.SelectMany<KeyValuePair<string, ProcessWorkItemTypeDefinition>, ProcessFieldDefinition>((Func<KeyValuePair<string, ProcessWorkItemTypeDefinition>, IEnumerable<ProcessFieldDefinition>>) (bwit => (IEnumerable<ProcessFieldDefinition>) bwit.Value.FieldDefinitions));
      IReadOnlyCollection<ProcessFieldDefinition> allOobFields = service3.GetAllOutOfBoxFieldDefinitions(requestContext);
      IVssRequestContext requestContext1 = requestContext;
      bool? checkFreshness = new bool?();
      List<FieldEntry> list = service2.GetAllFields(requestContext1, checkFreshness).Where<FieldEntry>((Func<FieldEntry, bool>) (fieldEntry => !allOobFields.Any<ProcessFieldDefinition>((Func<ProcessFieldDefinition, bool>) (oobField => TFStringComparer.WorkItemFieldReferenceName.Equals(oobField.ReferenceName, fieldEntry.ReferenceName))) && allFieldDefinitions.Any<ProcessFieldDefinition>((Func<ProcessFieldDefinition, bool>) (fieldDefinition => TFStringComparer.WorkItemFieldReferenceName.Equals(fieldEntry.ReferenceName, fieldDefinition.ReferenceName))))).ToList<FieldEntry>();
      IDictionary<int, IEnumerable<string>> dictionary = service3.FetchRulesListValuesInternal(requestContext, list.Select<FieldEntry, int>((Func<FieldEntry, int>) (fe => fe.FieldId)).ToArray<int>(), true);
      foreach (KeyValuePair<string, ProcessWorkItemTypeDefinition> keyValuePair in baseWorkItemTypesInCurrentProcess)
      {
        KeyValuePair<string, ProcessWorkItemTypeDefinition> workItemType = keyValuePair;
        WorkItemTypeDeclaration itemTypeDeclaration = workItemTypeDeclarations.First<WorkItemTypeDeclaration>((Func<WorkItemTypeDeclaration, bool>) (wit => wit.ReferenceName.Equals(workItemType.Value.ReferenceName)));
        ComposedWorkItemType composedWorkItemType = workItemTypesInTargetProcess[workItemType.Value.Name];
        foreach (ProcessFieldDefinition fieldDefinition in (IEnumerable<ProcessFieldDefinition>) workItemType.Value.FieldDefinitions)
        {
          ProcessFieldDefinition field = fieldDefinition;
          WorkItemFieldDeclaration fieldDeclaration = itemTypeDeclaration.Fields.First<WorkItemFieldDeclaration>((Func<WorkItemFieldDeclaration, bool>) (fe => fe.ReferenceName.Equals(field.ReferenceName)));
          if (!field.ReferenceName.StartsWith(ProcessFieldService.SystemFieldPrefix))
          {
            WorkItemTypeletFieldRuleProperties fieldProps = (WorkItemTypeletFieldRuleProperties) null;
            IReadOnlyCollection<FieldEntry> source = composedWorkItemType.IsDerived ? composedWorkItemType.DerivedWorkItemType.GetCombinedFields(requestContext) : composedWorkItemType.CustomWorkItemType.GetCombinedFields(requestContext);
            if (source == null || !source.Any<FieldEntry>((Func<FieldEntry, bool>) (f => TFStringComparer.WorkItemFieldReferenceName.Equals(f.ReferenceName, field.ReferenceName))))
            {
              bool flag1 = field.ReferenceName.StartsWith(ProcessFieldService.SystemFieldPrefix) || allOobFields.Any<ProcessFieldDefinition>((Func<ProcessFieldDefinition, bool>) (f => string.Equals(f.ReferenceName, field.ReferenceName, StringComparison.OrdinalIgnoreCase)));
              if (field.Type == InternalFieldType.Boolean && !flag1)
              {
                field.IsRequired = true;
                field.DefaultValue = "false";
                fieldProps = new WorkItemTypeletFieldRuleProperties(field.IsRequired, field.IsReadOnly, field.DefaultValue, RuleValueFrom.Value, new bool?(false), (string[]) null);
              }
              FieldEntry fieldEntry = list.Find((Predicate<FieldEntry>) (fe => TFStringComparer.WorkItemFieldReferenceName.Equals(fe.ReferenceName, field.ReferenceName)));
              if (fieldEntry != null && !flag1)
              {
                if (field.IsIdentity || fieldEntry.IsIdentity)
                  service3.UpdateField(requestContext, field.ReferenceName, fieldEntry.Description, false, new bool?(true), false);
                else if ((field.Type == InternalFieldType.String || field.Type == InternalFieldType.Integer) && dictionary.ContainsKey(fieldEntry.FieldId))
                {
                  bool flag2 = false;
                  if (fieldDeclaration.Rules.Children.Any<WorkItemRuleDeclaration>((Func<WorkItemRuleDeclaration, bool>) (rule => rule is SuggestedValuesRuleDeclaration)) && !fieldDeclaration.Rules.Children.Any<WorkItemRuleDeclaration>((Func<WorkItemRuleDeclaration, bool>) (rule => rule is AllowedValuesRuleDeclaration)))
                    flag2 = true;
                  try
                  {
                    ProcessFieldService processFieldService = service3;
                    IVssRequestContext requestContext2 = requestContext;
                    string referenceName = field.ReferenceName;
                    bool flag3 = flag2;
                    bool? isIdentityFromProcess = new bool?();
                    int num = flag3 ? 1 : 0;
                    processFieldService.UpdateField(requestContext2, referenceName, "", true, isIdentityFromProcess, num != 0);
                  }
                  catch (Exception ex)
                  {
                    requestContext.TraceException(917001, "CloneHostedXmlProcessToInherited", nameof (MergeFields), ex);
                    requestContext.Trace(917001, TraceLevel.Error, "CloneHostedXmlProcessToInherited", nameof (MergeFields), "Failed to create picklist for field named " + fieldEntry.Name);
                    throw new WorkItemTrackingPicklistConversionFailedBadValuesException(fieldEntry.ReferenceName);
                  }
                }
              }
              try
              {
                service1.AddOrUpdateWorkItemTypeField(requestContext, targetTemplateId, composedWorkItemType.ReferenceName, field.ReferenceName, fieldProps, true);
              }
              catch (WorkItemTrackingFieldDefinitionNotFoundException ex)
              {
                throw new XmlToInheritedFieldDefinitionNotFoundException(field.Name);
              }
            }
          }
        }
      }
    }

    internal void MergeWorkItemTypes(
      IVssRequestContext requestContext,
      Guid sourceTemplateId,
      Guid targetTemplateId,
      Dictionary<string, ComposedWorkItemType> workItemTypesInSourceProcess,
      Dictionary<string, ComposedWorkItemType> workItemTypesInTargetProcess,
      out List<ProcessWorkItemType> customWorkItemTypesInTarget)
    {
      IProcessWorkItemTypeService service = requestContext.GetService<IProcessWorkItemTypeService>();
      customWorkItemTypesInTarget = new List<ProcessWorkItemType>();
      List<ComposedWorkItemType> composedWorkItemTypeList1 = new List<ComposedWorkItemType>();
      List<ComposedWorkItemType> composedWorkItemTypeList2 = new List<ComposedWorkItemType>();
      List<ComposedWorkItemType> composedWorkItemTypeList3 = new List<ComposedWorkItemType>();
      foreach (KeyValuePair<string, ComposedWorkItemType> keyValuePair in workItemTypesInSourceProcess)
      {
        if (!workItemTypesInTargetProcess.ContainsKey(keyValuePair.Key))
          composedWorkItemTypeList2.Add(keyValuePair.Value);
        else
          composedWorkItemTypeList1.Add(keyValuePair.Value);
      }
      foreach (KeyValuePair<string, ComposedWorkItemType> keyValuePair in workItemTypesInTargetProcess)
      {
        if (!workItemTypesInSourceProcess.ContainsKey(keyValuePair.Key))
          composedWorkItemTypeList3.Add(keyValuePair.Value);
      }
      foreach (ComposedWorkItemType composedWorkItemType1 in composedWorkItemTypeList1)
      {
        ComposedWorkItemType composedWorkItemType2 = workItemTypesInTargetProcess[composedWorkItemType1.Name];
        if (!composedWorkItemType2.IsDerived && !((IEnumerable<string>) Customization.WorkItemTypesBlockedFromCustomization).Contains<string>(composedWorkItemType2.ReferenceName, (IEqualityComparer<string>) TFStringComparer.WorkItemTypeReferenceName) && !composedWorkItemType2.IsCustomType)
          service.CreateDerivedWorkItemType(requestContext, targetTemplateId, composedWorkItemType2.ReferenceName, TruncateDescription(composedWorkItemType1.Description), composedWorkItemType1.Color ?? "009CCC", composedWorkItemType1.Icon);
      }
      foreach (ComposedWorkItemType composedWorkItemType in composedWorkItemTypeList2)
      {
        ProcessWorkItemType workItemType = service.CreateWorkItemType(requestContext, targetTemplateId, composedWorkItemType.Name, composedWorkItemType.Color ?? "009CCC", composedWorkItemType.Icon, TruncateDescription(composedWorkItemType.Description));
        customWorkItemTypesInTarget.Add(workItemType);
      }
      foreach (ComposedWorkItemType composedWorkItemType in composedWorkItemTypeList3)
      {
        if (!((IEnumerable<string>) Customization.TcmWorkItemTypesBlockedFromDisable).Contains<string>(composedWorkItemType.ReferenceName))
          service.CreateDerivedWorkItemType(requestContext, targetTemplateId, composedWorkItemType.ReferenceName, TruncateDescription(composedWorkItemType.Description), composedWorkItemType.Color ?? "009CCC", composedWorkItemType.Icon, new bool?(true));
      }

      static string TruncateDescription(string description) => description == null || description.Length <= 256 ? description : description.Substring(0, 256);
    }

    internal void MergeStates(
      IVssRequestContext requestContext,
      Guid targetTemplateId,
      Dictionary<string, IList<string>> witNameToStateNames,
      Dictionary<string, string> witNameToInitialState,
      Dictionary<string, ComposedWorkItemType> workItemTypes,
      Dictionary<string, string> witNameToCategoryRefName,
      Dictionary<string, Dictionary<string, WorkItemStateCategory>> stateCategoryMappingByCategoryRefName,
      Dictionary<string, Dictionary<string, string>> workItemTypeNameToStateNameToStateColors)
    {
      IWorkItemStateDefinitionService service = requestContext.GetService<IWorkItemStateDefinitionService>();
      foreach (KeyValuePair<string, ComposedWorkItemType> workItemType in workItemTypes)
      {
        string key = (string) null;
        witNameToCategoryRefName.TryGetValue(workItemType.Value.Name, out key);
        IList<string> witNameToStateName = witNameToStateNames[workItemType.Key];
        string initialState = witNameToInitialState[workItemType.Key];
        Dictionary<string, string> phase1StatesDictionary = witNameToStateName.ToDictionary<string, string>((Func<string, string>) (x => x), (IEqualityComparer<string>) TFStringComparer.WorkItemStateName);
        ComposedWorkItemType composedWorkItemType = workItemType.Value;
        Dictionary<string, WorkItemStateDefinition> phase2StatesDictionary = new Dictionary<string, WorkItemStateDefinition>((IEqualityComparer<string>) TFStringComparer.WorkItemStateName);
        phase2StatesDictionary = !composedWorkItemType.IsCustomType ? composedWorkItemType.ParentWorkItemType.States.ToDictionary<WorkItemStateDefinition, string>((Func<WorkItemStateDefinition, string>) (x => x.Name), (IEqualityComparer<string>) TFStringComparer.WorkItemStateName) : composedWorkItemType.CustomWorkItemType.GetStates(requestContext).ToDictionary<WorkItemStateDefinition, string>((Func<WorkItemStateDefinition, string>) (x => x.Name), (IEqualityComparer<string>) TFStringComparer.WorkItemStateName);
        IEnumerable<string> source1 = witNameToStateName.Where<string>((Func<string, bool>) (s => !phase2StatesDictionary.ContainsKey(s)));
        if (source1.Contains<string>(initialState))
          source1 = ((IEnumerable<string>) new string[1]
          {
            initialState
          }).Concat<string>(source1.Where<string>((Func<string, bool>) (state => !TFStringComparer.WorkItemStateName.Equals(state, initialState))));
        IEnumerable<WorkItemStateDefinition> source2 = phase2StatesDictionary.Values.Where<WorkItemStateDefinition>((Func<WorkItemStateDefinition, bool>) (s => !phase1StatesDictionary.ContainsKey(s.Name)));
        Dictionary<string, string> dictionary1 = (Dictionary<string, string>) null;
        if (workItemTypeNameToStateNameToStateColors == null || !workItemTypeNameToStateNameToStateColors.TryGetValue(workItemType.Key, out dictionary1))
          dictionary1 = new Dictionary<string, string>();
        Dictionary<string, WorkItemStateCategory> dictionary2;
        if (key == null || !stateCategoryMappingByCategoryRefName.TryGetValue(key, out dictionary2))
        {
          dictionary2 = new Dictionary<string, WorkItemStateCategory>();
          if (witNameToStateName.Count > 0)
          {
            if (source1.Contains<string>(initialState))
              dictionary2[initialState] = WorkItemStateCategory.Proposed;
            else
              dictionary2[witNameToStateName.First<string>()] = WorkItemStateCategory.Proposed;
            bool flag = source2.Any<WorkItemStateDefinition>((Func<WorkItemStateDefinition, bool>) (stateDef => stateDef.StateCategory.Equals((object) WorkItemStateCategory.Completed)));
            if (witNameToStateName.Count > 1 & flag)
              dictionary2[witNameToStateName.Last<string>()] = WorkItemStateCategory.Completed;
          }
        }
        foreach (string str1 in source1)
        {
          CommonWITUtils.CheckValidName(str1, 128, WorkItemTypeMetadata.IllegalStateNameChars);
          WorkItemStateCategory itemStateCategory = WorkItemStateCategory.InProgress;
          if (!dictionary2.TryGetValue(str1, out itemStateCategory))
            itemStateCategory = WorkItemStateCategory.Removed;
          string str2 = "";
          if (!dictionary1.TryGetValue(str1, out str2))
            str2 = "FFFFFF";
          service.CreateStateDefinition(requestContext, targetTemplateId, composedWorkItemType.ReferenceName, new WorkItemStateDeclaration()
          {
            Name = str1,
            WorkItemTypeReferenceName = composedWorkItemType.ReferenceName,
            StateCategory = itemStateCategory,
            Color = str2
          }, true);
        }
        foreach (WorkItemStateDefinition itemStateDefinition in source2)
        {
          try
          {
            if (composedWorkItemType.IsCustomType)
              service.DeleteStateDefinition(requestContext, targetTemplateId, composedWorkItemType.ReferenceName, itemStateDefinition.Id, true);
            else
              service.HideStateDefinition(requestContext, targetTemplateId, composedWorkItemType.ReferenceName, itemStateDefinition.Id, true);
          }
          catch (WorkItemTypeTwoStateRestrictionException ex)
          {
            requestContext.TraceException(917000, "CloneHostedXmlProcessToInherited", nameof (MergeStates), (Exception) ex);
          }
        }
      }
    }

    internal void MergeBacklogLevels(
      IVssRequestContext requestContext,
      Guid targetInheritedProcessTemplateId,
      List<ProcessWorkItemType> customWorkItemTypesInTarget,
      ProcessBacklogs inheritedParentProcessBacklog,
      ProcessBacklogs sourceXMLBacklogs)
    {
      IProcessWorkItemTypeService workItemTypeService = requestContext.GetService<IProcessWorkItemTypeService>();
      IBehaviorService service = requestContext.GetService<IBehaviorService>();
      List<Behavior> list1 = service.GetBehaviors(requestContext, targetInheritedProcessTemplateId).ToList<Behavior>();
      requestContext.GetService<ILegacyWorkItemTrackingProcessService>();
      Action<IReadOnlyCollection<string>, Behavior> action = (Action<IReadOnlyCollection<string>, Behavior>) ((workItemTypeNames, targetBehavior) =>
      {
        foreach (string workItemTypeName1 in (IEnumerable<string>) workItemTypeNames)
        {
          string workItemTypeName = workItemTypeName1;
          ProcessWorkItemType processWorkItemType = customWorkItemTypesInTarget.Find((Predicate<ProcessWorkItemType>) (missingWit => TFStringComparer.WorkItemTypeName.Equals(missingWit.Name, workItemTypeName)));
          if (processWorkItemType != null)
            workItemTypeService.AddBehaviorToWorkItemType(requestContext, targetInheritedProcessTemplateId, processWorkItemType.ReferenceName, targetBehavior.ReferenceName);
        }
      });
      WorkItemTrackingProcessService.CheckPluralName(sourceXMLBacklogs.TaskBacklog.PluralName, "TaskBacklog");
      Behavior behavior1 = list1.Find((Predicate<Behavior>) (b => b.Rank == inheritedParentProcessBacklog.TaskBacklog.Rank));
      service.UpdateBehavior(requestContext, targetInheritedProcessTemplateId, behavior1.ReferenceName, sourceXMLBacklogs.TaskBacklog.PluralName);
      action(sourceXMLBacklogs.TaskBacklog.WorkItemTypesInCategory, behavior1);
      WorkItemTrackingProcessService.CheckPluralName(sourceXMLBacklogs.RequirementBacklog.PluralName, "RequirementBacklog");
      Behavior behavior2 = list1.Find((Predicate<Behavior>) (b => b.Rank == inheritedParentProcessBacklog.RequirementBacklog.Rank));
      service.UpdateBehavior(requestContext, targetInheritedProcessTemplateId, behavior2.ReferenceName, sourceXMLBacklogs.RequirementBacklog.PluralName);
      action(sourceXMLBacklogs.RequirementBacklog.WorkItemTypesInCategory, behavior2);
      Dictionary<string, Behavior> dictionary = new Dictionary<string, Behavior>();
      foreach (Behavior behavior3 in list1)
        dictionary.Add(behavior3.Name, behavior3);
      List<ProcessBacklogDefinition> list2 = sourceXMLBacklogs.PortfolioBacklogs.ToList<ProcessBacklogDefinition>();
      list2.Sort((Comparison<ProcessBacklogDefinition>) ((x, y) => x.Rank.CompareTo(y.Rank)));
      foreach (ProcessBacklogDefinition backlogDefinition in list2)
      {
        ProcessBacklogDefinition legacyPortfolioBacklog = backlogDefinition;
        WorkItemTrackingProcessService.CheckPluralName(legacyPortfolioBacklog.PluralName, "PortfolioBacklog");
        ProcessBacklogDefinition matchingOobPortfolioBacklog = inheritedParentProcessBacklog.PortfolioBacklogs.ToList<ProcessBacklogDefinition>().Find((Predicate<ProcessBacklogDefinition>) (oobPortfolioBacklog => TFStringComparer.WorkItemCategoryReferenceName.Equals(oobPortfolioBacklog.CategoryReferenceName, legacyPortfolioBacklog.CategoryReferenceName)));
        Behavior behavior4 = (Behavior) null;
        dictionary.TryGetValue(legacyPortfolioBacklog.PluralName, out behavior4);
        Behavior behavior5;
        if (matchingOobPortfolioBacklog != null)
        {
          if (behavior4 != null && !TFStringComparer.BehaviorName.Equals(matchingOobPortfolioBacklog.PluralName, behavior4.Name))
            throw new OOBBehaviorNamePluralNameMismatch(legacyPortfolioBacklog.CategoryReferenceName, matchingOobPortfolioBacklog.PluralName, behavior4.Name);
          behavior5 = list1.Find((Predicate<Behavior>) (b => b.Rank == matchingOobPortfolioBacklog.Rank));
          service.UpdateBehavior(requestContext, targetInheritedProcessTemplateId, behavior5.ReferenceName, legacyPortfolioBacklog.PluralName);
        }
        else
        {
          if (inheritedParentProcessBacklog.PortfolioBacklogs.ToList<ProcessBacklogDefinition>().Any<ProcessBacklogDefinition>((Func<ProcessBacklogDefinition, bool>) (oobPortfolioBacklog => oobPortfolioBacklog.PluralName.Equals(legacyPortfolioBacklog.PluralName))))
            throw new SystemCategoryRefnameHasBeenChangedInXmlProcessException(legacyPortfolioBacklog.PluralName);
          string color = customWorkItemTypesInTarget.Find((Predicate<ProcessWorkItemType>) (wit => TFStringComparer.WorkItemTypeName.Equals(wit.Name, legacyPortfolioBacklog.DefaultWorkItemTypeName)))?.Color;
          behavior5 = service.CreateBehavior(requestContext, targetInheritedProcessTemplateId, BehaviorService.PortfolioBehaviorReferenceName, legacyPortfolioBacklog.PluralName, color, legacyPortfolioBacklog.CategoryReferenceName);
        }
        action(legacyPortfolioBacklog.WorkItemTypesInCategory, behavior5);
      }
    }

    public static IEnumerable<string> GetAllowedAndSuggestedValuesFromWorkItemFieldRule(
      WorkItemRule rule)
    {
      HashSet<string> stringSet = new HashSet<string>();
      bool flag = false;
      Queue<WorkItemRule> queue = new Queue<WorkItemRule>();
      queue.Enqueue(rule);
      while (queue.Count > 0)
      {
        WorkItemRule workItemRule = queue.Dequeue();
        switch (workItemRule)
        {
          case AllowedValuesRule _:
            flag = true;
            stringSet.UnionWith((IEnumerable<string>) (workItemRule as AllowedValuesRule).Values);
            continue;
          case SuggestedValuesRule _:
            flag = true;
            stringSet.UnionWith((IEnumerable<string>) (workItemRule as SuggestedValuesRule).Values);
            continue;
          case RuleBlock _:
            ((IEnumerable<WorkItemRule>) (workItemRule as RuleBlock).SubRules).ForEach<WorkItemRule>((Action<WorkItemRule>) (subRule => queue.Enqueue(subRule)));
            continue;
          default:
            continue;
        }
      }
      return flag ? (IEnumerable<string>) stringSet : (IEnumerable<string>) null;
    }

    private string GenerateReferenceName(Guid processGuid)
    {
      string referenceName = "Inherited." + processGuid.ToString("N");
      WorkItemTrackingProcessService.CheckValidProcessReferenceName(referenceName);
      return referenceName;
    }

    protected virtual Guid GetUserIdentityIdInternal(IVssRequestContext requestContext) => requestContext.GetUserId();

    private int GetProcessCountLimit(IVssRequestContext requestContext) => (requestContext.IsFeatureEnabled("WebAccess.Process.Hierarchy") ? 1 : (requestContext.IsFeatureEnabled("WebAccess.Process.ProcessUpload") ? 1 : 0)) == 0 || requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment) ? int.MaxValue : requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) "/Service/Process/Settings/ProcessLimit", 128);

    private void CheckDeploymentLevelNameCollisions(
      IVssRequestContext requestContext,
      string name,
      string referenceName)
    {
      if (!requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        return;
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      IReadOnlyCollection<ProcessDescriptor> processDescriptors = vssRequestContext.GetService<ITeamFoundationProcessService>().GetProcessDescriptors(vssRequestContext, false);
      if (!string.IsNullOrEmpty(name) && processDescriptors.Any<ProcessDescriptor>((Func<ProcessDescriptor, bool>) (t => TFStringComparer.ProcessName.Equals(t.Name, name))))
        throw new ProcessNameConflictException(name);
      if (!string.IsNullOrEmpty(referenceName) && processDescriptors.Any<ProcessDescriptor>((Func<ProcessDescriptor, bool>) (t => TFStringComparer.ProcessName.Equals(t.ReferenceName, referenceName))))
        throw new ProcessNameConflictException(referenceName);
    }

    private ProcessTemplateComponent CreateComponent(IVssRequestContext requestContext)
    {
      ProcessTemplateComponent component = requestContext.CreateComponent<ProcessTemplateComponent>();
      component.NotificationAuthor = this.m_notificationAuthor;
      return component;
    }

    private string GetProcessTemplateTypeNameForCI(ProcessDescriptor process)
    {
      if (process.IsCustom)
        return "Legacy";
      if (process.IsDerived)
        return "Child";
      return !process.IsSystem ? "Other" : "OOB";
    }

    private void PublishSqlNotification(
      IVssRequestContext requestContext,
      Guid eventId,
      string eventData)
    {
      requestContext.GetService<ITeamFoundationSqlNotificationService>().SendNotification(requestContext, eventId, eventData);
    }

    private static void CheckValidProcessReferenceName(string referenceName)
    {
      if (string.IsNullOrEmpty(referenceName) || referenceName.Length > ProcessConstants.MaxReferenceNameLength || referenceName.IndexOfAny(ProcessConstants.IllegalProcessRefNameChars) != -1 || referenceName.StartsWith("system.", StringComparison.OrdinalIgnoreCase))
      {
        string illegalChars = string.Join<char>("", (IEnumerable<char>) ProcessConstants.IllegalProcessRefNameChars);
        throw new ProcessReferenceNameInvalidException(ProcessConstants.MaxReferenceNameLength, illegalChars);
      }
    }

    private void CheckMaxProcessLimitRespected(
      IVssRequestContext requestContext,
      int numberOfNewProcesses = 1)
    {
      if (this.GetProcessCountLimit(requestContext) < requestContext.GetService<ITeamFoundationProcessService>().GetProcessDescriptors(requestContext, false).Count + numberOfNewProcesses)
        throw new ProcessLimitExceededException();
    }

    private static void CheckDescriptionText(string description)
    {
      if (description != null && description.Length > ProcessConstants.MaxDescriptionLength)
        throw new ProcessDescriptionLengthExceededException(description.Length);
    }

    private static void CheckPluralName(string pluralName, string typeBacklog)
    {
      if (string.IsNullOrWhiteSpace(pluralName))
      {
        string str = Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.Validation_MissingAttributeValue((object) "PluralName");
        throw new VssPropertyValidationException("PluralName", Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.Validation_ElementError((object) typeBacklog, (object) str));
      }
    }
  }
}
