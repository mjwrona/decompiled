// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.QueryExecutionInformationHelper
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  public static class QueryExecutionInformationHelper
  {
    public static IEnumerable<QueryExecutionHistory> MergeWithQueryHistories(
      ICollection<QueryExecutionInformation> queryExecutionInfo,
      IEnumerable<QueryExecutionHistory> queryHistories,
      int historyRecordCountLimit)
    {
      ArgumentUtility.CheckForNull<ICollection<QueryExecutionInformation>>(queryExecutionInfo, nameof (queryExecutionInfo));
      if (queryHistories == null)
        queryHistories = (IEnumerable<QueryExecutionHistory>) new List<QueryExecutionHistory>();
      return queryExecutionInfo.GroupBy<QueryExecutionInformation, string>((Func<QueryExecutionInformation, string>) (q => q.QueryHash)).Select(g => new
      {
        History = new QueryExecutionHistory()
        {
          QueryHash = g.Key,
          ExecutionRecords = g.SelectMany<QueryExecutionInformation, QueryExecutionRecord>((Func<QueryExecutionInformation, IEnumerable<QueryExecutionRecord>>) (q => q.ExecutionHistory.Where<QueryExecutionRecord>((Func<QueryExecutionRecord, bool>) (history => history != null))))
        },
        NeedResetHistory = g.Any<QueryExecutionInformation>((Func<QueryExecutionInformation, bool>) (q => q.NeedResetHistory))
      }).GroupJoin(queryHistories, localRecord => localRecord.History.QueryHash, (Func<QueryExecutionHistory, string>) (remoteRecord => remoteRecord.QueryHash), (localRecord, remoteRecords) =>
      {
        localRecord.History.ExecutionRecords = !localRecord.NeedResetHistory ? localRecord.History.ExecutionRecords.Concat<QueryExecutionRecord>(remoteRecords.SelectMany<QueryExecutionHistory, QueryExecutionRecord>((Func<QueryExecutionHistory, IEnumerable<QueryExecutionRecord>>) (rr => rr.ExecutionRecords))).GroupBy<QueryExecutionRecord, DateTime>((Func<QueryExecutionRecord, DateTime>) (r => r.Bucket)).Select<IGrouping<DateTime, QueryExecutionRecord>, QueryExecutionRecord>((Func<IGrouping<DateTime, QueryExecutionRecord>, QueryExecutionRecord>) (g => g.Aggregate<QueryExecutionRecord>((Func<QueryExecutionRecord, QueryExecutionRecord, QueryExecutionRecord>) ((r1, r2) => r1.DurationInMs >= r2.DurationInMs ? r2 : r1)))).OrderByDescending<QueryExecutionRecord, DateTime>((Func<QueryExecutionRecord, DateTime>) (r => r.Bucket)).Take<QueryExecutionRecord>(historyRecordCountLimit) : (IEnumerable<QueryExecutionRecord>) new List<QueryExecutionRecord>();
        return localRecord.History;
      });
    }
  }
}
