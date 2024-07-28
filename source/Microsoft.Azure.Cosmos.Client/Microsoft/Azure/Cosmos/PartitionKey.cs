// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.PartitionKey
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Documents.Routing;
using System;

namespace Microsoft.Azure.Cosmos
{
  public readonly struct PartitionKey : IEquatable<PartitionKey>
  {
    private static readonly PartitionKeyInternal NullPartitionKeyInternal = new Microsoft.Azure.Documents.PartitionKey((object[]) null).InternalKey;
    private static readonly PartitionKeyInternal TruePartitionKeyInternal = new Microsoft.Azure.Documents.PartitionKey((object) true).InternalKey;
    private static readonly PartitionKeyInternal FalsePartitionKeyInternal = new Microsoft.Azure.Documents.PartitionKey((object) false).InternalKey;
    public static readonly PartitionKey None = new PartitionKey(Microsoft.Azure.Documents.PartitionKey.None.InternalKey, true);
    public static readonly PartitionKey Null = new PartitionKey(PartitionKey.NullPartitionKeyInternal);
    public static readonly string SystemKeyName = "_partitionKey";
    public static readonly string SystemKeyPath = "/_partitionKey";

    internal PartitionKeyInternal InternalKey { get; }

    internal bool IsNone { get; }

    public PartitionKey(string partitionKeyValue)
    {
      this.InternalKey = partitionKeyValue != null ? new Microsoft.Azure.Documents.PartitionKey((object) partitionKeyValue).InternalKey : PartitionKey.NullPartitionKeyInternal;
      this.IsNone = false;
    }

    public PartitionKey(bool partitionKeyValue)
    {
      this.InternalKey = partitionKeyValue ? PartitionKey.TruePartitionKeyInternal : PartitionKey.FalsePartitionKeyInternal;
      this.IsNone = false;
    }

    public PartitionKey(double partitionKeyValue)
    {
      this.InternalKey = new Microsoft.Azure.Documents.PartitionKey((object) partitionKeyValue).InternalKey;
      this.IsNone = false;
    }

    internal PartitionKey(object value)
    {
      this.InternalKey = new Microsoft.Azure.Documents.PartitionKey(value).InternalKey;
      this.IsNone = false;
    }

    internal PartitionKey(PartitionKeyInternal partitionKeyInternal)
    {
      this.InternalKey = partitionKeyInternal;
      this.IsNone = false;
    }

    private PartitionKey(PartitionKeyInternal partitionKeyInternal, bool isNone = false)
    {
      this.InternalKey = partitionKeyInternal;
      this.IsNone = isNone;
    }

    public override bool Equals(object obj) => obj is PartitionKey other && this.Equals(other);

    public override int GetHashCode() => this.InternalKey == null ? PartitionKey.NullPartitionKeyInternal.GetHashCode() : this.InternalKey.GetHashCode();

    public bool Equals(PartitionKey other)
    {
      PartitionKeyInternal partitionKeyInternal = this.InternalKey;
      PartitionKeyInternal other1 = other.InternalKey;
      if (partitionKeyInternal == null)
        partitionKeyInternal = PartitionKey.NullPartitionKeyInternal;
      if (other1 == null)
        other1 = PartitionKey.NullPartitionKeyInternal;
      return partitionKeyInternal.Equals(other1);
    }

    public override string ToString() => this.InternalKey == null ? PartitionKey.NullPartitionKeyInternal.ToJsonString() : this.InternalKey.ToJsonString();

    internal string ToJsonString() => this.InternalKey.ToJsonString();

    internal static bool TryParseJsonString(
      string partitionKeyString,
      out PartitionKey partitionKey)
    {
      if (partitionKeyString == null)
        throw new ArgumentNullException(partitionKeyString);
      try
      {
        PartitionKeyInternal partitionKeyInternal = PartitionKeyInternal.FromJsonString(partitionKeyString);
        partitionKey = partitionKeyInternal.Components != null ? new PartitionKey(partitionKeyInternal, false) : PartitionKey.None;
        return true;
      }
      catch (Exception ex)
      {
        partitionKey = new PartitionKey();
        return false;
      }
    }

    public static bool operator ==(PartitionKey left, PartitionKey right) => left.Equals(right);

    public static bool operator !=(PartitionKey left, PartitionKey right) => !left.Equals(right);
  }
}
