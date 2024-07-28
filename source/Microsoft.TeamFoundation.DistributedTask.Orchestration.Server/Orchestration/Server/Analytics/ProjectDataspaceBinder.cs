// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Analytics.ProjectDataspaceBinder
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.DataAccess;
using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Analytics
{
  internal class ProjectDataspaceBinder : TaskAnalyticsDataBinderBase<ProjectDataspace>
  {
    private SqlColumnBinder projectId = new SqlColumnBinder("DataspaceIdentifier");
    private SqlColumnBinder dataspaceId = new SqlColumnBinder("DataspaceId");

    public ProjectDataspaceBinder(TaskSqlComponentBase sqlComponent)
      : base(sqlComponent)
    {
    }

    protected override ProjectDataspace Bind() => new ProjectDataspace()
    {
      ProjectGuid = this.projectId.GetGuid((IDataReader) this.Reader, false),
      DataspaceId = this.dataspaceId.GetInt32((IDataReader) this.Reader)
    };
  }
}
