// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.XEventSessionBinder
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class XEventSessionBinder : ObjectBinder<XEventSession>
  {
    private SqlColumnBinder m_session_id = new SqlColumnBinder("session_id");
    private SqlColumnBinder m_session_name = new SqlColumnBinder("session_name");
    private SqlColumnBinder m_is_event_file_truncated = new SqlColumnBinder("is_event_file_truncated");
    private SqlColumnBinder m_buffers_logged = new SqlColumnBinder("buffers_logged");
    private SqlColumnBinder m_buffers_dropped = new SqlColumnBinder("buffers_dropped");
    private SqlColumnBinder m_event_file_name = new SqlColumnBinder("file_name");

    internal XEventSessionBinder()
    {
    }

    protected override XEventSession Bind() => new XEventSession(this.m_session_id.GetInt32((IDataReader) this.Reader, 0), this.m_session_name.GetString((IDataReader) this.Reader, true), this.m_is_event_file_truncated.GetBoolean((IDataReader) this.Reader, false), this.m_buffers_logged.GetInt32((IDataReader) this.Reader, 0), this.m_buffers_dropped.GetInt32((IDataReader) this.Reader, 0), this.m_event_file_name.GetString((IDataReader) this.Reader, true));
  }
}
