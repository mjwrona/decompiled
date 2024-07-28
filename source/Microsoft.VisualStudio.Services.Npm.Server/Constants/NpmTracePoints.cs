// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.Constants.NpmTracePoints
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.VisualStudio.Services.Content.Server.Common;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Telemetry;

namespace Microsoft.VisualStudio.Services.Npm.Server.Constants
{
  internal static class NpmTracePoints
  {
    private const int NpmBase = 12000000;

    internal static class NpmRegistryController
    {
      internal const int PutPackageAsync = 12000000;
      internal const int GetPackageRegistrationAsync = 12000010;
      internal const int GetUpstreamPackageAsync = 12000020;
      internal const int GetScopedUpstreamPackageAsync = 12000030;
      internal const int GetAuditBulk = 12000040;
      internal const int GetAuditQuick = 12000050;
      internal const int GetAuditFull = 12000060;
      private const int RegistryControllerBase = 12000000;
    }

    internal static class NpmPackageIngestionService
    {
      internal const int AddPackageAsync = 12000100;
      internal const int AddPackageToNewSearchIndexAsync = 12000110;
      internal const int CachePackageAsync = 12000130;
      private const int NpmPackageIngestionServiceBase = 12000100;

      internal static TraceData TraceData => new TraceData()
      {
        Area = "Npm",
        Layer = nameof (NpmPackageIngestionService)
      };
    }

    internal static class NpmUriBuilder
    {
      internal const int GetPackageDownloadRedirectUri = 12000300;
      internal const int GetDirectPackageContentUri = 12000310;
      private const int NpmUriBuilderBase = 12000300;

      internal static TraceData TraceData => new TraceData()
      {
        Area = "Npm",
        Layer = nameof (NpmUriBuilder)
      };
    }

    internal static class Telemetry
    {
      internal const int Publish = 12000400;
      private const int TelemetryBase = 12000400;

      internal static TraceData GenericServiceTraceData => new TraceData()
      {
        Area = "npm",
        Layer = "Service"
      };

      internal static TelemetryTraceInfo TraceInfo => new TelemetryTraceInfo()
      {
        Tracepoint = 12000400,
        TraceData = NpmTracePoints.Telemetry.GenericServiceTraceData
      };
    }

    internal static class NpmDistTagController
    {
      internal const int GetDistTagsUpscoped = 12000500;
      internal const int GetDistTagsScoped = 12000510;
      internal const int PutDistTagUpscoped = 12000520;
      internal const int PutDistTagScoped = 12000530;
      internal const int DeleteDistTagUpscoped = 12000540;
      internal const int DeleteDistTagScoped = 12000550;
      private const int NpmDistTagControllerBase = 12000500;

      internal static TraceData TraceData => new TraceData()
      {
        Area = "Npm",
        Layer = nameof (NpmDistTagController)
      };
    }

    internal static class NpmUnpublishController
    {
      internal const int UnpublishPackage = 12000800;
      internal const int UnpublishScopedPackage = 12000810;
      internal const int DeleteTarball = 12000820;
      internal const int DeleteScopedTarball = 12000830;
      private const int NpmUnpublishControllerBase = 12000800;

      internal static TraceData TraceData => new TraceData()
      {
        Area = "Npm",
        Layer = nameof (NpmUnpublishController)
      };
    }

    internal static class NpmVersionsController
    {
      internal const int UnpublishPackage = 12000900;
      internal const int UnpublishScopedPackage = 12000910;
      internal const int DeprecatePackage = 12000920;
      internal const int DeprecateScopedPackage = 12000930;
      internal const int GetPackageInfo = 12000940;
      internal const int GetScopedPackageInfo = 12000950;
      internal const int GetVersionsExposedToDownstream = 12000960;
      internal const int GetVersionsExposedToDownstreamScoped = 12000970;
      private const int NpmVersionsControllerBase = 12000900;

      internal static TraceData TraceData => new TraceData()
      {
        Area = "Npm",
        Layer = nameof (NpmVersionsController)
      };
    }

    internal static class NpmReadmeController
    {
      internal const int GetReadmeAsync = 12001300;
      private const int ReadmeControllerBase = 12001300;
    }

    internal static class NpmContentController
    {
      internal const int GetContentAsync = 12001400;
      private const int ContentControllerBase = 12001400;
    }

    internal static class NpmPackagesBatchController
    {
      internal const int PackagesBatchAsync = 12001500;
      private const int NpmPackagesBatchControllerBase = 12001500;
    }

    internal static class NpmRecycleBinController
    {
      internal const int DeletePackageVersionFromRecycleBin = 12001700;
      internal const int DeleteScopedPackageVersionFromRecycleBin = 12001710;
      internal const int RestorePackageVersionFromRecycleBin = 12001720;
      internal const int RestoreScopedPackageVersionFromRecycleBin = 12001730;
      internal const int GetPackageVersionMetadataFromRecycleBin = 12001740;
      internal const int GetScopedPackageVersionMetadataFromRecycleBin = 12001750;
      private const int NpmRecycleBinControllerBase = 12001700;

      internal static TraceData TraceData => new TraceData()
      {
        Area = "Npm",
        Layer = nameof (NpmRecycleBinController)
      };
    }
  }
}
