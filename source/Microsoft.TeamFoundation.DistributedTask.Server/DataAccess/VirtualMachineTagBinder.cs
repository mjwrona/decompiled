// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.VirtualMachineTagBinder
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  internal sealed class VirtualMachineTagBinder : ObjectBinder<VirtualMachineTag>
  {
    private SqlColumnBinder m_vmAgentId = new SqlColumnBinder("VMAgentId");
    private SqlColumnBinder m_tag = new SqlColumnBinder("Tag");

    protected override VirtualMachineTag Bind() => new VirtualMachineTag()
    {
      VMAgentId = this.m_vmAgentId.GetInt32((IDataReader) this.Reader),
      Tag = this.m_tag.GetString((IDataReader) this.Reader, false)
    };
  }
}
