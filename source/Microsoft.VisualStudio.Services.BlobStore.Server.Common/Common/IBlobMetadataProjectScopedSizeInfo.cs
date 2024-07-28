// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.Common.IBlobMetadataProjectScopedSizeInfo
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CB48D0BF-32A2-483C-A1D4-2F10DEBB3D56
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.Common.dll

using BuildXL.Cache.ContentStore.Hashing;
using System.Collections.Concurrent;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.Common
{
  public interface IBlobMetadataProjectScopedSizeInfo
  {
    BlobIdentifier BlobId { get; }

    BlobReferenceState StoredReferenceState { get; }

    long? BlobLength { get; set; }

    ConcurrentDictionary<string, long> IdReferenceCountByProject { get; }

    ConcurrentBag<Microsoft.VisualStudio.Services.Content.Server.Common.ExportFeedInfo> ExportFeedInfo { get; }
  }
}
