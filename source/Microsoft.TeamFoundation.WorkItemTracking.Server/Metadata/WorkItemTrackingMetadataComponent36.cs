// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemTrackingMetadataComponent36
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
  internal class WorkItemTrackingMetadataComponent36 : WorkItemTrackingMetadataComponent35
  {
    protected override SqlParameter BindCustomFieldTable(
      string parameterName,
      IEnumerable<CustomFieldEntry> fieldEntries)
    {
      return this.BindBasicTvp<CustomFieldEntry>((WorkItemTrackingResourceComponent.TvpRecordBinder<CustomFieldEntry>) new WorkItemTrackingMetadataComponent36.CustomFieldTableRecordBinder3(), parameterName, fieldEntries);
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

    protected class CustomFieldTableRecordBinder3 : 
      WorkItemTrackingMetadataComponent24.CustomFieldTableRecordBinder2
    {
      public override string TypeName => "typ_WitCustomFieldTable6";

      protected override SqlMetaData[] TvpMetadata => new List<SqlMetaData>((IEnumerable<SqlMetaData>) base.TvpMetadata)
      {
        new SqlMetaData("IsIdentityFromProcess", SqlDbType.Bit)
      }.ToArray();

      public override void SetRecordValues(
        WorkItemTrackingSqlDataRecord record,
        CustomFieldEntry fieldEntry)
      {
        base.SetRecordValues(record, fieldEntry);
        record.SetBoolean(13, fieldEntry.IsIdentityFromProcess);
      }
    }
  }
}
