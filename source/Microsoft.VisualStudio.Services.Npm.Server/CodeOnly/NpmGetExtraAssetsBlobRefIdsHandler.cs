// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.CodeOnly.NpmGetExtraAssetsBlobRefIdsHandler
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.VisualStudio.Services.Npm.Server.Utils;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobStore;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Npm.Server.CodeOnly
{
  public class NpmGetExtraAssetsBlobRefIdsHandler : 
    IAsyncHandler<NpmPackageRequestWithMetadata, IEnumerable<BlobReferenceIdentifier>>,
    IHaveInputType<NpmPackageRequestWithMetadata>,
    IHaveOutputType<IEnumerable<BlobReferenceIdentifier>>
  {
    public Task<IEnumerable<BlobReferenceIdentifier>> Handle(
      NpmPackageRequestWithMetadata requestWithMetadata)
    {
      return Task.FromResult<IEnumerable<BlobReferenceIdentifier>>(NpmBlobUtils.GetExtraAssetsBlobRefIds(requestWithMetadata.Request.Feed.Id, requestWithMetadata.Metadata.PackageIdentity, requestWithMetadata.Metadata.PackageManifest));
    }
  }
}
