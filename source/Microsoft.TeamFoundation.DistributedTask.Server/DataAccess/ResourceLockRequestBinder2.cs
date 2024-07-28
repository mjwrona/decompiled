// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.ResourceLockRequestBinder2
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  internal sealed class ResourceLockRequestBinder2 : ObjectBinder<ResourceLockRequest>
  {
    private SqlColumnBinder m_requestId = new SqlColumnBinder("RequestId");
    private SqlColumnBinder m_resourceId = new SqlColumnBinder("ResourceId");
    private SqlColumnBinder m_resourceType = new SqlColumnBinder("ResourceType");
    private SqlColumnBinder m_planId = new SqlColumnBinder("PlanId");
    private SqlColumnBinder m_nodeName = new SqlColumnBinder("NodeName");
    private SqlColumnBinder m_nodeAttempt = new SqlColumnBinder("NodeAttempt");
    private SqlColumnBinder m_status = new SqlColumnBinder("Status");
    private SqlColumnBinder m_queueTime = new SqlColumnBinder("QueueTime");
    private SqlColumnBinder m_assignTime = new SqlColumnBinder("AssignTime");
    private SqlColumnBinder m_finishTime = new SqlColumnBinder("FinishTime");
    private SqlColumnBinder m_checkRunId = new SqlColumnBinder("CheckRunId");
    private SqlColumnBinder m_projectId = new SqlColumnBinder("ProjectId");
    private SqlColumnBinder m_definitionId = new SqlColumnBinder("DefinitionId");
    private SqlColumnBinder m_type = new SqlColumnBinder("Type");

    protected override ResourceLockRequest Bind() => new ResourceLockRequest()
    {
      RequestId = this.m_requestId.GetInt64((IDataReader) this.Reader),
      ResourceId = this.m_resourceId.GetString((IDataReader) this.Reader, false),
      ResourceType = this.m_resourceType.GetString((IDataReader) this.Reader, false),
      PlanId = this.m_planId.GetGuid((IDataReader) this.Reader),
      NodeName = this.m_nodeName.GetString((IDataReader) this.Reader, false),
      NodeAttempt = this.m_nodeAttempt.GetInt32((IDataReader) this.Reader),
      Status = (ResourceLockStatus) this.m_status.GetByte((IDataReader) this.Reader),
      QueueTime = this.m_queueTime.GetDateTime((IDataReader) this.Reader),
      AssignTime = this.m_assignTime.GetNullableDateTime((IDataReader) this.Reader),
      FinishTime = this.m_finishTime.GetNullableDateTime((IDataReader) this.Reader),
      CheckRunId = this.m_checkRunId.GetGuid((IDataReader) this.Reader),
      ProjectId = this.m_projectId.GetGuid((IDataReader) this.Reader),
      DefinitionId = this.m_definitionId.GetInt32((IDataReader) this.Reader),
      LockType = this.m_type.ColumnExists((IDataReader) this.Reader) ? (ExclusiveLockType) this.m_type.GetByte((IDataReader) this.Reader) : ExclusiveLockType.RunLatest
    };
  }
}
