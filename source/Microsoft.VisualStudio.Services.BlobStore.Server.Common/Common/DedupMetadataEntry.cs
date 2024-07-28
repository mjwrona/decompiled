// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.Common.DedupMetadataEntry
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CB48D0BF-32A2-483C-A1D4-2F10DEBB3D56
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.Common.dll

using BuildXL.Cache.ContentStore.Hashing;
using System;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.Common
{
  public sealed class DedupMetadataEntry
  {
    public DedupIdentifier DedupId { get; }

    public string Scope { get; }

    public string ReferenceId { get; }

    public bool IsSoftDeleted { get; }

    public long? Size { get; }

    public DateTimeOffset? StateChangeTime { get; }

    public DedupMetadataEntry(
      DedupIdentifier id,
      string scope,
      string refId,
      bool isSoftDeleted,
      DateTimeOffset? stateChangeTime,
      long? size)
    {
      this.DedupId = id;
      this.Scope = scope;
      this.ReferenceId = refId;
      this.IsSoftDeleted = isSoftDeleted;
      this.StateChangeTime = stateChangeTime;
      this.Size = size;
    }
  }
}
