// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsAzure.Management.Monitoring.ResourceProvider.ResourceProviderConstants
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

namespace Microsoft.WindowsAzure.Management.Monitoring.ResourceProvider
{
  public class ResourceProviderConstants
  {
    public const string DataContractNamespace = "http://schemas.microsoft.com/windowsazure";

    public static class QueryString
    {
      public const string TimeGrainName = "TimeGrain";
    }

    public class MetricCodes
    {
      public const string Success = "Success";
      public const string MetricRetrievalInternalError = "MetricRetrievalInternalError";
      public const string InvalidName = "InvalidName";
      public const string UnsupportedTimeGrain = "UnsupportedTimeGrain";
      public const string NotFound = "NotFound";
    }

    public class RestUrlSegments
    {
      public const string MetricDefinitions = "metricdefinitions";
      public const string Metrics = "metrics";
      public const string MetricBatch = "metricbatch";
    }
  }
}
