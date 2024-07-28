// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemTrackingMetadataComponent23
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels;
using System;
using System.Data;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata
{
  internal class WorkItemTrackingMetadataComponent23 : WorkItemTrackingMetadataComponent22
  {
    protected override WorkItemTrackingMetadataComponent.FieldRecordBinder GetFieldRecordBinder() => (WorkItemTrackingMetadataComponent.FieldRecordBinder) new WorkItemTrackingMetadataComponent23.FieldRecordBinder5();

    protected class FieldRecordBinder5 : WorkItemTrackingMetadataComponent19.FieldRecordBinder4
    {
      private SqlColumnBinder m_listId = new SqlColumnBinder("ListId");
      private SqlColumnBinder m_IsHistoryEnabled = new SqlColumnBinder("IsHistoryEnabled");
      private SqlColumnBinder m_isDeleted = new SqlColumnBinder("IsDeleted");

      public override FieldRecord Bind(IDataReader reader)
      {
        FieldRecord fieldRecord = base.Bind(reader);
        fieldRecord.PickListId = new Guid?(this.m_listId.GetGuid(reader, true));
        fieldRecord.IsHistoryEnabled = !this.m_IsHistoryEnabled.ColumnExists(reader) || this.m_IsHistoryEnabled.GetBoolean(reader);
        fieldRecord.IsDeleted = this.m_isDeleted.ColumnExists(reader) && this.m_isDeleted.GetBoolean(reader, false);
        return fieldRecord;
      }
    }
  }
}
