// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.Controllers.PyPiDownloadGpgSignatureHandler
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions;
using Microsoft.VisualStudio.Services.PyPi.Server.Metadata;
using Microsoft.VisualStudio.Services.PyPi.Server.PackageIdentity;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.VisualStudio.Services.PyPi.Server.Controllers
{
  public class PyPiDownloadGpgSignatureHandler : 
    IAsyncHandler<
    #nullable disable
    PackageFileRequest<PyPiPackageIdentity>, HttpResponseMessage>,
    IHaveInputType<PackageFileRequest<PyPiPackageIdentity>>,
    IHaveOutputType<HttpResponseMessage>
  {
    public static readonly string GpgFileNameSuffix = ".asc";
    private readonly IReadMetadataDocumentService<PyPiPackageIdentity, IPyPiMetadataEntryWithRawMetadata> metadataStore;

    public PyPiDownloadGpgSignatureHandler(
      IReadMetadataDocumentService<PyPiPackageIdentity, IPyPiMetadataEntryWithRawMetadata> metadataStore)
    {
      this.metadataStore = metadataStore;
    }

    public async Task<HttpResponseMessage> Handle(PackageFileRequest<PyPiPackageIdentity> request)
    {
      if (!request.FilePath.EndsWith(PyPiDownloadGpgSignatureHandler.GpgFileNameSuffix, StringComparison.OrdinalIgnoreCase))
        throw new InvalidOperationException("GPG signature file name must end in .asc");
      string baseFilePath = request.FilePath.Substring(0, request.FilePath.Length - PyPiDownloadGpgSignatureHandler.GpgFileNameSuffix.Length);
      PyPiPackageFileWithRawMetadata packageFileWithPath = (await this.metadataStore.GetPackageVersionStateAsync((IPackageRequest<PyPiPackageIdentity>) request)).GetPackageFileWithPath(baseFilePath);
      DeflateCompressibleBytes compressibleBytes = packageFileWithPath != null ? packageFileWithPath.GpgSignature : throw new PackageNotFoundException(Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_PackageFileNotFound((object) request.Feed.FullyQualifiedName, (object) request.PackageId, (object) request.FilePath));
      if (compressibleBytes == null)
      {
        if (packageFileWithPath.StorageId.IsLocal)
          throw new PackageNotFoundException(Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_PackageFileNotFound((object) request.Feed.FullyQualifiedName, (object) request.PackageId, (object) request.FilePath));
        throw new PackageNotFoundException(Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_PackageFileNotFoundTryIngesting((object) request.Feed.FullyQualifiedName, (object) request.PackageId, (object) request.FilePath, (object) baseFilePath));
      }
      HttpResponseMessage httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK)
      {
        Content = (HttpContent) new ByteArrayContent(compressibleBytes.AsUncompressedBytes())
      };
      baseFilePath = (string) null;
      return httpResponseMessage;
    }

    public static bool IsSignatureFile(string fileName) => fileName.EndsWith(PyPiDownloadGpgSignatureHandler.GpgFileNameSuffix, StringComparison.OrdinalIgnoreCase);
  }
}
