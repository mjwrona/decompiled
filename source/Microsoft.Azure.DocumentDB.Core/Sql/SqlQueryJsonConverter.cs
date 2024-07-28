// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Sql.SqlQueryJsonConverter
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Newtonsoft.Json;
using System;

namespace Microsoft.Azure.Documents.Sql
{
  internal sealed class SqlQueryJsonConverter : JsonConverter
  {
    public override bool CanConvert(Type objectType) => (object) objectType == (object) typeof (SqlQuery);

    public override object ReadJson(
      JsonReader reader,
      Type objectType,
      object existingValue,
      JsonSerializer serializer)
    {
      throw new NotImplementedException();
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
      SqlQuery sqlQuery = (SqlQuery) value;
      writer.WriteValue(sqlQuery.ToString());
    }
  }
}
