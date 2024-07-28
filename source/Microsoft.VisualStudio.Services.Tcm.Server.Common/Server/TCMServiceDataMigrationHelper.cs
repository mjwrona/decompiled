// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TCMServiceDataMigrationHelper
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.Server.TCMServiceMigration;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.VisualStudio.Services.Tcm.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class TCMServiceDataMigrationHelper
  {
    private static readonly int BatchTracePointSize = 50;

    public static void MigrateDataToTCMService(
      TestManagementRequestContext targetRequestContext,
      IServicingContext servicingContext)
    {
      TCMServiceDataMigrationHelper.LogInfo(servicingContext, targetRequestContext, "TCMServiceDataMigrationHelper: enter MigrationDataToTCMService", nameof (TCMServiceDataMigrationHelper));
      IVssRegistryService service = targetRequestContext.RequestContext.GetService<IVssRegistryService>();
      if (!TCMServiceDataMigrationHelper.IsMigrationApplicable(targetRequestContext))
      {
        TCMServiceDataMigrationHelper.LogInfo(servicingContext, targetRequestContext, "TCMServiceDataMigrationHelper: No Test related data in Account", nameof (TCMServiceDataMigrationHelper));
      }
      else
      {
        string waterMark = service.GetValue(targetRequestContext.RequestContext, (RegistryQuery) "/Service/TestManagement/TCMServiceDataMigration/WaterMark", (string) null);
        TCMServiceDataMigrationTables step;
        Guid waterMarkProjectId;
        int waterMarkId;
        int waterMarkId2;
        int waterMarkId3;
        if (!TCMServiceDataMigrationHelper.TryParseWaterMark(targetRequestContext, servicingContext, waterMark, out step, out waterMarkProjectId, out waterMarkId, out waterMarkId2, out waterMarkId3))
          return;
        bool flag1 = service.GetValue<bool>(targetRequestContext.RequestContext, (RegistryQuery) "/Service/TestManagement/TCMServiceDataMigration/StopAfterCurrentStep", false);
        TCMServiceDataMigrationHttpClient client = targetRequestContext.RequestContext.GetClient<TCMServiceDataMigrationHttpClient>();
        try
        {
          if (!flag1)
          {
            TCMServiceDataMigrationHelper.SetMigrationStatusRegistrySetting(targetRequestContext, TCMServiceDataMigrationStatus.InProgress);
            client.SyncMigrationStatusAsync(TCMServiceDataMigrationStatus.InProgress).Wait();
          }
          bool flag2 = false;
          do
          {
            switch (step)
            {
              case TCMServiceDataMigrationTables.ReleaseRef:
                TCMServiceDataMigrationHelper.SyncReleaseRefs(targetRequestContext, servicingContext, waterMarkProjectId, waterMarkId, TCMServiceDataMigrationHelper.GetBatchSize(targetRequestContext, step));
                step = TCMServiceDataMigrationTables.BuildConfiguration;
                break;
              case TCMServiceDataMigrationTables.BuildConfiguration:
                TCMServiceDataMigrationHelper.SyncBuildConfiguration(targetRequestContext, servicingContext, waterMarkProjectId, waterMarkId, TCMServiceDataMigrationHelper.GetBatchSize(targetRequestContext, step));
                step = TCMServiceDataMigrationTables.TestRunContext;
                break;
              case TCMServiceDataMigrationTables.TestRunContext:
                TCMServiceDataMigrationHelper.SyncTestRunContext(targetRequestContext, servicingContext, waterMarkProjectId, waterMarkId, TCMServiceDataMigrationHelper.GetBatchSize(targetRequestContext, step));
                step = TCMServiceDataMigrationTables.TestMessageLog;
                break;
              case TCMServiceDataMigrationTables.TestMessageLog:
                TCMServiceDataMigrationHelper.SyncTestMessageLog(targetRequestContext, servicingContext, waterMarkId, TCMServiceDataMigrationHelper.GetBatchSize(targetRequestContext, step));
                step = TCMServiceDataMigrationTables.TestCaseReference;
                break;
              case TCMServiceDataMigrationTables.TestCaseReference:
                TCMServiceDataMigrationHelper.SyncTestCaseReference(targetRequestContext, servicingContext, waterMarkProjectId, waterMarkId, TCMServiceDataMigrationHelper.GetBatchSize(targetRequestContext, step));
                step = TCMServiceDataMigrationTables.TestResult;
                break;
              case TCMServiceDataMigrationTables.TestResult:
                TCMServiceDataMigrationHelper.SyncTestResults(targetRequestContext, servicingContext, waterMarkProjectId, waterMarkId, waterMarkId2, TCMServiceDataMigrationHelper.GetBatchSize(targetRequestContext, step));
                step = TCMServiceDataMigrationTables.TestMessageLogEntry;
                break;
              case TCMServiceDataMigrationTables.TestMessageLogEntry:
                TCMServiceDataMigrationHelper.SyncTestMessageLogEntry(targetRequestContext, servicingContext, waterMarkId, waterMarkId2, TCMServiceDataMigrationHelper.GetBatchSize(targetRequestContext, step));
                step = TCMServiceDataMigrationTables.TestRun2;
                break;
              case TCMServiceDataMigrationTables.TestRun:
                TCMServiceDataMigrationHelper.SyncTestRuns(targetRequestContext, servicingContext, waterMarkProjectId, waterMarkId, TCMServiceDataMigrationHelper.GetBatchSize(targetRequestContext, step));
                step = TCMServiceDataMigrationTables.TestRunSummary;
                break;
              case TCMServiceDataMigrationTables.TestRunSummary:
                TCMServiceDataMigrationHelper.SyncTestRunSummary(targetRequestContext, servicingContext, waterMarkProjectId, waterMarkId, TCMServiceDataMigrationHelper.GetBatchSize(targetRequestContext, step));
                step = TCMServiceDataMigrationTables.TestResultsEx;
                break;
              case TCMServiceDataMigrationTables.TestResultsEx:
                TCMServiceDataMigrationHelper.SyncTestResultsEx(targetRequestContext, servicingContext, waterMarkProjectId, waterMarkId, waterMarkId2, waterMarkId3, TCMServiceDataMigrationHelper.GetBatchSize(targetRequestContext, step));
                step = TCMServiceDataMigrationTables.TestCaseMetadata;
                break;
              case TCMServiceDataMigrationTables.TestCaseMetadata:
                TCMServiceDataMigrationHelper.SyncTestCaseMetadata(targetRequestContext, servicingContext, waterMarkProjectId, waterMarkId, TCMServiceDataMigrationHelper.GetBatchSize(targetRequestContext, step));
                step = TCMServiceDataMigrationTables.RequirementsToTestsMapping;
                break;
              case TCMServiceDataMigrationTables.RequirementsToTestsMapping:
                TCMServiceDataMigrationHelper.SyncRequirementsToTestsMapping(targetRequestContext, servicingContext, waterMarkProjectId, waterMarkId, TCMServiceDataMigrationHelper.GetBatchSize(targetRequestContext, step));
                step = TCMServiceDataMigrationTables.TestResultReset;
                break;
              case TCMServiceDataMigrationTables.TestResultReset:
                TCMServiceDataMigrationHelper.SyncTestResultReset(targetRequestContext, servicingContext, waterMarkProjectId, waterMarkId, waterMarkId2, waterMarkId3, TCMServiceDataMigrationHelper.GetBatchSize(targetRequestContext, step));
                step = TCMServiceDataMigrationTables.TestActionResult;
                break;
              case TCMServiceDataMigrationTables.TestActionResult:
                TCMServiceDataMigrationHelper.SyncTestActionResult(targetRequestContext, servicingContext, waterMarkId, waterMarkId2, waterMarkId3, TCMServiceDataMigrationHelper.GetBatchSize(targetRequestContext, step));
                step = TCMServiceDataMigrationTables.TestRunEx;
                break;
              case TCMServiceDataMigrationTables.TestRunEx:
                TCMServiceDataMigrationHelper.SyncTestRunEx(targetRequestContext, servicingContext, waterMarkProjectId, waterMarkId, waterMarkId2, TCMServiceDataMigrationHelper.GetBatchSize(targetRequestContext, step));
                step = TCMServiceDataMigrationTables.TestRunExtended;
                break;
              case TCMServiceDataMigrationTables.TestRunExtended:
                TCMServiceDataMigrationHelper.SyncTestRunExtended(targetRequestContext, servicingContext, waterMarkProjectId, waterMarkId, TCMServiceDataMigrationHelper.GetBatchSize(targetRequestContext, step));
                step = TCMServiceDataMigrationTables.TestParameter;
                break;
              case TCMServiceDataMigrationTables.TestParameter:
                TCMServiceDataMigrationHelper.SyncTestParameter(targetRequestContext, servicingContext, waterMarkId, waterMarkId2, waterMarkId3, TCMServiceDataMigrationHelper.GetBatchSize(targetRequestContext, step));
                step = TCMServiceDataMigrationTables.ModuleCoverage;
                break;
              case TCMServiceDataMigrationTables.CodeCoverage:
                TCMServiceDataMigrationHelper.SyncCodeCoverage(targetRequestContext, servicingContext, waterMarkId, TCMServiceDataMigrationHelper.GetBatchSize(targetRequestContext, step));
                step = TCMServiceDataMigrationTables.ReleaseRef;
                break;
              case TCMServiceDataMigrationTables.ModuleCoverage:
                TCMServiceDataMigrationHelper.SyncModuleCoverage(targetRequestContext, servicingContext, waterMarkId, waterMarkId2, TCMServiceDataMigrationHelper.GetBatchSize(targetRequestContext, step));
                step = TCMServiceDataMigrationTables.CodeCoverageSummary;
                break;
              case TCMServiceDataMigrationTables.FunctionCoverage:
                step = TCMServiceDataMigrationTables.CodeCoverageSummary;
                break;
              case TCMServiceDataMigrationTables.CodeCoverageSummary:
                TCMServiceDataMigrationHelper.SyncCodeCoverageSummary(targetRequestContext, servicingContext, waterMarkProjectId, waterMarkId, TCMServiceDataMigrationHelper.GetBatchSize(targetRequestContext, step));
                step = TCMServiceDataMigrationTables.TCMPropertyBag;
                break;
              case TCMServiceDataMigrationTables.TCMPropertyBag:
                TCMServiceDataMigrationHelper.SyncTCMPropertyBag(targetRequestContext, servicingContext, waterMarkId, waterMarkId2, TCMServiceDataMigrationHelper.GetBatchSize(targetRequestContext, step));
                flag2 = true;
                TCMServiceDataMigrationHelper.SetMigrationStatusRegistrySetting(targetRequestContext, TCMServiceDataMigrationStatus.Completed);
                client.SyncMigrationStatusAsync(TCMServiceDataMigrationStatus.Completed).Wait();
                break;
              case TCMServiceDataMigrationTables.TestRun2:
                TCMServiceDataMigrationHelper.SyncTestRuns2(targetRequestContext, servicingContext, waterMarkProjectId, waterMarkId, TCMServiceDataMigrationHelper.GetBatchSize(targetRequestContext, step));
                step = TCMServiceDataMigrationTables.TestRun;
                break;
              default:
                throw new Exception("Invalid step");
            }
            if (flag1)
            {
              TCMServiceDataMigrationHelper.LogInfo(servicingContext, targetRequestContext, "TCMServiceDataMigrationHelper: exit MigrationDataToTCMService after completing current step", nameof (TCMServiceDataMigrationHelper));
              break;
            }
            waterMarkProjectId = Guid.Empty;
            waterMarkId = 0;
            waterMarkId2 = 0;
            waterMarkId3 = 0;
          }
          while (!flag2);
          TCMServiceDataMigrationHelper.LogInfo(servicingContext, targetRequestContext, "TCMServiceDataMigrationHelper: exit MigrationDataToTCMService", nameof (TCMServiceDataMigrationHelper));
        }
        catch (Exception ex)
        {
          if (!flag1)
          {
            TCMServiceDataMigrationHelper.SetMigrationStatusRegistrySetting(targetRequestContext, TCMServiceDataMigrationStatus.Failed);
            client.SyncMigrationStatusAsync(TCMServiceDataMigrationStatus.Failed).Wait();
          }
          throw;
        }
      }
    }

    public static void MigratePointsResultsData(
      TestManagementRequestContext testManagementRequestContext,
      IServicingContext servicingContext)
    {
      IVssRegistryService service = testManagementRequestContext.RequestContext.GetService<IVssRegistryService>();
      try
      {
        TCMServiceDataMigrationHelper.LogInfo(servicingContext, testManagementRequestContext, "TCMServiceDataMigrationHelper: enter MigratePointsResultsToTCMService", "PointsResultsMigration");
        int waterMarkId;
        int waterMarkId2;
        if (!TCMServiceDataMigrationHelper.TryParsePointsResultsWaterMark(testManagementRequestContext, servicingContext, "/Service/TestManagement/TCMServiceDataMigration/PointResultsWaterMark", out waterMarkId, out waterMarkId2))
          return;
        List<PointsResults2> pointResults = new List<PointsResults2>();
        IVssRegistryService registryService1 = service;
        IVssRequestContext requestContext1 = testManagementRequestContext.RequestContext;
        RegistryQuery registryQuery = (RegistryQuery) "/Service/TestManagement/TCMServiceDataMigration/PointResultsBatchSize";
        ref RegistryQuery local1 = ref registryQuery;
        int batchSize = registryService1.GetValue<int>(requestContext1, in local1, 10000);
        IVssRegistryService registryService2 = service;
        IVssRequestContext requestContext2 = testManagementRequestContext.RequestContext;
        registryQuery = (RegistryQuery) "/Service/TestManagement/TCMServiceDataMigration/PointResultsMigrationStatus";
        ref RegistryQuery local2 = ref registryQuery;
        switch ((TCMServiceDataMigrationStatus) registryService2.GetValue<int>(requestContext2, in local2, 0))
        {
          case TCMServiceDataMigrationStatus.InProgress:
          case TCMServiceDataMigrationStatus.Completed:
            TCMServiceDataMigrationHelper.LogInfo(servicingContext, testManagementRequestContext, "TCMServiceDataMigrationHelper: Points Results Already Migrated or InProgress", "PointsResultsMigration");
            break;
          default:
            service.SetValue<int>(testManagementRequestContext.RequestContext, "/Service/TestManagement/TCMServiceDataMigration/PointResultsMigrationStatus", 1);
            bool flag;
            do
            {
              using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(testManagementRequestContext))
                pointResults = managementDatabase.FetchPointResults(waterMarkId, waterMarkId2, batchSize);
              if (pointResults.Count > 0)
              {
                using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(testManagementRequestContext))
                  managementDatabase.SyncPointResults((IEnumerable<PointsResults2>) pointResults);
              }
              int count = pointResults.Count;
              flag = count < batchSize;
              if (!flag)
              {
                waterMarkId = pointResults[count - 1].PlanId;
                waterMarkId2 = pointResults[count - 1].PointId;
                string str = string.Format("{0},{1}", (object) waterMarkId, (object) waterMarkId2);
                service.SetValue<string>(testManagementRequestContext.RequestContext, "/Service/TestManagement/TCMServiceDataMigration/PointResultsWaterMark", str);
                string message = string.Format("TCMServiceDataMigrationHelper: syncing Point Results. WaterMark: {0}", (object) str);
                TCMServiceDataMigrationHelper.LogInfo(servicingContext, testManagementRequestContext, message, "PointsResultsMigration");
              }
              else
              {
                string str = string.Format("{0},{1}", (object) -1, (object) -1);
                service.SetValue<string>(testManagementRequestContext.RequestContext, "/Service/TestManagement/TCMServiceDataMigration/PointResultsWaterMark", str);
                string message = string.Format("TCMServiceDataMigrationHelper: syncing PointResults done. WaterMark: {0}", (object) str);
                TCMServiceDataMigrationHelper.LogInfo(servicingContext, testManagementRequestContext, message, "PointsResultsMigration");
                service.SetValue<int>(testManagementRequestContext.RequestContext, "/Service/TestManagement/TCMServiceDataMigration/PointResultsMigrationStatus", 2);
              }
            }
            while (!flag);
            TCMServiceDataMigrationHelper.LogInfo(servicingContext, testManagementRequestContext, "TCMServiceDataMigrationHelper: exit MigratePointsResultsToTCMService", "PointsResultsMigration");
            break;
        }
      }
      catch (Exception ex)
      {
        service.SetValue<int>(testManagementRequestContext.RequestContext, "/Service/TestManagement/TCMServiceDataMigration/PointResultsMigrationStatus", 3);
        throw;
      }
    }

    public static void MigratePointOutcomeData(
      TestManagementRequestContext testManagementRequestContext,
      IServicingContext servicingContext)
    {
      IVssRegistryService service = testManagementRequestContext.RequestContext.GetService<IVssRegistryService>();
      try
      {
        TCMServiceDataMigrationHelper.LogInfo(servicingContext, testManagementRequestContext, "TCMServiceDataMigrationHelper: enter MigratePointOutcomeData", "PointsOutcomeMigration");
        if (!TCMServiceDataMigrationHelper.IsMigrationApplicable_PointsPresent(testManagementRequestContext))
        {
          string message = "TCMServiceDataMigrationHelper: No Points to migrate";
          TCMServiceDataMigrationHelper.LogInfo(servicingContext, testManagementRequestContext, message, "PointsOutcomeMigration");
          service.SetValue<int>(testManagementRequestContext.RequestContext, "/Service/TestManagement/TCMServiceDataMigration/PointOutcomeMigrationStatus", 2);
        }
        else
        {
          int waterMarkId;
          int waterMarkId2;
          if (!TCMServiceDataMigrationHelper.TryParsePointsResultsWaterMark(testManagementRequestContext, servicingContext, "/Service/TestManagement/TCMServiceDataMigration/PointOutcomeWaterMark", out waterMarkId, out waterMarkId2))
            return;
          TCMServiceDataMigrationHttpClient migrationClient = testManagementRequestContext.RequestContext.GetClient<TCMServiceDataMigrationHttpClient>();
          List<PointsResults2> source = new List<PointsResults2>();
          int batchSize = service.GetValue<int>(testManagementRequestContext.RequestContext, (RegistryQuery) "/Service/TestManagement/TCMServiceDataMigration/PointOutcomeBatchSize", 10000);
          switch ((TCMServiceDataMigrationStatus) service.GetValue<int>(testManagementRequestContext.RequestContext, (RegistryQuery) "/Service/TestManagement/TCMServiceDataMigration/PointOutcomeMigrationStatus", 0))
          {
            case TCMServiceDataMigrationStatus.InProgress:
            case TCMServiceDataMigrationStatus.Completed:
              TCMServiceDataMigrationHelper.LogInfo(servicingContext, testManagementRequestContext, "TCMServiceDataMigrationHelper: Points Results Already Migrated or InProgress", "PointsOutcomeMigration");
              break;
            default:
              if (!service.GetValue<DateTime?>(testManagementRequestContext.RequestContext, (RegistryQuery) "/Service/TestManagement/Settings/PointOutcomeSyncupWaterMark", new DateTime?()).HasValue)
                service.SetValue<DateTime>(testManagementRequestContext.RequestContext, "/Service/TestManagement/Settings/PointOutcomeSyncupWaterMark", DateTime.UtcNow);
              service.SetValue<int>(testManagementRequestContext.RequestContext, "/Service/TestManagement/TCMServiceDataMigration/PointOutcomeMigrationStatus", 1);
              bool flag;
              do
              {
                using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(testManagementRequestContext))
                  source = managementDatabase.FetchPointResults(waterMarkId, waterMarkId2, batchSize);
                IEnumerable<PointsReference2> pointReferences = source.Select<PointsResults2, PointsReference2>((Func<PointsResults2, PointsReference2>) (point => new PointsReference2()
                {
                  PointId = point.PointId,
                  PlanId = point.PlanId
                }));
                List<PointsResults2> pointResults = new RetryManager(3, TimeSpan.FromSeconds(5.0), (Action<Exception>) (ex => TCMServiceDataMigrationHelper.LogInfo(servicingContext, testManagementRequestContext, "FetchPointResultsAsync - Exception within RetryManager: " + ex.ToString(), "PointsOutcomeMigration"))).InvokeFunc<List<PointsResults2>>((Func<List<PointsResults2>>) (() => migrationClient.FetchPointResultsAsync(pointReferences, batchSize).Result));
                using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(testManagementRequestContext))
                  managementDatabase.SyncPointOutcome(pointResults);
                int count = source.Count;
                flag = count < batchSize;
                if (!flag)
                {
                  waterMarkId = source[count - 1].PlanId;
                  waterMarkId2 = source[count - 1].PointId;
                  string str = string.Format("{0},{1}", (object) waterMarkId, (object) waterMarkId2);
                  service.SetValue<string>(testManagementRequestContext.RequestContext, "/Service/TestManagement/TCMServiceDataMigration/PointOutcomeWaterMark", str);
                  string message = string.Format("TCMServiceDataMigrationHelper: syncing Point Results. WaterMark: {0}", (object) waterMarkId2);
                  TCMServiceDataMigrationHelper.LogInfo(servicingContext, testManagementRequestContext, message, "PointsOutcomeMigration");
                }
                else
                {
                  string str = string.Format("{0},{1}", (object) -1, (object) -1);
                  service.SetValue<string>(testManagementRequestContext.RequestContext, "/Service/TestManagement/TCMServiceDataMigration/PointOutcomeWaterMark", str);
                  string message = string.Format("TCMServiceDataMigrationHelper: syncing PointResults done. WaterMark: {0}", (object) waterMarkId2);
                  TCMServiceDataMigrationHelper.LogInfo(servicingContext, testManagementRequestContext, message, "PointsOutcomeMigration");
                  service.SetValue<int>(testManagementRequestContext.RequestContext, "/Service/TestManagement/TCMServiceDataMigration/PointOutcomeMigrationStatus", 2);
                }
              }
              while (!flag);
              TCMServiceDataMigrationHelper.LogInfo(servicingContext, testManagementRequestContext, "TCMServiceDataMigrationHelper: exit MigratePointOutcomeData", "PointsOutcomeMigration");
              break;
          }
        }
      }
      catch (Exception ex)
      {
        service.SetValue<int>(testManagementRequestContext.RequestContext, "/Service/TestManagement/TCMServiceDataMigration/PointOutcomeMigrationStatus", 3);
        throw;
      }
    }

    public static void BackfillOutcomeForPointHistory(
      TestManagementRequestContext testManagementRequestContext,
      IServicingContext servicingContext)
    {
      IVssRegistryService service = testManagementRequestContext.RequestContext.GetService<IVssRegistryService>();
      try
      {
        TCMServiceDataMigrationHelper.LogInfo(servicingContext, testManagementRequestContext, "TCMServiceDataMigrationHelper: enter BackfillOutcomeForPointHistory", "PointHistoryOutcomeBackfill");
        if (!TCMServiceDataMigrationHelper.IsMigrationApplicable_PointsPresent(testManagementRequestContext))
        {
          string message = "TCMServiceDataMigrationHelper: No Points to migrate";
          TCMServiceDataMigrationHelper.LogInfo(servicingContext, testManagementRequestContext, message, "PointHistoryOutcomeBackfill");
          service.SetValue<int>(testManagementRequestContext.RequestContext, "/Service/TestManagement/TCMServiceDataMigration/BackfillOutcomeForPointHistoryStatus", 2);
        }
        else
        {
          IVssRegistryService registryService1 = service;
          IVssRequestContext requestContext1 = testManagementRequestContext.RequestContext;
          RegistryQuery registryQuery = (RegistryQuery) "/Service/TestManagement/TCMServiceDataMigration/BackfillOutcomeForPointHistoryStatus";
          ref RegistryQuery local1 = ref registryQuery;
          switch ((TCMServiceDataMigrationStatus) registryService1.GetValue<int>(requestContext1, in local1, 0))
          {
            case TCMServiceDataMigrationStatus.InProgress:
            case TCMServiceDataMigrationStatus.Completed:
              TCMServiceDataMigrationHelper.LogInfo(servicingContext, testManagementRequestContext, "TCMServiceDataMigrationHelper: PointHistory backfill Completed or InProgress", "PointHistoryOutcomeBackfill");
              break;
            default:
              int planId1;
              int pointId1;
              int changeNumber1;
              if (!TCMServiceDataMigrationHelper.TryParsePointHistoryWaterMark(testManagementRequestContext, servicingContext, "/Service/TestManagement/TCMServiceDataMigration/BackfillOutcomeForPointHistoryWatermark", out planId1, out pointId1, out changeNumber1))
                break;
              IVssRegistryService registryService2 = service;
              IVssRequestContext requestContext2 = testManagementRequestContext.RequestContext;
              registryQuery = (RegistryQuery) "/Service/TestManagement/TCMServiceDataMigration/BackfillOutcomeForPointHistoryBatchSize";
              ref RegistryQuery local2 = ref registryQuery;
              int batchSize = registryService2.GetValue<int>(requestContext2, in local2, 10000);
              service.SetValue<int>(testManagementRequestContext.RequestContext, "/Service/TestManagement/TCMServiceDataMigration/BackfillOutcomeForPointHistoryStatus", 1);
              bool flag;
              do
              {
                int columnsUpdated;
                int planId2;
                int pointId2;
                int changeNumber2;
                using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(testManagementRequestContext))
                  managementDatabase.BackfillPointHistoryOutcome(planId1, pointId1, changeNumber1, batchSize, out columnsUpdated, out planId2, out pointId2, out changeNumber2);
                planId1 = planId2;
                pointId1 = pointId2;
                changeNumber1 = changeNumber2;
                flag = columnsUpdated < batchSize;
                if (!flag)
                {
                  service.SetValue<string>(testManagementRequestContext.RequestContext, "/Service/TestManagement/TCMServiceDataMigration/PointOutcomeWaterMark", string.Format("{0},{1},{2}", (object) planId1, (object) pointId1, (object) changeNumber1));
                  string message = string.Format("TCMServiceDataMigrationHelper: Backfilling pointhistory. WaterMark: {0},{1},{2}", (object) planId1, (object) pointId1, (object) changeNumber1);
                  TCMServiceDataMigrationHelper.LogInfo(servicingContext, testManagementRequestContext, message, "PointHistoryOutcomeBackfill");
                }
                else
                {
                  service.SetValue<string>(testManagementRequestContext.RequestContext, "/Service/TestManagement/TCMServiceDataMigration/PointOutcomeWaterMark", string.Format("0,0,0"));
                  string message = string.Format("TCMServiceDataMigrationHelper: Backfilling pointhistory. WaterMark: {0},{1},{2}", (object) planId1, (object) pointId1, (object) changeNumber1);
                  TCMServiceDataMigrationHelper.LogInfo(servicingContext, testManagementRequestContext, message, "PointHistoryOutcomeBackfill");
                  service.SetValue<int>(testManagementRequestContext.RequestContext, "/Service/TestManagement/TCMServiceDataMigration/BackfillOutcomeForPointHistoryStatus", 2);
                }
              }
              while (!flag);
              TCMServiceDataMigrationHelper.LogInfo(servicingContext, testManagementRequestContext, "TCMServiceDataMigrationHelper: exit BackfillOutcomeForPointHistory", "PointHistoryOutcomeBackfill");
              break;
          }
        }
      }
      catch (Exception ex)
      {
        TCMServiceDataMigrationHelper.LogInfo(servicingContext, testManagementRequestContext, ex.ToString(), "PointHistoryOutcomeBackfill");
        service.SetValue<int>(testManagementRequestContext.RequestContext, "/Service/TestManagement/TCMServiceDataMigration/BackfillOutcomeForPointHistoryStatus", 3);
        throw;
      }
    }

    internal static void SyncCodeCoverage(
      TestManagementRequestContext testManagementRequestContext,
      IServicingContext servicingContext,
      int waterMarkId,
      int batchSize)
    {
      TCMServiceDataMigrationHelper.LogInfo(servicingContext, testManagementRequestContext, "TCMServiceDataMigrationHelper: enter SyncCodeCoverage", nameof (TCMServiceDataMigrationHelper));
      string waterMark1 = (string) null;
      string batchedWaterMark = string.Empty;
      int tracePointCount = 0;
      TCMServiceDataMigrationHttpClient client = testManagementRequestContext.RequestContext.GetClient<TCMServiceDataMigrationHttpClient>();
      List<Coverage2> coverage2 = new List<Coverage2>();
      bool flag;
      do
      {
        TCMServiceDataMigrationHelper.ShouldCancelJob(testManagementRequestContext);
        using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(testManagementRequestContext))
          coverage2 = managementDatabase.FetchCoverage(waterMarkId, batchSize);
        client.SyncCodeCoverageAsync((IEnumerable<Coverage2>) coverage2).Wait();
        int count = coverage2.Count;
        string waterMark2 = count.ToString() + ";" + (object) waterMarkId + ";" + (object) batchSize;
        TCMServiceDataMigrationHelper.TraceInfo(testManagementRequestContext, servicingContext, "CodeCoverage", waterMark2);
        flag = count < batchSize;
        if (!flag)
        {
          waterMarkId = coverage2[count - 1].CoverageId;
          TCMServiceDataMigrationHelper.SetWaterMarkInRegistry(testManagementRequestContext, 17, Guid.Empty, out waterMark1, waterMarkId);
          TCMServiceDataMigrationHelper.BatchTraceInfo(testManagementRequestContext, servicingContext, "CodeCoverage", waterMark1, ref batchedWaterMark, ref tracePointCount);
        }
        else
        {
          TCMServiceDataMigrationHelper.SetWaterMarkInRegistry(testManagementRequestContext, 0, Guid.Empty, out waterMark1);
          batchedWaterMark = batchedWaterMark + ";" + waterMark1 + (object) DateTime.UtcNow;
          string message = "TCMServiceDataMigrationHelper: syncing codecoverage done. WaterMark: " + batchedWaterMark;
          TCMServiceDataMigrationHelper.LogInfo(servicingContext, testManagementRequestContext, message, nameof (TCMServiceDataMigrationHelper));
        }
      }
      while (!flag);
      TCMServiceDataMigrationHelper.LogInfo(servicingContext, testManagementRequestContext, "TCMServiceDataMigrationHelper: exit SyncCodeCoverage", nameof (TCMServiceDataMigrationHelper));
    }

    internal static void SyncReleaseRefs(
      TestManagementRequestContext testManagementRequestContext,
      IServicingContext servicingContext,
      Guid waterMarkProjectId,
      int waterMarkId,
      int batchSize)
    {
      TCMServiceDataMigrationHelper.LogInfo(servicingContext, testManagementRequestContext, "TCMServiceDataMigrationHelper: enter SyncReleaseRefs", nameof (TCMServiceDataMigrationHelper));
      string waterMark1 = (string) null;
      string batchedWaterMark = string.Empty;
      int tracePointCount = 0;
      TCMServiceDataMigrationHttpClient client = testManagementRequestContext.RequestContext.GetClient<TCMServiceDataMigrationHttpClient>();
      List<ReleaseReference2> references = new List<ReleaseReference2>();
      bool flag;
      do
      {
        TCMServiceDataMigrationHelper.ShouldCancelJob(testManagementRequestContext);
        int waterMarkDataspaceId = TCMServiceDataMigrationHelper.GetWaterMarkDataspaceId(testManagementRequestContext, waterMarkProjectId);
        using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(testManagementRequestContext))
          references = managementDatabase.FetchReleaseRefs(waterMarkDataspaceId, waterMarkId, batchSize);
        client.SyncReleaseRefAsync((IEnumerable<ReleaseReference2>) references).Wait();
        int count = references.Count;
        string waterMark2 = count.ToString() + ";" + (object) waterMarkDataspaceId + ";" + (object) waterMarkId + ";" + (object) batchSize;
        TCMServiceDataMigrationHelper.TraceInfo(testManagementRequestContext, servicingContext, "ReleaseRef", waterMark2);
        flag = count < batchSize;
        if (!flag)
        {
          waterMarkProjectId = references[count - 1].ProjectId;
          waterMarkId = references[count - 1].ReleaseRefId;
          TCMServiceDataMigrationHelper.SetWaterMarkInRegistry(testManagementRequestContext, 0, waterMarkProjectId, out waterMark1, waterMarkId);
          TCMServiceDataMigrationHelper.BatchTraceInfo(testManagementRequestContext, servicingContext, "ReleaseRef", waterMark1, ref batchedWaterMark, ref tracePointCount);
        }
        else
        {
          TCMServiceDataMigrationHelper.SetWaterMarkInRegistry(testManagementRequestContext, 1, Guid.Empty, out waterMark1);
          batchedWaterMark = batchedWaterMark + ";" + waterMark1 + (object) DateTime.UtcNow;
          string message = "TCMServiceDataMigrationHelper: syncing releaserefs done. WaterMark: " + batchedWaterMark;
          TCMServiceDataMigrationHelper.LogInfo(servicingContext, testManagementRequestContext, message, nameof (TCMServiceDataMigrationHelper));
        }
      }
      while (!flag);
      TCMServiceDataMigrationHelper.LogInfo(servicingContext, testManagementRequestContext, "TCMServiceDataMigrationHelper: exit SyncReleaseRefs", nameof (TCMServiceDataMigrationHelper));
    }

    internal static void SyncBuildConfiguration(
      TestManagementRequestContext testManagementRequestContext,
      IServicingContext servicingContext,
      Guid waterMarkProjectId,
      int waterMarkId,
      int batchSize)
    {
      TCMServiceDataMigrationHelper.LogInfo(servicingContext, testManagementRequestContext, "TCMServiceDataMigrationHelper: enter SyncBuildConfiguration", nameof (TCMServiceDataMigrationHelper));
      string waterMark1 = (string) null;
      string batchedWaterMark = string.Empty;
      int tracePointCount = 0;
      TCMServiceDataMigrationHttpClient client = testManagementRequestContext.RequestContext.GetClient<TCMServiceDataMigrationHttpClient>();
      List<BuildReference2> references = new List<BuildReference2>();
      bool flag;
      do
      {
        TCMServiceDataMigrationHelper.ShouldCancelJob(testManagementRequestContext);
        int waterMarkDataspaceId = TCMServiceDataMigrationHelper.GetWaterMarkDataspaceId(testManagementRequestContext, waterMarkProjectId);
        using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(testManagementRequestContext))
          references = managementDatabase.FetchBuildRefs(waterMarkDataspaceId, waterMarkId, batchSize);
        client.SyncBuildRefAsync((IEnumerable<BuildReference2>) references).Wait();
        int count = references.Count;
        string waterMark2 = count.ToString() + ";" + (object) waterMarkDataspaceId + ";" + (object) waterMarkId + ";" + (object) batchSize;
        TCMServiceDataMigrationHelper.TraceInfo(testManagementRequestContext, servicingContext, "BuildConfiguration", waterMark2);
        flag = count < batchSize;
        if (!flag)
        {
          waterMarkProjectId = references[count - 1].ProjectId;
          waterMarkId = references[count - 1].BuildConfigurationId;
          TCMServiceDataMigrationHelper.SetWaterMarkInRegistry(testManagementRequestContext, 1, waterMarkProjectId, out waterMark1, waterMarkId);
          TCMServiceDataMigrationHelper.BatchTraceInfo(testManagementRequestContext, servicingContext, "BuildConfiguration", waterMark1, ref batchedWaterMark, ref tracePointCount);
        }
        else
        {
          TCMServiceDataMigrationHelper.SetWaterMarkInRegistry(testManagementRequestContext, 2, Guid.Empty, out waterMark1);
          batchedWaterMark = batchedWaterMark + ";" + waterMark1 + (object) DateTime.UtcNow;
          string message = "TCMServiceDataMigrationHelper: syncing buildrefs done. WaterMark: " + batchedWaterMark;
          TCMServiceDataMigrationHelper.LogInfo(servicingContext, testManagementRequestContext, message, nameof (TCMServiceDataMigrationHelper));
        }
      }
      while (!flag);
      TCMServiceDataMigrationHelper.LogInfo(servicingContext, testManagementRequestContext, "TCMServiceDataMigrationHelper: exit SyncBuildConfiguration", nameof (TCMServiceDataMigrationHelper));
    }

    internal static void SyncTestRunContext(
      TestManagementRequestContext testManagementRequestContext,
      IServicingContext servicingContext,
      Guid waterMarkProjectId,
      int waterMarkId,
      int batchSize)
    {
      TCMServiceDataMigrationHelper.LogInfo(servicingContext, testManagementRequestContext, "TCMServiceDataMigrationHelper: enter SyncTestRunContext", nameof (TCMServiceDataMigrationHelper));
      string waterMark1 = (string) null;
      string batchedWaterMark = string.Empty;
      int tracePointCount = 0;
      TCMServiceDataMigrationHttpClient client = testManagementRequestContext.RequestContext.GetClient<TCMServiceDataMigrationHttpClient>();
      List<TestRunContext2> testRunContexts = new List<TestRunContext2>();
      bool flag;
      do
      {
        TCMServiceDataMigrationHelper.ShouldCancelJob(testManagementRequestContext);
        int waterMarkDataspaceId = TCMServiceDataMigrationHelper.GetWaterMarkDataspaceId(testManagementRequestContext, waterMarkProjectId);
        using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(testManagementRequestContext))
          testRunContexts = managementDatabase.FetchTestRunContexts(waterMarkDataspaceId, waterMarkId, batchSize);
        client.SyncTestRunContextAsync((IEnumerable<TestRunContext2>) testRunContexts).Wait();
        int count = testRunContexts.Count;
        string waterMark2 = count.ToString() + ";" + (object) waterMarkDataspaceId + ";" + (object) waterMarkId + ";" + (object) batchSize;
        TCMServiceDataMigrationHelper.TraceInfo(testManagementRequestContext, servicingContext, "TestRunContext", waterMark2);
        flag = count < batchSize;
        if (!flag)
        {
          waterMarkProjectId = testRunContexts[count - 1].ProjectId;
          waterMarkId = testRunContexts[count - 1].TestRunContextId;
          TCMServiceDataMigrationHelper.SetWaterMarkInRegistry(testManagementRequestContext, 2, waterMarkProjectId, out waterMark1, waterMarkId);
          TCMServiceDataMigrationHelper.BatchTraceInfo(testManagementRequestContext, servicingContext, "TestRunContext", waterMark1, ref batchedWaterMark, ref tracePointCount);
        }
        else
        {
          TCMServiceDataMigrationHelper.SetWaterMarkInRegistry(testManagementRequestContext, 3, Guid.Empty, out waterMark1);
          batchedWaterMark = batchedWaterMark + ";" + waterMark1 + (object) DateTime.UtcNow;
          string message = "TCMServiceDataMigrationHelper: syncing testruncontexts done. WaterMark: " + batchedWaterMark;
          TCMServiceDataMigrationHelper.LogInfo(servicingContext, testManagementRequestContext, message, nameof (TCMServiceDataMigrationHelper));
        }
      }
      while (!flag);
      TCMServiceDataMigrationHelper.LogInfo(servicingContext, testManagementRequestContext, "TCMServiceDataMigrationHelper: exit SyncTestRunContext", nameof (TCMServiceDataMigrationHelper));
    }

    internal static void SyncTestMessageLog(
      TestManagementRequestContext testManagementRequestContext,
      IServicingContext servicingContext,
      int waterMarkId,
      int batchSize)
    {
      TCMServiceDataMigrationHelper.LogInfo(servicingContext, testManagementRequestContext, "TCMServiceDataMigrationHelper: enter SyncTestMessageLog", nameof (TCMServiceDataMigrationHelper));
      string waterMark1 = (string) null;
      string batchedWaterMark = string.Empty;
      int tracePointCount = 0;
      TCMServiceDataMigrationHttpClient client = testManagementRequestContext.RequestContext.GetClient<TCMServiceDataMigrationHttpClient>();
      List<TestMessageLog2> testMessageLogs = new List<TestMessageLog2>();
      bool flag;
      do
      {
        TCMServiceDataMigrationHelper.ShouldCancelJob(testManagementRequestContext);
        using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(testManagementRequestContext))
          testMessageLogs = managementDatabase.FetchTestMessageLogs(waterMarkId, batchSize);
        client.SyncTestMessageLogAsync((IEnumerable<TestMessageLog2>) testMessageLogs).Wait();
        int count = testMessageLogs.Count;
        string waterMark2 = count.ToString() + ";" + (object) waterMarkId + ";" + (object) batchSize;
        TCMServiceDataMigrationHelper.TraceInfo(testManagementRequestContext, servicingContext, "TestMessageLog", waterMark2);
        flag = count < batchSize;
        if (!flag)
        {
          waterMarkId = testMessageLogs[count - 1].TestMessageLogId;
          TCMServiceDataMigrationHelper.SetWaterMarkInRegistry(testManagementRequestContext, 3, Guid.Empty, out waterMark1, waterMarkId);
          TCMServiceDataMigrationHelper.BatchTraceInfo(testManagementRequestContext, servicingContext, "TestMessageLog", waterMark1, ref batchedWaterMark, ref tracePointCount);
        }
        else
        {
          TCMServiceDataMigrationHelper.SetWaterMarkInRegistry(testManagementRequestContext, 4, Guid.Empty, out waterMark1);
          batchedWaterMark = batchedWaterMark + ";" + waterMark1 + (object) DateTime.UtcNow;
          string message = "TCMServiceDataMigrationHelper: syncing testmessagelogs done. WaterMark: " + batchedWaterMark;
          TCMServiceDataMigrationHelper.LogInfo(servicingContext, testManagementRequestContext, message, nameof (TCMServiceDataMigrationHelper));
        }
      }
      while (!flag);
      TCMServiceDataMigrationHelper.LogInfo(servicingContext, testManagementRequestContext, "TCMServiceDataMigrationHelper: exit SyncTestMessageLog", nameof (TCMServiceDataMigrationHelper));
    }

    internal static void SyncTestCaseReference(
      TestManagementRequestContext testManagementRequestContext,
      IServicingContext servicingContext,
      Guid waterMarkProjectId,
      int waterMarkId,
      int batchSize)
    {
      TCMServiceDataMigrationHelper.LogInfo(servicingContext, testManagementRequestContext, "TCMServiceDataMigrationHelper: enter SyncTestCaseReference", nameof (TCMServiceDataMigrationHelper));
      string waterMark1 = (string) null;
      string batchedWaterMark = string.Empty;
      int tracePointCount = 0;
      TCMServiceDataMigrationHttpClient client = testManagementRequestContext.RequestContext.GetClient<TCMServiceDataMigrationHttpClient>();
      List<TestCaseReference2> testCaseReferences = new List<TestCaseReference2>();
      bool flag;
      do
      {
        TCMServiceDataMigrationHelper.ShouldCancelJob(testManagementRequestContext);
        int waterMarkDataspaceId = TCMServiceDataMigrationHelper.GetWaterMarkDataspaceId(testManagementRequestContext, waterMarkProjectId);
        using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(testManagementRequestContext))
          testCaseReferences = managementDatabase.FetchTestCaseReferences(waterMarkDataspaceId, waterMarkId, batchSize);
        client.SyncTestCaseReferenceAsync((IEnumerable<TestCaseReference2>) testCaseReferences).Wait();
        int count = testCaseReferences.Count;
        string waterMark2 = count.ToString() + ";" + (object) waterMarkDataspaceId + ";" + (object) waterMarkId + ";" + (object) batchSize;
        TCMServiceDataMigrationHelper.TraceInfo(testManagementRequestContext, servicingContext, "TestCaseReference", waterMark2);
        flag = count < batchSize;
        if (!flag)
        {
          waterMarkProjectId = testCaseReferences[count - 1].ProjectId;
          waterMarkId = testCaseReferences[count - 1].TestCaseRefId;
          TCMServiceDataMigrationHelper.SetWaterMarkInRegistry(testManagementRequestContext, 4, waterMarkProjectId, out waterMark1, waterMarkId);
          TCMServiceDataMigrationHelper.BatchTraceInfo(testManagementRequestContext, servicingContext, "TestCaseReference", waterMark1, ref batchedWaterMark, ref tracePointCount);
        }
        else
        {
          TCMServiceDataMigrationHelper.SetWaterMarkInRegistry(testManagementRequestContext, 5, Guid.Empty, out waterMark1);
          batchedWaterMark = batchedWaterMark + ";" + waterMark1 + (object) DateTime.UtcNow;
          string message = "TCMServiceDataMigrationHelper: syncing testcasreferences done. WaterMark: " + batchedWaterMark;
          TCMServiceDataMigrationHelper.LogInfo(servicingContext, testManagementRequestContext, message, nameof (TCMServiceDataMigrationHelper));
        }
      }
      while (!flag);
      TCMServiceDataMigrationHelper.LogInfo(servicingContext, testManagementRequestContext, "TCMServiceDataMigrationHelper: exit SyncTestCaseReference", nameof (TCMServiceDataMigrationHelper));
    }

    internal static void SyncTestResults(
      TestManagementRequestContext testManagementRequestContext,
      IServicingContext servicingContext,
      Guid waterMarkProjectId,
      int waterMarkTestRunId,
      int waterMarkTestResultId,
      int batchSize)
    {
      TCMServiceDataMigrationHelper.LogInfo(servicingContext, testManagementRequestContext, "TCMServiceDataMigrationHelper: enter SyncTestResults", nameof (TCMServiceDataMigrationHelper));
      string waterMark1 = (string) null;
      string batchedWaterMark = string.Empty;
      int tracePointCount = 0;
      TCMServiceDataMigrationHttpClient client = testManagementRequestContext.RequestContext.GetClient<TCMServiceDataMigrationHttpClient>();
      List<TestResult2> testResults = new List<TestResult2>();
      bool flag;
      do
      {
        TCMServiceDataMigrationHelper.ShouldCancelJob(testManagementRequestContext);
        int waterMarkDataspaceId = TCMServiceDataMigrationHelper.GetWaterMarkDataspaceId(testManagementRequestContext, waterMarkProjectId);
        using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(testManagementRequestContext))
          testResults = managementDatabase.FetchTestResults(waterMarkDataspaceId, waterMarkTestRunId, waterMarkTestResultId, batchSize);
        client.SyncTestResultAsync((IEnumerable<TestResult2>) testResults).Wait();
        int count = testResults.Count;
        string waterMark2 = count.ToString() + ";" + (object) waterMarkDataspaceId + ";" + (object) waterMarkTestRunId + ";" + (object) waterMarkTestResultId + ";" + (object) batchSize;
        TCMServiceDataMigrationHelper.TraceInfo(testManagementRequestContext, servicingContext, "TestResult", waterMark2);
        flag = count < batchSize;
        if (!flag)
        {
          waterMarkProjectId = testResults[count - 1].ProjectId;
          waterMarkTestRunId = testResults[count - 1].TestRunId;
          waterMarkTestResultId = testResults[count - 1].TestResultId;
          TCMServiceDataMigrationHelper.SetWaterMarkInRegistry(testManagementRequestContext, 5, waterMarkProjectId, out waterMark1, waterMarkTestRunId, waterMarkTestResultId);
          TCMServiceDataMigrationHelper.BatchTraceInfo(testManagementRequestContext, servicingContext, "TestResult", waterMark1, ref batchedWaterMark, ref tracePointCount);
        }
        else
        {
          TCMServiceDataMigrationHelper.SetWaterMarkInRegistry(testManagementRequestContext, 6, Guid.Empty, out waterMark1);
          batchedWaterMark = batchedWaterMark + ";" + waterMark1 + (object) DateTime.UtcNow;
          string message = "TCMServiceDataMigrationHelper: syncing testresults done. WaterMark: " + batchedWaterMark;
          TCMServiceDataMigrationHelper.LogInfo(servicingContext, testManagementRequestContext, message, nameof (TCMServiceDataMigrationHelper));
        }
      }
      while (!flag);
      TCMServiceDataMigrationHelper.LogInfo(servicingContext, testManagementRequestContext, "TCMServiceDataMigrationHelper: exit SyncTestResults", nameof (TCMServiceDataMigrationHelper));
    }

    internal static void SyncTestMessageLogEntry(
      TestManagementRequestContext testManagementRequestContext,
      IServicingContext servicingContext,
      int waterMarkLogId,
      int waterMarkEntryId,
      int batchSize)
    {
      TCMServiceDataMigrationHelper.LogInfo(servicingContext, testManagementRequestContext, "TCMServiceDataMigrationHelper: enter SyncTestMessageLogEntry", nameof (TCMServiceDataMigrationHelper));
      string waterMark1 = (string) null;
      string batchedWaterMark = string.Empty;
      int tracePointCount = 0;
      TCMServiceDataMigrationHttpClient client = testManagementRequestContext.RequestContext.GetClient<TCMServiceDataMigrationHttpClient>();
      List<TestMessageLogEntry2> testMessageLogEntries = new List<TestMessageLogEntry2>();
      bool flag;
      do
      {
        TCMServiceDataMigrationHelper.ShouldCancelJob(testManagementRequestContext);
        using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(testManagementRequestContext))
          testMessageLogEntries = managementDatabase.FetchTestMessageLogEntry(waterMarkLogId, waterMarkEntryId, batchSize);
        client.SyncTestMessageLogEntryAsync((IEnumerable<TestMessageLogEntry2>) testMessageLogEntries).Wait();
        int count = testMessageLogEntries.Count;
        string waterMark2 = count.ToString() + ";" + (object) waterMarkLogId + ";" + (object) waterMarkEntryId + ";" + (object) batchSize;
        TCMServiceDataMigrationHelper.TraceInfo(testManagementRequestContext, servicingContext, "TestMessageLogEntry", waterMark2);
        flag = count < batchSize;
        if (!flag)
        {
          waterMarkLogId = testMessageLogEntries[count - 1].TestMessageLogId;
          waterMarkEntryId = testMessageLogEntries[count - 1].EntryId;
          TCMServiceDataMigrationHelper.SetWaterMarkInRegistry(testManagementRequestContext, 6, Guid.Empty, out waterMark1, waterMarkLogId, waterMarkEntryId);
          TCMServiceDataMigrationHelper.BatchTraceInfo(testManagementRequestContext, servicingContext, "TestMessageLogEntry", waterMark1, ref batchedWaterMark, ref tracePointCount);
        }
        else
        {
          TCMServiceDataMigrationHelper.SetWaterMarkInRegistry(testManagementRequestContext, 22, Guid.Empty, out waterMark1);
          batchedWaterMark = batchedWaterMark + ";" + waterMark1 + (object) DateTime.UtcNow;
          string message = "TCMServiceDataMigrationHelper: syncing testMessageLogEntry done. WaterMark: " + batchedWaterMark;
          TCMServiceDataMigrationHelper.LogInfo(servicingContext, testManagementRequestContext, message, nameof (TCMServiceDataMigrationHelper));
        }
      }
      while (!flag);
      TCMServiceDataMigrationHelper.LogInfo(servicingContext, testManagementRequestContext, "TCMServiceDataMigrationHelper: exit SyncTestMessageLogEntry", nameof (TCMServiceDataMigrationHelper));
    }

    internal static void SyncTestRuns2(
      TestManagementRequestContext testManagementRequestContext,
      IServicingContext servicingContext,
      Guid waterMarkProjectId,
      int waterMarkTestRunId,
      int batchSize)
    {
      TCMServiceDataMigrationHelper.LogInfo(servicingContext, testManagementRequestContext, "TCMServiceDataMigrationHelper: enter SyncTestRuns2", nameof (TCMServiceDataMigrationHelper));
      string waterMark1 = (string) null;
      string batchedWaterMark = string.Empty;
      int tracePointCount = 0;
      TCMServiceDataMigrationHttpClient client = testManagementRequestContext.RequestContext.GetClient<TCMServiceDataMigrationHttpClient>();
      List<TestRun2> testRuns = new List<TestRun2>();
      bool flag;
      do
      {
        TCMServiceDataMigrationHelper.ShouldCancelJob(testManagementRequestContext);
        int waterMarkDataspaceId = TCMServiceDataMigrationHelper.GetWaterMarkDataspaceId(testManagementRequestContext, waterMarkProjectId);
        using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(testManagementRequestContext))
          testRuns = managementDatabase.FetchTestRuns2(waterMarkDataspaceId, waterMarkTestRunId, batchSize);
        client.SyncTestRun2Async((IEnumerable<TestRun2>) testRuns).Wait();
        int count = testRuns.Count;
        string waterMark2 = count.ToString() + ";" + (object) waterMarkDataspaceId + ";" + (object) waterMarkTestRunId + ";" + (object) batchSize;
        TCMServiceDataMigrationHelper.TraceInfo(testManagementRequestContext, servicingContext, "TestRun2", waterMark2);
        flag = count < batchSize;
        if (!flag)
        {
          waterMarkProjectId = testRuns[count - 1].ProjectId;
          waterMarkTestRunId = testRuns[count - 1].TestRunId;
          TCMServiceDataMigrationHelper.SetWaterMarkInRegistry(testManagementRequestContext, 22, waterMarkProjectId, out waterMark1, waterMarkTestRunId);
          TCMServiceDataMigrationHelper.BatchTraceInfo(testManagementRequestContext, servicingContext, "TestRun2", waterMark1, ref batchedWaterMark, ref tracePointCount);
        }
        else
        {
          TCMServiceDataMigrationHelper.SetWaterMarkInRegistry(testManagementRequestContext, 7, Guid.Empty, out waterMark1);
          batchedWaterMark = batchedWaterMark + ";" + waterMark1 + (object) DateTime.UtcNow;
          string message = "TCMServiceDataMigrationHelper: syncing testRuns2 done. WaterMark: " + batchedWaterMark;
          TCMServiceDataMigrationHelper.LogInfo(servicingContext, testManagementRequestContext, message, nameof (TCMServiceDataMigrationHelper));
        }
      }
      while (!flag);
      TCMServiceDataMigrationHelper.LogInfo(servicingContext, testManagementRequestContext, "TCMServiceDataMigrationHelper: exit SyncTestRuns2", nameof (TCMServiceDataMigrationHelper));
    }

    internal static void SyncTestRuns(
      TestManagementRequestContext testManagementRequestContext,
      IServicingContext servicingContext,
      Guid waterMarkProjectId,
      int waterMarkTestRunId,
      int batchSize)
    {
      TCMServiceDataMigrationHelper.LogInfo(servicingContext, testManagementRequestContext, "TCMServiceDataMigrationHelper: enter SyncTestRuns", nameof (TCMServiceDataMigrationHelper));
      string waterMark1 = (string) null;
      string batchedWaterMark = string.Empty;
      int tracePointCount = 0;
      TCMServiceDataMigrationHttpClient client = testManagementRequestContext.RequestContext.GetClient<TCMServiceDataMigrationHttpClient>();
      List<TestRun2> testRuns = new List<TestRun2>();
      bool flag;
      do
      {
        TCMServiceDataMigrationHelper.ShouldCancelJob(testManagementRequestContext);
        int waterMarkDataspaceId = TCMServiceDataMigrationHelper.GetWaterMarkDataspaceId(testManagementRequestContext, waterMarkProjectId);
        using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(testManagementRequestContext))
          testRuns = managementDatabase.FetchTestRuns(waterMarkDataspaceId, waterMarkTestRunId, batchSize);
        client.SyncTestRunAsync((IEnumerable<TestRun2>) testRuns).Wait();
        int count = testRuns.Count;
        string waterMark2 = count.ToString() + ";" + (object) waterMarkDataspaceId + ";" + (object) waterMarkTestRunId + ";" + (object) batchSize;
        TCMServiceDataMigrationHelper.TraceInfo(testManagementRequestContext, servicingContext, "TestRun", waterMark2);
        flag = count < batchSize;
        if (!flag)
        {
          waterMarkProjectId = testRuns[count - 1].ProjectId;
          waterMarkTestRunId = testRuns[count - 1].TestRunId;
          TCMServiceDataMigrationHelper.SetWaterMarkInRegistry(testManagementRequestContext, 7, waterMarkProjectId, out waterMark1, waterMarkTestRunId);
          TCMServiceDataMigrationHelper.BatchTraceInfo(testManagementRequestContext, servicingContext, "TestRun", waterMark1, ref batchedWaterMark, ref tracePointCount);
        }
        else
        {
          TCMServiceDataMigrationHelper.SetWaterMarkInRegistry(testManagementRequestContext, 8, Guid.Empty, out waterMark1);
          batchedWaterMark = batchedWaterMark + ";" + waterMark1 + (object) DateTime.UtcNow;
          string message = "TCMServiceDataMigrationHelper: syncing testRuns done. WaterMark: " + batchedWaterMark;
          TCMServiceDataMigrationHelper.LogInfo(servicingContext, testManagementRequestContext, message, nameof (TCMServiceDataMigrationHelper));
        }
      }
      while (!flag);
      TCMServiceDataMigrationHelper.LogInfo(servicingContext, testManagementRequestContext, "TCMServiceDataMigrationHelper: exit SyncTestRuns", nameof (TCMServiceDataMigrationHelper));
    }

    internal static void SyncTestRunSummary(
      TestManagementRequestContext testManagementRequestContext,
      IServicingContext servicingContext,
      Guid waterMarkProjectId,
      int waterMarkTestRunId,
      int batchSize)
    {
      TCMServiceDataMigrationHelper.LogInfo(servicingContext, testManagementRequestContext, "TCMServiceDataMigrationHelper: enter SyncTestRunSummary", nameof (TCMServiceDataMigrationHelper));
      string waterMark1 = (string) null;
      string batchedWaterMark = string.Empty;
      int tracePointCount = 0;
      TCMServiceDataMigrationHttpClient client = testManagementRequestContext.RequestContext.GetClient<TCMServiceDataMigrationHttpClient>();
      List<TestRunSummary2> testRunSummary = new List<TestRunSummary2>();
      bool flag;
      do
      {
        TCMServiceDataMigrationHelper.ShouldCancelJob(testManagementRequestContext);
        int waterMarkDataspaceId = TCMServiceDataMigrationHelper.GetWaterMarkDataspaceId(testManagementRequestContext, waterMarkProjectId);
        using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(testManagementRequestContext))
          testRunSummary = managementDatabase.FetchTestRunSummary(waterMarkDataspaceId, waterMarkTestRunId, batchSize);
        client.SyncTestRunSummaryAsync((IEnumerable<TestRunSummary2>) testRunSummary).Wait();
        int count = testRunSummary.Count;
        string waterMark2 = count.ToString() + ";" + (object) waterMarkDataspaceId + ";" + (object) waterMarkTestRunId + ";" + (object) batchSize;
        TCMServiceDataMigrationHelper.TraceInfo(testManagementRequestContext, servicingContext, "TestRunSummary", waterMark2);
        flag = count < batchSize;
        if (!flag)
        {
          waterMarkProjectId = testRunSummary[count - 1].ProjectId;
          waterMarkTestRunId = testRunSummary[count - 1].TestRunId;
          TCMServiceDataMigrationHelper.SetWaterMarkInRegistry(testManagementRequestContext, 8, waterMarkProjectId, out waterMark1, waterMarkTestRunId);
          TCMServiceDataMigrationHelper.BatchTraceInfo(testManagementRequestContext, servicingContext, "TestRunSummary", waterMark1, ref batchedWaterMark, ref tracePointCount);
        }
        else
        {
          TCMServiceDataMigrationHelper.SetWaterMarkInRegistry(testManagementRequestContext, 9, Guid.Empty, out waterMark1);
          batchedWaterMark = batchedWaterMark + ";" + waterMark1 + (object) DateTime.UtcNow;
          string message = "TCMServiceDataMigrationHelper: syncing testRunSummary done. WaterMark: " + batchedWaterMark;
          TCMServiceDataMigrationHelper.LogInfo(servicingContext, testManagementRequestContext, message, nameof (TCMServiceDataMigrationHelper));
        }
      }
      while (!flag);
      TCMServiceDataMigrationHelper.LogInfo(servicingContext, testManagementRequestContext, "TCMServiceDataMigrationHelper: exit SyncTestRunSummary", nameof (TCMServiceDataMigrationHelper));
    }

    internal static void SyncTestResultsEx(
      TestManagementRequestContext testManagementRequestContext,
      IServicingContext servicingContext,
      Guid waterMarkProjectId,
      int waterMarkTestRunId,
      int waterMarkTestResultId,
      int waterMarkFieldId,
      int batchSize)
    {
      TCMServiceDataMigrationHelper.LogInfo(servicingContext, testManagementRequestContext, "TCMServiceDataMigrationHelper: enter SyncTestResultsEx", nameof (TCMServiceDataMigrationHelper));
      string waterMark1 = (string) null;
      string batchedWaterMark = string.Empty;
      int tracePointCount = 0;
      TCMServiceDataMigrationHttpClient client = testManagementRequestContext.RequestContext.GetClient<TCMServiceDataMigrationHttpClient>();
      List<TestResultsEx2> testResultsEx = new List<TestResultsEx2>();
      bool flag;
      do
      {
        TCMServiceDataMigrationHelper.ShouldCancelJob(testManagementRequestContext);
        int waterMarkDataspaceId = TCMServiceDataMigrationHelper.GetWaterMarkDataspaceId(testManagementRequestContext, waterMarkProjectId);
        using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(testManagementRequestContext))
          testResultsEx = managementDatabase.FetchTestResultsEx(waterMarkDataspaceId, waterMarkTestRunId, waterMarkTestResultId, waterMarkFieldId, batchSize);
        client.SyncTestResultsExAsync((IEnumerable<TestResultsEx2>) testResultsEx).Wait();
        int count = testResultsEx.Count;
        string waterMark2 = count.ToString() + ";" + (object) waterMarkDataspaceId + ";" + (object) waterMarkTestRunId + ";" + (object) waterMarkTestResultId + ";" + (object) waterMarkFieldId + ";" + (object) batchSize;
        TCMServiceDataMigrationHelper.TraceInfo(testManagementRequestContext, servicingContext, "TestResultsEx", waterMark2);
        flag = count < batchSize;
        if (!flag)
        {
          waterMarkProjectId = testResultsEx[count - 1].ProjectId;
          waterMarkTestRunId = testResultsEx[count - 1].TestRunId;
          waterMarkTestResultId = testResultsEx[count - 1].TestResultId;
          waterMarkFieldId = testResultsEx[count - 1].FieldId;
          TCMServiceDataMigrationHelper.SetWaterMarkInRegistry(testManagementRequestContext, 9, waterMarkProjectId, out waterMark1, waterMarkTestRunId, waterMarkTestResultId, waterMarkFieldId);
          TCMServiceDataMigrationHelper.BatchTraceInfo(testManagementRequestContext, servicingContext, "TestResultsEx", waterMark1, ref batchedWaterMark, ref tracePointCount);
        }
        else
        {
          TCMServiceDataMigrationHelper.SetWaterMarkInRegistry(testManagementRequestContext, 10, Guid.Empty, out waterMark1);
          batchedWaterMark = batchedWaterMark + ";" + waterMark1 + (object) DateTime.UtcNow;
          string message = "TCMServiceDataMigrationHelper: syncing testResultsEx done. WaterMark: " + batchedWaterMark;
          TCMServiceDataMigrationHelper.LogInfo(servicingContext, testManagementRequestContext, message, nameof (TCMServiceDataMigrationHelper));
        }
      }
      while (!flag);
      TCMServiceDataMigrationHelper.LogInfo(servicingContext, testManagementRequestContext, "TCMServiceDataMigrationHelper: exit SyncTestResultsEx", nameof (TCMServiceDataMigrationHelper));
    }

    internal static void SyncTestCaseMetadata(
      TestManagementRequestContext testManagementRequestContext,
      IServicingContext servicingContext,
      Guid waterMarkProjectId,
      int waterMarkTestMetadataId,
      int batchSize)
    {
      TCMServiceDataMigrationHelper.LogInfo(servicingContext, testManagementRequestContext, "TCMServiceDataMigrationHelper: enter SyncTestCaseMetaData", nameof (TCMServiceDataMigrationHelper));
      string waterMark1 = (string) null;
      string batchedWaterMark = string.Empty;
      int tracePointCount = 0;
      TCMServiceDataMigrationHttpClient client = testManagementRequestContext.RequestContext.GetClient<TCMServiceDataMigrationHttpClient>();
      List<TestCaseMetadata2> testCaseMetadata = new List<TestCaseMetadata2>();
      bool flag;
      do
      {
        TCMServiceDataMigrationHelper.ShouldCancelJob(testManagementRequestContext);
        int waterMarkDataspaceId = TCMServiceDataMigrationHelper.GetWaterMarkDataspaceId(testManagementRequestContext, waterMarkProjectId);
        using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(testManagementRequestContext))
          testCaseMetadata = managementDatabase.FetchTestCaseMetadata(waterMarkDataspaceId, waterMarkTestMetadataId, batchSize);
        client.SyncTestCaseMetadataAsync((IEnumerable<TestCaseMetadata2>) testCaseMetadata).Wait();
        int count = testCaseMetadata.Count;
        string waterMark2 = count.ToString() + ";" + (object) waterMarkDataspaceId + ";" + (object) waterMarkTestMetadataId + ";" + (object) batchSize;
        TCMServiceDataMigrationHelper.TraceInfo(testManagementRequestContext, servicingContext, "TestCaseMetadata", waterMark2);
        flag = count < batchSize;
        if (!flag)
        {
          waterMarkProjectId = testCaseMetadata[count - 1].ProjectId;
          waterMarkTestMetadataId = testCaseMetadata[count - 1].TestMetadataId;
          TCMServiceDataMigrationHelper.SetWaterMarkInRegistry(testManagementRequestContext, 10, waterMarkProjectId, out waterMark1, waterMarkTestMetadataId);
          TCMServiceDataMigrationHelper.BatchTraceInfo(testManagementRequestContext, servicingContext, "TestCaseMetadata", waterMark1, ref batchedWaterMark, ref tracePointCount);
        }
        else
        {
          TCMServiceDataMigrationHelper.SetWaterMarkInRegistry(testManagementRequestContext, 11, Guid.Empty, out waterMark1);
          batchedWaterMark = batchedWaterMark + ";" + waterMark1 + (object) DateTime.UtcNow;
          string message = "TCMServiceDataMigrationHelper: syncing testCaseMetadata done. WaterMark: " + batchedWaterMark;
          TCMServiceDataMigrationHelper.LogInfo(servicingContext, testManagementRequestContext, message, nameof (TCMServiceDataMigrationHelper));
        }
      }
      while (!flag);
      TCMServiceDataMigrationHelper.LogInfo(servicingContext, testManagementRequestContext, "TCMServiceDataMigrationHelper: exit SyncTestCaseMetadata", nameof (TCMServiceDataMigrationHelper));
    }

    internal static void SyncRequirementsToTestsMapping(
      TestManagementRequestContext testManagementRequestContext,
      IServicingContext servicingContext,
      Guid waterMarkProjectId,
      int waterMarkWorkItemId,
      int batchSize)
    {
      TCMServiceDataMigrationHelper.LogInfo(servicingContext, testManagementRequestContext, "TCMServiceDataMigrationHelper: enter SyncRequirementsToTestsMapping", nameof (TCMServiceDataMigrationHelper));
      string waterMark1 = (string) null;
      string batchedWaterMark = string.Empty;
      int tracePointCount = 0;
      TCMServiceDataMigrationHttpClient client = testManagementRequestContext.RequestContext.GetClient<TCMServiceDataMigrationHttpClient>();
      List<RequirementsToTestsMapping2> requirementsMapping = new List<RequirementsToTestsMapping2>();
      bool flag;
      do
      {
        TCMServiceDataMigrationHelper.ShouldCancelJob(testManagementRequestContext);
        int waterMarkDataspaceId = TCMServiceDataMigrationHelper.GetWaterMarkDataspaceId(testManagementRequestContext, waterMarkProjectId);
        using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(testManagementRequestContext))
          requirementsMapping = managementDatabase.FetchRequirementsToTestsMapping(waterMarkDataspaceId, waterMarkWorkItemId, batchSize);
        client.SyncRequirementsToTestsMappingAsync((IEnumerable<RequirementsToTestsMapping2>) requirementsMapping).Wait();
        int count = requirementsMapping.Count;
        string waterMark2 = count.ToString() + ";" + (object) waterMarkDataspaceId + ";" + (object) waterMarkWorkItemId + ";" + (object) batchSize;
        TCMServiceDataMigrationHelper.TraceInfo(testManagementRequestContext, servicingContext, "RequirementsToTestsMapping", waterMark2);
        flag = count < batchSize;
        if (!flag)
        {
          waterMarkProjectId = requirementsMapping[count - 1].ProjectId;
          waterMarkWorkItemId = requirementsMapping[count - 1].WorkItemId;
          TCMServiceDataMigrationHelper.SetWaterMarkInRegistry(testManagementRequestContext, 11, waterMarkProjectId, out waterMark1, waterMarkWorkItemId);
          TCMServiceDataMigrationHelper.BatchTraceInfo(testManagementRequestContext, servicingContext, "RequirementsToTestsMapping", waterMark1, ref batchedWaterMark, ref tracePointCount);
        }
        else
        {
          TCMServiceDataMigrationHelper.SetWaterMarkInRegistry(testManagementRequestContext, 12, Guid.Empty, out waterMark1);
          batchedWaterMark = batchedWaterMark + ";" + waterMark1 + (object) DateTime.UtcNow;
          string message = "TCMServiceDataMigrationHelper: syncing RequirementsToTestsMapping done. WaterMark: " + batchedWaterMark;
          TCMServiceDataMigrationHelper.LogInfo(servicingContext, testManagementRequestContext, message, nameof (TCMServiceDataMigrationHelper));
        }
      }
      while (!flag);
      TCMServiceDataMigrationHelper.LogInfo(servicingContext, testManagementRequestContext, "TCMServiceDataMigrationHelper: exit RequirementsToTestsMapping", nameof (TCMServiceDataMigrationHelper));
    }

    internal static void SyncTestResultReset(
      TestManagementRequestContext testManagementRequestContext,
      IServicingContext servicingContext,
      Guid waterMarkProjectId,
      int waterMarkTestRunId,
      int waterMarkTestResultId,
      int waterMarkRevision,
      int batchSize)
    {
      TCMServiceDataMigrationHelper.LogInfo(servicingContext, testManagementRequestContext, "TCMServiceDataMigrationHelper: enter SyncTestResultReset", nameof (TCMServiceDataMigrationHelper));
      string waterMark1 = (string) null;
      string batchedWaterMark = string.Empty;
      int tracePointCount = 0;
      TCMServiceDataMigrationHttpClient client = testManagementRequestContext.RequestContext.GetClient<TCMServiceDataMigrationHttpClient>();
      List<TestResultReset2> testResultResets = new List<TestResultReset2>();
      bool flag;
      do
      {
        TCMServiceDataMigrationHelper.ShouldCancelJob(testManagementRequestContext);
        int waterMarkDataspaceId = TCMServiceDataMigrationHelper.GetWaterMarkDataspaceId(testManagementRequestContext, waterMarkProjectId);
        using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(testManagementRequestContext))
          testResultResets = managementDatabase.FetchTestResultReset(waterMarkDataspaceId, waterMarkTestRunId, waterMarkTestResultId, waterMarkRevision, batchSize);
        client.SyncTestResultResetAsync((IEnumerable<TestResultReset2>) testResultResets).Wait();
        int count = testResultResets.Count;
        string waterMark2 = count.ToString() + ";" + (object) waterMarkDataspaceId + ";" + (object) waterMarkTestRunId + ";" + (object) waterMarkTestResultId + ";" + (object) waterMarkRevision + ";" + (object) batchSize;
        TCMServiceDataMigrationHelper.TraceInfo(testManagementRequestContext, servicingContext, "TestResultReset", waterMark2);
        flag = count < batchSize;
        if (!flag)
        {
          waterMarkProjectId = testResultResets[count - 1].ProjectId;
          waterMarkTestRunId = testResultResets[count - 1].TestRunId;
          waterMarkTestResultId = testResultResets[count - 1].TestResultId;
          waterMarkRevision = testResultResets[count - 1].Revision;
          TCMServiceDataMigrationHelper.SetWaterMarkInRegistry(testManagementRequestContext, 12, waterMarkProjectId, out waterMark1, waterMarkTestRunId, waterMarkTestResultId, waterMarkRevision);
          TCMServiceDataMigrationHelper.BatchTraceInfo(testManagementRequestContext, servicingContext, "TestResultReset", waterMark1, ref batchedWaterMark, ref tracePointCount);
        }
        else
        {
          TCMServiceDataMigrationHelper.SetWaterMarkInRegistry(testManagementRequestContext, 13, Guid.Empty, out waterMark1);
          batchedWaterMark = batchedWaterMark + ";" + waterMark1 + (object) DateTime.UtcNow;
          string message = "TCMServiceDataMigrationHelper: syncing TestResultsResets done. WaterMark: " + batchedWaterMark;
          TCMServiceDataMigrationHelper.LogInfo(servicingContext, testManagementRequestContext, message, nameof (TCMServiceDataMigrationHelper));
        }
      }
      while (!flag);
      TCMServiceDataMigrationHelper.LogInfo(servicingContext, testManagementRequestContext, "TCMServiceDataMigrationHelper: exit TestResultsResets", nameof (TCMServiceDataMigrationHelper));
    }

    internal static void SyncTestActionResult(
      TestManagementRequestContext testManagementRequestContext,
      IServicingContext servicingContext,
      int waterMarkTestRunId,
      int waterMarkTestResultId,
      int waterMarkIterationId,
      int batchSize)
    {
      TCMServiceDataMigrationHelper.LogInfo(servicingContext, testManagementRequestContext, "TCMServiceDataMigrationHelper: enter SyncTestActionResult", nameof (TCMServiceDataMigrationHelper));
      string waterMark1 = (string) null;
      string batchedWaterMark = string.Empty;
      int tracePointCount = 0;
      TCMServiceDataMigrationHttpClient client = testManagementRequestContext.RequestContext.GetClient<TCMServiceDataMigrationHttpClient>();
      List<TestActionResult2> testActionResults = new List<TestActionResult2>();
      bool flag;
      do
      {
        TCMServiceDataMigrationHelper.ShouldCancelJob(testManagementRequestContext);
        using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(testManagementRequestContext))
          testActionResults = managementDatabase.FetchTestActionResult(waterMarkTestRunId, waterMarkTestResultId, waterMarkIterationId, batchSize);
        client.SyncTestActionResultAsync((IEnumerable<TestActionResult2>) testActionResults).Wait();
        int count = testActionResults.Count;
        string waterMark2 = count.ToString() + ";" + (object) waterMarkTestRunId + ";" + (object) waterMarkTestResultId + ";" + (object) waterMarkIterationId + ";" + (object) batchSize;
        TCMServiceDataMigrationHelper.TraceInfo(testManagementRequestContext, servicingContext, "TestActionResult", waterMark2);
        flag = count < batchSize;
        if (!flag)
        {
          waterMarkTestRunId = testActionResults[count - 1].TestRunId;
          waterMarkTestResultId = testActionResults[count - 1].TestResultId;
          waterMarkIterationId = testActionResults[count - 1].IterationId;
          TCMServiceDataMigrationHelper.SetWaterMarkInRegistry(testManagementRequestContext, 13, Guid.Empty, out waterMark1, waterMarkTestRunId, waterMarkTestResultId, waterMarkIterationId);
          TCMServiceDataMigrationHelper.BatchTraceInfo(testManagementRequestContext, servicingContext, "TestActionResult", waterMark1, ref batchedWaterMark, ref tracePointCount);
        }
        else
        {
          TCMServiceDataMigrationHelper.SetWaterMarkInRegistry(testManagementRequestContext, 14, Guid.Empty, out waterMark1);
          batchedWaterMark = batchedWaterMark + ";" + waterMark1 + (object) DateTime.UtcNow;
          string message = "TCMServiceDataMigrationHelper: syncing SyncTestActionResult done. WaterMark: " + batchedWaterMark;
          TCMServiceDataMigrationHelper.LogInfo(servicingContext, testManagementRequestContext, message, nameof (TCMServiceDataMigrationHelper));
        }
      }
      while (!flag);
      TCMServiceDataMigrationHelper.LogInfo(servicingContext, testManagementRequestContext, "TCMServiceDataMigrationHelper: exit SyncTestActionResult", nameof (TCMServiceDataMigrationHelper));
    }

    internal static void SyncTestRunEx(
      TestManagementRequestContext testManagementRequestContext,
      IServicingContext servicingContext,
      Guid waterMarkProjectId,
      int waterMarkTestRunId,
      int waterMarkFieldId,
      int batchSize)
    {
      TCMServiceDataMigrationHelper.LogInfo(servicingContext, testManagementRequestContext, "TCMServiceDataMigrationHelper: enter SyncTestRunEx", nameof (TCMServiceDataMigrationHelper));
      string waterMark1 = (string) null;
      string batchedWaterMark = string.Empty;
      int tracePointCount = 0;
      TCMServiceDataMigrationHttpClient client = testManagementRequestContext.RequestContext.GetClient<TCMServiceDataMigrationHttpClient>();
      List<TestRunEx2> testRunEx = new List<TestRunEx2>();
      bool flag;
      do
      {
        TCMServiceDataMigrationHelper.ShouldCancelJob(testManagementRequestContext);
        int waterMarkDataspaceId = TCMServiceDataMigrationHelper.GetWaterMarkDataspaceId(testManagementRequestContext, waterMarkProjectId);
        using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(testManagementRequestContext))
          testRunEx = managementDatabase.FetchTestRunEx(waterMarkDataspaceId, waterMarkTestRunId, waterMarkFieldId, batchSize);
        client.SyncTestRunExAsync((IEnumerable<TestRunEx2>) testRunEx).Wait();
        int count = testRunEx.Count;
        string waterMark2 = count.ToString() + ";" + (object) waterMarkDataspaceId + ";" + (object) waterMarkTestRunId + ";" + (object) waterMarkFieldId + ";" + (object) batchSize;
        TCMServiceDataMigrationHelper.TraceInfo(testManagementRequestContext, servicingContext, "TestRunEx", waterMark2);
        flag = count < batchSize;
        if (!flag)
        {
          waterMarkProjectId = testRunEx[count - 1].ProjectId;
          waterMarkTestRunId = testRunEx[count - 1].TestRunId;
          waterMarkFieldId = testRunEx[count - 1].FieldId;
          TCMServiceDataMigrationHelper.SetWaterMarkInRegistry(testManagementRequestContext, 14, waterMarkProjectId, out waterMark1, waterMarkTestRunId, waterMarkFieldId);
          TCMServiceDataMigrationHelper.BatchTraceInfo(testManagementRequestContext, servicingContext, "TestRunEx", waterMark1, ref batchedWaterMark, ref tracePointCount);
        }
        else
        {
          TCMServiceDataMigrationHelper.SetWaterMarkInRegistry(testManagementRequestContext, 15, Guid.Empty, out waterMark1);
          batchedWaterMark = batchedWaterMark + ";" + waterMark1 + (object) DateTime.UtcNow;
          string message = "TCMServiceDataMigrationHelper: syncing SyncTestRunEx done. WaterMark: " + batchedWaterMark;
          TCMServiceDataMigrationHelper.LogInfo(servicingContext, testManagementRequestContext, message, nameof (TCMServiceDataMigrationHelper));
        }
      }
      while (!flag);
      TCMServiceDataMigrationHelper.LogInfo(servicingContext, testManagementRequestContext, "TCMServiceDataMigrationHelper: exit SyncTestRunEx", nameof (TCMServiceDataMigrationHelper));
    }

    internal static void SyncTestRunExtended(
      TestManagementRequestContext testManagementRequestContext,
      IServicingContext servicingContext,
      Guid waterMarkProjectId,
      int waterMarkTestRunId,
      int batchSize)
    {
      TCMServiceDataMigrationHelper.LogInfo(servicingContext, testManagementRequestContext, "TCMServiceDataMigrationHelper: enter SyncTestRunExtended", nameof (TCMServiceDataMigrationHelper));
      string waterMark1 = (string) null;
      string batchedWaterMark = string.Empty;
      int tracePointCount = 0;
      TCMServiceDataMigrationHttpClient client = testManagementRequestContext.RequestContext.GetClient<TCMServiceDataMigrationHttpClient>();
      List<TestRunExtended2> testRunExtended = new List<TestRunExtended2>();
      bool flag;
      do
      {
        TCMServiceDataMigrationHelper.ShouldCancelJob(testManagementRequestContext);
        int waterMarkDataspaceId = TCMServiceDataMigrationHelper.GetWaterMarkDataspaceId(testManagementRequestContext, waterMarkProjectId);
        using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(testManagementRequestContext))
          testRunExtended = managementDatabase.FetchTestRunExtended(waterMarkDataspaceId, waterMarkTestRunId, batchSize);
        client.SyncTestRunExtendedAsync((IEnumerable<TestRunExtended2>) testRunExtended).Wait();
        int count = testRunExtended.Count;
        string waterMark2 = count.ToString() + ";" + (object) waterMarkDataspaceId + ";" + (object) waterMarkTestRunId + ";" + (object) batchSize;
        TCMServiceDataMigrationHelper.TraceInfo(testManagementRequestContext, servicingContext, "TestRunExtended", waterMark2);
        flag = count < batchSize;
        if (!flag)
        {
          waterMarkProjectId = testRunExtended[count - 1].ProjectId;
          waterMarkTestRunId = testRunExtended[count - 1].TestRunId;
          TCMServiceDataMigrationHelper.SetWaterMarkInRegistry(testManagementRequestContext, 15, waterMarkProjectId, out waterMark1, waterMarkTestRunId);
          TCMServiceDataMigrationHelper.BatchTraceInfo(testManagementRequestContext, servicingContext, "TestRunExtended", waterMark1, ref batchedWaterMark, ref tracePointCount);
        }
        else
        {
          TCMServiceDataMigrationHelper.SetWaterMarkInRegistry(testManagementRequestContext, 16, Guid.Empty, out waterMark1);
          batchedWaterMark = batchedWaterMark + ";" + waterMark1 + (object) DateTime.UtcNow;
          string message = "TCMServiceDataMigrationHelper: syncing SyncTestRunExtended done. WaterMark: " + batchedWaterMark;
          TCMServiceDataMigrationHelper.LogInfo(servicingContext, testManagementRequestContext, message, nameof (TCMServiceDataMigrationHelper));
        }
      }
      while (!flag);
      TCMServiceDataMigrationHelper.LogInfo(servicingContext, testManagementRequestContext, "TCMServiceDataMigrationHelper: exit SyncTestRunExtended", nameof (TCMServiceDataMigrationHelper));
    }

    internal static void SyncTestParameter(
      TestManagementRequestContext testManagementRequestContext,
      IServicingContext servicingContext,
      int waterMarkTestRunId,
      int waterMarkTestResultId,
      int waterMarkIterationId,
      int batchSize)
    {
      TCMServiceDataMigrationHelper.LogInfo(servicingContext, testManagementRequestContext, "TCMServiceDataMigrationHelper: enter SyncTestParameter", nameof (TCMServiceDataMigrationHelper));
      string waterMark1 = (string) null;
      string batchedWaterMark = string.Empty;
      int tracePointCount = 0;
      TCMServiceDataMigrationHttpClient client = testManagementRequestContext.RequestContext.GetClient<TCMServiceDataMigrationHttpClient>();
      List<TestParameter2> testParameter = new List<TestParameter2>();
      bool flag;
      do
      {
        TCMServiceDataMigrationHelper.ShouldCancelJob(testManagementRequestContext);
        using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(testManagementRequestContext))
          testParameter = managementDatabase.FetchTestParameter(waterMarkTestRunId, waterMarkTestResultId, waterMarkIterationId, batchSize);
        client.SyncTestParameterAsync((IEnumerable<TestParameter2>) testParameter).Wait();
        int count = testParameter.Count;
        string waterMark2 = count.ToString() + ";" + (object) waterMarkTestRunId + ";" + (object) waterMarkTestResultId + ";" + (object) waterMarkIterationId + ";" + (object) batchSize;
        TCMServiceDataMigrationHelper.TraceInfo(testManagementRequestContext, servicingContext, "TestParameter", waterMark2);
        flag = count < batchSize;
        if (!flag)
        {
          waterMarkTestRunId = testParameter[count - 1].TestRunId;
          waterMarkTestResultId = testParameter[count - 1].TestResultId;
          waterMarkIterationId = testParameter[count - 1].IterationId;
          TCMServiceDataMigrationHelper.SetWaterMarkInRegistry(testManagementRequestContext, 16, Guid.Empty, out waterMark1, waterMarkTestRunId, waterMarkTestResultId, waterMarkIterationId);
          TCMServiceDataMigrationHelper.BatchTraceInfo(testManagementRequestContext, servicingContext, "TestParameter", waterMark1, ref batchedWaterMark, ref tracePointCount);
        }
        else
        {
          TCMServiceDataMigrationHelper.SetWaterMarkInRegistry(testManagementRequestContext, 18, Guid.Empty, out waterMark1);
          batchedWaterMark = batchedWaterMark + ";" + waterMark1 + (object) DateTime.UtcNow;
          string message = "TCMServiceDataMigrationHelper: syncing SyncTestParameter done. WaterMark: " + batchedWaterMark;
          TCMServiceDataMigrationHelper.LogInfo(servicingContext, testManagementRequestContext, message, nameof (TCMServiceDataMigrationHelper));
        }
      }
      while (!flag);
      TCMServiceDataMigrationHelper.LogInfo(servicingContext, testManagementRequestContext, "TCMServiceDataMigrationHelper: exit SyncTestParameter", nameof (TCMServiceDataMigrationHelper));
    }

    internal static void SyncModuleCoverage(
      TestManagementRequestContext testManagementRequestContext,
      IServicingContext servicingContext,
      int waterMarkCoverageId,
      int waterMarkModuleId,
      int batchSize)
    {
      TCMServiceDataMigrationHelper.LogInfo(servicingContext, testManagementRequestContext, "TCMServiceDataMigrationHelper: enter SyncModuleCoverage", nameof (TCMServiceDataMigrationHelper));
      string waterMark1 = (string) null;
      string batchedWaterMark = string.Empty;
      int tracePointCount = 0;
      TCMServiceDataMigrationHttpClient client = testManagementRequestContext.RequestContext.GetClient<TCMServiceDataMigrationHttpClient>();
      List<ModuleCoverage2> moduleCoverages = new List<ModuleCoverage2>();
      bool flag;
      do
      {
        TCMServiceDataMigrationHelper.ShouldCancelJob(testManagementRequestContext);
        using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(testManagementRequestContext))
          moduleCoverages = managementDatabase.FetchModuleCoverage(waterMarkCoverageId, waterMarkModuleId, batchSize);
        client.SyncModuleCoverageAsync((IEnumerable<ModuleCoverage2>) moduleCoverages).Wait();
        int count = moduleCoverages.Count;
        string waterMark2 = count.ToString() + ";" + (object) waterMarkCoverageId + ";" + (object) waterMarkModuleId + ";" + (object) batchSize;
        TCMServiceDataMigrationHelper.TraceInfo(testManagementRequestContext, servicingContext, "ModuleCoverage", waterMark2);
        flag = count < batchSize;
        if (!flag)
        {
          waterMarkCoverageId = moduleCoverages[count - 1].CoverageId;
          waterMarkModuleId = moduleCoverages[count - 1].ModuleId;
          TCMServiceDataMigrationHelper.SetWaterMarkInRegistry(testManagementRequestContext, 18, Guid.Empty, out waterMark1, waterMarkCoverageId, waterMarkModuleId);
          TCMServiceDataMigrationHelper.BatchTraceInfo(testManagementRequestContext, servicingContext, "ModuleCoverage", waterMark1, ref batchedWaterMark, ref tracePointCount);
        }
        else
        {
          TCMServiceDataMigrationHelper.SetWaterMarkInRegistry(testManagementRequestContext, 20, Guid.Empty, out waterMark1);
          batchedWaterMark = batchedWaterMark + ";" + waterMark1 + (object) DateTime.UtcNow;
          string message = "TCMServiceDataMigrationHelper: syncing SyncModuleCoverage done. WaterMark: " + batchedWaterMark;
          TCMServiceDataMigrationHelper.LogInfo(servicingContext, testManagementRequestContext, message, nameof (TCMServiceDataMigrationHelper));
        }
      }
      while (!flag);
      TCMServiceDataMigrationHelper.LogInfo(servicingContext, testManagementRequestContext, "TCMServiceDataMigrationHelper: exit SyncModuleCoverage", nameof (TCMServiceDataMigrationHelper));
    }

    internal static void SyncFunctionCoverage(
      TestManagementRequestContext testManagementRequestContext,
      IServicingContext servicingContext,
      int waterMarkCoverageId,
      int waterMarkModuleId,
      int waterMarkFunctionId,
      int batchSize)
    {
      TCMServiceDataMigrationHelper.LogInfo(servicingContext, testManagementRequestContext, "TCMServiceDataMigrationHelper: enter SyncFunctionCoverage", nameof (TCMServiceDataMigrationHelper));
      string waterMark1 = (string) null;
      string batchedWaterMark = string.Empty;
      int tracePointCount = 0;
      TCMServiceDataMigrationHttpClient client = testManagementRequestContext.RequestContext.GetClient<TCMServiceDataMigrationHttpClient>();
      List<FunctionCoverage2> functionCoverages = new List<FunctionCoverage2>();
      bool flag;
      do
      {
        TCMServiceDataMigrationHelper.ShouldCancelJob(testManagementRequestContext);
        using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(testManagementRequestContext))
          functionCoverages = managementDatabase.FetchFunctionCoverage(waterMarkCoverageId, waterMarkModuleId, waterMarkFunctionId, batchSize);
        client.SyncFunctionCoverageAsync((IEnumerable<FunctionCoverage2>) functionCoverages).Wait();
        int count = functionCoverages.Count;
        string waterMark2 = count.ToString() + ";" + (object) waterMarkCoverageId + ";" + (object) waterMarkModuleId + ";" + (object) waterMarkFunctionId + ";" + (object) batchSize;
        TCMServiceDataMigrationHelper.TraceInfo(testManagementRequestContext, servicingContext, "FunctionCoverage", waterMark2);
        flag = count < batchSize;
        if (!flag)
        {
          waterMarkCoverageId = functionCoverages[count - 1].CoverageId;
          waterMarkFunctionId = functionCoverages[count - 1].FunctionId;
          waterMarkModuleId = functionCoverages[count - 1].ModuleId;
          TCMServiceDataMigrationHelper.SetWaterMarkInRegistry(testManagementRequestContext, 19, Guid.Empty, out waterMark1, waterMarkCoverageId, waterMarkModuleId, waterMarkFunctionId);
          TCMServiceDataMigrationHelper.BatchTraceInfo(testManagementRequestContext, servicingContext, "FunctionCoverage", waterMark1, ref batchedWaterMark, ref tracePointCount);
        }
        else
        {
          TCMServiceDataMigrationHelper.SetWaterMarkInRegistry(testManagementRequestContext, 20, Guid.Empty, out waterMark1);
          batchedWaterMark = batchedWaterMark + ";" + waterMark1 + (object) DateTime.UtcNow;
          string message = "TCMServiceDataMigrationHelper: syncing SyncFunctionCoverage done. WaterMark: " + batchedWaterMark;
          TCMServiceDataMigrationHelper.LogInfo(servicingContext, testManagementRequestContext, message, nameof (TCMServiceDataMigrationHelper));
        }
      }
      while (!flag);
      TCMServiceDataMigrationHelper.LogInfo(servicingContext, testManagementRequestContext, "TCMServiceDataMigrationHelper: exit SyncFunctionCoverage", nameof (TCMServiceDataMigrationHelper));
    }

    internal static void SyncCodeCoverageSummary(
      TestManagementRequestContext testManagementRequestContext,
      IServicingContext servicingContext,
      Guid waterMarkProjectId,
      int waterMarkBuildConfigurationId,
      int batchSize)
    {
      TCMServiceDataMigrationHelper.LogInfo(servicingContext, testManagementRequestContext, "TCMServiceDataMigrationHelper: enter SyncCodeCoverageSummary", nameof (TCMServiceDataMigrationHelper));
      string waterMark1 = (string) null;
      string batchedWaterMark = string.Empty;
      int tracePointCount = 0;
      TCMServiceDataMigrationHttpClient client = testManagementRequestContext.RequestContext.GetClient<TCMServiceDataMigrationHttpClient>();
      List<CodeCoverageSummary2> codeCoverageSummary2 = new List<CodeCoverageSummary2>();
      bool flag;
      do
      {
        TCMServiceDataMigrationHelper.ShouldCancelJob(testManagementRequestContext);
        int waterMarkDataspaceId = TCMServiceDataMigrationHelper.GetWaterMarkDataspaceId(testManagementRequestContext, waterMarkProjectId);
        using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(testManagementRequestContext))
          codeCoverageSummary2 = managementDatabase.FetchCodeCoverageSummary(waterMarkDataspaceId, waterMarkBuildConfigurationId, batchSize);
        client.SyncCodeCoverageSummaryAsync((IEnumerable<CodeCoverageSummary2>) codeCoverageSummary2).Wait();
        int count = codeCoverageSummary2.Count;
        string waterMark2 = count.ToString() + ";" + (object) waterMarkDataspaceId + ";" + (object) waterMarkBuildConfigurationId + ";" + (object) batchSize;
        TCMServiceDataMigrationHelper.TraceInfo(testManagementRequestContext, servicingContext, "CodeCoverageSummary", waterMark2);
        flag = count < batchSize;
        if (!flag)
        {
          waterMarkProjectId = codeCoverageSummary2[count - 1].ProjectId;
          waterMarkBuildConfigurationId = codeCoverageSummary2[count - 1].BuildConfigurationId;
          TCMServiceDataMigrationHelper.SetWaterMarkInRegistry(testManagementRequestContext, 20, waterMarkProjectId, out waterMark1, waterMarkBuildConfigurationId);
          TCMServiceDataMigrationHelper.BatchTraceInfo(testManagementRequestContext, servicingContext, "CodeCoverageSummary", waterMark1, ref batchedWaterMark, ref tracePointCount);
        }
        else
        {
          TCMServiceDataMigrationHelper.SetWaterMarkInRegistry(testManagementRequestContext, 21, Guid.Empty, out waterMark1);
          batchedWaterMark = batchedWaterMark + ";" + waterMark1 + (object) DateTime.UtcNow;
          string message = "TCMServiceDataMigrationHelper: syncing SyncCodeCoverageSummary done. WaterMark: " + batchedWaterMark;
          TCMServiceDataMigrationHelper.LogInfo(servicingContext, testManagementRequestContext, message, nameof (TCMServiceDataMigrationHelper));
        }
      }
      while (!flag);
      TCMServiceDataMigrationHelper.LogInfo(servicingContext, testManagementRequestContext, "TCMServiceDataMigrationHelper: exit SyncCodeCoverageSummary", nameof (TCMServiceDataMigrationHelper));
    }

    internal static void SyncTCMPropertyBag(
      TestManagementRequestContext testManagementRequestContext,
      IServicingContext servicingContext,
      int waterMarkArtifactType,
      int waterMarkArtifactId,
      int batchSize)
    {
      TCMServiceDataMigrationHelper.LogInfo(servicingContext, testManagementRequestContext, "TCMServiceDataMigrationHelper: enter SyncTCMPropertyBag", nameof (TCMServiceDataMigrationHelper));
      string waterMark1 = (string) null;
      string batchedWaterMark = string.Empty;
      int tracePointCount = 0;
      TCMServiceDataMigrationHttpClient client = testManagementRequestContext.RequestContext.GetClient<TCMServiceDataMigrationHttpClient>();
      List<TCMPropertyBag2> tcmPropertyBag = new List<TCMPropertyBag2>();
      bool flag;
      do
      {
        TCMServiceDataMigrationHelper.ShouldCancelJob(testManagementRequestContext);
        using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(testManagementRequestContext))
          tcmPropertyBag = managementDatabase.FetchTCMPropertyBag(waterMarkArtifactType, waterMarkArtifactId, batchSize);
        client.SyncTCMPropertyBagAsync((IEnumerable<TCMPropertyBag2>) tcmPropertyBag).Wait();
        int count = tcmPropertyBag.Count;
        string waterMark2 = count.ToString() + ";" + (object) waterMarkArtifactType + ";" + (object) waterMarkArtifactId + ";" + (object) batchSize;
        TCMServiceDataMigrationHelper.TraceInfo(testManagementRequestContext, servicingContext, "TCMPropertyBag", waterMark2);
        flag = count < batchSize;
        if (!flag)
        {
          waterMarkArtifactType = tcmPropertyBag[count - 1].ArtifactType;
          waterMarkArtifactId = tcmPropertyBag[count - 1].ArtifactId;
          TCMServiceDataMigrationHelper.SetWaterMarkInRegistry(testManagementRequestContext, 21, Guid.Empty, out waterMark1, waterMarkArtifactType, waterMarkArtifactId);
          TCMServiceDataMigrationHelper.BatchTraceInfo(testManagementRequestContext, servicingContext, "TCMPropertyBag", waterMark1, ref batchedWaterMark, ref tracePointCount);
        }
        else
        {
          TCMServiceDataMigrationHelper.SetWaterMarkInRegistry(testManagementRequestContext, -1, Guid.Empty, out waterMark1);
          batchedWaterMark = batchedWaterMark + ";" + waterMark1 + (object) DateTime.UtcNow;
          string message = "TCMServiceDataMigrationHelper: syncing SyncTCMPropertyBag done. WaterMark: " + batchedWaterMark;
          TCMServiceDataMigrationHelper.LogInfo(servicingContext, testManagementRequestContext, message, nameof (TCMServiceDataMigrationHelper));
        }
      }
      while (!flag);
      TCMServiceDataMigrationHelper.LogInfo(servicingContext, testManagementRequestContext, "TCMServiceDataMigrationHelper: exit SyncTCMPropertyBag", nameof (TCMServiceDataMigrationHelper));
    }

    internal static bool TryParseWaterMark(
      TestManagementRequestContext testManagementRequestContext,
      IServicingContext servicingContext,
      string waterMark,
      out TCMServiceDataMigrationTables step,
      out Guid waterMarkProjectId,
      out int waterMarkId,
      out int waterMarkId2,
      out int waterMarkId3)
    {
      waterMarkProjectId = Guid.Empty;
      waterMarkId = 0;
      waterMarkId2 = 0;
      waterMarkId3 = 0;
      step = TCMServiceDataMigrationTables.CodeCoverage;
      if (!string.IsNullOrWhiteSpace(waterMark))
      {
        string[] strArray = waterMark.Split(',');
        if (strArray.Length < 3 || strArray.Length > 5)
        {
          TCMServiceDataMigrationHelper.LogInfo(servicingContext, testManagementRequestContext, "TCMServiceDataMigrationHelper: incorrect length of waterMark", nameof (TCMServiceDataMigrationHelper));
          return false;
        }
        if (!Enum.TryParse<TCMServiceDataMigrationTables>(strArray[0], out step))
        {
          TCMServiceDataMigrationHelper.LogInfo(servicingContext, testManagementRequestContext, "TCMServiceDataMigrationHelper: incorrect format of waterMark step", nameof (TCMServiceDataMigrationHelper));
          return false;
        }
        if (!Enum.IsDefined(typeof (TCMServiceDataMigrationTables), (object) step))
        {
          TCMServiceDataMigrationHelper.LogInfo(servicingContext, testManagementRequestContext, "TCMServiceDataMigrationHelper: incorrect value of waterMark step", nameof (TCMServiceDataMigrationHelper));
          return false;
        }
        if (!Guid.TryParse(strArray[1], out waterMarkProjectId))
        {
          TCMServiceDataMigrationHelper.LogInfo(servicingContext, testManagementRequestContext, "TCMServiceDataMigrationHelper: incorrect format of waterMarkProjectId", nameof (TCMServiceDataMigrationHelper));
          return false;
        }
        if (!int.TryParse(strArray[2], out waterMarkId))
        {
          TCMServiceDataMigrationHelper.LogInfo(servicingContext, testManagementRequestContext, "TCMServiceDataMigrationHelper: incorrect format of waterMarkId", nameof (TCMServiceDataMigrationHelper));
          return false;
        }
        if (TCMServiceDataMigrationHelper.IsValidWaterMarkId2ForStep(step) && !int.TryParse(strArray[3], out waterMarkId2))
        {
          TCMServiceDataMigrationHelper.LogInfo(servicingContext, testManagementRequestContext, "TCMServiceDataMigrationHelper: incorrect format of waterMarkId2", nameof (TCMServiceDataMigrationHelper));
          return false;
        }
        if (TCMServiceDataMigrationHelper.IsValidWaterMarkId3ForStep(step) && !int.TryParse(strArray[4], out waterMarkId3))
        {
          TCMServiceDataMigrationHelper.LogInfo(servicingContext, testManagementRequestContext, "TCMServiceDataMigrationHelper: incorrect format of waterMarkId3", nameof (TCMServiceDataMigrationHelper));
          return false;
        }
        string message = string.Format("TCMServiceDataMigrationHelper: {0},{1},{2},{3}", (object) step, (object) waterMarkProjectId, (object) waterMarkId, (object) waterMarkId2);
        TCMServiceDataMigrationHelper.LogInfo(servicingContext, testManagementRequestContext, message, nameof (TCMServiceDataMigrationHelper));
        return true;
      }
      TCMServiceDataMigrationHelper.LogInfo(servicingContext, testManagementRequestContext, "TCMServiceDataMigrationHelper: waterMark is empty", nameof (TCMServiceDataMigrationHelper));
      return true;
    }

    internal static bool TryParsePointsResultsWaterMark(
      TestManagementRequestContext testManagementRequestContext,
      IServicingContext servicingContext,
      string registryQuery,
      out int waterMarkId,
      out int waterMarkId2)
    {
      waterMarkId = 0;
      waterMarkId2 = 0;
      string str = testManagementRequestContext.RequestContext.GetService<IVssRegistryService>().GetValue(testManagementRequestContext.RequestContext, (RegistryQuery) registryQuery, (string) null);
      if (!string.IsNullOrWhiteSpace(str))
      {
        string[] strArray = str.Split(',');
        if (!int.TryParse(strArray[0], out waterMarkId))
        {
          TCMServiceDataMigrationHelper.LogInfo(servicingContext, testManagementRequestContext, "TCMServiceDataMigrationHelper: incorrect format of waterMarkId", "PointsResultsMigration");
          return false;
        }
        if (!int.TryParse(strArray[1], out waterMarkId2))
        {
          TCMServiceDataMigrationHelper.LogInfo(servicingContext, testManagementRequestContext, "TCMServiceDataMigrationHelper: incorrect format of waterMarkId2", "PointsResultsMigration");
          return false;
        }
        string message = string.Format("TCMServiceDataMigrationHelper: {0},{1}", (object) waterMarkId, (object) waterMarkId2);
        TCMServiceDataMigrationHelper.LogInfo(servicingContext, testManagementRequestContext, message, "PointsResultsMigration");
        return true;
      }
      TCMServiceDataMigrationHelper.LogInfo(servicingContext, testManagementRequestContext, "TCMServiceDataMigrationHelper: waterMark is empty", "PointsResultsMigration");
      return true;
    }

    internal static bool TryParsePointHistoryWaterMark(
      TestManagementRequestContext testManagementRequestContext,
      IServicingContext servicingContext,
      string registryQuery,
      out int planId,
      out int pointId,
      out int changeNumber)
    {
      planId = 0;
      pointId = int.MinValue;
      changeNumber = 0;
      string str = testManagementRequestContext.RequestContext.GetService<IVssRegistryService>().GetValue(testManagementRequestContext.RequestContext, (RegistryQuery) registryQuery, (string) null);
      if (!string.IsNullOrWhiteSpace(str))
      {
        string[] source = str.Split(',');
        if (((IEnumerable<string>) source).Count<string>() < 3)
        {
          TCMServiceDataMigrationHelper.LogInfo(servicingContext, testManagementRequestContext, "TCMServiceDataMigrationHelper: incorrect format of waterMark not enough columns", "PointHistoryOutcomeBackfill");
          return false;
        }
        if (!int.TryParse(source[0], out planId))
        {
          TCMServiceDataMigrationHelper.LogInfo(servicingContext, testManagementRequestContext, "TCMServiceDataMigrationHelper: incorrect format of planId", "PointHistoryOutcomeBackfill");
          return false;
        }
        if (!int.TryParse(source[1], out pointId))
        {
          TCMServiceDataMigrationHelper.LogInfo(servicingContext, testManagementRequestContext, "TCMServiceDataMigrationHelper: incorrect format of ponitId", "PointHistoryOutcomeBackfill");
          return false;
        }
        if (!int.TryParse(source[2], out changeNumber))
        {
          TCMServiceDataMigrationHelper.LogInfo(servicingContext, testManagementRequestContext, "TCMServiceDataMigrationHelper: incorrect format of changeNumber", "PointHistoryOutcomeBackfill");
          return false;
        }
        string message = string.Format("TCMServiceDataMigrationHelper: {0},{1},{2}", (object) planId, (object) pointId, (object) changeNumber);
        TCMServiceDataMigrationHelper.LogInfo(servicingContext, testManagementRequestContext, message, "PointHistoryOutcomeBackfill");
        return true;
      }
      TCMServiceDataMigrationHelper.LogInfo(servicingContext, testManagementRequestContext, "TCMServiceDataMigrationHelper: waterMark is empty", "PointHistoryOutcomeBackfill");
      return true;
    }

    public static bool IsMigrationApplicable(
      TestManagementRequestContext testManagementRequestContext)
    {
      List<TestRun2> testRun2List;
      using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(testManagementRequestContext))
        testRun2List = managementDatabase.FetchTestRuns(0, 0, 1);
      List<CodeCoverageSummary2> coverageSummary2List;
      using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(testManagementRequestContext))
        coverageSummary2List = managementDatabase.FetchCodeCoverageSummary(0, 0, 1);
      return (testRun2List.Count != 0 || coverageSummary2List.Count != 0) && testManagementRequestContext.RequestContext.GetService<IVssRegistryService>().GetValue<int>(testManagementRequestContext.RequestContext, (RegistryQuery) "/Service/TestManagement/TCMServiceDataMigration/MigrationStatus", 0) != 2;
    }

    private static bool IsMigrationApplicable_PointsPresent(
      TestManagementRequestContext testManagementRequestContext)
    {
      List<PointsResults2> pointsResults2List;
      using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(testManagementRequestContext))
        pointsResults2List = managementDatabase.FetchPointResults(0, 0, 1);
      return pointsResults2List.Count != 0;
    }

    private static int GetWaterMarkDataspaceId(
      TestManagementRequestContext testManagementRequestContext,
      Guid waterMarkProjectId)
    {
      int waterMarkDataspaceId = 0;
      if (waterMarkProjectId != Guid.Empty)
      {
        using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(testManagementRequestContext))
          waterMarkDataspaceId = managementDatabase.GetDataspaceIdWithLazyInitialization(waterMarkProjectId);
      }
      return waterMarkDataspaceId;
    }

    private static void SetWaterMarkInRegistry(
      TestManagementRequestContext targetRequestContext,
      int step,
      Guid waterMarkProjectId,
      out string waterMark,
      int waterMarkId = 0,
      int waterMarkId2 = 0,
      int waterMarkId3 = 0)
    {
      IVssRegistryService service = targetRequestContext.RequestContext.GetService<IVssRegistryService>();
      waterMark = string.Format("{0},{1},{2},{3},{4}", (object) step, (object) waterMarkProjectId, (object) waterMarkId, (object) waterMarkId2, (object) waterMarkId3);
      service.SetValue<string>(targetRequestContext.RequestContext, "/Service/TestManagement/TCMServiceDataMigration/WaterMark", waterMark);
    }

    private static int GetBatchSize(
      TestManagementRequestContext targetRequestContext,
      TCMServiceDataMigrationTables step)
    {
      IVssRegistryService service = targetRequestContext.RequestContext.GetService<IVssRegistryService>();
      int defaultValue = service.GetValue<int>(targetRequestContext.RequestContext, (RegistryQuery) "/Service/TestManagement/TCMServiceDataMigration/BatchSize", 10000);
      if (step == TCMServiceDataMigrationTables.TestRun2 || step == TCMServiceDataMigrationTables.TestRunContext)
        defaultValue = 1000;
      string query = string.Format("/Service/TestManagement/TCMServiceDataMigration/StepBatchSize/{0}", (object) (int) step);
      return service.GetValue<int>(targetRequestContext.RequestContext, (RegistryQuery) query, defaultValue);
    }

    private static void ShouldCancelJob(TestManagementRequestContext targetRequestContext)
    {
      if (targetRequestContext.IsFeatureEnabled("TestManagement.Server.DisableDataMigrationJob"))
        throw new Exception("Job forcefully stopped");
    }

    private static bool IsValidWaterMarkId2ForStep(TCMServiceDataMigrationTables step) => step == TCMServiceDataMigrationTables.TestResult || step == TCMServiceDataMigrationTables.TestMessageLogEntry || step == TCMServiceDataMigrationTables.TestResultsEx || step == TCMServiceDataMigrationTables.TestResultReset || step == TCMServiceDataMigrationTables.TestActionResult || step == TCMServiceDataMigrationTables.TestRunEx || step == TCMServiceDataMigrationTables.TestParameter;

    private static bool IsValidWaterMarkId3ForStep(TCMServiceDataMigrationTables step) => step == TCMServiceDataMigrationTables.TestResultsEx || step == TCMServiceDataMigrationTables.TestResultReset || step == TCMServiceDataMigrationTables.TestActionResult || step == TCMServiceDataMigrationTables.TestParameter;

    private static void SetMigrationStatusRegistrySetting(
      TestManagementRequestContext targetRequestContext,
      TCMServiceDataMigrationStatus migrationStatus)
    {
      targetRequestContext.RequestContext.GetService<IVssRegistryService>().SetValue<int>(targetRequestContext.RequestContext, "/Service/TestManagement/TCMServiceDataMigration/MigrationStatus", (int) migrationStatus);
    }

    private static void BatchTraceInfo(
      TestManagementRequestContext testManagementRequestContext,
      IServicingContext servicingContext,
      string tableName,
      string waterMark,
      ref string batchedWaterMark,
      ref int tracePointCount)
    {
      batchedWaterMark = batchedWaterMark + ";" + waterMark + (object) DateTime.UtcNow;
      ++tracePointCount;
      if (tracePointCount != TCMServiceDataMigrationHelper.BatchTracePointSize)
        return;
      string message = "TCMServiceDataMigrationHelper: syncing " + tableName + ". WaterMark: " + batchedWaterMark;
      TCMServiceDataMigrationHelper.LogInfo(servicingContext, testManagementRequestContext, message, nameof (TCMServiceDataMigrationHelper));
      tracePointCount = 0;
      batchedWaterMark = string.Empty;
    }

    private static void TraceInfo(
      TestManagementRequestContext testManagementRequestContext,
      IServicingContext servicingContext,
      string tableName,
      string waterMark)
    {
      string message = "TCMServiceDataMigrationHelper2: syncing " + tableName + ". WaterMark: " + waterMark;
      TCMServiceDataMigrationHelper.LogInfo(servicingContext, testManagementRequestContext, message, "TCMServiceDataMigrationHelper2");
    }

    private static void LogInfo(
      IServicingContext servicingContext,
      TestManagementRequestContext targetRequestContext,
      string message,
      string layer)
    {
      if (servicingContext != null)
        servicingContext.LogInfo(message);
      else
        targetRequestContext.TraceInfo(layer, message);
    }
  }
}
