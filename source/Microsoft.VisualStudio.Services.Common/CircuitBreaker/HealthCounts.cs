// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CircuitBreaker.HealthCounts
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

namespace Microsoft.VisualStudio.Services.CircuitBreaker
{
  public class HealthCounts
  {
    private readonly long totalCount;
    private readonly long errorCount;
    private readonly int errorPercentage;
    private readonly long semaphoreRejectedCount;

    public long TotalRequests => this.totalCount;

    public long ErrorCount => this.errorCount;

    public long SemaphoreRejected => this.semaphoreRejectedCount;

    public int ErrorPercentage => this.errorPercentage;

    public HealthCounts(long total, long error, long semaphoreRejected)
    {
      this.totalCount = total;
      this.errorCount = error;
      this.semaphoreRejectedCount = semaphoreRejected;
      if (total > 0L)
        this.errorPercentage = (int) ((double) error / (double) total * 100.0);
      else
        this.errorPercentage = 0;
    }
  }
}
