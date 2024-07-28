// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Analytics.ShallowTaskPlanBinder
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.DataAccess;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Analytics
{
  internal class ShallowTaskPlanBinder : TaskAnalyticsDataBinderBase<ShallowTaskPlan>
  {
    private Guid m_projectGuid;
    private SqlColumnBinder dataspaceId = new SqlColumnBinder("DataspaceId");
    private SqlColumnBinder internalId = new SqlColumnBinder("InternalId");
    private SqlColumnBinder lastUpdated = new SqlColumnBinder("LastUpdated");

    public ShallowTaskPlanBinder(TaskSqlComponentBase sqlComponent, Guid projectGuid)
      : base(sqlComponent)
    {
      this.m_projectGuid = projectGuid;
    }

    protected override ShallowTaskPlan Bind() => new ShallowTaskPlan()
    {
      ProjectGuid = this.m_projectGuid,
      PlanId = this.internalId.GetInt32((IDataReader) this.Reader),
      LastUpdated = this.lastUpdated.GetDateTime((IDataReader) this.Reader)
    };
  }
}
