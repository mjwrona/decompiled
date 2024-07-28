// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.QueryStoreOptionsBase
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public abstract class QueryStoreOptionsBase
  {
    public QueryStoreOptionsBase()
    {
    }

    public QueryStoreOptionsBase(QueryStoreOptionsBase queryStoreOptions)
    {
      ArgumentUtility.CheckForNull<QueryStoreOptionsBase>(queryStoreOptions, nameof (queryStoreOptions));
      this.DatabaseName = queryStoreOptions.DatabaseName;
      this.StaleQueryThresholdDays = queryStoreOptions.StaleQueryThresholdDays;
      this.MaxStorageSizeMB = queryStoreOptions.MaxStorageSizeMB;
      this.IntervalLengthMinutes = queryStoreOptions.IntervalLengthMinutes;
      this.FlushIntervalSeconds = queryStoreOptions.FlushIntervalSeconds;
      this.MaxPlansPerQuery = queryStoreOptions.MaxPlansPerQuery;
      this.QueryCaptureMode = queryStoreOptions.QueryCaptureMode;
      this.SizeBasedCleanupMode = queryStoreOptions.SizeBasedCleanupMode;
    }

    public string DatabaseName { get; set; }

    public long StaleQueryThresholdDays { get; set; } = 21;

    public long MaxStorageSizeMB { get; set; } = 2048;

    public long IntervalLengthMinutes { get; set; } = 30;

    public long FlushIntervalSeconds { get; set; } = 900;

    public long MaxPlansPerQuery { get; set; } = 200;

    public QueryStoreCaptureMode QueryCaptureMode { get; set; } = QueryStoreCaptureMode.Auto;

    public QueryStoreSizeBasedCleanupMode SizeBasedCleanupMode { get; set; } = QueryStoreSizeBasedCleanupMode.Auto;

    public override string ToString() => string.Format("DatabaseName: {0}, StaleQueryThresholdDays: {1}, MaxStorageSizeMB: {2}, IntervalLengthMinutes: {3}, ", (object) this.DatabaseName, (object) this.StaleQueryThresholdDays, (object) this.MaxStorageSizeMB, (object) this.IntervalLengthMinutes) + string.Format("FlushIntervalSeconds: {0}, MaxPlansPerQuery: {1}, QueryCaptureMode: {2}, SizeBasedCleanupMode: {3}", (object) this.FlushIntervalSeconds, (object) this.MaxPlansPerQuery, (object) this.QueryCaptureMode, (object) this.SizeBasedCleanupMode);
  }
}
