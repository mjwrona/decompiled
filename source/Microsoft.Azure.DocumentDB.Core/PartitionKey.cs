// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.PartitionKey
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Microsoft.Azure.Documents.Routing;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Documents
{
  public sealed class PartitionKey
  {
    public const string SystemKeyName = "_partitionKey";
    public const string SystemKeyPath = "/_partitionKey";

    private PartitionKey()
    {
    }

    public PartitionKey(object keyValue) => this.InternalKey = PartitionKeyInternal.FromObjectArray((IEnumerable<object>) new object[1]
    {
      keyValue
    }, true);

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
