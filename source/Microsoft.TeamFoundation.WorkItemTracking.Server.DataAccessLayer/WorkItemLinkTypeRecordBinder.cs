// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItemLinkTypeRecordBinder
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class WorkItemLinkTypeRecordBinder : ObjectBinder<WorkItemLinkTypeRecord>
  {
    private SqlColumnBinder ReferenceNameColumn = new SqlColumnBinder("ReferenceName");
    private SqlColumnBinder ForwardNameColumn = new SqlColumnBinder("ForwardName");
    private SqlColumnBinder ForwardIdColumn = new SqlColumnBinder("ForwardID");
    private SqlColumnBinder ReverseNameColumn = new SqlColumnBinder("ReverseName");
    private SqlColumnBinder ReverseIdColumn = new SqlColumnBinder("ReverseID");
    private SqlColumnBinder RulesColumn = new SqlColumnBinder("Rules");

    protected override WorkItemLinkTypeRecord Bind() => new WorkItemLinkTypeRecord()
    {
      ReferenceName = this.ReferenceNameColumn.GetString((IDataReader) this.Reader, false),
      ForwardName = this.ForwardNameColumn.GetString((IDataReader) this.Reader, false),
      ForwardId = (int) this.ForwardIdColumn.GetInt16((IDataReader) this.Reader),
      ReverseName = this.ReverseNameColumn.GetString((IDataReader) this.Reader, false),
      ReverseId = (int) this.ReverseIdColumn.GetInt16((IDataReader) this.Reader),
      Rules = this.RulesColumn.GetInt32((IDataReader) this.Reader)
    };
  }
}
