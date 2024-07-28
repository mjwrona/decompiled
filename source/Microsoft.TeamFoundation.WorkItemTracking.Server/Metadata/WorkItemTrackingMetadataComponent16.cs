// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemTrackingMetadataComponent16
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels;
using System.Data;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata
{
  internal class WorkItemTrackingMetadataComponent16 : WorkItemTrackingMetadataComponent15
  {
    protected override WorkItemTrackingMetadataComponent.SearchConstantRecordBinder GetSearchConstantRecordBinder() => (WorkItemTrackingMetadataComponent.SearchConstantRecordBinder) new WorkItemTrackingMetadataComponent16.SearchConstantRecordBinder2();

    protected class SearchConstantRecordBinder2 : 
      WorkItemTrackingMetadataComponent.SearchConstantRecordBinder
    {
      private SqlColumnBinder HasUniqueIdentityDisplayNameColumn = new SqlColumnBinder("HasUniqueIdentityDisplayName");

      public override ConstantsSearchRecord Bind(IDataReader reader)
      {
        ConstantsSearchRecord constantsSearchRecord = base.Bind(reader);
        constantsSearchRecord.HasUniqueIdentityDisplayName = this.HasUniqueIdentityDisplayNameColumn.GetBoolean(reader, true);
        return constantsSearchRecord;
      }
    }
  }
}
