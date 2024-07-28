// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.SearchTelemetryEvents
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4AA9C920-1627-4C01-9F3D-849A7BC9C916
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.dll

using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry
{
  public static class SearchTelemetryEvents
  {
    public static IDictionary<string, string> GetPropertyToEventMap(string traceArea) => !"Query Pipeline".Equals(traceArea, StringComparison.OrdinalIgnoreCase) ? SearchIndexerTelemetryEvents.PropertyToEventMap : SearchQueryTelemetryEvents.PropertyToRegistryMap;

    public static ICollection<string> GetAdditiveProperties(string traceArea) => "Query Pipeline".Equals(traceArea, StringComparison.OrdinalIgnoreCase) ? SearchQueryTelemetryEvents.GetAdditiveProperties() : SearchIndexerTelemetryEvents.GetAdditiveProperties();
  }
}
