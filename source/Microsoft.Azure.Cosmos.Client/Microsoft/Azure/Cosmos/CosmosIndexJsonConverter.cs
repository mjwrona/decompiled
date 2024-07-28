// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.CosmosIndexJsonConverter
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Documents;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Globalization;

namespace Microsoft.Azure.Cosmos
{
  internal sealed class CosmosIndexJsonConverter : JsonConverter
  {
    public override bool CanConvert(Type objectType) => typeof (Index).IsAssignableFrom(objectType);

    public override object ReadJson(
      JsonReader reader,
      Type objectType,
      object existingValue,
      JsonSerializer serializer)
    {
      if (objectType != typeof (Index))
        return (object) null;
      JToken jtoken1 = JToken.Load(reader);
      if (jtoken1.Type == JTokenType.Null)
        return (object) null;
      JToken jtoken2 = jtoken1.Type == JTokenType.Object ? jtoken1[(object) "kind"] : throw new JsonSerializationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, RMResources.InvalidIndexSpecFormat));
      if (jtoken2 == null || jtoken2.Type != JTokenType.String)
        throw new JsonSerializationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, RMResources.InvalidIndexSpecFormat));
      IndexKind result = IndexKind.Hash;
      if (!Enum.TryParse<IndexKind>(jtoken2.Value<string>(), out result))
        throw new JsonSerializationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, RMResources.InvalidIndexKindValue, (object) jtoken2.Value<string>()));
      object target;
      switch (result)
      {
        case IndexKind.Hash:
          target = (object) new HashIndex();
          break;
        case IndexKind.Range:
          target = (object) new RangeIndex();
          break;
        case IndexKind.Spatial:
          target = (object) new SpatialIndex();
          break;
        default:
          throw new JsonSerializationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, RMResources.InvalidIndexKindValue, (object) result));
      }
      serializer.Populate(jtoken1.CreateReader(), target);
      return target;
    }

    public override bool CanWrite => false;

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) => throw new NotImplementedException();
  }
}
