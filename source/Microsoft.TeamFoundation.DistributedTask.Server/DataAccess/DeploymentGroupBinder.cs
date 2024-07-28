// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.DeploymentGroupBinder
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  internal sealed class DeploymentGroupBinder : DistributedTaskObjectBinder<DeploymentGroup>
  {
    private SqlColumnBinder m_queueId = new SqlColumnBinder("QueueId");
    private SqlColumnBinder m_dataspaceId = new SqlColumnBinder("DataspaceId");
    private SqlColumnBinder m_queueName = new SqlColumnBinder("QueueName");
    private SqlColumnBinder m_poolId = new SqlColumnBinder("PoolId");
    private SqlColumnBinder m_description = new SqlColumnBinder("Description");
    private SqlColumnBinder m_size = new SqlColumnBinder("Size");

    public DeploymentGroupBinder(TaskResourceComponent resourceComponent)
      : base(resourceComponent)
    {
    }

    protected override DeploymentGroup Bind()
    {
      DeploymentGroup deploymentGroup1 = new DeploymentGroup();
      deploymentGroup1.Id = this.m_queueId.GetInt32((IDataReader) this.Reader);
      deploymentGroup1.Project = new ProjectReference()
      {
        Id = this.ResourceComponent.GetDataspaceIdentifier(this.m_dataspaceId.GetInt32((IDataReader) this.Reader))
      };
      deploymentGroup1.Name = this.m_queueName.GetString((IDataReader) this.Reader, false);
      deploymentGroup1.Pool = new TaskAgentPoolReference()
      {
        Id = this.m_poolId.GetInt32((IDataReader) this.Reader),
        PoolType = TaskAgentPoolType.Deployment
      };
      DeploymentGroup deploymentGroup2 = deploymentGroup1;
      if (this.m_description.ColumnExists((IDataReader) this.Reader))
        deploymentGroup2.Description = this.m_description.GetString((IDataReader) this.Reader, true);
      if (this.m_size.ColumnExists((IDataReader) this.Reader))
        deploymentGroup2.MachineCount = this.m_size.GetInt32((IDataReader) this.Reader);
      return deploymentGroup2;
    }
  }
}
