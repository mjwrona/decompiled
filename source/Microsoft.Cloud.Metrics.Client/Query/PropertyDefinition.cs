// Decompiled with JetBrains decompiler
// Type: Microsoft.Cloud.Metrics.Client.Query.PropertyDefinition
// Assembly: Microsoft.Cloud.Metrics.Client, Version=2.2023.705.2051, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 06B39E1C-7DF0-4BC1-AFBA-9AD635E73CB0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Cloud.Metrics.Client.dll

using Microsoft.Cloud.Metrics.Client.Metrics;
using Newtonsoft.Json;
using System;

namespace Microsoft.Cloud.Metrics.Client.Query
{
  public sealed class PropertyDefinition
  {
    [JsonConstructor]
    public PropertyDefinition(
      PropertyAggregationType propertyAggregationType,
      SamplingType samplingType)
    {
      this.PropertyAggregationType = propertyAggregationType;
      this.SamplingType = samplingType;
      this.PropertyName = PropertyDefinition.GetPropertyName(this.PropertyAggregationType, this.SamplingType.Name);
    }

    public PropertyAggregationType PropertyAggregationType { get; }

    public SamplingType SamplingType { get; }

    public string PropertyName { get; }

    public static string GetPropertyName(
      PropertyAggregationType propertyAggregationType,
      string samplingTypeName)
    {
      switch (propertyAggregationType)
      {
        case PropertyAggregationType.Average:
          return "TAVG(" + samplingTypeName + ")";
        case PropertyAggregationType.Sum:
          return "TSUM(" + samplingTypeName + ")";
        case PropertyAggregationType.Min:
          return "TMIN(" + samplingTypeName + ")";
        case PropertyAggregationType.Max:
          return "TMAX(" + samplingTypeName + ")";
        default:
          throw new ArgumentException(string.Format("Unexpected propertyAggregationType: {0}.", (object) propertyAggregationType), nameof (propertyAggregationType));
      }
    }
  }
}
