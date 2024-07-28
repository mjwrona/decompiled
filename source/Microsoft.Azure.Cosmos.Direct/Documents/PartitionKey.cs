// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.PartitionKey
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using Microsoft.Azure.Documents.Routing;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Documents
{
  internal sealed class PartitionKey
  {
    public const string SystemKeyName = "_partitionKey";
    public const string SystemKeyPath = "/_partitionKey";

    private PartitionKey()
    {
    }

    public PartitionKey(object keyValue) => this.InternalKey = PartitionKeyInternal.FromObject(keyValue, true);

    internal PartitionKey(object[] keyValues) => this.InternalKey = PartitionKeyInternal.FromObjectArray((IEnumerable<object>) (keyValues ?? new object[1]), true);

    public static PartitionKey FromJsonString(string keyValue) => !string.IsNullOrEmpty(keyValue) ? new PartitionKey()
    {
      InternalKey = PartitionKeyInternal.FromJsonString(keyValue)
    } : throw new ArgumentException("keyValue must not be null or empty.");

    public static PartitionKey None => new PartitionKey()
    {
      InternalKey = PartitionKeyInternal.None
    };

    internal static PartitionKey FromInternalKey(PartitionKeyInternal keyValue) => keyValue != null ? new PartitionKey()
    {
      InternalKey = keyValue
    } : throw new ArgumentException("keyValue must not be null or empty.");

    internal PartitionKeyInternal InternalKey { get; private set; }

    public override string ToString() => this.InternalKey.ToJsonString();

    public override bool Equals(object other)
    {
      if (other == null)
        return false;
      if (this == other)
        return true;
      return other is PartitionKey partitionKey && this.InternalKey.Equals(partitionKey.InternalKey);
    }

    public override int GetHashCode() => this.InternalKey == null ? base.GetHashCode() : this.InternalKey.GetHashCode();
  }
}
