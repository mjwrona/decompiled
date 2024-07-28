// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.DatabasePerformanceStatistics
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

namespace Microsoft.VisualStudio.Services.Cloud
{
  public class DatabasePerformanceStatistics
  {
    public string DatabaseName;
    public string ScaleUnit;
    public int AverageCpuPercentage;
    public int AverageMemoryUsagePercentage;

    public override string ToString() => string.Format("\r\n                      DatabaseName =      {0}\r\n                      ScaleUnit =      {1}\r\n                      AverageCpuPercentage =        {2}\r\n                      AverageMemoryUsagePercentage =    {3}\r\n                    ", (object) this.DatabaseName, (object) this.ScaleUnit, (object) this.AverageCpuPercentage, (object) this.AverageMemoryUsagePercentage);
  }
}
