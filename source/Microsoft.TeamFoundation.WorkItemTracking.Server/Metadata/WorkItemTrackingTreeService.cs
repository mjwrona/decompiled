// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemTrackingTreeService
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.Azure.Boards.CssNodes;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Internals;
using Microsoft.TeamFoundation.WorkItemTracking.Notification;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Events;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata
{
  public class WorkItemTrackingTreeService : WorkItemTrackingDictionaryService
  {
    private const int c_maxDepth = 14;
    private WorkItemChangedEventServiceBusPublisher m_serviceBusEventPublisher;
    private const string c_projectAreaPathLimitRegistryPath = "/Configuration/Service/WIT/ProjectAreaPathLimit";
    private const string c_projectIterationPathLimitRegistryPath = "/Configuration/Service/WIT/ProjectIterationPathLimit";
    private const int c_defaultAreaPathLimit = 10000;
    private const int c_defaultIterationPathLimit = 10000;
    private long m_pendingReclassificationStamp;

    public static int GetProjectAreaPathLimit(IVssRequestContext requestContext) => requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) "/Configuration/Service/WIT/ProjectAreaPathLimit", true, 10000);

    public static int GetProjectIterationPathLimit(IVssRequestContext requestContext) => requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) "/Configuration/Service/WIT/ProjectIterationPathLimit", true, 10000);

    protected override IEnumerable<MetadataTable> MetadataTables => (IEnumerable<MetadataTable>) new MetadataTable[1];

    protected override void ServiceStart(IVssRequestContext systemRequestContext)
    {
      base.ServiceStart(systemRequestContext);
      this.RegisterSqlNotifications(systemRequestContext, BaseTeamFoundationWorkItemTrackingService.SqlNotificationSubscription.Default(SpecialGuids.TreeChanged));
      this.QueueUpdateReclassificationStatusTask(systemRequestContext);
      systemRequestContext.GetService<IVssRegistryService>().RegisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnRegistryChange), true, "/Service/WorkItemTracking/Settings/WorkItemReclassificationStatusCheckInterval");
      this.m_serviceBusEventPublisher = new WorkItemChangedEventServiceBusPublisher(systemRequestContext);
    }

    private void OnRegistryChange(
      IVssRequestContext systemRequestContext,
      RegistryEntryCollection args)
    {
      this.QueueUpdateReclassificationStatusTask(systemRequestContext);
    }

    protected override void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.GetService<IVssRegistryService>().UnregisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnRegistryChange));
      systemRequestContext.To(TeamFoundationHostType.Deployment).GetService<TeamFoundationTaskService>().RemoveTask(systemRequestContext, new TeamFoundationTaskCallback(this.UpdateReclassificationStatusTaskCallback));
      base.ServiceEnd(systemRequestContext);
    }

    protected override CacheSnapshotBase CreateSnapshot(
      IVssRequestContext requestContext,
      CacheSnapshotBase currentSnapshot)
    {
      return (CacheSnapshotBase) requestContext.TraceBlock<WorkItemTrackingTreeService.TreeNodeCacheServiceImpl>(904711, 904720, 904715, "Services", "WorkItemService", "WorkItemTreeService.CreateSnapshot", (Func<WorkItemTrackingTreeService.TreeNodeCacheServiceImpl>) (() => new WorkItemTrackingTreeService.TreeNodeCacheServiceImpl(requestContext, this, requestContext.MetadataDbStamps(this.MetadataTables), currentSnapshot as WorkItemTrackingTreeService.TreeNodeCacheServiceImpl)));
    }

    public virtual ITreeDictionary GetSnapshot(IVssRequestContext requestContext) => (ITreeDictionary) requestContext.TraceBlock<WorkItemTrackingTreeService.TreeNodeCacheServiceImpl>(904691, 904700, 904695, "Services", "WorkItemService", "WorkItemTreeService.GetSnapshot", (Func<WorkItemTrackingTreeService.TreeNodeCacheServiceImpl>) (() =>
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      return this.GetSnapshot<WorkItemTrackingTreeService.TreeNodeCacheServiceImpl>(requestContext);
    }));

    public virtual TreeNode GetTreeNode(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid nodeId,
      bool throwIfNotFound = true)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      return this.GetTreeNodeInternal(requestContext, projectId, nodeId, throwIfNotFound);
    }

    protected virtual TreeNode GetTreeNodeInternal(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid nodeId,
      bool throwIfNotFound = true)
    {
      return requestContext.TraceBlock<TreeNode>(904701, 904705, 904703, "Services", "WorkItemService", "WorkItemTreeService.GetTreeNodeInternal", (Func<TreeNode>) (() =>
      {
        TreeNode node;
        bool treeNodeInternal = this.GetSnapshot<WorkItemTrackingTreeService.TreeNodeCacheServiceImpl>(requestContext, false).TryGetTreeNodeInternal(projectId, nodeId, out node);
        if (!treeNodeInternal)
          treeNodeInternal = this.GetSnapshot<WorkItemTrackingTreeService.TreeNodeCacheServiceImpl>(requestContext).TryGetTreeNodeInternal(projectId, nodeId, out node);
        if (!treeNodeInternal & throwIfNotFound)
          throw new WorkItemTrackingTreeNodeNotFoundException(nodeId);
        return !treeNodeInternal ? (TreeNode) null : node;
      }));
    }

    public virtual TreeNode GetTreeNode(
      IVssRequestContext requestContext,
      Guid projectId,
      int nodeId,
      bool throwIfNotFound = true)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      return this.GetTreeNodeInternal(requestContext, projectId, nodeId, throwIfNotFound);
    }

    protected virtual TreeNode GetTreeNodeInternal(
      IVssRequestContext requestContext,
      Guid projectId,
      int nodeId,
      bool throwIfNotFound = true)
    {
      return requestContext.TraceBlock<TreeNode>(904706, 904710, 904708, "Services", "WorkItemService", "WorkItemTreeService.LegacyGetTreeNodeInternal", (Func<TreeNode>) (() =>
      {
        TreeNode node;
        bool treeNodeInternal = this.GetSnapshot<WorkItemTrackingTreeService.TreeNodeCacheServiceImpl>(requestContext, false).TryGetTreeNodeInternal(projectId, nodeId, out node);
        if (!treeNodeInternal)
          treeNodeInternal = this.GetSnapshot<WorkItemTrackingTreeService.TreeNodeCacheServiceImpl>(requestContext).TryGetTreeNodeInternal(projectId, nodeId, out node);
        if (!treeNodeInternal & throwIfNotFound)
          throw new WorkItemTrackingTreeNodeNotFoundException(nodeId);
        return !treeNodeInternal ? (TreeNode) null : node;
      }));
    }

    public virtual TreeNode GetTreeNode(
      IVssRequestContext requestContext,
      Guid projectId,
      TreeStructureType structureType,
      string relativePath,
      bool throwIfNotFound = true)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      ArgumentUtility.CheckForNull<string>(relativePath, nameof (relativePath));
      return requestContext.TraceBlock<TreeNode>(904890, 904892, 904891, "Services", "WorkItemService", nameof (GetTreeNode), (Func<TreeNode>) (() =>
      {
        WorkItemTrackingTreeService.TreeNodeCacheServiceImpl snapshot = this.GetSnapshot<WorkItemTrackingTreeService.TreeNodeCacheServiceImpl>(requestContext, false);
        TreeNode node = (TreeNode) null;
        Guid projectId1 = projectId;
        string relativePath1 = relativePath;
        int type = (int) structureType;
        ref TreeNode local = ref node;
        bool treeNode = snapshot.TryGetTreeNode(projectId1, relativePath1, (TreeStructureType) type, out local);
        if (!treeNode)
          treeNode = this.GetSnapshot<WorkItemTrackingTreeService.TreeNodeCacheServiceImpl>(requestContext).TryGetTreeNode(projectId, relativePath, structureType, out node);
        if (!treeNode & throwIfNotFound)
          throw new WorkItemTrackingTreeNodeNotFoundException(relativePath);
        return node;
      }));
    }

    public virtual IDictionary<WorkItemTrackingTreeService.ClassificationNodeId, TreeNode> GetTreeNodes(
      IVssRequestContext requestContext,
      IEnumerable<WorkItemTrackingTreeService.ClassificationNodeId> ids,
      bool includeDeleted,
      bool throwIfNotFound = true)
    {
      return (IDictionary<WorkItemTrackingTreeService.ClassificationNodeId, TreeNode>) requestContext.TraceBlock<Dictionary<WorkItemTrackingTreeService.ClassificationNodeId, TreeNode>>(904831, 904840, 904835, "Services", "WorkItemService", "WorkItemTreeService.GetTreeNodes", (Func<Dictionary<WorkItemTrackingTreeService.ClassificationNodeId, TreeNode>>) (() =>
      {
        ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
        ArgumentUtility.CheckForNull<IEnumerable<WorkItemTrackingTreeService.ClassificationNodeId>>(ids, nameof (ids));
        Dictionary<WorkItemTrackingTreeService.ClassificationNodeId, TreeNode> treeNodes = new Dictionary<WorkItemTrackingTreeService.ClassificationNodeId, TreeNode>();
        HashSet<WorkItemTrackingTreeService.ClassificationNodeId> classificationNodeIdSet = new HashSet<WorkItemTrackingTreeService.ClassificationNodeId>();
        WorkItemTrackingTreeService.TreeNodeCacheServiceImpl snapshot = this.GetSnapshot<WorkItemTrackingTreeService.TreeNodeCacheServiceImpl>(requestContext, false);
        foreach (WorkItemTrackingTreeService.ClassificationNodeId id in ids)
        {
          if (!treeNodes.ContainsKey(id) && !classificationNodeIdSet.Contains(id))
          {
            TreeNode node;
            if (snapshot.TryGetTreeNode(id.ProjectId, id.NodeId, out node))
              treeNodes[id] = node;
            else if (includeDeleted)
              classificationNodeIdSet.Add(id);
            else if (throwIfNotFound)
              throw new WorkItemTrackingTreeNodeNotFoundException(id.NodeId);
          }
        }
        if (classificationNodeIdSet.Any<WorkItemTrackingTreeService.ClassificationNodeId>() & includeDeleted)
        {
          StringComparer serverStringComparer = requestContext.GetService<WorkItemTrackingConfigurationSettingService>().GetConfigurationInfo(requestContext).ServerStringComparer;
          ICollection<TreeNodeRecord> classificationNodes;
          using (ClassificationNodeComponent component = requestContext.CreateComponent<ClassificationNodeComponent>())
            classificationNodes = (ICollection<TreeNodeRecord>) component.GetClassificationNodes((IEnumerable<WorkItemTrackingTreeService.ClassificationNodeId>) classificationNodeIdSet, includeDeleted);
          IDictionary<WorkItemTrackingTreeService.ClassificationNodeId, WorkItemTrackingTreeService.TreeNodeImpl> nodesMapById;
          WorkItemTrackingTreeService.BuildTree(requestContext, (IEnumerable<TreeNodeRecord>) null, (IEnumerable<TreeNodeRecord>) classificationNodes, out nodesMapById);
          foreach (WorkItemTrackingTreeService.ClassificationNodeId key in classificationNodeIdSet)
          {
            WorkItemTrackingTreeService.TreeNodeImpl treeNodeImpl;
            if (nodesMapById.TryGetValue(key, out treeNodeImpl))
              treeNodes[key] = (TreeNode) treeNodeImpl;
            else if (throwIfNotFound)
              throw new WorkItemTrackingTreeNodeNotFoundException(key.NodeId);
          }
        }
        return treeNodes;
      }));
    }

    public virtual Uri GetTreeNodeUriForPermissionCheck(
      IVssRequestContext requestContext,
      Guid projectId,
      int nodeId)
    {
      return requestContext.TraceBlock<Uri>(904721, 904730, 904725, "Services", "WorkItemService", "WorkItemTreeService.GetTreeNodeUriForPermissionCheck", (Func<Uri>) (() =>
      {
        ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
        ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
        return this.GetTreeNodeUriForPermissionCheckInternal(requestContext, projectId, nodeId);
      }));
    }

    protected virtual Uri GetTreeNodeUriForPermissionCheckInternal(
      IVssRequestContext requestContext,
      Guid projectId,
      int nodeId)
    {
      TreeNode node;
      bool treeNodeInternal = this.GetSnapshot<WorkItemTrackingTreeService.TreeNodeCacheServiceImpl>(requestContext, false).TryGetTreeNodeInternal(projectId, nodeId, out node);
      if (!treeNodeInternal)
        treeNodeInternal = this.GetSnapshot<WorkItemTrackingTreeService.TreeNodeCacheServiceImpl>(requestContext).TryGetTreeNodeInternal(projectId, nodeId, out node);
      if (!treeNodeInternal)
        return (Uri) null;
      if (node.IsProject)
      {
        node = node.Children.Values.FirstOrDefault<TreeNode>((Func<TreeNode, bool>) (x => x.Type == TreeStructureType.Area));
        if (node == null)
          return (Uri) null;
      }
      return node.Type != TreeStructureType.Area ? (Uri) null : node.Uri;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual TreeNode LegacyGetTreeNode(
      IVssRequestContext requestContext,
      Guid nodeId,
      bool throwIfNotFound = true)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      return this.GetTreeNodeInternal(requestContext, Guid.Empty, nodeId, throwIfNotFound);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual TreeNode LegacyGetTreeNode(
      IVssRequestContext requestContext,
      int nodeId,
      bool throwIfNotFound = true)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      return this.GetTreeNodeInternal(requestContext, Guid.Empty, nodeId, throwIfNotFound);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Uri LegacyGetTreeNodeUriForPermissionCheck(
      IVssRequestContext requestContext,
      int nodeId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      return this.GetTreeNodeUriForPermissionCheckInternal(requestContext, Guid.Empty, nodeId);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual IDictionary<int, TreeNode> LegacyGetTreeNodes(
      IVssRequestContext requestContext,
      IEnumerable<int> ids,
      bool includeDeleted,
      bool throwIfNotFound = true)
    {
      return (IDictionary<int, TreeNode>) requestContext.TraceBlock<Dictionary<int, TreeNode>>(904751, 904760, 904755, "Services", "WorkItemService", "WorkItemTreeService.LegacyGetTreeNodes", (Func<Dictionary<int, TreeNode>>) (() =>
      {
        ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
        ArgumentUtility.CheckForNull<IEnumerable<int>>(ids, nameof (ids));
        Dictionary<int, TreeNode> treeNodes = new Dictionary<int, TreeNode>();
        HashSet<int> source = new HashSet<int>();
        WorkItemTrackingTreeService.TreeNodeCacheServiceImpl snapshot = this.GetSnapshot<WorkItemTrackingTreeService.TreeNodeCacheServiceImpl>(requestContext, false);
        foreach (int id in ids)
        {
          if (!treeNodes.ContainsKey(id) && !source.Contains(id))
          {
            TreeNode node;
            if (snapshot.LegacyTryGetTreeNode(id, out node))
              treeNodes[id] = node;
            else if (includeDeleted)
            {
              source.Add(id);
            }
            else
            {
              if (throwIfNotFound)
                throw new WorkItemTrackingTreeNodeNotFoundException(id);
              treeNodes[id] = (TreeNode) null;
            }
          }
        }
        if (source.Any<int>() & includeDeleted)
        {
          StringComparer serverStringComparer = requestContext.GetService<WorkItemTrackingConfigurationSettingService>().GetConfigurationInfo(requestContext).ServerStringComparer;
          ICollection<TreeNodeRecord> classificationNodes;
          using (ClassificationNodeComponent component = requestContext.CreateComponent<ClassificationNodeComponent>())
            classificationNodes = (ICollection<TreeNodeRecord>) component.GetClassificationNodes(source.Select<int, WorkItemTrackingTreeService.ClassificationNodeId>((Func<int, WorkItemTrackingTreeService.ClassificationNodeId>) (x => new WorkItemTrackingTreeService.ClassificationNodeId()
            {
              NodeId = x
            })), includeDeleted);
          IDictionary<WorkItemTrackingTreeService.ClassificationNodeId, WorkItemTrackingTreeService.TreeNodeImpl> nodesMapById;
          WorkItemTrackingTreeService.BuildTree(requestContext, (IEnumerable<TreeNodeRecord>) null, (IEnumerable<TreeNodeRecord>) classificationNodes, out nodesMapById);
          Dictionary<int, WorkItemTrackingTreeService.TreeNodeImpl> dictionary = nodesMapById.ToDictionary<KeyValuePair<WorkItemTrackingTreeService.ClassificationNodeId, WorkItemTrackingTreeService.TreeNodeImpl>, int, WorkItemTrackingTreeService.TreeNodeImpl>((Func<KeyValuePair<WorkItemTrackingTreeService.ClassificationNodeId, WorkItemTrackingTreeService.TreeNodeImpl>, int>) (x => x.Key.NodeId), (Func<KeyValuePair<WorkItemTrackingTreeService.ClassificationNodeId, WorkItemTrackingTreeService.TreeNodeImpl>, WorkItemTrackingTreeService.TreeNodeImpl>) (x => x.Value));
          foreach (int num in source)
          {
            WorkItemTrackingTreeService.TreeNodeImpl treeNodeImpl;
            if (dictionary.TryGetValue(num, out treeNodeImpl))
            {
              treeNodes[num] = (TreeNode) treeNodeImpl;
            }
            else
            {
              if (throwIfNotFound)
                throw new WorkItemTrackingTreeNodeNotFoundException(num);
              treeNodes[num] = (TreeNode) null;
            }
          }
        }
        return treeNodes;
      }));
    }

    public virtual bool HasAreaPathPermissions(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      Guid projectId,
      int areaId,
      int workItemPermission)
    {
      ArgumentUtility.CheckForNull<Microsoft.VisualStudio.Services.Identity.Identity>(identity, nameof (identity));
      return this.HasProjectReadPermission(requestContext, identity, projectId) && this.DoesIdentityHaveAreaPathPermissions(requestContext, identity, areaId, workItemPermission);
    }

    private bool DoesIdentityHaveAreaPathPermissions(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      int areaId,
      int permission)
    {
      ArgumentUtility.CheckForNull<Microsoft.VisualStudio.Services.Identity.Identity>(identity, nameof (identity));
      IVssSecurityNamespace securityNamespace = requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, Microsoft.Azure.Boards.CssNodes.AuthorizationSecurityConstants.CommonStructureNodeSecurityGuid);
      string itemSecurityToken = PermissionCheckHelper.GetWorkItemSecurityToken(requestContext, areaId);
      return itemSecurityToken != null && (permission & securityNamespace.QueryEffectivePermissions(requestContext, itemSecurityToken, new EvaluationPrincipal(identity.Descriptor))) == permission;
    }

    private bool HasProjectReadPermission(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      Guid projectId)
    {
      IVssSecurityNamespace securityNamespace = requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, FrameworkSecurity.TeamProjectNamespaceId);
      string token = TeamProjectSecurityConstants.GetToken(CommonStructureUtils.GetProjectUri(projectId));
      return token != null && (1 & securityNamespace.QueryEffectivePermissions(requestContext, token, new EvaluationPrincipal(identity.Descriptor))) == 1;
    }

    private Guid NormalizeTreePath(
      IVssRequestContext requestContext,
      string fullPath,
      out string relativePath)
    {
      Guid guid = Guid.NewGuid();
      relativePath = string.Empty;
      if (!string.IsNullOrWhiteSpace(fullPath))
      {
        IProjectService service = requestContext.GetService<IProjectService>();
        string[] strArray = fullPath.Split(new char[1]
        {
          '\\'
        }, StringSplitOptions.RemoveEmptyEntries);
        if (strArray.Length != 0)
        {
          try
          {
            guid = service.GetProjectId(requestContext.Elevate(), strArray[0]);
            int num = fullPath.IndexOf(strArray[0]);
            relativePath = fullPath.Substring(num + strArray[0].Length);
          }
          catch
          {
          }
        }
      }
      return guid;
    }

    public void CreateNodes(
      IVssRequestContext requestContext,
      Guid projectId,
      ICollection<ClassificationNodeUpdate> nodes)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      ArgumentUtility.CheckForNull<ICollection<ClassificationNodeUpdate>>(nodes, nameof (nodes));
      requestContext.TraceBlock(908800, 908801, "Services", "TreeDictionary", nameof (CreateNodes), (Action) (() =>
      {
        if (nodes.Count == 0)
          return;
        requestContext.Trace(908802, TraceLevel.Verbose, "Services", "TreeDictionary", "Creating {0} nodes", (object) nodes.Count);
        Microsoft.TeamFoundation.Core.WebApi.ProjectInfo project = requestContext.GetService<IProjectService>().GetProject(requestContext, projectId);
        IDataspaceService service1 = requestContext.GetService<IDataspaceService>();
        if (service1.QueryDataspace(requestContext, "WorkItem", projectId, false) == null)
        {
          service1.CreateDataspaces(requestContext, new string[1]
          {
            "WorkItem"
          }, projectId);
          requestContext.Trace(908803, TraceLevel.Verbose, "Services", "TreeDictionary", "New dataspace created.");
        }
        Dictionary<Guid, ClassificationNodeUpdate> dictionary1 = new Dictionary<Guid, ClassificationNodeUpdate>();
        Dictionary<Guid, WorkItemTrackingTreeService.NodeInfo> parents = new Dictionary<Guid, WorkItemTrackingTreeService.NodeInfo>();
        ITreeDictionary snapshot = this.GetSnapshot(requestContext);
        ClassificationNodeUpdate classificationNodeUpdate1 = (ClassificationNodeUpdate) null;
        ClassificationNodeUpdate classificationNodeUpdate2 = (ClassificationNodeUpdate) null;
        bool flag1 = false;
        int num1 = 0;
        int num2 = 0;
        int num3 = WorkItemTrackingFeatureFlags.IsLimitAreaPathsEnabled(requestContext) ? 1 : 0;
        bool flag2 = WorkItemTrackingFeatureFlags.IsLimitIterationPathsEnabled(requestContext);
        if ((num3 | (flag2 ? 1 : 0)) != 0)
        {
          Dictionary<TreeStructureType, int> dictionary2 = nodes.GroupBy<ClassificationNodeUpdate, TreeStructureType>((Func<ClassificationNodeUpdate, TreeStructureType>) (node => node.StructureType)).ToDictionary<IGrouping<TreeStructureType, ClassificationNodeUpdate>, TreeStructureType, int>((Func<IGrouping<TreeStructureType, ClassificationNodeUpdate>, TreeStructureType>) (x => x.Key), (Func<IGrouping<TreeStructureType, ClassificationNodeUpdate>, int>) (x => x.Count<ClassificationNodeUpdate>()));
          dictionary2.TryGetValue(TreeStructureType.Area, out num1);
          dictionary2.TryGetValue(TreeStructureType.Iteration, out num2);
        }
        if (num3 != 0 && num1 > 0)
        {
          int areaPathLimit = WorkItemTrackingTreeService.GetProjectAreaPathLimit(requestContext);
          int areasCount = snapshot.GetTreeNodeCount(projectId, TreeStructureType.Area);
          if (areasCount + num1 > areaPathLimit)
          {
            requestContext.TraceConditionally(908834, TraceLevel.Verbose, "Services", "TreeDictionary", (Func<string>) (() => string.Format("projectID: {0} has {1} areas, which is over limit of {2}", (object) projectId, (object) areasCount, (object) areaPathLimit)));
            throw new ClassificationNodeTooManyAreaPaths(areaPathLimit);
          }
        }
        if (flag2 && num2 > 0)
        {
          int iterationPathLimit = WorkItemTrackingTreeService.GetProjectIterationPathLimit(requestContext);
          int iterationsCount = snapshot.GetTreeNodeCount(projectId, TreeStructureType.Iteration);
          if (iterationsCount + num2 > iterationPathLimit)
          {
            requestContext.TraceConditionally(908835, TraceLevel.Verbose, "Services", "TreeDictionary", (Func<string>) (() => string.Format("projectID: {0} has {1} iterations, which is over limit of {2}", (object) projectId, (object) iterationsCount, (object) iterationPathLimit)));
            throw new ClassificationNodeTooManyIterationPaths(iterationPathLimit);
          }
        }
        foreach (ClassificationNodeUpdate node1 in (IEnumerable<ClassificationNodeUpdate>) nodes)
        {
          requestContext.Trace(908804, TraceLevel.Verbose, "Services", "TreeDictionary", "Identifier: {0}, Parent: {1}, Name: {2}, Type: {3}", (object) node1.Identifier, (object) node1.ParentIdentifier, (object) node1.Name, (object) node1.StructureType);
          if (node1.Identifier == Guid.Empty)
            throw new ClassificationNodeEmptyIdentifierException();
          if (dictionary1.ContainsKey(node1.Identifier))
            throw new ClassificationNodeDuplicateIdentifierException(node1.Identifier);
          dictionary1.Add(node1.Identifier, node1);
          TreeNode node2;
          if (snapshot.TryGetTreeNode(projectId, node1.Identifier, out node2))
            throw new ClassificationNodeDuplicateIdentifierException(node1.Identifier);
          node1.Name = CommonStructureUtils.NormalizeNodeName(node1.Name, "nodeName");
          if (node1.StructureType != TreeStructureType.Area && node1.StructureType != TreeStructureType.Iteration)
            throw new ClassificationNodeInvalidStructureTypeException(node1.Identifier);
          CommonStructureUtils.ValidateIterationDates(node1.StartDate, node1.FinishDate);
          if (node1.StartDate.HasValue && node1.StructureType != TreeStructureType.Iteration)
            throw new ClassificationNodeCannotAddDateToNonIterationException(node1.Identifier);
          Lazy<IEqualityComparer<string>> lazy = new Lazy<IEqualityComparer<string>>((Func<IEqualityComparer<string>>) (() => (IEqualityComparer<string>) requestContext.WitContext().ServerSettings.ServerStringComparer));
          if (node1.ParentIdentifier == Guid.Empty)
          {
            if (!flag1)
            {
              this.CheckCreateProjectPermission(requestContext);
              if (project.State != ProjectState.New)
                throw new ClassificationNodeCannotCreateRootNodeAfterProjectCreationException(projectId);
              flag1 = true;
            }
            if (snapshot.TryGetTreeNode(projectId, projectId, out node2))
              throw new ClassificationNodeCannotCreateRootNodeAfterProjectCreationException(projectId);
            if (node1.StructureType == TreeStructureType.Area)
              classificationNodeUpdate1 = classificationNodeUpdate1 == null ? node1 : throw new ClassificationNodeTooManyRootNodeException();
            else
              classificationNodeUpdate2 = classificationNodeUpdate2 == null ? node1 : throw new ClassificationNodeTooManyRootNodeException();
            WorkItemTrackingTreeService.NodeInfo nodeInfo;
            if (!parents.TryGetValue(node1.Identifier, out nodeInfo))
            {
              nodeInfo = new WorkItemTrackingTreeService.NodeInfo(node1, lazy.Value);
              parents[node1.Identifier] = nodeInfo;
              requestContext.Trace(908805, TraceLevel.Verbose, "Services", "TreeDictionary", "Node info object added for identifier {0}", (object) node1.Identifier);
            }
          }
          else
          {
            if (node1.ParentIdentifier == node1.Identifier)
              throw new ClassificationNodeCircularNodeReferenceException(node1.Identifier);
            WorkItemTrackingTreeService.NodeInfo nodeInfo1 = (WorkItemTrackingTreeService.NodeInfo) null;
            TreeStructureType treeStructureType = TreeStructureType.None;
            ClassificationNodeUpdate update;
            if (dictionary1.TryGetValue(node1.ParentIdentifier, out update))
            {
              requestContext.Trace(908806, TraceLevel.Verbose, "Services", "TreeDictionary", "Parent is in the same update batch");
              treeStructureType = update.StructureType;
              if (!parents.TryGetValue(node1.ParentIdentifier, out nodeInfo1))
              {
                nodeInfo1 = new WorkItemTrackingTreeService.NodeInfo(update, lazy.Value);
                parents[node1.ParentIdentifier] = nodeInfo1;
                requestContext.Trace(908807, TraceLevel.Verbose, "Services", "TreeDictionary", "Node info object added for parent identifier {0}", (object) node1.ParentIdentifier);
              }
            }
            if (nodeInfo1 == null)
            {
              TreeNode node3;
              if (!snapshot.TryGetTreeNode(projectId, node1.ParentIdentifier, out node3) || node3.IsProject)
                throw new ClassificationNodeParentNodeDoesNotExistException(node1.ParentIdentifier);
              this.CheckNodePermission(requestContext, node3, 4);
              treeStructureType = node3.Type;
              if (!parents.TryGetValue(node1.ParentIdentifier, out nodeInfo1))
              {
                nodeInfo1 = new WorkItemTrackingTreeService.NodeInfo(node3, lazy.Value);
                parents[node1.ParentIdentifier] = nodeInfo1;
                requestContext.Trace(908808, TraceLevel.Verbose, "Services", "TreeDictionary", "Node info object added for parent identifier {0}", (object) node1.ParentIdentifier);
              }
            }
            if (treeStructureType != node1.StructureType)
              throw new ClassificationNodeCannotChangeStructureTypeException(node1.Identifier, node1.ParentIdentifier);
            if (nodeInfo1.ChildNames.Contains(node1.Name))
              throw new ClassificationNodeDuplicateNameException(node1.ParentIdentifier, node1.Name);
            nodeInfo1.ChildNames.Add(node1.Name);
            WorkItemTrackingTreeService.NodeInfo nodeInfo2;
            if (!parents.TryGetValue(node1.Identifier, out nodeInfo2))
            {
              nodeInfo2 = new WorkItemTrackingTreeService.NodeInfo(node1, lazy.Value);
              parents[node1.Identifier] = nodeInfo2;
              requestContext.Trace(908809, TraceLevel.Verbose, "Services", "TreeDictionary", "Node info object added for identifier {0}", (object) node1.Identifier);
            }
            nodeInfo2.Parent = nodeInfo1;
          }
        }
        bool flag3 = false;
        List<ClassificationNodeUpdate> nodes1 = new List<ClassificationNodeUpdate>(nodes.Count + 1);
        foreach (ClassificationNodeUpdate node in (IEnumerable<ClassificationNodeUpdate>) nodes)
        {
          if (parents[node.Identifier].Depth > 14)
            throw new ClassificationNodeMaximumDepthExceededException(node.Identifier);
          if (node.ParentIdentifier != Guid.Empty)
          {
            nodes1.Add(node);
          }
          else
          {
            nodes1.Add(new ClassificationNodeUpdate()
            {
              Identifier = node.Identifier,
              Name = node.Name,
              StructureType = node.StructureType,
              ParentIdentifier = projectId,
              StartDate = node.StartDate,
              FinishDate = node.FinishDate
            });
            if (!flag3)
            {
              nodes1.Add(new ClassificationNodeUpdate()
              {
                Identifier = projectId,
                Name = projectId.ToString(),
                StructureType = TreeStructureType.None
              });
              requestContext.Trace(908810, TraceLevel.Verbose, "Services", "TreeDictionary", "Project node {0} added", (object) projectId);
              flag3 = true;
            }
          }
        }
        Guid id = requestContext.WitContext().RequestIdentity.Id;
        requestContext.Trace(900427, TraceLevel.Info, "ResourceComponent", "TreeDictionary", string.Format("RequestContext identity VSID: {0}", (object) id));
        IEnumerable<ClassificationNodeUpdateError> result = (IEnumerable<ClassificationNodeUpdateError>) null;
        int nodeSequenceId;
        using (ClassificationNodeComponent component = requestContext.CreateComponent<ClassificationNodeComponent>())
          result = component.CreateClassificationNodes(projectId, (IEnumerable<ClassificationNodeUpdate>) nodes1, id, out nodeSequenceId);
        requestContext.ResetMetadataDbStamps();
        int num4 = (int) WorkItemTrackingTreeService.ProcessNodeUpdateResult(requestContext, result, (Func<int, Guid, ClassificationNodeDuplicateNameException>) ((nodeIntegerId, nodeGuidId) =>
        {
          ClassificationNodeUpdate classificationNodeUpdate3 = nodes.First<ClassificationNodeUpdate>((Func<ClassificationNodeUpdate, bool>) (x => x.Identifier == nodeGuidId));
          return new ClassificationNodeDuplicateNameException(classificationNodeUpdate3.ParentIdentifier, classificationNodeUpdate3.Name);
        }), (Func<int, ClassificationNodeReclassifiedToSubTreeException>) null);
        this.InvalidateCache(requestContext);
        foreach (ClassificationNodeUpdate classificationNodeUpdate4 in (IEnumerable<ClassificationNodeUpdate>) nodes.OrderBy<ClassificationNodeUpdate, int>((Func<ClassificationNodeUpdate, int>) (x => parents[x.Identifier].Depth)))
        {
          try
          {
            requestContext.GetExtension<IAuthorizationProviderFactory>().RegisterObject(requestContext, requestContext.UserContext, CommonStructureUtils.GetNodeUri(classificationNodeUpdate4.Identifier), classificationNodeUpdate4.StructureType == TreeStructureType.Area ? "CSS_NODE" : "ITERATION_NODE", CommonStructureUtils.GetProjectUri(projectId), classificationNodeUpdate4.ParentIdentifier != Guid.Empty ? CommonStructureUtils.GetNodeUri(classificationNodeUpdate4.ParentIdentifier) : (string) null);
          }
          catch (RegisterObjectExistsException ex)
          {
          }
          requestContext.Trace(908811, TraceLevel.Verbose, "Services", "TreeDictionary", "Node {0} is registered", (object) classificationNodeUpdate4.Identifier);
        }
        ITeamFoundationEventService service2 = requestContext.GetService<ITeamFoundationEventService>();
        List<WorkItemClassificationNodeChangedEvent> eventsForCreatedNodes = (this.EventCreator ?? new WorkItemTrackingTreeService.NotificationEventCreator(this)).CreateNotificationEventsForCreatedNodes(requestContext, project.Id, nodes);
        if (WorkItemTrackingFeatureFlags.IsRicherClassificationNodeChangeEventEnabled(requestContext))
          service2.PublishNotification(requestContext, (object) new WorkItemClassificationNodesChangedNotification()
          {
            Events = (IEnumerable<WorkItemClassificationNodeChangedEvent>) eventsForCreatedNodes
          });
        if (nodeSequenceId >= 0)
          service2.PublishNotification(requestContext, (object) new StructureChangedNotification(nodeSequenceId));
        this.QueueReclassificationJobInTaskService(requestContext);
        this.TryFireServiceBusNotificationForClassificationNodeChange(requestContext, eventsForCreatedNodes);
      }));
    }

    public void UpdateNode(
      IVssRequestContext requestContext,
      Guid projectId,
      int nodeId,
      string newName,
      int? newParentId,
      WorkItemTrackingTreeService.IterationDates dates,
      bool force = false)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      requestContext.TraceBlock(908812, 908813, "Services", "TreeDictionary", nameof (UpdateNode), (Action) (() =>
      {
        requestContext.Trace(908814, TraceLevel.Verbose, "Services", "TreeDictionary", "Node: {0}, newName: {1}, newParent: {2}, Start: {3}, Finish: {4}", (object) nodeId, (object) (newName ?? string.Empty), (object) newParentId.GetValueOrDefault(), (object) (dates != null ? dates.StartDate : new DateTime?(DateTime.MinValue)), (object) (dates != null ? dates.FinishDate : new DateTime?(DateTime.MinValue)));
        requestContext.GetService<IProjectService>().GetProject(requestContext, projectId);
        ITreeDictionary snapshot = this.GetSnapshot(requestContext);
        TreeNode node = snapshot.GetTreeNode(projectId, nodeId);
        bool flag1 = false;
        bool flag2 = false;
        if (newName != null || newParentId.HasValue || dates != null)
        {
          this.CheckNodePermission(requestContext, node, 2);
          WorkItemTrackingTreeService.CheckNodeForUpdateEligibility(node, newName != null || newParentId.HasValue);
        }
        if (newName != null)
        {
          newName = CommonStructureUtils.NormalizeNodeName(newName, nameof (newName));
          if (!string.Equals(node.GetName((IVssRequestContext) null), newName, StringComparison.Ordinal))
          {
            flag1 = true;
            TreeNode treeNode;
            if (!newParentId.HasValue && node.Parent.Children.TryGetValue(newName, out treeNode) && treeNode.Id != nodeId)
              throw new ClassificationNodeDuplicateNameException(node.Parent.Id, newName);
          }
        }
        TreeNode node1 = (TreeNode) null;
        if (newParentId.HasValue && newParentId.Value != node.Parent.Id)
        {
          flag2 = true;
          if (!snapshot.TryGetTreeNode(projectId, newParentId.Value, out node1) || node1.IsProject)
            throw new ClassificationNodeParentNodeDoesNotExistException(newParentId.Value);
          this.CheckNodePermission(requestContext, node1, 4);
          if (node1.Type != node.Type)
            throw new ClassificationNodeCannotChangeStructureTypeException(nodeId, newParentId.Value);
          string str = newName ?? node.GetName((IVssRequestContext) null);
          if (node1.Children.ContainsKey(str))
            throw new ClassificationNodeDuplicateNameException(newParentId.Value, str);
          for (TreeNode treeNode = node1; treeNode != null; treeNode = treeNode.Parent)
          {
            if (treeNode.Id == nodeId)
              throw new ClassificationNodeCircularNodeReferenceException(nodeId);
          }
          int subTreeDepth = WorkItemTrackingTreeService.ComputeSubTreeDepth(node);
          if (node1.TypeId - subTreeDepth < -56)
            throw new ClassificationNodeMaximumDepthExceededException(nodeId);
        }
        bool setDates = WorkItemTrackingTreeService.ValidateIterationDates(node, dates);
        requestContext.Trace(908815, TraceLevel.Verbose, "Services", "TreeDictionary", "NameChanged: {0}, ParentChanged: {1}, SetDates: {2}, Force: {3}", (object) flag1, (object) flag2, (object) setDates, (object) force);
        if (!(flag1 | flag2 | setDates | force))
          return;
        ClassificationNodeUpdateError classificationNodeUpdateError = (ClassificationNodeUpdateError) null;
        Guid id = requestContext.WitContext().RequestIdentity.Id;
        int reclassificationTimeout = requestContext.WitContext().ServerSettings.WorkItemReclassificationTimeout;
        int nodeSequenceId;
        using (ClassificationNodeComponent component = requestContext.CreateComponent<ClassificationNodeComponent>())
          classificationNodeUpdateError = component.UpdateClassificationNode(projectId, id, nodeId, newName, newParentId, dates != null ? dates.StartDate : new DateTime?(), dates != null ? dates.FinishDate : new DateTime?(), setDates, reclassificationTimeout, force, out nodeSequenceId);
        requestContext.ResetMetadataDbStamps();
        if (classificationNodeUpdateError != null)
        {
          int num = (int) WorkItemTrackingTreeService.ProcessNodeUpdateResult(requestContext, (IEnumerable<ClassificationNodeUpdateError>) new ClassificationNodeUpdateError[1]
          {
            classificationNodeUpdateError
          }, (Func<int, Guid, ClassificationNodeDuplicateNameException>) ((nodeIntegerId, nodeGuidId) => new ClassificationNodeDuplicateNameException(newParentId ?? node.ParentId, newName ?? node.GetName((IVssRequestContext) null))), (Func<int, ClassificationNodeReclassifiedToSubTreeException>) null);
        }
        this.InvalidateCache(requestContext);
        if (flag2)
        {
          requestContext.GetExtension<IAuthorizationProviderFactory>().ResetInheritance(requestContext, node.Uri.AbsoluteUri, node1.Uri.AbsoluteUri);
          requestContext.Trace(908816, TraceLevel.Verbose, "Services", "TreeDictionary", "Reparented the object");
        }
        ITeamFoundationEventService service = requestContext.GetService<ITeamFoundationEventService>();
        List<WorkItemClassificationNodeChangedEvent> eventsForUpdatedNode = (this.EventCreator ?? new WorkItemTrackingTreeService.NotificationEventCreator(this)).CreateNotificationEventsForUpdatedNode(requestContext, projectId, node, flag1 ? newName : (string) null, setDates ? dates : (WorkItemTrackingTreeService.IterationDates) null, flag2 ? node1 : (TreeNode) null);
        if (WorkItemTrackingFeatureFlags.IsRicherClassificationNodeChangeEventEnabled(requestContext))
          service.PublishNotification(requestContext, (object) new WorkItemClassificationNodesChangedNotification()
          {
            Events = (IEnumerable<WorkItemClassificationNodeChangedEvent>) eventsForUpdatedNode
          });
        if (nodeSequenceId > 0)
          service.PublishNotification(requestContext, (object) new StructureChangedNotification(nodeSequenceId));
        this.QueueReclassificationJobInTaskService(requestContext);
        this.TryFireServiceBusNotificationForClassificationNodeChange(requestContext, eventsForUpdatedNode);
      }));
    }

    private static ClassificationNodeType GetNodeTypeFrom(TreeStructureType structureType) => structureType != TreeStructureType.Area ? ClassificationNodeType.Iteration : ClassificationNodeType.Area;

    private static void CheckNodeForUpdateEligibility(
      TreeNode node,
      bool prohibitStructureSpecifier)
    {
      if (node.IsProject || node.IsStructureSpecifier & prohibitStructureSpecifier)
        throw new ClassificationNodeCannotModifyRootNodeException(node.Id);
    }

    private static int ComputeSubTreeDepth(TreeNode node)
    {
      int num = 0;
      if (node.HasChildren)
        num = node.Children.Values.Select<TreeNode, int>((Func<TreeNode, int>) (x => WorkItemTrackingTreeService.ComputeSubTreeDepth(x))).Max();
      return num + 1;
    }

    private static bool ValidateIterationDates(
      TreeNode node,
      WorkItemTrackingTreeService.IterationDates dates)
    {
      if (dates == null)
        return false;
      if (node.Type != TreeStructureType.Iteration)
        throw new ClassificationNodeCannotAddDateToNonIterationException(node.Id);
      CommonStructureUtils.ValidateIterationDates(dates.StartDate, dates.FinishDate);
      DateTime? nullable1 = dates.StartDate;
      DateTime? nullable2 = node.StartDate;
      if ((nullable1.HasValue == nullable2.HasValue ? (nullable1.HasValue ? (nullable1.GetValueOrDefault() != nullable2.GetValueOrDefault() ? 1 : 0) : 0) : 1) != 0)
        return true;
      nullable2 = dates.FinishDate;
      nullable1 = node.FinishDate;
      if (nullable2.HasValue != nullable1.HasValue)
        return true;
      return nullable2.HasValue && nullable2.GetValueOrDefault() != nullable1.GetValueOrDefault();
    }

    public virtual void MarkProjectNodeDeletionStatus(
      IVssRequestContext requestContext,
      Guid projectId,
      bool deleted)
    {
      CustomerIntelligenceData properties = new CustomerIntelligenceData();
      properties.Add("ProjectId", (object) projectId);
      properties.Add("DeletionStatus", deleted);
      requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, nameof (WorkItemTrackingTreeService), nameof (MarkProjectNodeDeletionStatus), properties);
      Guid id = requestContext.WitContext().RequestIdentity.Id;
      int nodeSequenceId;
      using (ClassificationNodeComponent component = requestContext.CreateComponent<ClassificationNodeComponent>())
        component.MarkRootNodeDeletionStatus(projectId, id, deleted, out nodeSequenceId);
      requestContext.ResetMetadataDbStamps();
      this.InvalidateCache(requestContext);
      if (nodeSequenceId <= 0)
        return;
      requestContext.GetService<ITeamFoundationEventService>().PublishNotification(requestContext, (object) new StructureChangedNotification(nodeSequenceId));
    }

    public void DeleteNodes(
      IVssRequestContext requestContext,
      Guid projectId,
      ICollection<ClassificationNodeUpdate> nodes)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      ArgumentUtility.CheckForNull<ICollection<ClassificationNodeUpdate>>(nodes, nameof (nodes));
      requestContext.TraceBlock(908817, 908818, "Services", "TreeDictionary", "UpdateNode", (Action) (() =>
      {
        if (nodes.Count == 0)
          return;
        Microsoft.TeamFoundation.Core.WebApi.ProjectInfo project = requestContext.GetService<IProjectService>().GetProject(requestContext, projectId);
        Dictionary<Guid, TreeNode> deletedNodeToReclassifyNodeMapping = new Dictionary<Guid, TreeNode>();
        List<TreeNode> deletedNodes = new List<TreeNode>();
        ITreeDictionary snapshot = this.GetSnapshot(requestContext);
        foreach (ClassificationNodeUpdate node1 in (IEnumerable<ClassificationNodeUpdate>) nodes)
        {
          requestContext.Trace(908819, TraceLevel.Verbose, "Services", "TreeDictionary", "Deleting node {0}", (object) node1.Id);
          TreeNode treeNode1 = snapshot.GetTreeNode(projectId, node1.Id);
          WorkItemTrackingTreeService.CheckNodeForUpdateEligibility(treeNode1, true);
          this.CheckNodePermission(requestContext, treeNode1, 8);
          if (deletedNodeToReclassifyNodeMapping.ContainsKey(treeNode1.CssNodeId))
            throw new ClassificationNodeDuplicateDeletedNodeException(node1.Id);
          deletedNodeToReclassifyNodeMapping[treeNode1.CssNodeId] = (TreeNode) null;
          if (node1.ReclassifyId > 0)
          {
            TreeNode node2;
            if (!snapshot.TryGetTreeNode(projectId, node1.ReclassifyId, out node2) || node2.IsProject)
              throw new ClassificationNodeReclassificationNodeDoesNotExistException(node1.ReclassifyId);
            this.CheckNodePermission(requestContext, node2, 2);
            if (node2.Type != treeNode1.Type)
              throw new ClassificationNodeReclassifiedToDifferentStructureTypeException(node1.Id, node1.ReclassifyId);
            for (TreeNode treeNode2 = node2; treeNode2 != null; treeNode2 = treeNode2.Parent)
            {
              if (treeNode2.Id == node1.Id)
                throw new ClassificationNodeReclassifiedToSubTreeException(node1.Id, node1.ReclassifyId);
            }
            if (node2.IsStructureSpecifier)
              node2 = node2.Parent;
            deletedNodeToReclassifyNodeMapping[treeNode1.CssNodeId] = node2;
            deletedNodes.Add(treeNode1);
            node1.ReclassifyId = node2.Id;
          }
          else if (project.State != ProjectState.Deleting)
            throw new ClassificationNodeReclassificationNodeDoesNotExistException(node1.ReclassifyId);
        }
        TreeNode treeNode = deletedNodeToReclassifyNodeMapping.Values.FirstOrDefault<TreeNode>((Func<TreeNode, bool>) (x => deletedNodeToReclassifyNodeMapping.ContainsKey(x.CssNodeId)));
        if (treeNode != null)
          throw new ClassificationCannotDeleteReclassifyNodeException(treeNode.Id);
        IEnumerable<ClassificationNodeUpdateError> result = (IEnumerable<ClassificationNodeUpdateError>) null;
        Guid id = requestContext.WitContext().RequestIdentity.Id;
        int reclassificationTimeout = requestContext.WitContext().ServerSettings.WorkItemReclassificationTimeout;
        int nodeSequenceId;
        using (ClassificationNodeComponent component = requestContext.CreateComponent<ClassificationNodeComponent>())
          result = component.DeleteClassificationNodes(projectId, id, (IEnumerable<ClassificationNodeUpdate>) nodes, reclassificationTimeout, out nodeSequenceId);
        requestContext.ResetMetadataDbStamps();
        int num = (int) WorkItemTrackingTreeService.ProcessNodeUpdateResult(requestContext, result, (Func<int, Guid, ClassificationNodeDuplicateNameException>) null, (Func<int, ClassificationNodeReclassifiedToSubTreeException>) (nodeId => new ClassificationNodeReclassifiedToSubTreeException(nodeId, nodes.First<ClassificationNodeUpdate>((Func<ClassificationNodeUpdate, bool>) (x => x.Id == nodeId)).ReclassifyId)));
        this.InvalidateCache(requestContext);
        if (WorkItemTrackingFeatureFlags.IsCleaningACEsOnDeletingNodesEnabled(requestContext))
          this.CleanupNodeSecurityEntries(requestContext, deletedNodes);
        ITeamFoundationEventService service = requestContext.GetService<ITeamFoundationEventService>();
        List<WorkItemClassificationNodeChangedEvent> eventsForDeletedNodes = (this.EventCreator ?? new WorkItemTrackingTreeService.NotificationEventCreator(this)).CreateNotificationEventsForDeletedNodes(requestContext, projectId, nodes, snapshot);
        if (WorkItemTrackingFeatureFlags.IsRicherClassificationNodeChangeEventEnabled(requestContext))
          service.PublishNotification(requestContext, (object) new WorkItemClassificationNodesChangedNotification()
          {
            Events = (IEnumerable<WorkItemClassificationNodeChangedEvent>) eventsForDeletedNodes
          });
        if (nodeSequenceId > 0)
          service.PublishNotification(requestContext, (object) new StructureChangedNotification(nodeSequenceId));
        this.QueueReclassificationJobInTaskService(requestContext);
        this.TryFireServiceBusNotificationForClassificationNodeChange(requestContext, eventsForDeletedNodes);
      }));
    }

    protected void CleanupNodeSecurityEntries(
      IVssRequestContext requestContext,
      List<TreeNode> deletedNodes)
    {
      foreach (TreeNode deletedNode in deletedNodes)
      {
        try
        {
          Guid guid = deletedNode.Type == TreeStructureType.Area ? Microsoft.Azure.Boards.CssNodes.AuthorizationSecurityConstants.CommonStructureNodeSecurityGuid : Microsoft.Azure.Boards.CssNodes.AuthorizationSecurityConstants.IterationNodeSecurityGuid;
          string tokenFromNode = this.GetTokenFromNode(deletedNode);
          requestContext.GetExtension<IAuthorizationProviderFactory>().UnregisterObject(requestContext, deletedNode.Uri.AbsoluteUri, tokenFromNode, new Guid?(guid));
          requestContext.Trace(908820, TraceLevel.Verbose, "Services", "TreeDictionary", "Unregistered node {0}", (object) deletedNode.CssNodeId);
        }
        catch (SecurityObjectDoesNotExistException ex)
        {
        }
      }
    }

    public string GetTokenFromNode(TreeNode node)
    {
      StringBuilder stringBuilder = new StringBuilder();
      for (; !node.IsProject; node = node.Parent)
      {
        if (stringBuilder.Length > 0)
          stringBuilder.Append(Microsoft.Azure.Boards.CssNodes.AuthorizationSecurityConstants.SeparatorChar);
        char[] charArray = node.Uri.AbsoluteUri.ToCharArray();
        Array.Reverse((Array) charArray);
        stringBuilder.Append(charArray);
      }
      char[] charArray1 = stringBuilder.ToString().ToCharArray();
      Array.Reverse((Array) charArray1);
      return new string(charArray1);
    }

    private void TryFireServiceBusNotificationForClassificationNodeChange(
      IVssRequestContext requestContext,
      List<WorkItemClassificationNodeChangedEvent> events)
    {
      if (!requestContext.ExecutionEnvironment.IsHostedDeployment || !requestContext.IsFeatureEnabled("WorkItemTracking.Server.CssChangeServiceBusNotification"))
        return;
      requestContext.To(TeamFoundationHostType.Deployment).GetService<TeamFoundationTaskService>().AddTask(requestContext, (TeamFoundationTaskCallback) ((requestCtx, ignored) =>
      {
        try
        {
          ServiceBusPublisher.PublishToServiceBus(requestCtx, "Microsoft.TeamFoundation.WorkItemTracking.Server", "classification.node.changed", (object) new WorkItemClassificationNodesChangedNotification()
          {
            Events = (IEnumerable<WorkItemClassificationNodeChangedEvent>) events
          });
        }
        catch (Exception ex)
        {
          TeamFoundationEventLog.Default.LogException("Failed to send Create CSS nodes event to message bus [Microsoft.TeamFoundation.WorkItemTracking.Server]", ex);
        }
      }));
    }

    internal override void InvalidateCache(IVssRequestContext requestContext)
    {
      base.InvalidateCache(requestContext);
      this.UpdateReclassificationStatus(requestContext, true, this.m_pendingReclassificationStamp);
      this.QueueUpdateReclassificationStatusTask(requestContext);
    }

    internal virtual bool HasPendingReclassification { private set; get; }

    private void UpdateReclassificationStatusTaskCallback(
      IVssRequestContext requestContext,
      object args)
    {
      requestContext.TraceBlock(904761, 904770, 904765, "Services", "WorkItemService", "WorkItemTreeService.UpdateReclassificationStatusTaskCallback", (Action) (() =>
      {
        long reclassificationStamp = this.m_pendingReclassificationStamp;
        bool hasPendingReclassification;
        using (ClassificationNodeComponent component = requestContext.CreateComponent<ClassificationNodeComponent>())
          hasPendingReclassification = component.GetClassificationNodeChanges(true, out long _).Any<ClassificationNodeChange>();
        this.UpdateReclassificationStatus(requestContext, hasPendingReclassification, reclassificationStamp);
      }));
    }

    private void QueueUpdateReclassificationStatusTask(IVssRequestContext requestContext) => requestContext.TraceBlock(904771, 904780, 904775, "Services", "WorkItemService", "WorkItemTreeService.QueueUpdateReclassificationStatusTask", (Action) (() => requestContext.To(TeamFoundationHostType.Deployment).GetService<TeamFoundationTaskService>().AddTask(requestContext, new TeamFoundationTask(new TeamFoundationTaskCallback(this.UpdateReclassificationStatusTaskCallback), (object) null, DateTime.UtcNow.AddSeconds(1.0), requestContext.WitContext().ServerSettings.WorkItemReclassificationStatusCheckInterval * 1000))));

    internal IEnumerable<TreeNode> GetTreeNodePendingReclassification(
      IVssRequestContext requestContext,
      Guid? projectId)
    {
      long reclassificationStamp = this.m_pendingReclassificationStamp;
      IEnumerable<ClassificationNodeChange> classificationNodeChanges;
      using (ClassificationNodeComponent component = requestContext.CreateComponent<ClassificationNodeComponent>())
        classificationNodeChanges = component.GetClassificationNodeChanges(true, out long _);
      bool hasPendingReclassification = classificationNodeChanges.Any<ClassificationNodeChange>();
      this.UpdateReclassificationStatus(requestContext, hasPendingReclassification, reclassificationStamp);
      Lazy<ITreeDictionary> snapshot = new Lazy<ITreeDictionary>((Func<ITreeDictionary>) (() => this.GetSnapshot(requestContext)));
      foreach (ClassificationNodeChange classificationNodeChange in classificationNodeChanges)
      {
        TreeNode node;
        if (classificationNodeChange.Id > 0 && (!projectId.HasValue || classificationNodeChange.ProjectId == projectId.Value) && snapshot.Value.TryGetTreeNode(classificationNodeChange.ProjectId, classificationNodeChange.Id, out node))
          yield return node;
      }
    }

    internal virtual bool HasNodePermission(
      IVssRequestContext requestContext,
      TreeNode node,
      int requiredPermission)
    {
      return requestContext.TraceBlock<bool>(904731, 904740, 904735, "Services", "WorkItemService", "WorkItemTreeService.HasNodePermission", (Func<bool>) (() =>
      {
        IVssSecurityNamespace nameSpaceForNode = this.GetSecurityNameSpaceForNode(requestContext, node);
        return nameSpaceForNode.HasPermission(requestContext, nameSpaceForNode.NamespaceExtension.HandleIncomingToken(requestContext, nameSpaceForNode, node.Uri.AbsoluteUri), requiredPermission);
      }));
    }

    private void UpdateReclassificationStatus(
      IVssRequestContext requestContext,
      bool hasPendingReclassification,
      long stamp)
    {
      bool flag = true;
      lock (this)
      {
        if (stamp == this.m_pendingReclassificationStamp)
        {
          this.HasPendingReclassification = hasPendingReclassification;
          ++this.m_pendingReclassificationStamp;
          flag = false;
        }
      }
      if (!flag)
        return;
      this.QueueUpdateReclassificationStatusTask(requestContext);
    }

    private void CheckCreateProjectPermission(IVssRequestContext requestContext) => requestContext.GetService<IntegrationSecurityManager>().CheckGlobalPermission(requestContext, "CREATE_PROJECTS");

    private void CheckNodePermission(
      IVssRequestContext requestContext,
      TreeNode node,
      int requiredPermission)
    {
      requestContext.TraceBlock(904731, 904740, 904735, "Services", "WorkItemService", "WorkItemTreeService.CheckNodePermission", (Action) (() =>
      {
        IVssSecurityNamespace nameSpaceForNode = this.GetSecurityNameSpaceForNode(requestContext, node);
        nameSpaceForNode.CheckPermission(requestContext, nameSpaceForNode.NamespaceExtension.HandleIncomingToken(requestContext, nameSpaceForNode, node.Uri.AbsoluteUri), requiredPermission);
      }));
    }

    private IVssSecurityNamespace GetSecurityNameSpaceForNode(
      IVssRequestContext requestContext,
      TreeNode node)
    {
      ITeamFoundationSecurityService service = requestContext.GetService<ITeamFoundationSecurityService>();
      Guid guid = node.Type == TreeStructureType.Area ? Microsoft.Azure.Boards.CssNodes.AuthorizationSecurityConstants.CommonStructureNodeSecurityGuid : Microsoft.Azure.Boards.CssNodes.AuthorizationSecurityConstants.IterationNodeSecurityGuid;
      IVssRequestContext requestContext1 = requestContext;
      Guid namespaceId = guid;
      return service.GetSecurityNamespace(requestContext1, namespaceId);
    }

    private static WorkItemClassificationReconciliationState ProcessNodeUpdateResult(
      IVssRequestContext requestContext,
      IEnumerable<ClassificationNodeUpdateError> result,
      Func<int, Guid, ClassificationNodeDuplicateNameException> duplicateNameExceptionConstructor,
      Func<int, ClassificationNodeReclassifiedToSubTreeException> reclassifiedToSubTreeExceptionConstructor)
    {
      WorkItemClassificationReconciliationState reconciliationState = WorkItemClassificationReconciliationState.Canceled;
      foreach (ClassificationNodeUpdateError classificationNodeUpdateError in result)
      {
        requestContext.Trace(908821, TraceLevel.Verbose, "Services", "TreeDictionary", "Error: {0}", (object) classificationNodeUpdateError.ErrorCode);
        if (classificationNodeUpdateError.Id == 0 && classificationNodeUpdateError.Identifier == Guid.Empty)
        {
          reconciliationState = (WorkItemClassificationReconciliationState) classificationNodeUpdateError.ErrorCode;
        }
        else
        {
          switch (classificationNodeUpdateError.ErrorCode)
          {
            case 601001:
              throw new WorkItemTrackingTreeNodeNotFoundException(classificationNodeUpdateError.Id);
            case 601002:
              if (classificationNodeUpdateError.Id > 0)
                throw new ClassificationNodeParentNodeDoesNotExistException(classificationNodeUpdateError.Id);
              throw new ClassificationNodeParentNodeDoesNotExistException(classificationNodeUpdateError.Identifier);
            case 601003:
              throw new ClassificationNodeReclassificationNodeDoesNotExistException(classificationNodeUpdateError.Id);
            case 601004:
              throw new ClassificationNodeDuplicateIdentifierException(classificationNodeUpdateError.Identifier);
            case 601005:
              if (duplicateNameExceptionConstructor != null)
                throw duplicateNameExceptionConstructor(classificationNodeUpdateError.Id, classificationNodeUpdateError.Identifier);
              break;
            case 601006:
              throw new ClassificationNodeCircularNodeReferenceException(classificationNodeUpdateError.Id);
            case 601007:
              if (classificationNodeUpdateError.Id > 0)
                throw new ClassificationNodeMaximumDepthExceededException(classificationNodeUpdateError.Id);
              throw new ClassificationNodeMaximumDepthExceededException(classificationNodeUpdateError.Identifier);
            case 601008:
              if (reclassifiedToSubTreeExceptionConstructor != null)
                throw reclassifiedToSubTreeExceptionConstructor(classificationNodeUpdateError.Id);
              break;
          }
          throw new ClassificationNodeUnexpectedSqlException(classificationNodeUpdateError.ErrorCode);
        }
      }
      return reconciliationState;
    }

    private WorkItemTrackingTreeService.TreeCache BuildTree(
      IVssRequestContext requestContext,
      WorkItemTrackingTreeService.TreeNodeCacheServiceImpl existingSnapshot)
    {
      return requestContext.TraceBlock<WorkItemTrackingTreeService.TreeCache>(904741, 904750, 904745, "Services", "WorkItemService", "WorkItemTreeService.BuildTree", (Func<WorkItemTrackingTreeService.TreeCache>) (() =>
      {
        IEnumerable<TreeNodeRecord> existingNodes = (IEnumerable<TreeNodeRecord>) null;
        IEnumerable<TreeNodeRecord> changedNodes = (IEnumerable<TreeNodeRecord>) null;
        if (existingSnapshot == null)
        {
          requestContext.Trace(908822, TraceLevel.Verbose, "Services", "TreeDictionary", "Getting all nodes");
          using (ClassificationNodeComponent component = requestContext.CreateComponent<ClassificationNodeComponent>())
            changedNodes = (IEnumerable<TreeNodeRecord>) component.GetAllClassificationNodes(true);
        }
        else
        {
          existingNodes = (IEnumerable<TreeNodeRecord>) existingSnapshot.NodeRecords;
          long stamp = existingSnapshot.Stamps[MetadataTable.Hierarchy];
          long lastCachestmp;
          using (ClassificationNodeComponent component = requestContext.CreateComponent<ClassificationNodeComponent>())
            changedNodes = (IEnumerable<TreeNodeRecord>) component.GetChangedClassificationNodesByCachestamp(stamp, out lastCachestmp, true);
          requestContext.Trace(908823, TraceLevel.Verbose, "Services", "TreeDictionary", "Getting changed nodes. Existing: {0}, New: {1}", (object) stamp, (object) lastCachestmp);
        }
        return WorkItemTrackingTreeService.BuildTree(requestContext, existingNodes, changedNodes, out IDictionary<WorkItemTrackingTreeService.ClassificationNodeId, WorkItemTrackingTreeService.TreeNodeImpl> _);
      }));
    }

    private static WorkItemTrackingTreeService.TreeNodeImpl CreateNodeImpl(
      TreeNodeRecord nodeRecord,
      IEqualityComparer<string> serverComparer)
    {
      return nodeRecord.TreeTypeId == -42 ? (WorkItemTrackingTreeService.TreeNodeImpl) new WorkItemTrackingTreeService.ProjectNodeImpl(nodeRecord, serverComparer) : new WorkItemTrackingTreeService.TreeNodeImpl(nodeRecord, serverComparer);
    }

    private static WorkItemTrackingTreeService.TreeCache BuildTree(
      IVssRequestContext requestContext,
      IEnumerable<TreeNodeRecord> existingNodes,
      IEnumerable<TreeNodeRecord> changedNodes,
      out IDictionary<WorkItemTrackingTreeService.ClassificationNodeId, WorkItemTrackingTreeService.TreeNodeImpl> nodesMapById)
    {
      nodesMapById = (IDictionary<WorkItemTrackingTreeService.ClassificationNodeId, WorkItemTrackingTreeService.TreeNodeImpl>) new Dictionary<WorkItemTrackingTreeService.ClassificationNodeId, WorkItemTrackingTreeService.TreeNodeImpl>();
      Dictionary<WorkItemTrackingTreeService.ClassificationNodeId, TreeNodeRecord> dictionary1 = new Dictionary<WorkItemTrackingTreeService.ClassificationNodeId, TreeNodeRecord>();
      try
      {
        requestContext.TraceEnter(904831, "Services", "WorkItemService", "WorkItemTreeService.BuildTreeAndNodeDictionary");
        if (existingNodes != null)
        {
          foreach (TreeNodeRecord existingNode in existingNodes)
          {
            WorkItemTrackingTreeService.ClassificationNodeId key = new WorkItemTrackingTreeService.ClassificationNodeId()
            {
              ProjectId = existingNode.ProjectId,
              NodeId = existingNode.Id
            };
            dictionary1[key] = existingNode;
          }
        }
        if (changedNodes != null)
        {
          foreach (TreeNodeRecord changedNode in changedNodes)
          {
            WorkItemTrackingTreeService.ClassificationNodeId key = new WorkItemTrackingTreeService.ClassificationNodeId()
            {
              ProjectId = changedNode.ProjectId,
              NodeId = changedNode.Id
            };
            if (changedNode.IsDeleted)
              dictionary1.Remove(key);
            else
              dictionary1[key] = changedNode;
          }
        }
        StringComparer serverStringComparer = requestContext.GetService<WorkItemTrackingConfigurationSettingService>().GetConfigurationInfo(requestContext).ServerStringComparer;
        Dictionary<Guid, WorkItemTrackingTreeService.ProjectNodeImpl> dictionary2 = new Dictionary<Guid, WorkItemTrackingTreeService.ProjectNodeImpl>();
        foreach (TreeNodeRecord nodeRecord in dictionary1.Values)
        {
          WorkItemTrackingTreeService.ClassificationNodeId key = new WorkItemTrackingTreeService.ClassificationNodeId()
          {
            ProjectId = nodeRecord.ProjectId,
            NodeId = nodeRecord.Id
          };
          WorkItemTrackingTreeService.TreeNodeImpl nodeImpl = WorkItemTrackingTreeService.CreateNodeImpl(nodeRecord, (IEqualityComparer<string>) serverStringComparer);
          nodesMapById[key] = nodeImpl;
          if (nodeImpl is WorkItemTrackingTreeService.ProjectNodeImpl)
            dictionary2[nodeImpl.CssNodeId] = nodeImpl as WorkItemTrackingTreeService.ProjectNodeImpl;
        }
        foreach (WorkItemTrackingTreeService.TreeNodeImpl node1 in (IEnumerable<WorkItemTrackingTreeService.TreeNodeImpl>) nodesMapById.Values)
        {
          WorkItemTrackingTreeService.TreeNodeImpl treeNodeImpl;
          if (nodesMapById.TryGetValue(new WorkItemTrackingTreeService.ClassificationNodeId()
          {
            ProjectId = node1.ProjectId,
            NodeId = node1.ParentId
          }, out treeNodeImpl))
            treeNodeImpl.AddChildren(node1);
          else if (node1.ParentId > 0)
            requestContext.Trace(908826, TraceLevel.Verbose, "Services", "TreeDictionary", "Failed to find parent node: projectId {0}, parentId {1}", (object) node1.ProjectId, (object) node1.ParentId);
          WorkItemTrackingTreeService.ProjectNodeImpl node2;
          if (dictionary2.TryGetValue(node1.ProjectId, out node2))
          {
            node1.SetProjectNode((TreeNode) node2);
            node2.AddToNodesMaps((TreeNode) node1);
          }
          else
            requestContext.Trace(908827, TraceLevel.Verbose, "Services", "TreeDictionary", "Failed to find project {0}", (object) node1.ProjectId);
        }
        return new WorkItemTrackingTreeService.TreeCache()
        {
          ProjectNodes = dictionary2,
          NodeRecords = (ICollection<TreeNodeRecord>) dictionary1.Values.ToArray<TreeNodeRecord>()
        };
      }
      finally
      {
        requestContext.TraceLeave(904840, "Services", "WorkItemService", "WorkItemTreeService.BuildTreeAndNodeDictionary");
      }
    }

    internal WorkItemClassificationReconciliationState ReclassifyWorkItems(
      IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      return requestContext.TraceBlock<WorkItemClassificationReconciliationState>(908829, 908830, "Services", "TreeDictionary", nameof (ReclassifyWorkItems), (Func<WorkItemClassificationReconciliationState>) (() =>
      {
        int? commandTimeoutOverride = requestContext.GetService<WorkItemTrackingConfigurationSettingService>().GetConfigurationInfo(requestContext).ReclassificationSqlCommandTimeoutOverride;
        long cachestamp = 0;
        IEnumerable<ClassificationNodeChange> source = (IEnumerable<ClassificationNodeChange>) null;
        if (WorkItemTrackingFeatureFlags.IsReclassifyWorkItemsRaceConditionFixEnabled(requestContext))
        {
          using (ClassificationNodeComponent component = requestContext.CreateComponent<ClassificationNodeComponent>())
            source = component.GetClassificationNodeChanges(false, out cachestamp);
        }
        Guid id = requestContext.WitContext().RequestIdentity.Id;
        WorkItemClassificationReconciliationState errorCode;
        using (ClassificationNodeComponent component = requestContext.CreateComponent<ClassificationNodeComponent>())
        {
          component.CommandTimeoutOverride = commandTimeoutOverride;
          errorCode = component.ReconcileWorkItems(id);
        }
        if (errorCode == WorkItemClassificationReconciliationState.Canceled || errorCode == WorkItemClassificationReconciliationState.TimedOut)
        {
          requestContext.Trace(908831, TraceLevel.Info, "Services", "TreeDictionary", string.Format("Work item tree path reclassification {0}. Rescheduling the job again.", (object) errorCode));
          this.QueueReclassificationJob(requestContext);
        }
        else
        {
          if (errorCode != WorkItemClassificationReconciliationState.Succeeded)
            throw new ClassificationNodeUnexpectedSqlException((int) errorCode);
          if (!WorkItemTrackingFeatureFlags.IsReclassifyWorkItemsRaceConditionFixEnabled(requestContext))
          {
            using (ClassificationNodeComponent component = requestContext.CreateComponent<ClassificationNodeComponent>())
              source = component.GetClassificationNodeChanges(false, out cachestamp);
          }
          if (source.Any<ClassificationNodeChange>())
          {
            IWorkItemTrackingConfigurationInfo configurationInfo = requestContext.GetService<WorkItemTrackingConfigurationSettingService>().GetConfigurationInfo(requestContext);
            HashSet<int> intSet = new HashSet<int>();
            HashSet<string> stringSet = new HashSet<string>((IEqualityComparer<string>) configurationInfo.ServerStringComparer);
            HashSet<Guid> projectIds = new HashSet<Guid>();
            IProjectService service1 = requestContext.GetService<IProjectService>();
            foreach (ClassificationNodeChange classificationNodeChange in source)
            {
              if (classificationNodeChange.Id > 0)
                intSet.Add(classificationNodeChange.Id);
              if (!string.IsNullOrWhiteSpace(classificationNodeChange.Path))
              {
                try
                {
                  stringSet.Add(service1.GetProjectName(requestContext, classificationNodeChange.ProjectId) + classificationNodeChange.Path);
                  projectIds.Add(classificationNodeChange.ProjectId);
                }
                catch (ProjectDoesNotExistException ex)
                {
                }
              }
            }
            if (intSet.Any<int>() || stringSet.Any<string>())
            {
              int num = (int) requestContext.GetService<IWorkItemTypeExtensionService>().ReconcileExtensions(requestContext, intSet, stringSet, 0);
            }
            using (ClassificationNodeComponent component = requestContext.CreateComponent<ClassificationNodeComponent>())
              component.DeleteClassificationNodeChanges(cachestamp);
            if (projectIds.Count > 0)
            {
              ITeamFoundationEventService service2 = requestContext.GetService<ITeamFoundationEventService>();
              List<WorkItemChangedProjectEvent> changedProjectEventList = new List<WorkItemChangedProjectEvent>();
              foreach (Guid guid in projectIds)
                changedProjectEventList.Add(new WorkItemChangedProjectEvent()
                {
                  ProjectId = guid,
                  WorkItemUpdatedList = Enumerable.Empty<string>(),
                  EventTime = DateTime.UtcNow
                });
              service2.PublishNotification(requestContext, (object) new WorkItemChangedNotification()
              {
                Events = changedProjectEventList
              });
              if (requestContext.ExecutionEnvironment.IsHostedDeployment && requestContext.IsFeatureEnabled("WorkItemTracking.Server.EnableFireEventsToServiceBus"))
                this.m_serviceBusEventPublisher.TrySendNotificationsToServiceBus(requestContext, (ISet<Guid>) projectIds);
            }
          }
        }
        return errorCode;
      }));
    }

    private void QueueReclassificationJobInTaskService(IVssRequestContext requestContext) => requestContext.To(TeamFoundationHostType.Deployment).GetService<ITeamFoundationTaskService>().AddTask(requestContext, (TeamFoundationTaskCallback) ((ctx, args) => this.QueueReclassificationJob(ctx)));

    private void QueueReclassificationJob(IVssRequestContext requestContext)
    {
      try
      {
        requestContext.GetService<TeamFoundationJobService>().QueueJobsNow(requestContext, (IEnumerable<Guid>) new Guid[1]
        {
          WorkItemTrackingJobs.WorkItemReclassification
        }, true);
      }
      catch (JobDefinitionNotFoundException ex)
      {
      }
      catch (Exception ex)
      {
        requestContext.TraceException(908828, "Services", "TreeDictionary", ex);
      }
    }

    public WorkItemTrackingTreeService.NotificationEventCreator EventCreator { get; set; }

    public (int Count, int PathLimit) FetchAreaAndIterationCountWithLimits(
      IVssRequestContext requestContext,
      Guid projectId,
      TreeStructureType type)
    {
      WorkItemTrackingTreeService.TreeNodeCacheServiceImpl snapshot = this.GetSnapshot<WorkItemTrackingTreeService.TreeNodeCacheServiceImpl>(requestContext, false);
      if (type == TreeStructureType.Area)
        return this.FetchAreaCountWithLimits(snapshot, projectId, requestContext);
      return type == TreeStructureType.Iteration ? this.FetchIterationCountWithLimits(snapshot, projectId, requestContext) : (0, 0);
    }

    private (int Count, int PathLimit) FetchAreaCountWithLimits(
      WorkItemTrackingTreeService.TreeNodeCacheServiceImpl snapshot,
      Guid projectId,
      IVssRequestContext requestContext)
    {
      return (snapshot.GetTreeNodeCount(projectId, TreeStructureType.Area), WorkItemTrackingTreeService.GetProjectAreaPathLimit(requestContext));
    }

    private (int Count, int PathLimit) FetchIterationCountWithLimits(
      WorkItemTrackingTreeService.TreeNodeCacheServiceImpl snapshot,
      Guid projectId,
      IVssRequestContext requestContext)
    {
      return (snapshot.GetTreeNodeCount(projectId, TreeStructureType.Iteration), WorkItemTrackingTreeService.GetProjectIterationPathLimit(requestContext));
    }

    private class NodeInfo
    {
      private ClassificationNodeUpdate m_update;
      private TreeNode m_treeNode;

      public NodeInfo(ClassificationNodeUpdate update, IEqualityComparer<string> comparer)
      {
        this.m_update = update;
        this.ChildNames = new HashSet<string>(comparer);
      }

      public NodeInfo(TreeNode treeNode, IEqualityComparer<string> comparer)
      {
        this.m_treeNode = treeNode;
        this.ChildNames = new HashSet<string>((IEnumerable<string>) treeNode.Children.Keys, comparer);
      }

      public int Depth
      {
        get
        {
          if (this.m_treeNode != null)
            return -42 - this.m_treeNode.TypeId;
          if (this.m_update != null)
          {
            if (this.Parent != null)
              return this.Parent.Depth + 1;
            if (this.m_update.ParentIdentifier == Guid.Empty)
              return 1;
          }
          return 15;
        }
      }

      public WorkItemTrackingTreeService.NodeInfo Parent { get; set; }

      public HashSet<string> ChildNames { get; private set; }
    }

    public class IterationDates
    {
      public DateTime? StartDate { get; set; }

      public DateTime? FinishDate { get; set; }
    }

    public struct ClassificationNodeId : IEquatable<WorkItemTrackingTreeService.ClassificationNodeId>
    {
      public Guid ProjectId { get; set; }

      public int NodeId { get; set; }

      public override int GetHashCode() => this.ProjectId.GetHashCode() ^ this.NodeId.GetHashCode();

      public override bool Equals(object obj) => obj is WorkItemTrackingTreeService.ClassificationNodeId other && this.Equals(other);

      public bool Equals(
        WorkItemTrackingTreeService.ClassificationNodeId other)
      {
        return other.ProjectId == this.ProjectId && other.NodeId == this.NodeId;
      }

      public static bool operator ==(
        WorkItemTrackingTreeService.ClassificationNodeId a,
        WorkItemTrackingTreeService.ClassificationNodeId b)
      {
        return a.Equals(b);
      }

      public static bool operator !=(
        WorkItemTrackingTreeService.ClassificationNodeId a,
        WorkItemTrackingTreeService.ClassificationNodeId b)
      {
        return !a.Equals(b);
      }
    }

    public class NotificationEventCreator
    {
      private WorkItemTrackingTreeService m_treeService;

      public NotificationEventCreator(WorkItemTrackingTreeService treeService) => this.m_treeService = treeService;

      public virtual List<WorkItemClassificationNodeChangedEvent> CreateNotificationEventsForCreatedNodes(
        IVssRequestContext requestContext,
        Guid projectId,
        ICollection<ClassificationNodeUpdate> nodes)
      {
        ITreeDictionary snapshot = this.m_treeService.GetSnapshot(requestContext);
        List<WorkItemClassificationNodeChangedEvent> eventsForCreatedNodes = new List<WorkItemClassificationNodeChangedEvent>(nodes.Count);
        foreach (ClassificationNodeUpdate node in (IEnumerable<ClassificationNodeUpdate>) nodes)
        {
          TreeNode treeNode = snapshot.GetTreeNode(projectId, node.Identifier);
          if (treeNode.Type != TreeStructureType.None)
          {
            string path = treeNode.GetPath(requestContext);
            eventsForCreatedNodes.Add(new WorkItemClassificationNodeChangedEvent()
            {
              ProjectId = projectId,
              NodeId = treeNode.Id,
              CurrentNodePath = path,
              EventType = Microsoft.TeamFoundation.WorkItemTracking.Notification.EventType.Add,
              NodeType = WorkItemTrackingTreeService.GetNodeTypeFrom(treeNode.Type),
              StartDate = treeNode.StartDate,
              FinishDate = treeNode.FinishDate
            });
          }
        }
        return eventsForCreatedNodes;
      }

      public virtual List<WorkItemClassificationNodeChangedEvent> CreateNotificationEventsForDeletedNodes(
        IVssRequestContext requestContext,
        Guid projectId,
        ICollection<ClassificationNodeUpdate> nodes,
        ITreeDictionary snapshotBeforeNodeDelete)
      {
        List<WorkItemClassificationNodeChangedEvent> eventsForDeletedNodes = new List<WorkItemClassificationNodeChangedEvent>(nodes.Count);
        foreach (ClassificationNodeUpdate node in (IEnumerable<ClassificationNodeUpdate>) nodes)
        {
          TreeNode treeNode = snapshotBeforeNodeDelete.GetTreeNode(projectId, node.Id);
          string path1 = treeNode.GetPath(requestContext);
          string path2 = snapshotBeforeNodeDelete.GetTreeNode(projectId, node.ReclassifyId).GetPath(requestContext);
          eventsForDeletedNodes.Add(new WorkItemClassificationNodeChangedEvent()
          {
            ProjectId = projectId,
            NodeId = node.Id,
            CurrentNodePath = path1,
            NewNodePath = path2,
            EventType = Microsoft.TeamFoundation.WorkItemTracking.Notification.EventType.Delete,
            NodeType = WorkItemTrackingTreeService.GetNodeTypeFrom(treeNode.Type)
          });
        }
        return eventsForDeletedNodes;
      }

      public virtual List<WorkItemClassificationNodeChangedEvent> CreateNotificationEventsForUpdatedNode(
        IVssRequestContext requestContext,
        Guid projectId,
        TreeNode node,
        string newName,
        WorkItemTrackingTreeService.IterationDates dates,
        TreeNode newParent)
      {
        List<WorkItemClassificationNodeChangedEvent> eventsForUpdatedNode = new List<WorkItemClassificationNodeChangedEvent>(3);
        string path = node.GetPath(requestContext);
        ClassificationNodeType nodeTypeFrom = WorkItemTrackingTreeService.GetNodeTypeFrom(node.Type);
        if (newName != null)
          eventsForUpdatedNode.Add(new WorkItemClassificationNodeChangedEvent()
          {
            ProjectId = projectId,
            NodeId = node.Id,
            CurrentNodePath = path,
            NodeType = nodeTypeFrom,
            NewNodeName = newName,
            EventType = Microsoft.TeamFoundation.WorkItemTracking.Notification.EventType.Rename
          });
        if (newParent != null)
          eventsForUpdatedNode.Add(new WorkItemClassificationNodeChangedEvent()
          {
            ProjectId = projectId,
            NodeId = node.Id,
            CurrentNodePath = path,
            NodeType = nodeTypeFrom,
            NewNodePath = newParent.GetPath(requestContext),
            EventType = Microsoft.TeamFoundation.WorkItemTracking.Notification.EventType.Reclassify
          });
        if (dates != null)
          eventsForUpdatedNode.Add(new WorkItemClassificationNodeChangedEvent()
          {
            ProjectId = projectId,
            NodeId = node.Id,
            CurrentNodePath = path,
            NodeType = nodeTypeFrom,
            StartDate = dates.StartDate,
            FinishDate = dates.FinishDate,
            EventType = Microsoft.TeamFoundation.WorkItemTracking.Notification.EventType.UpdateDates
          });
        return eventsForUpdatedNode;
      }
    }

    private class TreeCache
    {
      public Dictionary<Guid, WorkItemTrackingTreeService.ProjectNodeImpl> ProjectNodes { get; set; }

      public ICollection<TreeNodeRecord> NodeRecords { get; set; }
    }

    private class TreeNodeCacheServiceImpl : CacheSnapshotBase, ITreeDictionary
    {
      private Dictionary<Guid, WorkItemTrackingTreeService.ProjectNodeImpl> m_projectNodes;
      private Dictionary<int, Guid> m_projectIdLookupById;
      private Dictionary<Guid, Guid> m_projectIdLookupByGuid;
      private WorkItemTrackingTreeService m_service;

      public TreeNodeCacheServiceImpl(
        IVssRequestContext requestContext,
        WorkItemTrackingTreeService service,
        MetadataDBStamps stamps,
        WorkItemTrackingTreeService.TreeNodeCacheServiceImpl existingSnapshot)
        : base(stamps)
      {
        this.m_service = service;
        this.Initialize(requestContext, service, existingSnapshot);
      }

      private void Initialize(
        IVssRequestContext requestContext,
        WorkItemTrackingTreeService service,
        WorkItemTrackingTreeService.TreeNodeCacheServiceImpl existingSnapshot)
      {
        requestContext.TraceBlock(901520, 901529, 901528, "Dictionaries", "TreeDictionary", "TreeNodeCacheServiceImpl.Initialize", (Action) (() =>
        {
          this.m_projectIdLookupById = new Dictionary<int, Guid>();
          this.m_projectIdLookupByGuid = new Dictionary<Guid, Guid>();
          WorkItemTrackingTreeService.TreeCache treeCache = service.BuildTree(requestContext, existingSnapshot);
          this.m_projectNodes = treeCache.ProjectNodes;
          this.NodeRecords = treeCache.NodeRecords;
          WorkItemTrackingTreeService.TreeNodeCacheServiceImpl.WalkNodes((IEnumerable<TreeNode>) this.m_projectNodes.Values, (Action<TreeNode>) (node =>
          {
            this.m_projectIdLookupById.Add(node.Id, node.Project.CssNodeId);
            this.m_projectIdLookupByGuid.Add(node.CssNodeId, node.Project.CssNodeId);
          }));
        }));
      }

      private static void WalkNodes(IEnumerable<TreeNode> nodes, Action<TreeNode> action)
      {
        foreach (TreeNode node in nodes)
        {
          action(node);
          if (node.HasChildren)
            WorkItemTrackingTreeService.TreeNodeCacheServiceImpl.WalkNodes((IEnumerable<TreeNode>) node.Children.Values, action);
        }
      }

      public ICollection<TreeNodeRecord> NodeRecords { get; private set; }

      public TreeNode GetTreeNode(Guid projectId, Guid nodeId)
      {
        ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
        return this.GetTreeNodeInternal(projectId, nodeId);
      }

      internal TreeNode GetTreeNodeInternal(Guid projectId, Guid nodeId)
      {
        TreeNode node;
        if (!this.TryGetTreeNodeInternal(projectId, nodeId, out node))
          throw new WorkItemTrackingTreeNodeNotFoundException(nodeId);
        return node;
      }

      public bool TryGetTreeNode(Guid projectId, Guid nodeId, out TreeNode node)
      {
        ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
        return this.TryGetTreeNodeInternal(projectId, nodeId, out node);
      }

      public int GetTreeNodeCount(Guid projectId, TreeStructureType type)
      {
        WorkItemTrackingTreeService.ProjectNodeImpl projectNodeImpl;
        return !this.m_projectNodes.TryGetValue(projectId, out projectNodeImpl) ? 0 : projectNodeImpl.NodesMapByGuid.Values.Count<TreeNode>((Func<TreeNode, bool>) (node => node.Type == type));
      }

      internal bool TryGetTreeNodeInternal(Guid projectId, Guid nodeId, out TreeNode node)
      {
        node = (TreeNode) null;
        WorkItemTrackingTreeService.ProjectNodeImpl projectNodeImpl;
        return (!(projectId == Guid.Empty) || this.TryFindProjectId(nodeId, out projectId)) && this.m_projectNodes.TryGetValue(projectId, out projectNodeImpl) && projectNodeImpl.NodesMapByGuid.TryGetValue(nodeId, out node);
      }

      public TreeNode GetTreeNode(Guid projectId, int nodeId)
      {
        ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
        return this.GetTreeNodeInternal(projectId, nodeId);
      }

      internal TreeNode GetTreeNodeInternal(Guid projectId, int nodeId)
      {
        TreeNode node;
        if (!this.TryGetTreeNodeInternal(projectId, nodeId, out node))
          throw new WorkItemTrackingTreeNodeNotFoundException(nodeId);
        return node;
      }

      public bool TryGetTreeNode(Guid projectId, int nodeId, out TreeNode node)
      {
        ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
        return this.TryGetTreeNodeInternal(projectId, nodeId, out node);
      }

      internal bool TryGetTreeNodeInternal(Guid projectId, int nodeId, out TreeNode node)
      {
        node = (TreeNode) null;
        WorkItemTrackingTreeService.ProjectNodeImpl projectNodeImpl;
        return (!(projectId == Guid.Empty) || this.TryFindProjectId(nodeId, out projectId)) && this.m_projectNodes.TryGetValue(projectId, out projectNodeImpl) && projectNodeImpl.NodesMapById.TryGetValue(nodeId, out node);
      }

      public bool TryGetTreeNode(
        Guid projectId,
        string relativePath,
        TreeStructureType type,
        out TreeNode node)
      {
        ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
        node = (TreeNode) null;
        WorkItemTrackingTreeService.ProjectNodeImpl projectNodeImpl;
        if (!this.m_projectNodes.TryGetValue(projectId, out projectNodeImpl))
          return false;
        node = projectNodeImpl.FindChildNodeByPath(relativePath, type);
        if (node == null)
          return false;
        return node.Type == type || type == TreeStructureType.None;
      }

      public TreeNode GetTreeNode(Guid projectId, string relativePath, TreeStructureType type)
      {
        ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
        TreeNode node;
        if (!this.TryGetTreeNode(projectId, relativePath, type, out node))
          throw new WorkItemTrackingTreeNodeNotFoundException(relativePath);
        return node;
      }

      public IEnumerable<TreeNode> GetRootTreeNodes(Guid projectId)
      {
        ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
        IEnumerable<TreeNode> nodes;
        if (!this.TryGetRootTreeNodes(projectId, out nodes))
          throw new WorkItemTrackingTreeNodeNotFoundException(projectId);
        return nodes;
      }

      public bool TryGetRootTreeNodes(Guid projectId, out IEnumerable<TreeNode> nodes)
      {
        ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
        nodes = (IEnumerable<TreeNode>) null;
        WorkItemTrackingTreeService.ProjectNodeImpl projectNodeImpl;
        if (!this.m_projectNodes.TryGetValue(projectId, out projectNodeImpl))
          return false;
        nodes = (IEnumerable<TreeNode>) projectNodeImpl.Children.Values.ToArray<TreeNode>();
        return true;
      }

      public TreeNode LegacyGetTreeNode(Guid nodeId) => this.GetTreeNodeInternal(Guid.Empty, nodeId);

      public bool LegacyTryGetTreeNode(Guid nodeId, out TreeNode node) => this.TryGetTreeNodeInternal(Guid.Empty, nodeId, out node);

      public TreeNode LegacyGetTreeNode(int nodeId) => this.GetTreeNodeInternal(Guid.Empty, nodeId);

      public bool LegacyTryGetTreeNode(int nodeId, out TreeNode node) => this.TryGetTreeNodeInternal(Guid.Empty, nodeId, out node);

      public int LegacyGetTreeNodeIdFromPath(
        IVssRequestContext requestContext,
        string absolutePath,
        TreeStructureType type)
      {
        TreeNode node = (TreeNode) null;
        return this.TryGetNodeFromPath(requestContext, absolutePath, type, out node) ? node.Id : -1;
      }

      public bool TryGetNodeFromPath(
        IVssRequestContext requestContext,
        string absolutePath,
        TreeStructureType type,
        out TreeNode node)
      {
        node = (TreeNode) null;
        string relativePath;
        Guid projectId = this.m_service.NormalizeTreePath(requestContext, absolutePath, out relativePath);
        if (!(projectId != Guid.Empty) || !this.TryGetTreeNode(projectId, relativePath, type, out node))
          return false;
        if (node.IsStructureSpecifier)
          node = node.Parent;
        return true;
      }

      public bool TryFindProjectId(int nodeId, out Guid projectId) => this.m_projectIdLookupById.TryGetValue(nodeId, out projectId);

      private bool TryFindProjectId(Guid nodeId, out Guid projectId) => this.m_projectIdLookupByGuid.TryGetValue(nodeId, out projectId);
    }

    protected class TreeNodeImpl : TreeNode
    {
      internal TreeNodeImpl(TreeNodeRecord treeNode, IEqualityComparer<string> comparer)
        : base(comparer)
      {
        this.Id = treeNode.Id;
        this.ParentId = treeNode.ParentId;
        this.ProjectId = treeNode.ProjectId;
        this.Name = treeNode.Name;
        this.Type = treeNode.StructureType;
        this.TypeId = treeNode.TreeTypeId;
        this.CssNodeId = treeNode.CssNodeId;
        this.StartDate = treeNode.StartDate;
        this.FinishDate = treeNode.FinishDate;
      }

      private TreeNodeImpl(IEqualityComparer<string> comparer)
        : base(comparer)
      {
      }

      public virtual void AddChildren(WorkItemTrackingTreeService.TreeNodeImpl node)
      {
        this.Children[node.Name] = (TreeNode) node;
        node.Parent = (TreeNode) this;
      }

      public virtual void SetProjectNode(TreeNode node) => this.Project = node;
    }

    protected class ProjectNodeImpl : WorkItemTrackingTreeService.TreeNodeImpl
    {
      internal ProjectNodeImpl(TreeNodeRecord treeNode, IEqualityComparer<string> comparer)
        : base(treeNode, comparer)
      {
        this.NodesMapById = new Dictionary<int, TreeNode>();
        this.NodesMapByGuid = new Dictionary<Guid, TreeNode>();
      }

      internal Dictionary<int, TreeNode> NodesMapById { get; private set; }

      internal Dictionary<Guid, TreeNode> NodesMapByGuid { get; private set; }

      internal void AddToNodesMaps(TreeNode node)
      {
        this.NodesMapByGuid.Add(node.CssNodeId, node);
        this.NodesMapById.Add(node.Id, node);
      }
    }
  }
}
