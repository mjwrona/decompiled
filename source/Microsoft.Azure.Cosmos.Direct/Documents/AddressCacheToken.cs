// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.AddressCacheToken
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using System;

namespace Microsoft.Azure.Documents
{
  internal sealed class AddressCacheToken
  {
    public readonly PartitionKeyRangeIdentity PartitionKeyRangeIdentity;

    public Uri ServiceEndpoint { get; private set; }

    public AddressCacheToken(
      PartitionKeyRangeIdentity partitionKeyRangeIdentity,
      Uri serviceEndpoint)
    {
      this.PartitionKeyRangeIdentity = partitionKeyRangeIdentity;
      this.ServiceEndpoint = serviceEndpoint;
    }

    public override bool Equals(object obj) => this.Equals(obj as AddressCacheToken);

    public bool Equals(AddressCacheToken token) => token != null && this.PartitionKeyRangeIdentity.Equals(token.PartitionKeyRangeIdentity) && this.ServiceEndpoint.Equals((object) token.ServiceEndpoint);

    public override int GetHashCode() => this.PartitionKeyRangeIdentity.GetHashCode() ^ this.ServiceEndpoint.GetHashCode();
  }
}
