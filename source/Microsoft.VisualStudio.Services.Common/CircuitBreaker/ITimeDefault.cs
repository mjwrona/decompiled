// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CircuitBreaker.ITimeDefault
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.CircuitBreaker
{
  internal class ITimeDefault : ITime
  {
    public static readonly ITimeDefault Instance = new ITimeDefault();
    private Stopwatch stopwatch;

    private ITimeDefault()
    {
      this.stopwatch = new Stopwatch();
      this.stopwatch.Start();
    }

    public long GetCurrentTimeInMillis() => this.stopwatch.ElapsedMilliseconds;
  }
}
