// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.Controllers.MavenMetadataController
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Maven.Server.Aggregations;
using Microsoft.VisualStudio.Services.Maven.Server.Models;
using Microsoft.VisualStudio.Services.Maven.Server.Utilities;
using Microsoft.VisualStudio.Services.Maven.WebApi.Types;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;


#nullable enable
namespace Microsoft.VisualStudio.Services.Maven.Server.Controllers
{
  [ControllerApiVersion(1.0)]
  [VersionedApiControllerCustomName(Area = "maven", ResourceName = "metadata", ResourceVersion = 1)]
  [FeatureEnabled("Packaging.Maven.MetadataController")]
  [ClientIgnore]
  public class MavenMetadataController : MavenBaseController
  {
    [HttpGet]
    [HttpHead]
    [ClientIgnore]
    [ControllerMethodTraceFilter(12090210)]
    public 
    #nullable disable
    IHttpActionResult Index(string feed)
    {
      this.GetFeedRequest(feed);
      return (IHttpActionResult) this.Json(new
      {
        Info = Microsoft.VisualStudio.Services.Maven.Server.Resources.Info_MetadataLandingPage((object) feed, (object) this.TraceArea)
      });
    }

    [HttpGet]
    [HttpHead]
    [ClientIgnore]
    [ClientResponseType(typeof (MavenPackage), null, null)]
    [ControllerMethodTraceFilter(12090200)]
    public async Task<MavenPackage> GetPackageMetadata(
      string feed,
      string groupId,
      string artifactId,
      string version = null,
      string fileName = null,
      bool includePom = false)
    {
      MavenMetadataController metadataController = this;
      IFactory<IFeedRequest, Task<IMavenMetadataDocumentService>> factory = MavenAggregationResolver.Bootstrap(metadataController.TfsRequestContext).FactoryFor<IMavenMetadataDocumentService>();
      IFeedRequest feedRequest = metadataController.GetFeedRequest(feed);
      IFeedRequest input1 = feedRequest;
      IMavenMetadataDocumentService metadataService = await factory.Get(input1);
      Guid feedId = feedRequest.Feed.Id;
      MavenPackageName packageName = new MavenPackageName(groupId, artifactId);
      MavenPackageVersion mavenPackageVersion = string.IsNullOrEmpty(version) ? (MavenPackageVersion) null : new MavenPackageVersion(version);
      if (mavenPackageVersion == null)
      {
        PackageNameQuery<IMavenMetadataEntry> packageNameQueryRequest = new PackageNameQuery<IMavenMetadataEntry>((IPackageNameRequest) new PackageNameRequest<MavenPackageName>(feedRequest, packageName));
        IEnumerable<MavenPackageVersion> versions = (await metadataService.GetPackageVersionStatesAsync(packageNameQueryRequest) ?? throw ExceptionHelper.PackageNotFound(Microsoft.VisualStudio.Services.Maven.Server.Resources.Error_ArtifactHasNoVersions((object) packageName.DisplayName, (object) feedId))).Where<IMavenMetadataEntry>((Func<IMavenMetadataEntry, bool>) (m => m.PackageIdentity != null)).Select<IMavenMetadataEntry, MavenPackageVersion>((Func<IMavenMetadataEntry, MavenPackageVersion>) (m => m.PackageIdentity.Version));
        return MavenMetadataController.GetMavenPackage(metadataController.TfsRequestContext, feedRequest.Feed, packageName, versions);
      }
      IConverter<IRawPackageRequest, PackageRequest<MavenPackageIdentity>> converter = new MavenRawPackageRequestToRequestConverterBootstrapper(metadataController.TfsRequestContext).Bootstrap();
      IAsyncHandler<IPackageRequest<MavenPackageIdentity>, IMavenMetadataEntry> pointQueryHandler = metadataService.ToPointQueryHandler<MavenPackageIdentity, IMavenMetadataEntry>();
      MavenRawPackageRequest input2 = new MavenRawPackageRequest(feedRequest, groupId, artifactId, mavenPackageVersion.NormalizedVersion);
      PackageRequest<MavenPackageIdentity> request = converter.Convert((IRawPackageRequest) input2);
      PackageRequest<MavenPackageIdentity> request1 = request;
      return (await pointQueryHandler.Handle((IPackageRequest<MavenPackageIdentity>) request1) ?? throw ExceptionHelper.PackageNotFound((IPackageIdentity) request.PackageId, feedRequest.Feed)).ToPackage(metadataController.TfsRequestContext, feedRequest.Feed, fileName, includePom);
    }

    private static MavenPackage GetMavenPackage(
      IVssRequestContext requestContext,
      FeedCore feed,
      MavenPackageName packageName,
      IEnumerable<MavenPackageVersion> versions)
    {
      MavenPackage mavenPackage = new MavenPackage(packageName.GroupId, packageName.ArtifactId, (string) null)
      {
        ArtifactMetadata = MavenUrlUtility.GetUrlForMetadataFile(requestContext, feed, packageName)
      };
      if (versions != null)
      {
        foreach (MavenPackageVersion version in versions)
          mavenPackage.Versions.AddLink(version.DisplayVersion, MavenUrlUtility.GetUrlForIndex(requestContext, feed, packageName, version).Href);
      }
      return mavenPackage;
    }
  }
}
