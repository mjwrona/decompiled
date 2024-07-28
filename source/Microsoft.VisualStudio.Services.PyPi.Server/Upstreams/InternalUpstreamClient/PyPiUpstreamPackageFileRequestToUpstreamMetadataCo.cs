// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.Upstreams.InternalUpstreamClient.PyPiUpstreamPackageFileRequestToUpstreamMetadataConverter
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using Microsoft.VisualStudio.Services.PyPi.Client.Internal;
using Microsoft.VisualStudio.Services.PyPi.Server.Metadata;
using Microsoft.VisualStudio.Services.PyPi.Server.PackageIdentity;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.VisualStudio.Services.PyPi.Server.Upstreams.InternalUpstreamClient
{
  public class PyPiUpstreamPackageFileRequestToUpstreamMetadataConverter : 
    IConverter<
    #nullable disable
    IPackageFileRequest<PyPiPackageIdentity>, Task<PyPiInternalUpstreamMetadata>>,
    IHaveInputType<IPackageFileRequest<PyPiPackageIdentity>>,
    IHaveOutputType<Task<PyPiInternalUpstreamMetadata>>
  {
    private readonly IMetadataDocumentService<PyPiPackageIdentity, IPyPiMetadataEntryWithRawMetadata> metadataDocService;

    public PyPiUpstreamPackageFileRequestToUpstreamMetadataConverter(
      IMetadataDocumentService<PyPiPackageIdentity, IPyPiMetadataEntryWithRawMetadata> metadataDocService)
    {
      this.metadataDocService = metadataDocService;
    }

    public async Task<PyPiInternalUpstreamMetadata> Convert(
      IPackageFileRequest<PyPiPackageIdentity> packageFileRequest)
    {
      IPyPiMetadataEntryWithRawMetadata versionStateAsync = await this.metadataDocService.GetPackageVersionStateAsync((IPackageRequest<PyPiPackageIdentity>) packageFileRequest);
      if (versionStateAsync == null || versionStateAsync.IsDeleted())
        return (PyPiInternalUpstreamMetadata) null;
      PyPiPackageFileWithRawMetadata packageFileWithPath = versionStateAsync.GetPackageFileWithPath(packageFileRequest.FilePath);
      IReadOnlyDictionary<string, string[]> rawMetadata = packageFileWithPath?.RawMetadata;
      Dictionary<string, string[]> collection = (Dictionary<string, string[]>) null;
      if (rawMetadata != null)
      {
        collection = new Dictionary<string, string[]>(rawMetadata.Count);
        collection.AddRange<KeyValuePair<string, string[]>, Dictionary<string, string[]>>((IEnumerable<KeyValuePair<string, string[]>>) rawMetadata);
        collection["name"] = new string[1]
        {
          versionStateAsync.PackageIdentity.Name.DisplayName
        };
        collection["version"] = new string[1]
        {
          versionStateAsync.PackageIdentity.Version.DisplayVersion
        };
        if (versionStateAsync.RequiresPython != null)
          collection["requires_python"] = new string[1]
          {
            versionStateAsync.RequiresPython.ToString()
          };
        else
          collection.Remove("requires_python");
      }
      return new PyPiInternalUpstreamMetadata()
      {
        RawFileMetadata = (IReadOnlyDictionary<string, string[]>) collection,
        Base64ZippedGpgSignature = packageFileWithPath?.GpgSignature?.AsDeflatedBase64String(),
        SourceChain = versionStateAsync.SourceChain.ToList<UpstreamSourceInfo>()
      };
    }
  }
}
