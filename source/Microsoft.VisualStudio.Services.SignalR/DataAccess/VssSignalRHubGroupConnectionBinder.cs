// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.SignalR.DataAccess.VssSignalRHubGroupConnectionBinder
// Assembly: Microsoft.VisualStudio.Services.SignalR, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: BD148864-3B8A-4D7D-BD16-EF04E9549DC9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.SignalR.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.VisualStudio.Services.SignalR.DataAccess
{
  internal class VssSignalRHubGroupConnectionBinder : ObjectBinder<VssSignalRHubGroupConnection>
  {
    private SqlColumnBinder m_hubName = new SqlColumnBinder("HubName");
    private SqlColumnBinder m_groupName = new SqlColumnBinder("GroupName");
    private SqlColumnBinder m_connectionId = new SqlColumnBinder("ConnectionId");
    private SqlColumnBinder m_userId = new SqlColumnBinder("UserId");

    protected override VssSignalRHubGroupConnection Bind() => new VssSignalRHubGroupConnection()
    {
      GroupId = new VssSignalRHubGroupId(this.m_hubName.GetString((IDataReader) this.Reader, false), this.m_groupName.GetString((IDataReader) this.Reader, false)),
      ConnectionId = this.m_connectionId.GetString((IDataReader) this.Reader, false),
      UserId = this.m_userId.GetGuid((IDataReader) this.Reader)
    };
  }
}
