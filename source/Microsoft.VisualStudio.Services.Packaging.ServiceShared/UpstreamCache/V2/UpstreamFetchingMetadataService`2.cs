// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamCache.V2.UpstreamFetchingMetadataService`2
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.CrossProtocol.Internal.WebApi.Types;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Settings;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Upstream;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.ValidationUtils;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamCache.V2
{
  public class UpstreamFetchingMetadataService<TPackageIdentity, TMetadataEntry> : 
    IReadMetadataDocumentService<
    #nullable disable
    TPackageIdentity, TMetadataEntry>,
    IReadMetadataService<
    #nullable enable
    TPackageIdentity, TMetadataEntry>,
    IReadSingleVersionMetadataService<TPackageIdentity, TMetadataEntry>,
    IReadMetadataDocumentService
    where TPackageIdentity : 
    #nullable disable
    IPackageIdentity
    where TMetadataEntry : class, IMetadataEntry<TPackageIdentity>
  {
    private readonly IReadMetadataDocumentService<TPackageIdentity, TMetadataEntry> innerMetadataService;
    private readonly IValidator<IPackageNameRequest> packageNameValidator;
    private readonly IUpstreamEntriesValidChecker upstreamEntriesStillValidChecker;
    private readonly IPendingAggBootstrapper<IUpstreamMetadataManager> upstreamMetadataManagerBootstrapper;
    private readonly IAsyncInvokerWithAggRebootstrapping<InlineRefreshKey, RefreshPackageResult, IUpstreamMetadataManager> inlineRefreshInvoker;
    private readonly IUpstreamsConfigurationHasher upstreamsConfigurationHasher;
    private readonly ICancellationFacade cancellation;
    private readonly ITracerService tracerService;
    private readonly IHandler<IFeedRequest, IEnumerable<UpstreamSource>> upstreamsFromFeedHandler;
    private readonly IFeedPerms feedPermsFacade;
    private readonly IExecutionEnvironment executionEnvironment;
    private readonly IOrgLevelPackagingSetting<bool> allowUpstreamsForPublicFeedsSettings;
    private readonly IOrgLevelPackagingSetting<bool> allowUpstreamsForPublicFeedsMSFTSettings;
    private readonly bool isFromExplicitUserAction;

    public UpstreamFetchingMetadataService(
      IReadMetadataDocumentService<TPackageIdentity, TMetadataEntry> innerMetadataService,
      IValidator<IPackageNameRequest> packageNameValidator,
      IHandler<IFeedRequest, IEnumerable<UpstreamSource>> upstreamsFromFeedHandler,
      IUpstreamEntriesValidChecker upstreamEntriesStillValidChecker,
      IPendingAggBootstrapper<IUpstreamMetadataManager> upstreamMetadataManagerBootstrapper,
      IAsyncInvokerWithAggRebootstrapping<InlineRefreshKey, RefreshPackageResult, IUpstreamMetadataManager> inlineRefreshInvoker,
      IUpstreamsConfigurationHasher upstreamsConfigurationHasher,
      ICancellationFacade cancellation,
      IFeedPerms feedPermsFacade,
      IExecutionEnvironment executionEnvironment,
      ITracerService tracerService,
      IOrgLevelPackagingSetting<bool> allowUpstreamsForPublicFeedsSettings,
      IOrgLevelPackagingSetting<bool> allowUpstreamsForPublicFeedsMSFTSettings,
      bool isFromExplicitUserAction)
    {
      this.innerMetadataService = innerMetadataService;
      this.packageNameValidator = packageNameValidator;
      this.upstreamsFromFeedHandler = upstreamsFromFeedHandler;
      this.upstreamEntriesStillValidChecker = upstreamEntriesStillValidChecker;
      this.upstreamMetadataManagerBootstrapper = upstreamMetadataManagerBootstrapper;
      this.inlineRefreshInvoker = inlineRefreshInvoker;
      this.upstreamsConfigurationHasher = upstreamsConfigurationHasher;
      this.cancellation = cancellation;
      this.feedPermsFacade = feedPermsFacade;
      this.executionEnvironment = executionEnvironment;
      this.tracerService = tracerService;
      this.allowUpstreamsForPublicFeedsSettings = allowUpstreamsForPublicFeedsSettings;
      this.allowUpstreamsForPublicFeedsMSFTSettings = allowUpstreamsForPublicFeedsMSFTSettings;
      this.isFromExplicitUserAction = isFromExplicitUserAction;
    }

    public async Task<TMetadataEntry> GetPackageVersionStateAsync(
      IPackageRequest<TPackageIdentity> packageRequest)
    {
      UpstreamFetchingMetadataService<TPackageIdentity, TMetadataEntry> sendInTheThisObject = this;
      using (ITracerBlock tracer = sendInTheThisObject.tracerService.Enter((object) sendInTheThisObject, nameof (GetPackageVersionStateAsync)))
      {
        PackageNameQuery<TMetadataEntry> packageNameQueryRequest = packageRequest.ToSingleVersionPackageNameQuery<TPackageIdentity, TMetadataEntry>();
        sendInTheThisObject.packageNameValidator.Validate(packageNameQueryRequest.RequestData);
        MetadataDocument<TMetadataEntry> statesDocumentAsync = await sendInTheThisObject.innerMetadataService.GetPackageVersionStatesDocumentAsync(packageNameQueryRequest);
        TMetadataEntry metadataEntry = statesDocumentAsync != null ? statesDocumentAsync.Entries.FirstOrDefault<TMetadataEntry>() : default (TMetadataEntry);
        if ((object) metadataEntry != null && metadataEntry.IsLocal)
          return statesDocumentAsync.Entries.First<TMetadataEntry>();
        MetadataDocument<TMetadataEntry> metadataDocument = await sendInTheThisObject.RefreshIfNecessary(packageNameQueryRequest, statesDocumentAsync, tracer);
        TMetadataEntry versionStateAsync;
        if (metadataDocument == null)
        {
          versionStateAsync = default (TMetadataEntry);
        }
        else
        {
          List<TMetadataEntry> entries = metadataDocument.Entries;
          versionStateAsync = entries != null ? entries.FirstOrDefault<TMetadataEntry>() : default (TMetadataEntry);
        }
        return versionStateAsync;
      }
    }

    public async Task<List<TMetadataEntry>> GetPackageVersionStatesAsync(
      PackageNameQuery<TMetadataEntry> packageNameQueryRequest)
    {
      UpstreamFetchingMetadataService<TPackageIdentity, TMetadataEntry> sendInTheThisObject = this;
      List<TMetadataEntry> versionStatesAsync;
      using (ITracerBlock tracer = sendInTheThisObject.tracerService.Enter((object) sendInTheThisObject, nameof (GetPackageVersionStatesAsync)))
      {
        sendInTheThisObject.packageNameValidator.Validate(packageNameQueryRequest.RequestData);
        MetadataDocument<TMetadataEntry> statesDocumentAsync = await sendInTheThisObject.innerMetadataService.GetPackageVersionStatesDocumentAsync(packageNameQueryRequest);
        versionStatesAsync = (await sendInTheThisObject.RefreshIfNecessary(packageNameQueryRequest, statesDocumentAsync, tracer))?.Entries ?? new List<TMetadataEntry>();
      }
      return versionStatesAsync;
    }

    public async Task<MetadataDocument<TMetadataEntry>> GetPackageVersionStatesDocumentAsync(
      PackageNameQuery<TMetadataEntry> packageNameRequest)
    {
      UpstreamFetchingMetadataService<TPackageIdentity, TMetadataEntry> sendInTheThisObject = this;
      MetadataDocument<TMetadataEntry> statesDocumentAsync1;
      using (ITracerBlock tracer = sendInTheThisObject.tracerService.Enter((object) sendInTheThisObject, nameof (GetPackageVersionStatesDocumentAsync)))
      {
        sendInTheThisObject.packageNameValidator.Validate(packageNameRequest.RequestData);
        MetadataDocument<TMetadataEntry> statesDocumentAsync2 = await sendInTheThisObject.innerMetadataService.GetPackageVersionStatesDocumentAsync(packageNameRequest);
        statesDocumentAsync1 = await sendInTheThisObject.RefreshIfNecessary(packageNameRequest, statesDocumentAsync2, tracer);
      }
      return statesDocumentAsync1;
    }

    private async Task<MetadataDocument<TMetadataEntry>> RefreshIfNecessary(
      PackageNameQuery<TMetadataEntry> packageNameQueryRequest,
      MetadataDocument<TMetadataEntry> doc,
      ITracerBlock tracer)
    {
      IFeedRequest feedRequest = (IFeedRequest) packageNameQueryRequest.RequestData;
      IPackageName packageName = packageNameQueryRequest.RequestData.PackageName;
      string packageNormalizedName = packageName.NormalizedName;
      IEnumerable<UpstreamSource> source = this.upstreamsFromFeedHandler.Handle(feedRequest);
      if (source != null && source.Any<UpstreamSource>())
      {
        if (this.allowUpstreamsForPublicFeedsSettings.Get() || this.allowUpstreamsForPublicFeedsMSFTSettings.Get() && this.executionEnvironment.IsCollectionInMicrosoftTenant())
        {
          bool flag1 = doc == null || doc.Properties == null;
          if (flag1 && this.executionEnvironment.IsUnauthenticatedWebRequest())
            throw new UnauthorizedRequestException(Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_AuthForPossibleUpstreamRefresh((object) packageName), HttpStatusCode.Unauthorized);
          bool flag2 = this.feedPermsFacade.HasPermissions(feedRequest.Feed, FeedPermissionConstants.AddUpstreamPackage);
          if (((flag1 ? 0 : (!this.upstreamEntriesStillValidChecker.IsUpstreamInfoValid(feedRequest, (IMetadataDocument) doc) ? 1 : 0)) != 0 ? 1 : (flag1 & flag2 ? 1 : 0)) == 0)
            return doc;
        }
        else if (this.UpstreamEntriesValid(feedRequest, doc))
          return doc;
        InlineRefreshKey request = new InlineRefreshKey(feedRequest.Protocol, feedRequest.Feed, packageName, this.isFromExplicitUserAction, this.upstreamsConfigurationHasher.GetHash(new UpstreamsConfiguration(feedRequest), (IPackageNameRequest) feedRequest.WithPackageName<IPackageName>(packageName)));
        if ((await this.inlineRefreshInvoker.RunAsync(string.Format("inline refresh of {0} in {1}", (object) packageName, (object) feedRequest.Feed), this.upstreamMetadataManagerBootstrapper, request, CancellationToken.None, (Func<IUpstreamMetadataManager, InlineRefreshKey, CancellationToken, Task<RefreshPackageResult>>) (async (upstreamMetadataManager, refreshKey, _) => await upstreamMetadataManager.RefreshPackageAsync((IFeedRequest) refreshKey, refreshKey.PackageName, false, (ISet<Guid>) ImmutableHashSet<Guid>.Empty, refreshKey.IsFromExplicitUserAction, (PushDrivenUpstreamsNotificationTelemetry) null, false))).EnforceCancellation<RefreshPackageResult>(this.cancellation.Token, file: "D:\\a\\_work\\1\\s\\Packaging\\Service\\Common\\Shared\\Upstream\\UpstreamCache\\V2\\UpstreamFetchingMetadataService.cs", member: nameof (RefreshIfNecessary), line: 192)).RefreshNeeded)
        {
          MetadataDocument<TMetadataEntry> statesDocumentAsync = await this.innerMetadataService.GetPackageVersionStatesDocumentAsync(packageNameQueryRequest);
          this.CheckValidityAfterRefresh(feedRequest, doc, statesDocumentAsync, packageNormalizedName, tracer);
          return statesDocumentAsync;
        }
      }
      return doc;
    }

    private bool UpstreamEntriesValid(
      IFeedRequest feedRequest,
      MetadataDocument<TMetadataEntry> doc)
    {
      if (doc == null)
        return false;
      return doc.Properties == null || this.upstreamEntriesStillValidChecker.IsUpstreamInfoValid(feedRequest, (IMetadataDocument) doc);
    }

    private void CheckValidityAfterRefresh(
      IFeedRequest feedRequest,
      MetadataDocument<TMetadataEntry> oldDoc,
      MetadataDocument<TMetadataEntry> newDoc,
      string packageNormalizedName,
      ITracerBlock tracer)
    {
      if (this.UpstreamEntriesValid(feedRequest, newDoc))
        return;
      tracer.TraceError(this.GetUpstreamDocumentErrorStates(feedRequest, oldDoc, newDoc));
    }

    private string GetUpstreamDocumentErrorStates(
      IFeedRequest feedRequest,
      MetadataDocument<TMetadataEntry> oldDoc,
      MetadataDocument<TMetadataEntry> newDoc)
    {
      string str1 = "Old document has no info.  ";
      if (oldDoc != null && oldDoc.Properties != null)
        str1 = string.Format("Old Last Refresh Time = {0}, old hash = {1}.  {2}", (object) oldDoc.Properties.UpstreamsLastRefreshedUtc, (object) oldDoc.Properties.UpstreamsConfigurationHash, (object) this.upstreamEntriesStillValidChecker.GetErrorMessage(feedRequest, (IMetadataDocument) oldDoc));
      string str2 = "New Document has no info.";
      if (newDoc != null && newDoc.Properties != null)
        str2 = string.Format("New Last Refresh Time = {0}, new hash = {1}. {2}", (object) newDoc.Properties.UpstreamsLastRefreshedUtc, (object) newDoc.Properties.UpstreamsConfigurationHash, (object) this.upstreamEntriesStillValidChecker.GetErrorMessage(feedRequest, (IMetadataDocument) newDoc));
      return str1 + str2;
    }

    public async Task<MetadataDocument<IMetadataEntry>> GetGenericUnfilteredPackageVersionStatesDocumentWithoutRefreshAsync(
      IPackageNameRequest packageNameRequest)
    {
      return await this.innerMetadataService.GetGenericUnfilteredPackageVersionStatesDocumentWithoutRefreshAsync(packageNameRequest);
    }
  }
}
