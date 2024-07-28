// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsAzure.Management.Monitoring.ResourceProvider.DataContracts.ResourceUsageMetric
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Microsoft.WindowsAzure.Management.Monitoring.ResourceProvider.DataContracts
{
  [DataContract(Namespace = "http://schemas.microsoft.com/windowsazure", Name = "Usage")]
  [ExcludeFromCodeCoverage]
  public class ResourceUsageMetric
  {
    [DataMember(EmitDefaultValue = false, Name = "Units")]
    private string unit;

    [DataMember(EmitDefaultValue = false)]
    public string Name { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string ResourceName { get; set; }

    [DataMember(EmitDefaultValue = true)]
    public double CurrentValue { get; set; }

    [DataMember(EmitDefaultValue = true)]
    public double Limit { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Unit
    {
      get => this.unit;
      set => this.unit = value;
    }

    [DataMember(EmitDefaultValue = false)]
    public DateTime? NextResetTime { get; set; }
  }
}
