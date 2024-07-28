// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.PartitionKeyRangeIdentity
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using System;
using System.Globalization;

namespace Microsoft.Azure.Documents
{
  internal sealed class PartitionKeyRangeIdentity : IEquatable<PartitionKeyRangeIdentity>
  {
    public PartitionKeyRangeIdentity(string collectionRid, string partitionKeyRangeId)
    {
      if (collectionRid == null)
        throw new ArgumentNullException(nameof (collectionRid));
      if (partitionKeyRangeId == null)
        throw new ArgumentNullException(nameof (partitionKeyRangeId));
      this.CollectionRid = collectionRid;
      this.PartitionKeyRangeId = partitionKeyRangeId;
    }

    public PartitionKeyRangeIdentity(string partitionKeyRangeId) => this.PartitionKeyRangeId = partitionKeyRangeId != null ? partitionKeyRangeId : throw new ArgumentNullException(nameof (partitionKeyRangeId));

    public static PartitionKeyRangeIdentity FromHeader(string header)
    {
      string[] strArray = header.Split(',');
      if (strArray.Length == 2)
        return new PartitionKeyRangeIdentity(strArray[0], strArray[1]);
      return strArray.Length == 1 ? new PartitionKeyRangeIdentity(strArray[0]) : throw new BadRequestException(RMResources.InvalidPartitionKeyRangeIdHeader);
    }

    public string ToHeader() => this.CollectionRid != null ? string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0},{1}", (object) this.CollectionRid, (object) this.PartitionKeyRangeId) : string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}", (object) this.PartitionKeyRangeId);

    public string CollectionRid { get; private set; }

    public string PartitionKeyRangeId { get; private set; }

    public bool Equals(PartitionKeyRangeIdentity other)
    {
      if (other == null)
        return false;
      if (this == other)
        return true;
      return StringComparer.Ordinal.Equals(this.CollectionRid, other.CollectionRid) && StringComparer.Ordinal.Equals(this.PartitionKeyRangeId, other.PartitionKeyRangeId);
    }

    public override bool Equals(object obj)
    {
      if (obj == null)
        return false;
      if (this == obj)
        return true;
      return obj is PartitionKeyRangeIdentity && this.Equals((PartitionKeyRangeIdentity) obj);
    }

    public override int GetHashCode() => (this.CollectionRid != null ? this.CollectionRid.GetHashCode() : 0) * 397 ^ (this.PartitionKeyRangeId != null ? this.PartitionKeyRangeId.GetHashCode() : 0);
  }
}
