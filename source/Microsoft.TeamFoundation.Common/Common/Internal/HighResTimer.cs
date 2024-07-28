// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Common.Internal.HighResTimer
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using System.ComponentModel;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Common.Internal
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class HighResTimer
  {
    private long m_startTime;
    private long m_stopTime;
    private double m_conversionFactor = HighResTimer.s_defaultConversionFactor;
    private static double s_defaultConversionFactor = 1000000.0;

    public static long TimeStamp => Stopwatch.GetTimestamp();

    public double ConversionFactor
    {
      get => this.m_conversionFactor;
      set => this.m_conversionFactor = value;
    }

    public long Duration => HighResTimer.diffTime(this.m_stopTime, this.m_startTime, this.m_conversionFactor);

    public void Start() => this.m_startTime = Stopwatch.GetTimestamp();

    public void Reset()
    {
      this.m_startTime = 0L;
      this.m_stopTime = 0L;
      this.m_conversionFactor = HighResTimer.s_defaultConversionFactor;
    }

    public void Stop() => this.m_stopTime = Stopwatch.GetTimestamp();

    public static long ElapsedTime(long stop, long start) => HighResTimer.diffTime(stop, start, HighResTimer.s_defaultConversionFactor);

    private static long diffTime(long stop, long start, double conversionFactor)
    {
      if (stop < start)
        stop = start;
      double num = (double) (stop - start) / (double) Stopwatch.Frequency;
      return (long) (conversionFactor * num);
    }
  }
}
