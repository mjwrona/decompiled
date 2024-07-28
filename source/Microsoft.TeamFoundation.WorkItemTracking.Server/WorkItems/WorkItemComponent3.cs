// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItemComponent3
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels;
using Microsoft.VisualStudio.Services.Identity;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems
{
  internal class WorkItemComponent3 : WorkItemComponent2
  {
    protected override void BindUpdateReconciledWorkItemsChangedByColumn(
      IVssIdentity changedByIdentity)
    {
      this.BindString("@changedBy", changedByIdentity.DisplayName, 256, false, SqlDbType.NVarChar);
    }

    protected override SqlParameter BindWorkItemIdRevPairsForGetWorkItemFieldValues(
      string paramName,
      IEnumerable<WorkItemIdRevisionPair> workItemIdRevPairs)
    {
      return this.BindWorkItemIdRevPairs(paramName, workItemIdRevPairs);
    }

    public override IEnumerable<KeyValuePair<int, int>> UpdateReconciledWorkItems(
      IEnumerable<WorkItemIdRevisionPair> workItemIdRevPairs,
      IEnumerable<WorkItemCustomFieldUpdateRecord> updateRecords,
      IVssIdentity callerIdentity,
      IVssIdentity changedByIdentity,
      int trendInterval,
      bool dualSave,
      bool continueWhenDateTimeShiftDetected = true,
      bool skipWITChangeDateUpdate = false)
    {
      this.PrepareStoredProcedure("prc_UpdateReconciledWorkItems");
      this.BindWorkItemIdRevPairs("@idRevs", workItemIdRevPairs);
      this.BindCustomFieldUpdates("@customFieldUpdates", updateRecords);
      this.BindIdentityColumn(callerIdentity);
      this.BindUpdateReconciledWorkItemsChangedByColumn(changedByIdentity);
      this.BindInt("@trendInterval", trendInterval);
      this.BindBoolean("@dualSave", dualSave);
      this.BindBoolean("@continueWhenDateTimeShiftDetected", continueWhenDateTimeShiftDetected);
      return this.ExecuteUnknown<IEnumerable<KeyValuePair<int, int>>>((System.Func<IDataReader, IEnumerable<KeyValuePair<int, int>>>) (reader =>
      {
        List<KeyValuePair<int, int>> keyValuePairList = new List<KeyValuePair<int, int>>();
        while (reader.Read())
          keyValuePairList.Add(new KeyValuePair<int, int>(reader.GetInt32(0), reader.GetInt32(1)));
        return (IEnumerable<KeyValuePair<int, int>>) keyValuePairList;
      }));
    }
  }
}
