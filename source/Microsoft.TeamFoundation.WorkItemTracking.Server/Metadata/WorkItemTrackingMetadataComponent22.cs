// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemTrackingMetadataComponent22
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata
{
  internal class WorkItemTrackingMetadataComponent22 : WorkItemTrackingMetadataComponent21
  {
    protected SqlParameter BindFieldIdTable(string parameterName, IEnumerable<int> fieldIds) => this.BindBasicTvp<int>((WorkItemTrackingResourceComponent.TvpRecordBinder<int>) new WorkItemTrackingMetadataComponent22.FieldIdTableRecordBinder(), parameterName, fieldIds);

    internal override void UpdateField(
      string referenceName,
      string description,
      Guid changedBy,
      Guid? convertToPicklistId,
      bool? isIdentityFromProcess)
    {
      this.PrepareStoredProcedure("prc_UpdateCustomField");
      this.BindString("@referenceName", referenceName, 386, false, SqlDbType.NVarChar);
      this.BindString("@description", description, 256, false, SqlDbType.NVarChar);
      this.BindGuid("@changedBy", changedBy);
      this.ExecuteNonQuery();
    }

    internal override void CreateFields(
      IReadOnlyCollection<CustomFieldEntry> fields,
      Guid changedBy)
    {
      this.PrepareStoredProcedure("prc_ProvisionCustomFields");
      this.BindCustomFieldTable("@customFields", (IEnumerable<CustomFieldEntry>) fields);
      this.BindGuid("@changedBy", changedBy);
      this.ExecuteNonQuery();
    }

    internal override void DeleteProcess(Guid processId, Guid changedBy, bool deleteFields = true)
    {
      this.PrepareStoredProcedure("prc_DeleteProcess");
      this.BindGuid("@processId", processId);
      this.BindGuid("@changedBy", changedBy);
      this.ExecuteNonQuery();
    }

    public override void DeleteFields(IReadOnlyCollection<int> fieldIds, Guid teamFoundationId)
    {
      this.PrepareStoredProcedure("prc_DeleteCustomFields");
      this.BindFieldIdTable("@fieldsToDelete", (IEnumerable<int>) fieldIds);
      this.BindGuid("@changedBy", teamFoundationId);
      this.ExecuteNonQueryEx();
    }

    protected class FieldIdTableRecordBinder : WorkItemTrackingResourceComponent.TvpRecordBinder<int>
    {
      protected static readonly SqlMetaData[] s_metadata = new SqlMetaData[1]
      {
        new SqlMetaData("Val", SqlDbType.Int)
      };

      public override string TypeName => "typ_Int32Table";

      protected override SqlMetaData[] TvpMetadata => WorkItemTrackingMetadataComponent22.FieldIdTableRecordBinder.s_metadata;

      public override void SetRecordValues(WorkItemTrackingSqlDataRecord record, int entry) => record.SetInt32(0, entry);
    }
  }
}
