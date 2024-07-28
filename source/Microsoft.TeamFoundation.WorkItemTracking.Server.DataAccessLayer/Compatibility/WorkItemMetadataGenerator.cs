// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Compatibility.WorkItemMetadataGenerator
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using Microsoft.Azure.Boards.ProcessTemplates;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Compatibility;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Rules;
using Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessMetadata;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Compatibility
{
  internal static class WorkItemMetadataGenerator
  {
    private const int c_baseId = 1932735283;

    internal static void GenerateWorkItemTypeActions(
      IVssRequestContext requestContext,
      MetadataCompatibilityContext compatContext,
      PayloadTable payloadTable,
      WorkItemMetadataCompatibilityService.MetadataTableDescriptor tableDescriptor,
      out int newMaskBits)
    {
      newMaskBits = 0;
      Dictionary<int, List<PayloadTable.PayloadRow>> dictionary1 = new Dictionary<int, List<PayloadTable.PayloadRow>>();
      foreach (PayloadTable.PayloadRow row in payloadTable.Rows)
      {
        if (!Convert.ToBoolean(row[5]))
        {
          int key = (int) row[2];
          List<PayloadTable.PayloadRow> payloadRowList;
          if (!dictionary1.TryGetValue(key, out payloadRowList))
          {
            payloadRowList = new List<PayloadTable.PayloadRow>();
            dictionary1[key] = payloadRowList;
          }
          payloadRowList.Add(row);
        }
      }
      int nextId = 1;
      if (payloadTable.Rows.Any<PayloadTable.PayloadRow>())
        nextId = payloadTable.Rows.Max<PayloadTable.PayloadRow, int>((Func<PayloadTable.PayloadRow, int>) (r => (int) r["ActionID"])) + 1;
      foreach (MetadataProjectCompatibilityDescriptor projectDescriptor in (IEnumerable<MetadataProjectCompatibilityDescriptor>) compatContext.ProjectDescriptors)
      {
        Dictionary<string, MetadataWorkItemTypeCompatibilityDescriptor> dictionary2 = projectDescriptor.TypeDescriptors.ToDictionary<MetadataWorkItemTypeCompatibilityDescriptor, string>((Func<MetadataWorkItemTypeCompatibilityDescriptor, string>) (td => td.Type.ReferenceName));
        ProcessWorkDefinition oobProcessDefinition;
        if (projectDescriptor.TryGetOobProcessDefinition(requestContext, out oobProcessDefinition))
        {
          foreach (ProcessWorkItemTypeDefinition itemTypeDefinition in (IEnumerable<ProcessWorkItemTypeDefinition>) oobProcessDefinition.WorkItemTypeDefinitions)
          {
            MetadataWorkItemTypeCompatibilityDescriptor compatibilityDescriptor = dictionary2[itemTypeDefinition.ReferenceName];
            List<PayloadTable.PayloadRow> witActionsInDb;
            if (!dictionary1.TryGetValue(compatibilityDescriptor.Type.Id.Value, out witActionsInDb))
              witActionsInDb = new List<PayloadTable.PayloadRow>();
            WorkItemMetadataGenerator.CreateMissingActionRecords(payloadTable, (IEnumerable<PayloadTable.PayloadRow>) witActionsInDb, itemTypeDefinition.Actions, compatContext.ConstantMap, compatibilityDescriptor.Type.Id.Value, ref nextId);
          }
        }
      }
      MetadataBucket bucket = tableDescriptor.Bucket;
      int localId = bucket.ParseLocalId(tableDescriptor.UserCacheStamp);
      MetadataBucket.EnsureBucketCapacity(Math.Max(payloadTable.RowCount, localId), ref bucket, ref newMaskBits);
      int num = 1;
      foreach (PayloadTable.PayloadRow row in payloadTable.Rows)
      {
        row[6] = (object) bucket.GenerateCacheStamp(num);
        ++num;
      }
      if (num > localId || !compatContext.IsBelowDeletedRowGenerationLimit(requestContext, num, localId, nameof (GenerateWorkItemTypeActions)))
        return;
      for (; num <= localId; ++num)
      {
        PayloadTable.PayloadRow payloadRow = payloadTable.AddNewPayloadRow();
        payloadRow[0] = (object) num;
        payloadRow[5] = (object) true;
        payloadRow[6] = (object) bucket.GenerateCacheStamp(num);
      }
    }

    internal static void CreateMissingActionRecords(
      PayloadTable payloadTable,
      IEnumerable<PayloadTable.PayloadRow> witActionsInDb,
      IEnumerable<ProcessWorkItemTypeActionDefinition> actionsInProcess,
      IDictionary<string, int> constantMap,
      int witId,
      ref int nextId)
    {
      foreach (ProcessWorkItemTypeActionDefinition actionDefinition in actionsInProcess)
      {
        ProcessWorkItemTypeActionDefinition action = actionDefinition;
        int fromStateConstId = constantMap[action.FromState];
        int toStateConstId = constantMap[action.ToState];
        if (!witActionsInDb.Any<PayloadTable.PayloadRow>((Func<PayloadTable.PayloadRow, bool>) (a => (int) a["FromStateConstID"] == fromStateConstId && (int) a["ToStateConstID"] == toStateConstId && (string) a["Name"] == action.Name)))
          WorkItemMetadataGenerator.AddNewWorkItemTypeActionsRow(payloadTable, nextId++, action.Name, witId, fromStateConstId, toStateConstId);
      }
    }

    private static void AddNewWorkItemTypeActionsRow(
      PayloadTable workItemTypeCategoryMembersTable,
      int actionId,
      string actionName,
      int workItemTypeId,
      int fromStateConstId,
      int toStateConstId)
    {
      PayloadTable.PayloadRow payloadRow = workItemTypeCategoryMembersTable.AddNewPayloadRow();
      payloadRow[0] = (object) actionId;
      payloadRow[1] = (object) actionName;
      payloadRow[2] = (object) workItemTypeId;
      payloadRow[3] = (object) fromStateConstId;
      payloadRow[4] = (object) toStateConstId;
      payloadRow[5] = (object) false;
    }

    internal static void GenerateWorkItemTypeCategories(
      IVssRequestContext requestContext,
      MetadataCompatibilityContext compatContext,
      PayloadTable payloadTable,
      WorkItemMetadataCompatibilityService.MetadataTableDescriptor tableDescriptor,
      out int newMaskBits)
    {
      newMaskBits = 0;
      Dictionary<int, MetadataProjectCompatibilityDescriptor> dictionary1 = compatContext.ProjectDescriptors.ToDictionary<MetadataProjectCompatibilityDescriptor, int>((Func<MetadataProjectCompatibilityDescriptor, int>) (pd => pd.ProjectNode.Id));
      Dictionary<int, HashSet<string>> dictionary2 = new Dictionary<int, HashSet<string>>();
      foreach (PayloadTable.PayloadRow row in payloadTable.Rows)
      {
        if (!Convert.ToBoolean(row[5]))
        {
          int key1 = (int) row[1];
          int num = (int) row[0];
          HashSet<string> stringSet;
          if (!dictionary2.TryGetValue(key1, out stringSet))
          {
            stringSet = new HashSet<string>((IEqualityComparer<string>) TFStringComparer.WorkItemCategoryReferenceName);
            dictionary2[key1] = stringSet;
          }
          string key2 = (string) row[3];
          MetadataProjectCompatibilityDescriptor compatibilityDescriptor;
          if (dictionary1.TryGetValue(key1, out compatibilityDescriptor))
            compatibilityDescriptor.CategoryIdsByRefName[key2] = num;
          stringSet.Add(key2);
          string str = (string) row[2];
          stringSet.Add(str);
        }
      }
      int nextId = 1;
      if (payloadTable.Rows.Any<PayloadTable.PayloadRow>())
        nextId = payloadTable.Rows.Max<PayloadTable.PayloadRow, int>((Func<PayloadTable.PayloadRow, int>) (r => (int) r["WorkItemTypeCategoryID"])) + 1;
      IWorkItemTypeCategoryService service = requestContext.GetService<IWorkItemTypeCategoryService>();
      foreach (MetadataProjectCompatibilityDescriptor projectDescriptor in (IEnumerable<MetadataProjectCompatibilityDescriptor>) compatContext.ProjectDescriptors)
      {
        int id = projectDescriptor.ProjectNode.Id;
        HashSet<string> categoryRefNamesInDb;
        if (!dictionary2.TryGetValue(id, out categoryRefNamesInDb))
          categoryRefNamesInDb = new HashSet<string>((IEqualityComparer<string>) TFStringComparer.WorkItemCategoryReferenceName);
        IEnumerable<WorkItemTypeCategory> categoriesInProcess = projectDescriptor.WorkItemTypeCategories == null ? service.GetWorkItemTypeCategories(requestContext.Elevate(), projectDescriptor.ProjectNode.ProjectId) : projectDescriptor.WorkItemTypeCategories;
        WorkItemMetadataGenerator.CreateMissingCategoryRecords(requestContext, payloadTable, projectDescriptor, categoryRefNamesInDb, categoriesInProcess, id, ref nextId);
      }
      MetadataBucket bucket = tableDescriptor.Bucket;
      int localId = bucket.ParseLocalId(tableDescriptor.UserCacheStamp);
      MetadataBucket.EnsureBucketCapacity(Math.Max(payloadTable.RowCount, localId), ref bucket, ref newMaskBits);
      int num1 = 1;
      foreach (PayloadTable.PayloadRow row in payloadTable.Rows)
      {
        row[6] = (object) bucket.GenerateCacheStamp(num1);
        ++num1;
      }
      if (num1 > localId || !compatContext.IsBelowDeletedRowGenerationLimit(requestContext, num1, localId, nameof (GenerateWorkItemTypeCategories)))
        return;
      for (; num1 <= localId; ++num1)
      {
        PayloadTable.PayloadRow payloadRow = payloadTable.AddNewPayloadRow();
        payloadRow[0] = (object) num1;
        payloadRow[5] = (object) true;
        payloadRow[6] = (object) bucket.GenerateCacheStamp(num1);
      }
    }

    internal static void CreateMissingCategoryRecords(
      IVssRequestContext requestContext,
      PayloadTable payloadTable,
      MetadataProjectCompatibilityDescriptor projectCompatDescriptor,
      HashSet<string> categoryRefNamesInDb,
      IEnumerable<WorkItemTypeCategory> categoriesInProcess,
      int projectId,
      ref int nextId)
    {
      foreach (WorkItemTypeCategory itemTypeCategory in categoriesInProcess)
      {
        WorkItemTypeCategory categoryToAdd = itemTypeCategory;
        if (!categoryRefNamesInDb.Contains(categoryToAdd.ReferenceName) && !categoryRefNamesInDb.Contains(categoryToAdd.Name))
        {
          categoryRefNamesInDb.Add(categoryToAdd.ReferenceName);
          categoryRefNamesInDb.Add(categoryToAdd.Name);
          if (projectCompatDescriptor.TypeDescriptors.Where<MetadataWorkItemTypeCompatibilityDescriptor>((Func<MetadataWorkItemTypeCompatibilityDescriptor, bool>) (t => t.Type != null && TFStringComparer.WorkItemTypeName.Equals(t.Type.Name, categoryToAdd.DefaultWorkItemTypeName))).FirstOrDefault<MetadataWorkItemTypeCompatibilityDescriptor>() != null)
          {
            int defaultTypeId = projectCompatDescriptor.TypeDescriptors.Where<MetadataWorkItemTypeCompatibilityDescriptor>((Func<MetadataWorkItemTypeCompatibilityDescriptor, bool>) (t => t.Type != null && TFStringComparer.WorkItemTypeName.Equals(t.Type.Name, categoryToAdd.DefaultWorkItemTypeName))).First<MetadataWorkItemTypeCompatibilityDescriptor>().Type.Id.Value;
            projectCompatDescriptor.CategoryIdsByRefName[categoryToAdd.ReferenceName] = nextId;
            WorkItemMetadataGenerator.AddNewWorkItemTypeCategoriesRow(payloadTable, nextId++, projectId, categoryToAdd.Name, categoryToAdd.ReferenceName, defaultTypeId);
          }
          else
            MetadataCompatibilityContext.ReportError(requestContext, nameof (CreateMissingCategoryRecords), string.Format("Missing work item type: {0}, total work items in project: {1}", (object) categoryToAdd.DefaultWorkItemTypeName, (object) projectCompatDescriptor.TypeDescriptors.Count));
        }
      }
    }

    private static void AddNewWorkItemTypeCategoriesRow(
      PayloadTable workItemTypeCategoriesTable,
      int categoryId,
      int projectId,
      string name,
      string refName,
      int defaultTypeId)
    {
      PayloadTable.PayloadRow payloadRow = workItemTypeCategoriesTable.AddNewPayloadRow();
      payloadRow[0] = (object) categoryId;
      payloadRow[1] = (object) projectId;
      payloadRow[2] = (object) name;
      payloadRow[3] = (object) refName;
      payloadRow[4] = (object) defaultTypeId;
      payloadRow[5] = (object) false;
    }

    internal static void GenerateConstantSets(
      IVssRequestContext requestContext,
      MetadataCompatibilityContext compatContext,
      PayloadTable payloadTable,
      WorkItemMetadataCompatibilityService.MetadataTableDescriptor tableDescriptor,
      out int newMaskBits)
    {
      newMaskBits = 0;
      int num1;
      if (WorkItemTrackingFeatureFlags.IsProcessCustomizationEnabled(requestContext) && tableDescriptor.LegacyCachestampDescriptor != null)
      {
        bool? identityOnlyChanges = tableDescriptor.LegacyCachestampDescriptor.IsFollowedByIdentityOnlyChanges;
        bool flag = true;
        if (identityOnlyChanges.GetValueOrDefault() == flag & identityOnlyChanges.HasValue)
        {
          num1 = tableDescriptor.UserCacheStamp > 0L ? 1 : 0;
          goto label_4;
        }
      }
      num1 = 0;
label_4:
      bool flag1 = num1 != 0;
      if (!flag1)
      {
        Dictionary<int, HashSet<int>> existingSetMappings = WorkItemMetadataGenerator.BuildSetMappings(requestContext, payloadTable);
        HashSet<Guid> guidSet = new HashSet<Guid>();
        foreach (MetadataProjectCompatibilityDescriptor projectDescriptor in (IEnumerable<MetadataProjectCompatibilityDescriptor>) compatContext.ProjectDescriptors)
        {
          foreach (MetadataWorkItemTypeCompatibilityDescriptor typeDescriptor in (IEnumerable<MetadataWorkItemTypeCompatibilityDescriptor>) projectDescriptor.TypeDescriptors)
          {
            string typeListName = CompatibilityConstants.WorkItemTypeAllowedValueListHead(projectDescriptor.ProjectNode.Id);
            WorkItemMetadataGenerator.AddMissingWorkItemTypeSetEntries(requestContext, compatContext, typeListName, typeDescriptor.Type.Name, existingSetMappings, payloadTable);
          }
          ProcessDescriptor processDescriptor;
          if (projectDescriptor.TryGetProcessDescriptor(requestContext, out processDescriptor))
          {
            if (!processDescriptor.IsCustom && !guidSet.Contains(processDescriptor.TypeId))
            {
              guidSet.Add(processDescriptor.TypeId);
              foreach (MetadataWorkItemTypeCompatibilityDescriptor typeDescriptor in (IEnumerable<MetadataWorkItemTypeCompatibilityDescriptor>) projectDescriptor.TypeDescriptors)
              {
                if (WorkItemTrackingFeatureFlags.IsFullOrPartialRuleGenerationEnabled(requestContext))
                  WorkItemMetadataGenerator.AddOutOfBoxAllowedAndSuggestedValueSets(requestContext, processDescriptor, typeDescriptor, compatContext, existingSetMappings, payloadTable);
                if (typeDescriptor.Type.IsDerived || typeDescriptor.Type.IsCustomType)
                {
                  ProcessWorkItemType typelet = requestContext.GetService<IProcessWorkItemTypeService>().GetTypelet<ProcessWorkItemType>(requestContext, processDescriptor.TypeId, typeDescriptor.BaseTypeReferenceName);
                  if (typelet != null && (typelet.IsDerived || typelet.IsCustomType) && typelet.FieldRules != null)
                  {
                    foreach (WorkItemFieldRule fieldRule in typelet.FieldRules)
                    {
                      AllowedValuesRule allowedValuesRule = fieldRule != null ? fieldRule.SubRules.OfType<AllowedValuesRule>().FirstOrDefault<AllowedValuesRule>() : (AllowedValuesRule) null;
                      if (allowedValuesRule != null)
                      {
                        string listName = allowedValuesRule.Id.ToString();
                        WorkItemMetadataGenerator.AddMissingListRuleSetEntries(requestContext, compatContext, existingSetMappings, listName, (IReadOnlyCollection<string>) allowedValuesRule.Values.Select<string, string>((Func<string, string>) (s => s.ToString())).ToList<string>(), payloadTable);
                      }
                      SuggestedValuesRule suggestedValuesRule = fieldRule != null ? fieldRule.SubRules.OfType<SuggestedValuesRule>().FirstOrDefault<SuggestedValuesRule>() : (SuggestedValuesRule) null;
                      if (suggestedValuesRule != null)
                      {
                        string listName = suggestedValuesRule.Id.ToString();
                        WorkItemMetadataGenerator.AddMissingListRuleSetEntries(requestContext, compatContext, existingSetMappings, listName, (IReadOnlyCollection<string>) suggestedValuesRule.Values.Select<string, string>((Func<string, string>) (s => s.ToString())).ToList<string>(), payloadTable);
                      }
                    }
                  }
                }
                foreach (FieldEntry field in (IEnumerable<FieldEntry>) typeDescriptor.Fields)
                {
                  if (field.IsPicklist)
                  {
                    WorkItemPickList list = requestContext.GetService<IWorkItemPickListService>().GetList(requestContext, field.PickListId.Value);
                    WorkItemMetadataGenerator.AddMissingPicklistSetEntries(requestContext, compatContext, existingSetMappings, payloadTable, list);
                  }
                  IReadOnlyCollection<string> values1 = (IReadOnlyCollection<string>) null;
                  if (OOBFieldValues.TryGetAllowedValues(requestContext, field.ReferenceName, out values1))
                  {
                    string listName = FieldsConstants.AllowedValuesListHead(field.ReferenceName);
                    WorkItemMetadataGenerator.AddMissingListRuleSetEntries(requestContext, compatContext, existingSetMappings, listName, values1, payloadTable);
                  }
                  IReadOnlyCollection<string> values2 = (IReadOnlyCollection<string>) null;
                  if (OOBFieldValues.TryGetSuggestedValues(requestContext, field.ReferenceName, out values2))
                  {
                    string listName = FieldsConstants.SuggestedValuesListHead(field.ReferenceName);
                    WorkItemMetadataGenerator.AddMissingListRuleSetEntries(requestContext, compatContext, existingSetMappings, listName, values2, payloadTable);
                  }
                }
                if (typeDescriptor.AreStatesCustomized)
                {
                  string listName = StatesConstants.AllowedValuesListHead(typeDescriptor.BaseTypeReferenceName);
                  List<string> list = typeDescriptor.States.Select<WorkItemStateDefinition, string>((Func<WorkItemStateDefinition, string>) (s => s.Name)).ToList<string>();
                  WorkItemMetadataGenerator.AddMissingListRuleSetEntries(requestContext, compatContext, existingSetMappings, listName, (IReadOnlyCollection<string>) list, payloadTable);
                }
              }
            }
          }
          else
            requestContext.Trace(910601, TraceLevel.Warning, nameof (WorkItemMetadataGenerator), nameof (GenerateConstantSets), string.Format("ProcessDescriptor not found for project '{0}'.", (object) projectDescriptor.ProjectNode.ProjectId));
        }
      }
      MetadataBucket bucket = tableDescriptor.Bucket;
      int localId = bucket.ParseLocalId(tableDescriptor.UserCacheStamp);
      MetadataBucket.EnsureBucketCapacity(Math.Max(payloadTable.RowCount, localId), ref bucket, ref newMaskBits);
      long? cachestamp = (long?) tableDescriptor.LegacyCachestampDescriptor?.Cachestamp;
      bool flag2 = WorkItemTrackingFeatureFlags.IsProcessCustomizationEnabled(requestContext);
      int num2 = 1;
      foreach (PayloadTable.PayloadRow row in payloadTable.Rows)
      {
        if (flag2)
        {
          int int32 = Convert.ToInt32(row[0]);
          long int64 = Convert.ToInt64(row[4]);
          if (flag1)
          {
            row[4] = (object) (ulong) (localId > 0 ? (long) bucket.GenerateCacheStamp(localId) : (long) bucket.GetBaseCachestamp());
          }
          else
          {
            if (int32 > 0 && cachestamp.HasValue)
            {
              long num3 = int64;
              long? nullable = cachestamp;
              long valueOrDefault = nullable.GetValueOrDefault();
              if (num3 <= valueOrDefault & nullable.HasValue && tableDescriptor.UserCacheStamp > 0L)
              {
                row.IsDeleted = true;
                continue;
              }
            }
            if (int32 == 0)
            {
              row[0] = (object) (1932735283 + num2);
              row[4] = (object) bucket.GenerateCacheStamp(num2);
              ++num2;
            }
            else
              row[4] = (object) bucket.GetBaseCachestamp();
          }
        }
        else
        {
          row[0] = (object) num2;
          row[4] = (object) bucket.GenerateCacheStamp(num2);
          ++num2;
        }
      }
      if (flag2)
        payloadTable.RemoveDeletedRows();
      if (flag1 || num2 > localId || !compatContext.IsBelowDeletedRowGenerationLimit(requestContext, num2, localId, nameof (GenerateConstantSets)))
        return;
      for (; num2 <= localId; ++num2)
      {
        PayloadTable.PayloadRow payloadRow = payloadTable.AddNewPayloadRow();
        payloadRow[0] = !flag2 ? (object) num2 : (object) (1932735283 + num2);
        payloadRow[3] = (object) true;
        payloadRow[4] = (object) bucket.GenerateCacheStamp(num2);
      }
    }

    private static Dictionary<int, HashSet<int>> BuildSetMappings(
      IVssRequestContext requestContext,
      PayloadTable payloadTable)
    {
      return requestContext.TraceBlock<Dictionary<int, HashSet<int>>>(900829, 900830, "WorkItemMetadataCompatibilityService", nameof (WorkItemMetadataGenerator), nameof (BuildSetMappings), (Func<Dictionary<int, HashSet<int>>>) (() =>
      {
        Dictionary<int, HashSet<int>> dictionary = new Dictionary<int, HashSet<int>>(payloadTable.RowCount);
        foreach (PayloadTable.PayloadRow row in payloadTable.Rows)
        {
          if (!Convert.ToBoolean(row[3]))
          {
            int int32 = Convert.ToInt32(row[1]);
            HashSet<int> intSet;
            if (!dictionary.TryGetValue(int32, out intSet))
            {
              intSet = new HashSet<int>();
              dictionary[int32] = intSet;
            }
            intSet.Add(Convert.ToInt32(row[2]));
          }
        }
        return dictionary;
      }));
    }

    private static void AddNewConstantSetRow(PayloadTable setsTable, int parentId, int constId)
    {
      PayloadTable.PayloadRow payloadRow = setsTable.AddNewPayloadRow();
      payloadRow[1] = (object) parentId;
      payloadRow[2] = (object) constId;
      payloadRow[3] = (object) false;
    }

    private static void AddMissingWorkItemTypeSetEntries(
      IVssRequestContext requestContext,
      MetadataCompatibilityContext compatContext,
      string typeListName,
      string typeName,
      Dictionary<int, HashSet<int>> existingSetMappings,
      PayloadTable setsTable)
    {
      int constId;
      if (compatContext.ConstantMap != null && compatContext.ConstantMap.TryGetValue(typeName, out constId))
      {
        int num;
        if (compatContext.ConstantMap.TryGetValue(typeListName, out num))
        {
          HashSet<int> intSet;
          if (!existingSetMappings.TryGetValue(num, out intSet))
          {
            intSet = new HashSet<int>();
            existingSetMappings[num] = intSet;
          }
          if (intSet.Contains(constId))
            return;
          WorkItemMetadataGenerator.AddNewConstantSetRow(setsTable, num, constId);
          intSet.Add(constId);
        }
        else
          MetadataCompatibilityContext.ReportError(requestContext, "GenerateConstantSets", string.Format("Constant not found for work item type allowed values list '{0}'.", (object) typeListName));
      }
      else
        MetadataCompatibilityContext.ReportError(requestContext, "GenerateConstantSets", string.Format("Constant not found for work item type '{0}'.", (object) typeName));
    }

    private static void AddOutOfBoxAllowedAndSuggestedValueSets(
      IVssRequestContext requestContext,
      ProcessDescriptor processDescriptor,
      MetadataWorkItemTypeCompatibilityDescriptor typeDescriptor,
      MetadataCompatibilityContext compatContext,
      Dictionary<int, HashSet<int>> existingSetMappings,
      PayloadTable setsTable)
    {
      IReadOnlyCollection<WorkItemFieldRule> oobRules;
      if (!typeDescriptor.TryGetOutOfBoxRulesAndHelpTexts(requestContext, processDescriptor, out oobRules, out IReadOnlyCollection<HelpTextDescriptor> _))
        return;
      foreach (ListRule listRule in ((IEnumerable<ListRule>) oobRules.SelectMany<WorkItemFieldRule, AllowedValuesRule>((Func<WorkItemFieldRule, IEnumerable<AllowedValuesRule>>) (r => r.SelectRules<AllowedValuesRule>()))).Union<ListRule>((IEnumerable<ListRule>) oobRules.SelectMany<WorkItemFieldRule, SuggestedValuesRule>((Func<WorkItemFieldRule, IEnumerable<SuggestedValuesRule>>) (r => r.SelectRules<SuggestedValuesRule>()))).Where<ListRule>((Func<ListRule, bool>) (r => r.Id != Guid.Empty)))
        WorkItemMetadataGenerator.AddMissingListRuleSetEntries(requestContext, compatContext, existingSetMappings, listRule.Id.ToString("D"), (IReadOnlyCollection<string>) listRule.Values, setsTable);
    }

    private static void AddMissingPicklistSetEntries(
      IVssRequestContext requestContext,
      MetadataCompatibilityContext compatContext,
      Dictionary<int, HashSet<int>> existingSetMappings,
      PayloadTable setsTable,
      WorkItemPickList picklist)
    {
      HashSet<int> intSet;
      if (!existingSetMappings.TryGetValue(picklist.ConstId, out intSet))
      {
        intSet = new HashSet<int>();
        existingSetMappings[picklist.ConstId] = intSet;
      }
      if (picklist.Items != null)
      {
        foreach (WorkItemPickListMember itemPickListMember in (IEnumerable<WorkItemPickListMember>) picklist.Items)
        {
          if (!intSet.Contains(itemPickListMember.ConstId))
          {
            WorkItemMetadataGenerator.AddNewConstantSetRow(setsTable, picklist.ConstId, itemPickListMember.ConstId);
            intSet.Add(itemPickListMember.ConstId);
          }
        }
      }
      int num;
      if (compatContext.ConstantMap != null && compatContext.ConstantMap.TryGetValue("299f07ef-6201-41b3-90fc-03eeb3977587", out num))
      {
        if (!existingSetMappings.TryGetValue(num, out intSet))
        {
          intSet = new HashSet<int>();
          existingSetMappings[num] = intSet;
        }
        if (intSet.Contains(picklist.ConstId))
          return;
        WorkItemMetadataGenerator.AddNewConstantSetRow(setsTable, num, picklist.ConstId);
        intSet.Add(picklist.ConstId);
      }
      else
        MetadataCompatibilityContext.ReportError(requestContext, "GenerateConstantSets", "Could not find GlobalListsSetName constant.");
    }

    private static void AddMissingListRuleSetEntries(
      IVssRequestContext requestContext,
      MetadataCompatibilityContext compatContext,
      Dictionary<int, HashSet<int>> existingSetMappings,
      string listName,
      IReadOnlyCollection<string> values,
      PayloadTable setsTable)
    {
      int num;
      if (compatContext.ConstantMap != null && compatContext.ConstantMap.TryGetValue(listName, out num))
      {
        HashSet<int> intSet;
        if (!existingSetMappings.TryGetValue(num, out intSet))
        {
          intSet = new HashSet<int>();
          existingSetMappings[num] = intSet;
        }
        foreach (string key in (IEnumerable<string>) values)
        {
          int constId;
          if (compatContext.ConstantMap.TryGetValue(key, out constId))
          {
            if (!intSet.Contains(constId))
            {
              WorkItemMetadataGenerator.AddNewConstantSetRow(setsTable, num, constId);
              intSet.Add(constId);
            }
          }
          else
            MetadataCompatibilityContext.ReportError(requestContext, "GenerateConstantSets", string.Format("Could not find constant for value {0} of list {1}.", (object) key, (object) listName));
        }
      }
      else
        MetadataCompatibilityContext.ReportError(requestContext, "GenerateConstantSets", string.Format("Could not find list head constant {0}.", (object) listName));
    }

    internal static void GenerateRules(
      IVssRequestContext requestContext,
      MetadataCompatibilityContext compatContext,
      PayloadTable payloadTable,
      WorkItemMetadataCompatibilityService.MetadataTableDescriptor tableDescriptor,
      out int newMaskBits)
    {
      newMaskBits = 0;
      bool flag1 = tableDescriptor.UserCacheStamp > 0L;
      int num1 = WorkItemTrackingFeatureFlags.IsFullOrPartialRuleGenerationEnabled(requestContext) ? 1 : 0;
      bool flag2 = WorkItemTrackingFeatureFlags.IsProcessCustomizationEnabled(requestContext);
      MetadataBucket bucket = tableDescriptor.Bucket;
      int localId1 = bucket.ParseLocalId(tableDescriptor.UserCacheStamp);
      IReadOnlyCollection<RuleRecord> ruleRecords = (IReadOnlyCollection<RuleRecord>) Array.Empty<RuleRecord>();
      if (num1 != 0 && !(flag1 & flag2))
        ruleRecords = CompatibilityRulesGenerator.GetOutOfBoxRules(requestContext, compatContext);
      IReadOnlyCollection<RuleRecord> customRules = CompatibilityRulesGenerator.GetCustomRules(requestContext, compatContext);
      payloadTable.AddCapacity(ruleRecords.Count + customRules.Count + compatContext.GetDeletedRowGenerationLimit(requestContext));
      foreach (RuleRecord ruleRecord in (IEnumerable<RuleRecord>) ruleRecords)
        WorkItemMetadataGenerator.AddNewRuleRow(payloadTable, ruleRecord);
      WorkItemMetadataGenerator.DeleteStateRulesForCustomizedWorkItemTypes(requestContext, compatContext, payloadTable);
      int localId2 = 0;
      foreach (RuleRecord ruleRecord in (IEnumerable<RuleRecord>) customRules)
      {
        ++localId2;
        PayloadTable.PayloadRow payloadRow = WorkItemMetadataGenerator.AddNewRuleRow(payloadTable, ruleRecord);
        if (flag2)
          payloadRow[0] = (object) (1932735283 + localId2);
      }
      MetadataBucket.EnsureBucketCapacity(Math.Max(payloadTable.RowCount, localId1), ref bucket, ref newMaskBits);
      int localId3 = 1;
      foreach (PayloadTable.PayloadRow row in payloadTable.Rows)
      {
        if (!flag2 || Convert.ToInt32(row[0]) < 1932735283)
          row[0] = (object) localId3;
        row[26] = !flag2 ? (object) bucket.GenerateCacheStamp(localId3) : (localId2 <= 0 ? (object) bucket.GetBaseCachestamp() : (object) bucket.GenerateCacheStamp(localId2));
        ++localId3;
      }
      int num2 = flag2 ? localId2 : localId3;
      if (localId1 <= 0 || num2 > localId1 || !compatContext.IsBelowDeletedRowGenerationLimit(requestContext, num2, localId1, nameof (GenerateRules)))
        return;
      if (flag2)
      {
        while (num2 <= localId1 - 1)
        {
          PayloadTable.PayloadRow payloadRow = payloadTable.AddNewPayloadRow();
          payloadRow[0] = (object) (++num2 + 1932735283);
          payloadRow[25] = (object) true;
          payloadRow[26] = (object) bucket.GenerateCacheStamp(num2);
        }
      }
      else
      {
        for (; num2 <= localId1; ++num2)
        {
          PayloadTable.PayloadRow payloadRow = payloadTable.AddNewPayloadRow();
          payloadRow[0] = (object) num2;
          payloadRow[25] = (object) true;
          payloadRow[26] = (object) bucket.GenerateCacheStamp(num2);
        }
      }
    }

    private static void DeleteStateRulesForCustomizedWorkItemTypes(
      IVssRequestContext requestContext,
      MetadataCompatibilityContext compatContext,
      PayloadTable payloadTable)
    {
      Dictionary<int, HashSet<int>> customizedStatesMap = CompatibilityRulesGenerator.GetProjectToTypesWithCustomizedStatesMap(requestContext, compatContext);
      if (!customizedStatesMap.Any<KeyValuePair<int, HashSet<int>>>())
        return;
      string field1 = string.Format("Fld{0}IsConstID", (object) 1);
      string field2 = string.Format("Fld{0}ID", (object) 2);
      foreach (PayloadTable.PayloadRow row in payloadTable.Rows)
      {
        HashSet<int> intSet;
        if (customizedStatesMap.TryGetValue(Convert.ToInt32(row[1]), out intSet))
        {
          int num1 = intSet.Contains(Convert.ToInt32(row[field1])) ? 1 : 0;
          bool flag1 = Convert.ToInt32(row[field2]) == 0;
          bool flag2 = Convert.ToInt32(row[21]) == 2;
          int num2 = flag1 ? 1 : 0;
          if ((num1 & num2 & (flag2 ? 1 : 0)) != 0)
            row[25] = (object) true;
        }
      }
    }

    private static PayloadTable.PayloadRow AddNewRuleRow(
      PayloadTable payloadTable,
      RuleRecord ruleRecord)
    {
      PayloadTable.PayloadRow payloadRow = payloadTable.AddNewPayloadRow();
      payloadRow[1] = (object) ruleRecord.RootTreeID;
      payloadRow[2] = (object) ruleRecord.AreaID;
      payloadRow[3] = (object) ruleRecord.PersonID;
      payloadRow[4] = (object) ruleRecord.ObjectTypeScopeID;
      payloadRow[5] = (object) ruleRecord.Fld1ID;
      payloadRow[6] = (object) ruleRecord.Fld1IsConstID;
      payloadRow[7] = (object) ruleRecord.Fld1WasConstID;
      payloadRow[8] = (object) ruleRecord.Fld2ID;
      payloadRow[9] = (object) ruleRecord.Fld2IsConstID;
      payloadRow[10] = (object) ruleRecord.Fld2WasConstID;
      payloadRow[11] = (object) ruleRecord.Fld3ID;
      payloadRow[12] = (object) ruleRecord.Fld3IsConstID;
      payloadRow[13] = (object) ruleRecord.Fld3WasConstID;
      payloadRow[14] = (object) ruleRecord.Fld4ID;
      payloadRow[15] = (object) ruleRecord.Fld4IsConstID;
      payloadRow[16] = (object) ruleRecord.Fld4WasConstID;
      payloadRow[17] = (object) ruleRecord.IfFldID;
      payloadRow[18] = (object) ruleRecord.IfConstID;
      payloadRow[19] = (object) ruleRecord.If2FldID;
      payloadRow[20] = (object) ruleRecord.If2ConstID;
      payloadRow[21] = (object) ruleRecord.ThenFldID;
      payloadRow[22] = (object) ruleRecord.ThenConstID;
      payloadRow[23] = (object) ruleRecord.RuleFlags;
      payloadRow[24] = (object) ruleRecord.RuleFlags2;
      payloadRow[25] = (object) false;
      return payloadRow;
    }

    internal static void GenerateProperties(
      IVssRequestContext requestContext,
      MetadataCompatibilityContext compatContext,
      PayloadTable payloadTable,
      WorkItemMetadataCompatibilityService.MetadataTableDescriptor tableDescriptor,
      out int newMaskBits)
    {
      newMaskBits = 0;
      int num1 = 0;
      bool flag = true;
      if (compatContext.TypeIdToFormPropIdMap == null)
        compatContext.TypeIdToFormPropIdMap = (IDictionary<int, int>) new Dictionary<int, int>();
      if (compatContext.TypeIdToDescriptionPropIdMap == null)
        compatContext.TypeIdToDescriptionPropIdMap = (IDictionary<int, int>) new Dictionary<int, int>();
      IEnumerable<FormProperties> formProperties1 = requestContext.GetService<WorkItemMetadataCompatibilityService>().GetFormProperties(requestContext, compatContext);
      foreach (PayloadTable.PayloadRow row in payloadTable.Rows)
      {
        if (!Convert.ToBoolean(row[5]))
        {
          int int32 = Convert.ToInt32(row[0]);
          int workItemTypeId = -1;
          if (row[7] == null)
            compatContext.TypeIdToDescriptionPropIdMap[workItemTypeId] = int32;
          else if (int.TryParse(row[7].ToString(), out workItemTypeId))
          {
            FormProperties formProperties2 = formProperties1.FirstOrDefault<FormProperties>((Func<FormProperties, bool>) (property => property.WorkItemTypeId == workItemTypeId));
            if (formProperties2 != null)
              row[4] = (object) formProperties2.Form;
            else
              MetadataCompatibilityContext.ReportError(requestContext, nameof (GenerateProperties), string.Format("FormProperties not found for work item type {0}.", (object) workItemTypeId));
            compatContext.TypeIdToFormPropIdMap[workItemTypeId] = int32;
          }
          num1 = Math.Max(num1, int32);
        }
      }
      foreach (MetadataProjectCompatibilityDescriptor projectDescriptor in (IEnumerable<MetadataProjectCompatibilityDescriptor>) compatContext.ProjectDescriptors)
      {
        ProcessDescriptor processDescriptor;
        foreach (MetadataWorkItemTypeCompatibilityDescriptor compatibilityDescriptor in (!flag || !projectDescriptor.TryGetProcessDescriptor(requestContext, out processDescriptor) ? 0 : (!processDescriptor.IsCustom ? 1 : 0)) != 0 ? (IEnumerable<MetadataWorkItemTypeCompatibilityDescriptor>) projectDescriptor.TypeDescriptors : projectDescriptor.TypeDescriptors.Where<MetadataWorkItemTypeCompatibilityDescriptor>((Func<MetadataWorkItemTypeCompatibilityDescriptor, bool>) (t => t.Type.IsCustomType)))
        {
          int workItemTypeId = compatibilityDescriptor.Type.Id.Value;
          int num2 = ++num1;
          FormProperties formProperties3 = formProperties1.FirstOrDefault<FormProperties>((Func<FormProperties, bool>) (property => property.WorkItemTypeId == workItemTypeId));
          string str = formProperties3 != null ? formProperties3.Form : compatibilityDescriptor.Form;
          if (!compatContext.TypeIdToFormPropIdMap.ContainsKey(workItemTypeId))
          {
            PayloadTable.PayloadRow payloadRow = payloadTable.AddNewPayloadRow();
            payloadRow[0] = (object) num2;
            payloadRow[1] = (object) projectDescriptor.ProjectNode.Id;
            payloadRow[2] = (object) 0;
            payloadRow[7] = (object) workItemTypeId;
            payloadRow[3] = (object) Guid.NewGuid().ToString("D");
            payloadRow[4] = (object) str;
            payloadRow[5] = (object) false;
            compatContext.TypeIdToFormPropIdMap[workItemTypeId] = num2;
          }
          if (!compatContext.TypeIdToDescriptionPropIdMap.ContainsKey(workItemTypeId))
          {
            int num3 = ++num1;
            PayloadTable.PayloadRow payloadRow = payloadTable.AddNewPayloadRow();
            payloadRow[0] = (object) num3;
            payloadRow[1] = (object) projectDescriptor.ProjectNode.Id;
            payloadRow[2] = (object) 0;
            payloadRow[3] = (object) Guid.NewGuid().ToString("D");
            payloadRow[4] = (object) compatibilityDescriptor.Type.Description;
            payloadRow[5] = (object) false;
            compatContext.TypeIdToDescriptionPropIdMap[workItemTypeId] = num3;
          }
        }
      }
      MetadataBucket bucket = tableDescriptor.Bucket;
      int localId = bucket.ParseLocalId(tableDescriptor.UserCacheStamp);
      MetadataBucket.EnsureBucketCapacity(Math.Max(num1, localId), ref bucket, ref newMaskBits);
      foreach (PayloadTable.PayloadRow row in payloadTable.Rows)
      {
        int int32 = Convert.ToInt32(row["PropID"]);
        row[6] = (object) bucket.GenerateCacheStamp(int32 > 0 ? int32 : 1);
      }
      if (num1 >= localId || !compatContext.IsBelowDeletedRowGenerationLimit(requestContext, num1, localId, nameof (GenerateProperties)))
        return;
      while (num1 < localId && num1 != 0)
      {
        PayloadTable.PayloadRow payloadRow = payloadTable.AddNewPayloadRow();
        payloadRow[0] = (object) ++num1;
        payloadRow[1] = (object) 0;
        payloadRow[2] = (object) 0;
        payloadRow[7] = (object) 0;
        payloadRow[3] = (object) Guid.NewGuid().ToString("D");
        payloadRow[4] = (object) string.Empty;
        payloadRow[5] = (object) true;
        payloadRow[6] = (object) bucket.GenerateCacheStamp(num1);
      }
    }

    internal static void GenerateWorkItemTypeCategoryMembers(
      IVssRequestContext requestContext,
      MetadataCompatibilityContext compatContext,
      PayloadTable payloadTable,
      WorkItemMetadataCompatibilityService.MetadataTableDescriptor tableDescriptor,
      out int newMaskBits)
    {
      newMaskBits = 0;
      Dictionary<int, HashSet<int>> dictionary = new Dictionary<int, HashSet<int>>();
      foreach (PayloadTable.PayloadRow row in payloadTable.Rows)
      {
        int int32_1 = Convert.ToInt32(row[1]);
        int int32_2 = Convert.ToInt32(row[2]);
        if (!Convert.ToBoolean(row[3]))
        {
          HashSet<int> intSet;
          if (!dictionary.TryGetValue(int32_1, out intSet))
          {
            intSet = new HashSet<int>();
            dictionary[int32_1] = intSet;
          }
          intSet.Add(int32_2);
        }
      }
      foreach (MetadataProjectCompatibilityDescriptor projectDescriptor in (IEnumerable<MetadataProjectCompatibilityDescriptor>) compatContext.ProjectDescriptors)
      {
        List<WorkItemType> list = projectDescriptor.TypeDescriptors.Select<MetadataWorkItemTypeCompatibilityDescriptor, WorkItemType>((Func<MetadataWorkItemTypeCompatibilityDescriptor, WorkItemType>) (d => d.Type)).ToList<WorkItemType>();
        if (list.Any<WorkItemType>())
        {
          IWorkItemTypeCategoryService service = requestContext.GetService<IWorkItemTypeCategoryService>();
          IEnumerable<WorkItemTypeCategory> source = projectDescriptor.WorkItemTypeCategories == null ? service.GetWorkItemTypeCategories(requestContext.Elevate(), projectDescriptor.ProjectNode.ProjectId) : projectDescriptor.WorkItemTypeCategories;
          foreach (WorkItemType workItemType in list)
          {
            WorkItemType type = workItemType;
            foreach (WorkItemTypeCategory itemTypeCategory in source.Where<WorkItemTypeCategory>((Func<WorkItemTypeCategory, bool>) (c => c.WorkItemTypeNames.Any<string>((Func<string, bool>) (n => TFStringComparer.WorkItemTypeName.Equals(n, type.Name))))).ToList<WorkItemTypeCategory>())
            {
              int num;
              if (!projectDescriptor.CategoryIdsByRefName.TryGetValue(itemTypeCategory.ReferenceName, out num))
                MetadataCompatibilityContext.ReportError(requestContext, nameof (GenerateWorkItemTypeCategoryMembers), string.Format("CategoryId not found in map for category refname {0} in project {1}.", (object) itemTypeCategory.ReferenceName, (object) projectDescriptor.ProjectNode.ProjectId));
              int typeId = type.Id.Value;
              HashSet<int> intSet;
              if (!dictionary.TryGetValue(num, out intSet))
              {
                intSet = new HashSet<int>();
                dictionary[num] = intSet;
              }
              if (!intSet.Contains(typeId))
              {
                WorkItemMetadataGenerator.AddNewWorkItemTypeCategoryMembersRow(payloadTable, num, typeId);
                intSet.Add(typeId);
              }
            }
          }
        }
      }
      MetadataBucket bucket = tableDescriptor.Bucket;
      int localId = bucket.ParseLocalId(tableDescriptor.UserCacheStamp);
      MetadataBucket.EnsureBucketCapacity(Math.Max(payloadTable.RowCount, localId), ref bucket, ref newMaskBits);
      int num1 = 1;
      foreach (PayloadTable.PayloadRow row in payloadTable.Rows)
      {
        row[0] = (object) num1;
        row[4] = (object) bucket.GenerateCacheStamp(num1);
        ++num1;
      }
      if (num1 > localId || !compatContext.IsBelowDeletedRowGenerationLimit(requestContext, num1, localId, nameof (GenerateWorkItemTypeCategoryMembers)))
        return;
      for (; num1 <= localId; ++num1)
      {
        PayloadTable.PayloadRow payloadRow = payloadTable.AddNewPayloadRow();
        payloadRow[0] = (object) num1;
        payloadRow[3] = (object) true;
        payloadRow[4] = (object) bucket.GenerateCacheStamp(num1);
      }
    }

    private static void AddNewWorkItemTypeCategoryMembersRow(
      PayloadTable workItemTypeCategoryMembersTable,
      int categoryId,
      int typeId)
    {
      PayloadTable.PayloadRow payloadRow = workItemTypeCategoryMembersTable.AddNewPayloadRow();
      payloadRow[1] = (object) categoryId;
      payloadRow[2] = (object) typeId;
      payloadRow[3] = (object) false;
    }

    internal static void GenerateWorkItemTypeUsages(
      IVssRequestContext requestContext,
      MetadataCompatibilityContext compatContext,
      PayloadTable payloadTable,
      WorkItemMetadataCompatibilityService.MetadataTableDescriptor tableDescriptor,
      out int newMaskBits)
    {
      newMaskBits = 0;
      requestContext.GetService<WorkItemMetadataCompatibilityService>();
      Dictionary<int, HashSet<int>> dictionary = new Dictionary<int, HashSet<int>>(payloadTable.RowCount);
      foreach (PayloadTable.PayloadRow row in payloadTable.Rows)
      {
        if (!Convert.ToBoolean(row[4]))
        {
          int int32_1 = Convert.ToInt32(row[1]);
          int int32_2 = Convert.ToInt32(row[2]);
          HashSet<int> intSet;
          if (!dictionary.TryGetValue(int32_2, out intSet))
          {
            intSet = new HashSet<int>();
            dictionary[int32_2] = intSet;
          }
          intSet.Add(int32_1);
        }
      }
      foreach (MetadataWorkItemTypeCompatibilityDescriptor compatibilityDescriptor in compatContext.ProjectDescriptors.SelectMany<MetadataProjectCompatibilityDescriptor, MetadataWorkItemTypeCompatibilityDescriptor>((Func<MetadataProjectCompatibilityDescriptor, IEnumerable<MetadataWorkItemTypeCompatibilityDescriptor>>) (d => (IEnumerable<MetadataWorkItemTypeCompatibilityDescriptor>) d.TypeDescriptors)))
      {
        int key = compatibilityDescriptor.Type.Id.Value;
        HashSet<int> intSet;
        if (!dictionary.TryGetValue(key, out intSet))
        {
          intSet = new HashSet<int>();
          dictionary[key] = intSet;
        }
        List<FieldEntry> list = compatibilityDescriptor.Fields.Where<FieldEntry>((Func<FieldEntry, bool>) (f => f.FieldId >= 10000)).ToList<FieldEntry>();
        if (compatibilityDescriptor.Type.IsCustomType)
          list.AddRange((IEnumerable<FieldEntry>) compatibilityDescriptor.Type.Source.GetCombinedFields(requestContext));
        foreach (FieldEntry fieldEntry in list)
        {
          if (!intSet.Contains(fieldEntry.FieldId))
          {
            PayloadTable.PayloadRow payloadRow = payloadTable.AddNewPayloadRow();
            payloadRow[2] = (object) key;
            payloadRow[1] = (object) fieldEntry.FieldId;
            payloadRow[4] = (object) false;
            intSet.Add(fieldEntry.FieldId);
          }
        }
      }
      MetadataBucket bucket = tableDescriptor.Bucket;
      int localId = bucket.ParseLocalId(tableDescriptor.UserCacheStamp);
      MetadataBucket.EnsureBucketCapacity(Math.Max(payloadTable.RowCount, localId), ref bucket, ref newMaskBits);
      int num = 1;
      foreach (PayloadTable.PayloadRow row in payloadTable.Rows)
      {
        row[0] = (object) num;
        row[5] = (object) bucket.GenerateCacheStamp(num);
        ++num;
      }
      if (num > localId || !compatContext.IsBelowDeletedRowGenerationLimit(requestContext, num, localId, nameof (GenerateWorkItemTypeUsages)))
        return;
      for (; num <= localId; ++num)
      {
        PayloadTable.PayloadRow payloadRow = payloadTable.AddNewPayloadRow();
        payloadRow[0] = (object) num;
        payloadRow[4] = (object) true;
        payloadRow[5] = (object) bucket.GenerateCacheStamp(num);
      }
    }

    internal static void GenerateWorkItemTypes(
      IVssRequestContext requestContext,
      MetadataCompatibilityContext compatContext,
      PayloadTable payloadTable,
      WorkItemMetadataCompatibilityService.MetadataTableDescriptor tableDescriptor,
      out int newMaskBits)
    {
      int traceNum = 0;
      newMaskBits = 0;
      try
      {
        WorkItemMetadataGenerator._GenerateWorkItemTypes(requestContext, compatContext, payloadTable, tableDescriptor, out newMaskBits, out traceNum);
      }
      catch (Exception ex)
      {
        string message = string.Format("{0} \n {1} \n {2}", (object) (ex.Message + "\n" + string.Format("Trace Num v2 is: {0}\n", (object) traceNum)), (object) ex.InnerException?.ToString(), (object) ex.StackTrace);
        requestContext.Trace(910601, TraceLevel.Error, "Services", "MetadataCompatibility", message);
        throw;
      }
    }

    internal static void _GenerateWorkItemTypes(
      IVssRequestContext requestContext,
      MetadataCompatibilityContext compatContext,
      PayloadTable payloadTable,
      WorkItemMetadataCompatibilityService.MetadataTableDescriptor tableDescriptor,
      out int newMaskBits,
      out int traceNum)
    {
      newMaskBits = 0;
      traceNum = 0;
      CustomerIntelligenceData properties = new CustomerIntelligenceData();
      if (compatContext != null && payloadTable != null)
      {
        traceNum = 1;
        HashSet<int> collection = new HashSet<int>();
        Dictionary<int, HashSet<int>> dictionary = new Dictionary<int, HashSet<int>>();
        foreach (PayloadTable.PayloadRow row in payloadTable.Rows)
        {
          if (row != null)
          {
            int num1 = (int) row[0];
            if (Convert.ToBoolean(row[4]))
            {
              traceNum = 2;
            }
            else
            {
              int key = (int) row[2];
              int num2 = (int) row[1];
              HashSet<int> intSet;
              if (!dictionary.TryGetValue(key, out intSet))
              {
                traceNum = 3;
                intSet = new HashSet<int>();
                dictionary[key] = intSet;
              }
              if (!intSet.Contains(num2))
              {
                traceNum = 4;
                intSet.Add(num2);
              }
            }
          }
        }
        int num3 = 0;
        foreach (MetadataProjectCompatibilityDescriptor projectDescriptor in (IEnumerable<MetadataProjectCompatibilityDescriptor>) compatContext.ProjectDescriptors)
        {
          if (projectDescriptor != null)
          {
            traceNum = 5;
            IEnumerable<WorkItemType> workItemTypes = projectDescriptor.TypeDescriptors.Select<MetadataWorkItemTypeCompatibilityDescriptor, WorkItemType>((Func<MetadataWorkItemTypeCompatibilityDescriptor, WorkItemType>) (t => t.Type)).Where<WorkItemType>((Func<WorkItemType, bool>) (t => t.Name != null));
            int num4 = 0;
            foreach (WorkItemType workItemType in workItemTypes)
            {
              if (workItemType != null)
              {
                traceNum = 6;
                int num5;
                if (compatContext.ConstantMap != null && compatContext.ConstantMap.TryGetValue(workItemType.Name, out num5))
                {
                  traceNum = 7;
                  HashSet<int> intSet;
                  if (projectDescriptor.ProjectNode != null && dictionary.TryGetValue(projectDescriptor.ProjectNode.Id, out intSet) && intSet.Contains(num5))
                  {
                    traceNum = 8;
                    requestContext.Trace(910601, TraceLevel.Verbose, "Services", "MetadataCompatibility", "Trace Num is: {0}", (object) traceNum);
                  }
                  else
                  {
                    traceNum = 9;
                    int num6 = 0;
                    if (compatContext.TypeIdToDescriptionPropIdMap == null || !compatContext.TypeIdToDescriptionPropIdMap.TryGetValue(workItemType.Id.Value, out num6))
                    {
                      traceNum = 10;
                      MetadataCompatibilityContext.ReportError(requestContext, "GenerateWorkItemTypes", "Failed to find description propid for WIT named " + workItemType.Name + " with int ID " + workItemType.Id.ToString() + " in project with node ID " + projectDescriptor.ProjectNode.Id.ToString());
                    }
                    traceNum = 11;
                    PayloadTable.PayloadRow payloadRow = payloadTable.AddNewPayloadRow();
                    payloadRow[0] = (object) workItemType.Id;
                    payloadRow[1] = (object) num5;
                    payloadRow[2] = (object) projectDescriptor.ProjectNode.Id;
                    payloadRow[3] = (object) num6;
                    payloadRow[4] = (object) false;
                    traceNum = 12;
                  }
                }
                else
                {
                  traceNum = 13;
                  MetadataCompatibilityContext.ReportError(requestContext, "GenerateWorkItemTypes", string.Format("Work item type name constant '{0}' not found.", (object) workItemType.Name));
                }
              }
              else
              {
                traceNum = 14;
                ++num4;
              }
            }
            traceNum = 15;
            collection.AddRange<int, HashSet<int>>(projectDescriptor.TypeDescriptors.Select<MetadataWorkItemTypeCompatibilityDescriptor, int>((Func<MetadataWorkItemTypeCompatibilityDescriptor, int>) (d => d.Type.Id.Value)));
            if (num4 > 0)
            {
              traceNum = 16;
              properties.Add(string.Format("NullTypesCountOfProjectDescriptor{0}", (object) projectDescriptor.ProjectNode.Id), (double) num4);
            }
          }
          else
          {
            traceNum = 17;
            ++num3;
          }
        }
        traceNum = 18;
        if (num3 > 0)
        {
          traceNum = 19;
          properties.Add("NullProjectDescriptorsCount", (double) num3);
        }
        int num7 = 1;
        foreach (PayloadTable.PayloadRow row in payloadTable.Rows)
          num7 = row != null ? Math.Max(num7, Convert.ToInt32(row[0])) : num7;
        traceNum = 20;
        MetadataBucket bucket = tableDescriptor.Bucket;
        int localId = bucket.ParseLocalId(tableDescriptor.UserCacheStamp);
        MetadataBucket.EnsureBucketCapacity(Math.Max(num7, localId), ref bucket, ref newMaskBits);
        List<int> intList = new List<int>();
        for (int index = 0; index < payloadTable.Rows.Count; ++index)
        {
          PayloadTable.PayloadRow row = payloadTable.Rows[index];
          if (row != null)
          {
            traceNum = 21;
            int int32 = Convert.ToInt32(row[0]);
            row[5] = (object) bucket.GenerateCacheStamp(int32 <= 0 ? 1 : int32);
            if (!collection.Contains(int32) && int32 > 0)
            {
              traceNum = 22;
              row[4] = (object) true;
            }
          }
          else
          {
            traceNum = 23;
            intList.Add(index);
          }
        }
        if (intList.Count > 0)
        {
          traceNum = 24;
          properties.Add("PayloadTableNullRowIndices", (object) intList);
        }
        if (num7 < localId && compatContext.IsBelowDeletedRowGenerationLimit(requestContext, num7, localId, "GenerateWorkItemTypes"))
        {
          traceNum = 25;
          while (num7 < localId)
          {
            PayloadTable.PayloadRow payloadRow = payloadTable.AddNewPayloadRow();
            payloadRow[0] = (object) ++num7;
            payloadRow[4] = (object) true;
            payloadRow[5] = (object) bucket.GenerateCacheStamp(num7);
          }
        }
      }
      else
      {
        if (compatContext == null)
        {
          traceNum = 26;
          properties.Add("CompatContextIsNull", string.Empty);
        }
        if (payloadTable == null)
        {
          traceNum = 27;
          properties.Add("PayloadTableIsNull", string.Empty);
        }
      }
      if (!properties.GetData().Any<KeyValuePair<string, object>>())
        return;
      traceNum = 28;
      requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, nameof (WorkItemMetadataGenerator), "GenerateWorkItemTypes", properties);
    }

    internal class ActionsTableColumnIndexes
    {
      internal const int ActionID = 0;
      internal const int ActionName = 1;
      internal const int WorkItemTypeID = 2;
      internal const int ActionFromStateConstId = 3;
      internal const int ActionToStateConstId = 4;
      internal const int fDeleted = 5;
      internal const int CacheStamp = 6;
    }

    internal class CategoriesTableColumnIndexes
    {
      internal const int WorkItemTypeCategoryId = 0;
      internal const int ProjectID = 1;
      internal const int WorkItemTypeCategoryName = 2;
      internal const int WorkItemTypeCategoryReferenceName = 3;
      internal const int WorkItemTypeCategoryDefaultWorkItemTypeID = 4;
      internal const int fDeleted = 5;
      internal const int CacheStamp = 6;
    }

    internal class ConstantSetsTableColumnIndexes
    {
      internal const int RuleSetID = 0;
      internal const int ParentID = 1;
      internal const int ConstId = 2;
      internal const int fDeleted = 3;
      internal const int CacheStamp = 4;
    }

    internal class RuleTableColumnIndexes
    {
      internal const int RuleId = 0;
      internal const int RootTreeId = 1;
      internal const int AreaId = 2;
      internal const int PersonId = 3;
      internal const int ObjectTypeScopeId = 4;
      internal const int Field1Id = 5;
      internal const int Field1IsConstId = 6;
      internal const int Field1WasConstId = 7;
      internal const int Field2Id = 8;
      internal const int Field2IsConstId = 9;
      internal const int Field2WasConstId = 10;
      internal const int Field3Id = 11;
      internal const int Field3IsConstId = 12;
      internal const int Field3WasConstId = 13;
      internal const int Field4Id = 14;
      internal const int Field4IsConstId = 15;
      internal const int Field4WasConstId = 16;
      internal const int IfFieldId = 17;
      internal const int IfConstId = 18;
      internal const int If2FieldId = 19;
      internal const int If2ConstId = 20;
      internal const int ThenFieldId = 21;
      internal const int ThenConstId = 22;
      internal const int RuleFlags1 = 23;
      internal const int RuleFlags2 = 24;
      internal const int fDeleted = 25;
      internal const int CacheStamp = 26;
    }

    internal class HierarchyPropertiesTableColumnIndexes
    {
      internal const int PropID = 0;
      internal const int AreaId = 1;
      internal const int TreeType = 2;
      internal const int Name = 3;
      internal const int Value = 4;
      internal const int fDeleted = 5;
      internal const int CacheStamp = 6;
      internal const int WorkItemTypeID = 7;
    }

    internal class CategoryMembersTableColumnIndexes
    {
      internal const int WorkItemTypeCategoryMemberId = 0;
      internal const int WorkItemTypeCategoryId = 1;
      internal const int WorkItemTypeID = 2;
      internal const int fDeleted = 3;
      internal const int CacheStamp = 4;
    }

    internal class WorkItemTypeUsagesTableColumnIndexes
    {
      internal const int WorkItemTypeUsageID = 0;
      internal const int WorkItemTypeUsageFieldId = 1;
      internal const int WorkItemTypeUsageWorkItemTypeId = 2;
      internal const int fGreyOut = 3;
      internal const int fDeleted = 4;
      internal const int CacheStamp = 5;
    }

    internal class WorkItemTypesTableColumnIndexes
    {
      internal const int WorkItemTypeID = 0;
      internal const int WorkItemTypeNameConstantId = 1;
      internal const int ProjectID = 2;
      internal const int WorkItemTypeDescriptionId = 3;
      internal const int fDeleted = 4;
      internal const int CacheStamp = 5;
    }
  }
}
