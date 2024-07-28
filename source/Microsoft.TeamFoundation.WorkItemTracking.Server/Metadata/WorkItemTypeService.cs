// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemTypeService
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.Azure.Boards.CssNodes;
using Microsoft.Azure.Boards.ProcessTemplates;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Events;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.AdminPermissionService;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Process;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata
{
  public class WorkItemTypeService : 
    BaseTeamFoundationWorkItemTrackingService,
    IWorkItemTypeService,
    IVssFrameworkService
  {
    private readonly MetadataTable[] s_stampsNonIdentity = new MetadataTable[7]
    {
      MetadataTable.Hierarchy,
      MetadataTable.HierarchyProperties,
      MetadataTable.Rules,
      MetadataTable.WorkItemTypes,
      MetadataTable.WorkItemTypeUsages,
      MetadataTable.ConstantsNonIdentity,
      MetadataTable.ConstantSetsNonIdentity
    };

    protected override void ServiceStart(IVssRequestContext systemRequestContext)
    {
      if (!systemRequestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        throw new UnexpectedHostTypeException(systemRequestContext.ServiceHost.HostType);
      systemRequestContext.GetService<TeamFoundationSqlNotificationService>().RegisterNotification(systemRequestContext, "Default", SqlNotificationEventClasses.ProjectsProcessMigrated, new SqlNotificationCallback(this.OnMigrateProjectsProcess), false);
      base.ServiceStart(systemRequestContext);
    }

    protected override void ServiceEnd(IVssRequestContext requestContext) => requestContext.GetService<TeamFoundationSqlNotificationService>().UnregisterNotification(requestContext, "Default", SqlNotificationEventClasses.ProjectsProcessMigrated, new SqlNotificationCallback(this.OnMigrateProjectsProcess), false);

    internal void OnMigrateProjectsProcess(
      IVssRequestContext requestContext,
      Guid eventClass,
      string eventData)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      WorkItemTypeCacheService service = requestContext.GetService<WorkItemTypeCacheService>();
      foreach (string input in (IEnumerable<string>) (eventData ?? string.Empty).Split(new char[1]
      {
        ';'
      }, StringSplitOptions.RemoveEmptyEntries))
      {
        Guid empty = Guid.Empty;
        ref Guid local = ref empty;
        if (Guid.TryParse(input, out local))
          service.RemoveProjectWorkItemTypes(requestContext, empty);
      }
    }

    public void RemoveProjectWorkitemTypesFromCache(
      IVssRequestContext requestContext,
      IEnumerable<ProjectInfo> projectInfos)
    {
      WorkItemTypeCacheService service = requestContext.GetService<WorkItemTypeCacheService>();
      foreach (ProjectInfo projectInfo in projectInfos)
        service.RemoveProjectWorkItemTypes(requestContext, projectInfo.Id);
    }

    public virtual IReadOnlyCollection<WorkItemType> GetWorkItemTypes(
      IVssRequestContext requestContext,
      Guid projectId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      IWITProcessReadPermissionCheckHelper permissionChecker = requestContext.WitContext().ProcessReadPermissionChecker;
      ProcessReadSecuredObject securedObject = (ProcessReadSecuredObject) null;
      Guid projectId1 = projectId;
      ref ProcessReadSecuredObject local = ref securedObject;
      if (!permissionChecker.HasProcessReadPermissionForProject(projectId1, out local))
        throw new ProjectWorkItemTypeNotFoundException(projectId);
      IReadOnlyCollection<WorkItemType> workItemTypes = this.GetWorkItemTypes(requestContext, (IEnumerable<Guid>) new Guid[1]
      {
        projectId
      });
      foreach (ProcessReadSecuredObject readSecuredObject in (IEnumerable<WorkItemType>) workItemTypes)
        readSecuredObject.SetSecuredObjectProperties(securedObject);
      return workItemTypes;
    }

    public virtual IReadOnlyCollection<WorkItemType> GetWorkItemTypes(
      IVssRequestContext requestContext,
      IEnumerable<Guid> projectIds)
    {
      return (IReadOnlyCollection<WorkItemType>) requestContext.TraceBlock<WorkItemType[]>(904682, 904689, 904686, "Services", "WorkItemService", nameof (GetWorkItemTypes), (Func<WorkItemType[]>) (() => this.GetProjectWorkItemTypes(requestContext, projectIds).SelectMany<ProjectWorkItemTypes, WorkItemType>((Func<ProjectWorkItemTypes, IEnumerable<WorkItemType>>) (pwits => pwits.WorkItemTypes.Where<WorkItemType>((Func<WorkItemType, bool>) (type =>
      {
        if (!type.Id.HasValue)
          return true;
        int? id = type.Id;
        int num = 0;
        return id.GetValueOrDefault() > num & id.HasValue;
      })))).ToArray<WorkItemType>()));
    }

    public WorkItemType GetWorkItemTypeByReferenceName(
      IVssRequestContext requestContext,
      Guid projectId,
      string workItemTypeName)
    {
      return this.GetWorkItemTypeByReferenceNameInternal(requestContext, projectId, workItemTypeName);
    }

    protected virtual WorkItemType GetWorkItemTypeByReferenceNameInternal(
      IVssRequestContext requestContext,
      Guid projectId,
      string workItemTypeName)
    {
      return requestContext.TraceBlock<WorkItemType>(904681, 904690, 904685, "Services", "WorkItemService", "GetWorkItemType.GetWorkItemTypeByReferenceNameInternal", (Func<WorkItemType>) (() =>
      {
        WorkItemType workItemType;
        if (!this.TryGetWorkItemTypeByReferenceName(requestContext, projectId, workItemTypeName, out workItemType))
          throw new WorkItemTypeNotFoundException(projectId, workItemTypeName);
        ProcessReadSecuredObject processReadSecuredObject = (ProcessReadSecuredObject) null;
        if (!requestContext.WitContext().ProcessReadPermissionChecker.HasProcessReadPermissionForProject(projectId, out processReadSecuredObject))
          throw new WorkItemTypeNotFoundException(projectId, workItemTypeName);
        workItemType.SetSecuredObjectProperties(processReadSecuredObject);
        return workItemType;
      }));
    }

    public bool TryGetWorkItemTypeByReferenceName(
      IVssRequestContext requestContext,
      Guid projectId,
      string workItemTypeName,
      out WorkItemType workItemType)
    {
      ProjectWorkItemTypes projectWorkItemTypes = this.GetProjectWorkItemTypes(requestContext, projectId);
      return projectWorkItemTypes.WorkItemTypesByName.TryGetValue(workItemTypeName, out workItemType) || projectWorkItemTypes.WorkItemTypesByReferenceName.TryGetValue(workItemTypeName, out workItemType);
    }

    public virtual bool TryGetWorkItemTypeByName(
      IVssRequestContext requestContext,
      Guid projectId,
      string workItemTypeName,
      out WorkItemType workItemType)
    {
      return this.GetProjectWorkItemTypes(requestContext, projectId).WorkItemTypesByName.TryGetValue(workItemTypeName, out workItemType);
    }

    public WorkItemType GetWorkItemTypeById(
      IVssRequestContext requestContext,
      Guid projectId,
      int typeId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      return requestContext.TraceBlock<WorkItemType>(904781, 904790, 904785, "Services", "WorkItemService", "GetWorkItemType.GetWorkItemTypeById", (Func<WorkItemType>) (() =>
      {
        WorkItemType workItemType;
        if (!this.TryGetWorkItemTypeById(requestContext, projectId, typeId, out workItemType))
          throw new WorkItemTypeNotFoundException(projectId, typeId.ToString());
        return workItemType;
      }));
    }

    public virtual bool TryGetWorkItemTypeById(
      IVssRequestContext requestContext,
      Guid projectId,
      int typeId,
      out WorkItemType workItemType)
    {
      return this.GetProjectWorkItemTypes(requestContext, projectId).WorkItemTypesById.TryGetValue(typeId, out workItemType);
    }

    public WorkItemType RenameWorkItemType(
      IVssRequestContext requestContext,
      Guid projectId,
      string oldWorkItemTypeName,
      string newWorkItemTypeName)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<string>(oldWorkItemTypeName, nameof (oldWorkItemTypeName));
      ArgumentUtility.CheckForNull<string>(newWorkItemTypeName, nameof (newWorkItemTypeName));
      if (newWorkItemTypeName.Length <= 0 || newWorkItemTypeName.Length > 256)
        throw new WorkItemTypeReferenceNameInvalidLengthException(projectId, newWorkItemTypeName);
      ProjectInfo project = requestContext.GetService<IProjectService>().GetProject(requestContext.Elevate(), projectId);
      requestContext.GetService<ITeamFoundationProjectAdminPermissionService>().CheckAdminPermissions(requestContext, projectId);
      WorkItemType typeByReferenceName = this.GetWorkItemTypeByReferenceName(requestContext, projectId, oldWorkItemTypeName);
      WorkItemType workItemType;
      if (newWorkItemTypeName == oldWorkItemTypeName)
      {
        workItemType = typeByReferenceName;
      }
      else
      {
        Guid id = requestContext.WitContext().RequestIdentity.Id;
        WorkItemTypeEntry workItemTypeEntry;
        using (WorkItemTrackingMetadataComponent component = requestContext.CreateComponent<WorkItemTrackingMetadataComponent>())
          workItemTypeEntry = component.UpdateWorkItemTypeName(projectId, id, oldWorkItemTypeName, newWorkItemTypeName);
        requestContext.ResetMetadataDbStamps();
        workItemType = new WorkItemType(workItemTypeEntry, projectId);
        requestContext.GetService<WorkItemTypeExtensionService>().ReconcileExtensions(requestContext, (IEnumerable<WorkItemTypeRenameEventData>) new WorkItemTypeRenameEventData[1]
        {
          new WorkItemTypeRenameEventData()
          {
            NewName = newWorkItemTypeName,
            OldName = oldWorkItemTypeName,
            ProjectName = project.Name
          }
        });
      }
      return workItemType;
    }

    public void CreateProjectWorkItemType(IVssRequestContext requestContext, Guid projectGuid)
    {
      using (WorkItemTrackingMetadataComponent component = requestContext.CreateComponent<WorkItemTrackingMetadataComponent>(connectionType: new DatabaseConnectionType?(DatabaseConnectionType.Dbo)))
        component.CreateDefaultWorkItemType(projectGuid);
    }

    public IReadOnlyCollection<ProcessChangedRecord> GetProcessesForChangedWorkItemTypes(
      IVssRequestContext requestContext,
      DateTime sinceWatermark)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      using (WorkItemTrackingMetadataComponent component = requestContext.CreateComponent<WorkItemTrackingMetadataComponent>())
        return component.GetProcessesForChangedWorkItemTypes(sinceWatermark);
    }

    public IReadOnlyCollection<ProjectGuidChangedRecord> GetProjectsForChangedWorkItemTypes(
      IVssRequestContext requestContext,
      long sinceWatermark)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      IReadOnlyCollection<ProjectIdChangedRecord> changedWorkItemTypes;
      using (WorkItemTrackingMetadataComponent component = requestContext.CreateComponent<WorkItemTrackingMetadataComponent>())
        changedWorkItemTypes = component.GetProjectsForChangedWorkItemTypes(sinceWatermark);
      return (IReadOnlyCollection<ProjectGuidChangedRecord>) WorkItemTypeService.ToProjectGuidChangedRecords(requestContext, (IEnumerable<ProjectIdChangedRecord>) changedWorkItemTypes).Where<ProjectGuidChangedRecord>((Func<ProjectGuidChangedRecord, bool>) (r => r.ProjectGuid != Guid.Empty)).ToList<ProjectGuidChangedRecord>();
    }

    public virtual bool HasAnyWorkItemsOfTypeForProcess(
      IVssRequestContext requestContext,
      Guid processType,
      string workItemTypeName)
    {
      using (WorkItemTrackingMetadataComponent component = requestContext.CreateComponent<WorkItemTrackingMetadataComponent>())
        return component.HasAnyWorkItemsOfTypeForProcess(processType, workItemTypeName);
    }

    public virtual bool HasAnyWorkItemsOfTypeForProject(
      IVssRequestContext requestContext,
      Guid projectId,
      string workItemTypeName)
    {
      using (WorkItemTrackingMetadataComponent component = requestContext.CreateComponent<WorkItemTrackingMetadataComponent>())
        return component.HasAnyWorkItemsOfTypeForProject(projectId, workItemTypeName);
    }

    private ProjectWorkItemTypes GetProjectWorkItemTypes(
      IVssRequestContext requestContext,
      Guid projectId)
    {
      return this.GetProjectWorkItemTypes(requestContext, (IEnumerable<Guid>) new Guid[1]
      {
        projectId
      }).First<ProjectWorkItemTypes>();
    }

    internal virtual IReadOnlyCollection<WorkItemTypeEntry> GetWorkItemTypeEntries(
      IVssRequestContext requestContext,
      IEnumerable<int> projectIds)
    {
      bool populateFormEntries = !WorkItemTrackingFeatureFlags.IsInheritedProcessCustomizationOnlyAccount(requestContext);
      using (WorkItemTrackingMetadataComponent component = requestContext.CreateComponent<WorkItemTrackingMetadataComponent>())
        return (IReadOnlyCollection<WorkItemTypeEntry>) component.GetWorkItemTypes(projectIds, populateFormEntries).ToList<WorkItemTypeEntry>();
    }

    private IReadOnlyCollection<ProjectWorkItemTypes> GetProjectWorkItemTypes(
      IVssRequestContext requestContext,
      IEnumerable<Guid> projectIds)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IEnumerable<Guid>>(projectIds, nameof (projectIds));
      projectIds = projectIds.Distinct<Guid>();
      WorkItemTrackingRequestContext witContext = requestContext.WitContext();
      MetadataDBStamps metadataDbStamps = witContext.MetadataDbStamps((IEnumerable<MetadataTable>) this.s_stampsNonIdentity);
      WorkItemTypeCacheService service1 = requestContext.GetService<WorkItemTypeCacheService>();
      List<ProjectWorkItemTypes> projectWorkItemTypes1 = new List<ProjectWorkItemTypes>();
      List<Guid> source = new List<Guid>();
      foreach (Guid projectId in projectIds)
      {
        ProjectWorkItemTypes projectWorkItemTypes2;
        if (service1.TryGetValue(requestContext, projectId, out projectWorkItemTypes2) && projectWorkItemTypes2.DbStamps.IsFresh(metadataDbStamps))
          projectWorkItemTypes1.Add(this.CloneWithAttachTypelets(witContext, projectWorkItemTypes2));
        else
          source.Add(projectId);
      }
      if (source.Any<Guid>())
      {
        Dictionary<Guid, int> dictionary1 = source.Select(projectId => new
        {
          LegacyId = this.GetProjectNodeId(witContext, projectId),
          Id = projectId
        }).ToDictionary(p => p.Id, p => p.LegacyId);
        ILookup<int, WorkItemTypeEntry> lookup = this.GetWorkItemTypeEntries(requestContext, (IEnumerable<int>) dictionary1.Values).ToLookup<WorkItemTypeEntry, int>((Func<WorkItemTypeEntry, int>) (t => t.ProjectId));
        ILegacyWorkItemTrackingProcessService service2 = requestContext.GetService<ILegacyWorkItemTrackingProcessService>();
        foreach (Guid guid in source)
        {
          IEnumerable<WorkItemTypeEntry> workItemTypeEntries = lookup[dictionary1[guid]];
          if (workItemTypeEntries == null || workItemTypeEntries.Count<WorkItemTypeEntry>() == 0)
            string.Format("No workitemtype entry found from the database for project: {0}", (object) guid);
          ProcessDescriptor processDescriptor;
          if (requestContext.GetService<IWorkItemTrackingProcessService>().TryGetLatestProjectProcessDescriptor(requestContext, guid, out processDescriptor))
          {
            Guid processTypeId = processDescriptor.IsDerived ? processDescriptor.Inherits : processDescriptor.TypeId;
            Dictionary<string, ProcessWorkItemTypeDefinition> dictionary2 = service2.GetProcessWorkDefinition(requestContext, processTypeId).WorkItemTypeDefinitions.ToDictionary<ProcessWorkItemTypeDefinition, string>((Func<ProcessWorkItemTypeDefinition, string>) (td => td.Name), (IEqualityComparer<string>) TFStringComparer.WorkItemTypeName);
            foreach (WorkItemTypeEntry workItemTypeEntry in workItemTypeEntries)
            {
              ProcessWorkItemTypeDefinition itemTypeDefinition;
              if (dictionary2.TryGetValue(workItemTypeEntry.Name, out itemTypeDefinition))
              {
                workItemTypeEntry.ReferenceName = itemTypeDefinition.ReferenceName ?? workItemTypeEntry.ReferenceName;
                workItemTypeEntry.Color = itemTypeDefinition.Color;
                workItemTypeEntry.Icon = itemTypeDefinition.Icon;
              }
            }
          }
          ProjectWorkItemTypes projectWorkItemTypes3 = new ProjectWorkItemTypes(workItemTypeEntries, guid, metadataDbStamps);
          service1.Set(requestContext, projectWorkItemTypes3.ProjectId, projectWorkItemTypes3);
          ProjectWorkItemTypes projectWorkItemTypes4 = this.CloneWithAttachTypelets(witContext, projectWorkItemTypes3);
          projectWorkItemTypes1.Add(projectWorkItemTypes4);
        }
      }
      return (IReadOnlyCollection<ProjectWorkItemTypes>) projectWorkItemTypes1;
    }

    private IEnumerable<WorkItemTypeEntry> GetProjectWorkItemTypesFromOobProcessXml(
      IVssRequestContext requestContext,
      ProcessWorkDefinition workDef,
      int projectNodeId)
    {
      foreach (ProcessWorkItemTypeDefinition itemTypeDefinition in (IEnumerable<ProcessWorkItemTypeDefinition>) workDef.WorkItemTypeDefinitions)
        yield return new WorkItemTypeEntry()
        {
          Color = itemTypeDefinition.Color,
          Icon = itemTypeDefinition.Icon,
          Description = itemTypeDefinition.Description,
          ProjectId = projectNodeId,
          Name = itemTypeDefinition.Name,
          ReferenceName = itemTypeDefinition.ReferenceName
        };
    }

    internal static void MergeMissingFieldsToOobWitEntries(
      IFieldTypeDictionary fieldDictionary,
      IEnumerable<WorkItemTypeEntry> entries,
      Dictionary<string, ProcessWorkItemTypeDefinition> mapByName)
    {
      foreach (WorkItemTypeEntry workItemTypeEntry in entries.Where<WorkItemTypeEntry>((Func<WorkItemTypeEntry, bool>) (e =>
      {
        if (!e.Id.HasValue)
          return true;
        int? id = e.Id;
        int num = 0;
        return id.GetValueOrDefault() > num & id.HasValue;
      })))
      {
        ProcessWorkItemTypeDefinition itemTypeDefinition;
        if (mapByName.TryGetValue(workItemTypeEntry.Name, out itemTypeDefinition))
        {
          HashSet<int> hashSet = workItemTypeEntry.UsageFields.Select<FieldUsageEntry, int>((Func<FieldUsageEntry, int>) (f => f.FieldId)).ToHashSet<int>();
          foreach (ProcessFieldDefinition fieldDefinition in (IEnumerable<ProcessFieldDefinition>) itemTypeDefinition.FieldDefinitions)
          {
            FieldEntry field;
            if (fieldDictionary.TryGetFieldByNameOrId(fieldDefinition.Name, out field))
            {
              int fieldId = field.FieldId;
              if (!hashSet.Contains(fieldId))
                workItemTypeEntry.AddField(fieldId);
            }
          }
        }
      }
    }

    private int GetProjectNodeId(WorkItemTrackingRequestContext witRequestContext, Guid projectId)
    {
      if (projectId == Guid.Empty)
        return 0;
      TreeNode node;
      if (!witRequestContext.TreeService.TryGetTreeNode(projectId, projectId, out node))
        throw new WorkItemTrackingProjectNotFoundException(projectId);
      return node.Id;
    }

    private ProjectWorkItemTypes CloneWithAttachTypelets(
      WorkItemTrackingRequestContext witRequestContext,
      ProjectWorkItemTypes projectWorkItemTypes)
    {
      ProcessDescriptor processDescriptor;
      if (WorkItemTrackingFeatureFlags.IsProcessCustomizationEnabled(witRequestContext.RequestContext) && witRequestContext.RequestContext.GetService<IWorkItemTrackingProcessService>().TryGetLatestProjectProcessDescriptor(witRequestContext.RequestContext, projectWorkItemTypes.ProjectId, out processDescriptor) && !processDescriptor.IsCustom)
      {
        IReadOnlyCollection<ProcessWorkItemType> processWorkItemTypes = (IReadOnlyCollection<ProcessWorkItemType>) null;
        Dictionary<string, int> customTypeProvisionedIdsByName = (Dictionary<string, int>) null;
        if (processDescriptor.IsDerived)
        {
          processWorkItemTypes = witRequestContext.RequestContext.GetService<IProcessWorkItemTypeService>().GetTypelets<ProcessWorkItemType>(witRequestContext.RequestContext, processDescriptor.TypeId);
          if (processWorkItemTypes != null && projectWorkItemTypes != null)
          {
            Dictionary<string, WorkItemType> workItemTypesByName = projectWorkItemTypes.WorkItemTypesById.Values.ToDictionary<WorkItemType, string>((Func<WorkItemType, string>) (wit => wit.Name), (IEqualityComparer<string>) TFStringComparer.WorkItemTypeName);
            WorkItemType workItemType;
            customTypeProvisionedIdsByName = processWorkItemTypes.Where<ProcessWorkItemType>((Func<ProcessWorkItemType, bool>) (t => t.IsCustomType)).Select<ProcessWorkItemType, WorkItemType>((Func<ProcessWorkItemType, WorkItemType>) (t => !workItemTypesByName.TryGetValue(t.Name, out workItemType) ? (WorkItemType) null : workItemType)).Where<WorkItemType>((Func<WorkItemType, bool>) (wit => wit != null && wit.Id.HasValue && wit.Name != null)).ToDictionary<WorkItemType, string, int>((Func<WorkItemType, string>) (wit => wit.Name), (Func<WorkItemType, int>) (wit => wit.Id.Value), (IEqualityComparer<string>) TFStringComparer.WorkItemTypeName);
          }
        }
        projectWorkItemTypes = this.MergeOobTypesWithProvisionedTypes(witRequestContext, projectWorkItemTypes, processDescriptor);
        if (processWorkItemTypes != null && processWorkItemTypes.Any<ProcessWorkItemType>())
          return projectWorkItemTypes.Clone(witRequestContext, (IEnumerable<ProcessWorkItemType>) processWorkItemTypes, customTypeProvisionedIdsByName);
      }
      return projectWorkItemTypes;
    }

    private ProjectWorkItemTypes MergeOobTypesWithProvisionedTypes(
      WorkItemTrackingRequestContext witRequestContext,
      ProjectWorkItemTypes projectWorkItemTypes,
      ProcessDescriptor processDescriptor)
    {
      ILegacyWorkItemTrackingProcessService service = witRequestContext.RequestContext.GetService<ILegacyWorkItemTrackingProcessService>();
      Guid guid = processDescriptor.IsDerived ? processDescriptor.Inherits : processDescriptor.TypeId;
      IVssRequestContext requestContext = witRequestContext.RequestContext;
      Guid processTypeId = guid;
      ProcessWorkDefinition processWorkDefinition = service.GetProcessWorkDefinition(requestContext, processTypeId);
      int projectNodeId = this.GetProjectNodeId(witRequestContext, projectWorkItemTypes.ProjectId);
      List<WorkItemTypeEntry> list = this.GetProjectWorkItemTypesFromOobProcessXml(witRequestContext.RequestContext, processWorkDefinition, projectNodeId).ToList<WorkItemTypeEntry>();
      Dictionary<string, ProcessWorkItemTypeDefinition> dictionary = processWorkDefinition.WorkItemTypeDefinitions.ToDictionary<ProcessWorkItemTypeDefinition, string>((Func<ProcessWorkItemTypeDefinition, string>) (td => td.Name), (IEqualityComparer<string>) TFStringComparer.WorkItemTypeName);
      WorkItemTypeService.MergeMissingFieldsToOobWitEntries(witRequestContext.FieldDictionary, (IEnumerable<WorkItemTypeEntry>) list, dictionary);
      if (projectWorkItemTypes.WorkItemTypes != null && projectWorkItemTypes.WorkItemTypes.Any<WorkItemType>())
        WorkItemTypeService.SetWorkItemTypeIds(projectWorkItemTypes.WorkItemTypes.ToDictionary<WorkItemType, string, int>((Func<WorkItemType, string>) (wit => wit.Name), (Func<WorkItemType, int>) (wit => wit.Id.Value), (IEqualityComparer<string>) TFStringComparer.WorkItemTypeName), list);
      WorkItemType workItemType;
      if (projectWorkItemTypes.WorkItemTypesById.TryGetValue(-projectNodeId, out workItemType))
        list.Add(new WorkItemTypeEntry()
        {
          Color = workItemType.Color,
          Icon = workItemType.Icon,
          Description = workItemType.Description,
          ProjectId = projectNodeId,
          Name = workItemType.Name,
          ReferenceName = workItemType.ReferenceName,
          Id = workItemType.Id
        });
      return new ProjectWorkItemTypes((IEnumerable<WorkItemTypeEntry>) list, projectWorkItemTypes.ProjectId, witRequestContext.RequestContext.MetadataDbStamps());
    }

    private static void SetWorkItemTypeIds(
      Dictionary<string, int> idsByWitName,
      List<WorkItemTypeEntry> entriesToSetIdOn)
    {
      foreach (WorkItemTypeEntry workItemTypeEntry in entriesToSetIdOn)
      {
        int num;
        if (idsByWitName.TryGetValue(workItemTypeEntry.Name, out num))
          workItemTypeEntry.Id = new int?(num);
      }
    }

    private static IReadOnlyCollection<ProjectGuidChangedRecord> ToProjectGuidChangedRecords(
      IVssRequestContext requestContext,
      IEnumerable<ProjectIdChangedRecord> projectIdChangeRecords)
    {
      if (projectIdChangeRecords == null || !projectIdChangeRecords.Any<ProjectIdChangedRecord>())
        return (IReadOnlyCollection<ProjectGuidChangedRecord>) Array.Empty<ProjectGuidChangedRecord>();
      IEnumerable<int> ids = projectIdChangeRecords.Select<ProjectIdChangedRecord, int>((Func<ProjectIdChangedRecord, int>) (pr => pr.ProjectId));
      IDictionary<int, TreeNode> treeNodes = requestContext.GetService<WorkItemTrackingTreeService>().LegacyGetTreeNodes(requestContext, ids, true, false);
      List<ProjectGuidChangedRecord> guidChangedRecords = new List<ProjectGuidChangedRecord>(projectIdChangeRecords.Count<ProjectIdChangedRecord>());
      foreach (ProjectIdChangedRecord projectIdChangeRecord in projectIdChangeRecords)
      {
        List<ProjectGuidChangedRecord> guidChangedRecordList = guidChangedRecords;
        ProjectGuidChangedRecord guidChangedRecord = new ProjectGuidChangedRecord();
        TreeNode treeNode = treeNodes[projectIdChangeRecord.ProjectId];
        guidChangedRecord.ProjectGuid = treeNode != null ? treeNode.CssNodeId : Guid.Empty;
        guidChangedRecord.MaxCacheStamp = projectIdChangeRecord.MaxCacheStamp;
        guidChangedRecordList.Add(guidChangedRecord);
      }
      return (IReadOnlyCollection<ProjectGuidChangedRecord>) guidChangedRecords;
    }

    private static IEnumerable<string> GetProjectUrisFromProjectSettingsChanged(string eventData) => (IEnumerable<string>) (eventData ?? string.Empty).Split(new char[1]
    {
      ';'
    }, StringSplitOptions.RemoveEmptyEntries);

    protected static bool TryGetProjectId(string projectUri, out Guid projectId)
    {
      try
      {
        return Guid.TryParse(CommonStructureUtils.ExtractProjectId(ref projectUri, nameof (projectUri)), out projectId);
      }
      catch (ArgumentException ex)
      {
        projectId = Guid.Empty;
        return false;
      }
    }

    public IReadOnlyCollection<ProcessChangedRecord> GetProcessesForChangedWorkItemTypeBehaviors(
      IVssRequestContext requestContext,
      DateTime sinceWatermark)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      using (WorkItemTrackingMetadataComponent component = requestContext.CreateComponent<WorkItemTrackingMetadataComponent>())
        return component.GetProcessesForChangedWorkItemTypeBehaviors(sinceWatermark);
    }
  }
}
