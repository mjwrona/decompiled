// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Common.DictionaryValueInt32Converter
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Common
{
  public class DictionaryValueInt32Converter : JsonConverter
  {
    public override bool CanConvert(Type objectType) => objectType == typeof (Dictionary<string, object>);

    public override object ReadJson(
      JsonReader reader,
      Type objectType,
      object existingValue,
      JsonSerializer serializer)
    {
      Dictionary<string, object> dictionary = new Dictionary<string, object>();
      string empty = string.Empty;
      reader.Read();
      while (reader.TokenType == JsonToken.PropertyName)
      {
        string key = reader.Value as string;
        if (!string.IsNullOrEmpty(key) && reader.Read())
        {
          object objectValue = this.ParseObjectValue(reader, serializer);
          dictionary.Add(key, objectValue);
        }
        reader.Read();
      }
      return (object) dictionary;
    }

    public override bool CanWrite => false;

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) => throw new NotImplementedException();

    private object ParseObjectValue(JsonReader reader, JsonSerializer serializer) => reader.TokenType == JsonToken.Integer ? (object) Convert.ToInt32(reader.Value) : serializer.Deserialize(reader);
  }
}
