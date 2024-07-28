// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemTrackingMetadataComponent55
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata
{
  internal class WorkItemTrackingMetadataComponent55 : WorkItemTrackingMetadataComponent54
  {
    protected override SqlParameter BindCustomFieldTable(
      string parameterName,
      IEnumerable<CustomFieldEntry> fieldEntries)
    {
      return this.BindBasicTvp<CustomFieldEntry>((WorkItemTrackingResourceComponent.TvpRecordBinder<CustomFieldEntry>) new WorkItemTrackingMetadataComponent55.CustomFieldTableRecordBinder4(), parameterName, fieldEntries);
    }

    protected class CustomFieldTableRecordBinder4 : 
      WorkItemTrackingMetadataComponent36.CustomFieldTableRecordBinder3
    {
      private static readonly SqlMetaData[] s_metadata = new SqlMetaData[15]
      {
        new SqlMetaData("Name", SqlDbType.NVarChar, 128L),
        new SqlMetaData("ReferenceName", SqlDbType.NVarChar, 386L),
        new SqlMetaData("Type", SqlDbType.Int),
        new SqlMetaData("ReportingType", SqlDbType.Int),
        new SqlMetaData("ReportingFormula", SqlDbType.Int),
        new SqlMetaData("ReportingEnabled", SqlDbType.Bit),
        new SqlMetaData("ReportingName", SqlDbType.NVarChar, 128L),
        new SqlMetaData("ReportingReferenceName", SqlDbType.NVarChar, 386L),
        new SqlMetaData("Usage", SqlDbType.Int),
        new SqlMetaData("ParentFieldId", SqlDbType.Int),
        new SqlMetaData("ProcessId", SqlDbType.UniqueIdentifier),
        new SqlMetaData("Description", SqlDbType.NVarChar, 256L),
        new SqlMetaData("PicklistId", SqlDbType.UniqueIdentifier),
        new SqlMetaData("IsIdentityFromProcess", SqlDbType.Bit),
        new SqlMetaData("IsLocked", SqlDbType.Bit)
      };

      public override string TypeName => "typ_WitCustomFieldTable7";

      protected override SqlMetaData[] TvpMetadata => WorkItemTrackingMetadataComponent55.CustomFieldTableRecordBinder4.s_metadata;

      public override void SetRecordValues(
        WorkItemTrackingSqlDataRecord record,
        CustomFieldEntry fieldEntry)
      {
        base.SetRecordValues(record, fieldEntry);
        record.SetBoolean(14, fieldEntry.IsLocked);
      }
    }
  }
}
