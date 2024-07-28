// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.RequestChargeTracker
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

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
