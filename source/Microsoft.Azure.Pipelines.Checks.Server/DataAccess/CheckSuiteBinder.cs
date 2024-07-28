// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Checks.Server.DataAccess.CheckSuiteBinder
// Assembly: Microsoft.Azure.Pipelines.Checks.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D08C7285-654E-4A4D-BA46-748F0D1E96AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Checks.Server.dll

using Microsoft.Azure.Pipelines.Checks.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System.Data;

namespace Microsoft.Azure.Pipelines.Checks.Server.DataAccess
{
  internal class CheckSuiteBinder : ObjectBinder<CheckRun>
  {
    private SqlColumnBinder m_checkSuiteId = new SqlColumnBinder("BatchRequestId");
    private SqlColumnBinder m_requestId = new SqlColumnBinder("RequestId");
    private SqlColumnBinder m_checkConfigurationId = new SqlColumnBinder("AssignmentId");
    private SqlColumnBinder m_status = new SqlColumnBinder("Status");
    private SqlColumnBinder m_createdDate = new SqlColumnBinder("CreatedDate");
    private SqlColumnBinder m_completedDate = new SqlColumnBinder("CompletedDate");
    private SqlColumnBinder m_resultMessage = new SqlColumnBinder("ResultMessage");
    private SqlColumnBinder m_typeId = new SqlColumnBinder("TypeId");
    private SqlColumnBinder m_scope = new SqlColumnBinder("Scope");

    protected override CheckRun Bind()
    {
      int int32 = this.m_checkConfigurationId.GetInt32((IDataReader) this.Reader);
      string toDeserialize = this.m_scope.GetString((IDataReader) this.Reader, string.Empty);
      Resource resource = new Resource();
      try
      {
        resource = JsonUtility.FromString<Resource>(toDeserialize);
      }
      catch (JsonReaderException ex)
      {
      }
      CheckRun checkRun = new CheckRun();
      checkRun.CheckSuiteRef = new CheckSuiteRef()
      {
        Id = this.m_checkSuiteId.GetGuid((IDataReader) this.Reader)
      };
      checkRun.Id = this.m_requestId.GetGuid((IDataReader) this.Reader);
      checkRun.Status = (CheckRunStatus) this.m_status.GetByte((IDataReader) this.Reader);
      checkRun.CreatedDate = this.m_createdDate.GetDateTime((IDataReader) this.Reader);
      checkRun.CompletedDate = this.m_completedDate.GetNullableDateTime((IDataReader) this.Reader);
      checkRun.ResultMessage = this.m_resultMessage.GetString((IDataReader) this.Reader, true);
      checkRun.CheckConfigurationRef = new CheckConfigurationRef()
      {
        Type = new CheckType()
        {
          Id = this.m_typeId.GetGuid((IDataReader) this.Reader)
        },
        Id = int32,
        Resource = resource
      };
      return checkRun;
    }
  }
}
