// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Checks.Server.DataAccess.CheckSuiteBinder3
// Assembly: Microsoft.Azure.Pipelines.Checks.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D08C7285-654E-4A4D-BA46-748F0D1E96AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Checks.Server.dll

using Microsoft.Azure.Pipelines.Checks.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data;

namespace Microsoft.Azure.Pipelines.Checks.Server.DataAccess
{
  internal class CheckSuiteBinder3 : CheckSuiteBinder2
  {
    private bool m_isStatusByte = true;
    private SqlColumnBinder m_checkSuiteId = new SqlColumnBinder("BatchRequestId");
    private SqlColumnBinder m_requestId = new SqlColumnBinder("RequestId");
    private SqlColumnBinder m_checkConfigurationId = new SqlColumnBinder("AssignmentId");
    private SqlColumnBinder m_checkConfiguratioVersion = new SqlColumnBinder("AssignmentVersion");
    protected SqlColumnBinder m_status = new SqlColumnBinder("Status");
    private SqlColumnBinder m_createdDate = new SqlColumnBinder("CreatedDate");
    private SqlColumnBinder m_completedDate = new SqlColumnBinder("CompletedDate");
    private SqlColumnBinder m_resultMessage = new SqlColumnBinder("ResultMessage");
    private SqlColumnBinder m_typeId = new SqlColumnBinder("TypeId");
    private SqlColumnBinder m_scope = new SqlColumnBinder("Scope");
    private SqlColumnBinder m_context = new SqlColumnBinder("Context");
    private SqlColumnBinder m_timeoutJobId = new SqlColumnBinder("TimeoutJobId");
    private SqlColumnBinder m_EvaluationOrder = new SqlColumnBinder("EvaluationOrder");

    protected override CheckRun Bind()
    {
      int int32_1 = this.m_checkConfigurationId.GetInt32((IDataReader) this.Reader);
      int int32_2 = this.m_checkConfiguratioVersion.GetInt32((IDataReader) this.Reader, 1, 1);
      string toDeserialize = this.m_scope.GetString((IDataReader) this.Reader, string.Empty);
      Resource resource = new Resource();
      try
      {
        resource = JsonUtility.FromString<Resource>(toDeserialize);
      }
      catch (JsonReaderException ex)
      {
      }
      string json = this.m_context.GetString((IDataReader) this.Reader, false, "");
      JObject jobject = (JObject) null;
      CheckConfigurationRef configurationRef = new CheckConfigurationRef()
      {
        Type = new CheckType()
        {
          Id = this.m_typeId.GetGuid((IDataReader) this.Reader)
        },
        Id = int32_1,
        Version = int32_2,
        Resource = resource
      };
      if (!string.IsNullOrEmpty(json))
      {
        try
        {
          jobject = JObject.Parse(json);
          if (!jobject.HasValues)
            jobject = (JObject) null;
        }
        catch (JsonReaderException ex)
        {
          jobject = (JObject) null;
        }
      }
      CheckRun checkRun = new CheckRun();
      checkRun.CheckSuiteRef = new CheckSuiteRef()
      {
        Id = this.m_checkSuiteId.GetGuid((IDataReader) this.Reader),
        Context = jobject
      };
      checkRun.Id = this.m_requestId.GetGuid((IDataReader) this.Reader);
      checkRun.Status = this.GetStatus();
      checkRun.CreatedDate = this.m_createdDate.GetDateTime((IDataReader) this.Reader);
      checkRun.CompletedDate = this.m_completedDate.GetNullableDateTime((IDataReader) this.Reader);
      checkRun.ResultMessage = this.m_resultMessage.GetString((IDataReader) this.Reader, true);
      checkRun.TimeoutJobId = this.m_timeoutJobId.GetNullableGuid((IDataReader) this.Reader);
      checkRun.CheckConfigurationRef = configurationRef;
      checkRun.EvaluationOrder = (CheckEvaluationOrder) this.m_EvaluationOrder.GetByte((IDataReader) this.Reader, (byte) 30, (byte) 30);
      return checkRun;
    }

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
