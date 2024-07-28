// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.CodeOnly.Readme.GetReadmeHandler
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.VisualStudio.Services.Npm.Server.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System.IO;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Npm.Server.CodeOnly.Readme
{
  public class GetReadmeHandler : 
    IAsyncHandler<PackageRequest<NpmPackageIdentity>, Stream>,
    IHaveInputType<PackageRequest<NpmPackageIdentity>>,
    IHaveOutputType<Stream>
  {
    private readonly IAsyncHandler<IPackageRequest<NpmPackageIdentity>, BlobIdentifier> blobIdHandler;
    private readonly IContentBlobStore blobStore;

    public GetReadmeHandler(
      IAsyncHandler<IPackageRequest<NpmPackageIdentity>, BlobIdentifier> blobIdHandler,
      IContentBlobStore blobStore)
    {
      this.blobIdHandler = blobIdHandler;
      this.blobStore = blobStore;
    }

    public async Task<Stream> Handle(PackageRequest<NpmPackageIdentity> request) => await this.blobStore.GetBlobAsync(await this.blobIdHandler.Handle((IPackageRequest<NpmPackageIdentity>) request));
  }
}
