// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemTrackingMetadataComponent18
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels;
using System.Data;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata
{
  internal class WorkItemTrackingMetadataComponent18 : WorkItemTrackingMetadataComponent17
  {
    protected override WorkItemTrackingMetadataComponent.FieldRecordBinder GetFieldRecordBinder() => (WorkItemTrackingMetadataComponent.FieldRecordBinder) new WorkItemTrackingMetadataComponent18.FieldRecordBinder3();

    protected class FieldRecordBinder3 : WorkItemTrackingMetadataComponent9.FieldRecordBinder2
    {
      protected SqlColumnBinder m_parentFieldId;

      public FieldRecordBinder3() => this.m_parentFieldId = new SqlColumnBinder("ParentFieldId");

      public override FieldRecord Bind(IDataReader reader)
      {
        FieldRecord fieldRecord = base.Bind(reader);
        fieldRecord.ParentFieldId = this.m_parentFieldId.GetInt32(reader);
        return fieldRecord;
      }
    }
  }
}
