// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Serialization.HybridRow.Schemas.PropertySchemaConverter
// Assembly: Microsoft.Azure.Cosmos.Serialization.HybridRow, Version=1.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 77F52C47-A4AE-4843-8DF5-462472B35FB8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Serialization.HybridRow.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Azure.Cosmos.Serialization.HybridRow.Schemas
{
  internal sealed class PropertySchemaConverter : JsonConverter
  {
    public override bool CanWrite => false;

    public override bool CanConvert(Type objectType) => typeof (PropertyType).IsAssignableFrom(objectType);

    public override object ReadJson(
      JsonReader reader,
      Type objectType,
      object existingValue,
      JsonSerializer serializer)
    {
      JObject jobject = reader.TokenType == JsonToken.StartObject ? JObject.Load(reader) : throw new JsonSerializationException();
      JToken jtoken;
      if (!jobject.TryGetValue("type", out jtoken))
        throw new JsonSerializationException("Required \"type\" property missing.");
      TypeKind typeKind;
      using (JsonReader reader1 = jtoken.CreateReader())
      {
        reader1.Read();
        typeKind = (TypeKind) new StringEnumConverter(true).ReadJson(reader1, typeof (TypeKind), (object) null, serializer);
      }
      PropertyType target;
      switch (typeKind)
      {
        case TypeKind.Object:
          target = (PropertyType) new ObjectPropertyType();
          break;
        case TypeKind.Array:
          target = (PropertyType) new ArrayPropertyType();
          break;
        case TypeKind.Set:
          target = (PropertyType) new SetPropertyType();
          break;
        case TypeKind.Map:
          target = (PropertyType) new MapPropertyType();
          break;
        case TypeKind.Tuple:
          target = (PropertyType) new TuplePropertyType();
          break;
        case TypeKind.Tagged:
          target = (PropertyType) new TaggedPropertyType();
          break;
        case TypeKind.Schema:
          target = (PropertyType) new UdtPropertyType();
          break;
        default:
          target = (PropertyType) new PrimitivePropertyType();
          break;
      }
      serializer.Populate(jobject.CreateReader(), (object) target);
      return (object) target;
    }

    [ExcludeFromCodeCoverage]
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
    }
  }
}
