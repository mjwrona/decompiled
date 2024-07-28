// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.Ingestion.MavenStoredPackageGenerator
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.Maven.Server.Models;
using Microsoft.VisualStudio.Services.Maven.Server.Utilities;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Ingestion;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Ingestion.CentralFeedServices;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System.IO;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Maven.Server.Ingestion
{
  public class MavenStoredPackageGenerator : 
    IAsyncHandler<PackageIngestionRequest<MavenPackageIdentity, MavenPackageFileInfo>, MavenStreamStorablePackageInfo>,
    IHaveInputType<PackageIngestionRequest<MavenPackageIdentity, MavenPackageFileInfo>>,
    IHaveOutputType<MavenStreamStorablePackageInfo>
  {
    private readonly IAsyncHandler<long> packageSizeValidatingHandler;
    private readonly IAsyncHandler<MavenPackageFileInfo, NullResult> validatingHandler;
    private readonly ITerrapinIngestionValidator terrapinValidator;

    public MavenStoredPackageGenerator(
      IAsyncHandler<long> packageSizeValidatingHandler,
      IAsyncHandler<MavenPackageFileInfo, NullResult> validatingHandler,
      ITerrapinIngestionValidator terrapinValidator)
    {
      this.packageSizeValidatingHandler = packageSizeValidatingHandler;
      this.validatingHandler = validatingHandler;
      this.terrapinValidator = terrapinValidator;
    }

    public async Task<MavenStreamStorablePackageInfo> Handle(
      PackageIngestionRequest<MavenPackageIdentity, MavenPackageFileInfo> request)
    {
      Stream packageFileStream = request.PackageContents.Stream;
      long packageSize = packageFileStream.Length;
      NullResult nullResult1 = await this.packageSizeValidatingHandler.Handle(packageSize);
      NullResult nullResult2 = await this.validatingHandler.Handle(request.PackageContents);
      MavenPackageIdentity mavenPackageIdentity = new MavenPackageIdentity(request.PackageContents.FilePath.PackageName, request.PackageContents.FilePath.PackageVersion);
      MavenStreamStorablePackageInfo result = new MavenStreamStorablePackageInfo(request.Feed, mavenPackageIdentity, request.PackageContents, packageSize)
      {
        SourceChain = request.SourceChain
      };
      if (MavenFileNameUtility.IsPomFile(result.ProtocolSpecificInfo.FilePath.FileName))
        result.ProtocolSpecificInfo.IsPomFile = true;
      result.PackageStorageId = MavenStoredPackageGenerator.CalculateBlobStorageId(packageFileStream);
      await this.terrapinValidator.ValidateAsync((IPackageRequest) request.WithPackage<MavenPackageIdentity>(mavenPackageIdentity), (IPackageFileName) request.PackageContents.FilePath, request.SourceChain);
      MavenStreamStorablePackageInfo storablePackageInfo = result;
      packageFileStream = (Stream) null;
      result = (MavenStreamStorablePackageInfo) null;
      return storablePackageInfo;
    }

    private static BlobStorageId CalculateBlobStorageId(Stream packageContentStream)
    {
      packageContentStream.Seek(0L, SeekOrigin.Begin);
      BlobStorageId blobStorageId = new BlobStorageId(packageContentStream.CalculateBlobIdentifier((IBlobHasher) VsoHash.Instance));
      packageContentStream.Seek(0L, SeekOrigin.Begin);
      return blobStorageId;
    }
  }
}
