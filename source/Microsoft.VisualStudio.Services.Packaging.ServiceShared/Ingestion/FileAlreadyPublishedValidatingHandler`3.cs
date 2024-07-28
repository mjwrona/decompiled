// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Ingestion.FileAlreadyPublishedValidatingHandler`3
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Ingestion
{
  public class FileAlreadyPublishedValidatingHandler<TPackageId, TEntry, TStorable> : 
    IAsyncHandler<(TStorable Storable, TEntry MetadataEntry)>,
    IAsyncHandler<(TStorable Storable, TEntry MetadataEntry), NullResult>,
    IHaveInputType<(TStorable Storable, TEntry MetadataEntry)>,
    IHaveOutputType<NullResult>
    where TPackageId : IPackageIdentity
    where TEntry : IMetadataEntry<TPackageId>, IPackageFiles
    where TStorable : IPackageFileRequest<TPackageId>, IStorablePackageInfo<TPackageId>
  {
    public Task<NullResult> Handle(
      (TStorable Storable, TEntry MetadataEntry) requestTuple)
    {
      TEntry metadataEntry = requestTuple.MetadataEntry;
      if ((object) metadataEntry == null)
        return Task.FromResult<NullResult>((NullResult) null);
      TStorable storable = requestTuple.Storable;
      if (metadataEntry.IsDeleted())
        throw new PackageExistsAsDeletedException(Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_FeedAlreadyContainsPackageDeleted((object) storable.PackageId.Version, (object) storable.PackageId.Name));
      ref TEntry local = ref metadataEntry;
      if ((object) default (TEntry) == null)
      {
        TEntry entry = local;
        local = ref entry;
      }
      string filePath = storable.FilePath;
      IPackageFile packageFileWithPath = local.GetPackageFileWithPath(filePath);
      bool flag = storable.IngestionDirection == IngestionDirection.DirectPush;
      if (packageFileWithPath != null && (packageFileWithPath.StorageId == null || packageFileWithPath.StorageId.IsLocal))
      {
        if (flag)
          throw new PackageAlreadyExistsException(Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_FeedAlreadyContainsPackageFile((object) storable.Feed.FullyQualifiedName, (object) storable.FilePath, (object) storable.PackageId));
        throw new PackageExistsIngestingFromUpstreamException(Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_FeedAlreadyContainsPackageFile((object) storable.Feed.FullyQualifiedName, (object) storable.FilePath, (object) storable.PackageId));
      }
      if (flag)
      {
        UpstreamSourceInfo upstreamSourceInfo1 = metadataEntry.PackageFiles.FirstOrDefault<IPackageFile>((Func<IPackageFile, bool>) (f => f.StorageId is UpstreamStorageId))?.StorageId is UpstreamStorageId storageId ? storageId.UpstreamContentSource : (UpstreamSourceInfo) null;
        if (upstreamSourceInfo1 == null)
        {
          IEnumerable<UpstreamSourceInfo> sourceChain = metadataEntry.SourceChain;
          upstreamSourceInfo1 = sourceChain != null ? sourceChain.FirstOrDefault<UpstreamSourceInfo>() : (UpstreamSourceInfo) null;
        }
        UpstreamSourceInfo upstreamSourceInfo2 = upstreamSourceInfo1;
        if (upstreamSourceInfo2 != null)
          throw new PackageAlreadyExistsException(Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_CannotPublishExistsOnUpstream((object) storable.PackageId, (object) upstreamSourceInfo2.Name));
        return Task.FromResult<NullResult>((NullResult) null);
      }
      Guid? expectedUpstream = packageFileWithPath?.StorageId is UpstreamStorageId storageId1 ? new Guid?(storageId1.UpstreamContentSource.Id) : new Guid?();
      if (!expectedUpstream.HasValue && packageFileWithPath == null)
      {
        IEnumerable<UpstreamSourceInfo> sourceChain = metadataEntry.SourceChain;
        expectedUpstream = sourceChain != null ? sourceChain.FirstOrDefault<UpstreamSourceInfo>()?.Id : new Guid?();
      }
      this.ThrowOnUnexpectedUpstreamException(new Guid?(storable.SourceChain.Select<UpstreamSourceInfo, Guid>((Func<UpstreamSourceInfo, Guid>) (c => c.Id)).FirstOrDefault<Guid>()), expectedUpstream, (IPackageIdentity) storable.PackageId);
      return Task.FromResult<NullResult>((NullResult) null);
    }

    private void ThrowOnUnexpectedUpstreamException(
      Guid? actualUpstream,
      Guid? expectedUpstream,
      IPackageIdentity packageIdentity)
    {
      if (!actualUpstream.Equals((object) expectedUpstream))
        throw new CannotMixFilesFromDifferentUpstreamsException(Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_CannotMixFilesFromDifferentUpstreams((object) packageIdentity.DisplayStringForMessages, (object) actualUpstream, (object) expectedUpstream));
    }
  }
}
