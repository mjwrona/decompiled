// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.PackageIngestion.IPackageIngestionService
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.NuGet.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.NuGet.Server.PackageIngestion
{
  [DefaultServiceImplementation(typeof (NuGetPackageIngestionService))]
  public interface IPackageIngestionService : IVssFrameworkService
  {
    Task AddPackageFromStreamAsync(
      IVssRequestContext requestContext,
      IFeedRequest feedRequest,
      Stream nupkgStream,
      string protocolVersion,
      IEnumerable<UpstreamSourceInfo> sourceChain,
      bool addAsDelisted,
      VssNuGetPackageIdentity expectedIdentity = null);

    Task AddPackageFromBlobAsync(
      IVssRequestContext requestContext,
      IFeedRequest feedRequest,
      BlobIdentifierWithOrWithoutBlocks blobIdWithOrWithoutBlocks,
      string protocolVersion,
      IEnumerable<UpstreamSourceInfo> sourceChain,
      bool addAsDelisted,
      VssNuGetPackageIdentity expectedIdentity = null);

    Task AddPackageFromDropAsync(
      IVssRequestContext requestContext,
      IFeedRequest feedRequest,
      string dropName,
      string protocolVersion);
  }
}
