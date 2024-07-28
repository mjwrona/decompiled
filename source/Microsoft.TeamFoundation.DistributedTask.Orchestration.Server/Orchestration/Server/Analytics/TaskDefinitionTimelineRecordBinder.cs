// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Analytics.TaskDefinitionTimelineRecordBinder
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.DataAccess;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Analytics
{
  internal class TaskDefinitionTimelineRecordBinder : 
    TaskAnalyticsDataBinderBase<TaskDefinitionTimelineRecord>
  {
    private SqlColumnBinder dataspaceId = new SqlColumnBinder("DataspaceId");
    private SqlColumnBinder planId = new SqlColumnBinder("PlanId");
    private SqlColumnBinder instanceId = new SqlColumnBinder("InstanceId");
    private SqlColumnBinder referenceId = new SqlColumnBinder("ReferenceId");
    private SqlColumnBinder planLastUpdated = new SqlColumnBinder("PlanLastUpdated");
    private Guid m_projectGuid;

    public TaskDefinitionTimelineRecordBinder(TaskSqlComponentBase sqlComponent, Guid projectGuid)
      : base(sqlComponent)
    {
      this.m_projectGuid = projectGuid;
    }

    protected override TaskDefinitionTimelineRecord Bind() => new TaskDefinitionTimelineRecord()
    {
      ProjectGuid = this.m_projectGuid,
      TimelineRecordGuid = this.instanceId.GetGuid((IDataReader) this.Reader, false),
      TaskDefinitionReferenceId = this.referenceId.GetInt32((IDataReader) this.Reader),
      PlanId = this.planId.GetInt32((IDataReader) this.Reader),
      PlanLastUpdated = this.planLastUpdated.GetDateTime((IDataReader) this.Reader)
    };
  }
}
