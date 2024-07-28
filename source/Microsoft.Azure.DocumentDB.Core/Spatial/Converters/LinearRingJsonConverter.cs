// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Spatial.Converters.LinearRingJsonConverter
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Documents.Spatial.Converters
{
  internal sealed class LinearRingJsonConverter : JsonConverter
  {
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
      LinearRing linearRing = (LinearRing) value;
      serializer.Serialize(writer, (object) linearRing.Positions);
    }

    public override object ReadJson(
      JsonReader reader,
      Type objectType,
      object existingValue,
      JsonSerializer serializer)
    {
      return (object) new LinearRing((IList<Position>) serializer.Deserialize<Position[]>(reader));
    }

    public override bool CanConvert(Type objectType) => CustomTypeExtensions.IsAssignableFrom(typeof (IEnumerable<LinearRing>), objectType);
  }
}
