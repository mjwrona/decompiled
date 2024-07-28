// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.PatchOperationsJsonConverter
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.Azure.Cosmos
{
  internal sealed class PatchOperationsJsonConverter : JsonConverter
  {
    private readonly CosmosSerializer userSerializer;

    public PatchOperationsJsonConverter(CosmosSerializer userSerializer) => this.userSerializer = userSerializer ?? throw new ArgumentNullException(nameof (userSerializer));

    public override bool CanConvert(Type objectType) => true;

    public override bool CanRead => false;

    public override object ReadJson(
      JsonReader reader,
      Type objectType,
      object existingValue,
      JsonSerializer serializer)
    {
      throw new NotImplementedException("Unnecessary because CanRead is false. The type will skip the converter.");
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
      IReadOnlyList<PatchOperation> patchOperationList = value is PatchSpec patchSpec ? patchSpec.PatchOperations : throw new ArgumentOutOfRangeException("nameof(value) should be of type PatchSpec.");
      writer.WriteStartObject();
      patchSpec.RequestOptions.Match((Action<PatchItemRequestOptions>) (patchRequestOptions =>
      {
        if (patchRequestOptions == null || string.IsNullOrWhiteSpace(patchRequestOptions.FilterPredicate))
          return;
        writer.WritePropertyName("condition");
        writer.WriteValue(patchRequestOptions.FilterPredicate);
      }), (Action<TransactionalBatchPatchItemRequestOptions>) (transactionalBatchPatchRequestOptions =>
      {
        if (transactionalBatchPatchRequestOptions == null || string.IsNullOrWhiteSpace(transactionalBatchPatchRequestOptions.FilterPredicate))
          return;
        writer.WritePropertyName("condition");
        writer.WriteValue(transactionalBatchPatchRequestOptions.FilterPredicate);
      }));
      writer.WritePropertyName("operations");
      writer.WriteStartArray();
      foreach (PatchOperation patchOperation in (IEnumerable<PatchOperation>) patchOperationList)
      {
        writer.WriteStartObject();
        writer.WritePropertyName("op");
        writer.WriteValue(patchOperation.OperationType.ToEnumMemberString());
        writer.WritePropertyName("path");
        writer.WriteValue(patchOperation.Path);
        Stream valueParam;
        if (patchOperation.TrySerializeValueParameter(this.userSerializer, out valueParam))
        {
          string end;
          using (valueParam)
          {
            using (StreamReader streamReader = new StreamReader(valueParam))
              end = streamReader.ReadToEnd();
          }
          writer.WritePropertyName(nameof (value));
          writer.WriteRawValue(end);
        }
        writer.WriteEndObject();
      }
      writer.WriteEndArray();
      writer.WriteEndObject();
    }

    internal static CosmosSerializer CreatePatchOperationsSerializer(
      CosmosSerializer cosmosSerializer,
      CosmosSerializer propertiesSerializer)
    {
      if (cosmosSerializer == propertiesSerializer)
        return propertiesSerializer;
      return (CosmosSerializer) new CosmosJsonSerializerWrapper((CosmosSerializer) new CosmosJsonDotNetSerializer(new JsonSerializerSettings()
      {
        Converters = (IList<JsonConverter>) new List<JsonConverter>()
        {
          (JsonConverter) new PatchOperationsJsonConverter(cosmosSerializer)
        },
        MaxDepth = new int?(64)
      }));
    }
  }
}
