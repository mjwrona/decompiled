// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.QueryItemModelBinder
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 74AD14A4-225D-46D2-B154-945941A2D167
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.dll

using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking
{
  public class QueryItemModelBinder : JsonModelBinder
  {
    public override JsonConverter[] GetConverters() => (JsonConverter[]) new JsonConverterWithCallbacks<QueryItem>[1]
    {
      new JsonConverterWithCallbacks<QueryItem>(QueryItemModelBinder.\u003C\u003EO.\u003C0\u003E__DeserializeJsonQueryItem ?? (QueryItemModelBinder.\u003C\u003EO.\u003C0\u003E__DeserializeJsonQueryItem = new JsonDeserializationCallback<QueryItem>(QueryItemModelBinder.DeserializeJsonQueryItem)))
    };

    public static QueryItem DeserializeJsonQueryItem(IDictionary<string, object> dictionary)
    {
      QueryItem queryItem = new QueryItem();
      if (dictionary != null)
      {
        QueryItemModelBinder.ReadGuid(dictionary, "id", out queryItem.Id);
        QueryItemModelBinder.ReadGuid(dictionary, "parentId", out queryItem.ParentId);
        QueryItemModelBinder.ReadString(dictionary, "name", out queryItem.Name);
        QueryItemModelBinder.ReadString(dictionary, "queryText", out queryItem.QueryText);
      }
      return queryItem;
    }

    private static void ReadString(
      IDictionary<string, object> dictionary,
      string key,
      out string stringValue)
    {
      stringValue = (string) null;
      object obj;
      if (!dictionary.TryGetValue(key, out obj) || obj == null)
        return;
      stringValue = obj.ToString();
      if (stringValue == null)
        throw new InvalidOperationException("Invalid data type for the field '" + key + "'.");
    }

    private static void ReadGuid(
      IDictionary<string, object> dictionary,
      string key,
      out Guid guidValue)
    {
      guidValue = Guid.Empty;
      object obj;
      if (!dictionary.TryGetValue(key, out obj) || obj == null)
        return;
      string input = obj.ToString();
      if (input == null)
        throw new InvalidOperationException("Invalid data type for the field '" + key + "'.");
      if (!string.IsNullOrEmpty(input) && !Guid.TryParse(input, out guidValue))
        throw new InvalidOperationException("Invalid guid format for the field '" + key + "'.");
    }

    public override JavaScriptConverter[] GetJsConverters() => (JavaScriptConverter[]) new JsonConverterJsSerializerWithCallbacks<QueryItem>[1]
    {
      new JsonConverterJsSerializerWithCallbacks<QueryItem>(QueryItemModelBinder.\u003C\u003EO.\u003C1\u003E__DeserializeJsonQueryItemJsSerializer ?? (QueryItemModelBinder.\u003C\u003EO.\u003C1\u003E__DeserializeJsonQueryItemJsSerializer = new JsonDeserializationJSCallback<QueryItem>(QueryItemModelBinder.DeserializeJsonQueryItemJsSerializer)))
    };

    public static QueryItem DeserializeJsonQueryItemJsSerializer(
      IDictionary<string, object> dictionary,
      JavaScriptSerializer serializer)
    {
      QueryItem queryItem = new QueryItem();
      if (dictionary != null)
      {
        QueryItemModelBinder.ReadGuid(dictionary, "id", out queryItem.Id);
        QueryItemModelBinder.ReadGuid(dictionary, "parentId", out queryItem.ParentId);
        QueryItemModelBinder.ReadString(dictionary, "name", out queryItem.Name);
        QueryItemModelBinder.ReadString(dictionary, "queryText", out queryItem.QueryText);
      }
      return queryItem;
    }
  }
}
