// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DatabaseResourceStatsBinder
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class DatabaseResourceStatsBinder : ObjectBinder<DatabaseResourceStats>
  {
    private SqlColumnBinder m_perfTime = new SqlColumnBinder("PerfTime");
    private SqlColumnBinder m_avgCpuPercent = new SqlColumnBinder("AvgCpuPercent");
    private SqlColumnBinder m_avgDataIOPercent = new SqlColumnBinder("AvgDataIOPercent");
    private SqlColumnBinder m_avgLogWritePercent = new SqlColumnBinder("AvgLogWritePercent");
    private SqlColumnBinder m_avgMemoryUsagePercent = new SqlColumnBinder("AvgMemoryUsagePercent");
    private SqlColumnBinder m_max_worker_percent = new SqlColumnBinder("MaxWorkerPercent");
    private SqlColumnBinder m_currentTime = new SqlColumnBinder("CurrentTime");
    private SqlColumnBinder m_serviceObjective = new SqlColumnBinder("ServiceObjective");
    private SqlColumnBinder m_pageLatchAvgWaitTimeMS = new SqlColumnBinder("PageLatchAvgWaitTimeMS");

    internal DatabaseResourceStatsBinder()
    {
    }

    protected override DatabaseResourceStats Bind()
    {
      DatabaseResourceStats databaseResourceStats = new DatabaseResourceStats();
      databaseResourceStats.PerfTime = this.m_perfTime.GetDateTime((IDataReader) this.Reader);
      databaseResourceStats.AvgCpuPercent = Convert.ToDecimal(this.m_avgCpuPercent.GetObject((IDataReader) this.Reader));
      databaseResourceStats.AvgDataIOPercent = Convert.ToDecimal(this.m_avgDataIOPercent.GetObject((IDataReader) this.Reader));
      databaseResourceStats.AvgLogWritePercent = Convert.ToDecimal(this.m_avgLogWritePercent.GetObject((IDataReader) this.Reader));
      databaseResourceStats.AvgMemoryUsagePercent = Convert.ToDecimal(this.m_avgMemoryUsagePercent.GetObject((IDataReader) this.Reader));
      databaseResourceStats.CurrentTime = DateTime.UtcNow;
      databaseResourceStats.MaxWorkerPercent = 0M;
      if (this.m_max_worker_percent.ColumnExists((IDataReader) this.Reader))
        databaseResourceStats.MaxWorkerPercent = Convert.ToDecimal(this.m_max_worker_percent.GetObject((IDataReader) this.Reader));
      databaseResourceStats.CurrentTime = this.m_currentTime.GetDateTime((IDataReader) this.Reader, new DateTime());
      databaseResourceStats.ServiceObjective = this.m_serviceObjective.GetString((IDataReader) this.Reader, (string) null);
      databaseResourceStats.PageLatchAvgTimeMS = this.m_pageLatchAvgWaitTimeMS.GetInt32((IDataReader) this.Reader, 0, 0);
      return databaseResourceStats;
    }
  }
}
