// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.Constants.MavenTracerPoints
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.VisualStudio.Services.Content.Server.Common;

namespace Microsoft.VisualStudio.Services.Maven.Server.Constants
{
  public static class MavenTracerPoints
  {
    private const int Start = 12090000;
    private const int End = 12099999;

    public static class MavenController
    {
      public const int PutPackageAsync = 12090000;
      public const int GetPackageAsync = 12090010;
      public const int HeadPackageAsync = 12090020;
      public const int FeedInfo = 12090030;
      private const int MavenControllerBase = 12090000;
    }

    public static class MavenMetadataController
    {
      public const int GetPackageMetadataAsync = 12090200;
      public const int FeedInfo = 12090210;
      public const int DeletePackageAsync = 12090220;
      private const int MavenMetadataControllerBase = 12090200;
    }

    public static class Plugins
    {
      public const int Metadata = 12090400;
      public const int FeedIndex = 12090410;
      public const int StorageCleanup = 12090420;
      private const int PluginsBase = 12090400;
      public static readonly TraceData MetadataTraceData = new TraceData()
      {
        Area = "maven",
        Layer = "Plugins.Metadata"
      };
      public static readonly TraceData FeedIndexTraceData = new TraceData()
      {
        Area = "maven",
        Layer = "Plugins.FeedIndex"
      };
      public static readonly TraceData StorageCleanupTraceData = new TraceData()
      {
        Area = "maven",
        Layer = "Plugins.StorageCleanup"
      };
    }

    public static class Telemetry
    {
      public const int Push = 12090500;
      public const int Download = 12090510;
      public const int Get = 12090520;
      public const int Metadata = 12090550;
      private const int TelemetryBase = 12090500;
      internal static readonly TraceData GenericServiceTraceData = new TraceData()
      {
        Area = "maven",
        Layer = "Service"
      };
    }

    public static class Ingestion
    {
      public const int AddPackageAsync = 12090600;
      public const int TryAddEntryToDataViews = 12090610;
      public const int PomParsingFailed = 12090620;
      public const int DeleteTempFile = 12090630;
      private const int IngestionBase = 12090600;
      internal static readonly TraceData IngestionTraceData = new TraceData()
      {
        Area = "maven",
        Layer = nameof (Ingestion)
      };
    }

    public static class PluginService
    {
      public const int Set = 12090700;
      public const int Get = 12090710;
      public const int Append = 12090720;
      private const int PluginServiceBase = 12090700;
      public static readonly TraceData TraceData = new TraceData()
      {
        Area = "maven",
        Layer = "MavenPluginService"
      };
    }

    public static class VersionService
    {
      public const int TryAddEntryToDataPromote = 12090800;
      public const int TryAddEntryToDataDelete = 12090810;
      public const int DeletePackageVersion = 12090820;
      public const int RestorePackageVersionToFeed = 12090830;
      public const int PermanentDeletePackageVersion = 12090840;
      public const int GetPackageVersionMetadataFromRecycleBin = 12090880;
      private const int VersionServiceBase = 12090800;
      internal static readonly TraceData VersionTraceData = new TraceData()
      {
        Area = "maven",
        Layer = nameof (VersionService)
      };
    }

    public static class MavenVersionsController
    {
      public const int DeletePackageVersion = 12090910;
      public const int GetPackageVersion = 12090920;
      private const int MavenVersionsControllerBase = 12090900;
    }

    public static class DeletedPackageProcessor
    {
      public const int ProcessChange = 12091000;
      private const int DeletedPackageProcessorBase = 12091000;
      public static readonly TraceData DeletedPackageProcessorTraceData = new TraceData()
      {
        Area = "maven",
        Layer = "MavenDeletedPackageProcessor"
      };
    }

    public static class DeletedFeedProcessor
    {
      public const int ProcessChange = 12091200;
      private const int DeletedFeedProcessorBase = 12091200;
      public static readonly TraceData DeletedFeedProcessorTraceData = new TraceData()
      {
        Area = "maven",
        Layer = "MavenDeletedFeedProcessor"
      };
    }

    public static class MavenRecycleBinController
    {
      public const int DeletePackageVersionFromRecycleBin = 12091300;
      public const int RestorePackageVersionFromRecycleBin = 12091310;
      public const int GetPackageVersionMetadataFromRecycleBin = 12091320;
      private const int MavenRecycleBinControllerBase = 12091300;
    }

    public static class MavenRecycleBinPackagesBatchController
    {
      public const int PackagesBatchAsync = 12091400;
      private const int MavenRecycleBinPackagesBatchControllerBase = 12091400;
    }

    public static class MavenPackagesBatchController
    {
      public const int PackagesBatchAsync = 12091500;
      private const int MavenPackagesBatchControllerBase = 12091500;
    }

    public static class MavenContentController
    {
      public const int DownloadPackage = 12091600;
      private const int MavenContentControllerBase = 12091600;
    }
  }
}
