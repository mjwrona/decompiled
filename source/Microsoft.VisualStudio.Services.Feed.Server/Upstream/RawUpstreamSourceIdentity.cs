// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.Upstream.RawUpstreamSourceIdentity
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Feed.Server.Upstream
{
  public class RawUpstreamSourceIdentity : IEquatable<RawUpstreamSourceIdentity>
  {
    public RawUpstreamSourceIdentity(
      Guid collectionId,
      Guid feedId,
      Guid? projectId,
      Guid viewId,
      string protocolType)
    {
      ArgumentUtility.CheckForEmptyGuid(collectionId, nameof (collectionId));
      ArgumentUtility.CheckForEmptyGuid(feedId, nameof (feedId));
      Guid? nullable = projectId;
      Guid empty = Guid.Empty;
      if ((nullable.HasValue ? (nullable.HasValue ? (nullable.GetValueOrDefault() == empty ? 1 : 0) : 1) : 0) != 0)
        throw new ArgumentException("Use null for collection level feeds", nameof (projectId));
      ArgumentUtility.CheckForEmptyGuid(viewId, nameof (viewId));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(protocolType, nameof (protocolType));
      this.CollectionId = collectionId;
      this.FeedId = feedId;
      this.ProjectId = projectId;
      this.ViewId = viewId;
      this.ProtocolType = protocolType;
    }

    public Guid CollectionId { get; }

    public Guid FeedId { get; }

    public Guid? ProjectId { get; }

    public Guid ViewId { get; }

    public string ProtocolType { get; }

    public override bool Equals(object obj) => this.Equals(obj as RawUpstreamSourceIdentity);

    public bool Equals(RawUpstreamSourceIdentity other)
    {
      if (other == null || !(this.ViewId == other.ViewId) || !(this.FeedId == other.FeedId))
        return false;
      Guid? projectId1 = this.ProjectId;
      Guid? projectId2 = other.ProjectId;
      return (projectId1.HasValue == projectId2.HasValue ? (projectId1.HasValue ? (projectId1.GetValueOrDefault() == projectId2.GetValueOrDefault() ? 1 : 0) : 1) : 0) != 0 && this.CollectionId == other.CollectionId && this.ProtocolType.Equals(other.ProtocolType, StringComparison.OrdinalIgnoreCase);
    }

    public override string ToString() => string.Join<bool>(":", new List<Guid?>()
    {
      new Guid?(this.CollectionId),
      this.ProjectId,
      new Guid?(this.FeedId),
      new Guid?(this.ViewId)
    }.Select<Guid?, bool>((Func<Guid?, bool>) (x => x.HasValue))) + ":" + this.ProtocolType;

    public override int GetHashCode()
    {
      Guid guid = this.CollectionId;
      int num1 = (guid.GetHashCode() * 397 ^ this.ProjectId.GetHashCode()) * 397;
      guid = this.FeedId;
      int hashCode1 = guid.GetHashCode();
      int num2 = (num1 ^ hashCode1) * 397;
      guid = this.ViewId;
      int hashCode2 = guid.GetHashCode();
      return (num2 ^ hashCode2) * 397 ^ this.ProtocolType.GetHashCode();
    }
  }
}
