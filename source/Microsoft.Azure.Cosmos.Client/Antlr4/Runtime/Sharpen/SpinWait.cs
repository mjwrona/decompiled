// Decompiled with JetBrains decompiler
// Type: Antlr4.Runtime.Sharpen.SpinWait
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;
using System.Threading;

namespace Antlr4.Runtime.Sharpen
{
  internal struct SpinWait
  {
    private const int step = 10;
    private const int maxTime = 200;
    private static readonly bool isSingleCpu = Environment.ProcessorCount == 1;
    private int ntime;

    public void SpinOnce()
    {
      ++this.ntime;
      ManualResetEvent manualResetEvent = new ManualResetEvent(false);
      if (SpinWait.isSingleCpu)
        manualResetEvent.WaitOne(0);
      else if (this.ntime % 10 == 0)
        manualResetEvent.WaitOne(0);
      else
        manualResetEvent.WaitOne(Math.Min(this.ntime, 200) << 1);
    }
  }
}
