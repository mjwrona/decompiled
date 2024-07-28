// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemTrackingMetadataComponent10
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata
{
  internal class WorkItemTrackingMetadataComponent10 : WorkItemTrackingMetadataComponent9
  {
    public override WorkItemTypeEntry UpdateWorkItemTypeName(
      Guid projectId,
      Guid teamFoundationId,
      string oldWorkItemTypeName,
      string newWorkItemTypeName)
    {
      this.PrepareStoredProcedure("prc_UpdateWorkItemTypeName");
      this.BindGuid("@projectId", projectId);
      this.BindGuid("@teamFoundationId", teamFoundationId);
      this.BindString("@oldWorkItemTypeName", oldWorkItemTypeName, 256, false, SqlDbType.NVarChar);
      this.BindString("@newWorkItemTypeName", newWorkItemTypeName, 256, false, SqlDbType.NVarChar);
      return this.ExecuteUnknown<WorkItemTypeEntry>((System.Func<IDataReader, WorkItemTypeEntry>) (reader =>
      {
        reader.Read();
        WorkItemTypeRecord workItemTypeRecord = new WorkItemTrackingMetadataComponent.WorkItemTypeRecordBinder().Bind(reader);
        reader.NextResult();
        List<WorkItemTypeUsageRecord> list = new WorkItemTrackingMetadataComponent.WorkItemTypeUsageRecordBinder().BindAll(reader).ToList<WorkItemTypeUsageRecord>();
        WorkItemTypeEntry workItemTypeEntry = WorkItemTypeEntry.Create(workItemTypeRecord);
        foreach (WorkItemTypeUsageRecord itemTypeUsageRecord in list)
          workItemTypeEntry.AddField(itemTypeUsageRecord.FieldId);
        return workItemTypeEntry;
      }));
    }

    public override IEnumerable<IdentityConstantRecord> SearchConstantIdentityRecords(
      string searchTerm,
      SearchIdentityType identityType = SearchIdentityType.All)
    {
      this.PrepareStoredProcedure("prc_SearchConstantIdentityRecords");
      this.BindString("@searchTerm", searchTerm, 256, false, SqlDbType.NVarChar);
      return (IEnumerable<IdentityConstantRecord>) this.ExecuteUnknown<List<ConstantsSearchRecord>>((System.Func<IDataReader, List<ConstantsSearchRecord>>) (reader => new WorkItemTrackingMetadataComponent.SearchConstantRecordBinder().BindAll(reader).ToList<ConstantsSearchRecord>()));
    }

    public override IEnumerable<ConstantsSearchRecord> SearchConstantsRecords(
      IEnumerable<string> searchValues,
      IEnumerable<Guid> tfIds,
      bool includeInactiveIdentities,
      bool isHostedDeployment)
    {
      IEnumerable<string> rows = searchValues.Distinct<string>();
      this.PrepareStoredProcedure("prc_SearchConstantsNames");
      this.BindStringTable("@personNames", rows);
      return this.ExecuteUnknown<IEnumerable<ConstantsSearchRecord>>((System.Func<IDataReader, IEnumerable<ConstantsSearchRecord>>) (reader => (IEnumerable<ConstantsSearchRecord>) this.GetSearchConstantRecordBinder().BindAll(reader).ToList<ConstantsSearchRecord>()));
    }
  }
}
