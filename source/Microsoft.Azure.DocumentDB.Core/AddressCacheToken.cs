// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.AddressCacheToken
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

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
