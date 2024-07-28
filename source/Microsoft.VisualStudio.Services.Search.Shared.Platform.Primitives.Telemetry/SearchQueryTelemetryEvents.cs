// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.SearchQueryTelemetryEvents
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4AA9C920-1627-4C01-9F3D-849A7BC9C916
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.dll

using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry
{
  public static class SearchQueryTelemetryEvents
  {
    public const string QueryEventName = "TFS/Search/Query";
    public static readonly IDictionary<string, string> PropertyToRegistryMap = (IDictionary<string, string>) new Dictionary<string, string>()
    {
      {
        "EntityType",
        "TFS.Search.Query.EntityType"
      },
      {
        "ESReportedQueryTime",
        "TFS.Search.Query.ESReportedQueryTime"
      },
      {
        "E2EPlatformQueryTime",
        "TFS.Search.Query.E2EPlatformQueryTime"
      },
      {
        "E2EQueryTime",
        "TFS.Search.Query.E2EQueryTime"
      },
      {
        "NoOfFailedQueryRequests",
        "TFS.Search.Query.NoOfFailedQueryRequests"
      },
      {
        "NoOfQueryRequests",
        "TFS.Search.Query.NoOfQueryRequests"
      },
      {
        "SearchServiceErrorCode",
        "TFS.Search.Query.SearchServiceErrorCode"
      },
      {
        "SearchPlatformErrorMessage",
        "TFS.Search.Query.SearchPlatformErrorMessage"
      }
    };
    private const string QueryBaseName = "TFS.Search.Query.";

    public static ICollection<string> GetAdditiveProperties() => (ICollection<string>) new List<string>();
  }
}
