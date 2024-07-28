// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsAzure.Management.Monitoring.ResourceProvider.DataContracts.ResourceMetricSet
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Microsoft.WindowsAzure.Management.Monitoring.ResourceProvider.DataContracts
{
  [DataContract(Namespace = "http://schemas.microsoft.com/windowsazure", Name = "MetricSet")]
  [ExcludeFromCodeCoverage]
  public class ResourceMetricSet
  {
    public ResourceMetricSet()
    {
      this.Dimensions = new ResourceMetricDimensions();
      this.Payload = new List<ResourceMetricSample>();
      this.MetricAvailabilities = (IList<ResourceMetricAvailability>) new List<ResourceMetricAvailability>();
    }

    [DataMember(EmitDefaultValue = false)]
    public string Name { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string DisplayName { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public ResourceMetricDimensions Dimensions { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string PrimaryAggregationType { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Unit { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IList<ResourceMetricAvailability> MetricAvailabilities { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime EndTime { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime StartTime { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "Values")]
    public List<ResourceMetricSample> Payload { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "TimeGrain")]
    public string TimeGrainAsString { get; set; }

    public TimeSpan TimeGrain
    {
      get => ResourceProviderUtility.ParseTimeSpan(this.TimeGrainAsString);
      set => this.TimeGrainAsString = value.ToString();
    }

    [DataMember(EmitDefaultValue = false)]
    public bool IsAlertable { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "MinimumAlertableTimeWindow")]
    public string MinimumAlertableTimeWindowAsString { get; set; }

    public TimeSpan MinimumAlertableTimeWindow
    {
      get => ResourceProviderUtility.ParseTimeSpan(this.MinimumAlertableTimeWindowAsString);
      set => this.MinimumAlertableTimeWindowAsString = value.ToString();
    }

    public string SelfLink { get; set; }
  }
}
