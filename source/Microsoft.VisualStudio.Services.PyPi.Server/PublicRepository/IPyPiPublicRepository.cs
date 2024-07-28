// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.PublicRepository.IPyPiPublicRepository
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PublicRepositories;
using Microsoft.VisualStudio.Services.PyPi.Server.PackageIdentity;
using Microsoft.VisualStudio.Services.PyPi.Server.Upstreams;
using System.IO;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.VisualStudio.Services.PyPi.Server.PublicRepository
{
  public interface IPyPiPublicRepository : 
    IPublicRepository<IUpstreamPyPiClient>,
    IPublicRepository,
    IPublicRepositoryWithCursorAssistedInvalidation<PyPiPackageName, PyPiPackageVersion, PyPiChangelogCursor>
  {
    IPublicUpstreamPyPiClient DirectClient { get; }

    IPublicRepositoryInterestTracker<PyPiPackageName> InterestTracker { get; }

    Task<PyPiPubCachePackageNameFile> GetPackageMetadataAsync(PyPiPackageName packageName);

    Task<Stream> GetFileAsync(PyPiPackageIdentity packageIdentity, string filePath);

    Task<Stream?> GetGpgSignatureForFileAsync(PyPiPackageIdentity packageIdentity, string filePath);

    Task<PyPiUpstreamMetadata> GetUpstreamMetadataAsync(
      PyPiPackageIdentity packageIdentity,
      string filePath);
  }
}
