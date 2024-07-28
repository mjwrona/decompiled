// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.WebApi.Internal.Converters.PackageMetricsDataConverter
// Assembly: Microsoft.VisualStudio.Services.Feed.WebApi.Internal, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4BC34C1F-0F07-4DDD-8B37-907579B359F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.WebApi.Internal.dll

using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Feed.WebApi.Internal.Converters
{
  public class PackageMetricsDataConverter : VssJsonCreationConverter<PackageMetricsData>
  {
    private Dictionary<RawMetricType, Func<RawMetricData>> supportedTypes = new Dictionary<RawMetricType, Func<RawMetricData>>()
    {
      {
        RawMetricType.UserDownloadMetric,
        (Func<RawMetricData>) (() => (RawMetricData) new UserDownloadRawMetric())
      }
    };

    protected override PackageMetricsData Create(Type objectType, JObject jsonObject)
    {
      string operationTypeString = this.GetBatchOperationTypeString(jsonObject);
      return !this.IsSupportedOperation(operationTypeString) ? (PackageMetricsData) null : this.Create(operationTypeString);
    }

    private bool IsSupportedOperation(string operationType) => Enum.TryParse<RawMetricType>(operationType, true, out RawMetricType _);

    private string GetBatchOperationTypeString(JObject jsonObject)
    {
      JToken jtoken = jsonObject.GetValue("RawMetricType", StringComparison.OrdinalIgnoreCase);
      return jtoken == null ? (string) null : jtoken.Value<string>();
    }

    private PackageMetricsData Create(string operationType)
    {
      RawMetricType key = (RawMetricType) Enum.Parse(typeof (RawMetricType), operationType, true);
      RawMetricData rawMetricData = this.supportedTypes[key]();
      return new PackageMetricsData()
      {
        RawMetricType = key,
        RawMetricData = rawMetricData
      };
    }
  }
}
