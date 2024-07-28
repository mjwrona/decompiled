// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess.WorkItemTypeExtensionComponent18
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessMetadata;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess
{
  internal class WorkItemTypeExtensionComponent18 : WorkItemTypeExtensionComponent17
  {
    protected override IEnumerable<WorkItemTypeletFieldProperties> GetWorkItemTypeletFieldPropertiesInternal(
      Guid? typeletId = null)
    {
      this.PrepareStoredProcedure("prc_GetWorkItemTypeletFieldProperties");
      if (typeletId.HasValue)
        this.BindGuid("@typeletId", typeletId.Value);
      ResultCollection resultCollection = new ResultCollection(this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<WorkItemTypeletFieldProperties>((ObjectBinder<WorkItemTypeletFieldProperties>) new WorkItemTypeletFieldPropertiesRecordBinder());
      return (IEnumerable<WorkItemTypeletFieldProperties>) resultCollection.GetCurrent<WorkItemTypeletFieldProperties>().Items;
    }

    internal override void UpdateDefaultWorkItemTypeForBehavior(
      Guid processId,
      Guid witId,
      string behaviorReferenceName,
      Guid changerId,
      bool isDefault)
    {
      this.PrepareStoredProcedure("prc_UpdateDefaultWorkItemTypeForBehavior");
      this.BindGuid("@processId", processId);
      this.BindGuid("@workItemTypeId", witId);
      this.BindString("@behaviorReferenceName", behaviorReferenceName, 386, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindBoolean("@isDefault", isDefault);
      this.BindGuid("@changerId", changerId);
      this.ExecuteNonQueryEx();
    }

    protected class WorkItemTypeletFieldPropertiesBinder : 
      WorkItemTrackingResourceComponent.TvpRecordBinder<WorkItemTypeletFieldProperties>
    {
      private static readonly SqlMetaData[] s_metadata = new SqlMetaData[3]
      {
        new SqlMetaData("TypeletId", SqlDbType.UniqueIdentifier),
        new SqlMetaData("FieldId", SqlDbType.Int),
        new SqlMetaData("IsSuggested", SqlDbType.Bit)
      };

      public override string TypeName => "typ_WorkItemTypeletFieldPropertyTable2";

      protected override SqlMetaData[] TvpMetadata => WorkItemTypeExtensionComponent18.WorkItemTypeletFieldPropertiesBinder.s_metadata;

      public override void SetRecordValues(
        WorkItemTrackingSqlDataRecord record,
        WorkItemTypeletFieldProperties properties)
      {
        if (properties.TypeletId.HasValue && properties.TypeletId.Value != Guid.Empty)
          record.SetGuid(0, properties.TypeletId.Value);
        record.SetInt32(1, properties.FieldId);
        bool? isSuggested = properties.IsSuggested;
        if (!isSuggested.HasValue)
          return;
        WorkItemTrackingSqlDataRecord trackingSqlDataRecord = record;
        isSuggested = properties.IsSuggested;
        int num = isSuggested.Value ? 1 : 0;
        trackingSqlDataRecord.SetBoolean(2, num != 0);
      }
    }
  }
}
