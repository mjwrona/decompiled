// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Common.ByteArrayAsNumberArrayJsonConvertor
// Assembly: Microsoft.VisualStudio.Services.Content.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC45E7D4-4445-41B3-8FA2-C13CD848D0F1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Common.dll

using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Content.Common
{
  public sealed class ByteArrayAsNumberArrayJsonConvertor : VssSecureJsonConverter
  {
    public override bool CanConvert(Type objectType) => objectType == typeof (byte[]);

    public override object ReadJson(
      JsonReader reader,
      Type objectType,
      object existingValue,
      Newtonsoft.Json.JsonSerializer serializer)
    {
      return (object) ((IEnumerable<int>) serializer.Deserialize<int[]>(reader)).Select<int, byte>((Func<int, byte>) (x => (byte) x)).ToArray<byte>();
    }

    public override void WriteJson(JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer)
    {
      base.WriteJson(writer, value, serializer);
      serializer.Serialize(writer, (object) ((IEnumerable<byte>) (byte[]) value).Select<byte, int>((Func<byte, int>) (x => (int) x)).ToArray<int>(), typeof (int[]));
    }
  }
}
