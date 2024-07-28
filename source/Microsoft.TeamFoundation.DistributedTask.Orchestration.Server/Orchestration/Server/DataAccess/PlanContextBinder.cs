// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.DataAccess.PlanContextBinder
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.DataAccess
{
  internal class PlanContextBinder : ObjectBinder<PlanContext>
  {
    private SqlColumnBinder m_contextName = new SqlColumnBinder("ContextName");
    private SqlColumnBinder m_contextType = new SqlColumnBinder("ContextType");
    private SqlColumnBinder m_contextData = new SqlColumnBinder("ContextData");

    protected override PlanContext Bind() => new PlanContext()
    {
      ContextName = this.m_contextName.GetString((IDataReader) this.Reader, false),
      ContextType = this.m_contextType.GetString((IDataReader) this.Reader, false),
      ContextData = this.m_contextData.GetBytes((IDataReader) this.Reader, true)
    };
  }
}
