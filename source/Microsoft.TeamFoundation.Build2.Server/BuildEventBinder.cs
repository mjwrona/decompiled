// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.BuildEventBinder
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal sealed class BuildEventBinder : ObjectBinder<BuildChangeEvent>
  {
    private SqlColumnBinder m_dataspaceId = new SqlColumnBinder("DataspaceId");
    private SqlColumnBinder m_buildEventId = new SqlColumnBinder("BuildEventId");
    private SqlColumnBinder m_buildId = new SqlColumnBinder("BuildId");
    private SqlColumnBinder m_eventType = new SqlColumnBinder("EventType");
    private SqlColumnBinder m_status = new SqlColumnBinder("Status");
    private SqlColumnBinder m_createdTime = new SqlColumnBinder("CreatedTime");
    private SqlColumnBinder m_modifiedTime = new SqlColumnBinder("ModifiedTime");
    private BuildSqlComponentBase m_resourceComponent;

    public BuildEventBinder(BuildSqlComponentBase component) => this.m_resourceComponent = component;

    protected override BuildChangeEvent Bind() => new BuildChangeEvent()
    {
      ProjectId = this.m_resourceComponent.GetDataspaceIdentifier(this.m_dataspaceId.GetInt32((IDataReader) this.Reader)),
      BuildEventId = this.m_buildEventId.GetInt64((IDataReader) this.Reader),
      BuildId = this.m_buildId.GetInt32((IDataReader) this.Reader),
      EventType = (BuildEventType) this.m_eventType.GetByte((IDataReader) this.Reader),
      Status = (BuildEventStatus) this.m_status.GetByte((IDataReader) this.Reader),
      CreatedTime = this.m_createdTime.GetDateTime((IDataReader) this.Reader),
      ModifiedTime = new DateTime?(this.m_modifiedTime.GetDateTime((IDataReader) this.Reader))
    };
  }
}
