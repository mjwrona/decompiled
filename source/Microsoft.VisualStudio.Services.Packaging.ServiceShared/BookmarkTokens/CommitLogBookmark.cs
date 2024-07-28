// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BookmarkTokens.CommitLogBookmark
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Newtonsoft.Json;
using System;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BookmarkTokens
{
  [DebuggerDisplay("seq: {SequenceNumber}, id: {CommitId}")]
  public struct CommitLogBookmark : IEquatable<CommitLogBookmark>
  {
    public static readonly CommitLogBookmark Empty = new CommitLogBookmark(PackagingCommitId.Empty, new long?());

    [JsonProperty("id")]
    public PackagingCommitId CommitId { get; }

    [JsonProperty("seq")]
    public long? SequenceNumber { get; }

    [JsonConstructor]
    public CommitLogBookmark(PackagingCommitId id, long? seq)
    {
      this.CommitId = id;
      this.SequenceNumber = seq;
    }

    public bool Equals(CommitLogBookmark other) => this.CommitId.Equals(other.CommitId);

    public override bool Equals(object obj) => obj != null && obj is CommitLogBookmark other && this.Equals(other);

    public override int GetHashCode() => this.CommitId.GetHashCode();

    public static bool operator ==(CommitLogBookmark left, CommitLogBookmark right) => left.Equals(right);

    public static bool operator !=(CommitLogBookmark left, CommitLogBookmark right) => !left.Equals(right);

    public override string ToString() => string.Format("seq: {0}, id: {1}", (object) this.SequenceNumber, (object) this.CommitId);
  }
}
