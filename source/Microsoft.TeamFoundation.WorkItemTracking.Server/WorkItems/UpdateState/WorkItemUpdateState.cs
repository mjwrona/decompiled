// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.UpdateState.WorkItemUpdateState
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.Azure.Devops.Tags.Server;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Rules;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.UpdateState
{
  internal class WorkItemUpdateState
  {
    private WorkItemTrackingRequestContext m_witRequestContext;
    private readonly bool m_isNoHistoryEnabledFieldsSupported;
    private readonly ISet<int> m_historyDisabledFieldIds;
    private IRuleEvaluationContext m_ruleEvalContext;
    private List<KeyValuePair<int, object>> m_fieldUpdates;
    private Dictionary<int, object> m_fieldUpdatesMap;
    private bool? m_hasTeamProjectChanged;
    private bool? m_hasWorkItemTypeChanged;
    private bool m_isMarkdownFormatEnabled;

    public WorkItemUpdateState(
      WorkItemTrackingRequestContext witRequestContext,
      WorkItemUpdate update,
      bool isNoHistoryEnabledFieldsSupported = false)
    {
      if (update == null)
        throw new ArgumentNullException(nameof (update));
      this.m_witRequestContext = witRequestContext ?? throw new ArgumentNullException(nameof (witRequestContext));
      this.m_isNoHistoryEnabledFieldsSupported = isNoHistoryEnabledFieldsSupported;
      this.m_historyDisabledFieldIds = witRequestContext.FieldDictionary.GetHistoryDisabledFieldIds();
      this.DBFieldUpdates = (IDictionary<int, object>) new Dictionary<int, object>();
      this.Id = update.Id;
      this.Update = update;
      this.UpdateResult = new WorkItemUpdateResult()
      {
        Id = update.Id,
        UpdateId = update.Id,
        Rev = update.Rev
      };
      this.m_hasTeamProjectChanged = new bool?();
      this.ResourceLinks = (IEnumerable<WorkItemResourceLinkInfo>) new List<WorkItemResourceLinkInfo>();
    }

    public ISet<int> HistoryDisabledFieldIds => this.m_historyDisabledFieldIds;

    public int Id { get; private set; }

    public bool Success => this.UpdateResult == null || this.UpdateResult.Exception == null;

    public WorkItemUpdate Update { get; private set; }

    public DateTime UpdateDate { get; set; }

    public WorkItemUpdateResult UpdateResult { get; private set; }

    public WorkItemFieldData FieldData { get; set; }

    public IEnumerable<WorkItemResourceLinkInfo> ResourceLinks { get; set; }

    public IEnumerable<KeyValuePair<int, object>> FieldUpdates
    {
      get
      {
        this.EnsureFieldUpdates();
        return (IEnumerable<KeyValuePair<int, object>>) this.m_fieldUpdates;
      }
    }

    public IDictionary<int, object> DBFieldUpdates { get; private set; }

    public IEnumerable<TagDefinition> CurrentTags { get; set; }

    public IEnumerable<TagDefinition> AddedTags { get; set; }

    public IEnumerable<TagDefinition> RemovedTags { get; set; }

    public bool HasTeamProjectChanged
    {
      get
      {
        if (!this.m_hasTeamProjectChanged.HasValue)
        {
          WorkItemUpdateState workItemUpdateState = this;
          if (!workItemUpdateState.Update.IsNew && WorkItemTrackingFeatureFlags.IsTeamProjectMoveEnabled(workItemUpdateState.m_witRequestContext.RequestContext))
          {
            string fieldName = this.m_witRequestContext.FieldDictionary.GetField(-42).ReferenceName;
            IEnumerable<KeyValuePair<string, object>> fields = this.Update.Fields;
            var data = fields != null ? fields.Where<KeyValuePair<string, object>>((Func<KeyValuePair<string, object>, bool>) (kvp => TFStringComparer.WorkItemFieldReferenceName.Equals(kvp.Key, fieldName))).Select(kvp => new
            {
              Key = kvp.Key,
              Value = kvp.Value
            }).FirstOrDefault() : null;
            if (data != null)
            {
              if (this.FieldData == null)
                return true;
              this.m_hasTeamProjectChanged = new bool?(!TFStringComparer.TeamProjectName.Equals(this.FieldData.GetFieldValue(this.m_witRequestContext, -42, true), data.Value));
              return this.m_hasTeamProjectChanged.Value;
            }
          }
          this.m_hasTeamProjectChanged = new bool?(false);
        }
        return this.m_hasTeamProjectChanged.Value;
      }
    }

    public bool HasWorkItemTypeChanged
    {
      get
      {
        if (!this.m_hasWorkItemTypeChanged.HasValue)
        {
          if (!this.Update.IsNew && WorkItemTrackingFeatureFlags.IsChangeWorkItemTypeEnabled(this.m_witRequestContext.RequestContext))
          {
            string fieldName = this.m_witRequestContext.FieldDictionary.GetField(25).ReferenceName;
            IEnumerable<KeyValuePair<string, object>> fields = this.Update.Fields;
            var data = fields != null ? fields.Where<KeyValuePair<string, object>>((Func<KeyValuePair<string, object>, bool>) (kvp => TFStringComparer.WorkItemFieldReferenceName.Equals(kvp.Key, fieldName))).Select(kvp => new
            {
              Key = kvp.Key,
              Value = kvp.Value
            }).FirstOrDefault() : null;
            if (data != null)
            {
              if (this.FieldData == null)
                return true;
              this.m_hasWorkItemTypeChanged = new bool?(!TFStringComparer.WorkItemTypeName.Equals(this.FieldData.GetFieldValue(this.m_witRequestContext, 25, true), data.Value));
              return this.m_hasWorkItemTypeChanged.Value;
            }
          }
          this.m_hasWorkItemTypeChanged = new bool?(false);
        }
        return this.m_hasWorkItemTypeChanged.Value;
      }
    }

    public bool HasFieldUpdates
    {
      get
      {
        List<KeyValuePair<int, object>> fieldUpdates = this.m_fieldUpdates;
        return fieldUpdates != null && fieldUpdates.Any<KeyValuePair<int, object>>();
      }
    }

    public bool HasTagsFieldChanges => this.HasFieldUpdates && this.m_fieldUpdatesMap.ContainsKey(80);

    public bool HasAreaChange
    {
      get
      {
        if (!this.HasFieldUpdates)
          return false;
        return this.HasFieldUpdate(-2) || this.HasFieldUpdate(-7);
      }
    }

    public bool HasIterationChange
    {
      get
      {
        if (!this.HasFieldUpdates)
          return false;
        return this.HasFieldUpdate(-104) || this.HasFieldUpdate(-105);
      }
    }

    public bool HasLinkUpdates
    {
      get
      {
        WorkItemUpdate update = this.Update;
        bool? nullable;
        if (update == null)
        {
          nullable = new bool?();
        }
        else
        {
          IEnumerable<WorkItemLinkUpdate> linkUpdates = update.LinkUpdates;
          nullable = linkUpdates != null ? new bool?(linkUpdates.Any<WorkItemLinkUpdate>()) : new bool?();
        }
        return nullable.GetValueOrDefault();
      }
    }

    public bool HasResourceLinkUpdates
    {
      get
      {
        WorkItemUpdate update = this.Update;
        bool? nullable;
        if (update == null)
        {
          nullable = new bool?();
        }
        else
        {
          IEnumerable<WorkItemResourceLinkUpdate> resourceLinkUpdates = update.ResourceLinkUpdates;
          nullable = resourceLinkUpdates != null ? new bool?(resourceLinkUpdates.Any<WorkItemResourceLinkUpdate>()) : new bool?();
        }
        return nullable.GetValueOrDefault();
      }
    }

    public bool HasHtmlFieldUpdates
    {
      get
      {
        List<KeyValuePair<int, object>> fieldUpdates = this.m_fieldUpdates;
        return fieldUpdates != null && fieldUpdates.Any<KeyValuePair<int, object>>((Func<KeyValuePair<int, object>, bool>) (fu => this.m_witRequestContext.FieldDictionary.GetField(fu.Key).IsHtml));
      }
    }

    public bool HasMarkdownCommentUpdate
    {
      private set => this.m_isMarkdownFormatEnabled = value;
      get => this.m_isMarkdownFormatEnabled;
    }

    public bool HasResourceLinkUpdatesRequiringNewRevision
    {
      get
      {
        WorkItemUpdate update = this.Update;
        bool? nullable;
        if (update == null)
        {
          nullable = new bool?();
        }
        else
        {
          IEnumerable<WorkItemResourceLinkUpdate> resourceLinkUpdates = update.ResourceLinkUpdates;
          nullable = resourceLinkUpdates != null ? new bool?(resourceLinkUpdates.Any<WorkItemResourceLinkUpdate>((Func<WorkItemResourceLinkUpdate, bool>) (u => u.UpdateType != LinkUpdateType.Update))) : new bool?();
        }
        return nullable.GetValueOrDefault();
      }
    }

    public bool HasTagUpdates
    {
      get
      {
        if (this.AddedTags != null && this.AddedTags.Any<TagDefinition>())
          return true;
        return this.RemovedTags != null && this.RemovedTags.Any<TagDefinition>();
      }
    }

    public bool HasOnlyLinkUpdates => !this.HasFieldUpdates && !this.HasResourceLinkUpdates && !this.HasTagUpdates;

    public bool? HasNewRevision { get; private set; }

    public IRuleEvaluationContext RuleEvalContext
    {
      get
      {
        if (this.m_ruleEvalContext == null)
          this.m_ruleEvalContext = this.FieldData != null ? this.FieldData.GetRuleEvaluationContext(this.m_witRequestContext) : throw new InvalidOperationException();
        return this.m_ruleEvalContext;
      }
    }

    public WorkItemTypeExtension[] OldExtensions { get; set; }

    public WorkItemTypeExtension[] CurrentExtensions { get; set; }

    public WorkItemCommentUpdateRecord WorkItemComment { get; set; }

    public bool IsNoHistoryEnabledFieldsSupported => this.m_isNoHistoryEnabledFieldsSupported;

    public bool IsEmpty()
    {
      if (!this.HasFieldUpdates)
      {
        WorkItemFieldData fieldData = this.FieldData;
        if ((fieldData != null ? (fieldData.HasUpdates ? 1 : 0) : 0) == 0)
        {
          if (this.HasLinkUpdates)
          {
            WorkItemUpdateResult updateResult = this.UpdateResult;
            bool? nullable;
            if (updateResult == null)
            {
              nullable = new bool?();
            }
            else
            {
              List<WorkItemLinkUpdateResult> linkUpdates = updateResult.LinkUpdates;
              nullable = linkUpdates != null ? new bool?(linkUpdates.Any<WorkItemLinkUpdateResult>((Func<WorkItemLinkUpdateResult, bool>) (x => x.UpdateTypeExecuted.HasValue))) : new bool?();
            }
            if (nullable.GetValueOrDefault())
              goto label_9;
          }
          if (!this.HasResourceLinkUpdates && !this.HasTagUpdates)
            return !this.HasTeamProjectChanged;
        }
      }
label_9:
      return false;
    }

    public bool NeedsNewRevision(bool ignoreChangedBy)
    {
      if (!this.Success)
        return false;
      if (this.Update.IsNew)
        return true;
      IEnumerable<KeyValuePair<int, object>> source = !this.HasFieldUpdates ? Enumerable.Empty<KeyValuePair<int, object>>() : (ignoreChangedBy ? this.FieldUpdates.Where<KeyValuePair<int, object>>((Func<KeyValuePair<int, object>, bool>) (fu => fu.Key != 9)) : this.FieldUpdates);
      if (this.m_isNoHistoryEnabledFieldsSupported)
      {
        if (source.Any<KeyValuePair<int, object>>((Func<KeyValuePair<int, object>, bool>) (fu => !(fu.Value is ServerDefaultFieldValue) && !this.m_historyDisabledFieldIds.Contains(fu.Key))))
          return true;
      }
      else if (source.Any<KeyValuePair<int, object>>((Func<KeyValuePair<int, object>, bool>) (fu => !(fu.Value is ServerDefaultFieldValue))))
        return true;
      return this.HasResourceLinkUpdatesRequiringNewRevision;
    }

    internal void SetResultRevision()
    {
      if (this.NeedsNewRevision(false))
      {
        this.UpdateResult.Rev = this.FieldData.Revision + 1;
        this.HasNewRevision = new bool?(true);
      }
      else
      {
        this.UpdateResult.Rev = this.FieldData.Revision;
        this.HasNewRevision = new bool?(false);
      }
    }

    internal bool NeedsNewDataRow(int fieldId) => !this.IsNoHistoryEnabledFieldsSupported || this.Update.IsNew || this.FieldData != null && !this.FieldData.HasField(fieldId) || !this.m_historyDisabledFieldIds.Contains(fieldId);

    internal void ClearFieldUpdates()
    {
      this.EnsureFieldUpdates();
      this.m_fieldUpdates.Clear();
      this.m_fieldUpdatesMap.Clear();
      this.HasMarkdownCommentUpdate = false;
    }

    internal void AddFieldUpdate(int fieldId, object value, bool isMarkdownFieldUpdated = false)
    {
      this.EnsureFieldUpdates();
      this.m_fieldUpdates.Add(new KeyValuePair<int, object>(fieldId, value));
      this.m_fieldUpdatesMap[fieldId] = value;
      if (!isMarkdownFieldUpdated || this.HasMarkdownCommentUpdate)
        return;
      this.HasMarkdownCommentUpdate = isMarkdownFieldUpdated;
    }

    internal void AddFieldUpdates(
      IEnumerable<KeyValuePair<int, object>> updates,
      bool isMarkdownFieldUpdated = false)
    {
      this.EnsureFieldUpdates();
      this.m_fieldUpdates.AddRange(updates);
      foreach (KeyValuePair<int, object> update in updates)
        this.m_fieldUpdatesMap[update.Key] = update.Value;
      if (!isMarkdownFieldUpdated || this.HasMarkdownCommentUpdate)
        return;
      this.HasMarkdownCommentUpdate = isMarkdownFieldUpdated;
    }

    private void EnsureFieldUpdates()
    {
      if (this.m_fieldUpdates != null)
        return;
      this.m_fieldUpdates = new List<KeyValuePair<int, object>>();
      this.m_fieldUpdatesMap = new Dictionary<int, object>();
    }

    public bool HasFieldUpdate(int fieldId) => this.m_fieldUpdatesMap != null && this.m_fieldUpdatesMap.ContainsKey(fieldId);

    public void RemoveFieldUpdate(int fieldId)
    {
      if (!this.m_fieldUpdatesMap.ContainsKey(fieldId))
        return;
      this.m_fieldUpdatesMap.Remove(fieldId);
      for (int index = this.m_fieldUpdates.Count - 1; index >= 0; --index)
      {
        if (this.m_fieldUpdates[index].Key == fieldId)
          this.m_fieldUpdates.RemoveAt(index);
      }
    }

    internal IEnumerable<KeyValuePair<int, object>> GetFieldUpdatesToBeReported()
    {
      if (this.m_fieldUpdatesMap == null || this.m_fieldUpdatesMap.Count <= 0)
        return (IEnumerable<KeyValuePair<int, object>>) this.DBFieldUpdates;
      IFieldTypeDictionary fieldDict = this.m_witRequestContext.FieldDictionary;
      return this.DBFieldUpdates.Where<KeyValuePair<int, object>>((Func<KeyValuePair<int, object>, bool>) (fieldUpdate =>
      {
        int key = fieldUpdate.Key;
        FieldEntry f = fieldDict.GetFieldByNameOrId(key.ToString());
        switch (key)
        {
          case -6:
          case -5:
          case -4:
          case -3:
          case 3:
          case 8:
            return false;
          default:
            if (f.IsIdentity)
            {
              IEnumerable<KeyValuePair<string, object>> source = this.Update.Fields.Where<KeyValuePair<string, object>>((Func<KeyValuePair<string, object>, bool>) (pair => pair.Key == f.ReferenceName));
              if (source.Any<KeyValuePair<string, object>>())
              {
                object objA = source.First<KeyValuePair<string, object>>().Value;
                if (objA is WorkItemIdentity)
                  objA = (object) objA.ToString();
                if (!object.Equals(objA, fieldUpdate.Value))
                  return true;
              }
            }
            object objA1;
            return !this.m_fieldUpdatesMap.TryGetValue(key, out objA1) || !object.Equals(objA1, fieldUpdate.Value);
        }
      }));
    }
  }
}
