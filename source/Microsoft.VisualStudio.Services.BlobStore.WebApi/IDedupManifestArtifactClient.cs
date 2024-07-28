// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.WebApi.IDedupManifestArtifactClient
// Assembly: Microsoft.VisualStudio.Services.BlobStore.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4500AC57-FBCC-4F18-B11F-F661A75E4A46
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.WebApi.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.BlobStore.WebApi
{
  public interface IDedupManifestArtifactClient : IDisposable
  {
    Task DownloadAsync(
      DedupIdentifier manifestId,
      string targetDirectory,
      CancellationToken cancellationToken);

    Task DownloadAsync(
      DownloadDedupManifestArtifactOptions downloadPipelineArtifactOptions,
      CancellationToken cancellationToken);

    Task DownloadAsyncWithManifestPath(
      string fullManifestPath,
      string targetDirectory,
      Uri proxyUri,
      CancellationToken cancellationToken);

    Task DownloadAsyncWithManifestPath(
      DownloadDedupManifestArtifactOptions downloadPipelineArtifactOptions,
      CancellationToken cancellationToken);

    Task DownloadFileToPathAsync(
      DedupIdentifier dedupId,
      string fullFileOutputPath,
      Uri proxyUri,
      CancellationToken cancellationToken);

    Task DownloadToStreamAsync(
      DedupIdentifier dedupId,
      Stream stream,
      Uri proxyUri,
      CancellationToken cancellationToken);

    Task<PublishResult> PublishAsync(
      string fullPath,
      ArtifactPublishOptions artifactPublishOptions,
      string manifestFileOutputPath,
      CancellationToken cancellationToken);

    Task<PublishResult> PublishAsync(
      string sourceDirectory,
      IReadOnlyCollection<FileInfo> fileInfoList,
      IReadOnlyDictionary<string, FileInfo> otherFiles,
      ArtifactPublishOptions artifactPublishOptions,
      string manifestFileOutputPath,
      CancellationToken cancellationToken);
  }
}
