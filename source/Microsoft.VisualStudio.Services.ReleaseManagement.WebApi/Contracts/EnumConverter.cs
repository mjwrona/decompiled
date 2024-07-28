// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.EnumConverter
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AE7F604E-30D7-44A7-BE7B-AB7FB5A67B31
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Linq;
using System.Reflection;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts
{
  public class EnumConverter : StringEnumConverter
  {
    public override bool CanConvert(Type objectType) => typeof (Enum).GetTypeInfo().IsAssignableFrom(objectType.GetTypeInfo());

    public override object ReadJson(
      JsonReader reader,
      Type objectType,
      object existingValue,
      JsonSerializer serializer)
    {
      if (reader == null)
        throw new ArgumentNullException(nameof (reader));
      if (reader.TokenType == JsonToken.String || reader.TokenType == JsonToken.Integer)
      {
        try
        {
          existingValue = Enum.Parse(objectType, reader.Value.ToString());
        }
        catch (ArgumentException ex)
        {
          existingValue = EnumConverter.GetDefaultValueForType(objectType);
        }
      }
      return existingValue;
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
      if (writer == null)
        throw new ArgumentNullException(nameof (writer));
      if (serializer.Converters != null && serializer.Converters.Any<JsonConverter>((Func<JsonConverter, bool>) (c => c is StringEnumConverter)))
      {
        base.WriteJson(writer, value, serializer);
      }
      else
      {
        if (value == null)
          return;
        writer.WriteValue(value);
      }
    }

    private static object GetDefaultValueForType(Type objectType) => Activator.CreateInstance(objectType);
  }
}
