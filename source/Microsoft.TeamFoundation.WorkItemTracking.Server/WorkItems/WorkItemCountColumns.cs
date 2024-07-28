// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItemCountColumns
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems
{
  internal class WorkItemCountColumns : ObjectBinder<WorkItemCountContainer>
  {
    private SqlColumnBinder m_hostId = new SqlColumnBinder("HostId");
    private SqlColumnBinder WorkItemCount = new SqlColumnBinder("count");

    protected override WorkItemCountContainer Bind() => new WorkItemCountContainer()
    {
      HostId = this.m_hostId.GetGuid((IDataReader) this.Reader, false),
      WorkItemCount = this.WorkItemCount.GetInt64((IDataReader) this.Reader)
    };
  }
}
