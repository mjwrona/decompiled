// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.RntbdConnectionDispenser
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Microsoft.Azure.Cosmos.Core.Trace;
using Microsoft.Azure.Documents.Rntbd;
using System;
using System.Threading.Tasks;

namespace Microsoft.Azure.Documents
{
  internal sealed class RntbdConnectionDispenser : IConnectionDispenser, IDisposable
  {
    private readonly int requestTimeoutInSeconds;
    private readonly int idleConnectionTimeoutInSeconds;
    private readonly string overrideHostNameInCertificate;
    private readonly int openTimeoutInSeconds;
    private readonly UserAgentContainer userAgent;
    private bool isDisposed;
    private TimerPool timerPool;

    public RntbdConnectionDispenser(
      int requestTimeoutInSeconds,
      string overrideHostNameInCertificate,
      int openTimeoutInSeconds,
      int idleConnectionTimeoutInSeconds,
      int timerPoolGranularityInSeconds,
      UserAgentContainer userAgent)
    {
      this.requestTimeoutInSeconds = requestTimeoutInSeconds;
      this.overrideHostNameInCertificate = overrideHostNameInCertificate;
      this.idleConnectionTimeoutInSeconds = idleConnectionTimeoutInSeconds;
      this.openTimeoutInSeconds = openTimeoutInSeconds;
      this.userAgent = userAgent;
      int minSupportedTimerDelayInSeconds = 0;
      if (timerPoolGranularityInSeconds > 0 && timerPoolGranularityInSeconds < openTimeoutInSeconds && timerPoolGranularityInSeconds < requestTimeoutInSeconds)
        minSupportedTimerDelayInSeconds = timerPoolGranularityInSeconds;
      else if (openTimeoutInSeconds > 0 && requestTimeoutInSeconds > 0)
        minSupportedTimerDelayInSeconds = Math.Min(openTimeoutInSeconds, requestTimeoutInSeconds);
      else if (openTimeoutInSeconds > 0)
        minSupportedTimerDelayInSeconds = openTimeoutInSeconds;
      else if (requestTimeoutInSeconds > 0)
        minSupportedTimerDelayInSeconds = requestTimeoutInSeconds;
      this.timerPool = new TimerPool(minSupportedTimerDelayInSeconds);
      DefaultTrace.TraceInformation("RntbdConnectionDispenser: requestTimeoutInSeconds: {0}, openTimeoutInSeconds: {1}, timerValueInSeconds: {2}", (object) requestTimeoutInSeconds, (object) openTimeoutInSeconds, (object) minSupportedTimerDelayInSeconds);
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    private void Dispose(bool disposing)
    {
      if (this.isDisposed)
        return;
      if (disposing)
      {
        this.timerPool.Dispose();
        this.timerPool = (TimerPool) null;
        DefaultTrace.TraceInformation("RntbdConnectionDispenser Disposed");
      }
      this.isDisposed = true;
    }

    public async Task<IConnection> OpenNewConnection(Guid activityId, Uri fullUri, string poolKey)
    {
      RntbdConnection connection = new RntbdConnection(fullUri, (double) this.requestTimeoutInSeconds, this.overrideHostNameInCertificate, (double) this.openTimeoutInSeconds, (double) this.idleConnectionTimeoutInSeconds, poolKey, this.userAgent, this.timerPool);
      DateTimeOffset now = DateTimeOffset.Now;
      try
      {
        await connection.Open(activityId, fullUri);
      }
      finally
      {
        ChannelOpenTimeline.LegacyWriteTrace(connection.ConnectionTimers);
      }
      return (IConnection) connection;
    }
  }
}
