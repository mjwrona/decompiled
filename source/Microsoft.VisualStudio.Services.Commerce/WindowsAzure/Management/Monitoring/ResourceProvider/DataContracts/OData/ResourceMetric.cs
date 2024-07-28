// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsAzure.Management.Monitoring.ResourceProvider.DataContracts.OData.ResourceMetric
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using System.Diagnostics.CodeAnalysis;

namespace Microsoft.WindowsAzure.Management.Monitoring.ResourceProvider.DataContracts.OData
{
  [ExcludeFromCodeCoverage]
  public class ResourceMetric
  {
    public string Id { get; set; }

    public string Name { get; set; }

    public string DisplayName { get; set; }

    public string Unit { get; set; }

    public string PrimaryAggregation { get; set; }
  }
}
