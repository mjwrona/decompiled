// Decompiled with JetBrains decompiler
// Type: Microsoft.Cloud.Metrics.Client.MetricsServerRelativeUris
// Assembly: Microsoft.Cloud.Metrics.Client, Version=2.2023.705.2051, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 06B39E1C-7DF0-4BC1-AFBA-9AD635E73CB0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Cloud.Metrics.Client.dll

namespace Microsoft.Cloud.Metrics.Client
{
  public static class MetricsServerRelativeUris
  {
    public const string DataRelativeUrl = "v1/data/metrics";
    public const string MetaDataRelativeUrl = "v1/hint";
    public const string MetaDataRelativeUrlV2 = "v2/hint";
    public const string ConfigRelativeUrl = "v1/config/metrics";
    public const string ConfigRelativeUrlV2 = "v2/config/metrics";
    public const string TenantConfigRelativeUrl = "v1/config";
    public const string HealthConfigRelativeUrl = "v2/config/health";
    public const string AccountConfigRelativeUrl = "v1/config/tenant";
    public const string HealthRelativeUrl = "v3/data/health";
    public const string DistributedQueryRelativeUrl = "flight/dq/batchedReadv3";
    public const string QueryServiceRelativeUrl = "query";
  }
}
