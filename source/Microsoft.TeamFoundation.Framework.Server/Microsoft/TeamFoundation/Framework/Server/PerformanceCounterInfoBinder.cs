// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.PerformanceCounterInfoBinder
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class PerformanceCounterInfoBinder : ObjectBinder<PerformanceCounterInfo>
  {
    private SqlColumnBinder m_counterName = new SqlColumnBinder("counter_name");
    private SqlColumnBinder m_counterValue = new SqlColumnBinder("cntr_value");

    protected override PerformanceCounterInfo Bind() => new PerformanceCounterInfo()
    {
      CounterName = this.m_counterName.GetString((IDataReader) this.Reader, true),
      CounterValue = this.m_counterValue.GetInt64((IDataReader) this.Reader, 0L)
    };
  }
}
