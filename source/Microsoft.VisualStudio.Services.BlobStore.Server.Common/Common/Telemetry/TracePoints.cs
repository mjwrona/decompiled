// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.Common.Telemetry.TracePoints
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CB48D0BF-32A2-483C-A1D4-2F10DEBB3D56
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.Common.dll

using Microsoft.VisualStudio.Services.Content.Server.Common;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.Common.Telemetry
{
  public static class TracePoints
  {
    private const int BlobStoreSharedBase = 5700000;

    public static class StorageStatistics
    {
      public const int TracePoint = 5702099;

      public static TraceData TraceData => new TraceData()
      {
        Area = "Blobstore",
        Layer = nameof (StorageStatistics)
      };
    }

    internal static class MultiDomainBlobHandler
    {
      public const int AddBlockBlobBlockAsync = 5704031;
      public const int ContentLengthMismatchError = 5704032;
      public const int AddBlockBlobBlockCacheError = 5704033;
      public static readonly EnterLeaveTracePoint ReadToBufferAsyncCall = TracePoints.MultiDomainBlobHandler.CreateEnterLeaveTracePoint(5704034);

      public static TraceData TraceData => new TraceData()
      {
        Area = "Blobstore",
        Layer = nameof (MultiDomainBlobHandler)
      };

      private static EnterLeaveTracePoint CreateEnterLeaveTracePoint(int tracePoint) => new EnterLeaveTracePoint(tracePoint, tracePoint, TracePoints.MultiDomainBlobHandler.TraceData.Area, TracePoints.MultiDomainBlobHandler.TraceData.Layer);
    }

    internal static class Domain
    {
      internal const int DomainMin = 5701700;
      internal const int DomainMax = 5701799;

      internal static class RegistryHostDomainProvider
      {
        internal const int GetAdminDomainInfo = 5701700;
        internal const int RegistrySetValue = 5701701;
        internal const int CreateProjectDomainsForAdminAsync = 5701702;
      }

      internal static class HostDomainStoreService
      {
        internal const int GetDefaultDomainForOrganizationAsync = 5701750;
        internal const int GetDomainsForOrganizationAsync = 5701751;
        internal const int GetDomainForOrganizationAsync = 5701752;
        internal const int CreateProjectDomainsForAdminAsync = 5701753;
      }

      internal static class AdminHostDomainStoreService
      {
        internal const int GetDomainsForOrganizationForAdminAsync = 5701780;
      }
    }

    internal static class Jobs
    {
      internal const int JobsMin = 5701900;
      internal const int JobsMax = 5701999;

      internal static class BlobIdReferenceExporterBase
      {
        internal const int ExportIdReference = 5701910;
      }

      internal static class Copy
      {
        internal const int CopyDedupAsync = 5701920;
      }
    }
  }
}
