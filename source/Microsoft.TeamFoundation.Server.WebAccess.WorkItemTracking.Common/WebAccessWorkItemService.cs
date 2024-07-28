// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.WebAccessWorkItemService
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.Azure.Boards.Linking;
using Microsoft.Azure.Devops.Tags.Server;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.WorkItemTracking.Common.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Internals;
using Microsoft.TeamFoundation.WorkItemTracking.LegacyInterfaces;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels;
using Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  public class WebAccessWorkItemService : IVssFrameworkService
  {
    private ConcurrentDictionary<string, MetadataDbStampedCacheEntry<IEnumerable<WorkItemTypeCategory>>> m_workItemTypeCategoryCache = new ConcurrentDictionary<string, MetadataDbStampedCacheEntry<IEnumerable<WorkItemTypeCategory>>>((IEqualityComparer<string>) TFStringComparer.TeamProjectName);

    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    void IVssFrameworkService.ServiceStart(IVssRequestContext systemRequestContext)
    {
      if (!systemRequestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        throw new InvalidOperationException(FrameworkResources.UnexpectedHostType((object) systemRequestContext.ServiceHost.HostType.ToString()));
    }

    internal WorkItemTrackingFieldService GetFieldDictionary(IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      return requestContext.GetService<WorkItemTrackingFieldService>();
    }

    internal WorkItemTrackingTreeService GetTreeDictionary(IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      return requestContext.GetService<WorkItemTrackingTreeService>();
    }

    public virtual int GetProjectNodeId(IVssRequestContext requestContext, string projectName)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(projectName, nameof (projectName));
      return this.GetTreeNodeId(requestContext, projectName, TreeStructureType.None);
    }

    private Guid GetProjectId(IVssRequestContext requestContext, int legacyProjectId) => requestContext.WitContext().TreeService.LegacyGetTreeNode(legacyProjectId).CssNodeId;

    public virtual Guid GetProjectId(IVssRequestContext requestContext, string projectName)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(projectName, nameof (projectName));
      return requestContext.WitContext().ProjectService.GetProjectId(requestContext, projectName);
    }

    public virtual int GetAreaId(IVssRequestContext requestContext, string areaPath)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(areaPath, nameof (areaPath));
      return this.GetTreeNodeId(requestContext, areaPath, TreeStructureType.Area);
    }

    public int GetIterationId(IVssRequestContext requestContext, string iterationPath)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(iterationPath, nameof (iterationPath));
      return this.GetTreeNodeId(requestContext, iterationPath, TreeStructureType.Iteration);
    }

    private int GetTreeNodeId(
      IVssRequestContext requestContext,
      string path,
      TreeStructureType type)
    {
      return requestContext.WitContext().TreeService.LegacyGetTreeNodeIdFromPath(requestContext, path, type);
    }

    private Guid GetProjectIdentifier(IVssRequestContext requestContext, string projectName)
    {
      Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode treeNode = (Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode) null;
      return requestContext.WitContext().TreeService.TryGetNodeFromPath(requestContext, projectName, TreeStructureType.None, out treeNode) ? treeNode.CssNodeId : Guid.Empty;
    }

    public virtual TreeNode GetNode(IVssRequestContext requestContext, int nodeId)
    {
      WorkItemTrackingTreeService treeDictionary = this.GetTreeDictionary(requestContext);
      return TreeNode.CreateFrom(requestContext, treeDictionary.LegacyGetTreeNode(requestContext, nodeId), (ISecuredObject) null);
    }

    public virtual IEnumerable<Project> GetProjects(
      IVssRequestContext requestContext,
      bool includeProcessId = false)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      using (requestContext.TraceBlock(290064, 290065, "WebAccess.WorkItem", TfsTraceLayers.BusinessLogic, "WitService.GetProjects"))
      {
        WorkItemTrackingRequestContext witContext = requestContext.WitContext();
        IEnumerable<ProjectInfo> projectInfos = witContext.ProjectService.GetProjects(requestContext, ProjectState.WellFormed);
        if (includeProcessId)
          projectInfos = requestContext.GetService<ILegacyProjectPropertiesReaderService>().PopulateProperties(projectInfos, requestContext, ProcessTemplateIdPropertyNames.CurrentProcessTemplateId);
        return (IEnumerable<Project>) projectInfos.Select<ProjectInfo, Project>((System.Func<ProjectInfo, Project>) (p =>
        {
          Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode node;
          if (!witContext.TreeService.TryGetTreeNode(p.Id, p.Id, out node))
            return (Project) null;
          return new Project()
          {
            Id = node.Id,
            Name = p.Name,
            Guid = p.Id,
            ProcessTemplateId = p.Properties == null || !p.Properties.Any<ProjectProperty>() ? Guid.Empty : new Guid((string) p.Properties[0].Value),
            Visibility = p.Visibility
          };
        })).Where<Project>((System.Func<Project, bool>) (p => p != null)).ToArray<Project>();
      }
    }

    public virtual Project GetProject(IVssRequestContext requestContext, string name)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckStringForNullOrEmpty(name, nameof (name));
      WorkItemTrackingRequestContext trackingRequestContext = requestContext.WitContext();
      ProjectInfo project = trackingRequestContext.ProjectService.GetProject(requestContext, name);
      return new Project()
      {
        Name = project.Name,
        Id = trackingRequestContext.TreeService.GetTreeNode(project.Id, project.Id).Id,
        Guid = project.Id,
        Visibility = project.Visibility
      };
    }

    public virtual Project GetProject(IVssRequestContext requestContext, Guid projectId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      WorkItemTrackingRequestContext trackingRequestContext = requestContext.WitContext();
      ProjectInfo project = trackingRequestContext.ProjectService.GetProject(requestContext, projectId);
      return new Project()
      {
        Name = project.Name,
        Id = trackingRequestContext.TreeService.GetTreeNode(project.Id, project.Id).Id,
        Guid = project.Id,
        Visibility = project.Visibility
      };
    }

    internal static Guid GetProjectTemplateTypeId(ProjectInfo project)
    {
      if (project.Properties.Any<ProjectProperty>())
      {
        ProjectProperty projectProperty = project.Properties.FirstOrDefault<ProjectProperty>((System.Func<ProjectProperty, bool>) (p => p.Name.Equals(ProcessTemplateIdPropertyNames.ProcessTemplateType, StringComparison.OrdinalIgnoreCase)));
        Guid result;
        if (projectProperty != null && Guid.TryParse((string) projectProperty.Value, out result))
          return result;
      }
      return Guid.Empty;
    }

    public virtual CultureInfo GetServerCulture(IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      return requestContext.Elevate().GetService<WorkItemTrackingConfigurationSettingService>().GetConfigurationInfo(requestContext).ServerCulture;
    }

    private IEnumerable<FieldDefinition> GetFieldsInternal(IVssRequestContext requestContext)
    {
      IEnumerable<FieldDefinitionRecord> fieldDefinitions;
      IEnumerable<FieldUsageRecord> fieldUsages;
      new DataAccessLayerImpl(requestContext).GetFields(out fieldDefinitions, out fieldUsages);
      IDictionary<int, FieldDefinition> dictionary = (IDictionary<int, FieldDefinition>) fieldDefinitions.Select<FieldDefinitionRecord, FieldDefinition>((System.Func<FieldDefinitionRecord, FieldDefinition>) (r => new FieldDefinition(r))).ToDictionary<FieldDefinition, int>((System.Func<FieldDefinition, int>) (fd => fd.Id));
      foreach (FieldUsageRecord usage in fieldUsages)
        FieldUsage.AttachFieldUsage(usage, dictionary);
      return (IEnumerable<FieldDefinition>) dictionary.Values;
    }

    public virtual bool TryGetFieldDefinitionByName(
      IVssRequestContext requestContext,
      string fieldName,
      out FieldDefinition field)
    {
      WorkItemTrackingFieldService service = requestContext.GetService<WorkItemTrackingFieldService>();
      FieldEntry fieldEntry = (FieldEntry) null;
      IVssRequestContext requestContext1 = requestContext;
      string name = fieldName;
      ref FieldEntry local = ref fieldEntry;
      bool? checkFreshness = new bool?();
      if (service.TryGetField(requestContext1, name, out local, checkFreshness))
      {
        field = new FieldDefinition(fieldEntry);
        return true;
      }
      field = (FieldDefinition) null;
      return false;
    }

    public virtual IEnumerable<IDataRecord> GetWorkItems(
      IVssRequestContext requestContext,
      IEnumerable<int> workitemIDs,
      IEnumerable<string> fieldIDs,
      bool returnIdentityRef = false)
    {
      WebAccessWorkItemService accessWorkItemService1 = this;
      ArgumentUtility.CheckForNull<IEnumerable<int>>(workitemIDs, nameof (workitemIDs));
      ArgumentUtility.CheckForNull<IEnumerable<string>>(fieldIDs, nameof (fieldIDs));
      int witMaxPageSize = 200;
      for (; workitemIDs.Any<int>(); workitemIDs = workitemIDs.Skip<int>(witMaxPageSize))
      {
        IEnumerable<int> ints = workitemIDs.Take<int>(witMaxPageSize);
        WebAccessWorkItemService accessWorkItemService2 = accessWorkItemService1;
        IVssRequestContext requestContext1 = requestContext;
        IEnumerable<int> ids = ints;
        IEnumerable<string> fields = fieldIDs;
        bool flag = returnIdentityRef;
        DateTime? asOf = new DateTime?();
        int num = flag ? 1 : 0;
        foreach (IDataRecord pageWorkItem in accessWorkItemService2.PageWorkItems(requestContext1, ids, fields, asOf, returnIdentityRef: num != 0))
          yield return pageWorkItem;
      }
    }

    public virtual IReadOnlyCollection<IWorkItemType> GetWorkItemTypes(
      IVssRequestContext requestContext,
      int legacyProjectId)
    {
      return this.GetWorkItemTypes(requestContext, (IEnumerable<Guid>) new Guid[1]
      {
        this.GetProjectId(requestContext, legacyProjectId)
      });
    }

    public virtual IReadOnlyCollection<IWorkItemType> GetWorkItemTypes(
      IVssRequestContext requestContext,
      Guid projectId)
    {
      return this.GetWorkItemTypes(requestContext, (IEnumerable<Guid>) new Guid[1]
      {
        projectId
      });
    }

    public virtual IReadOnlyCollection<IWorkItemType> GetWorkItemTypes(
      IVssRequestContext requestContext,
      IEnumerable<Guid> projectIds)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IEnumerable<Guid>>(projectIds, nameof (projectIds));
      List<WorkItemType> workItemTypes = new List<WorkItemType>();
      if (projectIds.Any<Guid>())
      {
        projectIds = (IEnumerable<Guid>) projectIds.Distinct<Guid>().ToList<Guid>();
        WorkItemTrackingRequestContext trackingRequestContext = requestContext.WitContext();
        foreach (IGrouping<Guid, Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemType> grouping in requestContext.GetService<IWorkItemTypeService>().GetWorkItemTypes(requestContext, projectIds).GroupBy<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemType, Guid>((System.Func<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemType, Guid>) (type => type.ProjectId)))
        {
          Guid key = grouping.Key;
          int id1 = trackingRequestContext.TreeService.GetTreeNode(key, key).Id;
          foreach (Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemType workItemType1 in (IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemType>) grouping)
          {
            int? id2 = workItemType1.Id;
            if (WorkItemTrackingFeatureFlags.IsProcessCustomizationEnabled(requestContext))
              id2 = new int?();
            WorkItemType workItemType2 = new WorkItemType(id2, id1, key, workItemType1.Name, workItemType1.ReferenceName, workItemType1.Description, workItemType1.Color);
            foreach (int fieldId in (IEnumerable<int>) workItemType1.GetFieldIds(requestContext))
              workItemType2.InternalFields.Add(fieldId);
            workItemTypes.Add(workItemType2);
          }
        }
      }
      return (IReadOnlyCollection<IWorkItemType>) workItemTypes;
    }

    public virtual IEnumerable<string> GetWorkItemNamesForCategories(
      IVssRequestContext requestContext,
      string projectName,
      IEnumerable<string> categoryRefNames)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckStringForNullOrEmpty(projectName, nameof (projectName));
      return this.GetWorkItemTypeCategories(requestContext, projectName, categoryRefNames).SelectMany<WorkItemTypeCategory, string>((System.Func<WorkItemTypeCategory, IEnumerable<string>>) (category => category.WorkItemTypeNames));
    }

    public virtual IDictionary<string, IEnumerable<string>> GetWorkItemTypeMappingsForCategories(
      IVssRequestContext requestContext,
      string projectName,
      IEnumerable<string> categoryRefNames)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckStringForNullOrEmpty(projectName, nameof (projectName));
      return (IDictionary<string, IEnumerable<string>>) this.GetWorkItemTypeCategories(requestContext, projectName, categoryRefNames).ToDictionary<WorkItemTypeCategory, string, IEnumerable<string>>((System.Func<WorkItemTypeCategory, string>) (category => category.ReferenceName), (System.Func<WorkItemTypeCategory, IEnumerable<string>>) (category => category.WorkItemTypeNames));
    }

    internal IDictionary<ConstantSetReference, SetRecord[]> GetConstantSets(
      IVssRequestContext requestContext,
      IEnumerable<ConstantSetReference> setReferences)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      return new DataAccessLayerImpl(requestContext).GetConstantSets(setReferences);
    }

    internal Dictionary<int, string> GetConstants(
      IVssRequestContext requestContext,
      IEnumerable<int> constantIds)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      return new DataAccessLayerImpl(requestContext).GetConstants(constantIds);
    }

    public virtual WorkItemLinkTypeCollection GetLinkTypes(IVssRequestContext requestContext) => new WorkItemLinkTypeCollection(requestContext.GetService<IWorkItemTrackingLinkService>().GetLinkTypes(requestContext).Select<MDWorkItemLinkType, WorkItemLinkType>((System.Func<MDWorkItemLinkType, WorkItemLinkType>) (lt => new WorkItemLinkType(new Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItemLinkTypeRecord()
    {
      ForwardId = lt.ForwardId,
      ForwardName = lt.ForwardEndName,
      ReverseId = lt.ReverseId,
      ReverseName = lt.ReverseEndName,
      ReferenceName = lt.ReferenceName,
      Rules = lt.Rules
    }))));

    public virtual IEnumerable<RegisteredLinkType> GetRegisteredLinkTypes(
      IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      return requestContext.GetService<IArtifactLinkTypesService>().GetArtifactLinkTypes(requestContext, "WorkItemTracking").Where<RegistrationArtifactType>((System.Func<RegistrationArtifactType, bool>) (artifactType => VssStringComparer.ArtifactType.Equals(artifactType.Name, "WorkItem"))).SelectMany<RegistrationArtifactType, RegisteredLinkType>((System.Func<RegistrationArtifactType, IEnumerable<RegisteredLinkType>>) (type => ((IEnumerable<OutboundLinkType>) type.OutboundLinkTypes).Select<OutboundLinkType, RegisteredLinkType>((System.Func<OutboundLinkType, RegisteredLinkType>) (outboundType => new RegisteredLinkType()
      {
        Name = outboundType.Name,
        ToolId = outboundType.TargetArtifactTypeTool
      }))));
    }

    public virtual IEnumerable<string> GetAllowedValues(
      IVssRequestContext requestContext,
      int fieldId)
    {
      return this.GetAllowedValues(requestContext, fieldId, (string) null);
    }

    public virtual IEnumerable<string> GetAllowedValues(
      IVssRequestContext requestContext,
      int fieldId,
      string project)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      return requestContext.GetService<ITeamFoundationWorkItemService>().GetAllowedValues(requestContext, fieldId, project);
    }

    public virtual IEnumerable<string> GetAllowedValues(
      IVssRequestContext requestContext,
      int fieldId,
      string project,
      IEnumerable<string> workItemTypeNames,
      bool excludeIdentities = false)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      return requestContext.GetService<ITeamFoundationWorkItemService>().GetAllowedValues(requestContext, fieldId, project, workItemTypeNames, excludeIdentities);
    }

    public virtual IEnumerable<string> GetGlobalAndProjectGroups(
      IVssRequestContext requestContext,
      int projectId,
      bool includeGlobal)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      return new DataAccessLayerImpl(requestContext).GetGlobalAndProjectGroups(projectId, includeGlobal);
    }

    public virtual IEnumerable<WorkItemTypeCategory> GetWorkItemTypeCategories(
      IVssRequestContext requestContext,
      string projectName,
      IEnumerable<string> categoryRefNames)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      return this.GetWorkItemTypeCategories(requestContext, projectName).Where<WorkItemTypeCategory>((System.Func<WorkItemTypeCategory, bool>) (c => categoryRefNames == null || categoryRefNames.Contains<string>(c.ReferenceName, (IEqualityComparer<string>) TFStringComparer.WorkItemCategoryReferenceName)));
    }

    public virtual IEnumerable<WorkItemTypeCategory> GetWorkItemTypeCategories(
      IVssRequestContext requestContext,
      string projectName)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      IProjectService service = requestContext.GetService<IProjectService>();
      return requestContext.GetService<IWorkItemTypeCategoryService>().GetWorkItemTypeCategories(requestContext, service.GetProjectId(requestContext, projectName));
    }

    public virtual IDictionary<string, IList<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItemTypeTransition>> GetWorkItemTypeTransitions(
      IVssRequestContext requestContext,
      IEnumerable<IWorkItemType> workItemTypes)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      IDictionary<string, IList<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItemTypeTransition>> itemTypeTransitions = (IDictionary<string, IList<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItemTypeTransition>>) new Dictionary<string, IList<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItemTypeTransition>>();
      foreach (IWorkItemType workItemType in workItemTypes)
      {
        AdditionalWorkItemTypeProperties extendedProperties = workItemType.GetExtendedProperties(requestContext);
        List<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItemTypeTransition> itemTypeTransitionList = new List<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItemTypeTransition>();
        foreach (KeyValuePair<string, HashSet<string>> transition in (IEnumerable<KeyValuePair<string, HashSet<string>>>) extendedProperties.Transitions)
        {
          foreach (string toState in transition.Value)
            itemTypeTransitionList.Add(new Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItemTypeTransition(workItemType.Name, transition.Key, toState));
        }
        itemTypeTransitions[workItemType.Name] = (IList<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItemTypeTransition>) itemTypeTransitionList;
      }
      return itemTypeTransitions;
    }

    private IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems.QueryItem> FlattenQueryItem(
      Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems.QueryItem queryItem)
    {
      List<Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems.QueryItem> queryItemList = new List<Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems.QueryItem>();
      if (queryItem != null)
        queryItemList.Add(queryItem);
      if (queryItem is QueryFolder queryFolder)
        queryItemList.AddRange(queryFolder.Children.SelectMany<Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems.QueryItem, Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems.QueryItem>((System.Func<Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems.QueryItem, IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems.QueryItem>>) (qc => this.FlattenQueryItem(qc))));
      return (IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems.QueryItem>) queryItemList;
    }

    public virtual void GetQueryItems(
      IVssRequestContext requestContext,
      int projectId,
      out IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems.QueryItem> publicQueries,
      out IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems.QueryItem> privateQueries)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ITeamFoundationQueryItemService service = requestContext.GetService<ITeamFoundationQueryItemService>();
      Guid cssNodeId = requestContext.WitContext().TreeService.LegacyGetTreeNode(projectId).CssNodeId;
      IVssRequestContext requestContext1 = requestContext;
      Guid projectId1 = cssNodeId;
      QueryFolder[] queryHierarchy = service.GetQueryHierarchy(requestContext1, projectId1);
      publicQueries = queryHierarchy[0] != null ? this.FlattenQueryItem((Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems.QueryItem) queryHierarchy[0]) : Enumerable.Empty<Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems.QueryItem>();
      privateQueries = queryHierarchy[1] != null ? this.FlattenQueryItem((Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems.QueryItem) queryHierarchy[1]) : Enumerable.Empty<Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems.QueryItem>();
    }

    public virtual Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItem GetQueryItem(
      IVssRequestContext requestContext,
      Guid queryId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      return Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItem.Create(requestContext.GetService<ITeamFoundationQueryItemService>().GetQueryById(requestContext, queryId, new int?(0), true));
    }

    public virtual void DeleteQueryItem(IVssRequestContext requestContext, Guid queryId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      new DataAccessLayerImpl(requestContext).DeleteQueryItem(queryId);
    }

    public virtual void UpdateQueryItem(
      IVssRequestContext requestContext,
      Guid id,
      Guid parentId,
      string name,
      string queryText)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      new DataAccessLayerImpl(requestContext).UpdateQueryItem(id, parentId, name, queryText);
    }

    public virtual void CreateQueryItem(
      IVssRequestContext requestContext,
      Guid id,
      Guid parentId,
      string name,
      string queryText)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      new DataAccessLayerImpl(requestContext).CreateQueryItem(id, parentId, name, queryText);
    }

    public virtual GenericDataReader PageWorkItems(
      IVssRequestContext requestContext,
      IEnumerable<int> ids,
      IEnumerable<string> fields,
      DateTime? asOf = null,
      bool isDeleted = false,
      int batchSize = 200,
      bool returnIdentityRef = false)
    {
      return this.PageWorkItems(requestContext, ids, fields, asOf, isDeleted ? WorkItemRetrievalMode.Deleted : WorkItemRetrievalMode.NonDeleted, batchSize, returnIdentityRef);
    }

    public virtual GenericDataReader PageWorkItems(
      IVssRequestContext requestContext,
      IEnumerable<int> ids,
      IEnumerable<string> fields,
      DateTime? asOf,
      WorkItemRetrievalMode workItemRetrievalMode,
      int batchSize = 200,
      bool returnIdentityRef = false)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IEnumerable<int>>(ids, nameof (ids));
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) fields, nameof (fields));
      WorkItemTrackingRequestContext witReqContext = requestContext.WitContext();
      IFieldTypeDictionary fieldDict = witReqContext.FieldDictionary;
      FieldEntry[] array = fields.Select<string, FieldEntry>((System.Func<string, FieldEntry>) (fname => fieldDict.GetField(fname))).ToArray<FieldEntry>();
      ITeamFoundationWorkItemService service = requestContext.GetService<ITeamFoundationWorkItemService>();
      using (PerformanceTimer.StartMeasure(requestContext, "WebAccessWorkItemService.PageWorkItems"))
      {
        ITeamFoundationWorkItemService foundationWorkItemService = service;
        IVssRequestContext requestContext1 = requestContext;
        IEnumerable<int> workItemIds = ids;
        FieldEntry[] fields1 = array;
        DateTime? asOf1 = asOf;
        WorkItemRetrievalMode itemRetrievalMode = workItemRetrievalMode;
        int batchSize1 = batchSize;
        int workItemRetrievalMode1 = (int) itemRetrievalMode;
        int num = returnIdentityRef ? 1 : 0;
        IEnumerable<WorkItemFieldData> workItemFieldValues = foundationWorkItemService.GetWorkItemFieldValues(requestContext1, workItemIds, (IEnumerable<FieldEntry>) fields1, asOf: asOf1, batchSize: batchSize1, workItemRetrievalMode: (WorkItemRetrievalMode) workItemRetrievalMode1, useWorkItemIdentity: num != 0);
        return this.GetReaderForPageData(witReqContext, array, workItemFieldValues, returnIdentityRef);
      }
    }

    public GenericDataReader PageWorkItems(
      IVssRequestContext requestContext,
      int[] ids,
      int[] revisions,
      IEnumerable<string> fields,
      WorkItemRetrievalMode workItemRetrievalMode,
      bool returnIdentityRef = false)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<int[]>(ids, nameof (ids));
      ArgumentUtility.CheckForNull<int[]>(revisions, nameof (revisions));
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) fields, nameof (fields));
      List<WorkItemIdRevisionPair> workItemIdRevPairs = new List<WorkItemIdRevisionPair>();
      for (int index = 0; index < ids.Length; ++index)
        workItemIdRevPairs.Add(new WorkItemIdRevisionPair()
        {
          Id = ids[index],
          Revision = revisions[index]
        });
      WorkItemTrackingRequestContext witReqContext = requestContext.WitContext();
      IFieldTypeDictionary fieldDict = witReqContext.FieldDictionary;
      FieldEntry[] array = fields.Select<string, FieldEntry>((System.Func<string, FieldEntry>) (fname => fieldDict.GetField(fname))).ToArray<FieldEntry>();
      ITeamFoundationWorkItemService service = requestContext.GetService<ITeamFoundationWorkItemService>();
      using (PerformanceTimer.StartMeasure(requestContext, "WebAccessWorkItemService.PageWorkItems"))
      {
        IEnumerable<WorkItemFieldData> workItemFieldValues = service.GetWorkItemFieldValues(requestContext, (IEnumerable<WorkItemIdRevisionPair>) workItemIdRevPairs, (IEnumerable<FieldEntry>) array, workItemRetrievalMode: workItemRetrievalMode, useWorkItemIdentity: returnIdentityRef);
        return this.GetReaderForPageData(witReqContext, array, workItemFieldValues, returnIdentityRef);
      }
    }

    private GenericDataReader GetReaderForPageData(
      WorkItemTrackingRequestContext witReqContext,
      FieldEntry[] columns,
      IEnumerable<WorkItemFieldData> records,
      bool returnIdentityRef = false)
    {
      KeyValuePair<string, Type>[] array1 = ((IEnumerable<FieldEntry>) columns).Select<FieldEntry, KeyValuePair<string, Type>>((System.Func<FieldEntry, KeyValuePair<string, Type>>) (fd => new KeyValuePair<string, Type>(fd.ReferenceName, fd.SystemType))).ToArray<KeyValuePair<string, Type>>();
      int columnCount = columns.Length;
      object[][] array2 = records.Select<WorkItemFieldData, object[]>((System.Func<WorkItemFieldData, object[]>) (record =>
      {
        object[] readerForPageData = new object[columnCount];
        int index1 = 0;
        for (int index2 = columnCount; index1 < index2; ++index1)
        {
          FieldEntry column = columns[index1];
          readerForPageData[index1] = record.GetFieldValue(witReqContext, column.FieldId, false);
          if (returnIdentityRef && readerForPageData[index1] is WorkItemIdentity workItemIdentity2)
            readerForPageData[index1] = !WorkItemTrackingFeatureFlags.AreServicePrincipalsAllowed(witReqContext.RequestContext) ? (object) workItemIdentity2.ToWitIdentityRef().IdentityRef : (object) workItemIdentity2.ToWitIdentityRef();
        }
        return readerForPageData;
      })).ToArray<object[]>();
      return new GenericDataReader((IEnumerable<KeyValuePair<string, Type>>) array1, (IEnumerable<IEnumerable<object>>) array2);
    }

    private SecuredGenericDataReader GetSecuredReaderForPageData(
      WorkItemTrackingRequestContext witReqContext,
      FieldEntry[] columns,
      IEnumerable<WorkItemFieldData> records,
      bool returnIdentityRef = false)
    {
      KeyValuePair<string, Type>[] array1 = ((IEnumerable<FieldEntry>) columns).Select<FieldEntry, KeyValuePair<string, Type>>((System.Func<FieldEntry, KeyValuePair<string, Type>>) (fd => new KeyValuePair<string, Type>(fd.ReferenceName, fd.SystemType))).ToArray<KeyValuePair<string, Type>>();
      int columnCount = columns.Length;
      SecuredGenericData[] array2 = records.Select<WorkItemFieldData, SecuredGenericData>((System.Func<WorkItemFieldData, SecuredGenericData>) (record =>
      {
        object[] data = new object[columnCount];
        int index1 = 0;
        for (int index2 = columnCount; index1 < index2; ++index1)
        {
          FieldEntry column = columns[index1];
          data[index1] = record.GetFieldValue(witReqContext, column.FieldId, false);
          if (returnIdentityRef && data[index1] is WorkItemIdentity workItemIdentity2)
            data[index1] = (object) workItemIdentity2.ToWitIdentityRef();
        }
        return new SecuredGenericData((IEnumerable<object>) data, ((ISecuredObject) record).NamespaceId, ((ISecuredObject) record).RequiredPermissions, ((ISecuredObject) record).GetToken());
      })).ToArray<SecuredGenericData>();
      return new SecuredGenericDataReader((IEnumerable<KeyValuePair<string, Type>>) array1, (IEnumerable<SecuredGenericData>) array2);
    }

    public virtual SecuredGenericDataReader SecuredPageWorkItems(
      IVssRequestContext requestContext,
      IEnumerable<int> ids,
      IEnumerable<string> fields,
      DateTime? asOf,
      WorkItemRetrievalMode workItemRetrievalMode,
      int batchSize = 200,
      bool returnIdentityRef = false)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IEnumerable<int>>(ids, nameof (ids));
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) fields, nameof (fields));
      WorkItemTrackingRequestContext witReqContext = requestContext.WitContext();
      IFieldTypeDictionary fieldDict = witReqContext.FieldDictionary;
      FieldEntry[] array = fields.Select<string, FieldEntry>((System.Func<string, FieldEntry>) (fname => fieldDict.GetField(fname))).ToArray<FieldEntry>();
      ITeamFoundationWorkItemService service = requestContext.GetService<ITeamFoundationWorkItemService>();
      IVssRequestContext requestContext1 = requestContext;
      IEnumerable<int> workItemIds = ids;
      FieldEntry[] fields1 = array;
      DateTime? asOf1 = asOf;
      WorkItemRetrievalMode itemRetrievalMode = workItemRetrievalMode;
      int batchSize1 = batchSize;
      int workItemRetrievalMode1 = (int) itemRetrievalMode;
      int num = returnIdentityRef ? 1 : 0;
      IEnumerable<WorkItemFieldData> workItemFieldValues = service.GetWorkItemFieldValues(requestContext1, workItemIds, (IEnumerable<FieldEntry>) fields1, asOf: asOf1, batchSize: batchSize1, workItemRetrievalMode: (WorkItemRetrievalMode) workItemRetrievalMode1, useWorkItemIdentity: num != 0);
      return this.GetSecuredReaderForPageData(witReqContext, array, workItemFieldValues, returnIdentityRef);
    }

    public virtual SecuredGenericDataReader SecuredPageWorkItems(
      IVssRequestContext requestContext,
      IEnumerable<int> ids,
      IEnumerable<string> fields,
      DateTime? asOf = null,
      bool isDeleted = false,
      int batchSize = 200,
      bool returnIdentityRef = false)
    {
      return this.SecuredPageWorkItems(requestContext, ids, fields, asOf, isDeleted ? WorkItemRetrievalMode.Deleted : WorkItemRetrievalMode.NonDeleted, batchSize, returnIdentityRef);
    }

    public virtual IEnumerable<WorkItemUpdateResult> SaveWorkItems(
      IVssRequestContext requestContext,
      IEnumerable<WorkItemUpdate> workItemUpdates,
      bool bypassRules = false,
      bool includeInRecentActivity = false,
      int[] workItemIdsToIncludeInRecentActivity = null)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      CommonUtility.CheckEnumerableElements<WorkItemUpdate>(workItemUpdates, nameof (workItemUpdates), (Action<WorkItemUpdate, string>) ((update, paramName) => ArgumentUtility.CheckForNull<WorkItemUpdate>(update, paramName)));
      return requestContext.GetService<TeamFoundationWorkItemService>().UpdateWorkItems(requestContext, workItemUpdates, bypassRules, false, includeInRecentActivity, (IReadOnlyCollection<int>) workItemIdsToIncludeInRecentActivity, false, false, true, false);
    }

    internal void GetFileAttachment(IVssRequestContext requestContext, Stream output, int fileId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<Stream>(output, nameof (output));
      TeamFoundationWorkItemAttachmentService service = requestContext.GetService<TeamFoundationWorkItemAttachmentService>();
      long num = 0;
      IVssRequestContext requestContext1 = requestContext;
      int tfsFileId = fileId;
      ref long local1 = ref num;
      CompressionType compressionType;
      ref CompressionType local2 = ref compressionType;
      using (Stream stream = service.RetrieveAttachmentInternal(requestContext1, tfsFileId, out local1, out local2))
        stream.CopyTo(output);
    }

    internal int GetAttachmentFileId(IVssRequestContext requestContext, int attachmentId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      return requestContext.GetService<TeamFoundationWorkItemAttachmentService>().GetAttachmentTfsFileId(requestContext, new Guid?(), attachmentId, out ISecuredObject _, out Guid _);
    }

    internal int GetAttachmentFileId(IVssRequestContext requestContext, Guid attachmentGuid)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      return requestContext.GetService<TeamFoundationWorkItemAttachmentService>().GetAttachmentTfsFileId(requestContext, new Guid?(), attachmentGuid, out ISecuredObject _, false);
    }

    internal MetadataDBStamps GetMetadataTableTimestamps(
      IVssRequestContext requestContext,
      IEnumerable<MetadataTable> tableNames,
      int projectId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) tableNames, nameof (tableNames));
      return requestContext.MetadataDbStamps(tableNames);
    }

    public MetadataDBStamps GetMetadataTableTimestamps(IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      return requestContext.MetadataDbStamps();
    }

    internal IEnumerable<long> GetQueryItemsTimestamps(
      IVssRequestContext requestContext,
      int projectId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      return new DataAccessLayerImpl(requestContext).GetQueryItemsTimestamps(projectId);
    }

    internal static bool RequestContainsTags(IEnumerable<string> fields)
    {
      ArgumentUtility.CheckForNull<IEnumerable<string>>(fields, nameof (fields));
      return fields.Contains<string>("System.Tags", (IEqualityComparer<string>) TFStringComparer.WorkItemFieldReferenceName);
    }

    internal IEnumerable<Tuple<TagDefinition, IEnumerable<int>>> GetTagWorkitemIds(
      IVssRequestContext requestContext,
      IEnumerable<int> workitemIds)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) workitemIds, nameof (workitemIds));
      TagArtifact<int>[] array = this.PageWorkItems(requestContext, workitemIds, (IEnumerable<string>) new string[2]
      {
        CoreFieldReferenceNames.Id,
        CoreFieldReferenceNames.TeamProject
      }, new DateTime?(), WorkItemRetrievalMode.All).Select<IDataRecord, TagArtifact<int>>((System.Func<IDataRecord, TagArtifact<int>>) (record =>
      {
        int artifactId = (int) record[CoreFieldReferenceNames.Id];
        return new TagArtifact<int>(this.GetProjectIdentifier(requestContext, (string) record[CoreFieldReferenceNames.TeamProject]), artifactId);
      })).ToArray<TagArtifact<int>>();
      IEnumerable<ArtifactTags<int>> tagsForArtifacts = requestContext.GetService<ITeamFoundationTaggingService>().GetTagsForArtifacts<int>(requestContext, WorkItemArtifactKinds.WorkItem, (IEnumerable<TagArtifact<int>>) array);
      Dictionary<TagDefinition, HashSet<int>> source = new Dictionary<TagDefinition, HashSet<int>>();
      foreach (ArtifactTags<int> artifactTags in tagsForArtifacts)
      {
        foreach (TagDefinition tag in artifactTags.Tags)
        {
          HashSet<int> intSet;
          if (!source.TryGetValue(tag, out intSet))
          {
            intSet = new HashSet<int>();
            source[tag] = intSet;
          }
          if (!intSet.Contains(artifactTags.Artifact.Id))
            intSet.Add(artifactTags.Artifact.Id);
        }
      }
      return source.Select<KeyValuePair<TagDefinition, HashSet<int>>, Tuple<TagDefinition, IEnumerable<int>>>((System.Func<KeyValuePair<TagDefinition, HashSet<int>>, Tuple<TagDefinition, IEnumerable<int>>>) (item => new Tuple<TagDefinition, IEnumerable<int>>(item.Key, (IEnumerable<int>) item.Value)));
    }

    internal virtual IDataAccessLayer GetDataAccessLayer(IVssRequestContext requestContext) => (IDataAccessLayer) new DataAccessLayerImpl(requestContext);
  }
}
