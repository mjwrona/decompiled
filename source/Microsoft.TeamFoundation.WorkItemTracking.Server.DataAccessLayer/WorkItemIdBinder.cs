// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItemIdBinder
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class WorkItemIdBinder : ObjectBinder<WorkItemId>
  {
    private long m_offset;
    private SqlColumnBinder id = new SqlColumnBinder("ID");
    private SqlColumnBinder rowVersion = new SqlColumnBinder("RowVersion");

    public WorkItemIdBinder(long offset) => this.m_offset = offset;

    protected override WorkItemId Bind() => new WorkItemId()
    {
      Id = this.id.GetInt32((IDataReader) this.Reader),
      RowVersion = this.rowVersion.GetInt64((IDataReader) this.Reader) + this.m_offset
    };
  }
}
