// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Query.Core.QueryClient.ContainerQueryProperties
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Documents;

namespace Microsoft.Azure.Cosmos.Query.Core.QueryClient
{
  internal readonly struct ContainerQueryProperties
  {
    public ContainerQueryProperties(
      string resourceId,
      string effectivePartitionKeyString,
      PartitionKeyDefinition partitionKeyDefinition)
    {
      this.ResourceId = resourceId;
      this.EffectivePartitionKeyString = effectivePartitionKeyString;
      this.PartitionKeyDefinition = partitionKeyDefinition;
    }

    public string ResourceId { get; }

    public string EffectivePartitionKeyString { get; }

    public PartitionKeyDefinition PartitionKeyDefinition { get; }
  }
}
