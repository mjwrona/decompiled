// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsAzure.Management.Monitoring.ResourceProvider.DataContracts.ResourceMetricDimension
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Microsoft.WindowsAzure.Management.Monitoring.ResourceProvider.DataContracts
{
  [DataContract(Namespace = "http://schemas.microsoft.com/windowsazure", Name = "Dimension")]
  [ExcludeFromCodeCoverage]
  public class ResourceMetricDimension
  {
    [DataMember(Order = 0, EmitDefaultValue = false)]
    public string Name { get; set; }

    [DataMember(Order = 1, EmitDefaultValue = false)]
    public string DisplayName { get; set; }

    [DataMember(Order = 2, EmitDefaultValue = false)]
    public string Value { get; set; }
  }
}
