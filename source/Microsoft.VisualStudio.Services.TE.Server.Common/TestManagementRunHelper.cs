// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestExecution.Server.TestManagementRunHelper
// Assembly: Microsoft.VisualStudio.Services.TE.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2BC2680F-A5FB-41BE-A4CF-F78BF7AC3E02
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.TE.Server.Common.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Test.WebApi;
using Microsoft.TeamFoundation.TestManagement.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Text;

namespace Microsoft.TeamFoundation.TestExecution.Server
{
  [CLSCompliant(false)]
  public class TestManagementRunHelper : ITestManagementRunHelper
  {
    private ITestExecutionServiceIdentityHelper _identityHelper;
    private ITcmLogger _tcmLogger;

    public TestManagementRunHelper(ITestExecutionServiceIdentityHelper identityHelper = null) => this._identityHelper = identityHelper;

    public Microsoft.TeamFoundation.TestManagement.WebApi.TestRun GetTestRun(
      TestExecutionRequestContext context,
      int testRunId,
      TeamProjectReference teamProject = null,
      bool includeTestRunDetails = true)
    {
      if (teamProject == null)
        teamProject = TestRunPropertiesService.GetProjectReference(context, testRunId);
      for (int index = 0; index < 2; ++index)
      {
        DtaLogger logger = TestManagementRunHelper.GetLogger(context);
        try
        {
          return string.IsNullOrWhiteSpace(teamProject.Name) ? context.TestResultsHttpClient.GetTestRunByIdAsync(teamProject.Id, testRunId, new bool?(includeTestRunDetails)).Result : context.TestResultsHttpClient.GetTestRunByIdAsync(teamProject.Name, testRunId, new bool?(includeTestRunDetails)).Result;
        }
        catch (AggregateException ex)
        {
          ex.Handle((Func<Exception, bool>) (exception =>
          {
            switch (exception)
            {
              case Microsoft.TeamFoundation.TestManagement.WebApi.TestObjectNotFoundException _:
                logger.Error(6200511, "Test run with id '{0}' not found. Project : {1} : {2}", (object) testRunId, (object) teamProject.Name, (object) ex.Message);
                throw new TestExecutionObjectNotFoundException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, TestExecutionServiceResources.InvalidRunId, (object) testRunId));
              case HttpRequestException _:
              case TimeoutException _:
              case VssServiceException _:
                logger.Error(6200511, string.Format("Failed to get Test run with id '{0}' Retrying.. Exception. {1}", (object) testRunId, (object) exception));
                return true;
              default:
                logger.Error(6200512, string.Format("Failed to get Test run with id '{0}' No Retry left Exception. {1}", (object) testRunId, (object) exception));
                return false;
            }
          }));
        }
        catch (Microsoft.TeamFoundation.TestManagement.WebApi.TestObjectNotFoundException ex)
        {
          logger.Error(6200513, "Test run with id '{0}' not found. Project : {1} : {2}", (object) testRunId, (object) teamProject.Name, (object) ex.Message);
          throw new TestExecutionObjectNotFoundException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, TestExecutionServiceResources.InvalidRunId, (object) testRunId));
        }
        catch (Exception ex)
        {
          switch (ex)
          {
            case HttpRequestException _:
            case TimeoutException _:
            case VssServiceException _:
              logger.Error(6200514, string.Format("Failed to get Test run with id '{0}' Retrying.. Exception. {1}", (object) testRunId, (object) ex));
              continue;
            default:
              throw;
          }
        }
      }
      throw new TestExecutionServiceException(TestExecutionServiceResources.QueryRunDetailsFailed);
    }

    public Microsoft.TeamFoundation.TestManagement.WebApi.TestRun ValidateTestRunIsInProgress(
      TestExecutionRequestContext requestContext,
      int testRunId,
      TeamProjectReference teamProject = null,
      bool includeTestRunDetails = true)
    {
      Microsoft.TeamFoundation.TestManagement.WebApi.TestRun testRun = this.GetTestRun(requestContext, testRunId, teamProject, includeTestRunDetails);
      if (string.Equals("InProgress", testRun.State, StringComparison.OrdinalIgnoreCase))
        return testRun;
      TeamProjectReference projectReference = TestRunPropertiesService.GetProjectReference(requestContext, testRunId);
      string message = string.Format((IFormatProvider) CultureInfo.CurrentCulture, TestExecutionServiceResources.RunNotInProgress, (object) testRun.State);
      this.TcmLogger.AddLogToTcmRun(requestContext, testRunId, projectReference, string.Format((IFormatProvider) CultureInfo.CurrentCulture, TestExecutionServiceResources.ErrorOccured, (object) message));
      throw new TestExecutionServiceInvalidOperationException(message, (Exception) null);
    }

    public bool IsTestRunCompleted(TestExecutionRequestContext requestContext, int testRunId)
    {
      Microsoft.TeamFoundation.TestManagement.WebApi.TestRun testRun = this.GetTestRun(requestContext, testRunId, (TeamProjectReference) null, false);
      return string.Equals("Completed", testRun.State, StringComparison.OrdinalIgnoreCase) || string.Equals("Aborted", testRun.State, StringComparison.OrdinalIgnoreCase);
    }

    public void PopulateTestRunInformation(
      TestExecutionRequestContext context,
      TestRunInformation testRunInfo)
    {
      TestRunProperties testRunProperties = TestRunPropertiesService.GetTestRunProperties(context, testRunInfo.TcmRun.Id);
      testRunInfo.ProjectReference = TestRunPropertiesService.GetProjectReference(context, testRunInfo.TcmRun.Id);
      DtaLogger logger = TestManagementRunHelper.GetLogger(context);
      testRunInfo.BuildConfigurationId = testRunProperties.BuildId;
      testRunInfo.BuildPlatform = testRunProperties.BuildPlatform;
      testRunInfo.BuildFlavor = testRunProperties.BuildFlavor;
      testRunInfo.TestPlanId = testRunProperties.TestPlanId;
      testRunInfo.IsTestPlanRun = testRunProperties.TestPlanId > 0;
      testRunInfo.TestDropPath = testRunProperties.DropLocation;
      string sourceFilter = TestRunPropertiesService.GetSourceFilter(context, testRunProperties.TestRunId);
      string testCaseFilter = TestRunPropertiesService.GetTestCaseFilter(context, testRunProperties.TestRunId);
      if (string.IsNullOrWhiteSpace(sourceFilter) && string.IsNullOrWhiteSpace(testCaseFilter))
      {
        logger.Error(6200512, "Source Filter or Test Filter not found. Run Id : {0}", (object) testRunProperties.TestRunId);
        throw new TestExecutionServiceException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, TestExecutionServiceResources.FilterValuesNotPresent, (object) testRunProperties.TestRunId));
      }
      Microsoft.TeamFoundation.TestManagement.WebApi.RunFilter runFilter = new Microsoft.TeamFoundation.TestManagement.WebApi.RunFilter()
      {
        SourceFilter = sourceFilter,
        TestCaseFilter = testCaseFilter
      };
      testRunInfo.Filters = new Microsoft.TeamFoundation.Test.WebApi.RunFilter()
      {
        SourceFilter = runFilter.SourceFilter,
        TestCaseFilter = runFilter.TestCaseFilter
      };
    }

    public void AbortTestRun(
      TestExecutionRequestContext context,
      int testRunId,
      string reason,
      Guid? abortedBy = null,
      bool timedOut = false)
    {
      DtaLogger logger = TestManagementRunHelper.GetLogger(context);
      logger.Info(6200513, "Aborting tcm run .TcmRunId: {0}", (object) testRunId);
      Microsoft.TeamFoundation.TestManagement.WebApi.TestRun testRun = this.GetTestRun(context, testRunId, (TeamProjectReference) null, false);
      TeamProjectReference projectReference = TestRunPropertiesService.GetProjectReference(context, testRunId);
      if (string.Equals("InProgress", testRun.State, StringComparison.OrdinalIgnoreCase) || string.Equals("NotStarted", testRun.State, StringComparison.OrdinalIgnoreCase))
      {
        using (DtaSliceDatabase component = context.RequestContext.CreateComponent<DtaSliceDatabase>())
          component.AbortSlicesByRunId(testRunId);
      }
      this.LogSliceMessages(context, testRunId, projectReference);
      this.AbortTcmRun(context, testRunId, reason, abortedBy, timedOut);
      logger.Info(6200514, "Tcm run aborted .TcmRunId: {0}", (object) testRunId);
    }

    public void AbortTcmRun(
      TestExecutionRequestContext context,
      int testRunId,
      string reason,
      Guid? abortedBy = null,
      bool timedOut = false)
    {
      this.ChangeRunState(context, testRunId, new TestManagementRunHelper.ChangeRunPropertyWithoutRetry(this.AbortTcmRunWithoutRetry), (Func<RunUpdateModel>) null, reason, abortedBy, timedOut);
    }

    public void UpdateTestRunStateToCompleted(
      TestExecutionRequestContext requestContext,
      WorkFlowJobDetails workFlowJobDetails)
    {
      DtaLogger logger = TestManagementRunHelper.GetLogger(requestContext);
      logger.Info(6200515, "Updating tcm run state to Completed .TcmRunId: {0}", (object) workFlowJobDetails.TestRunId);
      TestRunSubstate completedTestRun = this.GetSubstateForCompletedTestRun(requestContext, workFlowJobDetails.TestRunId);
      this.SetRunState(requestContext, workFlowJobDetails.TestRunId, TestRunState.Completed, completedTestRun);
      logger.Error(6200516, "Updated tcm run state to Completed, substate to {0} .TcmRunId: {1}", (object) completedTestRun.ToString(), (object) workFlowJobDetails.TestRunId);
    }

    public void UpdateTestRunStateToInProgress(
      TestExecutionRequestContext requestContext,
      WorkflowJobData workFlowJobData)
    {
      Microsoft.TeamFoundation.TestManagement.WebApi.TestRun testRun = this.GetTestRun(requestContext, workFlowJobData.TestRunId, (TeamProjectReference) null, false);
      DtaLogger logger = TestManagementRunHelper.GetLogger(requestContext);
      if (!string.Equals("Unspecified", testRun.State, StringComparison.OrdinalIgnoreCase) && !string.Equals("NotStarted", testRun.State, StringComparison.OrdinalIgnoreCase))
        return;
      logger.Error(6200517, "Updating tcm run state to in progress.TcmRunId: {0}", (object) workFlowJobData.TestRunId);
      this.SetRunState(requestContext, workFlowJobData.TestRunId, TestRunState.InProgress);
    }

    public void UpdateTestRunSubstate(
      TestExecutionRequestContext context,
      int testRunId,
      Phase phase)
    {
      TestRunSubstate runSubstate = this.ConvertWorkflowPhaseToTestRunSubstate(context, testRunId, phase);
      this.ChangeRunState(context, testRunId, new TestManagementRunHelper.ChangeRunPropertyWithoutRetry(this.UpdateRunPropertyWithoutRetry), (Func<RunUpdateModel>) (() => new RunUpdateModel(substate: runSubstate)), (string) null);
    }

    public void UpdateDtlEnvironmentUrl(
      TestExecutionRequestContext context,
      int testRunId,
      string dtlEnvironmentUrl)
    {
      Microsoft.TeamFoundation.TestManagement.WebApi.ShallowReference dtlEnvUrl = new Microsoft.TeamFoundation.TestManagement.WebApi.ShallowReference()
      {
        Url = dtlEnvironmentUrl
      };
      this.ChangeRunState(context, testRunId, new TestManagementRunHelper.ChangeRunPropertyWithoutRetry(this.UpdateRunPropertyWithoutRetry), (Func<RunUpdateModel>) (() => new RunUpdateModel(dtlEnvironment: new Microsoft.TeamFoundation.TestManagement.WebApi.ShallowReference()
      {
        Url = dtlEnvUrl.Url
      })), (string) null);
    }

    public Microsoft.TeamFoundation.TestManagement.WebApi.TestRun UpdateTestRun(
      TestExecutionRequestContext context,
      int testRunId,
      RunUpdateModel runUpdateModel,
      TeamProjectReference teamProject)
    {
      return string.IsNullOrWhiteSpace(teamProject.Name) ? context.TestResultsHttpClient.UpdateTestRunAsync(runUpdateModel, teamProject.Id, testRunId).Result : context.TestResultsHttpClient.UpdateTestRunAsync(runUpdateModel, teamProject.Name, testRunId).Result;
    }

    public Microsoft.TeamFoundation.TestManagement.WebApi.TestSettings GetTestSettings(
      TestExecutionRequestContext requestContext,
      Microsoft.TeamFoundation.TestManagement.WebApi.TestRun testRun)
    {
      TeamProjectReference projectReference = TestRunPropertiesService.GetProjectReference(requestContext, testRun.Id);
      int result;
      if (testRun.TestSettings != null && int.TryParse(testRun.TestSettings.Id, out result))
      {
        Microsoft.TeamFoundation.TestManagement.WebApi.TestSettings testSettings = this.GetTestSettings(requestContext, result, projectReference);
        if (!string.IsNullOrWhiteSpace(testSettings.TestSettingsContent))
          return testSettings;
      }
      return (Microsoft.TeamFoundation.TestManagement.WebApi.TestSettings) null;
    }

    private void SetRunState(
      TestExecutionRequestContext context,
      int testRunId,
      TestRunState testRunState,
      TestRunSubstate testRunSubstate = TestRunSubstate.None)
    {
      Func<RunUpdateModel> updateRunProperty = (Func<RunUpdateModel>) (() =>
      {
        string state = testRunState.ToString();
        TestRunSubstate testRunSubstate1 = testRunSubstate;
        bool? deleteUnexecutedResults = new bool?();
        int substate = (int) testRunSubstate1;
        return new RunUpdateModel(state: state, deleteUnexecutedResults: deleteUnexecutedResults, substate: (TestRunSubstate) substate);
      });
      this.ChangeRunState(context, testRunId, new TestManagementRunHelper.ChangeRunPropertyWithoutRetry(this.UpdateRunPropertyWithoutRetry), updateRunProperty, (string) null);
    }

    private void ChangeRunState(
      TestExecutionRequestContext context,
      int testRunId,
      TestManagementRunHelper.ChangeRunPropertyWithoutRetry changeRunPropertyWithoutRetry,
      Func<RunUpdateModel> updateRunProperty,
      string reason = "",
      Guid? abortedBy = null,
      bool timedOut = false)
    {
      TeamProjectReference projectReference = TestRunPropertiesService.GetProjectReference(context, testRunId);
      DtaLogger logger = TestManagementRunHelper.GetLogger(context);
      for (int count = 0; count <= 2; count++)
      {
        try
        {
          changeRunPropertyWithoutRetry(context, testRunId, projectReference, updateRunProperty, reason, abortedBy, timedOut);
          break;
        }
        catch (AggregateException ex)
        {
          Func<Exception, bool> predicate = (Func<Exception, bool>) (exception =>
          {
            if (exception is Microsoft.TeamFoundation.TestManagement.WebApi.TestObjectUpdatedException)
            {
              TestManagementRunHelper.CheckForRetry(testRunId, exception as Microsoft.TeamFoundation.TestManagement.WebApi.TestObjectUpdatedException, logger, count);
              return true;
            }
            logger.Error(6200521, string.Format("Updating the test run {0} failed : InnerException: {1}", (object) testRunId, (object) exception));
            return false;
          });
          ex.Handle(predicate);
        }
        catch (Microsoft.TeamFoundation.TestManagement.WebApi.TestObjectUpdatedException ex)
        {
          TestManagementRunHelper.CheckForRetry(testRunId, ex, logger, count);
        }
      }
    }

    private static void CheckForRetry(
      int testRunId,
      Microsoft.TeamFoundation.TestManagement.WebApi.TestObjectUpdatedException exception,
      DtaLogger logger,
      int count)
    {
      if (exception == null)
      {
        logger.Error(6200522, string.Format("Updating the test run {0} failed. Invalid Exception", (object) testRunId));
        throw new TestExecutionServiceException(TestExecutionServiceResources.UpdateFailed);
      }
      if (count >= 2)
      {
        logger.Error(6200519, string.Format("Retry exhausted. Updating the test run {0} failed : {1}", (object) testRunId, (object) exception));
        throw new TestExecutionServiceException(TestExecutionServiceResources.UpdateFailed);
      }
      logger.Warning(6200518, string.Format("Updating the test run {0} failed {1}. Retrying", (object) testRunId, (object) exception.Message));
    }

    private void UpdateRunPropertyWithoutRetry(
      TestExecutionRequestContext context,
      int testRunId,
      TeamProjectReference teamProject,
      Func<RunUpdateModel> updateRunProperty,
      string reason = "",
      Guid? changedBy = null,
      bool timedOut = false)
    {
      RunUpdateModel runUpdateModel = new RunUpdateModel();
      if (updateRunProperty != null)
        runUpdateModel = updateRunProperty();
      this.UpdateTestRun(context, testRunId, runUpdateModel, teamProject);
    }

    private void AbortTcmRunWithoutRetry(
      TestExecutionRequestContext context,
      int testRunId,
      TeamProjectReference teamProject,
      Func<RunUpdateModel> updateRunProperty,
      string reason = "",
      Guid? abortedBy = null,
      bool timedOut = false)
    {
      RunUpdateModel runUpdateModel = new RunUpdateModel(state: "Aborted", substate: !timedOut ? (this.IsCancelledByUser(context, abortedBy) ? TestRunSubstate.CanceledByUser : TestRunSubstate.AbortedBySystem) : TestRunSubstate.TimedOut, errorMessage: reason);
      this.UpdateTestRun(context, testRunId, runUpdateModel, teamProject);
    }

    private bool IsCancelledByUser(TestExecutionRequestContext context, Guid? abortedBy) => this.IdentityHelper.IsUserIdentity(context, abortedBy.GetValueOrDefault(Guid.Empty));

    private Microsoft.TeamFoundation.TestManagement.WebApi.TestSettings GetTestSettings(
      TestExecutionRequestContext requestContext,
      int testSettingsId,
      TeamProjectReference teamProject)
    {
      return string.IsNullOrWhiteSpace(teamProject.Name) ? requestContext.TestResultsHttpClient.GetTestSettingsByIdAsync(teamProject.Id, testSettingsId).Result : requestContext.TestResultsHttpClient.GetTestSettingsByIdAsync(teamProject.Name, testSettingsId).Result;
    }

    private TestRunSubstate ConvertWorkflowPhaseToTestRunSubstate(
      TestExecutionRequestContext context,
      int testRunId,
      Phase phase)
    {
      switch (phase)
      {
        case Phase.EnvironmentCreationPhase:
          return TestRunSubstate.CreatingEnvironment;
        case Phase.DiscoveryPhase:
        case Phase.SlicingPhase:
        case Phase.ExecutionPhase:
          return TestRunSubstate.RunningTests;
        default:
          return TestRunSubstate.None;
      }
    }

    private TestRunSubstate GetSubstateForCompletedTestRun(
      TestExecutionRequestContext context,
      int testRunId)
    {
      Microsoft.TeamFoundation.TestManagement.WebApi.TestRun testRun = this.GetTestRun(context, testRunId, (TeamProjectReference) null, false);
      return testRun.TotalTests != testRun.PassedTests + testRun.NotApplicableTests ? TestRunSubstate.PendingAnalysis : TestRunSubstate.Analyzed;
    }

    private void LogSliceMessages(
      TestExecutionRequestContext context,
      int testRunId,
      TeamProjectReference project)
    {
      IEnumerable<TestAutomationRunSlice> automationRunSlices = context.RequestContext.GetService<ITestExecutionService>().QuerySlicesByTestRunId(context, testRunId);
      StringBuilder stringBuilder = new StringBuilder();
      foreach (TestAutomationRunSlice automationRunSlice in automationRunSlices)
      {
        if (automationRunSlice.Messages != null)
        {
          foreach (Message message in automationRunSlice.Messages)
            stringBuilder.AppendLine(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0} : {1}", (object) message.Type, (object) message.Data));
        }
      }
      string message1 = stringBuilder.ToString();
      if (string.IsNullOrEmpty(message1))
        return;
      this.TcmLogger.AddLogToTcmRun(context, testRunId, project, message1);
    }

    public ITcmLogger TcmLogger
    {
      get => this._tcmLogger ?? (this._tcmLogger = (ITcmLogger) new Microsoft.TeamFoundation.TestExecution.Server.TcmLogger());
      set => this._tcmLogger = value;
    }

    public ITestExecutionServiceIdentityHelper IdentityHelper
    {
      get => this._identityHelper ?? Utilities.IdentityHelper;
      set => this._identityHelper = value;
    }

    private static DtaLogger GetLogger(TestExecutionRequestContext context) => new DtaLogger(context, TestExecutionServiceConstants.TestExecutionServiceArea, TestExecutionServiceConstants.JobsLayer);

    private delegate void ChangeRunPropertyWithoutRetry(
      TestExecutionRequestContext context,
      int testRunId,
      TeamProjectReference teamProject,
      Func<RunUpdateModel> updateRunProperty,
      string reason,
      Guid? changedBy = null,
      bool timedOut = false);
  }
}
