// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Analytics.TaskDefinitionReferenceBinder
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.DataAccess;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Analytics
{
  internal class TaskDefinitionReferenceBinder : TaskAnalyticsDataBinderBase<TaskDefinitionReference>
  {
    private SqlColumnBinder dataspaceId = new SqlColumnBinder("DataspaceId");
    private SqlColumnBinder referenceId = new SqlColumnBinder("ReferenceId");
    private SqlColumnBinder taskId = new SqlColumnBinder("TaskId");
    private SqlColumnBinder taskName = new SqlColumnBinder("TaskName");
    private SqlColumnBinder taskVersion = new SqlColumnBinder("TaskVersion");
    private Guid m_projectGuid;

    public TaskDefinitionReferenceBinder(TaskSqlComponentBase sqlComponent)
      : base(sqlComponent)
    {
    }

    public TaskDefinitionReferenceBinder(TaskSqlComponentBase sqlComponent, Guid projectGuid)
      : this(sqlComponent)
    {
      this.m_projectGuid = projectGuid;
    }

    protected override TaskDefinitionReference Bind() => new TaskDefinitionReference()
    {
      ProjectGuid = this.m_projectGuid,
      TaskDefinitionReferenceId = this.referenceId.GetInt32((IDataReader) this.Reader),
      TaskDefinitionGuid = this.taskId.GetGuid((IDataReader) this.Reader, false),
      TaskDefinitionName = this.taskName.GetString((IDataReader) this.Reader, false),
      TaskDefinitionVersion = this.taskVersion.GetString((IDataReader) this.Reader, false)
    };
  }
}
