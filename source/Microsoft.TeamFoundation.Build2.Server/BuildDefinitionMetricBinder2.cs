// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.BuildDefinitionMetricBinder2
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal sealed class BuildDefinitionMetricBinder2 : ObjectBinder<BuildDefinitionMetric>
  {
    private SqlColumnBinder m_definitionId = new SqlColumnBinder("DefinitionId");
    private SqlColumnBinder m_name = new SqlColumnBinder("Name");
    private SqlColumnBinder m_intValue = new SqlColumnBinder("IntValue");
    private SqlColumnBinder m_scopedDate = new SqlColumnBinder("ScopedDate");
    private SqlColumnBinder m_branchName = new SqlColumnBinder("BranchName");

    protected override BuildDefinitionMetric Bind() => new BuildDefinitionMetric()
    {
      DefinitionId = this.m_definitionId.GetInt32((IDataReader) this.Reader),
      Metric = new BuildMetric()
      {
        Name = this.m_name.GetString((IDataReader) this.Reader, false),
        Scope = this.m_branchName.GetString((IDataReader) this.Reader, false),
        IntValue = this.m_intValue.GetInt32((IDataReader) this.Reader),
        Date = this.m_scopedDate.GetNullableDateTime((IDataReader) this.Reader)
      }
    };
  }
}
