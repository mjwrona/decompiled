// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.JsonExtensions
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 74AD14A4-225D-46D2-B154-945941A2D167
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.dll

using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models;
using Microsoft.TeamFoundation.WorkItemTracking.Internals;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking
{
  public static class JsonExtensions
  {
    public static JsObject ToJson(this WorkItemTypeCategory witCategory)
    {
      JsObject json = new JsObject();
      json.Add("name", (object) witCategory.Name);
      json.Add("referenceName", (object) witCategory.ReferenceName);
      json.Add("defaultWorkItemTypeName", (object) witCategory.DefaultWorkItemTypeName);
      json.Add("workItemTypeNames", (object) witCategory.WorkItemTypeNames);
      return json;
    }

    public static JsObject ToJson(this WorkItemModel workItem)
    {
      JsObject json = new JsObject();
      json["fields"] = (object) workItem.LatestData.Where<KeyValuePair<int, object>>((System.Func<KeyValuePair<int, object>, bool>) (pair => pair.Value != null)).ToDictionary<KeyValuePair<int, object>, string, object>((System.Func<KeyValuePair<int, object>, string>) (pair => pair.Key.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo)), (System.Func<KeyValuePair<int, object>, object>) (pair => JsonExtensions.SerializePotentialWitIdentityRef(pair.Value)));
      json["revisions"] = (object) workItem.RevisionData.Select<IReadOnlyDictionary<int, object>, Dictionary<string, object>>((System.Func<IReadOnlyDictionary<int, object>, Dictionary<string, object>>) (dict => dict.ToDictionary<KeyValuePair<int, object>, string, object>((System.Func<KeyValuePair<int, object>, string>) (pair => pair.Key.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo)), (System.Func<KeyValuePair<int, object>, object>) (pair => JsonExtensions.SerializePotentialWitIdentityRef(pair.Value)))));
      json["files"] = (object) workItem.Files;
      json["relations"] = (object) workItem.Relations;
      json["relationRevisions"] = (object) workItem.RelationRevisions;
      json["projectId"] = (object) workItem.ProjectId;
      json["referencedPersons"] = (object) workItem.ReferencedPersons.ToDictionary<KeyValuePair<int, WitIdentityRef>, string, JsObject>((System.Func<KeyValuePair<int, WitIdentityRef>, string>) (pair => pair.Key.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo)), (System.Func<KeyValuePair<int, WitIdentityRef>, JsObject>) (pair => pair.Value.ToJson()));
      json["tags"] = (object) workItem.Tags.Select(tag => new
      {
        tagId = tag.TagId,
        name = tag.Name
      });
      json["currentExtensions"] = (object) workItem.CurrentExtensions;
      json["loadTime"] = (object) workItem.LoadTime;
      json["referencedNodes"] = (object) workItem.ReferencedNodes.ToJson();
      json["isReadOnly"] = (object) workItem.IsReadOnly;
      json["commentVersions"] = (object) workItem.CommentVersions;
      json["isCommentingAvailable"] = (object) workItem.isCommentingAvailable;
      return json;
    }

    public static JsObject ToJson(this WitIdentityRef witIdentityRef)
    {
      JsObject json = new JsObject();
      json["distinctDisplayName"] = (object) witIdentityRef.DistinctDisplayName;
      json["identityRef"] = (object) witIdentityRef.IdentityRef.ToJson();
      return json;
    }

    public static JsObject ToJson(this ReferencedNodes nodes)
    {
      JsObject json = new JsObject();
      IEnumerable<ExtendedTreeNode> areaNodes = nodes.AreaNodes;
      json["areaNodes"] = (object) (areaNodes != null ? areaNodes.Select<ExtendedTreeNode, JsObject>((System.Func<ExtendedTreeNode, JsObject>) (n => n.ToJson())) : (IEnumerable<JsObject>) null);
      IEnumerable<ExtendedTreeNode> iterationNodes = nodes.IterationNodes;
      json["iterationNodes"] = (object) (iterationNodes != null ? iterationNodes.Select<ExtendedTreeNode, JsObject>((System.Func<ExtendedTreeNode, JsObject>) (n => n.ToJson())) : (IEnumerable<JsObject>) null);
      return json;
    }

    public static JsObject ToJson(this ExtendedTreeNode node)
    {
      JsObject json = new JsObject();
      json["id"] = (object) node.Id;
      json["parentId"] = (object) node.ParentId;
      json["name"] = (object) node.Name;
      json["guid"] = (object) node.Guid;
      json["type"] = (object) node.Type;
      json["structure"] = (object) node.StructureType;
      json["startDate"] = (object) node.StartDate;
      json["finishDate"] = (object) node.FinishDate;
      json["path"] = (object) node.Path;
      return json;
    }

    public static JsObject ToJson(
      this GenericDataReader reader,
      bool omitHeaders,
      bool keepIdentityRefs)
    {
      JsObject json = new JsObject();
      int columnCount = reader.FieldCount;
      if (!omitHeaders)
        json["columns"] = (object) Enumerable.Range(0, columnCount).Select<int, string>((System.Func<int, string>) (i => reader.GetName(i))).ToArray<string>();
      json["rows"] = (object) reader.Select<IDataRecord, object[]>((System.Func<IDataRecord, object[]>) (record =>
      {
        object[] values = new object[columnCount];
        record.GetValues(values);
        for (int index = 0; index < columnCount; ++index)
        {
          if (values[index] is DBNull)
            values[index] = (object) null;
          else if (values[index] is IdentityRef)
            values[index] = !keepIdentityRefs ? (object) ((IdentityRef) values[index]).DisplayName : (object) ((IdentityRef) values[index]).ToJson();
        }
        return values;
      })).ToArray<object[]>();
      return json;
    }

    public static JsObject ToJson(
      this WorkItemFieldInvalidException fieldInvalidException)
    {
      JsObject json = PlatformJsonExtensions.ToJson(fieldInvalidException);
      json["fieldReferenceName"] = (object) fieldInvalidException.ReferenceName;
      return json;
    }

    public static JsObject ToJson(
      this RuleValidationException ruleValidationException)
    {
      JsObject json = PlatformJsonExtensions.ToJson(ruleValidationException);
      json["fieldReferenceName"] = (object) ruleValidationException.FieldReferenceName;
      return json;
    }

    private static object SerializePotentialWitIdentityRef(object value) => value is WitIdentityRef witIdentityRef ? (object) witIdentityRef.ToJson() : value;
  }
}
