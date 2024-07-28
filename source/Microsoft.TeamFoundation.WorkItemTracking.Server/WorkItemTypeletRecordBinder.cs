// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItemTypeletRecordBinder
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.ComponentModel;
using System.Data;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  internal class WorkItemTypeletRecordBinder : ObjectBinder<WorkItemTypeletRecord>
  {
    private SqlColumnBinder m_columnId = new SqlColumnBinder("Id");
    private SqlColumnBinder m_columnTypeletType = new SqlColumnBinder("TypeletType");
    private SqlColumnBinder m_columnName = new SqlColumnBinder("Name");
    private SqlColumnBinder m_columnReferenceName = new SqlColumnBinder("ReferenceName");
    private SqlColumnBinder m_columnDescription = new SqlColumnBinder("Description");
    private SqlColumnBinder m_columnRules = new SqlColumnBinder("Rules");
    private SqlColumnBinder m_columnForm = new SqlColumnBinder("Form");
    private SqlColumnBinder m_columnLastChangeDate = new SqlColumnBinder("LastChangeDate");
    private SqlColumnBinder m_columnProcessId = new SqlColumnBinder("ProcessId");
    private SqlColumnBinder m_columnParentTypeRefName = new SqlColumnBinder("ParentTypeRefName");

    protected IDataReader Reader => this.BaseReader;

    protected override WorkItemTypeletRecord Bind()
    {
      IDataReader reader = this.Reader;
      return new WorkItemTypeletRecord()
      {
        Id = this.m_columnId.GetGuid(reader),
        ProcessId = this.m_columnProcessId.GetGuid(reader),
        Name = this.m_columnName.GetString(reader, false),
        ReferenceName = this.m_columnReferenceName.GetString(reader, false),
        Description = this.m_columnDescription.GetString(reader, true),
        Rules = this.m_columnRules.GetString(reader, true),
        Form = this.m_columnForm.GetString(reader, true),
        LastChangeDate = this.m_columnLastChangeDate.GetDateTime(reader, DateTime.MinValue),
        ParentTypeRefName = this.m_columnParentTypeRefName.GetString(reader, true),
        TypeletType = this.m_columnTypeletType.GetInt32(reader),
        IsAbstract = false
      };
    }
  }
}
