// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.Legacy.TestRunStatisticConverter
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.TeamFoundation.TestManagement.WebApi.Legacy;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server.Legacy
{
  internal static class TestRunStatisticConverter
  {
    public static Microsoft.TeamFoundation.TestManagement.Server.TestResolutionState Convert(
      Microsoft.TeamFoundation.TestManagement.WebApi.TestResolutionState resolutionState)
    {
      if (resolutionState == null)
        return (Microsoft.TeamFoundation.TestManagement.Server.TestResolutionState) null;
      return new Microsoft.TeamFoundation.TestManagement.Server.TestResolutionState()
      {
        Id = resolutionState.Id,
        Name = resolutionState.Name,
        TeamProject = resolutionState.project?.Name
      };
    }

    public static Microsoft.TeamFoundation.TestManagement.WebApi.TestResolutionState Convert(
      Microsoft.TeamFoundation.TestManagement.Server.TestResolutionState resolutionState)
    {
      if (resolutionState == null)
        return (Microsoft.TeamFoundation.TestManagement.WebApi.TestResolutionState) null;
      return new Microsoft.TeamFoundation.TestManagement.WebApi.TestResolutionState()
      {
        Id = resolutionState.Id,
        Name = resolutionState.Name,
        project = new ShallowReference()
        {
          Name = resolutionState.Name
        }
      };
    }

    public static Microsoft.TeamFoundation.TestManagement.Server.TestRunStatistic Convert(
      LegacyTestRunStatistic testRunStatistic)
    {
      if (testRunStatistic == null)
        return (Microsoft.TeamFoundation.TestManagement.Server.TestRunStatistic) null;
      return new Microsoft.TeamFoundation.TestManagement.Server.TestRunStatistic()
      {
        TestRunId = testRunStatistic.TestRunId,
        State = testRunStatistic.State,
        Outcome = testRunStatistic.Outcome,
        ResolutionState = TestRunStatisticConverter.Convert(testRunStatistic.ResolutionState),
        Count = testRunStatistic.Count
      };
    }

    public static LegacyTestRunStatistic Convert(Microsoft.TeamFoundation.TestManagement.Server.TestRunStatistic testRunStatistic)
    {
      if (testRunStatistic == null)
        return (LegacyTestRunStatistic) null;
      return new LegacyTestRunStatistic()
      {
        TestRunId = testRunStatistic.TestRunId,
        State = testRunStatistic.State,
        Outcome = testRunStatistic.Outcome,
        ResolutionState = TestRunStatisticConverter.Convert(testRunStatistic.ResolutionState),
        Count = testRunStatistic.Count
      };
    }

    public static IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.TestRunStatistic> Convert(
      IEnumerable<LegacyTestRunStatistic> testRunStatistics)
    {
      return testRunStatistics == null ? (IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.TestRunStatistic>) null : testRunStatistics.Select<LegacyTestRunStatistic, Microsoft.TeamFoundation.TestManagement.Server.TestRunStatistic>((Func<LegacyTestRunStatistic, Microsoft.TeamFoundation.TestManagement.Server.TestRunStatistic>) (stats => TestRunStatisticConverter.Convert(stats)));
    }

    public static IEnumerable<LegacyTestRunStatistic> Convert(
      IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.TestRunStatistic> testRunStatistics)
    {
      return testRunStatistics == null ? (IEnumerable<LegacyTestRunStatistic>) null : testRunStatistics.Select<Microsoft.TeamFoundation.TestManagement.Server.TestRunStatistic, LegacyTestRunStatistic>((Func<Microsoft.TeamFoundation.TestManagement.Server.TestRunStatistic, LegacyTestRunStatistic>) (stats => TestRunStatisticConverter.Convert(stats)));
    }
  }
}
