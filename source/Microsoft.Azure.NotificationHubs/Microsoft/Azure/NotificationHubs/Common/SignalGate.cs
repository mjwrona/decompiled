// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Common.SignalGate
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System;
using System.Threading;

namespace Microsoft.Azure.NotificationHubs.Common
{
  [Serializable]
  internal class SignalGate
  {
    private int state;

    internal bool IsLocked => this.state == 0;

    internal bool IsSignalled => this.state == 3;

    public bool Signal()
    {
      int num = this.state;
      if (num == 0)
        num = Interlocked.CompareExchange(ref this.state, 1, 0);
      if (num == 2)
      {
        this.state = 3;
        return true;
      }
      if (num != 0)
        SignalGate.ThrowInvalidSignalGateState();
      return false;
    }

    public bool Unlock()
    {
      int num = this.state;
      if (num == 0)
        num = Interlocked.CompareExchange(ref this.state, 2, 0);
      if (num == 1)
      {
        this.state = 3;
        return true;
      }
      if (num != 0)
        SignalGate.ThrowInvalidSignalGateState();
      return false;
    }

    private static void ThrowInvalidSignalGateState() => throw Fx.Exception.AsError((Exception) new InvalidOperationException(SRCore.InvalidSemaphoreExit));

    private static class GateState
    {
      public const int Locked = 0;
      public const int SignalPending = 1;
      public const int Unlocked = 2;
      public const int Signalled = 3;
    }
  }
}
