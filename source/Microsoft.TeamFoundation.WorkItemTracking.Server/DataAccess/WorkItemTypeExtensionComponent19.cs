// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess.WorkItemTypeExtensionComponent19
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess
{
  internal class WorkItemTypeExtensionComponent19 : WorkItemTypeExtensionComponent18
  {
    protected override SqlParameter BindCustomFieldTable(
      string parameterName,
      IEnumerable<CustomFieldEntry> fieldEntries)
    {
      return this.BindBasicTvp<CustomFieldEntry>((WorkItemTrackingResourceComponent.TvpRecordBinder<CustomFieldEntry>) new WorkItemTypeExtensionComponent19.CustomFieldTableRecordBinder5(), parameterName, fieldEntries);
    }

    protected class CustomFieldTableRecordBinder5 : 
      WorkItemTypeExtensionComponent10.CustomFieldTableRecordBinder4
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
