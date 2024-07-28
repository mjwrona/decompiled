// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsAzure.Management.Monitoring.ResourceProvider.DataContracts.ResourceMetricSample
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Microsoft.WindowsAzure.Management.Monitoring.ResourceProvider.DataContracts
{
  [DataContract(Namespace = "http://schemas.microsoft.com/windowsazure", Name = "MetricSample")]
  [ExcludeFromCodeCoverage]
  public sealed class ResourceMetricSample
  {
    [DataMember(EmitDefaultValue = false)]
    public DateTime TimeCreated { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public double? Total { get; set; }

    [DataMember(EmitDefaultValue = true, IsRequired = false)]
    public double? Minimum { get; set; }

    [DataMember(EmitDefaultValue = true, IsRequired = false)]
    public double? Maximum { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int? Count { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Annotation { get; set; }
  }
}
