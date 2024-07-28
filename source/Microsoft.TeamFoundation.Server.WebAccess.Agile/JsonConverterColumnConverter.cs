// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Agile.JsonConverterColumnConverter
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Agile, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 577172B7-1034-4DD0-9CB1-238BFF966AC0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Agile.dll

using Microsoft.TeamFoundation.Server.WebAccess.Agile.Utility;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Server.WebAccess.Agile
{
  internal class JsonConverterColumnConverter : JsonConverter
  {
    private const string NameAttribute = "name";
    private const string WidthAttribute = "width";
    private const string NotAFieldAttribute = "notafield";
    private const string RollupAttribute = "rollup";
    private const string RollupCalculationAttribute = "rollupCalculation";

    public override bool CanConvert(Type objectType) => objectType == typeof (Column);

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
      if (value == null)
      {
        writer.WriteNull();
      }
      else
      {
        Column column = (Column) value;
        new JObject()
        {
          {
            "name",
            (JToken) column.FieldName
          },
          {
            "width",
            (JToken) column.ColumnWidth
          },
          {
            "notafield",
            (JToken) column.NotAField
          },
          {
            "rollup",
            (JToken) column.Rollup
          },
          {
            "rollupCalculation",
            (JToken) column.RollupCalculation?.ToString()
          }
        }.WriteTo(writer);
      }
    }

    public override object ReadJson(
      JsonReader reader,
      Type objectType,
      object existingValue,
      JsonSerializer serializer)
    {
      if (reader.TokenType == JsonToken.Null)
        return (object) null;
      JObject jobject = JObject.Load(reader);
      Column column = new Column()
      {
        FieldName = jobject.Value<string>((object) "name")
      };
      JToken jtoken1;
      if (jobject.TryGetValue("width", out jtoken1))
        column.ColumnWidth = jtoken1.Value<int>();
      JToken jtoken2;
      if (jobject.TryGetValue("notafield", out jtoken2))
        column.NotAField = jtoken2.Value<bool>();
      JToken jtoken3;
      if (jobject.TryGetValue("rollup", out jtoken3))
        column.Rollup = jtoken3.Value<bool>();
      JToken jtoken4;
      if (jobject.TryGetValue("rollupCalculation", out jtoken4))
        column.RollupCalculation = RollupUtils.GetRollupCalculation(jtoken4.ToObject<Dictionary<string, object>>());
      return (object) column;
    }
  }
}
