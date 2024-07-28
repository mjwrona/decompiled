// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemTrackingMetadataComponent9
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata
{
  internal class WorkItemTrackingMetadataComponent9 : WorkItemTrackingMetadataComponent8
  {
    protected override bool BindPartitionIdForInstallWorkItemWordsContains => true;

    protected override WorkItemTrackingMetadataComponent.FieldRecordBinder GetFieldRecordBinder() => (WorkItemTrackingMetadataComponent.FieldRecordBinder) new WorkItemTrackingMetadataComponent9.FieldRecordBinder2();

    protected override void BindUserSid()
    {
    }

    protected class FieldRecordBinder2 : WorkItemTrackingMetadataComponent.FieldRecordBinder
    {
      public FieldRecordBinder2()
      {
        this.m_fieldId = new SqlColumnBinder("FieldId");
        this.m_fieldReportable = new SqlColumnBinder("IsReportingEnabled");
        this.m_fieldOftenQueriedAsText = new SqlColumnBinder("IsOftenQueriedAsText");
        this.m_fieldSupportsTextQuery = new SqlColumnBinder("SupportsTextQuery");
        this.m_fieldCore = new SqlColumnBinder("IsCore");
        this.m_fieldObjectId = new SqlColumnBinder("ObjectId");
        this.m_isIdentityField = new SqlColumnBinder("IsIdentityField");
      }
    }
  }
}
