// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItemTypeActionBinder
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class WorkItemTypeActionBinder : ObjectBinder<WorkItemTypeAction>
  {
    private SqlColumnBinder fromState = new SqlColumnBinder("FromState");
    private SqlColumnBinder toState = new SqlColumnBinder("ToState");
    private SqlColumnBinder actionName = new SqlColumnBinder("Name");
    private SqlColumnBinder workItemType = new SqlColumnBinder("WorkItemType");

    protected override WorkItemTypeAction Bind()
    {
      WorkItemTypeAction workItemTypeAction = new WorkItemTypeAction();
      if (this.workItemType.ColumnExists((IDataReader) this.Reader))
        workItemTypeAction.WorkItemType = this.workItemType.GetString((IDataReader) this.Reader, false);
      workItemTypeAction.FromState = this.fromState.GetString((IDataReader) this.Reader, false);
      workItemTypeAction.ToState = this.toState.GetString((IDataReader) this.Reader, false);
      workItemTypeAction.Name = this.actionName.GetString((IDataReader) this.Reader, false);
      return workItemTypeAction;
    }
  }
}
