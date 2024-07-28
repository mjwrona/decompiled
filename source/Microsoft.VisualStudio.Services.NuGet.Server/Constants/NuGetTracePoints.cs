// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.Constants.NuGetTracePoints
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.VisualStudio.Services.Content.Server.Common;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Telemetry;

namespace Microsoft.VisualStudio.Services.NuGet.Server.Constants
{
  internal static class NuGetTracePoints
  {
    private const int NuGetBase = 5720000;

    internal static class NuGetRecycleBinController
    {
      internal const int DeletePackageVersionFromRecycleBin = 5720200;
      internal const int RestorePackageVersionFromRecycleBin = 5720210;
      internal const int GetPackageVersionMetadataFromRecycleBin = 5720220;
      private const int NuGetRecycleBinControllerBase = 5720200;

      internal static TraceData TraceData => new TraceData()
      {
        Area = "NuGet",
        Layer = nameof (NuGetRecycleBinController)
      };
    }

    internal static class NuGetPackagesBatchController
    {
      internal const int BatchUpdatePackages = 5720500;
      private const int NuGetPackagesBatchControllerBase = 5720500;

      internal static TraceData TraceData => new TraceData()
      {
        Area = "NuGet",
        Layer = nameof (NuGetPackagesBatchController)
      };
    }

    internal static class NuGetPackagingApiController
    {
      internal const int GetPackageVersion = 5720600;
      internal const int DeletePackageVersion = 5720610;
      internal const int UpdatePackageVersion = 5720620;
      private const int NuGetPackagingApiControllerBase = 5720600;

      internal static TraceData TraceData => new TraceData()
      {
        Area = "NuGet",
        Layer = nameof (NuGetPackagingApiController)
      };
    }

    internal static class Telemetry
    {
      internal const int Publish = 5720800;
      private const int TelemetryBase = 5720800;

      internal static TraceData GenericServiceTraceData => new TraceData()
      {
        Area = "NuGet",
        Layer = "Service"
      };

      internal static TelemetryTraceInfo TraceInfo => new TelemetryTraceInfo()
      {
        Tracepoint = 5720800,
        TraceData = NuGetTracePoints.Telemetry.GenericServiceTraceData
      };
    }

    internal static class V2PushController
    {
      internal const int TryDeleteFile = 5720900;
      internal const int PushPackageV2 = 5720910;
      internal const int PushProgressClientResults = 5720920;
      internal const int PushProgressBlobResults = 5720930;
      internal const int TryDeleteDirectory = 5720940;
      private const int V2PushControllerBase = 5720900;

      internal static TraceData TraceData => new TraceData()
      {
        Area = "NuGet",
        Layer = "Controller"
      };
    }

    internal static class PackagesController
    {
      internal const int AddPackageFromBlobStore = 5721000;
      internal const int GetPackage = 5721010;
      private const int PackagesControllerBase = 5721000;
    }

    internal static class ServiceIndexController
    {
      internal const int GetFeedsIndex = 5721300;
      private const int ServiceIndexControllerBase = 5721300;
    }

    internal static class DeleteController
    {
      internal const int DeletePackage = 5721500;
      private const int DeleteControllerBase = 5721500;

      internal static TraceData TraceData => new TraceData()
      {
        Area = "NuGet",
        Layer = "NuGetV2DeleteController"
      };
    }

    internal static class CommitLogController
    {
      internal const int GetIndex = 5721800;
      internal const int GetEntry = 5721810;
      private const int CommitLogControllerBase = 5721800;
    }

    internal static class NewRegistrationsController
    {
      internal const int GetPackageIndex = 5722000;
      internal const int GetPackagePage = 5722010;
      internal const int GetPackageVersion = 5722020;
      private const int NewRegistrationsControllerBase = 5722000;
    }

    internal static class NuGetUriBuilder
    {
      internal const int GetPackageVersionNewRegistrationUri = 5722110;
      internal const int GetCommitLogEntryUri = 5722120;
      private const int NuGetUriBuilderBase = 5722100;

      internal static TraceData TraceData => new TraceData()
      {
        Area = "NuGet",
        Layer = nameof (NuGetUriBuilder)
      };
    }

    internal static class V2PackagesController
    {
      internal const int GetPackageByIdAndVersion = 5722300;
      internal const int FindPackagesById = 5722310;
      internal const int Search = 5722320;
      internal const int FindPackagesByIdCount = 5722330;
      internal const int SearchCount = 5722340;
      internal const int GetAllPackages = 5722350;
      internal const int GetAllPackagesCount = 5722360;
      private const int V2PackagesControllerBase = 5722300;

      internal static TraceData TraceData => new TraceData()
      {
        Area = "NuGet",
        Layer = nameof (V2PackagesController)
      };
    }

    internal static class NewFlatController
    {
      internal const int GetVersions = 5722500;
      internal const int GetFile = 5722510;
      internal const int GetNuspecs = 5722520;
      internal const int GetNames = 5722530;
      internal const int GetAllPackageVersions = 5722540;
      private const int NewFlatControllerBase = 5722500;
    }

    internal static class NewQueryController
    {
      internal const int ExecuteQuery = 5722700;
      private const int QueryControllerBase = 5722700;
    }

    internal static class PackageIngestionService
    {
      internal const int AddPackageToNewStore = 5722800;
      internal const int ValidatePackage = 5722810;
      internal const int CheckForDuplicatePackage = 5722820;
      internal const int AddPackageFromStream = 5722830;
      internal const int AddPackageFromBlobIdentifier = 5722840;
      internal const int AddPackageFromDrop = 5722850;
      internal const int GetBlob = 5722860;
      internal const int GetStoredNupkgInfo = 5722870;
      internal const int AddPackageToNewSearchIndex = 5722880;
      internal const int QueuePackageIndexUpdateJobAsync = 5722890;
      private const int PackageIngestionServiceBase = 5722800;

      internal static TraceData TraceData => new TraceData()
      {
        Area = "NuGet",
        Layer = nameof (PackageIngestionService)
      };
    }

    internal static class TracingBlobStoreWrapper
    {
      internal const int TryReferenceAsync = 5722900;
      internal const int TryReferenceWithBlocksAsync = 5722910;
      internal const int RemoveReferencesAsync = 5722920;
      internal const int GetDownloadUrisAsync = 5722930;
      internal const int GetBlobAsync = 5722940;
      internal const int PutSingleBlockBlobAndReferenceAsync = 5722950;
      internal const int PutBlobBlockAsync = 5722960;
      internal const int PutBlobAndReferenceAsync = 5722970;
      internal const int GetDownloadUriAsync = 5722980;
      internal const int GetSasUrisAsync = 5722990;
      internal const int ValidateKeepUntilReferencesAsync = 5723500;
      private const int TracingBlobStoreWrapperBase = 5722900;
      private const int TracingBlobStoreWrapperContinueBase = 5723500;

      internal static TraceData TraceData => new TraceData()
      {
        Area = "NuGet",
        Layer = "NuGetBlobStore"
      };
    }

    internal static class PackageNotFoundRedisCache
    {
      internal const int IsEnabled = 5723400;
      internal const int GetPackageVersion = 5723410;
      internal const int GetPackage = 5723420;
      internal const int AddPackageVersion = 5723430;
      internal const int AddPackage = 5723440;
      internal const int InvalidatePackgeVersion = 5723450;
      internal const int InvalidatePackage = 5723460;
      internal const int RedisException = 5723470;
      internal const int CircuitBreakerException = 5723480;
      private const int PackageNotFoundRedisCacheBase = 5723400;

      internal static TraceData TraceData => new TraceData()
      {
        Area = "NuGet",
        Layer = nameof (PackageNotFoundRedisCache)
      };
    }

    internal static class NuGetAzureDownloadUrlCacheService
    {
      internal const int SetDownloadUrl = 5723600;
      internal const int InvalidateDownloadUrl = 5723610;
      internal const int TryGetDownloadUrl = 5723620;
      private const int NuGetAzureDownloadUrlCacheServiceBase = 5723600;

      internal static TraceData TraceData => new TraceData()
      {
        Area = "NuGet",
        Layer = nameof (NuGetAzureDownloadUrlCacheService)
      };
    }

    internal static class NuGetRecyleBinPackagesBatchController
    {
      internal const int UpdateRecycleBinPackageVersions = 5723700;
      private const int NuGetRecyleBinPackagesBatchControllerBase = 5723700;

      internal static TraceData TraceData => new TraceData()
      {
        Area = "NuGet",
        Layer = nameof (NuGetRecyleBinPackagesBatchController)
      };
    }
  }
}
