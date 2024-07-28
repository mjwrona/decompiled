// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItemTypeRecordBinder
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class WorkItemTypeRecordBinder : ObjectBinder<WorkItemTypeRecord>
  {
    private SqlColumnBinder IdColumn = new SqlColumnBinder("ID");
    private SqlColumnBinder NameColumn = new SqlColumnBinder("Name");
    private SqlColumnBinder DescriptionColumn = new SqlColumnBinder("Description");
    private SqlColumnBinder ProjectIdColumn = new SqlColumnBinder("ProjectId");

    public WorkItemTypeRecordBinder()
      : this(-1)
    {
    }

    public WorkItemTypeRecordBinder(int defaultProject) => this.DefaultProject = defaultProject;

    protected override WorkItemTypeRecord Bind() => new WorkItemTypeRecord()
    {
      Id = this.IdColumn.GetInt32((IDataReader) this.Reader),
      Name = this.NameColumn.GetString((IDataReader) this.Reader, false),
      Description = this.DescriptionColumn.GetString((IDataReader) this.Reader, true),
      ProjectId = this.ProjectIdColumn.GetInt32((IDataReader) this.Reader, this.DefaultProject, this.DefaultProject)
    };

    private int DefaultProject { get; set; }
  }
}
