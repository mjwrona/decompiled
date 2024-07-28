// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemTypeCategoryService
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.Azure.Boards.ProcessTemplates;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Process;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata
{
  public class WorkItemTypeCategoryService : IWorkItemTypeCategoryService, IVssFrameworkService
  {
    private ConcurrentDictionary<Guid, MetadataDbStampedCacheEntry<WorkItemTypeCategoryService.WorkItemTypeCategoryCache>> m_workItemTypeCategoryCache = new ConcurrentDictionary<Guid, MetadataDbStampedCacheEntry<WorkItemTypeCategoryService.WorkItemTypeCategoryCache>>();

    void IVssFrameworkService.ServiceStart(IVssRequestContext systemRequestContext)
    {
      if (!systemRequestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        throw new UnexpectedHostTypeException(systemRequestContext.ServiceHost.HostType);
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public IEnumerable<WorkItemTypeCategory> LegacyGetWorkItemTypeCategories(
      IVssRequestContext requestContext,
      Guid projectId,
      bool includeOobCategoriesNotInDb)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      return this.GetWorkItemTypeCategories(requestContext.WitContext(), this.GetProjectNode(requestContext, projectId), false, includeOobCategoriesNotInDb);
    }

    public IEnumerable<WorkItemTypeCategory> GetWorkItemTypeCategories(
      IVssRequestContext requestContext,
      Guid projectId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      WorkItemTrackingRequestContext witRequestContext = requestContext.WitContext();
      TreeNode projectNode = this.GetProjectNode(requestContext, projectId);
      ProcessReadSecuredObject processReadSecuredObject = (ProcessReadSecuredObject) null;
      if (!witRequestContext.ProcessReadPermissionChecker.HasProcessReadPermissionForProject(projectId, out processReadSecuredObject))
        throw new WorkItemTypeCategoriesNotFoundException(projectNode.GetName(witRequestContext.RequestContext));
      List<WorkItemTypeCategory> list = this.GetWorkItemTypeCategories(witRequestContext, projectNode, true, true).Select<WorkItemTypeCategory, WorkItemTypeCategory>((Func<WorkItemTypeCategory, WorkItemTypeCategory>) (category => category.ShallowCopy())).ToList<WorkItemTypeCategory>();
      foreach (ProcessReadSecuredObject readSecuredObject in list)
        readSecuredObject.SetSecuredObjectProperties(processReadSecuredObject);
      return (IEnumerable<WorkItemTypeCategory>) list;
    }

    public IEnumerable<WorkItemTypeCategory> GetWorkItemTypeCategories(
      IVssRequestContext requestContext,
      string projectName)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckStringForNullOrEmpty(projectName, nameof (projectName));
      return this.GetWorkItemTypeCategories(requestContext, WorkItemTypeCategoryService.ResolveProjectId(requestContext, projectName));
    }

    public WorkItemTypeCategory GetWorkItemTypeCategory(
      IVssRequestContext requestContext,
      Guid projectId,
      string categoryNameOrRefName)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      WorkItemTrackingRequestContext witRequestContext = requestContext.WitContext();
      ProcessReadSecuredObject processReadSecuredObject = (ProcessReadSecuredObject) null;
      if (!witRequestContext.ProcessReadPermissionChecker.HasProcessReadPermissionForProject(projectId, out processReadSecuredObject))
        throw new WorkItemTypeCategoryNotFoundException(this.GetProjectNode(requestContext, projectId).GetName(witRequestContext.RequestContext), categoryNameOrRefName);
      TreeNode projectNode = this.GetProjectNode(requestContext, projectId);
      WorkItemTypeCategory itemTypeCategory = this.GetWorkItemTypeCategory(witRequestContext, projectNode, categoryNameOrRefName).ShallowCopy();
      itemTypeCategory.SetSecuredObjectProperties(processReadSecuredObject);
      return itemTypeCategory;
    }

    public WorkItemTypeCategory GetWorkItemTypeCategory(
      IVssRequestContext requestContext,
      string projectName,
      string categoryNameOrRefName)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckStringForNullOrEmpty(projectName, nameof (projectName));
      return this.GetWorkItemTypeCategory(requestContext, WorkItemTypeCategoryService.ResolveProjectId(requestContext, projectName), categoryNameOrRefName);
    }

    public bool TryGetWorkItemTypeCategory(
      IVssRequestContext requestContext,
      Guid projectId,
      string categoryNameOrRefName,
      out WorkItemTypeCategory workItemTypeCategory)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      WorkItemTrackingRequestContext witRequestContext = requestContext.WitContext();
      ProcessReadSecuredObject processReadSecuredObject = (ProcessReadSecuredObject) null;
      if (!witRequestContext.ProcessReadPermissionChecker.HasProcessReadPermissionForProject(projectId, out processReadSecuredObject))
      {
        workItemTypeCategory = (WorkItemTypeCategory) null;
        return false;
      }
      TreeNode projectNode = this.GetProjectNode(requestContext, projectId);
      if (!this.TryGetWorkItemTypeCategory(witRequestContext, projectNode, categoryNameOrRefName, out workItemTypeCategory))
        return false;
      workItemTypeCategory = workItemTypeCategory.ShallowCopy();
      workItemTypeCategory.SetSecuredObjectProperties(processReadSecuredObject);
      return true;
    }

    public bool TryGetWorkItemTypeCategory(
      IVssRequestContext requestContext,
      string projectName,
      string categoryNameOrRefName,
      out WorkItemTypeCategory workItemTypeCategory)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckStringForNullOrEmpty(projectName, nameof (projectName));
      return this.TryGetWorkItemTypeCategory(requestContext, WorkItemTypeCategoryService.ResolveProjectId(requestContext, projectName), categoryNameOrRefName, out workItemTypeCategory);
    }

    public IReadOnlyCollection<ProjectGuidChangedRecord> GetProjectsForChangedWorkItemTypeCategories(
      IVssRequestContext requestContext,
      long sinceWatermark)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      IReadOnlyCollection<ProjectIdChangedRecord> projectIdChangeRecords = (IReadOnlyCollection<ProjectIdChangedRecord>) null;
      using (WorkItemTrackingMetadataComponent component = requestContext.CreateComponent<WorkItemTrackingMetadataComponent>())
        projectIdChangeRecords = component.GetProjectsForChangedWorkItemTypeCategories(sinceWatermark);
      return (IReadOnlyCollection<ProjectGuidChangedRecord>) WorkItemTypeCategoryService.ToProjectGuidChangedRecords(requestContext, (IEnumerable<ProjectIdChangedRecord>) projectIdChangeRecords).Where<ProjectGuidChangedRecord>((Func<ProjectGuidChangedRecord, bool>) (r => r.ProjectGuid != Guid.Empty)).ToList<ProjectGuidChangedRecord>();
    }

    private static Guid ResolveProjectId(IVssRequestContext requestContext, string projectName) => requestContext.GetService<IProjectService>().GetProjectId(requestContext.Elevate(), projectName);

    private TreeNode GetProjectNode(IVssRequestContext requestContext, Guid projectId)
    {
      TreeNode node;
      if (!requestContext.WitContext().TreeService.TryGetTreeNode(projectId, projectId, out node))
        throw new WorkItemTrackingProjectNotFoundException(projectId);
      return node;
    }

    private IEnumerable<WorkItemTypeCategory> GetWorkItemTypeCategories(
      WorkItemTrackingRequestContext witRequestContext,
      TreeNode projectNode,
      bool includeProcessCustomizationMetadata,
      bool includeOobCategoriesNotInDb)
    {
      return (IEnumerable<WorkItemTypeCategory>) this.GetWorkItemTypeCategoryCache(witRequestContext, projectNode, includeProcessCustomizationMetadata, includeOobCategoriesNotInDb).WorkItemTypeCategories;
    }

    private WorkItemTypeCategory GetWorkItemTypeCategory(
      WorkItemTrackingRequestContext witRequestContext,
      TreeNode projectNode,
      string categoryNameOrRefName)
    {
      WorkItemTypeCategory workItemTypeCategory;
      if (!this.TryGetWorkItemTypeCategory(witRequestContext, projectNode, categoryNameOrRefName, out workItemTypeCategory))
        throw new WorkItemTypeCategoryNotFoundException(projectNode.GetName(witRequestContext.RequestContext), categoryNameOrRefName);
      return workItemTypeCategory;
    }

    private bool TryGetWorkItemTypeCategory(
      WorkItemTrackingRequestContext witRequestContext,
      TreeNode projectNode,
      string categoryNameOrRefName,
      out WorkItemTypeCategory workItemTypeCategory)
    {
      ArgumentUtility.CheckForNull<string>(categoryNameOrRefName, nameof (categoryNameOrRefName));
      WorkItemTypeCategoryService.WorkItemTypeCategoryCache typeCategoryCache = this.GetWorkItemTypeCategoryCache(witRequestContext, projectNode, true, true);
      return typeCategoryCache.WorkItemTypeCategoriesByReferenceName.TryGetValue(categoryNameOrRefName, out workItemTypeCategory) || typeCategoryCache.WorkItemTypeCategoriesByName.TryGetValue(categoryNameOrRefName, out workItemTypeCategory);
    }

    private WorkItemTypeCategoryService.WorkItemTypeCategoryCache GetWorkItemTypeCategoryCache(
      WorkItemTrackingRequestContext witRequestContext,
      TreeNode projectNode,
      bool includeProcessCustomizationMetadata,
      bool includeOobCategoriesNotInDb)
    {
      ArgumentUtility.CheckForNull<WorkItemTrackingRequestContext>(witRequestContext, nameof (witRequestContext));
      ArgumentUtility.CheckForNull<TreeNode>(projectNode, nameof (projectNode));
      MetadataDBStamps stamps = witRequestContext.RequestContext.MetadataDbStamps((IEnumerable<MetadataTable>) new MetadataTable[3]
      {
        MetadataTable.WorkItemTypes,
        MetadataTable.WorkItemTypeCategories,
        MetadataTable.WorkItemTypeCategoryMembers
      });
      IWorkItemTrackingProcessService service = witRequestContext.RequestContext.GetService<IWorkItemTrackingProcessService>();
      ProcessDescriptor processDescriptor;
      WorkItemTypeCategoryService.WorkItemTypeCategoryCache workItemTypeCategories1 = !service.TryGetLatestProjectProcessDescriptor(witRequestContext.RequestContext, projectNode.ProjectId, out processDescriptor) || processDescriptor.IsCustom ? this.m_workItemTypeCategoryCache.AddOrUpdate(projectNode.CssNodeId, (Func<Guid, MetadataDbStampedCacheEntry<WorkItemTypeCategoryService.WorkItemTypeCategoryCache>>) (_ => new MetadataDbStampedCacheEntry<WorkItemTypeCategoryService.WorkItemTypeCategoryCache>(this.CreateWorkItemTypeCategoryCache(witRequestContext, projectNode), stamps)), (Func<Guid, MetadataDbStampedCacheEntry<WorkItemTypeCategoryService.WorkItemTypeCategoryCache>, MetadataDbStampedCacheEntry<WorkItemTypeCategoryService.WorkItemTypeCategoryCache>>) ((_, existing) => existing.IsFresh(stamps) ? existing : new MetadataDbStampedCacheEntry<WorkItemTypeCategoryService.WorkItemTypeCategoryCache>(this.CreateWorkItemTypeCategoryCache(witRequestContext, projectNode), stamps))).Data : new WorkItemTypeCategoryService.WorkItemTypeCategoryCache(Enumerable.Empty<WorkItemTypeCategoryRecord>(), projectNode);
      if (service.TryGetLatestProjectProcessDescriptor(witRequestContext.RequestContext, projectNode.ProjectId, out processDescriptor))
      {
        if (includeOobCategoriesNotInDb)
          workItemTypeCategories1 = this.AttachOobWorkItemTypeCategories(witRequestContext.RequestContext, projectNode.ProjectId, processDescriptor, workItemTypeCategories1);
        if (includeProcessCustomizationMetadata && processDescriptor.IsDerived && WorkItemTrackingFeatureFlags.IsProcessCustomizationEnabled(witRequestContext.RequestContext))
        {
          WorkItemTypeCategoryService.WorkItemTypeCategoryCache workItemTypeCategories2 = this.AttachWorkItemTypesFromBehaviors(witRequestContext.RequestContext, processDescriptor, workItemTypeCategories1);
          workItemTypeCategories1 = this.UpdateWorkItemTypeCategoryCacheForDisabledTypes(witRequestContext.RequestContext, projectNode, workItemTypeCategories2);
        }
      }
      return workItemTypeCategories1;
    }

    private WorkItemTypeCategoryService.WorkItemTypeCategoryCache CreateWorkItemTypeCategoryCache(
      WorkItemTrackingRequestContext witRequestContext,
      TreeNode projectNode)
    {
      return witRequestContext.RequestContext.TraceBlock<WorkItemTypeCategoryService.WorkItemTypeCategoryCache>(911698, 911699, "Services", "WorkItemTypeCategories", nameof (CreateWorkItemTypeCategoryCache), (Func<WorkItemTypeCategoryService.WorkItemTypeCategoryCache>) (() =>
      {
        IEnumerable<WorkItemTypeCategoryRecord> list;
        using (WorkItemTrackingMetadataComponent component = witRequestContext.RequestContext.CreateComponent<WorkItemTrackingMetadataComponent>())
          list = (IEnumerable<WorkItemTypeCategoryRecord>) component.GetWorkItemTypeCategories(projectNode.CssNodeId).ToList<WorkItemTypeCategoryRecord>();
        return new WorkItemTypeCategoryService.WorkItemTypeCategoryCache(list, projectNode);
      }));
    }

    private WorkItemTypeCategoryService.WorkItemTypeCategoryCache AttachOobWorkItemTypeCategories(
      IVssRequestContext requestContext,
      Guid projectId,
      ProcessDescriptor processDescriptor,
      WorkItemTypeCategoryService.WorkItemTypeCategoryCache workItemTypeCategories)
    {
      return processDescriptor.IsCustom ? workItemTypeCategories : requestContext.TraceBlock<WorkItemTypeCategoryService.WorkItemTypeCategoryCache>(911706, 911707, "Services", "WorkItemTypeCategories", nameof (AttachOobWorkItemTypeCategories), (Func<WorkItemTypeCategoryService.WorkItemTypeCategoryCache>) (() =>
      {
        Guid processTypeId = processDescriptor.IsDerived ? processDescriptor.Inherits : processDescriptor.TypeId;
        return new WorkItemTypeCategoryService.WorkItemTypeCategoryCache(workItemTypeCategories, requestContext.GetService<ILegacyWorkItemTrackingProcessService>().GetProcessWorkDefinition(requestContext, processTypeId).WorkItemTypeCategories, projectId);
      }));
    }

    private WorkItemTypeCategoryService.WorkItemTypeCategoryCache AttachWorkItemTypesFromBehaviors(
      IVssRequestContext requestContext,
      ProcessDescriptor processDescriptor,
      WorkItemTypeCategoryService.WorkItemTypeCategoryCache workItemTypeCategories)
    {
      return requestContext.TraceBlock<WorkItemTypeCategoryService.WorkItemTypeCategoryCache>(911700, 911701, "Services", "WorkItemTypeCategories", nameof (AttachWorkItemTypesFromBehaviors), (Func<WorkItemTypeCategoryService.WorkItemTypeCategoryCache>) (() => new WorkItemTypeCategoryService.WorkItemTypeCategoryCache(workItemTypeCategories, this.GetWorkItemTypesFromBehaviors(requestContext, processDescriptor.TypeId, processDescriptor.Inherits), (IReadOnlyDictionary<string, string>) new Dictionary<string, string>((IEqualityComparer<string>) TFStringComparer.WorkItemTypeName))));
    }

    private WorkItemTypeCategoryService.WorkItemTypeCategoryCache UpdateWorkItemTypeCategoryCacheForDisabledTypes(
      IVssRequestContext requestContext,
      TreeNode projectNode,
      WorkItemTypeCategoryService.WorkItemTypeCategoryCache workItemTypeCategories)
    {
      return requestContext.TraceBlock<WorkItemTypeCategoryService.WorkItemTypeCategoryCache>(911704, 911705, "Services", "WorkItemTypeCategories", nameof (UpdateWorkItemTypeCategoryCacheForDisabledTypes), (Func<WorkItemTypeCategoryService.WorkItemTypeCategoryCache>) (() =>
      {
        List<string> list = requestContext.GetService<IWorkItemTypeService>().GetWorkItemTypes(requestContext, projectNode.ProjectId).Where<WorkItemType>((Func<WorkItemType, bool>) (t => t.IsDisabled)).Select<WorkItemType, string>((Func<WorkItemType, string>) (t => t.Name)).ToList<string>();
        if (!list.Any<string>())
          return workItemTypeCategories;
        Dictionary<string, ICollection<string>> categoryWorkItemTypeMappingsToMerge = new Dictionary<string, ICollection<string>>()
        {
          {
            "Microsoft.HiddenCategory",
            (ICollection<string>) list
          }
        };
        Dictionary<string, string> defaultWorkItemTypeMappingToMerge = new Dictionary<string, string>((IEqualityComparer<string>) TFStringComparer.WorkItemTypeName);
        HashSet<string> disableWorkItemTypeMap = new HashSet<string>((IEnumerable<string>) list, (IEqualityComparer<string>) TFStringComparer.WorkItemTypeName);
        foreach (WorkItemTypeCategory itemTypeCategory in (IEnumerable<WorkItemTypeCategory>) workItemTypeCategories.WorkItemTypeCategories)
        {
          if (disableWorkItemTypeMap.Contains(itemTypeCategory.DefaultWorkItemTypeName))
          {
            IOrderedEnumerable<string> source = itemTypeCategory.WorkItemTypeNames.Where<string>((Func<string, bool>) (typeName => !disableWorkItemTypeMap.Contains(typeName))).OrderBy<string, string>((Func<string, string>) (typeName => typeName));
            if (source.Any<string>())
              defaultWorkItemTypeMappingToMerge.Add(itemTypeCategory.ReferenceName, source.First<string>());
          }
        }
        return new WorkItemTypeCategoryService.WorkItemTypeCategoryCache(workItemTypeCategories, (IReadOnlyDictionary<string, ICollection<string>>) categoryWorkItemTypeMappingsToMerge, (IReadOnlyDictionary<string, string>) defaultWorkItemTypeMappingToMerge);
      }));
    }

    private IReadOnlyDictionary<string, ICollection<string>> GetWorkItemTypesFromBehaviors(
      IVssRequestContext requestContext,
      Guid processId,
      Guid parentProcessId)
    {
      return (IReadOnlyDictionary<string, ICollection<string>>) requestContext.TraceBlock<Dictionary<string, ICollection<string>>>(911702, 911703, "Services", "WorkItemTypeCategories", nameof (GetWorkItemTypesFromBehaviors), (Func<Dictionary<string, ICollection<string>>>) (() =>
      {
        IReadOnlyDictionary<string, ICollection<ProcessWorkItemType>> itemTypesInBehavior = requestContext.GetService<IProcessWorkItemTypeService>().GetDerivedWorkItemTypesInBehavior(requestContext, processId);
        Dictionary<string, ICollection<string>> typesFromBehaviors = new Dictionary<string, ICollection<string>>((IEqualityComparer<string>) TFStringComparer.WorkItemCategoryReferenceName);
        if (itemTypesInBehavior.Any<KeyValuePair<string, ICollection<ProcessWorkItemType>>>())
        {
          ProcessBacklogs processBacklogs = requestContext.GetService<ILegacyWorkItemTrackingProcessService>().GetProcessWorkDefinition(requestContext, parentProcessId).ProcessBacklogs;
          IReadOnlyCollection<Behavior> portfolioBehaviors = requestContext.GetService<IBehaviorService>().GetPortfolioBehaviors(requestContext, processId);
          ICollection<ProcessWorkItemType> source;
          if (!string.IsNullOrWhiteSpace(processBacklogs.RequirementBacklog.CategoryReferenceName) && itemTypesInBehavior.TryGetValue(BehaviorService.RequirementBehaviorReferenceName, out source))
            typesFromBehaviors[processBacklogs.RequirementBacklog.CategoryReferenceName] = (ICollection<string>) source.Select<ProcessWorkItemType, string>((Func<ProcessWorkItemType, string>) (t => t.Name)).ToList<string>();
          if (!string.IsNullOrWhiteSpace(processBacklogs.TaskBacklog.CategoryReferenceName) && itemTypesInBehavior.TryGetValue(BehaviorService.TaskBehaviorReferenceName, out source))
            typesFromBehaviors[processBacklogs.TaskBacklog.CategoryReferenceName] = (ICollection<string>) source.Select<ProcessWorkItemType, string>((Func<ProcessWorkItemType, string>) (t => t.Name)).ToList<string>();
          foreach (ProcessBacklogDefinition portfolioBacklog1 in (IEnumerable<ProcessBacklogDefinition>) processBacklogs.PortfolioBacklogs)
          {
            ProcessBacklogDefinition portfolioBacklog = portfolioBacklog1;
            Behavior behavior = portfolioBehaviors.FirstOrDefault<Behavior>((Func<Behavior, bool>) (b => b.Rank == portfolioBacklog.Rank));
            if (behavior != null && !string.IsNullOrWhiteSpace(portfolioBacklog.CategoryReferenceName) && itemTypesInBehavior.TryGetValue(behavior.ReferenceName, out source))
              typesFromBehaviors[portfolioBacklog.CategoryReferenceName] = (ICollection<string>) source.Select<ProcessWorkItemType, string>((Func<ProcessWorkItemType, string>) (t => t.Name)).ToList<string>();
          }
        }
        return typesFromBehaviors;
      }));
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

    internal class WorkItemTypeCategoryCache
    {
      private Lazy<IDictionary<string, WorkItemTypeCategory>> m_lazyWorkItemTypeCategoryByReferenceName;
      private Lazy<IDictionary<string, WorkItemTypeCategory>> m_lazyWorkItemTypeCategoryByName;

      public WorkItemTypeCategoryCache(
        IEnumerable<WorkItemTypeCategoryRecord> workItemTypeCategoryRecords,
        TreeNode projectNode)
      {
        this.WorkItemTypeCategories = (IReadOnlyCollection<WorkItemTypeCategory>) new ReadOnlyCollection<WorkItemTypeCategory>((IList<WorkItemTypeCategory>) workItemTypeCategoryRecords.Select<WorkItemTypeCategoryRecord, WorkItemTypeCategory>((Func<WorkItemTypeCategoryRecord, WorkItemTypeCategory>) (wicr => new WorkItemTypeCategory(wicr, projectNode.CssNodeId))).ToList<WorkItemTypeCategory>());
        this.InitializeLazyDictionaries();
      }

      public WorkItemTypeCategoryCache(
        WorkItemTypeCategoryService.WorkItemTypeCategoryCache cache,
        IReadOnlyDictionary<string, ICollection<string>> categoryWorkItemTypeMappingsToMerge,
        IReadOnlyDictionary<string, string> defaultWorkItemTypeMappingToMerge)
      {
        this.WorkItemTypeCategories = (IReadOnlyCollection<WorkItemTypeCategory>) WorkItemTypeCategoryService.WorkItemTypeCategoryCache.MergeWorkItemTypeCategories(cache, categoryWorkItemTypeMappingsToMerge, defaultWorkItemTypeMappingToMerge);
        this.InitializeLazyDictionaries();
      }

      public WorkItemTypeCategoryCache(
        WorkItemTypeCategoryService.WorkItemTypeCategoryCache cache,
        IReadOnlyDictionary<string, CategoryWorkItemTypes> categoriesByRefName,
        Guid projectIdOfNewCategories)
      {
        this.WorkItemTypeCategories = (IReadOnlyCollection<WorkItemTypeCategory>) WorkItemTypeCategoryService.WorkItemTypeCategoryCache.MergeWorkItemTypeCategories(cache, categoriesByRefName, projectIdOfNewCategories);
        this.InitializeLazyDictionaries();
      }

      public IReadOnlyCollection<WorkItemTypeCategory> WorkItemTypeCategories { get; private set; }

      public IDictionary<string, WorkItemTypeCategory> WorkItemTypeCategoriesByReferenceName => this.m_lazyWorkItemTypeCategoryByReferenceName.Value;

      public IDictionary<string, WorkItemTypeCategory> WorkItemTypeCategoriesByName => this.m_lazyWorkItemTypeCategoryByName.Value;

      private void InitializeLazyDictionaries()
      {
        this.m_lazyWorkItemTypeCategoryByReferenceName = new Lazy<IDictionary<string, WorkItemTypeCategory>>((Func<IDictionary<string, WorkItemTypeCategory>>) (() => (IDictionary<string, WorkItemTypeCategory>) this.WorkItemTypeCategories.ToDictionary<WorkItemTypeCategory, string>((Func<WorkItemTypeCategory, string>) (witc => witc.ReferenceName), (IEqualityComparer<string>) TFStringComparer.WorkItemCategoryName)));
        this.m_lazyWorkItemTypeCategoryByName = new Lazy<IDictionary<string, WorkItemTypeCategory>>((Func<IDictionary<string, WorkItemTypeCategory>>) (() =>
        {
          Dictionary<string, WorkItemTypeCategory> dictionary = new Dictionary<string, WorkItemTypeCategory>((IEqualityComparer<string>) TFStringComparer.WorkItemCategoryName);
          foreach (WorkItemTypeCategory itemTypeCategory in (IEnumerable<WorkItemTypeCategory>) this.WorkItemTypeCategories)
            dictionary[itemTypeCategory.Name] = itemTypeCategory;
          return (IDictionary<string, WorkItemTypeCategory>) dictionary;
        }));
      }

      private static List<WorkItemTypeCategory> MergeWorkItemTypeCategories(
        WorkItemTypeCategoryService.WorkItemTypeCategoryCache cache,
        IReadOnlyDictionary<string, ICollection<string>> categoryWorkItemTypeMappingsToMerge,
        IReadOnlyDictionary<string, string> defaultWorkItemTypeMappingToMerge)
      {
        List<WorkItemTypeCategory> itemTypeCategoryList = new List<WorkItemTypeCategory>();
        foreach (WorkItemTypeCategory itemTypeCategory in (IEnumerable<WorkItemTypeCategory>) cache.WorkItemTypeCategories)
        {
          string str = (string) null;
          ICollection<string> first;
          if (categoryWorkItemTypeMappingsToMerge.TryGetValue(itemTypeCategory.ReferenceName, out first) || defaultWorkItemTypeMappingToMerge.TryGetValue(itemTypeCategory.ReferenceName, out str))
          {
            HashSet<string> workItemTypeNames = first == null ? new HashSet<string>(itemTypeCategory.WorkItemTypeNames, (IEqualityComparer<string>) TFStringComparer.WorkItemTypeName) : new HashSet<string>(first.Union<string>(itemTypeCategory.WorkItemTypeNames), (IEqualityComparer<string>) TFStringComparer.WorkItemTypeName);
            string defaultWorkItemTypeName = string.IsNullOrEmpty(str) ? itemTypeCategory.DefaultWorkItemTypeName : str;
            itemTypeCategoryList.Add(new WorkItemTypeCategory(itemTypeCategory.ProjectId, itemTypeCategory.Id, itemTypeCategory.Name, itemTypeCategory.ReferenceName, (IEnumerable<string>) workItemTypeNames, defaultWorkItemTypeName));
          }
          else
            itemTypeCategoryList.Add(itemTypeCategory);
        }
        return itemTypeCategoryList;
      }

      internal static List<WorkItemTypeCategory> MergeWorkItemTypeCategories(
        WorkItemTypeCategoryService.WorkItemTypeCategoryCache cache,
        IReadOnlyDictionary<string, CategoryWorkItemTypes> newCategories,
        Guid projectIdOfNewCategories)
      {
        List<WorkItemTypeCategory> itemTypeCategoryList = new List<WorkItemTypeCategory>();
        HashSet<string> stringSet = new HashSet<string>((IEqualityComparer<string>) TFStringComparer.WorkItemCategoryReferenceName);
        foreach (WorkItemTypeCategory itemTypeCategory in (IEnumerable<WorkItemTypeCategory>) cache.WorkItemTypeCategories)
        {
          stringSet.Add(itemTypeCategory.ReferenceName);
          CategoryWorkItemTypes categoryWorkItemTypes;
          if (newCategories.TryGetValue(itemTypeCategory.ReferenceName, out categoryWorkItemTypes))
          {
            HashSet<string> workItemTypeNames = new HashSet<string>(itemTypeCategory.WorkItemTypeNames.Union<string>((IEnumerable<string>) categoryWorkItemTypes.WorkItemTypeNames), (IEqualityComparer<string>) TFStringComparer.WorkItemTypeName);
            itemTypeCategoryList.Add(new WorkItemTypeCategory(itemTypeCategory.ProjectId, itemTypeCategory.Id, itemTypeCategory.Name, itemTypeCategory.ReferenceName, (IEnumerable<string>) workItemTypeNames, itemTypeCategory.DefaultWorkItemTypeName));
          }
        }
        foreach (CategoryWorkItemTypes categoryWorkItemTypes in newCategories.Values)
        {
          if (!stringSet.Contains(categoryWorkItemTypes.CategoryReferenceName))
            itemTypeCategoryList.Add(new WorkItemTypeCategory(projectIdOfNewCategories, new int?(), categoryWorkItemTypes.Name, categoryWorkItemTypes.CategoryReferenceName, (IEnumerable<string>) categoryWorkItemTypes.WorkItemTypeNames, categoryWorkItemTypes.DefaultWorkItemTypeName));
        }
        return itemTypeCategoryList;
      }
    }
  }
}
