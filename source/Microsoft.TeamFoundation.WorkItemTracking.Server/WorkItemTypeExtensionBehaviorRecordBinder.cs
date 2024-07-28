// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItemTypeExtensionBehaviorRecordBinder
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.ComponentModel;
using System.Data;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  internal class WorkItemTypeExtensionBehaviorRecordBinder : 
    ObjectBinder<WorkItemTypeExtensionBehaviorRecord>
  {
    private SqlColumnBinder m_workItemTypeIdColumn = new SqlColumnBinder("WorkItemTypeId");
    private SqlColumnBinder m_behaviorReferenceNameColumn = new SqlColumnBinder("BehaviorReferenceName");
    private SqlColumnBinder m_isDefaultColumn = new SqlColumnBinder("IsDefault");

    protected override WorkItemTypeExtensionBehaviorRecord Bind() => new WorkItemTypeExtensionBehaviorRecord()
    {
      WorkItemTypeId = this.m_workItemTypeIdColumn.GetGuid(this.Reader),
      BehaviorReferenceName = this.m_behaviorReferenceNameColumn.GetString(this.Reader, false),
      IsDefault = this.m_isDefaultColumn.GetBoolean(this.Reader)
    };

    protected IDataReader Reader => this.BaseReader;
  }
}
