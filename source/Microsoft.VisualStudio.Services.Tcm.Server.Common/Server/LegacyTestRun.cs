// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.LegacyTestRun
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Core.WebApi;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  public class LegacyTestRun
  {
    public int TestRunId { get; internal set; }

    public Guid TmiRunId { get; internal set; }

    public string Title { get; internal set; }

    public string OwnerName { get; internal set; }

    public string BuildPlatform { get; internal set; }

    public string BuildFlavor { get; internal set; }

    public string TeamProject { get; internal set; }

    internal string TeamProjectUri { get; set; }

    internal Guid Owner { get; set; }

    internal int QueryStatistics(
      TestManagementRequestContext context,
      out int passedTests,
      out int failedTests)
    {
      int num = 0;
      passedTests = 0;
      failedTests = 0;
      foreach (TestRunStatistic testRunStatistic in TestRunStatistic.Query(context, this.TeamProject, this.TestRunId))
      {
        num += testRunStatistic.Count;
        if (testRunStatistic.Outcome == (byte) 2)
          passedTests += testRunStatistic.Count;
        else if (testRunStatistic.Outcome == (byte) 3 || testRunStatistic.Outcome == (byte) 6 || testRunStatistic.Outcome == (byte) 5 || testRunStatistic.Outcome == (byte) 10)
          failedTests += testRunStatistic.Count;
      }
      return num;
    }

    internal static List<LegacyTestRun> Query(
      TestManagementRequestContext context,
      string buildUri,
      string buildPlatform,
      string buildFlavor)
    {
      List<LegacyTestRun> runs = (List<LegacyTestRun>) null;
      Dictionary<Guid, List<LegacyTestRun>> projectsRunsMap = new Dictionary<Guid, List<LegacyTestRun>>();
      using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
        runs = managementDatabase.QueryLegacyTestRuns(buildUri, buildPlatform, buildFlavor, out projectsRunsMap);
      LegacyTestRun.UpdateProjectDataForLegacyRuns(context, projectsRunsMap);
      LegacyTestRun.CheckViewTestRunsPermission(context, runs);
      LegacyTestRun.SetOwnerNames(context, runs);
      return runs;
    }

    internal static void UpdateProjectDataForLegacyRuns(
      TestManagementRequestContext context,
      Dictionary<Guid, List<LegacyTestRun>> projectsRunsMap)
    {
      if (projectsRunsMap == null || !projectsRunsMap.Any<KeyValuePair<Guid, List<LegacyTestRun>>>())
        return;
      foreach (Guid key in projectsRunsMap.Keys)
      {
        ProjectInfo projectFromGuid = context.ProjectServiceHelper.GetProjectFromGuid(key);
        foreach (LegacyTestRun legacyTestRun in projectsRunsMap[key])
        {
          legacyTestRun.TeamProject = projectFromGuid.Name;
          legacyTestRun.TeamProjectUri = projectFromGuid.Uri;
        }
      }
    }

    private static void SetOwnerNames(
      TestManagementRequestContext context,
      List<LegacyTestRun> runs)
    {
      Dictionary<Guid, string> dictionary = new Dictionary<Guid, string>();
      foreach (LegacyTestRun run in runs)
        dictionary[run.Owner] = (string) null;
      foreach (KeyValuePair<Guid, string> resolveIdentity in IdentityHelper.ResolveIdentities(context, (IEnumerable<Guid>) dictionary.Keys))
      {
        if (resolveIdentity.Value != null)
          dictionary[resolveIdentity.Key] = resolveIdentity.Value;
      }
      foreach (LegacyTestRun run in runs)
      {
        string str;
        run.OwnerName = !dictionary.TryGetValue(run.Owner, out str) ? run.Owner.ToString("D", (IFormatProvider) CultureInfo.InvariantCulture) : str;
      }
    }

    private static void CheckViewTestRunsPermission(
      TestManagementRequestContext context,
      List<LegacyTestRun> runs)
    {
      int count = runs.Count;
      Dictionary<string, bool> dictionary = new Dictionary<string, bool>((IEqualityComparer<string>) TFStringComparer.ProjectUri);
      int index = 0;
      while (index < runs.Count)
      {
        bool flag;
        if (!dictionary.TryGetValue(runs[index].TeamProjectUri, out flag))
        {
          flag = context.SecurityManager.HasViewTestResultsPermission(context, runs[index].TeamProjectUri);
          dictionary[runs[index].TeamProjectUri] = flag;
        }
        if (flag)
          ++index;
        else
          runs.RemoveAt(index);
      }
      context.TraceInfo("BusinessLayer", "Filtered out {0} runs. Returning {1} runs.", (object) (count - runs.Count), (object) runs.Count);
    }
  }
}
