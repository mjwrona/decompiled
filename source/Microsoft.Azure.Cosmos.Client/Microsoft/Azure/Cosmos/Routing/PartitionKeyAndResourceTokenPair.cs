// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Routing.PartitionKeyAndResourceTokenPair
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Documents.Routing;

namespace Microsoft.Azure.Cosmos.Routing
{
  internal sealed class PartitionKeyAndResourceTokenPair
  {
    public PartitionKeyAndResourceTokenPair(PartitionKeyInternal partitionKey, string resourceToken)
    {
      this.PartitionKey = partitionKey;
      this.ResourceToken = resourceToken;
    }

    public PartitionKeyInternal PartitionKey { get; private set; }

    public string ResourceToken { get; private set; }
  }
}
