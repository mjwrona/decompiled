// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Common.TypeConverter`1
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FAFB0281-5CF2-4D3F-992C-49FBB9BEC906
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.BlobStore.Common.dll

using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System;

namespace Microsoft.VisualStudio.Services.BlobStore.Common
{
  public class TypeConverter<T> : VssSecureJsonConverter
  {
    public override bool CanConvert(Type objectType) => objectType == typeof (T);

    public override object ReadJson(
      JsonReader reader,
      Type objectType,
      object existingValue,
      JsonSerializer serializer)
    {
      if (reader.TokenType != JsonToken.String)
        throw new JsonReaderException(string.Format("Unexpected JSON token type {0} while reading {1}", (object) reader.TokenType, (object) nameof (T)));
      try
      {
        return (object) (T) Enum.Parse(typeof (T), (string) reader.Value ?? string.Empty, true);
      }
      catch (Exception ex)
      {
        throw new JsonReaderException(string.Format("Failed to parse {0} as {1}", reader.Value, (object) nameof (T)), ex);
      }
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
      base.WriteJson(writer, value, serializer);
      writer.WriteValue(Enum.GetName(typeof (T), value)?.ToLower());
    }
  }
}
