// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.DeleteProcessingJob.MavenGetExtraAssetsBlobRefIdsHandler
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.VisualStudio.Services.Maven.Server.Metadata;
using Microsoft.VisualStudio.Services.Maven.Server.Models;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobStore;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Maven.Server.DeleteProcessingJob
{
  public class MavenGetExtraAssetsBlobRefIdsHandler : 
    IConverter<MavenPackageRequestWithMetadata, IEnumerable<BlobReferenceIdentifier>>,
    IHaveInputType<MavenPackageRequestWithMetadata>,
    IHaveOutputType<IEnumerable<BlobReferenceIdentifier>>
  {
    private readonly IConverter<IPackageNameRequest<MavenPackageName, IEnumerable<MavenPackageFileNew>>, IEnumerable<BlobReferenceIdentifier>> fileToBlobReferenceConverter;

    public MavenGetExtraAssetsBlobRefIdsHandler(
      IConverter<IPackageNameRequest<MavenPackageName, IEnumerable<MavenPackageFileNew>>, IEnumerable<BlobReferenceIdentifier>> fileToBlobReferenceConverter)
    {
      this.fileToBlobReferenceConverter = fileToBlobReferenceConverter;
    }

    public IEnumerable<BlobReferenceIdentifier> Convert(MavenPackageRequestWithMetadata request) => this.fileToBlobReferenceConverter.Convert((IPackageNameRequest<MavenPackageName, IEnumerable<MavenPackageFileNew>>) ((IPackageRequest<IPackageIdentity<MavenPackageName, MavenPackageVersion>>) request.Request).ToPackageNameRequest<MavenPackageName, MavenPackageVersion>().WithData<MavenPackageName, IReadOnlyCollection<MavenPackageFileNew>>(request.Metadata.PackageFiles));
  }
}
