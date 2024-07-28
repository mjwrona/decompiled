// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.TaskAgentRequestQueryContinuationTokenHelper
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using System;
using System.Text;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  internal static class TaskAgentRequestQueryContinuationTokenHelper
  {
    private const string c_continuationTokenSeparator = "|";

    internal static string GetContinuationToken(TaskAgentRequestQueryResult result)
    {
      StringBuilder stringBuilder = new StringBuilder();
      if (result.RunningRequests.Count > 0)
        stringBuilder.Append(result.RunningRequests[result.RunningRequests.Count - 1].AssignTime.Value.ToString());
      stringBuilder.Append("|");
      if (result.QueuedRequests.Count > 0)
        stringBuilder.Append(result.QueuedRequests[result.QueuedRequests.Count - 1].RequestId);
      stringBuilder.Append("|");
      if (result.FinishedRequests.Count > 0)
        stringBuilder.Append(result.FinishedRequests[result.FinishedRequests.Count - 1].FinishTime.Value.ToString());
      return stringBuilder.ToString();
    }

    internal static bool TryParseContinuationToken(
      string token,
      out DateTime? lastRunningAssignTime,
      out long? lastQueuedRequestId,
      out DateTime? lastFinishedFInishTime)
    {
      lastRunningAssignTime = new DateTime?();
      lastQueuedRequestId = new long?();
      lastFinishedFInishTime = new DateTime?();
      if (string.IsNullOrEmpty(token))
        return true;
      string[] strArray = token.Split(new string[1]{ "|" }, StringSplitOptions.None);
      if (strArray.Length != 3)
        return false;
      if (!string.IsNullOrEmpty(strArray[0]))
      {
        DateTime result;
        if (!DateTime.TryParse(strArray[0], out result))
          return false;
        lastRunningAssignTime = new DateTime?(result);
      }
      if (!string.IsNullOrEmpty(strArray[1]))
      {
        long result;
        if (!long.TryParse(strArray[1], out result))
          return false;
        lastQueuedRequestId = new long?(result);
      }
      if (!string.IsNullOrEmpty(strArray[2]))
      {
        DateTime result;
        if (!DateTime.TryParse(strArray[2], out result))
          return false;
        lastFinishedFInishTime = new DateTime?(result);
      }
      return true;
    }
  }
}
