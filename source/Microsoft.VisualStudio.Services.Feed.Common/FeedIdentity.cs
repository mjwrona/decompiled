// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Common.FeedIdentity
// Assembly: Microsoft.VisualStudio.Services.Feed.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AAC6BCA4-7F6C-4DFE-8058-1CCDD886477F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Common.dll

using System;

namespace Microsoft.VisualStudio.Services.Feed.Common
{
  public sealed class FeedIdentity : IEquatable<FeedIdentity>
  {
    public FeedIdentity(Guid? projectId, Guid id)
    {
      Guid? nullable = projectId;
      Guid empty = Guid.Empty;
      if ((nullable.HasValue ? (nullable.HasValue ? (nullable.GetValueOrDefault() == empty ? 1 : 0) : 1) : 0) != 0)
        throw new ArgumentException("Use null for collection level feeds");
      this.ProjectId = projectId;
      this.Id = id;
    }

    public Guid? ProjectId { get; }

    public Guid Id { get; }

    public override bool Equals(object obj) => this.Equals(obj as FeedIdentity);

    public bool Equals(FeedIdentity other)
    {
      if (other == null)
        return false;
      Guid? projectId1 = this.ProjectId;
      Guid? projectId2 = other.ProjectId;
      return (projectId1.HasValue == projectId2.HasValue ? (projectId1.HasValue ? (projectId1.GetValueOrDefault() == projectId2.GetValueOrDefault() ? 1 : 0) : 1) : 0) != 0 && this.Id == other.Id;
    }

    public override int GetHashCode() => this.ProjectId.GetHashCode() * 397 ^ this.Id.GetHashCode();

    public override string ToString()
    {
      Guid? projectId = this.ProjectId;
      ref Guid? local = ref projectId;
      return string.Format("[Project: {0}, Feed: {1}]", (object) ((local.HasValue ? local.GetValueOrDefault().ToString() : (string) null) ?? "(collection-level)"), (object) this.Id);
    }
  }
}
