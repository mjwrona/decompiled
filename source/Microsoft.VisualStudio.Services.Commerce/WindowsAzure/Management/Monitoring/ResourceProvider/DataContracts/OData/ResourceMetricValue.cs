// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsAzure.Management.Monitoring.ResourceProvider.DataContracts.OData.ResourceMetricValue
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using System;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.WindowsAzure.Management.Monitoring.ResourceProvider.DataContracts.OData
{
  [ExcludeFromCodeCoverage]
  public class ResourceMetricValue
  {
    public string Id { get; set; }

    public DateTime Timestamp { get; set; }

    public double? Min { get; set; }

    public double? Max { get; set; }

    public double? Total { get; set; }

    public double? Average { get; set; }
  }
}
