// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsAzure.Management.Monitoring.ResourceProvider.DataContracts.ResourceMetricDefinition
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Microsoft.WindowsAzure.Management.Monitoring.ResourceProvider.DataContracts
{
  [DataContract(Namespace = "http://schemas.microsoft.com/windowsazure", Name = "MetricDefinition")]
  [ExcludeFromCodeCoverage]
  public class ResourceMetricDefinition
  {
    private string unit;

    public ResourceMetricDefinition() => this.Dimensions = new ResourceMetricDefinitionDimensions();

    public ResourceMetricDefinition(ResourceMetricDefinition metricDefinition)
    {
      this.Name = metricDefinition.Name;
      this.DisplayName = metricDefinition.DisplayName;
      this.Dimensions = metricDefinition.Dimensions;
      this.PrimaryAggregationTypeInternal = metricDefinition.PrimaryAggregationTypeInternal;
      this.SelfLink = metricDefinition.SelfLink;
      this.MetricAvailabilities = metricDefinition.MetricAvailabilities;
      this.Unit = metricDefinition.Unit;
      this.IsAlertable = metricDefinition.IsAlertable;
      this.MinimumAlertableTimeWindow = metricDefinition.MinimumAlertableTimeWindow;
    }

    [DataMember(EmitDefaultValue = false, IsRequired = true, Name = "Name")]
    public string Name { get; set; }

    [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "DisplayName")]
    public string DisplayName { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public ResourceMetricDefinitionDimensions Dimensions { get; set; }

    [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "Unit")]
    public string Unit
    {
      get => !string.IsNullOrWhiteSpace(this.unit) ? this.unit : "Other";
      set => this.unit = value;
    }

    [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "SelfLink")]
    public string SelfLink { get; set; }

    [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "PrimaryAggregationType")]
    public string PrimaryAggregationType
    {
      get => this.PrimaryAggregationTypeInternal.ToString();
      set => this.PrimaryAggregationTypeInternal = (ResourceAggregationType) Enum.Parse(typeof (ResourceAggregationType), value);
    }

    [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "MetricAvailabilities")]
    public IList<ResourceMetricAvailability> MetricAvailabilities { get; set; }

    public ResourceAggregationType PrimaryAggregationTypeInternal { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool IsAlertable { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "MinimumAlertableTimeWindow")]
    public string MinimumAlertableTimeWindowAsString { get; set; }

    public TimeSpan MinimumAlertableTimeWindow
    {
      get => ResourceProviderUtility.ParseTimeSpan(this.MinimumAlertableTimeWindowAsString);
      set => this.MinimumAlertableTimeWindowAsString = value.ToString();
    }
  }
}
