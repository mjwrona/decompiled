// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.JsonHelpers.CustomJsonConverter
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Telemetry.JsonHelpers
{
  internal sealed class CustomJsonConverter : JsonConverter
  {
    private readonly Dictionary<Type, Func<object, string>> typeConverters = new Dictionary<Type, Func<object, string>>();
    private readonly HashSet<Type> usedConverter = new HashSet<Type>();

    public void AddConverter(Type type, Func<object, string> converter) => this.typeConverters[type] = converter;

    public override bool CanConvert(Type objectType) => this.typeConverters.ContainsKey(objectType);

    public override object ReadJson(
      JsonReader reader,
      Type objectType,
      object existingValue,
      JsonSerializer serializer)
    {
      throw new NotImplementedException("Unnecessary because CanRead is false. The type will skip the converter.");
    }

    public override bool CanRead => false;

    public void ResetUsageInformation() => this.usedConverter.Clear();

    public bool WasConverterUsed(Type typeOfConverter) => this.usedConverter.Contains(typeOfConverter);

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
      if (value == null)
        return;
      serializer.Serialize(writer, (object) this.typeConverters[value.GetType()](value));
      this.usedConverter.Add(value.GetType());
    }
  }
}
