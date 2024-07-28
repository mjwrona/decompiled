// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.WebApi.Contracts.AnalyticsHeaders
// Assembly: Microsoft.VisualStudio.Services.Analytics.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F858B048-FFE8-4E5F-8EBC-9B25DAC23DF8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Analytics.WebApi.dll

namespace Microsoft.VisualStudio.Services.Analytics.WebApi.Contracts
{
  public static class AnalyticsHeaders
  {
    public const string prefix = "X-TFS-";
    public const string SyncDateConditionsHeader = "X-TFS-If-SyncDates-Newer";
    public const string DeprecatedGetSyncDatesHeader = "GetSyncDates";
    public const string GetSyncDatesHeader = "X-TFS-GetSyncDates";
  }
}
