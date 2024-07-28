// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.WorkItemRecentHistoryCache.CacheKey
// Assembly: Microsoft.VisualStudio.Services.Search.Server.WorkItemRecentHistoryCache, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9D4A0500-806F-44D4-BA97-D409A2311716
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Server.WorkItemRecentHistoryCache.dll

using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Server.WorkItemRecentHistoryCache
{
  public struct CacheKey : IEqualityComparer<CacheKey>, IEquatable<CacheKey>
  {
    public CacheKey(Guid userId, Guid projectId)
    {
      this.UserId = userId;
      this.ProjectId = projectId;
    }

    public Guid UserId { get; }

    public Guid ProjectId { get; }

    public override string ToString()
    {
      Guid guid = this.UserId;
      string str1 = guid.ToString();
      guid = this.ProjectId;
      string str2 = guid.ToString();
      return str1 + str2;
    }

    bool IEqualityComparer<CacheKey>.Equals(CacheKey a, CacheKey b) => a.Equals(b);

    public bool Equals(CacheKey other) => this.UserId == other.UserId && this.ProjectId == other.ProjectId;

    public override bool Equals(object obj) => obj is CacheKey other && this.Equals(other);

    public override int GetHashCode() => (17 * 23 + this.UserId.GetHashCode()) * 23 + this.ProjectId.GetHashCode();

    public static bool operator ==(CacheKey left, CacheKey right) => left.Equals(right);

    public static bool operator !=(CacheKey left, CacheKey right) => !left.Equals(right);

    int IEqualityComparer<CacheKey>.GetHashCode(CacheKey key) => key.GetHashCode();
  }
}
