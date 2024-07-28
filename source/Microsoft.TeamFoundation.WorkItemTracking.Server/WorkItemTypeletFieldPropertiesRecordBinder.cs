// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItemTypeletFieldPropertiesRecordBinder
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessMetadata;
using System;
using System.ComponentModel;
using System.Data;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  internal class WorkItemTypeletFieldPropertiesRecordBinder : 
    ObjectBinder<WorkItemTypeletFieldProperties>
  {
    private SqlColumnBinder m_typeletIdColumn = new SqlColumnBinder("TypeletId");
    private SqlColumnBinder m_fieldIdColumn = new SqlColumnBinder("FieldId");
    private SqlColumnBinder m_isSuggestedColumn = new SqlColumnBinder("IsSuggested");

    protected override WorkItemTypeletFieldProperties Bind() => new WorkItemTypeletFieldProperties()
    {
      TypeletId = new Guid?(this.m_typeletIdColumn.GetGuid((IDataReader) this.Reader)),
      FieldId = this.m_fieldIdColumn.GetInt32((IDataReader) this.Reader),
      IsSuggested = this.m_isSuggestedColumn.IsNull((IDataReader) this.Reader) ? new bool?() : new bool?(this.m_isSuggestedColumn.GetBoolean((IDataReader) this.Reader))
    };
  }
}
