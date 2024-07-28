// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItemComponent2
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems
{
  internal class WorkItemComponent2 : WorkItemComponent
  {
    protected override bool DatasetBinderBindsTitle => false;

    protected override bool UpdateBindsTags => false;

    protected virtual SqlParameter BindWorkItemIdRevPairsForGetWorkItemFieldValues(
      string paramName,
      IEnumerable<WorkItemIdRevisionPair> workItemIdRevPairs)
    {
      return this.BindWorkItemIdRevPairsAsKeyValuePairInt32Int32(paramName, workItemIdRevPairs);
    }

    protected virtual void ReadTipValueForWorkItemRevision(
      IDataReader reader,
      Dictionary<int, IEnumerable<WorkItemFieldValues>> fieldValuesRecordsMap)
    {
      Dictionary<int, int> dictionary = new Dictionary<int, int>();
      while (reader.Read())
        dictionary.Add(reader.GetInt32(0), reader.GetInt32(1));
      foreach (KeyValuePair<int, IEnumerable<WorkItemFieldValues>> fieldValuesRecords in fieldValuesRecordsMap)
      {
        int num;
        if (dictionary.TryGetValue(fieldValuesRecords.Key, out num))
        {
          foreach (WorkItemFieldValues workItemFieldValues in fieldValuesRecords.Value)
            workItemFieldValues.LatestAreaId = num;
        }
      }
    }

    protected virtual void BindTableVarThresholdValue(int threshold)
    {
    }

    public override IEnumerable<WorkItemFieldValues> GetWorkItemFieldValues(
      IEnumerable<WorkItemIdRevisionPair> workItemIdRevPairs,
      IEnumerable<int> wideFields,
      IEnumerable<int> longFields,
      IEnumerable<int> textFields,
      bool byRevision,
      DateTime? asOf,
      int maxLongTextLength,
      IdentityDisplayType identityDisplayType,
      bool disableProjectionLevelThree,
      out int wideTableProjectionLevel,
      int useTableVarThreshold = 4)
    {
      workItemIdRevPairs = (IEnumerable<WorkItemIdRevisionPair>) workItemIdRevPairs.Distinct<WorkItemIdRevisionPair>().ToArray<WorkItemIdRevisionPair>();
      HashSet<int> wideTableFields = new HashSet<int>(wideFields);
      wideTableFields.Add(-3);
      IFieldTypeDictionary fieldDictionary = this.RequestContext.WitContext().FieldDictionary;
      WorkItemComponent.WorkItemFieldValuesBinder<WorkItemFieldValues> valuesBinder = this.GetWorkItemFieldValuesBinder((IEnumerable<int>) wideTableFields, identityDisplayType, disableProjectionLevelThree);
      this.PrepareStoredProcedure("prc_GetWorkItemFieldValues");
      this.BindWorkItemIdRevPairsForGetWorkItemFieldValues("@workItemIdRevPairs", workItemIdRevPairs);
      wideTableProjectionLevel = valuesBinder.ProjectionLevel;
      if (wideTableProjectionLevel >= 3 & disableProjectionLevelThree)
        wideTableProjectionLevel = 2;
      this.BindInt("@wideTableProjectionLevel", wideTableProjectionLevel);
      this.BindInt32Table("@longTableFields", longFields);
      this.BindInt32Table("@textTableFields", textFields);
      this.BindBoolean("@byRevision", byRevision);
      this.BindNullableDateTime("@asOf", asOf);
      this.BindMaxLongTextLength(maxLongTextLength);
      this.BindTableVarThresholdValue(useTableVarThreshold);
      return this.ExecuteUnknown<IEnumerable<WorkItemFieldValues>>((System.Func<IDataReader, IEnumerable<WorkItemFieldValues>>) (reader =>
      {
        WorkItemFieldValues[] array = valuesBinder.BindAll(reader).ToArray<WorkItemFieldValues>();
        Dictionary<int, IEnumerable<WorkItemFieldValues>> dictionary = ((IEnumerable<WorkItemFieldValues>) array).GroupBy<WorkItemFieldValues, int>((System.Func<WorkItemFieldValues, int>) (wfv => wfv.Id)).ToDictionary<IGrouping<int, WorkItemFieldValues>, int, IEnumerable<WorkItemFieldValues>>((System.Func<IGrouping<int, WorkItemFieldValues>, int>) (wfvg => wfvg.Key), (System.Func<IGrouping<int, WorkItemFieldValues>, IEnumerable<WorkItemFieldValues>>) (wfvg => wfvg.AsEnumerable<WorkItemFieldValues>()));
        List<WorkItemCustomFieldValue> source1 = new List<WorkItemCustomFieldValue>();
        if (longFields.Any<int>())
        {
          reader.NextResult();
          source1.AddRange(this.GetWorkItemCustomFieldValueBinder(fieldDictionary, identityDisplayType).BindAll(reader));
        }
        if (textFields.Any<int>())
        {
          reader.NextResult();
          source1.AddRange(new WorkItemComponent.WorkItemTextFieldValueBinder().BindAll(reader));
        }
        if ((byRevision || asOf.HasValue) && reader.NextResult())
          this.ReadTipValueForWorkItemRevision(reader, dictionary);
        foreach (IGrouping<int, WorkItemCustomFieldValue> grouping in source1.GroupBy<WorkItemCustomFieldValue, int>((System.Func<WorkItemCustomFieldValue, int>) (fv => fv.Id)))
        {
          IEnumerable<WorkItemFieldValues> source2;
          if (!dictionary.TryGetValue(grouping.Key, out source2))
            throw new WorkItemTrackingSqlDataNotFoundException(string.Format("No field value records are found for the work item id : {0}", (object) grouping.Key));
          foreach (WorkItemCustomFieldValue customFieldValue2 in (IEnumerable<WorkItemCustomFieldValue>) grouping)
          {
            WorkItemCustomFieldValue value = customFieldValue2;
            foreach (WorkItemFieldValues workItemFieldValues in source2.Where<WorkItemFieldValues>((System.Func<WorkItemFieldValues, bool>) (fvr =>
            {
              if (value.AuthorizedDate == fvr.AuthorizedDate)
                return true;
              return ((IRevisedWorkItemEntity) value).SpansMultipleRevisions && value.AuthorizedDate <= fvr.AuthorizedDate && value.RevisedDate >= fvr.RevisedDate;
            })))
            {
              if (value is WorkItemCustomFieldValue.WorkItemLargeTextCustomFieldValue customFieldValue3)
              {
                workItemFieldValues.Fields[value.FieldId] = (object) new WorkItemLargeTextValue()
                {
                  Text = customFieldValue3.StringValue,
                  IsHtml = customFieldValue3.IsHtml
                };
              }
              else
              {
                workItemFieldValues.Fields[value.FieldId] = value.Value;
                if (value.IsIdentityField && value.GuidValue.HasValue)
                {
                  Guid? guidValue = value.GuidValue;
                  Guid empty = Guid.Empty;
                  if ((guidValue.HasValue ? (guidValue.HasValue ? (guidValue.GetValueOrDefault() != empty ? 1 : 0) : 0) : 1) != 0 && !workItemFieldValues.IdentityFields.ContainsKey(value.FieldId))
                  {
                    Dictionary<int, Guid> identityFields = workItemFieldValues.IdentityFields;
                    int fieldId = value.FieldId;
                    guidValue = value.GuidValue;
                    Guid guid = guidValue.Value;
                    identityFields[fieldId] = guid;
                  }
                }
              }
            }
          }
        }
        return (IEnumerable<WorkItemFieldValues>) array;
      }));
    }
  }
}
