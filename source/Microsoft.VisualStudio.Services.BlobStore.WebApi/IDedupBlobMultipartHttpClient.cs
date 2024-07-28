// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.WebApi.IDedupBlobMultipartHttpClient
// Assembly: Microsoft.VisualStudio.Services.BlobStore.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4500AC57-FBCC-4F18-B11F-F661A75E4A46
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.WebApi.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.BlobStore.WebApi.Contracts;
using Microsoft.VisualStudio.Services.Content.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.BlobStore.WebApi
{
  public interface IDedupBlobMultipartHttpClient : IArtifactHttpClient
  {
    Task<string> PostFileStreamAsync(
      IDomainId domainId,
      Stream file,
      CancellationToken cancellationToken);

    Task<Stream> GetFileStreamAsync(
      IDomainId domainId,
      DedupIdentifier nodeId,
      bool requestCompession,
      CancellationToken cancellationToken);

    Task<Guid> CreateSessionAsync(IDomainId domainId, CancellationToken cancellationToken);

    Task<Microsoft.VisualStudio.Services.BlobStore.WebApi.Contracts.Session> GetSessionAsync(
      IDomainId domainId,
      Guid sessionId,
      CancellationToken cancellationToken);

    Task DeleteSessionAsync(
      IDomainId domainId,
      Guid sessionId,
      CancellationToken cancellationToken);

    Task<string> CompleteSessionAsync(
      IDomainId domainId,
      Guid sessionId,
      CancellationToken cancellationToken);

    Task<IEnumerable<Part>> GetPartsAsync(
      IDomainId domainId,
      Guid sessionId,
      CancellationToken cancellationToken);

    Task UploadPartsAsync(
      IDomainId domainId,
      Guid sessionId,
      Stream file,
      long from,
      long to,
      long totalSize,
      CancellationToken cancellationToken);
  }
}
