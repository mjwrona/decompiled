// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DiagnosticDumper
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class DiagnosticDumper
  {
    private TimeSpan m_delay;
    private TimeSpan m_duration;
    private int m_fastMethodsCount;
    private int m_truncatedMethodsCount;
    private long m_threshold;
    private KeyValuePair<DiagnosticLocation, int>[] m_diagnosticInfo;

    public DiagnosticDumper(
      TimeSpan delay,
      TimeSpan duration,
      KeyValuePair<DiagnosticLocation, int>[] diagnosticInfo,
      int fastMethodsCount,
      int truncatedMethodsCount,
      long threshold)
    {
      this.m_delay = delay;
      this.m_duration = duration;
      this.m_diagnosticInfo = diagnosticInfo;
      this.m_fastMethodsCount = fastMethodsCount;
      this.m_truncatedMethodsCount = truncatedMethodsCount;
      this.m_threshold = threshold;
    }

    internal StringBuilder ToStringBuilder()
    {
      StringBuilder stringBuilder = new StringBuilder(1024);
      stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "Total Time:{0} (Delay {1}ms) (Duration {2}ms) (Threshold {3}ms) (Fast {4}) (Truncated {5})\r\n", (object) (this.m_delay.TotalMilliseconds + this.m_duration.TotalMilliseconds), (object) this.m_delay.TotalMilliseconds, (object) this.m_duration.TotalMilliseconds, (object) this.m_threshold, (object) this.m_fastMethodsCount, (object) this.m_truncatedMethodsCount);
      if (this.m_diagnosticInfo != null)
      {
        for (int index = 0; index < this.m_diagnosticInfo.Length; ++index)
        {
          KeyValuePair<DiagnosticLocation, int> keyValuePair = this.m_diagnosticInfo[index];
          if (keyValuePair.Key.IsInitialized())
            stringBuilder.AppendFormat("{0}:{1} ", (object) keyValuePair.Key, (object) keyValuePair.Value);
          else
            break;
        }
      }
      return stringBuilder;
    }

    public override string ToString() => this.ToStringBuilder().ToString();
  }
}
