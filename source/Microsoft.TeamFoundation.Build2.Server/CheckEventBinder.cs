// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.CheckEventBinder
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal sealed class CheckEventBinder : ObjectBinder<BuildCheckEvent>
  {
    private SqlColumnBinder m_dataspaceId = new SqlColumnBinder("DataspaceId");
    private SqlColumnBinder m_checkEventId = new SqlColumnBinder("CheckEventId");
    private SqlColumnBinder m_buildId = new SqlColumnBinder("BuildId");
    private SqlColumnBinder m_eventType = new SqlColumnBinder("EventType");
    private SqlColumnBinder m_payload = new SqlColumnBinder("Payload");
    private SqlColumnBinder m_status = new SqlColumnBinder("Status");
    private SqlColumnBinder m_createdTime = new SqlColumnBinder("CreatedTime");
    private SqlColumnBinder m_scheduledTime = new SqlColumnBinder("ScheduledTime");
    private SqlColumnBinder m_attempts = new SqlColumnBinder("Attempts");
    private BuildSqlComponentBase m_resourceComponent;
    private bool m_securityFixEnabled;

    public CheckEventBinder(BuildSqlComponentBase component, bool securityFixEnabled)
    {
      this.m_resourceComponent = component;
      this.m_securityFixEnabled = securityFixEnabled;
    }

    protected override BuildCheckEvent Bind() => new BuildCheckEvent()
    {
      ProjectId = this.m_resourceComponent.GetDataspaceIdentifier(this.m_dataspaceId.GetInt32((IDataReader) this.Reader)),
      CheckEventId = this.m_checkEventId.GetInt64((IDataReader) this.Reader),
      BuildId = this.m_buildId.GetInt32((IDataReader) this.Reader),
      EventType = (CheckEventType) this.m_eventType.GetByte((IDataReader) this.Reader),
      Payload = CheckEventSerializerUtil.Deserialize<IBuildCheckEventPayload>(this.m_payload.GetBytes((IDataReader) this.Reader, true), this.m_securityFixEnabled),
      Status = (CheckEventStatus) this.m_status.GetByte((IDataReader) this.Reader),
      CreatedTime = this.m_createdTime.GetDateTime((IDataReader) this.Reader),
      ScheduledTime = this.m_scheduledTime.GetDateTime((IDataReader) this.Reader),
      Attempts = this.m_attempts.GetByte((IDataReader) this.Reader)
    };
  }
}
