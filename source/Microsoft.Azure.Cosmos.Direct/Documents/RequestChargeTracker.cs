// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.RequestChargeTracker
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using System.Threading;

namespace Microsoft.Azure.Documents
{
  internal sealed class RequestChargeTracker
  {
    private long totalRUsNotServedToClient;
    private long totalRUs;
    private const int numberOfDecimalPointToReserveFactor = 1000;

    public double TotalRequestCharge => (double) this.totalRUs / 1000.0;

    public void AddCharge(double ruUsage)
    {
      Interlocked.Add(ref this.totalRUsNotServedToClient, (long) (ruUsage * 1000.0));
      Interlocked.Add(ref this.totalRUs, (long) (ruUsage * 1000.0));
    }

    public double GetAndResetCharge() => (double) Interlocked.Exchange(ref this.totalRUsNotServedToClient, 0L) / 1000.0;
  }
}
