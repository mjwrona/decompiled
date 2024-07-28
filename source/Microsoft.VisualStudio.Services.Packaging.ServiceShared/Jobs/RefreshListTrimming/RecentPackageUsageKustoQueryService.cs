// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Jobs.RefreshListTrimming.RecentPackageUsageKustoQueryService
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Kusto.Data.Exceptions;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Cloud;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Jobs.RefreshListTrimming
{
  public class RecentPackageUsageKustoQueryService : IRecentPackageUsageService
  {
    private readonly IPackagingKustoQueryService kustoQueryService;
    private readonly IRetryHelper retryHelper;
    private readonly IRegistryService registryService;
    private readonly ITracerService tracerService;
    private static readonly RegistryQuery SemaphoreTimeoutRegistryQuery = (RegistryQuery) "/Configuration/Packaging/Upstreams/RefreshListTrimming/ConcurrencySemaphoreTimeoutSeconds";
    private static readonly TimeSpan DefaultConcurrencySemaphoreTimeout = TimeSpan.FromMinutes(5.0);
    private static readonly SemaphoreSlim ConcurrentRequestsSemaphore = new SemaphoreSlim(1);
    private static readonly ImmutableList<string> RealUsageCommands = ImmutableList.Create<string>("Maven.GetPackageFileAsync", "MavenContent.DownloadPackage", "MavenVersions.GetPackageVersion", "NpmContent.GetContentScopedPackageAsync", "NpmContent.GetContentUnscopedPackageAsync", "NpmDistTag.GetDistTags", "NpmDownload.GetPackageAsync", "NpmReadme.GetReadmeScopedPackageAsync", "NpmReadme.GetReadmeUnscopedPackageAsync", "NpmRegistry.GetScopedPackageRegistrationAsync", "NpmRegistry.GetUnscopedPackageRegistrationAsync", "NuGetStorage.GetNuGetPackageContentStorageInfo", "NuGetV2Download.GetNupkgAsync", "NuGetVersions.GetPackageVersion", "PackageVersionContent.DownloadPackage", "PyPiContent.DownloadPackage", "PyPiDownload.GetFileAsync", "PyPiSimple.GetPackageAsync", "UPackPackages.GetPackageMetadata", "UPackPackages.GetPackageVersionsMetadata", "UPackVersions.GetPackageVersion", "V2Packages.FindPackagesById", "V2Packages.FindPackagesByIdCount", "V2Packages.GetV2FeedPackage", "V3Flat.GetFileAsync", "V3Flat.GetIndexAsync", "V3Registrations.GetPackageIndexAsync", "V3Registrations.GetPackagePageAsync", "V3Registrations.GetPackageVersionAsync");
    private static readonly string RealUsageCommandsString = RecentPackageUsageKustoQueryService.RealUsageCommands.Serialize<ImmutableList<string>>(true);

    public RecentPackageUsageKustoQueryService(
      IPackagingKustoQueryService kustoQueryService,
      IRetryHelper retryHelper,
      IRegistryService registryService,
      ITracerService tracerService)
    {
      this.kustoQueryService = kustoQueryService;
      this.retryHelper = retryHelper;
      this.registryService = registryService;
      this.tracerService = tracerService;
    }

    public async Task<IEnumerable<(Guid FeedId, int Count)>> GetCountsOfRecentlyUsedPackageNamesPerFeedAsync()
    {
      RecentPackageUsageKustoQueryService sendInTheThisObject = this;
      IEnumerable<(Guid, int)> namesPerFeedAsync1;
      using (sendInTheThisObject.tracerService.Enter((object) sendInTheThisObject, nameof (GetCountsOfRecentlyUsedPackageNamesPerFeedAsync)))
      {
        KustoTabularExpressionStatement query = RecentPackageUsageKustoQueryService.BuildRecentUsageQuery("summarize Count = count() by FeedId", "project FeedId, Count");
        namesPerFeedAsync1 = await sendInTheThisObject.retryHelper.Invoke<IEnumerable<(Guid, int)>>((Func<Task<IEnumerable<(Guid, int)>>>) (async () =>
        {
          IEnumerable<(Guid, int)> namesPerFeedAsync2;
          using (await this.WaitForConcurrencySemaphoreOrTimeoutAsync())
            namesPerFeedAsync2 = this.kustoQueryService.QueryPackagingTraces<RecentPackageUsageKustoQueryService.GetCountsOfRecentlyUsedPackageNamesPerFeedResult>((string) (KustoStatement) query).Select<RecentPackageUsageKustoQueryService.GetCountsOfRecentlyUsedPackageNamesPerFeedResult, (Guid, int)>((Func<RecentPackageUsageKustoQueryService.GetCountsOfRecentlyUsedPackageNamesPerFeedResult, (Guid, int)>) (x => (x.FeedId, x.Count)));
          return namesPerFeedAsync2;
        }));
      }
      return namesPerFeedAsync1;
    }

    public async Task<IEnumerable<(Guid FeedId, string ProtocolName, string PackageName)>> GetRecentlyUsedPackageNamesForFeedsAsync(
      IEnumerable<Guid> feedIds)
    {
      RecentPackageUsageKustoQueryService sendInTheThisObject = this;
      IEnumerable<(Guid, string, string)> namesForFeedsAsync1;
      using (sendInTheThisObject.tracerService.Enter((object) sendInTheThisObject, nameof (GetRecentlyUsedPackageNamesForFeedsAsync)))
      {
        KustoTabularExpressionStatement query = RecentPackageUsageKustoQueryService.BuildRecentUsageQuery("where FeedId in (dynamic(" + feedIds.Select<Guid, string>((Func<Guid, string>) (x => x.ToString("d"))).Serialize<IEnumerable<string>>(true) + "))", "project FeedId, Protocol, PackageName");
        namesForFeedsAsync1 = await sendInTheThisObject.retryHelper.Invoke<IEnumerable<(Guid, string, string)>>((Func<Task<IEnumerable<(Guid, string, string)>>>) (async () =>
        {
          IEnumerable<(Guid, string, string)> namesForFeedsAsync2;
          using (await this.WaitForConcurrencySemaphoreOrTimeoutAsync())
            namesForFeedsAsync2 = this.kustoQueryService.QueryPackagingTraces<RecentPackageUsageKustoQueryService.GetRecentlyUsedPackageNamesByProtocolForFeedsResult>((string) (KustoStatement) query).Select<RecentPackageUsageKustoQueryService.GetRecentlyUsedPackageNamesByProtocolForFeedsResult, (Guid, string, string)>((Func<RecentPackageUsageKustoQueryService.GetRecentlyUsedPackageNamesByProtocolForFeedsResult, (Guid, string, string)>) (x => (x.FeedId, x.Protocol, x.PackageName)));
          return namesForFeedsAsync2;
        }));
      }
      return namesForFeedsAsync1;
    }

    public static bool ShouldRetryOnException(Exception exception)
    {
      return Core(exception) || Core(exception.InnerException);

      static bool Core(Exception ex) => ex is KustoException kustoException && !kustoException.IsPermanent;
    }

    private static KustoTabularExpressionStatement BuildRecentUsageQuery(
      params string[] additionalOperators)
    {
      KustoTabularExpressionStatement expressionStatement = new KustoTabularExpressionStatement("PackagingTraces", new string[3]
      {
        "where Command in (dynamic(" + RecentPackageUsageKustoQueryService.RealUsageCommandsString + "))",
        "where isnotempty(PackageName) and isnotempty(FeedId) and isnotempty(Protocol)",
        "distinct FeedId, Protocol, PackageName"
      });
      foreach (string additionalOperator in additionalOperators)
        expressionStatement.AppendOperator(additionalOperator);
      return expressionStatement;
    }

    private async Task<IDisposable> WaitForConcurrencySemaphoreOrTimeoutAsync()
    {
      RecentPackageUsageKustoQueryService sendInTheThisObject = this;
      IDisposable disposable;
      using (ITracerBlock traceBlock = sendInTheThisObject.tracerService.Enter((object) sendInTheThisObject, nameof (WaitForConcurrencySemaphoreOrTimeoutAsync)))
      {
        TimeSpan timeout = TimeSpan.FromSeconds(sendInTheThisObject.registryService.GetValue<double>(RecentPackageUsageKustoQueryService.SemaphoreTimeoutRegistryQuery, RecentPackageUsageKustoQueryService.DefaultConcurrencySemaphoreTimeout.TotalSeconds));
        int num = await RecentPackageUsageKustoQueryService.ConcurrentRequestsSemaphore.WaitAsync(timeout) ? 1 : 0;
        if (num == 0)
          traceBlock.TraceInfoAlways(string.Format("Did not acquire concurrent-requests semaphore within {0}, continuing on to the query anyway.", (object) timeout));
        disposable = (IDisposable) new RecentPackageUsageKustoQueryService.SemaphoreReleaser(num != 0);
      }
      return disposable;
    }

    private class GetRecentlyUsedPackageNamesByProtocolForFeedsResult
    {
      public Guid FeedId;
      public string Protocol;
      public string PackageName;
    }

    private class GetCountsOfRecentlyUsedPackageNamesPerFeedResult
    {
      public Guid FeedId;
      public int Count;
    }

    private class SemaphoreReleaser : IDisposable
    {
      private readonly bool didAcquireLock;

      public SemaphoreReleaser(bool didAcquireLock) => this.didAcquireLock = didAcquireLock;

      public void Dispose()
      {
        if (!this.didAcquireLock)
          return;
        RecentPackageUsageKustoQueryService.ConcurrentRequestsSemaphore.Release();
      }
    }
  }
}
