// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.PendingJobInfoColumns
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class PendingJobInfoColumns : ObjectBinder<PendingJobInfo>
  {
    private SqlColumnBinder PriorityColumn = new SqlColumnBinder("Priority");
    private SqlColumnBinder MaxDelaySecColumn = new SqlColumnBinder("MaxDelaySec");
    private SqlColumnBinder CountColumn = new SqlColumnBinder("Count");

    protected override PendingJobInfo Bind() => new PendingJobInfo()
    {
      Priority = (int) this.PriorityColumn.GetByte((IDataReader) this.Reader),
      MaxDelay = TimeSpan.FromSeconds((double) this.MaxDelaySecColumn.GetInt32((IDataReader) this.Reader)),
      Count = this.CountColumn.GetInt32((IDataReader) this.Reader)
    };
  }
}
