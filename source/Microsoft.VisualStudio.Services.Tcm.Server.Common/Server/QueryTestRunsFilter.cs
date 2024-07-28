// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.QueryTestRunsFilter
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  public class QueryTestRunsFilter
  {
    private static readonly string s_dateTimeFormat = "yyyy-MM-dd HH:mm:ss.fff";

    internal int State { get; set; }

    internal DateTime? MinLastUpdatedDate { get; set; }

    internal DateTime? MaxLastUpdatedDate { get; set; }

    internal string SourceWorkflow { get; set; }

    internal List<int> PlanIds { get; set; }

    internal bool? IsAutomated { get; set; }

    internal List<int> BuildIds { get; set; }

    internal List<int> BuildDefIds { get; set; }

    internal string BranchName { get; set; }

    internal List<int> ReleaseIds { get; set; }

    internal List<int> ReleaseDefIds { get; set; }

    internal List<int> ReleaseEnvIds { get; set; }

    internal List<int> ReleaseEnvDefIds { get; set; }

    internal string RunTitle { get; set; }

    internal int MinNextBatchTestRunId { get; set; }

    internal static string GetContinuationToken(DateTime lastUpdated) => string.Format("{0}", (object) lastUpdated.ToString(QueryTestRunsFilter.s_dateTimeFormat, (IFormatProvider) CultureInfo.InvariantCulture));

    internal static void ParseContinuationToken(string continuationToken, out DateTime lastUpdated)
    {
      lastUpdated = DateTime.MaxValue;
      DateTime result;
      if (string.IsNullOrEmpty(continuationToken) || !DateTime.TryParseExact(continuationToken, QueryTestRunsFilter.s_dateTimeFormat, (IFormatProvider) CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal, out result))
        return;
      lastUpdated = result;
    }
  }
}
