// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.PackagingCommitId
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Newtonsoft.Json;
using System;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog
{
  [JsonConverter(typeof (PackagingCommitIdJsonConverter))]
  public struct PackagingCommitId : IEquatable<PackagingCommitId>
  {
    public static readonly PackagingCommitId Empty = new PackagingCommitId(Guid.Empty);
    private readonly Guid id;

    private PackagingCommitId(Guid id)
      : this()
    {
      this.id = id;
    }

    public static bool operator ==(PackagingCommitId left, PackagingCommitId right) => left.Equals(right);

    public static bool operator !=(PackagingCommitId left, PackagingCommitId right) => !left.Equals(right);

    public static PackagingCommitId CreateNew() => new PackagingCommitId(Guid.NewGuid());

    public static PackagingCommitId Parse(string value) => new PackagingCommitId(Guid.ParseExact(value, "N"));

    public bool Equals(PackagingCommitId other) => this.id.Equals(other.id);

    public override bool Equals(object obj) => obj != null && obj is PackagingCommitId other && this.Equals(other);

    public override int GetHashCode() => this.id.GetHashCode();

    public override string ToString() => this.id.ToString("N");

    internal Guid ToGuid() => this.id;
  }
}
