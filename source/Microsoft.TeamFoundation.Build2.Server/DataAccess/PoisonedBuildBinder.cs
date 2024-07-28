// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.DataAccess.PoisonedBuildBinder
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Build2.Server.ObjectModel;
using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.Build2.Server.DataAccess
{
  internal class PoisonedBuildBinder : ObjectBinder<PoisonedBuild>
  {
    private SqlColumnBinder m_buildId = new SqlColumnBinder("BuildId");
    private SqlColumnBinder m_OrchestrationId = new SqlColumnBinder("OrchestrationId");

    protected override PoisonedBuild Bind() => new PoisonedBuild()
    {
      BuildId = this.m_buildId.GetInt32((IDataReader) this.Reader),
      OrchestrationId = this.m_OrchestrationId.GetGuid((IDataReader) this.Reader, true)
    };
  }
}
