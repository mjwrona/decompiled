// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Orchestration.Server.DataAccess.OrchestrationSessionBinder2
// Assembly: Microsoft.VisualStudio.Services.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 953225F5-5DFE-4840-B8F7-3B94A5257E43
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Orchestration.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.VisualStudio.Services.Orchestration.Server.DataAccess
{
  internal sealed class OrchestrationSessionBinder2 : ObjectBinder<OrchestrationSession>
  {
    private SqlColumnBinder m_sessionId = new SqlColumnBinder("SessionId");
    private SqlColumnBinder m_compressionType = new SqlColumnBinder("CompressionType");
    private SqlColumnBinder m_state = new SqlColumnBinder("State");
    private SqlColumnBinder m_evaluationTime = new SqlColumnBinder("EvaluationTime");
    private SqlColumnBinder m_nextRunTime = new SqlColumnBinder("NextRunTime");

    protected override OrchestrationSession Bind() => new OrchestrationSession()
    {
      SessionId = this.m_sessionId.GetString((IDataReader) this.Reader, false),
      CompressionType = (CompressionType) this.m_compressionType.GetByte((IDataReader) this.Reader),
      State = this.m_state.IsNull((IDataReader) this.Reader) ? (byte[]) null : this.m_state.GetBytes((IDataReader) this.Reader, true),
      NextRun = this.m_nextRunTime.GetDateTime((IDataReader) this.Reader) - this.m_evaluationTime.GetDateTime((IDataReader) this.Reader)
    };
  }
}
