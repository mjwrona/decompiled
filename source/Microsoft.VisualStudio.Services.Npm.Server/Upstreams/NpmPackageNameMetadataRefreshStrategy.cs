// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.Upstreams.NpmPackageNameMetadataRefreshStrategy
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Npm.Server.CodeOnly;
using Microsoft.VisualStudio.Services.Npm.Server.CommitLog;
using Microsoft.VisualStudio.Services.Npm.Server.NpmPackageMetadata;
using Microsoft.VisualStudio.Services.Npm.Server.Utils;
using Microsoft.VisualStudio.Services.Npm.WebApi.Exceptions;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Upstream;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamCache.V2;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamFailureTracking.Classifier;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Versioning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.VisualStudio.Services.Npm.Server.Upstreams
{
  public class NpmPackageNameMetadataRefreshStrategy : 
    IUpstreamPackageNameMetadataRefreshStrategy<
    #nullable disable
    NpmPackageName, INpmMetadataEntry>
  {
    private readonly IFactory<UpstreamSource, Task<IUpstreamMetadataService<NpmPackageName, SemanticVersion, NpmPackageIdentity, INpmMetadataEntry>>> upstreamMetadataServiceFactory;
    private readonly ITracerService tracerService;
    private readonly DistTagsUtils distTagsUtils = new DistTagsUtils();

    public NpmPackageNameMetadataRefreshStrategy(
      IFactory<UpstreamSource, Task<IUpstreamMetadataService<NpmPackageName, SemanticVersion, NpmPackageIdentity, INpmMetadataEntry>>> upstreamMetadataServiceFactory,
      ITracerService tracerService)
    {
      this.tracerService = tracerService;
      this.upstreamMetadataServiceFactory = upstreamMetadataServiceFactory;
    }

    public async Task<object> RefreshPackageNameMetadata(
      IFeedRequest feedRequest,
      NpmPackageName packageName,
      IEnumerable<UpstreamSource> upstreams,
      object localNameMetadata,
      IEnumerable<INpmMetadataEntry> availableVersions,
      bool forceUpdate,
      IUpstreamStatusClassifier upstreamStatusClassifer)
    {
      NpmPackageNameMetadataRefreshStrategy sendInTheThisObject = this;
      using (ITracerBlock tracer = sendInTheThisObject.tracerService.Enter((object) sendInTheThisObject, nameof (RefreshPackageNameMetadata)))
      {
        if (upstreams == null || !upstreams.Any<UpstreamSource>())
          return localNameMetadata;
        NpmLocalAndUpstreamSharedPackageMetadata localNpmMetadata = localNameMetadata as NpmLocalAndUpstreamSharedPackageMetadata;
        if (localNameMetadata != localNpmMetadata)
          throw new ArgumentException("Unexpected metadata type", nameof (localNameMetadata));
        IDictionary<string, string> originalUpstreamTags = localNpmMetadata?.UpstreamDistributionTags ?? (IDictionary<string, string>) new Dictionary<string, string>();
        Dictionary<string, string> mergedUpstreamTags = new Dictionary<string, string>();
        IEnumerable<SemanticVersion> unpublishedVersions = availableVersions.Where<INpmMetadataEntry>((Func<INpmMetadataEntry, bool>) (x => x.DeletedDate.HasValue)).Select<INpmMetadataEntry, SemanticVersion>((Func<INpmMetadataEntry, SemanticVersion>) (x => x.PackageIdentity.Version));
        IEnumerable<SemanticVersion> publishedVersions = availableVersions.Where<INpmMetadataEntry>((Func<INpmMetadataEntry, bool>) (x => !x.DeletedDate.HasValue)).Select<INpmMetadataEntry, SemanticVersion>((Func<INpmMetadataEntry, SemanticVersion>) (x => x.PackageIdentity.Version));
        foreach (UpstreamSource upstream in upstreams)
        {
          IUpstreamMetadataService<NpmPackageName, SemanticVersion, NpmPackageIdentity, INpmMetadataEntry> upstreamMetadataService = await sendInTheThisObject.upstreamMetadataServiceFactory.Get(upstream);
          try
          {
            NpmSharedPackageMetadata packageNameMetadata = (NpmSharedPackageMetadata) await upstreamMetadataService.GetPackageNameMetadata(feedRequest, packageName);
            if (packageNameMetadata.DistributionTags != null)
              mergedUpstreamTags = sendInTheThisObject.distTagsUtils.MergeDistTags((IDictionary<string, string>) mergedUpstreamTags, packageNameMetadata.DistributionTags, (IEnumerable<IPackageVersion>) unpublishedVersions, (IEnumerable<IPackageVersion>) publishedVersions);
          }
          catch (Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions.PackageNotFoundException ex)
          {
            tracer.TraceInfo(string.Format("Didn't find any versions of '{0}' on the upstream '{1}'", (object) packageName, (object) upstream.Location));
          }
          catch (Exception ex) when (UnknownUpstreamErrorException.IsUnknown(ex) && !(ex is InvalidUpstreamPackage) && !(ex is OperationCanceledException))
          {
            string message = Microsoft.VisualStudio.Services.Npm.Server.Resources.Error_UpstreamFailureException((object) (string.Format("{{Name = {0}, Location = {1}, Protocol = {2}, UpstreamSourceType = {3} ", (object) upstream.Name, (object) upstream.Location, (object) upstream.Protocol, (object) upstream.UpstreamSourceType) + string.Format("ExceptionType = {0}, ExceptionMessage = {1}}}", (object) ex.GetType(), (object) ex.Message)));
            UpstreamFailureException failureException = upstreamStatusClassifer.Classify((Exception) new UnknownUpstreamErrorException(message, ex), upstream, feedRequest.Feed);
            UpstreamFailureWithUpstreamSourceException upstreamSourceException = new UpstreamFailureWithUpstreamSourceException(message, failureException.InnerException, failureException.ErrorCategory, upstream);
            tracer.TraceException((Exception) upstreamSourceException);
            throw upstreamSourceException;
          }
        }
        if (!forceUpdate && originalUpstreamTags.Count == mergedUpstreamTags.Count && !originalUpstreamTags.Except<KeyValuePair<string, string>>((IEnumerable<KeyValuePair<string, string>>) mergedUpstreamTags).Any<KeyValuePair<string, string>>())
          return localNameMetadata;
        return (object) new NpmLocalAndUpstreamSharedPackageMetadata()
        {
          Revision = Guid.NewGuid().ToString(),
          LocalDistributionTags = (localNpmMetadata?.LocalDistributionTags ?? (IDictionary<string, string>) new Dictionary<string, string>()),
          UpstreamDistributionTags = (IDictionary<string, string>) mergedUpstreamTags
        };
      }
    }
  }
}
