// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.Upstreams.InternalUpstreamClient.SameCollectionUpstreamPyPiClient
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Upstream;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using Microsoft.VisualStudio.Services.PyPi.Client.Internal;
using Microsoft.VisualStudio.Services.PyPi.Server.PackageIdentity;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.PyPi.Server.Upstreams.InternalUpstreamClient
{
  public class SameCollectionUpstreamPyPiClient : IUpstreamPyPiClient
  {
    private readonly IVssRequestContext collectionContext;
    private readonly FeedServiceFacade feedServiceFacade;
    private readonly UpstreamSource upstreamSource;

    public SameCollectionUpstreamPyPiClient(
      IVssRequestContext collectionContext,
      FeedServiceFacade feedServiceFacade,
      UpstreamSource upstreamSource)
    {
      this.collectionContext = collectionContext;
      this.feedServiceFacade = feedServiceFacade;
      this.upstreamSource = upstreamSource;
    }

    public async Task<Stream> GetFile(
      IFeedRequest downstreamFeedRequest,
      PyPiPackageIdentity packageIdentity,
      string filePath)
    {
      return await this.GetUpstreamsHelper().GetFile(packageIdentity, filePath);
    }

    public async Task<Stream> GetGpgSignatureForFile(
      IFeedRequest downstreamFeedRequest,
      PyPiPackageIdentity packageIdentity,
      string filePath)
    {
      return await this.GetUpstreamsHelper().GetGpgSignatureForFile(packageIdentity, filePath);
    }

    public async Task<IEnumerable<LimitedPyPiMetadata>> GetLimitedMetadataList(
      PyPiPackageName packageName,
      IEnumerable<PyPiPackageVersion> versions)
    {
      return await this.GetUpstreamsHelper().GetLimitedMetadataList(packageName, versions);
    }

    public async Task<IReadOnlyList<VersionWithSourceChain<PyPiPackageVersion>>> GetPackageVersions(
      IFeedRequest _,
      PyPiPackageName packageName)
    {
      return await this.GetUpstreamsHelper().GetPackageVersions(packageName);
    }

    public async Task<PyPiUpstreamMetadata> GetUpstreamMetadata(
      IFeedRequest downstreamFeedRequest,
      PyPiPackageIdentity packageIdentity,
      string requestFilePath)
    {
      PyPiInternalUpstreamMetadata upstreamMetadata = await this.GetUpstreamsHelper().GetUpstreamMetadata(packageIdentity, requestFilePath);
      return new PyPiUpstreamMetadata(upstreamMetadata.RawFileMetadata, upstreamMetadata.SourceChain.ToImmutableArray<UpstreamSourceInfo>());
    }

    private PyPiInternalUpstreamHelper GetUpstreamsHelper() => new PyPiInternalUpstreamHelper(this.collectionContext, this.feedServiceFacade, new Uri(this.upstreamSource.Location), this.upstreamSource.GetFullyQualifiedFeedId(), this.upstreamSource.GetProjectId());
  }
}
