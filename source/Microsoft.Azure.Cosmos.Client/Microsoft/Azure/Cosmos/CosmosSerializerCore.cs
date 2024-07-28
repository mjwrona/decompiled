// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.CosmosSerializerCore
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.ChangeFeed;
using Microsoft.Azure.Cosmos.Query.Core;
using Microsoft.Azure.Cosmos.Scripts;
using Microsoft.Azure.Documents;
using System;
using System.IO;

namespace Microsoft.Azure.Cosmos
{
  internal class CosmosSerializerCore
  {
    private static readonly CosmosSerializer propertiesSerializer = (CosmosSerializer) new CosmosJsonSerializerWrapper((CosmosSerializer) new CosmosJsonDotNetSerializer());
    private readonly CosmosSerializer customSerializer;
    private readonly CosmosSerializer sqlQuerySpecSerializer;
    private CosmosSerializer patchOperationSerializer;

    internal CosmosSerializerCore(CosmosSerializer customSerializer = null)
    {
      if (customSerializer == null)
      {
        this.customSerializer = (CosmosSerializer) null;
        this.sqlQuerySpecSerializer = CosmosSqlQuerySpecJsonConverter.CreateSqlQuerySpecSerializer((CosmosSerializer) null, CosmosSerializerCore.propertiesSerializer);
        this.patchOperationSerializer = (CosmosSerializer) null;
      }
      else
      {
        this.customSerializer = (CosmosSerializer) new CosmosJsonSerializerWrapper(customSerializer);
        this.sqlQuerySpecSerializer = CosmosSqlQuerySpecJsonConverter.CreateSqlQuerySpecSerializer(this.customSerializer, CosmosSerializerCore.propertiesSerializer);
        this.patchOperationSerializer = PatchOperationsJsonConverter.CreatePatchOperationsSerializer(this.customSerializer, CosmosSerializerCore.propertiesSerializer);
      }
    }

    internal static CosmosSerializerCore Create(
      CosmosSerializer customSerializer,
      CosmosSerializationOptions serializationOptions)
    {
      if (customSerializer != null && serializationOptions != null)
        throw new ArgumentException("Customer serializer and serialization options can not be set at the same time.");
      if (serializationOptions != null)
        customSerializer = (CosmosSerializer) new CosmosJsonSerializerWrapper((CosmosSerializer) new CosmosJsonDotNetSerializer(serializationOptions));
      return new CosmosSerializerCore(customSerializer);
    }

    internal T FromStream<T>(Stream stream) => this.GetSerializer<T>().FromStream<T>(stream);

    internal T[] FromFeedStream<T>(Stream stream) => this.GetSerializer<T>().FromStream<T[]>(stream);

    internal Stream ToStream<T>(T input) => this.GetSerializer<T>().ToStream<T>(input);

    internal Stream ToStreamSqlQuerySpec(SqlQuerySpec input, ResourceType resourceType)
    {
      CosmosSerializer cosmosSerializer = CosmosSerializerCore.propertiesSerializer;
      if (resourceType == ResourceType.Database || resourceType == ResourceType.Collection || resourceType == ResourceType.Document || resourceType == ResourceType.Trigger || resourceType == ResourceType.UserDefinedFunction || resourceType == ResourceType.StoredProcedure || resourceType == ResourceType.Permission || resourceType == ResourceType.User || resourceType == ResourceType.Conflict)
        cosmosSerializer = this.sqlQuerySpecSerializer;
      return cosmosSerializer.ToStream<SqlQuerySpec>(input);
    }

    internal CosmosSerializer GetCustomOrDefaultSerializer() => this.customSerializer != null ? this.customSerializer : CosmosSerializerCore.propertiesSerializer;

    private CosmosSerializer GetSerializer<T>()
    {
      Type type = typeof (T);
      if (type == typeof (PatchSpec))
      {
        if (this.patchOperationSerializer == null)
          this.patchOperationSerializer = PatchOperationsJsonConverter.CreatePatchOperationsSerializer(this.customSerializer ?? (CosmosSerializer) new CosmosJsonDotNetSerializer(), CosmosSerializerCore.propertiesSerializer);
        return this.patchOperationSerializer;
      }
      if (this.customSerializer == null || type == typeof (AccountProperties) || type == typeof (DatabaseProperties) || type == typeof (ContainerProperties) || type == typeof (PermissionProperties) || type == typeof (StoredProcedureProperties) || type == typeof (TriggerProperties) || type == typeof (UserDefinedFunctionProperties) || type == typeof (UserProperties) || type == typeof (ConflictProperties) || type == typeof (ThroughputProperties) || type == typeof (OfferV2) || type == typeof (ClientEncryptionKeyProperties) || type == typeof (Microsoft.Azure.Cosmos.Query.Core.QueryPlan.PartitionedQueryExecutionInfo) || type == typeof (ChangeFeedQuerySpec))
        return CosmosSerializerCore.propertiesSerializer;
      if (type == typeof (SqlQuerySpec))
        throw new ArgumentException("SqlQuerySpec to stream must use the SqlQuerySpec override");
      return this.customSerializer;
    }
  }
}
