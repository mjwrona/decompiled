// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Agile.JsonExtensions
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Agile, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 577172B7-1034-4DD0-9CB1-238BFF966AC0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Agile.dll

using Microsoft.TeamFoundation.Agile.Web.Utilities;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.Agile.Models;
using Microsoft.TeamFoundation.Server.WebAccess.Agile.ViewModels;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.Agile
{
  internal static class JsonExtensions
  {
    public static JsObject ToJson(this BoardCardSettings cardSettings)
    {
      ArgumentUtility.CheckForNull<BoardCardSettings>(cardSettings, nameof (cardSettings));
      JsObject jsObject1 = new JsObject();
      jsObject1.Add("scope", (object) cardSettings.Scope.ToString());
      jsObject1.Add("scopeId", (object) cardSettings.ScopeId);
      JsObject json = jsObject1;
      if (cardSettings.Cards != null && cardSettings.Cards.Count > 0)
      {
        JsObject jsObject2 = new JsObject();
        foreach (KeyValuePair<string, List<FieldSetting>> keyValuePair in (IEnumerable<KeyValuePair<string, List<FieldSetting>>>) cardSettings.Cards.OrderBy<KeyValuePair<string, List<FieldSetting>>, string>((Func<KeyValuePair<string, List<FieldSetting>>, string>) (c => c.Key)))
          jsObject2[keyValuePair.Key] = (object) keyValuePair.Value.Select<FieldSetting, JsObject>((Func<FieldSetting, JsObject>) (item => item.ToJson())).ToArray<JsObject>();
        json["cards"] = (object) jsObject2;
      }
      if (cardSettings.Rules != null)
      {
        JsObject[] array = cardSettings.Rules.Select<CardRule, JsObject>((Func<CardRule, JsObject>) (style => style.ToJson())).ToArray<JsObject>();
        json["styles"] = (object) array;
      }
      if (cardSettings.TestSettings != null)
        json["testSettings"] = (object) cardSettings.TestSettings;
      return json;
    }

    public static JsObject ToJson(this BoardFilterSettings boardFilterSettings)
    {
      ArgumentUtility.CheckForNull<BoardFilterSettings>(boardFilterSettings, nameof (boardFilterSettings));
      JsObject jsObject = new JsObject();
      jsObject.Add("boardId", (object) boardFilterSettings.BoardId);
      JsObject json = jsObject;
      if (boardFilterSettings.InitialFilter != null)
        json["initialFilter"] = (object) boardFilterSettings.InitialFilter.ToJson();
      return json;
    }

    public static JsObject ToJson(this Dictionary<string, WorkItemFilter> filter)
    {
      ArgumentUtility.CheckForNull<Dictionary<string, WorkItemFilter>>(filter, "boardFilterSettings");
      JsObject json = new JsObject();
      foreach (KeyValuePair<string, WorkItemFilter> keyValuePair in filter)
      {
        JsObject enumerable = new JsObject();
        if (keyValuePair.Value.Options != null)
          enumerable["options"] = (object) keyValuePair.Value.Options;
        if (keyValuePair.Value.Values != null)
          enumerable["values"] = (object) keyValuePair.Value.Values;
        if (!enumerable.IsNullOrEmpty<KeyValuePair<string, object>>())
          json[keyValuePair.Key] = (object) enumerable;
      }
      return json;
    }

    public static JsObject ToJson(this ParentChildWIMap parentChildMap)
    {
      ArgumentUtility.CheckForNull<ParentChildWIMap>(parentChildMap, "parentFieldFilterModel");
      JsObject json = new JsObject();
      json.Add("id", (object) parentChildMap.Id);
      json.Add("title", (object) parentChildMap.Title);
      json.Add("childWorkItemIds", (object) parentChildMap.ChildWorkItemIds.ToArray<int>());
      return json;
    }

    public static JsObject[] ToJson(
      this IEnumerable<ParentChildWIMap> parentChildWIMaps)
    {
      ArgumentUtility.CheckForNull<IEnumerable<ParentChildWIMap>>(parentChildWIMaps, "parentFieldFilterModels");
      return parentChildWIMaps.Select<ParentChildWIMap, JsObject>((Func<ParentChildWIMap, JsObject>) (member => member.ToJson())).ToArray<JsObject>();
    }

    public static JsObject ToJson(
      this BoardFilterSettingsModel boardFilterSettingsModel,
      bool includeParentWIIds)
    {
      JsObject jsObject = new JsObject();
      JsObject json = new JsObject();
      if (boardFilterSettingsModel.QueryText != null)
        json["queryText"] = (object) boardFilterSettingsModel.QueryText;
      if (boardFilterSettingsModel.QueryExpression != null)
        json["criteria"] = (object) boardFilterSettingsModel.QueryExpression.ToJson();
      if (includeParentWIIds && boardFilterSettingsModel.ParentWorkItemIds != null)
        json["parentWIIds"] = (object) boardFilterSettingsModel.ParentWorkItemIds.ToArray<int>();
      return json;
    }

    public static JsObject[] ToJson(this CardSetting cardSettings)
    {
      ArgumentUtility.CheckForNull<CardSetting>(cardSettings, nameof (cardSettings));
      JsObject[] json = (JsObject[]) null;
      if (cardSettings != null && cardSettings.Count > 0)
        json = cardSettings.Select<FieldSetting, JsObject>((Func<FieldSetting, JsObject>) (item => item.ToJson())).ToArray<JsObject>();
      return json;
    }

    public static JsObject ToJson(this FieldSetting fieldSetting)
    {
      ArgumentUtility.CheckForNull<FieldSetting>(fieldSetting, nameof (fieldSetting));
      JsObject json = new JsObject();
      if (fieldSetting != null && fieldSetting.Count > 0)
      {
        foreach (KeyValuePair<string, string> keyValuePair in (Dictionary<string, string>) fieldSetting)
        {
          if (!keyValuePair.Value.IsNullOrEmpty<char>())
            json[keyValuePair.Key] = (object) keyValuePair.Value;
        }
      }
      return json;
    }

    public static JsObject ToJson(
      this BoardSettings board,
      IVssRequestContext requestContext,
      bool isBoardSettingsValid)
    {
      ArgumentUtility.CheckForNull<BoardSettings>(board, nameof (board));
      requestContext.GetService<ITeamFoundationFeatureAvailabilityService>();
      bool flag = requestContext.GetService<TeamFoundationResourceManagementService>().GetServiceVersion(requestContext, "BoardSettings", "Default").Version >= 13;
      JsObject json = new JsObject();
      json.Add("id", (object) board.Id);
      json.Add("extensionId", (object) board.ExtensionId);
      json.Add("boardFields", (object) board.BoardFields);
      json.Add("teamId", (object) board.TeamId);
      json.Add("backlogLevelId", (object) board.BacklogLevelId);
      json.Add("columns", (object) board.Columns.Select<BoardColumn, JsObject>((Func<BoardColumn, JsObject>) (c => c.ToJson())).ToList<JsObject>());
      json.Add("rows", (object) board.Rows.Select<BoardRow, JsObject>((Func<BoardRow, JsObject>) (r => r.ToJson())).ToList<JsObject>());
      json.Add("isValid", (object) isBoardSettingsValid);
      json.Add("isBoardValid", (object) isBoardSettingsValid);
      json.Add("allowedMappings", (object) board.AllowedMappings);
      json.Add("canEdit", (object) board.CanEdit);
      json.Add("cardReorderingFeatureEnabled", (object) flag);
      json.Add("statusBadgeIsPublic", (object) board.StatusBadgeIsPublic);
      json.Add("preserveBacklogOrder", (object) (bool) (!flag ? 0 : (board.PreserveBacklogOrder ? 1 : 0)));
      json.Add("autoRefreshState", (object) board.AutoRefreshState);
      json.Add("sortableFieldsByColumnType", (object) board.SortableFieldsByColumnType);
      return json;
    }

    public static JsObject ToJson(this BoardColumn column)
    {
      JsObject json = new JsObject();
      json.Add("id", (object) column.Id);
      json.Add("name", (object) column.Name);
      json.Add("order", (object) column.Order);
      json.Add("columnType", (object) (int) column.ColumnType);
      json.Add("itemLimit", (object) column.ItemLimit);
      json.Add("stateMappings", (object) column.StateMappings);
      json.Add("isDeleted", (object) column.IsDeleted);
      json.Add("isSplit", (object) column.IsSplit);
      json.Add("description", (object) column.Description);
      return json;
    }

    public static JsObject ToJson(this CardRule styleRule)
    {
      JsObject obj = new JsObject();
      JsObject jsObject = new JsObject();
      jsObject.Add("name", (object) styleRule.Name);
      jsObject.Add("type", (object) styleRule.Type);
      jsObject.Add("isEnabled", (object) styleRule.IsEnabled);
      JsObject json = jsObject;
      if (styleRule.StyleAttributes != null)
      {
        styleRule.StyleAttributes.ForEach((Action<KeyValuePair<string, string>>) (kvp => obj[kvp.Key] = (object) kvp.Value));
        json["styles"] = (object) obj;
        json["style"] = (object) obj;
      }
      if (styleRule.QueryExpression != null)
        json["criteria"] = (object) styleRule.QueryExpression.ToJson();
      return json;
    }

    public static JsObject ToJson(this CardStyle cardStyle)
    {
      JsObject json = new JsObject();
      json.Add("styleType", (object) cardStyle.StyleType);
      json.Add("properties", (object) cardStyle.Properties.Select<KeyValuePair<string, string>, JsObject>((Func<KeyValuePair<string, string>, JsObject>) (item =>
      {
        return new JsObject()
        {
          [item.Key] = (object) item.Value
        };
      })).ToArray<JsObject>());
      return json;
    }

    public static JsObject ToJson(this FilterModel filterModel)
    {
      JsObject json = new JsObject();
      json.Add("clauses", (object) filterModel.Clauses.Select<FilterClause, JsObject>((Func<FilterClause, JsObject>) (clause => clause.ToJson())).ToArray<JsObject>());
      json.Add("groups", (object) filterModel.Groups.Select<FilterGroup, JsObject>((Func<FilterGroup, JsObject>) (group => group.ToJson())).ToArray<JsObject>());
      json.Add("maxGroupLevel", (object) filterModel.MaxGroupLevel);
      return json;
    }

    public static JsObject ToJson(this FilterClause filterClause)
    {
      JsObject json = new JsObject();
      json.Add("logicalOperator", (object) filterClause.LogicalOperator);
      json.Add("fieldName", (object) filterClause.FieldName);
      json.Add("operator", (object) filterClause.Operator);
      json.Add("value", (object) filterClause.Value);
      json.Add("index", (object) filterClause.Index);
      return json;
    }

    public static JsObject ToJson(this FilterGroup filterGroup)
    {
      JsObject json = new JsObject();
      json.Add("start", (object) filterGroup.Start);
      json.Add("end", (object) filterGroup.End);
      json.Add("level", (object) filterGroup.Level);
      return json;
    }

    public static JsObject ToJson(this BoardRow row)
    {
      JsObject json = new JsObject();
      json.Add("id", (object) row.Id);
      json.Add("name", (object) row.Name);
      json.Add("isDefault", (object) row.IsDefault);
      json.Add("color", (object) row.Color);
      return json;
    }

    public static JsObject ToJson(this IBoard board, IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IBoard>(board, nameof (board));
      JsObject jsObject = new JsObject();
      jsObject.Add("id", (object) board.Id);
      jsObject.Add("fields", (object) board.FieldTypes);
      jsObject.Add("membership", (object) board.Membership.ToJson());
      jsObject.Add("filterableFieldNamesByItemType", (object) board.FilterableFieldNamesByItemType);
      JsObject json = jsObject;
      if (board.Node != null)
        json["node"] = (object) board.Node.ToJson();
      json["pageSize"] = (object) board.PageSize;
      json["filterPageSize"] = (object) board.FilterPageSize;
      return json;
    }

    public static JsObject ToJson(this IBoardNode node)
    {
      ArgumentUtility.CheckForNull<IBoardNode>(node, nameof (node));
      JsObject jsObject = new JsObject();
      jsObject.Add("fieldName", (object) node.FieldName);
      jsObject.Add("layoutStyle", (object) node.LayoutStyle);
      jsObject.Add("members", (object) node.Members.ToJson());
      JsObject json = jsObject;
      if (node.IsItemDriven)
        json["isItemDriven"] = (object) node.IsItemDriven;
      return json;
    }

    public static JsObject ToJson(this IBoardMember member)
    {
      ArgumentUtility.CheckForNull<IBoardMember>(member, nameof (member));
      JsObject jsObject = new JsObject();
      jsObject.Add("id", (object) member.Id);
      jsObject.Add("title", (object) member.Title);
      jsObject.Add("values", (object) member.Values);
      jsObject.Add("canCreateNewItems", (object) member.CanCreateNewItems);
      JsObject json = jsObject;
      if (member.Items != null && member.Items.Any<IItem>())
        json["itemIds"] = (object) member.Items.Select<IItem, int>((Func<IItem, int>) (item => item.GetField<int>("System.Id")));
      if (member.ChildNode != null)
        json["childNode"] = (object) member.ChildNode.ToJson();
      if (member.LayoutOptions != null)
        json["layoutOptions"] = (object) member.LayoutOptions.ToJson();
      if (member.ItemOrdering != null)
        json["itemOrdering"] = (object) member.ItemOrdering.ToJson();
      if (member.SortValue != null)
        json["sortValue"] = (object) member.SortValue.ToJson();
      if (member.Limits != null)
        json["limits"] = (object) member.Limits.ToJson();
      if (member.Metadata != null)
        json["metadata"] = (object) member.Metadata;
      if (member.Description != null)
        json["description"] = (object) member.Description;
      json["handlesNull"] = (object) member.HandlesNull;
      return json;
    }

    public static JsObject[] ToJson(this IEnumerable<IBoardMember> members)
    {
      ArgumentUtility.CheckForNull<IEnumerable<IBoardMember>>(members, nameof (members));
      return members.Select<IBoardMember, JsObject>((Func<IBoardMember, JsObject>) (member => member.ToJson())).ToArray<JsObject>();
    }

    public static JsObject ToJson(this LayoutOptions layoutOptions)
    {
      ArgumentUtility.CheckForNull<LayoutOptions>(layoutOptions, "layout");
      JsObject json = new JsObject();
      json.Add("cssClass", (object) layoutOptions.CssClass);
      return json;
    }

    public static JsObject ToJson(this MemberLimit limit)
    {
      ArgumentUtility.CheckForNull<MemberLimit>(limit, nameof (limit));
      JsObject json = new JsObject();
      json.Add(nameof (limit), (object) limit.Limit);
      return json;
    }

    public static JsObject ToJson(this FunctionReference function)
    {
      ArgumentUtility.CheckForNull<FunctionReference>(function, nameof (function));
      JsObject jsObject1 = new JsObject();
      jsObject1.Add("id", (object) function.FunctionId.ToString());
      JsObject json = jsObject1;
      if (function.FunctionData != null)
      {
        JsObject jsObject2 = new JsObject();
        if (function.FunctionData is FieldOrderData)
          jsObject2["fields"] = (object) new JsObject[1]
          {
            ((FieldOrderData) function.FunctionData).ToJson()
          };
        else if (function.FunctionData is IEnumerable<FieldOrderData>)
          jsObject2["fields"] = (object) ((IEnumerable<FieldOrderData>) function.FunctionData).Select<FieldOrderData, JsObject>((Func<FieldOrderData, JsObject>) (f => f.ToJson())).ToArray<JsObject>();
        else if (function.FunctionData is string[])
          jsObject2["fields"] = function.FunctionData;
        else
          jsObject2["fields"] = function.FunctionData;
        json["data"] = (object) jsObject2;
      }
      return json;
    }

    public static JsObject ToJson(this FieldOrderData data)
    {
      ArgumentUtility.CheckForNull<FieldOrderData>(data, nameof (data));
      JsObject json = new JsObject();
      json.Add("fieldName", (object) data.FieldName);
      json.Add("order", (object) data.FieldOrder);
      return json;
    }

    public static JsObject ToJson(
      this WorkItemSource itemSource,
      bool includeParentPayload,
      BoardSettings boardSettings,
      bool keepIdentityRefs)
    {
      JsObject json = new JsObject();
      try
      {
        json.Add("type", (object) itemSource.Type);
        json.Add("transitions", (object) itemSource.Transitions);
        json.Add("itemTypes", (object) itemSource.GetItemTypes());
        json.Add("fieldDefinitions", (object) itemSource.GetCardFieldDefinitions().Select<ICardFieldDefinition, JsObject>((Func<ICardFieldDefinition, JsObject>) (f => f.ToJson())).ToArray<JsObject>());
        json.Add("payload", (object) itemSource.GetPayload(boardSettings).ToJson(false, keepIdentityRefs));
        if (includeParentPayload)
        {
          ICollection<ParentChildWIMap> parentPayload = itemSource.GetParentPayload(boardSettings);
          if (!parentPayload.IsNullOrEmpty<ParentChildWIMap>())
            json.Add("parentPayload", (object) parentPayload.ToJson());
        }
        if (itemSource.HasReachedInProgressDisplayLimit)
        {
          string str = string.Format(AgileControlsServerResources.Board_InProgressLimitReached, (object) itemSource.ItemDisplayLimit);
          json.Add("warningMessage", (object) str);
        }
      }
      catch (WorkItemTrackingQueryResultSizeLimitExceededException ex)
      {
        json.Add("error", (object) "true");
        json.Add("warningMessage", (object) string.Format(AgileControlsServerResources.Board_QueryLimitExceeded, (object) ex.Limit));
      }
      return json;
    }

    public static JsObject ToJson(
      this HierarchyDataReader hierarchyReader,
      bool omitHeaders,
      bool keepIdentityRefs)
    {
      JsObject json = Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.JsonExtensions.ToJson(hierarchyReader, omitHeaders, keepIdentityRefs);
      if (hierarchyReader.Hierarchy != null)
        json["hierarchy"] = (object) hierarchyReader.Hierarchy.Aggregate<Tuple<int, int>, JsObject>(new JsObject(), (Func<JsObject, Tuple<int, int>, JsObject>) ((jsh, t) =>
        {
          jsh[t.Item1.ToString()] = (object) t.Item2;
          return jsh;
        }));
      json["orderedIncomingIds"] = (object) hierarchyReader.OrderedIncomingIds;
      json["orderedOutgoingIds"] = (object) hierarchyReader.OrderedOutgoingIds;
      return json;
    }

    public static JsObject ToJson(this EffortDataViewModel effortData)
    {
      JsObject json = new JsObject();
      json.Add("ids", (object) effortData.Ids);
      json.Add("efforts", (object) effortData.Efforts);
      json.Add("effortFieldName", (object) effortData.EffortFieldName);
      return json;
    }

    public static JsObject ToJson(this IItemSource itemSource)
    {
      JsObject json = new JsObject();
      json.Add("type", (object) itemSource.Type);
      return json;
    }
  }
}
