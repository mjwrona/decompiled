// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.TimerWheel
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Timers;
using System;


#nullable enable
namespace Microsoft.Azure.Cosmos
{
  internal abstract class TimerWheel : IDisposable
  {
    public abstract void Dispose();

    public abstract TimerWheelTimer CreateTimer(TimeSpan timeout);

    public abstract void SubscribeForTimeouts(TimerWheelTimer timer);

    public static TimerWheel CreateTimerWheel(TimeSpan resolution, int buckets) => (TimerWheel) new TimerWheelCore(resolution, buckets);
  }
}
