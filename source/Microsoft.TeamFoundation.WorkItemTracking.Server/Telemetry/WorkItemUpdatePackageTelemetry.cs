// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Telemetry.WorkItemUpdatePackageTelemetry
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.UpdateState;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Telemetry
{
  internal class WorkItemUpdatePackageTelemetry
  {
    private const string Feature = "WorkItemUpdatePackage";

    public static void Publish(
      WorkItemTrackingRequestContext witRequestContext,
      ISet<int> workItemIds,
      WorkItemUpdateDataset updateDataset,
      IEnumerable<WorkItemUpdateState> updateStates,
      IEnumerable<WorkItemChangedEventExtended> workItemChangedEvents)
    {
      IVssRequestContext requestContext = witRequestContext.RequestContext;
      IFieldTypeDictionary fieldDictionary = witRequestContext.FieldDictionary;
      FieldEntry field1;
      Func<int, string> transformFieldById = (Func<int, string>) (fieldId => !fieldDictionary.TryGetField(fieldId, out field1) ? string.Format("???:{0}", (object) fieldId) : string.Format("{0}:{1}", (object) field1.Name, (object) fieldId));
      FieldEntry field2;
      Func<string, string> transformFieldByName = (Func<string, string>) (fieldNameOrId => !fieldDictionary.TryGetFieldByNameOrId(fieldNameOrId, out field2) ? fieldNameOrId + ":???" : string.Format("{0}:{1}", (object) field2.Name, (object) field2.FieldId));
      FieldEntry field3;
      Func<string, int> getFieldIdByName = (Func<string, int>) (fieldName => !fieldDictionary.TryGetField(fieldName, out field3) ? 0 : field3.FieldId);
      CustomerIntelligenceData properties = new CustomerIntelligenceData();
      properties.Add("ActivityId", (object) requestContext.ActivityId);
      properties.Add("Updates", (object) workItemIds.OrderBy<int, int>((Func<int, int>) (id => id)).Select<int, Dictionary<string, object>>((Func<int, Dictionary<string, object>>) (id =>
      {
        IEnumerable<WorkItemUpdateState> updateStatesForId = updateStates.Where<WorkItemUpdateState>((Func<WorkItemUpdateState, bool>) (us => us.Id == id));
        Dictionary<string, object> dictionary1 = new Dictionary<string, object>();
        dictionary1["Input"] = (object) updateStatesForId.Select<WorkItemUpdateState, WorkItemUpdate>((Func<WorkItemUpdateState, WorkItemUpdate>) (us => us.Update)).Select(update => new
        {
          Rev = update.Rev,
          Fields = WorkItemUpdatePackageTelemetry.ToSortedDictionary<string, object, KeyValuePair<string, object>>(update.Fields, (Func<KeyValuePair<string, object>, string>) (pair => transformFieldByName(pair.Key)), (Func<KeyValuePair<string, object>, object>) (pair => pair.Value)),
          LinkUpdates = update.LinkUpdates,
          ResourceLinkUpdates = update.ResourceLinkUpdates
        }).FirstOrDefault();
        dictionary1["UpdateState"] = (object) updateStatesForId.Select(updateState =>
        {
          // ISSUE: variable of a compiler-generated type
          WorkItemUpdatePackageTelemetry.\u003C\u003Ec__DisplayClass1_0 cDisplayClass10 = this;
          WorkItemUpdateState updateState2 = updateState;
          // ISSUE: reference to a compiler-generated field
          return new
          {
            Success = updateState2.Success,
            DBFieldUpdates = WorkItemUpdatePackageTelemetry.ToSortedDictionary<string, object, KeyValuePair<int, object>>((IEnumerable<KeyValuePair<int, object>>) updateState2.DBFieldUpdates, (Func<KeyValuePair<int, object>, string>) (pair => transformFieldById(pair.Key)), (Func<KeyValuePair<int, object>, object>) (pair => pair.Value)),
            FieldUpdates = updateState2.FieldUpdates.Select(fu => new
            {
              Field = cDisplayClass10.transformFieldById(fu.Key),
              Old = updateState2.FieldData.LatestData.GetValueOrDefault<object>(fu.Key),
              New = fu.Value
            }),
            CurrentTags = updateState2.CurrentTags,
            AddedTags = updateState2.AddedTags,
            RemovedTags = updateState2.RemovedTags,
            HasNewRevision = updateState2.HasNewRevision,
            UpdateResult = new
            {
              Id = updateState2.UpdateResult.Id,
              Rev = updateState2.UpdateResult.Rev,
              Exception = updateState2.UpdateResult.Exception,
              Fields = WorkItemUpdatePackageTelemetry.ToSortedDictionary<string, object, KeyValuePair<string, object>>((IEnumerable<KeyValuePair<string, object>>) updateState2.UpdateResult.Fields, (Func<KeyValuePair<string, object>, string>) (pair => transformFieldByName(pair.Key)), (Func<KeyValuePair<string, object>, object>) (pair => pair.Value))
            }
          };
        }).FirstOrDefault();
        if (updateDataset != null)
          dictionary1["UpdateDataset"] = (object) new
          {
            CoreFieldUpdates = updateDataset.CoreFieldUpdates.Where<WorkItemCoreFieldUpdatesRecord>((Func<WorkItemCoreFieldUpdatesRecord, bool>) (cfu => cfu.Id == id)),
            CustomFieldUpdates = updateDataset.CustomFieldUpdates.Where<WorkItemCustomFieldUpdateRecord>((Func<WorkItemCustomFieldUpdateRecord, bool>) (cfu => cfu.WorkItemId == id)).Select(cfu => new
            {
              Field = transformFieldById(cfu.Field.FieldId),
              Value = cfu.Value,
              NeedsNewRevision = cfu.NeedsNewRevision
            }).FirstOrDefault(),
            TextFieldUpdates = updateDataset.TextFieldUpdates.Where<WorkItemTextFieldUpdateRecord>((Func<WorkItemTextFieldUpdateRecord, bool>) (tfu => tfu.WorkItemId == id)),
            WorkItemLinkUpdates = updateDataset.WorkItemLinkUpdates.Where<WorkItemLinkUpdateRecord>((Func<WorkItemLinkUpdateRecord, bool>) (wlu => wlu.SourceId == id || wlu.TargetId == id)),
            ResourceLinkUpdates = updateDataset.ResourceLinkUpdates.Where<WorkItemResourceLinkUpdateRecord>((Func<WorkItemResourceLinkUpdateRecord, bool>) (rlu => rlu.SourceId == id))
          };
        IEnumerable<WorkItemChangedEventExtended> source = workItemChangedEvents;
        WorkItemChangedEvent itemChangedEvent = source != null ? source.Where<WorkItemChangedEventExtended>((Func<WorkItemChangedEventExtended, bool>) (e => e.LegacyChangedEvent.WorkItemId == id.ToString())).Select<WorkItemChangedEventExtended, WorkItemChangedEvent>((Func<WorkItemChangedEventExtended, WorkItemChangedEvent>) (e => e.LegacyChangedEvent)).FirstOrDefault<WorkItemChangedEvent>() : (WorkItemChangedEvent) null;
        if (itemChangedEvent != null)
        {
          Dictionary<string, object> dictionary2 = dictionary1;
          ChangedFieldsType changedFields1 = itemChangedEvent.ChangedFields;
          IEnumerable<\u003C\u003Ef__AnonymousType48<string, bool, bool>> datas1;
          if (changedFields1 == null)
          {
            datas1 = null;
          }
          else
          {
            BooleanField[] booleanFields = changedFields1.BooleanFields;
            datas1 = booleanFields != null ? ((IEnumerable<BooleanField>) booleanFields).Select(f => new
            {
              Field = transformFieldByName(f.ReferenceName),
              Old = f.OldValue,
              New = f.NewValue
            }) : null;
          }
          ChangedFieldsType changedFields2 = itemChangedEvent.ChangedFields;
          IEnumerable<\u003C\u003Ef__AnonymousType48<string, int, int>> datas2;
          if (changedFields2 == null)
          {
            datas2 = null;
          }
          else
          {
            IntegerField[] integerFields = changedFields2.IntegerFields;
            datas2 = integerFields != null ? ((IEnumerable<IntegerField>) integerFields).Select(f => new
            {
              Field = transformFieldByName(f.ReferenceName),
              Old = f.OldValue,
              New = f.NewValue
            }) : null;
          }
          ChangedFieldsType changedFields3 = itemChangedEvent.ChangedFields;
          IEnumerable<\u003C\u003Ef__AnonymousType48<string, string, string>> datas3;
          if (changedFields3 == null)
          {
            datas3 = null;
          }
          else
          {
            StringField[] stringFields = changedFields3.StringFields;
            datas3 = stringFields != null ? ((IEnumerable<StringField>) stringFields).Select(f => new
            {
              Field = transformFieldByName(f.ReferenceName),
              Old = f.OldValue,
              New = f.NewValue
            }) : null;
          }
          TextField[] textFields = itemChangedEvent.TextFields;
          IEnumerable<\u003C\u003Ef__AnonymousType53<string, object, string, string>> datas4 = textFields != null ? ((IEnumerable<TextField>) textFields).Select(f => new
          {
            Field = transformFieldByName(f.ReferenceName),
            Old = updateStatesForId.Select<WorkItemUpdateState, object>((Func<WorkItemUpdateState, object>) (us => us.FieldData.LatestData.GetValueOrDefault<object>(getFieldIdByName(f.ReferenceName)))).FirstOrDefault<object>(),
            New = f.Value,
            FieldType = f.FieldType.ToString()
          }) : null;
          var data = new
          {
            BooleanFields = datas1,
            IntegerFields = datas2,
            StringFields = datas3,
            TextFields = datas4
          };
          dictionary2["ChangedEvents"] = (object) data;
        }
        return dictionary1;
      })));
      requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, "WorkItemService", "WorkItemUpdatePackage", properties);
    }

    private static SortedDictionary<TKey, TValue> ToSortedDictionary<TKey, TValue, TObj>(
      IEnumerable<TObj> enumerable,
      Func<TObj, TKey> keySelector,
      Func<TObj, TValue> valueSelector)
    {
      SortedDictionary<TKey, TValue> sortedDictionary = new SortedDictionary<TKey, TValue>();
      foreach (TObj obj1 in enumerable)
      {
        TKey key = keySelector(obj1);
        TValue obj2 = valueSelector(obj1);
        sortedDictionary[key] = obj2;
      }
      return sortedDictionary;
    }
  }
}
