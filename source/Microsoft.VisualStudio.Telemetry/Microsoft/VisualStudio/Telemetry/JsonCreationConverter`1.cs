// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.JsonCreationConverter`1
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Telemetry.JsonHelpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Microsoft.VisualStudio.Telemetry
{
  public abstract class JsonCreationConverter<T> : JsonConverter
  {
    public override bool CanConvert(Type objectType) => typeof (T).IsAssignableFrom(objectType);

    public override object ReadJson(
      JsonReader reader,
      Type objectType,
      object existingValue,
      JsonSerializer serializer)
    {
      if (reader.TokenType == JsonToken.Null)
        return (object) null;
      JObject jsonObject = JObject.Load(reader);
      T target = this.Create(objectType, jsonObject);
      serializer.Populate(jsonObject.CreateReader(), (object) target);
      return (object) target;
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) => throw new NotImplementedException();

    internal abstract T Create(Type objectType, JObject jsonObject);

    protected bool FieldExists(string fieldName, JObject jsonObject) => jsonObject.FieldExists(fieldName);
  }
}
