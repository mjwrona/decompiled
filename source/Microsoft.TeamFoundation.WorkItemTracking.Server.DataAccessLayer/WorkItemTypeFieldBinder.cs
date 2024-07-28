// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItemTypeFieldBinder
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class WorkItemTypeFieldBinder : ObjectBinder<DalWorkItemTypeField>
  {
    private SqlColumnBinder workItemTypeId = new SqlColumnBinder("WorkItemTypeId");
    private SqlColumnBinder workItemTypeName = new SqlColumnBinder("WorkItemTypeName");
    private SqlColumnBinder projectId = new SqlColumnBinder("ProjectId");
    private SqlColumnBinder fieldId = new SqlColumnBinder("FieldId");

    protected override DalWorkItemTypeField Bind() => new DalWorkItemTypeField()
    {
      WorkItemTypeId = this.workItemTypeId.GetInt32((IDataReader) this.Reader),
      WorkItemTypeName = this.workItemTypeName.GetString((IDataReader) this.Reader, false),
      ProjectId = this.projectId.GetInt32((IDataReader) this.Reader),
      FieldId = this.fieldId.GetInt32((IDataReader) this.Reader)
    };
  }
}
