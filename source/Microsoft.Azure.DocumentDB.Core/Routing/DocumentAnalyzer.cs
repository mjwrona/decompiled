// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Routing.DocumentAnalyzer
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Azure.Documents.Routing
{
  internal class DocumentAnalyzer
  {
    public static PartitionKeyInternal ExtractPartitionKeyValue(
      Document document,
      PartitionKeyDefinition partitionKeyDefinition)
    {
      if (partitionKeyDefinition == null || partitionKeyDefinition.Paths.Count == 0)
        return PartitionKeyInternal.Empty;
      return CustomTypeExtensions.IsSubclassOf(document.GetType(), typeof (Document)) ? DocumentAnalyzer.ExtractPartitionKeyValue<Document>(document, partitionKeyDefinition, (Func<Document, JToken>) (doc => JToken.FromObject((object) doc))) : PartitionKeyInternal.FromObjectArray((IEnumerable<object>) partitionKeyDefinition.Paths.Select<string, object>((Func<string, object>) (path => document.GetValueByPath<object>(PathParser.GetPathParts(path), (object) Undefined.Value))).ToArray<object>(), false);
    }

    public static PartitionKeyInternal ExtractPartitionKeyValue(
      string documentString,
      PartitionKeyDefinition partitionKeyDefinition)
    {
      return partitionKeyDefinition == null || partitionKeyDefinition.Paths.Count == 0 ? PartitionKeyInternal.Empty : DocumentAnalyzer.ExtractPartitionKeyValue<string>(documentString, partitionKeyDefinition, (Func<string, JToken>) (docString => JToken.Parse(docString)));
    }

    internal static PartitionKeyInternal ExtractPartitionKeyValue<T>(
      T data,
      PartitionKeyDefinition partitionKeyDefinition,
      Func<T, JToken> convertToJToken)
    {
      return PartitionKeyInternal.FromObjectArray((IEnumerable<object>) partitionKeyDefinition.Paths.Select<string, object>((Func<string, object>) (path =>
      {
        string[] pathParts = PathParser.GetPathParts(path);
        JToken jtoken = convertToJToken(data);
        foreach (string key in pathParts)
        {
          if (jtoken != null)
            jtoken = jtoken[(object) key];
          else
            break;
        }
        return jtoken == null ? (object) Undefined.Value : jtoken.ToObject<object>();
      })).ToArray<object>(), false);
    }
  }
}
