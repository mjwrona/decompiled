// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.WebApi.IDedupUploadSession
// Assembly: Microsoft.VisualStudio.Services.BlobStore.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4500AC57-FBCC-4F18-B11F-F661A75E4A46
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.WebApi.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.BlobStore.Common.Telemetry;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.BlobStore.WebApi
{
  [CLSCompliant(false)]
  public interface IDedupUploadSession
  {
    IReadOnlyDictionary<NodeDedupIdentifier, DedupNode> AllNodes { get; }

    IReadOnlyDictionary<DedupIdentifier, NodeDedupIdentifier> ParentLookup { get; }

    DedupUploadStatistics UploadStatistics { get; }

    Task<KeepUntilReceipt> UploadAsync(
      DedupNode node,
      IReadOnlyDictionary<DedupIdentifier, string> filePaths,
      CancellationToken cancellationToken);

    Task<Dictionary<DedupIdentifier, CheckIfUploadNeededResult>> CheckIfUploadIsNeededAsync(
      IReadOnlyDictionary<DedupIdentifier, long> fileSizes,
      CancellationToken cancellationToken);
  }
}
