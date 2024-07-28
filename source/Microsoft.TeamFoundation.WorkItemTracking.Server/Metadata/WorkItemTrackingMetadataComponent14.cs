// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemTrackingMetadataComponent14
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels;
using System.Data;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata
{
  internal class WorkItemTrackingMetadataComponent14 : WorkItemTrackingMetadataComponent13
  {
    public override void SetIdentityFieldBit()
    {
      this.PrepareStoredProcedure("prc_SetIdentityFieldBit");
      this.ExecuteNonQuery();
    }

    protected class ConstantRecordBinder2 : WorkItemTrackingMetadataComponent.ConstantRecordBinder
    {
      private SqlColumnBinder HasUniqueIdentityDisplayNameColumn = new SqlColumnBinder("HasUniqueIdentityDisplayName");

      public ConstantRecordBinder2(bool backCompat = false)
        : base(backCompat)
      {
      }

      public override ConstantRecord Bind(IDataReader reader)
      {
        ConstantRecord constantRecord = base.Bind(reader);
        constantRecord.HasUniqueIdentityDisplayName = this.HasUniqueIdentityDisplayNameColumn.GetBoolean(reader, true);
        return constantRecord;
      }
    }
  }
}
