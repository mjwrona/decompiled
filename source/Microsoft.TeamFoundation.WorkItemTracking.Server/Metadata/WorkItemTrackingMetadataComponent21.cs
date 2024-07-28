// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemTrackingMetadataComponent21
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata
{
  internal class WorkItemTrackingMetadataComponent21 : WorkItemTrackingMetadataComponent20
  {
    protected virtual SqlParameter BindUsedConstantIdsTable(
      string parameterName,
      IReadOnlyCollection<int> usedConstantIds)
    {
      return this.BindBasicTvp<int>((WorkItemTrackingResourceComponent.TvpRecordBinder<int>) new WorkItemTrackingMetadataComponent21.UsedConstantIdsTableRecordBinder(), parameterName, (IEnumerable<int>) usedConstantIds);
    }

    internal override void CleanupConstants(
      IReadOnlyCollection<int> usedConstants,
      long constantsMetadataStamp)
    {
      this.PrepareStoredProcedure(nameof (CleanupConstants));
      this.BindUsedConstantIdsTable("@usedConstants", usedConstants);
      this.BindLong("@metadataStamp", constantsMetadataStamp);
      this.ExecuteNonQueryEx();
    }

    protected class UsedConstantIdsTableRecordBinder : 
      WorkItemTrackingResourceComponent.TvpRecordBinder<int>
    {
      protected static readonly SqlMetaData[] s_metadata = new SqlMetaData[1]
      {
        new SqlMetaData("Val", SqlDbType.Int)
      };

      public override string TypeName => "typ_Int32Table";

      protected override SqlMetaData[] TvpMetadata => WorkItemTrackingMetadataComponent21.UsedConstantIdsTableRecordBinder.s_metadata;

      public override void SetRecordValues(WorkItemTrackingSqlDataRecord record, int entry) => record.SetInt32(0, entry);
    }
  }
}
