// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.Common.IBlobMetadataSizeInfo
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CB48D0BF-32A2-483C-A1D4-2F10DEBB3D56
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.Common.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using System;
using System.Collections.Concurrent;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.Common
{
  public interface IBlobMetadataSizeInfo
  {
    BlobIdentifier BlobId { get; }

    BlobReferenceState StoredReferenceState { get; }

    DateTimeOffset? BlobAddedTime { get; set; }

    DateTimeOffset? KeepUntilTime { get; }

    long? BlobLength { get; set; }

    long IdReferenceCount { get; }

    ConcurrentDictionary<ArtifactScopeType, long> IdReferenceCountByScope { get; }

    ConcurrentDictionary<string, ulong> IdReferenceCountByFeed { get; set; }
  }
}
