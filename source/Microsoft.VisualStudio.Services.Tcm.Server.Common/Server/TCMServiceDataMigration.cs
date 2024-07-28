// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TCMServiceDataMigration
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.VisualStudio.Services.Tcm.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  public class TCMServiceDataMigration
  {
    internal static void SyncMigrationStatus(
      TestManagementRequestContext context,
      TCMServiceDataMigrationStatus migrationStatus)
    {
      TCMServiceDataMigration.AuthenticateS2SCall(context.RequestContext);
      if (migrationStatus == TCMServiceDataMigrationStatus.InProgress)
        TCMServiceDataMigration.UpdateTCMThresholds(context);
      context.RequestContext.GetService<IVssRegistryService>().SetValue<int>(context.RequestContext, "/Service/TestManagement/TCMServiceDataMigration/MigrationStatus", (int) migrationStatus);
    }

    internal static void SyncReleaseRefs(
      TestManagementRequestContext context,
      IEnumerable<ReleaseReference2> releaseRefs)
    {
      TCMServiceDataMigration.AuthenticateS2SCall(context.RequestContext);
      using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
        managementDatabase.SyncReleaseRefs(releaseRefs);
    }

    internal static void SyncBuildRefs(
      TestManagementRequestContext context,
      IEnumerable<BuildReference2> buildRefs)
    {
      TCMServiceDataMigration.AuthenticateS2SCall(context.RequestContext);
      using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
        managementDatabase.SyncBuildRefs(buildRefs);
    }

    internal static void SyncTestRunContexts(
      TestManagementRequestContext context,
      IEnumerable<TestRunContext2> testRunContexts)
    {
      TCMServiceDataMigration.AuthenticateS2SCall(context.RequestContext);
      using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
        managementDatabase.SyncTestRunContexts(testRunContexts);
    }

    internal static void SyncTestMessageLogs(
      TestManagementRequestContext context,
      IEnumerable<TestMessageLog2> testMessageLogs)
    {
      TCMServiceDataMigration.AuthenticateS2SCall(context.RequestContext);
      using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
        managementDatabase.SyncTestMessageLogs(testMessageLogs);
    }

    internal static void SyncTestCaseReferences(
      TestManagementRequestContext context,
      IEnumerable<TestCaseReference2> testCaseReferences)
    {
      TCMServiceDataMigration.AuthenticateS2SCall(context.RequestContext);
      using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
        managementDatabase.SyncTestCaseReferences(testCaseReferences);
    }

    internal static void SyncTestResults(
      TestManagementRequestContext context,
      IEnumerable<TestResult2> testResults)
    {
      TCMServiceDataMigration.AuthenticateS2SCall(context.RequestContext);
      using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
        managementDatabase.SyncTestResults(testResults);
    }

    internal static void SyncTestMessageLogEntry(
      TestManagementRequestContext context,
      IEnumerable<TestMessageLogEntry2> testMessageLogEntry)
    {
      TCMServiceDataMigration.AuthenticateS2SCall(context.RequestContext);
      using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
        managementDatabase.SyncTestMessageLogEntry(testMessageLogEntry);
    }

    internal static void SyncTestRuns(
      TestManagementRequestContext context,
      IEnumerable<TestRun2> testRuns)
    {
      TCMServiceDataMigration.AuthenticateS2SCall(context.RequestContext);
      using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
        managementDatabase.SyncTestRuns(testRuns);
    }

    internal static void SyncTestRuns2(
      TestManagementRequestContext context,
      IEnumerable<TestRun2> testRuns)
    {
      TCMServiceDataMigration.AuthenticateS2SCall(context.RequestContext);
      using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
        managementDatabase.SyncTestRuns2(testRuns);
    }

    internal static void SyncTestRunSummary(
      TestManagementRequestContext context,
      IEnumerable<TestRunSummary2> testRunSummary)
    {
      TCMServiceDataMigration.AuthenticateS2SCall(context.RequestContext);
      using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
        managementDatabase.SyncTestRunSummary(testRunSummary);
    }

    internal static void SyncTestResultsEx(
      TestManagementRequestContext context,
      IEnumerable<TestResultsEx2> testResultsEx)
    {
      TCMServiceDataMigration.AuthenticateS2SCall(context.RequestContext);
      using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
        managementDatabase.SyncTestResultsEx(testResultsEx);
    }

    internal static void SyncTestCaseMetadata(
      TestManagementRequestContext context,
      IEnumerable<TestCaseMetadata2> testCaseMetadata2s)
    {
      TCMServiceDataMigration.AuthenticateS2SCall(context.RequestContext);
      using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
        managementDatabase.SyncTestCaseMetadata(testCaseMetadata2s);
    }

    internal static void SyncRequirementsToTestsMapping(
      TestManagementRequestContext context,
      IEnumerable<RequirementsToTestsMapping2> requirementsToTestsMapping2s)
    {
      TCMServiceDataMigration.AuthenticateS2SCall(context.RequestContext);
      using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
        managementDatabase.SyncRequirementsToTestsMapping(requirementsToTestsMapping2s);
    }

    internal static void SyncTestResultReset(
      TestManagementRequestContext context,
      IEnumerable<TestResultReset2> testResultsResets)
    {
      TCMServiceDataMigration.AuthenticateS2SCall(context.RequestContext);
      using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
        managementDatabase.SyncTestResultReset(testResultsResets);
    }

    internal static void SyncTestActionResult(
      TestManagementRequestContext context,
      IEnumerable<TestActionResult2> testActionResults)
    {
      TCMServiceDataMigration.AuthenticateS2SCall(context.RequestContext);
      using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
        managementDatabase.SyncTestActionResult(testActionResults);
    }

    internal static void SyncTestRunEx(
      TestManagementRequestContext context,
      IEnumerable<TestRunEx2> testRunEx)
    {
      TCMServiceDataMigration.AuthenticateS2SCall(context.RequestContext);
      using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
        managementDatabase.SyncTestRunEx(testRunEx);
    }

    internal static void SyncTestRunExtended(
      TestManagementRequestContext context,
      IEnumerable<TestRunExtended2> testRunExtended)
    {
      TCMServiceDataMigration.AuthenticateS2SCall(context.RequestContext);
      using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
        managementDatabase.SyncTestRunExtended(testRunExtended);
    }

    internal static void SyncTestParameter(
      TestManagementRequestContext context,
      IEnumerable<TestParameter2> testParameter)
    {
      TCMServiceDataMigration.AuthenticateS2SCall(context.RequestContext);
      using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
        managementDatabase.SyncTestParameter(testParameter);
    }

    internal static void SyncCodeCoverageSummary(
      TestManagementRequestContext context,
      IEnumerable<CodeCoverageSummary2> codeCoverageSummary2)
    {
      TCMServiceDataMigration.AuthenticateS2SCall(context.RequestContext);
      using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
        managementDatabase.SyncCodeCoverageSummary(codeCoverageSummary2);
    }

    internal static void SyncCoverage(
      TestManagementRequestContext context,
      IEnumerable<Coverage2> codeCoverages)
    {
      TCMServiceDataMigration.AuthenticateS2SCall(context.RequestContext);
      using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
        managementDatabase.SyncCoverage(codeCoverages);
    }

    internal static void SyncFunctionCoverage(
      TestManagementRequestContext context,
      IEnumerable<FunctionCoverage2> functionCoverage)
    {
      TCMServiceDataMigration.AuthenticateS2SCall(context.RequestContext);
      using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
        managementDatabase.SyncFunctionCoverage(functionCoverage);
    }

    internal static void SyncModuleCoverage(
      TestManagementRequestContext context,
      IEnumerable<ModuleCoverage2> moduleCoverage)
    {
      TCMServiceDataMigration.AuthenticateS2SCall(context.RequestContext);
      using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
        managementDatabase.SyncModuleCoverage(moduleCoverage);
    }

    internal static void SyncTCMPropertyBag(
      TestManagementRequestContext context,
      IEnumerable<TCMPropertyBag2> tCMPropertyBag2s)
    {
      TCMServiceDataMigration.AuthenticateS2SCall(context.RequestContext);
      using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
        managementDatabase.SyncTCMPropertyBag(tCMPropertyBag2s);
    }

    internal static void SyncPointsResults(
      TestManagementRequestContext context,
      IEnumerable<PointsResults2> pointResults)
    {
      TCMServiceDataMigration.AuthenticateS2SCall(context.RequestContext);
      using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
        managementDatabase.SyncPointResults(pointResults);
    }

    internal static IEnumerable<PointsResults2> FetchPointsResults(
      TestManagementRequestContext context,
      IEnumerable<PointsReference2> pointReferences)
    {
      TCMServiceDataMigration.AuthenticateS2SCall(context.RequestContext);
      using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
        return (IEnumerable<PointsResults2>) managementDatabase.FetchPointResults(pointReferences);
    }

    private static void AuthenticateS2SCall(IVssRequestContext context)
    {
      if (!context.GetService<IVssRegistryService>().GetValue<bool>(context, (RegistryQuery) "/Service/TestManagement/TCMServiceDataMigration/AllowNonS2SCreds", false) && !ServicePrincipals.IsServicePrincipal(context, context.UserContext))
        throw new AccessDeniedException(FrameworkResources.UnauthorizedUserError((object) context.GetUserId()));
    }

    private static void UpdateTCMThresholds(TestManagementRequestContext targetRequestContext)
    {
      try
      {
        List<KeyValuePair<string, int>> result = targetRequestContext.RequestContext.GetClient<TCMServiceMigrationHttpClient>().GetMigrationThresholdAsync().Result;
        KeyValuePair<string, int> keyValuePair1 = result.Find((Predicate<KeyValuePair<string, int>>) (th => string.Equals(th.Key, "TestRunThreshold")));
        KeyValuePair<string, int> keyValuePair2 = result.Find((Predicate<KeyValuePair<string, int>>) (th => string.Equals(th.Key, "TestAttachmentThreshold")));
        if (keyValuePair1.Equals((object) new KeyValuePair<string, int>()) || keyValuePair2.Equals((object) new KeyValuePair<string, int>()))
          return;
        using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(targetRequestContext))
          managementDatabase.UpdateTCMServiceMigrationThreshold(keyValuePair1.Value, keyValuePair2.Value);
        IVssRegistryService service = targetRequestContext.RequestContext.GetService<IVssRegistryService>();
        service.SetValue<int>(targetRequestContext.RequestContext, "/Service/TestManagement/Settings/TcmServiceTestRunIdThreshold", keyValuePair1.Value);
        service.SetValue<int>(targetRequestContext.RequestContext, "/Service/TestManagement/Settings/TcmServiceTestAttachmentIdThreshold", keyValuePair2.Value);
      }
      catch (Exception ex)
      {
      }
    }
  }
}
