// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.CosmosElements.CosmosElementJsonConverter
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Json;
using Microsoft.Azure.Cosmos.Json.Interop;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


#nullable enable
namespace Microsoft.Azure.Cosmos.CosmosElements
{
  internal sealed class CosmosElementJsonConverter : JsonConverter
  {
    private static readonly HashSet<Type> NumberTypes = new HashSet<Type>()
    {
      typeof (double),
      typeof (float),
      typeof (long),
      typeof (int),
      typeof (short),
      typeof (byte),
      typeof (uint),
      typeof (CosmosNumber)
    };
    private static readonly HashSet<Type> StringTypes = new HashSet<Type>()
    {
      typeof (string),
      typeof (CosmosString)
    };
    private static readonly HashSet<Type> NullTypes = new HashSet<Type>()
    {
      typeof (object),
      typeof (CosmosNull)
    };
    private static readonly HashSet<Type> ArrayTypes = new HashSet<Type>()
    {
      typeof (object[]),
      typeof (CosmosArray)
    };
    private static readonly HashSet<Type> ObjectTypes = new HashSet<Type>()
    {
      typeof (Dictionary<string, object>),
      typeof (CosmosObject)
    };
    private static readonly HashSet<Type> BooleanTypes = new HashSet<Type>()
    {
      typeof (bool),
      typeof (CosmosBoolean)
    };
    private static readonly HashSet<Type> ConvertableTypes = new HashSet<Type>(CosmosElementJsonConverter.NumberTypes.Concat<Type>((IEnumerable<Type>) CosmosElementJsonConverter.StringTypes).Concat<Type>((IEnumerable<Type>) CosmosElementJsonConverter.NullTypes).Concat<Type>((IEnumerable<Type>) CosmosElementJsonConverter.ArrayTypes).Concat<Type>((IEnumerable<Type>) CosmosElementJsonConverter.ObjectTypes).Concat<Type>((IEnumerable<Type>) CosmosElementJsonConverter.BooleanTypes));

    public override bool CanConvert(Type objectType) => CosmosElementJsonConverter.ConvertableTypes.Contains(objectType) || CosmosElementJsonConverter.ConvertableTypes.Contains(objectType.BaseType) || objectType == typeof (CosmosElement);

    public override object ReadJson(
      Newtonsoft.Json.JsonReader reader,
      Type objectType,
      object existingValue,
      Newtonsoft.Json.JsonSerializer serializer)
    {
      return (object) CosmosElement.CreateFromBuffer((ReadOnlyMemory<byte>) Encoding.UTF8.GetBytes(JsonConvert.SerializeObject((object) JToken.Load(reader))));
    }

    public override void WriteJson(Newtonsoft.Json.JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer)
    {
      NewtonsoftToCosmosDBWriter fromWriter = NewtonsoftToCosmosDBWriter.CreateFromWriter(writer);
      (value as CosmosElement ?? throw new InvalidCastException("Failed to cast to CosmosElement.")).WriteTo((IJsonWriter) fromWriter);
    }
  }
}
