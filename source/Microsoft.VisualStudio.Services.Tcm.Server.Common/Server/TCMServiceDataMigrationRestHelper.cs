// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TCMServiceDataMigrationRestHelper
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal class TCMServiceDataMigrationRestHelper
  {
    internal static void BlockRequestsIfDataMigrationInProgressOrFailed(
      IVssRequestContext requestContext,
      int testRunId)
    {
      if (testRunId <= 0 || !TCMServiceDataMigrationRestHelper.IsRunInTfs(requestContext, testRunId))
        return;
      TCMServiceDataMigrationRestHelper.BlockRequestsIfDataMigrationInProgressOrFailed(requestContext);
    }

    internal static void BlockRequestsIfDataMigrationInProgressOrFailed(
      IVssRequestContext requestContext)
    {
      if (TCMServiceDataMigrationRestHelper.IsMigrationInProgressOrFailed(requestContext))
        throw new TCMDataMigrationInProgressException(ServerResources.TCMDataMigrationInProgressError);
    }

    internal static List<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun> FilterTfsRunsBelowThresholdFromTCM(
      TestManagementRequestContext requestContext,
      List<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun> runs)
    {
      if (runs == null || runs.Count == 0)
        return runs;
      int runIdThreshold = TCMServiceDataMigrationRestHelper.GetRunIdThreshold(requestContext.RequestContext);
      if (!requestContext.IsTcmService || !TCMServiceDataMigrationRestHelper.IsMigrationInProgressOrFailed(requestContext.RequestContext) || runIdThreshold == int.MaxValue)
        return runs;
      return requestContext.IsFeatureEnabled("TestManagement.Server.DisableCallToTfsForTestRun") ? runs.Where<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun>((Func<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun, bool>) (run => run.Id >= runIdThreshold)).ToList<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun>() : runs.Where<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun>((Func<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun, bool>) (run => run.Id >= runIdThreshold && run.Id % 2 == 0)).ToList<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun>();
    }

    internal static List<TestRun> FilterTfsRunsBelowThresholdFromTCM(
      TestManagementRequestContext requestContext,
      List<TestRun> runs)
    {
      if (runs == null || runs.Count == 0)
        return runs;
      int runIdThreshold = TCMServiceDataMigrationRestHelper.GetRunIdThreshold(requestContext.RequestContext);
      if (!requestContext.IsTcmService || !TCMServiceDataMigrationRestHelper.IsMigrationInProgressOrFailed(requestContext.RequestContext) || runIdThreshold == int.MaxValue)
        return runs;
      return requestContext.IsFeatureEnabled("TestManagement.Server.DisableCallToTfsForTestRun") ? runs.Where<TestRun>((Func<TestRun, bool>) (run => run.TestRunId >= runIdThreshold)).ToList<TestRun>() : runs.Where<TestRun>((Func<TestRun, bool>) (run => run.TestRunId >= runIdThreshold && run.TestRunId % 2 == 0)).ToList<TestRun>();
    }

    internal static IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> FilterTfsResultsBelowThresholdFromTCM(
      TestManagementRequestContext requestContext,
      IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> testCaseResults)
    {
      if (testCaseResults == null || testCaseResults.Count == 0)
        return testCaseResults;
      int runIdThreshold = TCMServiceDataMigrationRestHelper.GetRunIdThreshold(requestContext.RequestContext);
      if (!requestContext.IsTcmService || !TCMServiceDataMigrationRestHelper.IsMigrationInProgressOrFailed(requestContext.RequestContext) || runIdThreshold == int.MaxValue)
        return testCaseResults;
      return requestContext.IsFeatureEnabled("TestManagement.Server.DisableCallToTfsForTestRun") ? (IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>) testCaseResults.Where<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>((Func<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult, bool>) (testCaseResult => Convert.ToInt32(testCaseResult.TestRun.Id) >= runIdThreshold)).ToList<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>() : (IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>) testCaseResults.Where<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>((Func<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult, bool>) (testCaseResult => Convert.ToInt32(testCaseResult.TestRun.Id) >= runIdThreshold && Convert.ToInt32(testCaseResult.TestRun.Id) % 2 == 0)).ToList<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>();
    }

    internal static List<TestCaseResult> FilterTfsResultsBelowThresholdFromTCM(
      TestManagementRequestContext requestContext,
      List<TestCaseResult> testCaseResults)
    {
      if (testCaseResults == null || testCaseResults.Count == 0)
        return testCaseResults;
      int runIdThreshold = TCMServiceDataMigrationRestHelper.GetRunIdThreshold(requestContext.RequestContext);
      if (!requestContext.IsTcmService || !TCMServiceDataMigrationRestHelper.IsMigrationInProgressOrFailed(requestContext.RequestContext) || runIdThreshold == int.MaxValue)
        return testCaseResults;
      return requestContext.IsFeatureEnabled("TestManagement.Server.DisableCallToTfsForTestRun") ? testCaseResults.Where<TestCaseResult>((Func<TestCaseResult, bool>) (testCaseResult => testCaseResult.TestRunId >= runIdThreshold)).ToList<TestCaseResult>() : testCaseResults.Where<TestCaseResult>((Func<TestCaseResult, bool>) (testCaseResult => testCaseResult.TestRunId >= runIdThreshold && testCaseResult.TestRunId % 2 == 0)).ToList<TestCaseResult>();
    }

    internal static IList<ShallowTestCaseResult> FilterTfsShallowResultsBelowThresholdFromTCM(
      TestManagementRequestContext requestContext,
      IList<ShallowTestCaseResult> testCaseShallowResults)
    {
      if (testCaseShallowResults == null || testCaseShallowResults.Count == 0)
        return testCaseShallowResults;
      int runIdThreshold = TCMServiceDataMigrationRestHelper.GetRunIdThreshold(requestContext.RequestContext);
      if (!requestContext.IsTcmService || !TCMServiceDataMigrationRestHelper.IsMigrationInProgressOrFailed(requestContext.RequestContext) || runIdThreshold == int.MaxValue)
        return testCaseShallowResults;
      return requestContext.IsFeatureEnabled("TestManagement.Server.DisableCallToTfsForTestRun") ? (IList<ShallowTestCaseResult>) testCaseShallowResults.Where<ShallowTestCaseResult>((Func<ShallowTestCaseResult, bool>) (testCaseShallowResult => testCaseShallowResult.RunId >= runIdThreshold)).ToList<ShallowTestCaseResult>() : (IList<ShallowTestCaseResult>) testCaseShallowResults.Where<ShallowTestCaseResult>((Func<ShallowTestCaseResult, bool>) (testCaseShallowResult => testCaseShallowResult.RunId >= runIdThreshold && testCaseShallowResult.RunId % 2 == 0)).ToList<ShallowTestCaseResult>();
    }

    internal static bool IsMigrationCompleted(IVssRequestContext context) => context.ExecutionEnvironment.IsOnPremisesDeployment || context.GetService<IVssRegistryService>().GetValue<int>(context, (RegistryQuery) "/Service/TestManagement/TCMServiceDataMigration/MigrationStatus", 0) == 2;

    internal static int GetRunIdThresholdIfDataMigrationInProgressOrFailed(
      IVssRequestContext requestContext)
    {
      return new TestManagementRequestContext(requestContext).IsTcmService && TCMServiceDataMigrationRestHelper.IsMigrationInProgressOrFailed(requestContext) ? TCMServiceDataMigrationRestHelper.GetRunIdThreshold(requestContext) : 0;
    }

    internal static void InvokeAction(Action func, IVssRequestContext requestContext)
    {
      if (!new TestManagementRequestContext(requestContext).IsTcmService && TCMServiceDataMigrationRestHelper.IsMigrationCompleted(requestContext))
        return;
      func();
    }

    internal static bool IsRunningInTfsAndMigrationInProgressOrFailed(
      IVssRequestContext requestContext)
    {
      return !new TestManagementRequestContext(requestContext).IsTcmService && TCMServiceDataMigrationRestHelper.IsMigrationInProgressOrFailed(requestContext);
    }

    private static bool IsRunInTfs(IVssRequestContext requestContext, int runId)
    {
      if (requestContext.IsFeatureEnabled("TestManagement.Server.DisableCallToTfsForTestRun"))
        return false;
      int runIdThreshold = TCMServiceDataMigrationRestHelper.GetRunIdThreshold(requestContext);
      return runIdThreshold != int.MaxValue && runId < runIdThreshold || runId % 2 != 0;
    }

    private static int GetRunIdThreshold(IVssRequestContext requestContext) => requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) "/Service/TestManagement/Settings/TcmServiceTestRunIdThreshold", int.MaxValue);

    private static bool IsMigrationInProgressOrFailed(IVssRequestContext requestContext)
    {
      switch ((TCMServiceDataMigrationStatus) requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) "/Service/TestManagement/TCMServiceDataMigration/MigrationStatus", 0))
      {
        case TCMServiceDataMigrationStatus.InProgress:
        case TCMServiceDataMigrationStatus.Failed:
          return true;
        default:
          return false;
      }
    }
  }
}
