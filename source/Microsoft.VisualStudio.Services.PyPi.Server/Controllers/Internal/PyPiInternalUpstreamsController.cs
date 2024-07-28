// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.Controllers.Internal.PyPiInternalUpstreamsController
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.CrossProtocol.Internal.WebApi.Types;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using Microsoft.VisualStudio.Services.PyPi.Client.Internal;
using Microsoft.VisualStudio.Services.PyPi.Server.Converters;
using Microsoft.VisualStudio.Services.PyPi.Server.PackageIdentity;
using Microsoft.VisualStudio.Services.PyPi.Server.Upstreams.InternalUpstreamClient;
using Microsoft.VisualStudio.Services.PyPi.WebApi.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.PyPi.Server.Controllers.Internal
{
  [ControllerApiVersion(5.0)]
  [VersionedApiControllerCustomName(Area = "pypi", ResourceName = "pypiInternalUpstreams", ResourceVersion = 1)]
  public class PyPiInternalUpstreamsController : PyPiApiController
  {
    [HttpPost]
    [ClientLocationId("5D924882-AFBB-47E1-A7B0-264B1D07F1AE")]
    [ClientResponseType(typeof (LimitedPyPiMetadataResponse), null, null)]
    public async Task<LimitedPyPiMetadataResponse> GetLimitedMetadata(
      string feedId,
      string packageName,
      [FromBody] List<string> versions,
      Guid aadTenantId)
    {
      PyPiInternalUpstreamsController upstreamsController = this;
      IFeedRequest feedRequest = upstreamsController.GetFeedRequest(feedId);
      PyPiIdentityResolver instance = PyPiIdentityResolver.Instance;
      PyPiPackageName packageName1 = instance.ResolvePackageName(packageName);
      IEnumerable<PyPiPackageVersion> versions1 = versions.Select<string, PyPiPackageVersion>(new Func<string, PyPiPackageVersion>(((IdentityResolverBase<PyPiPackageName, PyPiPackageVersion, PyPiPackageIdentity, SimplePackageFileName>) instance).ResolvePackageVersion));
      new UpstreamVerificationHelperBootstrapper(upstreamsController.TfsRequestContext).Bootstrap().ThrowIfFeedIsNotWidelyVisible(upstreamsController.TfsRequestContext, feedRequest.Feed, aadTenantId);
      return new LimitedPyPiMetadataResponseConverter().Convert(await PyPiInternalUpstreamsController.GetUpstreamsHelper(upstreamsController.TfsRequestContext, feedId, feedRequest.Feed.Project?.Id).GetLimitedMetadataList(packageName1, versions1));
    }

    [HttpGet]
    [ClientLocationId("BA32174F-B9B2-4F0A-8F3E-7BC61E7036B1")]
    [ClientResponseType(typeof (VersionsExposedToDownstreamsResponse), null, null)]
    public async Task<VersionsExposedToDownstreamsResponse> GetPackageVersionsExposedToDownstreams(
      string feedId,
      string packageName,
      Guid aadTenantId)
    {
      PyPiInternalUpstreamsController upstreamsController = this;
      IFeedRequest feedRequest = upstreamsController.GetFeedRequest(feedId);
      PyPiPackageName packageName1 = PyPiIdentityResolver.Instance.ResolvePackageName(packageName);
      new UpstreamVerificationHelperBootstrapper(upstreamsController.TfsRequestContext).Bootstrap().ThrowIfFeedIsNotWidelyVisible(upstreamsController.TfsRequestContext, feedRequest.Feed, aadTenantId);
      IReadOnlyList<VersionWithSourceChain<PyPiPackageVersion>> packageVersions = await PyPiInternalUpstreamsController.GetUpstreamsHelper(upstreamsController.TfsRequestContext, feedId, feedRequest.Feed.Project?.Id).GetPackageVersions(packageName1);
      return new VersionsExposedToDownstreamsResponse()
      {
        Versions = (IReadOnlyList<string>) packageVersions.Select<VersionWithSourceChain<PyPiPackageVersion>, string>((Func<VersionWithSourceChain<PyPiPackageVersion>, string>) (x => x.Version.NormalizedVersion)).ToList<string>(),
        VersionInfo = (IReadOnlyList<RawVersionWithSourceChain>) packageVersions.Select<VersionWithSourceChain<PyPiPackageVersion>, RawVersionWithSourceChain>((Func<VersionWithSourceChain<PyPiPackageVersion>, RawVersionWithSourceChain>) (x => new RawVersionWithSourceChain()
        {
          NormalizedVersion = x.Version.NormalizedVersion,
          SourceChain = (IEnumerable<UpstreamSourceInfo>) x.SourceChain
        })).ToList<RawVersionWithSourceChain>()
      };
    }

    [HttpGet]
    [ClientLocationId("05B4B86D-3851-4E6E-B447-681B8E4C10C8")]
    [ClientResponseType(typeof (PyPiInternalUpstreamMetadata), null, null)]
    public async Task<PyPiInternalUpstreamMetadata> GetUpstreamMetadata(
      string feedId,
      string packageName,
      string packageVersion,
      string fileName,
      Guid aadTenantId)
    {
      PyPiInternalUpstreamsController upstreamsController = this;
      IFeedRequest feedRequest = upstreamsController.GetFeedRequest(feedId);
      PyPiPackageIdentity packageIdentity = PyPiIdentityResolver.Instance.ResolvePackageIdentity(packageName, packageVersion);
      new UpstreamVerificationHelperBootstrapper(upstreamsController.TfsRequestContext).Bootstrap().ThrowIfFeedIsNotWidelyVisible(upstreamsController.TfsRequestContext, feedRequest.Feed, aadTenantId);
      return await PyPiInternalUpstreamsController.GetUpstreamsHelper(upstreamsController.TfsRequestContext, feedId, feedRequest.Feed.Project?.Id).GetUpstreamMetadata(packageIdentity, fileName);
    }

    private static PyPiInternalUpstreamHelper GetUpstreamsHelper(
      IVssRequestContext requestContext,
      string feedId,
      Guid? projectId)
    {
      return new PyPiInternalUpstreamHelper(requestContext, new FeedServiceFacade(requestContext), requestContext.RequestUri(), feedId, projectId);
    }
  }
}
