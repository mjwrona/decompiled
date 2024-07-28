// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemTrackingMetadataComponent25
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Internals;
using Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata
{
  internal class WorkItemTrackingMetadataComponent25 : WorkItemTrackingMetadataComponent24
  {
    internal override IDictionary<ConstantSetReference, SetRecord[]> GetDirectConstantSets(
      IEnumerable<ConstantSetReference> setReferences)
    {
      WorkItemTrackingMetadataComponent8.FilterableSetReferenceCollection setCollection = new WorkItemTrackingMetadataComponent8.FilterableSetReferenceCollection();
      setCollection.AddSetReferences(setReferences);
      this.PrepareStoredProcedure("prc_GetDirectConstantSets");
      this.BindUserSid();
      this.BindConstantSetReferenceTable("@sets", setReferences);
      return this.ExecuteUnknown<IDictionary<ConstantSetReference, SetRecord[]>>((System.Func<IDataReader, IDictionary<ConstantSetReference, SetRecord[]>>) (reader =>
      {
        WorkItemTrackingMetadataComponent25.DirectSetRecordBinder directSetRecordBinder = new WorkItemTrackingMetadataComponent25.DirectSetRecordBinder();
        while (reader.Read())
          setCollection.SetResult(directSetRecordBinder.Bind(reader));
        return setCollection.GetResult();
      }));
    }

    public override void SetCollectionWebLayoutVersion(int version)
    {
      this.PrepareDynamicProcedure("dynprc_SetCollectionWebLayoutVersion", "\r\n    SET NOCOUNT ON\r\n    SET XACT_ABORT ON\r\n\r\n    BEGIN TRAN\r\n        DECLARE @entries dbo.typ_KeyValuePairStringTableNullable\r\n        INSERT INTO @entries Values('#\\Service\\WorkItemTracking\\Settings\\CollectionWebLayoutVersion\\', @version);\r\n        EXEC prc_UpdateRegistry\r\n          @partitionId=@partitionId,\r\n          @registryUpdates = @entries\r\n\r\n        -- Stamp the form so VS will get the update\r\n        EXEC prc_ForceFormDownload @partitionId\r\n\r\n        -- Touching a row in types to force the cache to be cleared\r\n        UPDATE dbo.WorkItemTypes\r\n        SET fDeleted = fDeleted\r\n        WHERE PartitionId = @partitionId\r\n        AND WorkItemTypeID IN (SELECT TOP 1 WorkItemTypeID FROM dbo.WorkItemTypes WHERE PartitionId = @partitionId)\r\n        OPTION (OPTIMIZE FOR (@partitionId UNKNOWN))\r\n    COMMIT TRAN");
      this.BindInt("@version", version);
      this.ExecuteNonQuery();
    }

    protected class DirectSetRecordBinder : WorkItemTrackingObjectBinder<SetRecord>
    {
      private SqlColumnBinder ParentIdColumn = new SqlColumnBinder("ParentID");
      private SqlColumnBinder ItemIdColumn = new SqlColumnBinder("ItemID");
      private SqlColumnBinder ItemColumn = new SqlColumnBinder("Item");
      private SqlColumnBinder TeamFoundationId = new SqlColumnBinder(nameof (TeamFoundationId));
      private SqlColumnBinder IsListColumn = new SqlColumnBinder("IsList");

      public override SetRecord Bind(IDataReader reader) => new SetRecord()
      {
        ParentId = this.ParentIdColumn.GetInt32(reader),
        ItemId = this.ItemIdColumn.GetInt32(reader),
        Item = this.ItemColumn.GetString(reader, false),
        TeamFoundationId = this.TeamFoundationId.GetGuid(reader, true, Guid.Empty),
        IsList = this.IsListColumn.GetBoolean(reader),
        Direct = true,
        IncludeTop = false,
        IncludeGroups = true,
        Generation = 2
      };
    }
  }
}
