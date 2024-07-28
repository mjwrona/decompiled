// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.Upstreams.IUpstreamPyPiClient
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.PyPi.Server.PackageIdentity;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.VisualStudio.Services.PyPi.Server.Upstreams
{
  public interface IUpstreamPyPiClient
  {
    Task<Stream> GetFile(
      IFeedRequest downstreamFeedRequest,
      PyPiPackageIdentity packageIdentity,
      string filePath);

    Task<Stream?> GetGpgSignatureForFile(
      IFeedRequest downstreamFeedRequest,
      PyPiPackageIdentity packageIdentity,
      string filePath);

    Task<PyPiUpstreamMetadata> GetUpstreamMetadata(
      IFeedRequest downstreamFeedRequest,
      PyPiPackageIdentity packageIdentity,
      string requestFilePath);

    Task<IReadOnlyList<VersionWithSourceChain<PyPiPackageVersion>>> GetPackageVersions(
      IFeedRequest downstreamFeedRequest,
      PyPiPackageName packageName);

    Task<IEnumerable<LimitedPyPiMetadata>> GetLimitedMetadataList(
      PyPiPackageName packageName,
      IEnumerable<PyPiPackageVersion> versions);
  }
}
