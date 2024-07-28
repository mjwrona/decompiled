// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Query.JTokenAndQueryResultConversionUtils
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Microsoft.Azure.Documents.Query
{
  internal static class JTokenAndQueryResultConversionUtils
  {
    public static JToken GetJTokenFromObject(
      object document,
      out JsonSerializer jsonSerializer,
      out string ownerFullName)
    {
      switch (document)
      {
        case QueryResult queryResult:
          jsonSerializer = queryResult.JsonSerializer;
          ownerFullName = queryResult.OwnerFullName;
          return (JToken) queryResult.Payload;
        case JToken jtokenFromObject:
          jsonSerializer = (JsonSerializer) null;
          ownerFullName = (string) null;
          return jtokenFromObject;
        default:
          jsonSerializer = (JsonSerializer) null;
          ownerFullName = (string) null;
          return document == null ? (JToken) JValue.CreateNull() : JToken.FromObject(document);
      }
    }

    public static object GetObjectFromJToken(
      JToken token,
      JsonSerializer jsonSerializer,
      string ownerFullName)
    {
      if (token is JValue objectFromJtoken)
        return (object) objectFromJtoken;
      return jsonSerializer != null || ownerFullName != null ? (object) new QueryResult((JContainer) token, ownerFullName, jsonSerializer) : (object) token;
    }
  }
}
