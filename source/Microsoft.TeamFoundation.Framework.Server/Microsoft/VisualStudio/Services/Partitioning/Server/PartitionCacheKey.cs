// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Partitioning.Server.PartitionCacheKey
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.VisualStudio.Services.Partitioning.Server
{
  internal struct PartitionCacheKey : IEquatable<PartitionCacheKey>
  {
    public string PartitionKey;
    public Guid ContainerType;

    public PartitionCacheKey(string partitionKey, Guid containerType)
    {
      this.PartitionKey = partitionKey;
      this.ContainerType = containerType;
    }

    public override string ToString() => string.Format("{0}::{1}", (object) this.PartitionKey.ToUpperInvariant(), (object) this.ContainerType);

    public bool Equals(PartitionCacheKey other) => StringComparer.OrdinalIgnoreCase.Equals(this.PartitionKey, other.PartitionKey) && this.ContainerType == other.ContainerType;

    public override bool Equals(object obj) => obj is PartitionCacheKey other && this.Equals(other);

    public override int GetHashCode() => StringComparer.OrdinalIgnoreCase.GetHashCode(this.PartitionKey) ^ this.ContainerType.GetHashCode();

    public static bool operator ==(PartitionCacheKey a, PartitionCacheKey b) => a.Equals(b);

    public static bool operator !=(PartitionCacheKey a, PartitionCacheKey b) => !(a == b);
  }
}
