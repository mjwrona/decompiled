// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Checks.Server.DataAccess.CheckRunUpdateBinder
// Assembly: Microsoft.Azure.Pipelines.Checks.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D08C7285-654E-4A4D-BA46-748F0D1E96AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Checks.Server.dll

using Microsoft.Azure.Pipelines.Checks.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System.Data;

namespace Microsoft.Azure.Pipelines.Checks.Server.DataAccess
{
  internal class CheckRunUpdateBinder : ObjectBinder<CheckRunUpdate>
  {
    private bool m_isStatusByte = true;
    private SqlColumnBinder m_checkSuiteId = new SqlColumnBinder("BatchRequestId");
    private SqlColumnBinder m_requestId = new SqlColumnBinder("RequestId");
    protected SqlColumnBinder m_status = new SqlColumnBinder("Status");
    private SqlColumnBinder m_modifiedBy = new SqlColumnBinder("ModifiedBy");
    private SqlColumnBinder m_modifiedOn = new SqlColumnBinder("ModifiedOn");

    protected override CheckRunUpdate Bind() => new CheckRunUpdate()
    {
      CheckSuiteId = this.m_checkSuiteId.GetGuid((IDataReader) this.Reader),
      CheckRunId = this.m_requestId.GetGuid((IDataReader) this.Reader),
      Status = this.GetStatus(),
      ModifiedBy = new IdentityRef()
      {
        Id = this.m_modifiedBy.GetGuid((IDataReader) this.Reader).ToString("D")
      },
      ModifiedOn = this.m_modifiedOn.GetDateTime((IDataReader) this.Reader)
    };

    protected virtual CheckRunStatus GetStatus()
    {
      try
      {
        return this.m_isStatusByte ? (CheckRunStatus) this.m_status.GetByte((IDataReader) this.Reader) : (CheckRunStatus) this.m_status.GetInt16((IDataReader) this.Reader);
      }
      catch
      {
        this.m_isStatusByte = false;
        return (CheckRunStatus) this.m_status.GetInt16((IDataReader) this.Reader);
      }
    }
  }
}
