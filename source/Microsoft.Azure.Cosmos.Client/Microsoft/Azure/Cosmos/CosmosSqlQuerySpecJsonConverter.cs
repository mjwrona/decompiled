// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.CosmosSqlQuerySpecJsonConverter
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Query.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.Azure.Cosmos
{
  internal sealed class CosmosSqlQuerySpecJsonConverter : JsonConverter
  {
    private readonly CosmosSerializer UserSerializer;

    internal CosmosSqlQuerySpecJsonConverter(CosmosSerializer userSerializer) => this.UserSerializer = userSerializer ?? throw new ArgumentNullException(nameof (userSerializer));

    public override bool CanConvert(Type objectType) => typeof (SqlParameter) == objectType;

    public override object ReadJson(
      JsonReader reader,
      Type objectType,
      object existingValue,
      JsonSerializer serializer)
    {
      throw new NotImplementedException();
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
      SqlParameter sqlParameter = (SqlParameter) value;
      writer.WriteStartObject();
      writer.WritePropertyName("name");
      serializer.Serialize(writer, (object) sqlParameter.Name);
      writer.WritePropertyName(nameof (value));
      if (sqlParameter.Value is SerializedParameterValue serializedParameterValue)
      {
        writer.WriteRawValue(serializedParameterValue.rawSerializedJsonValue);
      }
      else
      {
        using (Stream stream = this.UserSerializer.ToStream<object>(sqlParameter.Value))
        {
          using (StreamReader streamReader = new StreamReader(stream))
          {
            string end = streamReader.ReadToEnd();
            writer.WriteRawValue(end);
          }
        }
      }
      writer.WriteEndObject();
    }

    internal static CosmosSerializer CreateSqlQuerySpecSerializer(
      CosmosSerializer cosmosSerializer,
      CosmosSerializer propertiesSerializer)
    {
      if (propertiesSerializer is CosmosJsonSerializerWrapper serializerWrapper)
        propertiesSerializer = serializerWrapper.InternalJsonSerializer;
      return (CosmosSerializer) new CosmosJsonSerializerWrapper((CosmosSerializer) new CosmosJsonDotNetSerializer(new JsonSerializerSettings()
      {
        Converters = (IList<JsonConverter>) new List<JsonConverter>()
        {
          (JsonConverter) new CosmosSqlQuerySpecJsonConverter(cosmosSerializer ?? propertiesSerializer)
        },
        MaxDepth = new int?(64)
      }));
    }
  }
}
