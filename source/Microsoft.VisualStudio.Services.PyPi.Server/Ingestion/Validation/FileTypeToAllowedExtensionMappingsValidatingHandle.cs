// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.Ingestion.Validation.FileTypeToAllowedExtensionMappingsValidatingHandler
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Ingestion;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions;
using Microsoft.VisualStudio.Services.PyPi.Server.Metadata;
using Microsoft.VisualStudio.Services.PyPi.Server.PackageIdentity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.PyPi.Server.Ingestion.Validation
{
  public class FileTypeToAllowedExtensionMappingsValidatingHandler : 
    IAsyncHandler<IStorablePackageInfo<PyPiPackageIdentity, PyPiUploadedPackageMetadata>>,
    IAsyncHandler<IStorablePackageInfo<PyPiPackageIdentity, PyPiUploadedPackageMetadata>, NullResult>,
    IHaveInputType<IStorablePackageInfo<PyPiPackageIdentity, PyPiUploadedPackageMetadata>>,
    IHaveOutputType<NullResult>
  {
    private static readonly IDictionary<PyPiDistType, string[]> AllowedExtensions = (IDictionary<PyPiDistType, string[]>) new Dictionary<PyPiDistType, string[]>()
    {
      {
        PyPiDistType.sdist,
        new string[2]{ ".tar.gz", ".zip" }
      },
      {
        PyPiDistType.bdist_egg,
        new string[1]{ ".egg" }
      },
      {
        PyPiDistType.bdist_wheel,
        new string[1]{ ".whl" }
      }
    };
    private static readonly IDictionary<PyPiDistType, string[]> AllowedExtensionsForUpstreams = (IDictionary<PyPiDistType, string[]>) new Dictionary<PyPiDistType, string[]>()
    {
      {
        PyPiDistType.sdist,
        new string[1]{ ".tar.bz2" }
      }
    };

    public Task<NullResult> Handle(
      IStorablePackageInfo<PyPiPackageIdentity, PyPiUploadedPackageMetadata> request)
    {
      PyPiDistType distType = request.ProtocolSpecificInfo.Metadata.DistType;
      int num = request.IngestionDirection == IngestionDirection.PullFromUpstream ? 1 : 0;
      if (!(num != 0 ? FileTypeToAllowedExtensionMappingsValidatingHandler.AllowedExtensions.Keys.Concat<PyPiDistType>((IEnumerable<PyPiDistType>) FileTypeToAllowedExtensionMappingsValidatingHandler.AllowedExtensionsForUpstreams.Keys).ToHashSet<PyPiDistType>() : FileTypeToAllowedExtensionMappingsValidatingHandler.AllowedExtensions.Keys.ToHashSet<PyPiDistType>()).Contains(distType))
        throw new InvalidPackageException(Resources.Error_NonIngestableDistributionType((object) distType));
      string filePath = request.ProtocolSpecificInfo.PackageFileStream.FilePath;
      string[] source = num != 0 ? (FileTypeToAllowedExtensionMappingsValidatingHandler.AllowedExtensionsForUpstreams.ContainsKey(distType) ? ((IEnumerable<string>) FileTypeToAllowedExtensionMappingsValidatingHandler.AllowedExtensions[distType]).Concat<string>((IEnumerable<string>) FileTypeToAllowedExtensionMappingsValidatingHandler.AllowedExtensionsForUpstreams[distType]).ToArray<string>() : FileTypeToAllowedExtensionMappingsValidatingHandler.AllowedExtensions[distType]) : FileTypeToAllowedExtensionMappingsValidatingHandler.AllowedExtensions[distType];
      if (((IEnumerable<string>) source).Any<string>((Func<string, bool>) (extension => filePath.EndsWith(extension, StringComparison.OrdinalIgnoreCase))))
        return Task.FromResult<NullResult>((NullResult) null);
      throw new InvalidPackageException(Resources.Error_NonIngestableExtension((object) filePath, (object) distType, (object) string.Join(",", source)));
    }
  }
}
