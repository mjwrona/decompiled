// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Common.RawFeedIdentity
// Assembly: Microsoft.VisualStudio.Services.Feed.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AAC6BCA4-7F6C-4DFE-8058-1CCDD886477F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Common.dll

using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Feed.Common
{
  public sealed class RawFeedIdentity : IEquatable<RawFeedIdentity>
  {
    private static readonly IEqualityComparer<string> FeedNameOrIdComparer = (IEqualityComparer<string>) StringComparer.InvariantCultureIgnoreCase;

    public RawFeedIdentity(Guid? projectId, string feedNameOrId)
    {
      Guid? nullable = projectId;
      Guid empty = Guid.Empty;
      if ((nullable.HasValue ? (nullable.HasValue ? (nullable.GetValueOrDefault() == empty ? 1 : 0) : 1) : 0) != 0)
        throw new ArgumentException("Use null for collection level feeds");
      if (string.IsNullOrWhiteSpace(feedNameOrId))
        throw new ArgumentException("Value cannot be null or whitespace.", nameof (feedNameOrId));
      this.ProjectId = projectId;
      this.FeedNameOrId = feedNameOrId;
    }

    public Guid? ProjectId { get; }

    public string FeedNameOrId { get; }

    public override bool Equals(object obj) => this.Equals(obj as RawFeedIdentity);

    public bool Equals(RawFeedIdentity other)
    {
      if (other == null)
        return false;
      Guid? projectId1 = this.ProjectId;
      Guid? projectId2 = other.ProjectId;
      return (projectId1.HasValue == projectId2.HasValue ? (projectId1.HasValue ? (projectId1.GetValueOrDefault() == projectId2.GetValueOrDefault() ? 1 : 0) : 1) : 0) != 0 && RawFeedIdentity.FeedNameOrIdComparer.Equals(this.FeedNameOrId, other.FeedNameOrId);
    }

    public override int GetHashCode() => this.ProjectId.GetHashCode() * 397 ^ RawFeedIdentity.FeedNameOrIdComparer.GetHashCode(this.FeedNameOrId);

    public override string ToString()
    {
      string[] strArray = new string[5]
      {
        "[Project: ",
        null,
        null,
        null,
        null
      };
      Guid? projectId = this.ProjectId;
      ref Guid? local = ref projectId;
      strArray[1] = (local.HasValue ? local.GetValueOrDefault().ToString() : (string) null) ?? "(collection-level)";
      strArray[2] = ", Feed: ";
      strArray[3] = this.FeedNameOrId;
      strArray[4] = "]";
      return string.Concat(strArray);
    }

    public static implicit operator RawFeedIdentity(FeedIdentity obj) => new RawFeedIdentity(obj.ProjectId, obj.Id.ToString());
  }
}
