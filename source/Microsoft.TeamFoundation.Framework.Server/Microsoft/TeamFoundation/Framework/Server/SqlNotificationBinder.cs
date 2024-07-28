// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.SqlNotificationBinder
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class SqlNotificationBinder : ObjectBinder<SqlNotification>
  {
    private SqlColumnBinder m_eventClass = new SqlColumnBinder("EventClass");
    private SqlColumnBinder m_eventId = new SqlColumnBinder("EventId");
    private SqlColumnBinder m_eventData = new SqlColumnBinder("EventData");
    private SqlColumnBinder m_eventAuthor = new SqlColumnBinder("EventAuthor");
    private SqlColumnBinder m_hostId = new SqlColumnBinder("HostId");
    private bool m_isGlobal;

    public SqlNotificationBinder(bool isGlobal) => this.m_isGlobal = isGlobal;

    protected override SqlNotification Bind() => new SqlNotification()
    {
      EventClass = this.m_eventClass.GetGuid((IDataReader) this.Reader),
      EventId = this.m_eventId.GetInt64((IDataReader) this.Reader),
      EventData = this.m_eventData.GetString((IDataReader) this.Reader, true),
      EventAuthor = this.m_eventAuthor.GetGuid((IDataReader) this.Reader, true),
      HostId = this.m_hostId.GetGuid((IDataReader) this.Reader, true, Guid.Empty),
      IsGlobal = this.m_isGlobal
    };
  }
}
