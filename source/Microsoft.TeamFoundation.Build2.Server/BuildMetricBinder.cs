// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.BuildMetricBinder
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal sealed class BuildMetricBinder : ObjectBinder<BuildMetric>
  {
    private SqlColumnBinder m_name = new SqlColumnBinder("Name");
    private SqlColumnBinder m_intValue = new SqlColumnBinder("IntValue");
    private SqlColumnBinder m_scopedDate = new SqlColumnBinder("ScopedDate");

    protected override BuildMetric Bind() => new BuildMetric()
    {
      Name = this.m_name.GetString((IDataReader) this.Reader, false),
      IntValue = this.m_intValue.GetInt32((IDataReader) this.Reader),
      Date = this.m_scopedDate.GetNullableDateTime((IDataReader) this.Reader)
    };
  }
}
