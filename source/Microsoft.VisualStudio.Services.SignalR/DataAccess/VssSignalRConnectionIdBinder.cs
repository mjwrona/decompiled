// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.SignalR.DataAccess.VssSignalRConnectionIdBinder
// Assembly: Microsoft.VisualStudio.Services.SignalR, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: BD148864-3B8A-4D7D-BD16-EF04E9549DC9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.SignalR.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.VisualStudio.Services.SignalR.DataAccess
{
  internal class VssSignalRConnectionIdBinder : ObjectBinder<string>
  {
    private SqlColumnBinder m_connectionId = new SqlColumnBinder("ConnectionId");

    protected override string Bind() => this.m_connectionId.GetString((IDataReader) this.Reader, false);
  }
}
