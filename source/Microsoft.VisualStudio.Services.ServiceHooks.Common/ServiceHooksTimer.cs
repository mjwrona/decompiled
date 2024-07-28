// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Common.ServiceHooksTimer
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E36C8A02-D97F-45E0-9F96-E7385D8CA092
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.Common.dll

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Common
{
  public class ServiceHooksTimer
  {
    private Stopwatch m_stopwatch;
    private List<long> m_ticks;
    private string m_percents;

    public long Millis => this.m_stopwatch.ElapsedMilliseconds;

    public string Percents
    {
      get
      {
        if (this.m_percents == null)
          this.GeneratePercents();
        return this.m_percents;
      }
    }

    public ServiceHooksTimer()
    {
      this.m_ticks = new List<long>();
      this.m_ticks.Add(0L);
      this.m_stopwatch = Stopwatch.StartNew();
    }

    public static ServiceHooksTimer StartNew() => new ServiceHooksTimer();

    public void RecordTick() => this.m_ticks.Add(this.m_stopwatch.ElapsedTicks);

    public void Stop()
    {
      this.m_stopwatch.Stop();
      this.RecordTick();
    }

    private void GeneratePercents()
    {
      long num1 = this.m_ticks.Last<long>();
      StringBuilder stringBuilder = new StringBuilder("[");
      for (int index = 1; index < this.m_ticks.Count; ++index)
      {
        int num2 = (int) ((double) (this.m_ticks[index] - this.m_ticks[index - 1]) * 100.0 / (double) num1);
        stringBuilder.AppendFormat("{0},", (object) num2);
      }
      --stringBuilder.Length;
      stringBuilder.Append("]");
      this.m_percents = stringBuilder.ToString();
    }
  }
}
