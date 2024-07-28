// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItemTypeUsageBinder
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class WorkItemTypeUsageBinder : ObjectBinder<WorkItemTypeUsage>
  {
    private SqlColumnBinder FieldIdColumn = new SqlColumnBinder("FieldID");
    private SqlColumnBinder WorkItemTypeIdColumn = new SqlColumnBinder("WorkItemTypeID");
    private SqlColumnBinder ProjectIdColumn = new SqlColumnBinder("ProjectId");

    public WorkItemTypeUsageBinder()
      : this(-1)
    {
    }

    public WorkItemTypeUsageBinder(int defaultProject) => this.DefaultProject = defaultProject;

    protected override WorkItemTypeUsage Bind() => new WorkItemTypeUsage()
    {
      FieldId = this.FieldIdColumn.GetInt32((IDataReader) this.Reader),
      WorkItemTypeId = this.WorkItemTypeIdColumn.GetInt32((IDataReader) this.Reader),
      ProjectId = this.ProjectIdColumn.GetInt32((IDataReader) this.Reader, this.DefaultProject, this.DefaultProject)
    };

    private int DefaultProject { get; set; }
  }
}
