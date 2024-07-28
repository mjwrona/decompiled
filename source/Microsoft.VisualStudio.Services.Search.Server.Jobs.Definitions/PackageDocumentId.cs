// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions.PackageDocumentId
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10D2EBC4-B606-4155-939F-EEB226A80181
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions.dll

using System;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions
{
  public class PackageDocumentId : IEquatable<PackageDocumentId>
  {
    public Guid FeedId { get; }

    public Guid PackageId { get; }

    public Guid VersionId { get; }

    public PackageDocumentId(Guid feedId, Guid packageId, Guid versionId)
    {
      this.FeedId = feedId;
      this.PackageId = packageId;
      this.VersionId = versionId;
    }

    public override bool Equals(object obj) => this.Equals(obj as PackageDocumentId);

    public bool Equals(PackageDocumentId other)
    {
      if (other == null)
        return false;
      if (this == other)
        return true;
      return this.FeedId == other.FeedId && this.PackageId == other.PackageId && this.VersionId == other.VersionId;
    }

    public override int GetHashCode() => ((160924800 * -1521134295 + this.FeedId.GetHashCode()) * -1521134295 + this.PackageId.GetHashCode()) * -1521134295 + this.VersionId.GetHashCode();

    public override string ToString() => FormattableString.Invariant(FormattableStringFactory.Create("Feed Id: [{0}] Package Id: [{1}] Version Id: [{2}]", (object) this.FeedId, (object) this.PackageId, (object) this.VersionId));
  }
}
