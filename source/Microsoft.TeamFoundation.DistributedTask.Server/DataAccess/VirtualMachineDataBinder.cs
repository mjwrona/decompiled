// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.VirtualMachineDataBinder
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  internal sealed class VirtualMachineDataBinder : ObjectBinder<VirtualMachineData>
  {
    private SqlColumnBinder m_virtualMachineId = new SqlColumnBinder("VMAgentId");
    private SqlColumnBinder m_agentId = new SqlColumnBinder("AgentId");
    private SqlColumnBinder m_environmentId = new SqlColumnBinder("EnvironmentId");
    private SqlColumnBinder m_resourceId = new SqlColumnBinder("ResourceId");

    protected override VirtualMachineData Bind() => new VirtualMachineData()
    {
      VMAgentId = this.m_virtualMachineId.GetInt32((IDataReader) this.Reader),
      AgentId = this.m_agentId.GetInt32((IDataReader) this.Reader),
      EnvironmentId = this.m_environmentId.GetInt32((IDataReader) this.Reader),
      ResourceId = this.m_resourceId.GetInt32((IDataReader) this.Reader)
    };
  }
}
