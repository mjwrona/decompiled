// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.MavenPackageMetricsHandler
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.VisualStudio.Services.Maven.Server.Metadata;
using Microsoft.VisualStudio.Services.Maven.Server.Models;
using Microsoft.VisualStudio.Services.Maven.Server.Utilities;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Maven.Server
{
  public class MavenPackageMetricsHandler : 
    IAsyncHandler<MavenFileRequest>,
    IAsyncHandler<MavenFileRequest, NullResult>,
    IHaveInputType<MavenFileRequest>,
    IHaveOutputType<NullResult>
  {
    private readonly IConverter<MavenFileRequest, MavenArtifactFileRequest> mavenFileRequestConverter;
    private readonly IFactory<IPackageRequest<MavenPackageIdentity>, Task<IMavenMetadataEntry>> metadataFactory;
    private readonly IFactory<MavenArtifactFileRequest, IPackageMetricsServiceFacade> packageMetricsServiceFactory;

    public MavenPackageMetricsHandler(
      IConverter<MavenFileRequest, MavenArtifactFileRequest> mavenFileRequestConverter,
      IFactory<IPackageRequest<MavenPackageIdentity>, Task<IMavenMetadataEntry>> metadataFactory,
      IFactory<MavenArtifactFileRequest, IPackageMetricsServiceFacade> packageMetricsServiceFactory)
    {
      this.mavenFileRequestConverter = mavenFileRequestConverter;
      this.metadataFactory = metadataFactory;
      this.packageMetricsServiceFactory = packageMetricsServiceFactory;
    }

    public async Task<NullResult> Handle(MavenFileRequest request)
    {
      MavenArtifactFileRequest artifactFileRequest = this.mavenFileRequestConverter.Convert(request);
      IMavenMetadataEntry mavenMetadataEntry = await this.metadataFactory.Get((IPackageRequest<MavenPackageIdentity>) artifactFileRequest);
      if (mavenMetadataEntry == null)
        return (NullResult) null;
      int num = mavenMetadataEntry.PackageFiles.Count<MavenPackageFileNew>((Func<MavenPackageFileNew, bool>) (file => file.StorageId != null && !MavenFileNameUtility.IsChecksumFile(file.Path)));
      if (num == 0)
        return (NullResult) null;
      double downloadCount = 1.0 / (double) num;
      await this.packageMetricsServiceFactory.Get(artifactFileRequest).UpdatePackageMetricsAsync(artifactFileRequest.Feed.Id, artifactFileRequest.Feed.Project?.Id, artifactFileRequest.Protocol, (IPackageIdentity) artifactFileRequest.PackageId, downloadCount);
      NullResult nullResult;
      return nullResult;
    }
  }
}
