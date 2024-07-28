// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItemFieldData
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.Azure.Devops.Tags.Server;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Common.Predicates;
using Microsoft.TeamFoundation.WorkItemTracking.Internals;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Identity;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Rules;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Notifications.Server;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems
{
  public class WorkItemFieldData : WorkItemSecuredObject
  {
    internal static readonly HashSet<string> c_SafeHtmlExcludedFields = new HashSet<string>((IEnumerable<string>) new string[3]
    {
      "Microsoft.VSTS.TCM.Steps",
      "Microsoft.VSTS.TCM.LocalDataSource",
      "Microsoft.VSTS.TCM.Parameters"
    }, (IEqualityComparer<string>) TFStringComparer.WorkItemFieldReferenceName);
    private Dictionary<int, object> m_updates;
    private Guid? m_projectId;

    public WorkItemFieldData()
      : this((IDictionary<int, object>) null)
    {
    }

    internal WorkItemFieldData(IDictionary<int, object> latestData, int permissionsRequested = 0)
      : base(permissionsRequested)
    {
      if (latestData == null)
        this.LatestData = new ReadOnlyDictionary<int, object>((IDictionary<int, object>) new Dictionary<int, object>());
      else
        this.LatestData = new ReadOnlyDictionary<int, object>(latestData);
    }

    internal WorkItemFieldData(
      IDictionary<int, object> latestData,
      Guid projectId,
      int permissionsRequested = 0)
      : this(latestData, permissionsRequested)
    {
      this.m_projectId = new Guid?(projectId);
    }

    internal WorkItemFieldData(
      IDictionary<int, object> latestData,
      IDictionary<int, Guid> identityFieldValues,
      int permissionsRequested = 0)
      : this(latestData, permissionsRequested)
    {
      if (identityFieldValues == null)
        this.IdentitityFields = new ReadOnlyDictionary<int, Guid>((IDictionary<int, Guid>) new Dictionary<int, Guid>());
      else
        this.IdentitityFields = new ReadOnlyDictionary<int, Guid>(identityFieldValues);
    }

    internal WorkItemFieldData(
      IDictionary<int, object> latestData,
      IDictionary<int, Guid> identityFieldValues,
      WorkItemCommentVersionRecord commentVersion,
      int permissionsRequested = 0)
      : this(latestData, identityFieldValues, permissionsRequested)
    {
      this.CommentVersion = commentVersion;
    }

    internal WorkItemFieldData(
      IDictionary<int, object> latestData,
      IDictionary<int, Guid> identityFieldValues,
      Guid projectId,
      int permissionsRequested = 0)
      : this(latestData, identityFieldValues, permissionsRequested)
    {
      this.m_projectId = new Guid?(projectId);
    }

    internal WorkItemFieldData(
      IDictionary<int, object> latestData,
      IDictionary<int, Guid> identityFieldValues,
      Guid projectId,
      IEnumerable<TagDefinition> tagDefinitions,
      int permissionsRequested)
      : this(latestData, identityFieldValues, projectId, permissionsRequested)
    {
      this.TagDefinitions = tagDefinitions;
    }

    internal WorkItemFieldData(
      IDictionary<int, object> latestData,
      IDictionary<int, Guid> identityFieldValues,
      Guid projectId,
      IEnumerable<TagDefinition> tagDefinitions,
      int permissionsRequested,
      WorkItemCommentVersionRecord commentVersion)
      : this(latestData, identityFieldValues, projectId, tagDefinitions, permissionsRequested)
    {
      this.CommentVersion = commentVersion;
    }

    public ReadOnlyDictionary<int, object> LatestData { get; set; }

    public ReadOnlyDictionary<int, Guid> IdentitityFields { get; set; }

    public IEnumerable<TagDefinition> TagDefinitions { get; internal set; }

    public Guid? ProjectId => this.m_projectId;

    public bool HasUpdates => this.m_updates != null && this.m_updates.Count > 0;

    public Dictionary<int, object> Updates
    {
      get
      {
        if (this.m_updates == null)
          this.m_updates = new Dictionary<int, object>();
        return this.m_updates;
      }
      protected set => this.m_updates = value;
    }

    public int Id
    {
      get => this.GetFieldValueInternal<int>(-3, false);
      protected set => this.SetFieldValueInternal<int>(-3, value);
    }

    public bool IsDeleted => this.GetFieldValueInternal<bool>(-404, false);

    public int Revision
    {
      get => this.GetFieldValueInternal<int>(8, false);
      protected set => this.SetFieldValueInternal<int>(8, value);
    }

    public int AreaId
    {
      get => this.GetFieldValueInternal<int>(-2, false);
      protected set => this.SetFieldValueInternal<int>(-2, value);
    }

    public int IterationId => this.GetFieldValueInternal<int>(-104, false);

    public string CreatedBy => this.GetFieldValueInternal<string>(33, false);

    public DateTime CreatedDate => this.GetFieldValueInternal<DateTime>(32, false);

    public string ModifiedBy => this.GetFieldValueInternal<string>(9, false);

    public DateTime ModifiedDate => this.GetFieldValueInternal<DateTime>(-4, false);

    public string WorkItemType
    {
      get => this.GetFieldValueInternal<string>(25, false);
      protected set => this.SetFieldValueInternal<string>(25, value);
    }

    public string Title => this.GetFieldValueInternal<string>(1, false);

    public string State
    {
      get => this.GetFieldValueInternal<string>(2, false);
      internal set => this.SetFieldValueInternal<string>(2, value);
    }

    public string Reason => this.GetFieldValueInternal<string>(22, false);

    public string AssignedTo => this.GetFieldValueInternal<string>(24, false);

    public string Description => this.GetFieldValueInternal<string>(52, false);

    public WorkItemCommentVersionRecord CommentVersion { get; }

    public string GetAreaPath(IVssRequestContext requestContext) => WorkItemFieldData.GetTreePath(requestContext, this.AreaId);

    public string GetIterationPath(IVssRequestContext requestContext) => WorkItemFieldData.GetTreePath(requestContext, this.IterationId);

    private static string GetTreePath(IVssRequestContext requestContext, int nodeId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      TreeNode node;
      return !requestContext.WitContext().TreeService.LegacyTryGetTreeNode(nodeId, out node) ? (string) null : node.GetPath(requestContext);
    }

    public string GetProjectName(IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      return this.GetProjectName(requestContext.WitContext());
    }

    internal string GetProjectName(WorkItemTrackingRequestContext witRequestContext)
    {
      TreeNode projectNode = this.GetProjectNode(witRequestContext);
      return projectNode == null ? string.Empty : projectNode.GetName(witRequestContext.RequestContext);
    }

    public int GetProjectId(IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      return this.GetProjectId(requestContext.WitContext());
    }

    internal int GetProjectId(WorkItemTrackingRequestContext witRequestContext)
    {
      TreeNode projectNode = this.GetProjectNode(witRequestContext);
      return projectNode == null ? -1 : projectNode.Id;
    }

    public virtual Guid GetProjectGuid(IVssRequestContext requestContext, bool oldValue = false)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      return this.GetProjectGuid(requestContext.WitContext(), oldValue);
    }

    internal virtual Guid GetProjectGuid(
      WorkItemTrackingRequestContext witRequestContext,
      bool oldValue = false)
    {
      int num1;
      if (this.m_projectId.HasValue)
      {
        Guid? projectId = this.m_projectId;
        Guid empty = Guid.Empty;
        num1 = projectId.HasValue ? (projectId.HasValue ? (projectId.GetValueOrDefault() != empty ? 1 : 0) : 0) : 1;
      }
      else
        num1 = 0;
      int num2 = oldValue ? 1 : 0;
      if ((num1 & num2) != 0)
        return this.m_projectId.Value;
      TreeNode projectNode = this.GetProjectNode(witRequestContext, oldValue);
      return projectNode == null ? Guid.Empty : projectNode.CssNodeId;
    }

    internal TreeNode GetProjectNode(
      WorkItemTrackingRequestContext witRequestContext,
      bool oldValue = false)
    {
      return this.GetProjectNode(witRequestContext, out int _, oldValue);
    }

    internal TreeNode GetProjectNode(
      WorkItemTrackingRequestContext witRequestContext,
      out int nodeId,
      bool oldValue = false)
    {
      nodeId = this.GetFieldValueInternal<int>(-2, oldValue);
      TreeNode node;
      return !witRequestContext.TreeService.LegacyTryGetTreeNode(nodeId, out node) ? (TreeNode) null : node.Project;
    }

    public bool HasField(int fieldId) => this.LatestData.ContainsKey(fieldId) || this.IdentitityFields != null && this.IdentitityFields.ContainsKey(fieldId);

    public WorkItemTypeExtension[] GetActiveExtensions(IVssRequestContext requestContext) => requestContext.TraceBlock<WorkItemTypeExtension[]>(901210, 901219, 901218, "Update", nameof (WorkItemFieldData), nameof (GetActiveExtensions), (Func<WorkItemTypeExtension[]>) (() =>
    {
      IWorkItemTypeExtensionsMatcher extensionMatcher = requestContext.GetService<WorkItemTypeExtensionService>().GetExtensionMatcher(requestContext, new Guid?(), new Guid?());
      return this.GetActiveExtensions(requestContext.WitContext(), extensionMatcher);
    }));

    internal WorkItemTypeExtension[] GetActiveExtensions(
      WorkItemTrackingRequestContext witRequestContext,
      IWorkItemTypeExtensionsMatcher extensionMatcher)
    {
      return extensionMatcher.GetActiveExtensions((IExtendedPredicateEvaluationContext) new WorkItemFieldData.FieldDataPredicateEvaluationContext(witRequestContext, this)).ToArray<WorkItemTypeExtension>();
    }

    internal WorkItemTypeExtension[] GetApplicableExtensions(
      WorkItemTrackingRequestContext witRequestContext)
    {
      return witRequestContext.RequestContext.TraceBlock<WorkItemTypeExtension[]>(901200, 901209, 901208, "Update", nameof (WorkItemFieldData), nameof (GetApplicableExtensions), (Func<WorkItemTypeExtension[]>) (() => this.GetApplicableExtensions(witRequestContext, witRequestContext.RequestContext.GetService<WorkItemTypeExtensionService>().GetExtensionMatcher(witRequestContext.RequestContext, new Guid?(), new Guid?()))));
    }

    internal WorkItemTypeExtension[] GetApplicableExtensions(
      WorkItemTrackingRequestContext witRequestContext,
      IWorkItemTypeExtensionsMatcher extensionMatcher)
    {
      return extensionMatcher.GetMatchingExtensions((IExtendedPredicateEvaluationContext) new WorkItemFieldData.FieldDataPredicateEvaluationContext(witRequestContext, this)).ToArray<WorkItemTypeExtension>();
    }

    internal bool IsPredicateMatch(
      WorkItemTrackingRequestContext witRequestContext,
      WorkItemTypeExtension extension)
    {
      return extension.IsPredicateMatch((IPredicateEvaluationHelper) new WorkItemFieldData.FieldDataPredicateEvaluationContext(witRequestContext, this));
    }

    internal void EvaluateRules(
      WorkItemTrackingRequestContext witRequestContext,
      IEnumerable<WorkItemFieldRule> fieldRules,
      RuleEngineConfiguration configuration = null,
      IEnumerable<int> fieldIds = null)
    {
      RuleEngine ruleEngine = new RuleEngine(fieldRules, configuration);
      this.EvaluateRules(witRequestContext, ruleEngine, fieldIds);
    }

    internal void EvaluateRules(
      WorkItemTrackingRequestContext witRequestContext,
      RuleEngine ruleEngine,
      IEnumerable<int> fieldIds = null)
    {
      ruleEngine.Evaluate(this.GetRuleEvaluationContext(witRequestContext), fieldIds);
    }

    public IRuleEvaluationContext GetRuleEvaluationContext(IVssRequestContext requestContext) => this.GetRuleEvaluationContext(requestContext.WitContext());

    internal IRuleEvaluationContext GetRuleEvaluationContext(
      WorkItemTrackingRequestContext witRequestContext)
    {
      return (IRuleEvaluationContext) new WorkItemFieldData.FieldDataRuleEvaluationContext(witRequestContext, this);
    }

    internal InternalFieldType GetFieldType(
      WorkItemTrackingRequestContext witRequestContext,
      int fieldId)
    {
      ArgumentUtility.CheckForNull<WorkItemTrackingRequestContext>(witRequestContext, nameof (witRequestContext));
      return witRequestContext.FieldDictionary.GetField(fieldId).FieldType;
    }

    public void SetFieldUpdates(
      IVssRequestContext requestContext,
      IEnumerable<KeyValuePair<string, object>> updates)
    {
      requestContext.TraceBlock(904591, 904600, 904595, "Services", "WorkItemService", "WorkItemFieldData.SetFieldUpdates", (Action) (() =>
      {
        ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, "witRequestContext");
        ArgumentUtility.CheckForNull<IEnumerable<KeyValuePair<string, object>>>(updates, nameof (updates));
        WorkItemTrackingRequestContext witRequestContext = requestContext.WitContext();
        foreach (KeyValuePair<string, object> update in updates)
          this.SetFieldValue(witRequestContext, update.Key, update.Value);
      }));
    }

    internal void SetFieldUpdates(
      WorkItemTrackingRequestContext witRequestContext,
      IEnumerable<KeyValuePair<string, object>> updates)
    {
      foreach (KeyValuePair<string, object> update in updates)
        this.SetFieldValue(witRequestContext, update.Key, update.Value);
    }

    public void SetFieldUpdates(
      IVssRequestContext requestContext,
      IEnumerable<KeyValuePair<int, object>> updates)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IEnumerable<KeyValuePair<int, object>>>(updates, nameof (updates));
      this.SetFieldUpdates(requestContext.WitContext(), updates);
    }

    internal void SetFieldUpdates(
      WorkItemTrackingRequestContext witRequestContext,
      IEnumerable<KeyValuePair<int, object>> updates)
    {
      foreach (KeyValuePair<int, object> update in updates)
        this.SetFieldValue(witRequestContext, update.Key, update.Value);
    }

    internal void ClearFieldUpdates() => this.m_updates?.Clear();

    public void SetFieldValue(IVssRequestContext requestContext, string fieldRefName, object value)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<string>(fieldRefName, nameof (fieldRefName));
      this.SetFieldValue(requestContext.WitContext(), fieldRefName, value);
    }

    internal void SetFieldValue(
      WorkItemTrackingRequestContext witRequestContext,
      string fieldRefName,
      object value)
    {
      FieldEntry fieldByNameOrId = witRequestContext.FieldDictionary.GetFieldByNameOrId(fieldRefName);
      this.SetFieldValue(witRequestContext, fieldByNameOrId, value);
    }

    public void SetFieldValue(IVssRequestContext requestContext, int fieldId, object value)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      this.SetFieldValue(requestContext.WitContext(), fieldId, value);
    }

    internal void SetFieldValue(
      WorkItemTrackingRequestContext witRequestContext,
      int fieldId,
      object value)
    {
      FieldEntry field = witRequestContext.FieldDictionary.GetField(fieldId);
      this.SetFieldValue(witRequestContext, field, value);
    }

    internal void SetFieldValue(
      WorkItemTrackingRequestContext witRequestContext,
      FieldEntry field,
      object value)
    {
      IVssRequestContext requestContext = witRequestContext.RequestContext;
      int fieldId1 = field.FieldId;
      int fieldId2;
      if (fieldId1 <= -42)
      {
        if (fieldId1 != -105)
        {
          if ((uint) (fieldId1 - -56) <= 14U)
            return;
        }
        else
        {
          if (!(value is string str))
            str = string.Empty;
          string absolutePath = str;
          value = (object) -1;
          if (!string.IsNullOrEmpty(absolutePath))
            value = (object) witRequestContext.TreeService.LegacyGetTreeNodeIdFromPath(witRequestContext.RequestContext, absolutePath, TreeStructureType.Iteration);
          fieldId2 = -104;
          goto label_22;
        }
      }
      else
      {
        if (fieldId1 == -12)
          return;
        if (fieldId1 == -7)
        {
          if (!(value is string str))
            str = string.Empty;
          string absolutePath = str;
          value = (object) -1;
          if (!string.IsNullOrEmpty(absolutePath))
            value = (object) witRequestContext.TreeService.LegacyGetTreeNodeIdFromPath(witRequestContext.RequestContext, absolutePath, TreeStructureType.Area);
          fieldId2 = -2;
          goto label_22;
        }
      }
      fieldId2 = field.FieldId;
      if (field.IsHtml)
      {
        string str = value as string;
        if (!string.IsNullOrWhiteSpace(str))
        {
          if (!WorkItemFieldData.c_SafeHtmlExcludedFields.Contains(field.ReferenceName))
          {
            bool flag = WorkItemTrackingFeatureFlags.IsMarkdownDiscussionEnabled(requestContext) && requestContext.IsContributedFeatureEnabled("ms.vss-work-web.new-boards-hub-feature");
            str = !(field.FieldType == InternalFieldType.History & flag) ? SafeHtmlWrapper.MakeSafe(str) : SafeHtmlWrapper.MakeSafeWithMdMentions(str);
          }
          value = (object) str;
        }
      }
label_22:
      this.SetFieldValueInternal<object>(fieldId2, value);
    }

    protected void SetFieldValueInternal<T>(int fieldId, T value)
    {
      object obj = (object) value;
      object tag = fieldId != 54 ? this.GetFieldValueInternal(fieldId, true) : (object) null;
      if (fieldId == 80)
      {
        tag = (object) WorkItemFieldData.TrimAndSort(tag as string);
        obj = (object) WorkItemFieldData.TrimAndSort((object) value as string);
      }
      if (!WorkItemFieldData.AreFieldValuesEqual(tag, obj))
      {
        this.Updates[fieldId] = (object) value;
      }
      else
      {
        if (this.m_updates == null)
          return;
        this.m_updates.Remove(fieldId);
      }
    }

    private static string TrimAndSort(string tag) => tag != null ? Microsoft.TeamFoundation.Core.WebApi.TaggingHelper.FormatTagsValue((IEnumerable<string>) Microsoft.TeamFoundation.Core.WebApi.TaggingHelper.SplitTagText(tag).OrderBy<string, string>((Func<string, string>) (t => t))).ToLowerInvariant() : (string) null;

    private static bool IsFieldValueEmpty(object value) => value == null || value as string == string.Empty;

    private static bool AreFieldValuesEqual(object value1, object value2)
    {
      bool flag1 = WorkItemFieldData.IsFieldValueEmpty(value1);
      bool flag2 = WorkItemFieldData.IsFieldValueEmpty(value2);
      if (flag1 & flag2)
        return true;
      return !(flag1 | flag2) && object.Equals(value1, value2);
    }

    public virtual object GetFieldValue(
      IVssRequestContext requestContext,
      string fieldReferenceName,
      bool originalValue = false)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckStringForNullOrEmpty(fieldReferenceName, nameof (fieldReferenceName));
      return this.GetFieldValue(requestContext.WitContext(), fieldReferenceName, originalValue);
    }

    internal virtual object GetFieldValue(
      WorkItemTrackingRequestContext witRequestContext,
      string fieldReferenceName,
      bool originalValue = false)
    {
      FieldEntry field;
      return witRequestContext.FieldDictionary.TryGetField(fieldReferenceName, out field) ? this.GetFieldValue(witRequestContext, field.FieldId, originalValue) : (object) null;
    }

    internal virtual object GetFieldValue(
      WorkItemTrackingRequestContext witRequestContext,
      int fieldId)
    {
      return this.GetFieldValue(witRequestContext, fieldId, false);
    }

    public object GetFieldValue(IVssRequestContext requestContext, int fieldId, bool oldValue)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      return this.GetFieldValue(requestContext.WitContext(), fieldId, oldValue);
    }

    internal virtual object GetFieldValue(
      WorkItemTrackingRequestContext witRequestContext,
      int fieldId,
      bool oldValue)
    {
      ITreeDictionary treeService = witRequestContext.TreeService;
      IVssRequestContext requestContext = witRequestContext.RequestContext;
      switch (fieldId)
      {
        case -105:
          TreeNode node1;
          return treeService.LegacyTryGetTreeNode(this.GetFieldValue<int>(witRequestContext, -104, oldValue), out node1) ? (object) node1.GetPath(witRequestContext.RequestContext) : (object) null;
        case -56:
        case -55:
        case -54:
        case -53:
        case -52:
        case -51:
        case -50:
          TreeNode node2;
          return treeService.LegacyTryGetTreeNode(this.GetFieldValue<int>(witRequestContext, -104, oldValue), out node2) ? (object) this.GetTreeLevelName(witRequestContext.RequestContext, node2, fieldId) : (object) null;
        case -49:
        case -48:
        case -47:
        case -46:
        case -45:
        case -44:
        case -43:
          TreeNode node3;
          return treeService.LegacyTryGetTreeNode(this.GetFieldValue<int>(witRequestContext, -2, oldValue), out node3) ? (object) this.GetTreeLevelName(witRequestContext.RequestContext, node3, fieldId) : (object) null;
        case -42:
          if (this.m_projectId.HasValue)
          {
            try
            {
              return (object) witRequestContext.GetProjectName(this.m_projectId.Value);
            }
            catch
            {
            }
          }
          TreeNode node4;
          if (!treeService.LegacyTryGetTreeNode(this.GetFieldValue<int>(witRequestContext, -2, oldValue), out node4))
            return (object) null;
          TreeNode project = node4.Project;
          return project == null ? (object) null : (object) project.GetName(witRequestContext.RequestContext);
        case -12:
          TreeNode node5;
          return treeService.LegacyTryGetTreeNode(this.GetFieldValue<int>(witRequestContext, -2, oldValue), out node5) ? (object) node5.GetName(witRequestContext.RequestContext) : (object) null;
        case -7:
          TreeNode node6;
          return treeService.LegacyTryGetTreeNode(this.GetFieldValue<int>(witRequestContext, -2, oldValue), out node6) ? (object) node6.GetPath(witRequestContext.RequestContext) : (object) null;
        default:
          return this.GetFieldValueInternal(fieldId, oldValue);
      }
    }

    protected object GetFieldValueInternal(int fieldId, bool oldValue)
    {
      object obj;
      return !oldValue && this.m_updates != null && this.m_updates.TryGetValue(fieldId, out obj) || this.LatestData.TryGetValue(fieldId, out obj) ? obj : (object) null;
    }

    public T GetFieldValue<T>(IVssRequestContext requestContext, int fieldId, bool oldValue = false)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      return this.GetFieldValue<T>(requestContext.WitContext(), fieldId, oldValue);
    }

    internal T GetFieldValue<T>(
      WorkItemTrackingRequestContext witRequestContext,
      int fieldId,
      bool oldValue = false)
    {
      return CommonWITUtils.ConvertValue<T>(this.GetFieldValue(witRequestContext, fieldId, oldValue));
    }

    public virtual T GetFieldValue<T>(
      IVssRequestContext requestContext,
      string fieldReferenceName,
      bool oldValue = false)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      return this.GetFieldValue<T>(requestContext.WitContext(), fieldReferenceName, oldValue);
    }

    internal T GetFieldValue<T>(
      WorkItemTrackingRequestContext witRequestContext,
      string fieldReferenceName,
      bool oldValue = false)
    {
      return CommonWITUtils.ConvertValue<T>(this.GetFieldValue(witRequestContext, fieldReferenceName, oldValue));
    }

    protected T GetFieldValueInternal<T>(int fieldId, bool oldValue) => CommonWITUtils.ConvertValue<T>(this.GetFieldValueInternal(fieldId, oldValue));

    internal IEnumerable<KeyValuePair<FieldEntry, object>> GetUpdatesByFieldEntry(
      WorkItemTrackingRequestContext witRequestContext,
      bool includeCalculatedFields = false)
    {
      if (!this.HasUpdates)
        return Enumerable.Empty<KeyValuePair<FieldEntry, object>>();
      IFieldTypeDictionary fieldDict = witRequestContext.FieldDictionary;
      IEnumerable<KeyValuePair<FieldEntry, object>> fieldUpdates = this.m_updates.Select<KeyValuePair<int, object>, KeyValuePair<FieldEntry, object>>((Func<KeyValuePair<int, object>, KeyValuePair<FieldEntry, object>>) (pair => new KeyValuePair<FieldEntry, object>(fieldDict.GetField(pair.Key), pair.Value)));
      return includeCalculatedFields ? fieldUpdates.SelectMany<KeyValuePair<FieldEntry, object>, KeyValuePair<FieldEntry, object>>((Func<KeyValuePair<FieldEntry, object>, IEnumerable<KeyValuePair<FieldEntry, object>>>) (fieldUpdate => this.GetAssociatedFieldData(witRequestContext, fieldUpdate, fieldUpdates, true))) : fieldUpdates;
    }

    internal IEnumerable<KeyValuePair<FieldEntry, object>> GetAllFieldValuesByFieldEntry(
      WorkItemTrackingRequestContext witRequestContext,
      bool includeCalculatedFields = false)
    {
      IEnumerable<int> source;
      if (this.HasUpdates)
      {
        HashSet<int> intSet = new HashSet<int>((IEnumerable<int>) this.LatestData.Keys);
        intSet.UnionWith((IEnumerable<int>) this.m_updates.Keys);
        source = (IEnumerable<int>) intSet;
      }
      else
        source = (IEnumerable<int>) this.LatestData.Keys;
      if (!source.Any<int>())
        return Enumerable.Empty<KeyValuePair<FieldEntry, object>>();
      IFieldTypeDictionary fieldDict = witRequestContext.FieldDictionary;
      IEnumerable<KeyValuePair<FieldEntry, object>> fieldValues = source.Select<int, KeyValuePair<FieldEntry, object>>((Func<int, KeyValuePair<FieldEntry, object>>) (fieldId => new KeyValuePair<FieldEntry, object>(fieldDict.GetField(fieldId), this.GetFieldValue(witRequestContext, fieldId, false))));
      return includeCalculatedFields ? fieldValues.SelectMany<KeyValuePair<FieldEntry, object>, KeyValuePair<FieldEntry, object>>((Func<KeyValuePair<FieldEntry, object>, IEnumerable<KeyValuePair<FieldEntry, object>>>) (fd => this.GetAssociatedFieldData(witRequestContext, fd, fieldValues, false))) : fieldValues;
    }

    private string GetTreeLevelName(
      IVssRequestContext requestContext,
      TreeNode node,
      int levelFieldId)
    {
      int num = levelFieldId;
      switch (levelFieldId)
      {
        case -56:
          num = -49;
          break;
        case -55:
          num = -48;
          break;
        case -54:
          num = -47;
          break;
        case -53:
          num = -46;
          break;
        case -52:
          num = -45;
          break;
        case -51:
          num = -44;
          break;
        case -50:
        case -43:
          num = -42;
          break;
      }
      for (; node != null; node = node.Parent)
      {
        if (node.TypeId == num)
          return node.GetName(requestContext);
      }
      return (string) null;
    }

    private IEnumerable<KeyValuePair<FieldEntry, object>> GetAssociatedFieldData(
      WorkItemTrackingRequestContext witRequestContext,
      KeyValuePair<FieldEntry, object> fieldData,
      IEnumerable<KeyValuePair<FieldEntry, object>> fieldDataCollection,
      bool filterUnchanged)
    {
      IFieldTypeDictionary fieldDict = witRequestContext.FieldDictionary;
      yield return fieldData;
      int[] numArray;
      int index;
      if (fieldData.Key.FieldId == -2)
      {
        if (!fieldDataCollection.Any<KeyValuePair<FieldEntry, object>>((Func<KeyValuePair<FieldEntry, object>, bool>) (kvp => kvp.Key.FieldId.Equals(-7))))
          yield return new KeyValuePair<FieldEntry, object>(fieldDict.GetField(-7), this.GetFieldValue(witRequestContext, -7, false));
        if (!fieldDataCollection.Any<KeyValuePair<FieldEntry, object>>((Func<KeyValuePair<FieldEntry, object>, bool>) (kvp => kvp.Key.FieldId.Equals(-42))))
        {
          KeyValuePair<FieldEntry, object> fieldData1 = this.GetFieldData(witRequestContext, -42, filterUnchanged);
          if (fieldData1.Key != null)
            yield return fieldData1;
        }
        yield return new KeyValuePair<FieldEntry, object>(fieldDict.GetField(-12), this.GetFieldValue(witRequestContext, -12, false));
        numArray = new int[7]
        {
          -43,
          -44,
          -45,
          -46,
          -47,
          -48,
          -49
        };
        for (index = 0; index < numArray.Length; ++index)
        {
          int fieldId = numArray[index];
          KeyValuePair<FieldEntry, object> fieldData2 = this.GetFieldData(witRequestContext, fieldId, filterUnchanged);
          if (fieldData2.Key != null)
            yield return fieldData2;
        }
        numArray = (int[]) null;
      }
      if (fieldData.Key.FieldId == -104)
      {
        if (!fieldDataCollection.Any<KeyValuePair<FieldEntry, object>>((Func<KeyValuePair<FieldEntry, object>, bool>) (kvp => kvp.Key.FieldId.Equals(-105))))
          yield return new KeyValuePair<FieldEntry, object>(fieldDict.GetField(-105), this.GetFieldValue(witRequestContext, -105, false));
        numArray = new int[7]
        {
          -50,
          -51,
          -52,
          -53,
          -54,
          -55,
          -56
        };
        for (index = 0; index < numArray.Length; ++index)
        {
          int fieldId = numArray[index];
          KeyValuePair<FieldEntry, object> fieldData3 = this.GetFieldData(witRequestContext, fieldId, filterUnchanged);
          if (fieldData3.Key != null)
            yield return fieldData3;
        }
        numArray = (int[]) null;
      }
    }

    private KeyValuePair<FieldEntry, object> GetFieldData(
      WorkItemTrackingRequestContext witRequestContext,
      int fieldId,
      bool filterUnchanged)
    {
      IFieldTypeDictionary fieldDictionary = witRequestContext.FieldDictionary;
      object fieldValue = this.GetFieldValue(witRequestContext, fieldId, false);
      return filterUnchanged && this.GetFieldValue(witRequestContext, fieldId, true) != fieldValue || !filterUnchanged && fieldValue != null ? new KeyValuePair<FieldEntry, object>(fieldDictionary.GetField(fieldId), fieldValue) : new KeyValuePair<FieldEntry, object>((FieldEntry) null, (object) null);
    }

    private IEnumerable<KeyValuePair<FieldEntry, object>> GetAssociatedFieldData(
      KeyValuePair<FieldEntry, object> fieldData,
      IFieldTypeDictionary fieldDict,
      bool filterUnchanged)
    {
      yield return fieldData;
      int[] numArray;
      int index;
      if (fieldData.Key.FieldId == -2)
      {
        yield return new KeyValuePair<FieldEntry, object>(fieldDict.GetField(-7), this.GetFieldValueInternal(-7, false));
        yield return new KeyValuePair<FieldEntry, object>(fieldDict.GetField(-12), this.GetFieldValueInternal(-12, false));
        numArray = new int[8]
        {
          -42,
          -43,
          -44,
          -45,
          -46,
          -47,
          -48,
          -49
        };
        for (index = 0; index < numArray.Length; ++index)
        {
          int num = numArray[index];
          object fieldValueInternal = this.GetFieldValueInternal(num, false);
          if (!filterUnchanged || this.GetFieldValueInternal(num, true) != fieldValueInternal)
            yield return new KeyValuePair<FieldEntry, object>(fieldDict.GetField(num), fieldValueInternal);
        }
        numArray = (int[]) null;
      }
      if (fieldData.Key.FieldId == -104)
      {
        yield return new KeyValuePair<FieldEntry, object>(fieldDict.GetField(-105), this.GetFieldValueInternal(-105, false));
        numArray = new int[7]
        {
          -50,
          -51,
          -52,
          -53,
          -54,
          -55,
          -56
        };
        for (index = 0; index < numArray.Length; ++index)
        {
          int num = numArray[index];
          object fieldValueInternal = this.GetFieldValueInternal(num, false);
          if (!filterUnchanged || this.GetFieldValueInternal(num, true) != fieldValueInternal)
            yield return new KeyValuePair<FieldEntry, object>(fieldDict.GetField(num), fieldValueInternal);
        }
        numArray = (int[]) null;
      }
    }

    private class FieldDataPredicateEvaluationContext : 
      IExtendedPredicateEvaluationContext,
      IPredicateEvaluationHelper,
      IPredicateValidationHelper
    {
      private WorkItemTrackingRequestContext m_witRequestContext;
      private WorkItemFieldData m_fieldData;
      private Dictionary<string, bool> m_witInCategory;

      public FieldDataPredicateEvaluationContext(
        WorkItemTrackingRequestContext witRequestContext,
        WorkItemFieldData fieldData)
      {
        this.m_witRequestContext = witRequestContext;
        this.m_fieldData = fieldData;
        this.m_witInCategory = new Dictionary<string, bool>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      }

      public object GetFieldValue(int fieldId) => this.m_fieldData.GetFieldValue(this.m_witRequestContext, fieldId, false);

      public InternalFieldType GetFieldType(string fieldReferenceName)
      {
        FieldEntry field;
        return this.m_witRequestContext.FieldDictionary.TryGetField(fieldReferenceName, out field) ? field.FieldType : InternalFieldType.String;
      }

      public int GetFieldId(string fieldReferenceName)
      {
        FieldEntry field;
        return this.m_witRequestContext.FieldDictionary.TryGetField(fieldReferenceName, out field) ? field.FieldId : 0;
      }

      public string GetTreePath(int treeId)
      {
        TreeNode node;
        return this.m_witRequestContext.TreeService.LegacyTryGetTreeNode(treeId, out node) ? node.GetPath(this.m_witRequestContext.RequestContext) : (string) null;
      }

      public bool IsWorkItemTypeInCategory(string workItemType, string categoryId)
      {
        string key = workItemType + "/" + categoryId;
        bool flag;
        if (!this.m_witInCategory.TryGetValue(key, out flag))
        {
          Guid projectId = this.m_fieldData.GetProjectGuid(this.m_witRequestContext);
          WorkItemTypeCategory workItemTypeCategory;
          flag = this.m_witRequestContext.GetOrAddCacheItem<HashSet<string>>("WorkItemTypeNamesPerCategory/" + projectId.ToString("D") + "/" + categoryId, (Func<HashSet<string>>) (() => this.m_witRequestContext.RequestContext.GetService<IWorkItemTypeCategoryService>().TryGetWorkItemTypeCategory(this.m_witRequestContext.RequestContext, projectId, categoryId, out workItemTypeCategory) ? new HashSet<string>(workItemTypeCategory.WorkItemTypeNames, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) : new HashSet<string>(Enumerable.Empty<string>()))).Contains(workItemType);
          this.m_witInCategory[key] = flag;
        }
        return flag;
      }

      public int GetTreeId(string path, TreeStructureType type) => this.m_witRequestContext.TreeService.LegacyGetTreeNodeIdFromPath(this.m_witRequestContext.RequestContext, path, type);

      public bool IsTreeNodeUnder(int parentTreeId, int childTreeId)
      {
        TreeNode node;
        if (this.m_witRequestContext.TreeService.LegacyTryGetTreeNode(childTreeId, out node))
        {
          for (; node != null; node = node.Parent)
          {
            if (node.Id == parentTreeId)
              return true;
          }
        }
        return false;
      }

      public TreeNode GetTreeNode(int treeId)
      {
        TreeNode node;
        return this.m_witRequestContext.TreeService.LegacyTryGetTreeNode(treeId, out node) ? node : (TreeNode) null;
      }
    }

    internal class FieldDataRuleEvaluationContext : IRuleEvaluationContext
    {
      private WorkItemTrackingRequestContext m_witRequestContext;
      private WorkItemFieldData m_fieldData;
      private int? m_firstInvalidFieldId;
      private int? m_firstFieldRequiresPendingCheck;
      private Lazy<Dictionary<int, FieldRuleEvalutionStatus>> m_lazyRuleEvalutionStatuses;
      private IDictionary<string, Microsoft.VisualStudio.Services.Identity.Identity> m_identityMap;
      private ResolvedIdentityNamesInfo m_resolvedIdentityNamesInfo;

      public FieldDataRuleEvaluationContext(
        WorkItemTrackingRequestContext witRequestContext,
        WorkItemFieldData fieldData)
      {
        this.m_witRequestContext = witRequestContext;
        this.m_fieldData = fieldData;
        this.m_lazyRuleEvalutionStatuses = new Lazy<Dictionary<int, FieldRuleEvalutionStatus>>((Func<Dictionary<int, FieldRuleEvalutionStatus>>) (() => new Dictionary<int, FieldRuleEvalutionStatus>()));
        this.m_identityMap = witRequestContext.GetIdentityMap();
        this.m_resolvedIdentityNamesInfo = new ResolvedIdentityNamesInfo();
      }

      object IRuleEvaluationContext.GetCurrentFieldValue(int fieldId)
      {
        object currentFieldValue = this.GetFieldValue(fieldId, false);
        if (currentFieldValue != null && fieldId == 8)
        {
          switch (currentFieldValue)
          {
            case int _:
            case long _:
              if (Convert.ToInt64(currentFieldValue) == 0L)
              {
                currentFieldValue = (object) null;
                break;
              }
              break;
            default:
              FieldEntry field = this.m_witRequestContext.FieldDictionary.GetField(fieldId);
              throw new VssPropertyValidationException(currentFieldValue.ToString(), Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.FieldInvalidIntegerValue((object) field.Name));
          }
        }
        return currentFieldValue;
      }

      object IRuleEvaluationContext.GetOriginalFieldValue(int fieldId) => this.GetFieldValue(fieldId, true);

      bool IRuleEvaluationContext.TryComputeFieldValue(
        int fieldId,
        int parentFieldId,
        out object value)
      {
        return this.TryComputeFieldValue(fieldId, parentFieldId, false, out value);
      }

      string IRuleEvaluationContext.GetCurrentUser() => ((IRuleEvaluationContext) this).ServerDefaultValueTransformer.CurrentUser;

      Microsoft.VisualStudio.Services.Identity.Identity IRuleEvaluationContext.GetCurrentIdentity() => ((IRuleEvaluationContext) this).ServerDefaultValueTransformer.CurrentIdentity;

      InternalFieldType IRuleEvaluationContext.GetFieldType(int fieldId) => this.m_witRequestContext.FieldDictionary.GetField(fieldId).FieldType;

      bool IRuleEvaluationContext.IsIdentityField(int fieldId) => this.m_witRequestContext.FieldDictionary.GetField(fieldId).IsIdentity;

      FieldStatusFlags IRuleEvaluationContext.GetFieldFlags(int fieldId) => this.GetFieldFlags(fieldId);

      private object GetFieldValue(int fieldId, bool oldValue)
      {
        if (fieldId != 54)
          return this.m_fieldData.GetFieldValue(this.m_witRequestContext, fieldId, oldValue);
        object obj;
        return oldValue || this.m_fieldData.m_updates == null || !this.m_fieldData.m_updates.TryGetValue(fieldId, out obj) ? (object) null : obj;
      }

      private FieldStatusFlags GetFieldFlags(int fieldId)
      {
        FieldRuleEvalutionStatus ruleEvalutionStatus;
        return this.m_lazyRuleEvalutionStatuses.IsValueCreated && this.m_lazyRuleEvalutionStatuses.Value.TryGetValue(fieldId, out ruleEvalutionStatus) ? ruleEvalutionStatus.Flags : FieldStatusFlags.None;
      }

      bool IRuleEvaluationContext.IsAreaPathValid(string path) => this.m_witRequestContext.RequestContext.TraceBlock<bool>(904551, 904560, 904555, "Services", "WorkItemService", "WorkItemFieldData.IsAreaPathValid", (Func<bool>) (() => this.m_witRequestContext.TreeService.LegacyGetTreeNodeIdFromPath(this.m_witRequestContext.RequestContext, path, TreeStructureType.Area) > 0));

      bool IRuleEvaluationContext.IsIterationPathValid(string path) => this.m_witRequestContext.RequestContext.TraceBlock<bool>(904561, 904570, 904565, "Services", "WorkItemService", "WorkItemFieldData.IsIterationPathValid", (Func<bool>) (() => this.m_witRequestContext.TreeService.LegacyGetTreeNodeIdFromPath(this.m_witRequestContext.RequestContext, path, TreeStructureType.Iteration) > 0));

      void IRuleEvaluationContext.SetFieldRuleEvalutionStatus(FieldRuleEvalutionStatus status) => this.m_witRequestContext.RequestContext.TraceBlock(904541, 904550, 904545, "Services", "WorkItemService", "WorkItemFieldData.SetFieldRuleEvalutionStatus", (Action) (() =>
      {
        if ((status.Flags & FieldStatusFlags.InvalidMask) != FieldStatusFlags.None && !this.m_firstInvalidFieldId.HasValue)
          this.m_firstInvalidFieldId = new int?(status.FieldId);
        if ((status.Flags & FieldStatusFlags.PendingListCheck) != FieldStatusFlags.None && !this.m_firstFieldRequiresPendingCheck.HasValue)
          this.m_firstFieldRequiresPendingCheck = new int?(status.FieldId);
        this.m_lazyRuleEvalutionStatuses.Value[status.FieldId] = status;
        this.m_fieldData.SetFieldValue(this.m_witRequestContext, status.FieldId, status.Value);
      }));

      void IRuleEvaluationContext.ClearPendingListChecks(IEnumerable<int> exceptFieldIds)
      {
        if (this.m_lazyRuleEvalutionStatuses.IsValueCreated)
        {
          Dictionary<int, FieldRuleEvalutionStatus> dictionary = this.m_lazyRuleEvalutionStatuses.Value;
          int[] array = dictionary.Values.Where<FieldRuleEvalutionStatus>((Func<FieldRuleEvalutionStatus, bool>) (status => (status.Flags & FieldStatusFlags.PendingListCheck) != 0)).Select<FieldRuleEvalutionStatus, int>((Func<FieldRuleEvalutionStatus, int>) (status => status.FieldId)).ToArray<int>();
          HashSet<int> intSet = new HashSet<int>(exceptFieldIds);
          foreach (int key in array)
          {
            FieldRuleEvalutionStatus ruleEvalutionStatus = dictionary[key];
            ruleEvalutionStatus.Flags &= ~FieldStatusFlags.PendingListCheck;
            if (intSet.Contains(key))
            {
              if (!this.m_firstInvalidFieldId.HasValue)
                this.m_firstInvalidFieldId = new int?(key);
              ruleEvalutionStatus.Flags |= FieldStatusFlags.InvalidListValue;
            }
            dictionary[key] = ruleEvalutionStatus;
          }
        }
        this.m_firstFieldRequiresPendingCheck = new int?();
      }

      IWorkItemRuleFilter IRuleEvaluationContext.RuleFilter => (IWorkItemRuleFilter) this.m_witRequestContext.RuleMembershipFilter;

      IServerDefaultValueTransformer IRuleEvaluationContext.ServerDefaultValueTransformer => this.m_witRequestContext.ServerDefaultValueTransformer;

      int? IRuleEvaluationContext.FirstInvalidFieldId => this.m_firstInvalidFieldId;

      internal void SetFirstInvalidFieldId(int? id) => this.m_firstInvalidFieldId = id;

      int? IRuleEvaluationContext.FirstFieldRequiresPendingCheck => this.m_firstFieldRequiresPendingCheck;

      IDictionary<int, FieldRuleEvalutionStatus> IRuleEvaluationContext.RuleEvaluationStatuses => (IDictionary<int, FieldRuleEvalutionStatus>) this.m_lazyRuleEvalutionStatuses.Value;

      public IVssRequestContext RequestContext => this.m_witRequestContext.RequestContext;

      string IRuleEvaluationContext.ResolveIdentityValue(int fieldId, object value)
      {
        Microsoft.VisualStudio.Services.Identity.Identity identity1 = (Microsoft.VisualStudio.Services.Identity.Identity) null;
        switch (value)
        {
          case string _ when this.m_identityMap.TryGetValue((string) value, out identity1):
            return (string) value;
          case string _:
            ResolvedIdentityNamesInfo identityNamesInfo = this.m_witRequestContext.RequestContext.GetService<IWorkItemIdentityService>().ResolveIdentityNames(this.m_witRequestContext, (IEnumerable<string>) new string[1]
            {
              (string) value
            }, false);
            if (identityNamesInfo.NamesLookup.Keys.Any<string>())
            {
              IDictionary<string, Microsoft.VisualStudio.Services.Identity.Identity> dictionary = identityNamesInfo.IdentityMap.Value;
              if (dictionary.Keys.Count == 1)
              {
                string key = dictionary.Keys.First<string>();
                Microsoft.VisualStudio.Services.Identity.Identity identity2 = dictionary[key];
                this.m_identityMap[(string) value] = identity2;
                this.m_identityMap[key] = identity2;
                WorkItemIdentityHelper.AddVsidToDistinctDisplayNameMap(this.m_witRequestContext.RequestContext, identity2.Id);
                return key;
              }
              break;
            }
            break;
        }
        return (string) null;
      }

      bool IRuleEvaluationContext.IsRealIdentity(string value) => this.m_identityMap.TryGetValue(value, out Microsoft.VisualStudio.Services.Identity.Identity _);

      bool IRuleEvaluationContext.IsIdentityFieldValueAmbiguous(object value) => value is string && this.m_resolvedIdentityNamesInfo.AmbiguousNamesLookup.ContainsKey((string) value);

      Microsoft.VisualStudio.Services.Identity.Identity[] IRuleEvaluationContext.GetAmbiguousIdentities(
        object value)
      {
        List<Microsoft.VisualStudio.Services.Identity.Identity> source = new List<Microsoft.VisualStudio.Services.Identity.Identity>();
        if (value is string && this.m_resolvedIdentityNamesInfo.AmbiguousNamesLookup.ContainsKey((string) value))
        {
          foreach (IdentityConstantRecord identityConstantRecord in ((IEnumerable<ConstantsSearchRecord>) this.m_resolvedIdentityNamesInfo.AmbiguousNamesLookup[(string) value]).Where<ConstantsSearchRecord>((Func<ConstantsSearchRecord, bool>) (record => record.TeamFoundationId != Guid.Empty)).ToArray<ConstantsSearchRecord>())
          {
            Microsoft.VisualStudio.Services.Identity.Identity identity;
            if (this.m_identityMap.TryGetValue(identityConstantRecord.DisplayPart, out identity))
              source.Add(identity);
          }
        }
        if (value is string && !source.Any<Microsoft.VisualStudio.Services.Identity.Identity>() && this.m_resolvedIdentityNamesInfo.NamesLookup.ContainsKey((string) value) && this.m_resolvedIdentityNamesInfo.NamesLookup[(string) value].TeamFoundationId == Guid.Empty)
        {
          foreach (IdentityConstantRecord identityConstantRecord in this.m_resolvedIdentityNamesInfo.AllRecords.Where<ConstantsSearchRecord>((Func<ConstantsSearchRecord, bool>) (record => record.TeamFoundationId != Guid.Empty && record.DisplayPart != null && record.DisplayPart.StartsWith((string) value + " <", StringComparison.OrdinalIgnoreCase))))
          {
            Microsoft.VisualStudio.Services.Identity.Identity identity;
            if (this.m_identityMap.TryGetValue(identityConstantRecord.DisplayPart, out identity))
              source.Add(identity);
          }
        }
        return source.ToArray();
      }

      IdentityDisplayType IRuleEvaluationContext.GetIdentityDisplayType() => this.m_witRequestContext.RequestContext.GetIdentityDisplayType();

      bool IRuleEvaluationContext.IsIdentityMemberOfGroup(
        string value,
        ConstantSetReference setReference)
      {
        if (setReference.IdentityDescriptor == (IdentityDescriptor) null && setReference.Id >= 0)
          throw new InvalidOperationException("Set does is not an identity group");
        Microsoft.VisualStudio.Services.Identity.Identity identityToCheck;
        if (!this.m_identityMap.TryGetValue(value, out identityToCheck))
          throw new InvalidOperationException("Unable to resolve identity");
        List<IdentityDescriptor> source = new List<IdentityDescriptor>();
        bool flag1 = !setReference.ExcludeGroups;
        bool flag2 = true;
        if (setReference.IdentityDescriptor != (IdentityDescriptor) null)
        {
          source.Add(setReference.IdentityDescriptor);
        }
        else
        {
          switch (setReference.Id)
          {
            case -30:
              flag1 = true;
              source.Add(GroupWellKnownIdentityDescriptors.NamespaceAdministratorsGroup);
              source.Add(GroupWellKnownIdentityDescriptors.ServiceUsersGroup);
              break;
            case -10:
              flag1 = true;
              flag2 = false;
              source.Add(GroupWellKnownIdentityDescriptors.EveryoneGroup);
              break;
            case -2:
              flag1 = false;
              source.Add(GroupWellKnownIdentityDescriptors.EveryoneGroup);
              break;
            case -1:
              flag1 = true;
              source.Add(GroupWellKnownIdentityDescriptors.EveryoneGroup);
              break;
            default:
              throw new InvalidOperationException();
          }
        }
        if (!flag1 && identityToCheck.IsContainer || !flag2 && !identityToCheck.IsContainer)
          return false;
        Func<IdentityDescriptor, bool> predicate = (Func<IdentityDescriptor, bool>) (descriptor =>
        {
          if (setReference.IncludeTop && IdentityDescriptorComparer.Instance.Equals(descriptor, identityToCheck.Descriptor) || IdentityDescriptorComparer.Instance.Equals(descriptor, GroupWellKnownIdentityDescriptors.EveryoneGroup) && ServicePrincipals.IsServicePrincipal(this.m_witRequestContext.RequestContext, identityToCheck.Descriptor))
            return true;
          bool flag3 = this.m_witRequestContext.IdentityService.IsMember(this.m_witRequestContext.RequestContext, descriptor, identityToCheck.Descriptor);
          this.m_witRequestContext.RequestContext.Trace(904648, TraceLevel.Verbose, "Services", "WorkItemService", string.Format("IsMember: {0}, GroupDescriptor: {1},  MemberDescriptor: {2} ", (object) flag3, (object) descriptor, (object) identityToCheck.Descriptor));
          return flag3;
        });
        return source.Any<IdentityDescriptor>(predicate);
      }

      internal bool TryComputeFieldValue(
        int fieldId,
        int parentFieldId,
        bool oldValue,
        out object value)
      {
        value = (object) null;
        object fieldValue = this.m_fieldData.GetFieldValue(this.m_witRequestContext, parentFieldId, oldValue);
        switch (fieldId)
        {
          case -105:
          case -7:
            TreeNode node;
            value = !this.m_witRequestContext.TreeService.LegacyTryGetTreeNode(Convert.ToInt32(fieldValue), out node) ? (object) null : (object) node.GetPath(this.m_witRequestContext.RequestContext);
            return true;
          case -104:
            value = (object) this.m_witRequestContext.TreeService.LegacyGetTreeNodeIdFromPath(this.m_witRequestContext.RequestContext, fieldValue as string, TreeStructureType.Iteration);
            return true;
          case -2:
            value = (object) this.m_witRequestContext.TreeService.LegacyGetTreeNodeIdFromPath(this.m_witRequestContext.RequestContext, fieldValue as string, TreeStructureType.Area);
            return true;
          default:
            return false;
        }
      }

      void IRuleEvaluationContext.UpdateIdentityMap(IDictionary<string, Microsoft.VisualStudio.Services.Identity.Identity> identityMap)
      {
        foreach (KeyValuePair<string, Microsoft.VisualStudio.Services.Identity.Identity> identity in (IEnumerable<KeyValuePair<string, Microsoft.VisualStudio.Services.Identity.Identity>>) identityMap)
          this.m_identityMap[identity.Key] = identity.Value;
      }

      void IRuleEvaluationContext.SetResolvedIdentityNamesInfo(
        ResolvedIdentityNamesInfo resolvedIdentityNamesInfo)
      {
        this.m_resolvedIdentityNamesInfo = resolvedIdentityNamesInfo;
      }
    }
  }
}
