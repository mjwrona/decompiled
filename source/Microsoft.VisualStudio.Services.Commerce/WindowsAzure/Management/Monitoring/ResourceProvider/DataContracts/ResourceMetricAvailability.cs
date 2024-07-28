// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsAzure.Management.Monitoring.ResourceProvider.DataContracts.ResourceMetricAvailability
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Microsoft.WindowsAzure.Management.Monitoring.ResourceProvider.DataContracts
{
  [DataContract(Namespace = "http://schemas.microsoft.com/windowsazure", Name = "MetricAvailability")]
  [ExcludeFromCodeCoverage]
  public class ResourceMetricAvailability
  {
    [DataMember(EmitDefaultValue = false, Name = "TimeGrain")]
    public string TimeGrainAsString { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "Retention")]
    public string RetentionAsString { get; set; }

    public TimeSpan TimeGrain
    {
      get => ResourceProviderUtility.ParseTimeSpan(this.TimeGrainAsString);
      set => this.TimeGrainAsString = value.ToString();
    }

    public TimeSpan Retention
    {
      get => ResourceProviderUtility.ParseTimeSpan(this.RetentionAsString);
      set => this.RetentionAsString = value.ToString();
    }
  }
}
