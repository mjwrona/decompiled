// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.CacheKeys
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using System;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal static class CacheKeys
  {
    public struct CrossHostOdbId : IEquatable<CacheKeys.CrossHostOdbId>
    {
      public readonly Guid HostId;
      public readonly OdbId OdbId;
      public const int Size = 32;

      public CrossHostOdbId(Guid hostId, OdbId odbId)
      {
        this.HostId = hostId;
        this.OdbId = odbId;
      }

      public bool Equals(CacheKeys.CrossHostOdbId other) => this.HostId == other.HostId && this.OdbId == other.OdbId;

      public override bool Equals(object other) => other is CacheKeys.CrossHostOdbId other1 && this.Equals(other1);

      public override int GetHashCode() => HashCodeUtil.GetHashCode<Guid, OdbId>(this.HostId, this.OdbId);
    }

    public struct CrossHostOdbScopedObjectId : IEquatable<CacheKeys.CrossHostOdbScopedObjectId>
    {
      public readonly CacheKeys.CrossHostOdbId CrossHostOdbId;
      public readonly Sha1Id ObjectId;
      public const int Size = 52;

      public CrossHostOdbScopedObjectId(CacheKeys.CrossHostOdbId crossHostOdbId, Sha1Id objectId)
      {
        this.CrossHostOdbId = crossHostOdbId;
        this.ObjectId = objectId;
      }

      public bool Equals(CacheKeys.CrossHostOdbScopedObjectId other) => this.CrossHostOdbId.Equals(other.CrossHostOdbId) && this.ObjectId == other.ObjectId;

      public override bool Equals(object other) => other is CacheKeys.CrossHostOdbScopedObjectId other1 && this.Equals(other1);

      public override int GetHashCode() => HashCodeUtil.GetHashCode<CacheKeys.CrossHostOdbId, Sha1Id>(this.CrossHostOdbId, this.ObjectId);
    }
  }
}
