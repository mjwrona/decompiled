// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TraceFlagStatusBinder
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class TraceFlagStatusBinder : ObjectBinder<TraceFlagStatus>
  {
    private SqlColumnBinder traceFlag = new SqlColumnBinder("TraceFlag");
    private SqlColumnBinder status = new SqlColumnBinder("Status");
    private SqlColumnBinder global = new SqlColumnBinder("Global");
    private SqlColumnBinder session = new SqlColumnBinder("Session");

    protected override TraceFlagStatus Bind() => new TraceFlagStatus(this.traceFlag.GetInt16((IDataReader) this.Reader), this.status.GetInt16((IDataReader) this.Reader), this.global.GetInt16((IDataReader) this.Reader), this.session.GetInt16((IDataReader) this.Reader));
  }
}
