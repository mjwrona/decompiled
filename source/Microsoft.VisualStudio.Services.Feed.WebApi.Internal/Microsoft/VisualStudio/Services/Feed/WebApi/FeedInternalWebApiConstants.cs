// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.WebApi.FeedInternalWebApiConstants
// Assembly: Microsoft.VisualStudio.Services.Feed.WebApi.Internal, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4BC34C1F-0F07-4DDD-8B37-907579B359F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.WebApi.Internal.dll

using System;

namespace Microsoft.VisualStudio.Services.Feed.WebApi
{
  public static class FeedInternalWebApiConstants
  {
    public const string FeedInternalResourceName = "FeedsInternal";
    public const string FeedInternalLocationIdString = "AFEAA2F0-9A40-41B4-A53E-D6674E31FA27";
    public const string FeedIndexEntryResourceName = "IndexEntries";
    public const string FeedIndexEntryLocationIdString = "{599F9C96-F9DE-4BCC-9F71-BF6AB23B2B93}";
    public const string CachedPackagesResourceName = "CachedPackages";
    public const string CachedPackagesLocationIdString = "{6210AC73-E579-4EEA-94C9-3D0B0A784F35}";
    public const string MetricsResourceName = "Metrics";
    public const string MetricsLocationIdString = "{A907C63A-23CA-444E-B847-2CCC6E2EE4F8}";
    public const string FeedCapabilitiesResourceName = "Capabilities";
    public const string FeedCapabilitiesLocationIdString = "{2FBF20FD-376A-4D12-95C6-6876B759CD25}";
    public const string FeedBatchResourceName = "FeedBatch";
    public const string FeedBatchLocationIdString = "{858E27B5-E7F3-4237-8FEB-730E72821B8A}";
    public static readonly Guid FeedInternalLocationId = new Guid("AFEAA2F0-9A40-41B4-A53E-D6674E31FA27");
    public static readonly Guid FeedIndexEntryLocationId = new Guid("{599F9C96-F9DE-4BCC-9F71-BF6AB23B2B93}");
    public static readonly Guid CachedPackagesLocationId = new Guid("{6210AC73-E579-4EEA-94C9-3D0B0A784F35}");
    public static readonly Guid MetricsLocationId = new Guid("{A907C63A-23CA-444E-B847-2CCC6E2EE4F8}");
    public static readonly Guid FeedBatchLocationId = new Guid("{858E27B5-E7F3-4237-8FEB-730E72821B8A}");
    public static readonly Guid FeedCapabilitiesLocationId = new Guid("{2FBF20FD-376A-4D12-95C6-6876B759CD25}");
  }
}
