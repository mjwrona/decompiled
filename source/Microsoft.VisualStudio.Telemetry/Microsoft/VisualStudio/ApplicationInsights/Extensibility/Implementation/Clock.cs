// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation.Clock
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using System;
using System.Diagnostics;

namespace Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation
{
  internal class Clock : IClock
  {
    private static readonly DateTimeOffset InitialTimeStamp = DateTimeOffset.Now;
    private static readonly Stopwatch OffsetStopwatch = Stopwatch.StartNew();
    private static IClock instance = (IClock) new Clock();

    protected Clock()
    {
    }

    public static IClock Instance
    {
      get => Clock.instance;
      protected set => Clock.instance = value ?? (IClock) new Clock();
    }

    public DateTimeOffset Time => Clock.InitialTimeStamp + Clock.OffsetStopwatch.Elapsed;
  }
}
