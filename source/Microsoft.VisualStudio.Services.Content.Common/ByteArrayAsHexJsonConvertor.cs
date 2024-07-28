// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Common.ByteArrayAsHexJsonConvertor
// Assembly: Microsoft.VisualStudio.Services.Content.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC45E7D4-4445-41B3-8FA2-C13CD848D0F1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Common.dll

using Newtonsoft.Json;
using System;

namespace Microsoft.VisualStudio.Services.Content.Common
{
  public sealed class ByteArrayAsHexJsonConvertor : JsonConverter
  {
    public override bool CanConvert(Type objectType) => objectType == typeof (byte[]);

    public override object ReadJson(
      JsonReader reader,
      Type objectType,
      object existingValue,
      Newtonsoft.Json.JsonSerializer serializer)
    {
      return (object) (reader.Value as string).ToByteArray();
    }

    public override void WriteJson(JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer) => writer.WriteValue(((byte[]) value).ToHexString());
  }
}
