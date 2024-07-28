// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.WebApi.UnknownEnumJsonConverter
// Assembly: Microsoft.Azure.Pipelines.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9955A178-37CB-46CB-B455-32EA2A66C5BA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.WebApi.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Azure.Pipelines.WebApi
{
  public class UnknownEnumJsonConverter : StringEnumConverter
  {
    private const string UnknownName = "Unknown";

    public UnknownEnumJsonConverter() => this.CamelCaseText = true;

    public override bool CanConvert(Type objectType) => objectType.IsEnum && ((IEnumerable<string>) Enum.GetNames(objectType)).Any<string>((Func<string, bool>) (name => string.Equals(name, "Unknown", StringComparison.OrdinalIgnoreCase)));

    public override object ReadJson(
      JsonReader reader,
      Type objectType,
      object existingValue,
      JsonSerializer serializer)
    {
      if (!this.CanConvert(objectType))
        return base.ReadJson(reader, objectType, existingValue, serializer);
      if (reader.TokenType == JsonToken.Integer)
      {
        int int32 = Convert.ToInt32(reader.Value);
        if (((IEnumerable<int>) Enum.GetValues(objectType)).Contains<int>(int32))
          return Enum.Parse(objectType, int32.ToString());
      }
      if (reader.TokenType != JsonToken.String)
        return Enum.Parse(objectType, "Unknown");
      string stringValue = reader.Value.ToString();
      return UnknownEnum.Parse(objectType, stringValue);
    }
  }
}
