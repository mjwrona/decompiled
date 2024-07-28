// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.TimeoutHelper
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using System;
using System.Threading;

namespace Microsoft.Azure.Documents
{
  internal sealed class TimeoutHelper
  {
    private readonly DateTime startTime;
    private readonly TimeSpan timeOut;
    private readonly CancellationToken cancellationToken;

    public TimeoutHelper(TimeSpan timeOut, CancellationToken cancellationToken = default (CancellationToken))
    {
      this.startTime = DateTime.UtcNow;
      this.timeOut = timeOut;
      this.cancellationToken = cancellationToken;
    }

    public bool IsElapsed() => DateTime.UtcNow.Subtract(this.startTime) >= this.timeOut;

    public TimeSpan GetRemainingTime() => this.timeOut.Subtract(DateTime.UtcNow.Subtract(this.startTime));

    public void ThrowTimeoutIfElapsed()
    {
      if (this.IsElapsed())
        throw new RequestTimeoutException(RMResources.RequestTimeout);
    }

    public void ThrowGoneIfElapsed()
    {
      this.cancellationToken.ThrowIfCancellationRequested();
      if (this.IsElapsed())
        throw new GoneException(RMResources.Gone, SubStatusCodes.TimeoutGenerated410);
    }
  }
}
