// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.HostMoveRequestBinder
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;
using System.Data.SqlClient;

namespace Microsoft.VisualStudio.Services.Cloud
{
  internal class HostMoveRequestBinder : ObjectBinder<HostMoveRequest>
  {
    private SqlColumnBinder m_hostIdColumn = new SqlColumnBinder("HostId");
    private SqlColumnBinder m_targetDatabaseIdColumn = new SqlColumnBinder("TargetDatabaseId");
    private SqlColumnBinder m_priorityColumn = new SqlColumnBinder("Priority");
    private SqlColumnBinder m_optionsColumnColumn = new SqlColumnBinder("Options");
    private SqlColumnBinder m_jobIdColumn = new SqlColumnBinder("JobId");

    protected override HostMoveRequest Bind()
    {
      SqlDataReader reader = this.Reader;
      return new HostMoveRequest()
      {
        HostId = this.m_hostIdColumn.GetGuid((IDataReader) reader),
        TargetDatabaseId = this.m_targetDatabaseIdColumn.GetInt32((IDataReader) reader),
        Priority = this.m_priorityColumn.GetByte((IDataReader) reader),
        Options = (HostMoveOptions) this.m_optionsColumnColumn.GetByte((IDataReader) reader),
        JobId = this.m_jobIdColumn.GetGuid((IDataReader) reader, true)
      };
    }
  }
}
