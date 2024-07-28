// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.PersistedStageBinder
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  internal sealed class PersistedStageBinder : ObjectBinder<PersistedStage>
  {
    private SqlColumnBinder m_stageId = new SqlColumnBinder("StageId");
    private SqlColumnBinder m_stageName = new SqlColumnBinder("StageName");
    private SqlColumnBinder m_definitionId = new SqlColumnBinder("DefinitionId");

    protected override PersistedStage Bind() => new PersistedStage()
    {
      Id = this.m_stageId.GetInt64((IDataReader) this.Reader),
      Name = this.m_stageName.GetString((IDataReader) this.Reader, false),
      DefinitionId = this.m_definitionId.GetInt32((IDataReader) this.Reader)
    };
  }
}
