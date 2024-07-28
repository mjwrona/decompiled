// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Ingestion.PutBlobAndReferenceHandler`2
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Ingestion
{
  public class PutBlobAndReferenceHandler<TPackageId, TStorable> : 
    IAsyncHandler<TStorable>,
    IAsyncHandler<TStorable, NullResult>,
    IHaveInputType<TStorable>,
    IHaveOutputType<NullResult>
    where TPackageId : IPackageIdentity
    where TStorable : IPackageFileRequest<TPackageId>, IStorablePackageInfo<TPackageId>, IContentStreamStorable
  {
    private readonly IContentBlobStore contentBlobStore;
    private readonly IConverter<IPackageFileRequest<TPackageId>, string> refCalculatingConverter;

    public PutBlobAndReferenceHandler(
      IContentBlobStore contentBlobStore,
      IConverter<IPackageFileRequest<TPackageId>, string> refCalculatingConverter)
    {
      this.contentBlobStore = contentBlobStore;
      this.refCalculatingConverter = refCalculatingConverter;
    }

    public async Task<NullResult> Handle(TStorable request)
    {
      BlobStorageId packageStorageId = request.PackageStorageId;
      if (packageStorageId != null)
      {
        string id = this.refCalculatingConverter.Convert((IPackageFileRequest<TPackageId>) request);
        await this.contentBlobStore.PutBlobAndReferenceAsync(packageStorageId.BlobId, request.ContentStream, new BlobReference(id, request.PackageId.Name.Protocol.LowercasedName));
      }
      return (NullResult) null;
    }
  }
}
