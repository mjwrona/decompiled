// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.DataQualityResult
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using System;

namespace Microsoft.VisualStudio.Services.Analytics
{
  public class DataQualityResult
  {
    public int PartitionId { get; set; }

    public DateTime RunDate { get; set; }

    public DateTime RunEndDate { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public string Name { get; set; }

    public string Scope { get; set; }

    public string TargetTable { get; set; }

    public long ExpectedValue { get; set; }

    public long ActualValue { get; set; }

    public bool Failed { get; set; }

    public double KpiValue { get; set; }
  }
}
