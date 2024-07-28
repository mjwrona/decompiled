// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TeamFoundationTestManagementIterationResultService
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal class TeamFoundationTestManagementIterationResultService : 
    TeamFoundationTestManagementService,
    ITeamFoundationTestManagementIterationResultService,
    IVssFrameworkService
  {
    private const char c_stepIdentifierDelimiter = ';';

    public TeamFoundationTestManagementIterationResultService()
    {
    }

    public TeamFoundationTestManagementIterationResultService(
      TestManagementRequestContext requestContext)
      : base(requestContext)
    {
    }

    public TestResultArtifacts GetIterationDetails(
      IVssRequestContext tfsRequestContext,
      int runId,
      int resultId,
      List<TestIterationDetailsModel> iterationDetails)
    {
      return this.ExecuteAction<TestResultArtifacts>(tfsRequestContext, "TeamFoundationTestManagementIterationResultService.GetIterationDetailsFromWebApiModel", (Func<TestResultArtifacts>) (() =>
      {
        List<TestActionResult> testActionResultList = new List<TestActionResult>();
        List<TestResultParameter> testResultParameterList = new List<TestResultParameter>();
        foreach (TestIterationDetailsModel iterationDetail in iterationDetails)
        {
          if (iterationDetail != null)
          {
            testActionResultList.AddRange((IEnumerable<TestActionResult>) this.CreateTestActionResults(tfsRequestContext, runId, resultId, iterationDetail));
            testResultParameterList.AddRange((IEnumerable<TestResultParameter>) this.CreateTestParameters(runId, resultId, iterationDetail.Parameters));
          }
        }
        return new TestResultArtifacts()
        {
          ActionResults = testActionResultList,
          ParameterResults = testResultParameterList
        };
      }), 1015140, "TestResultsInsights");
    }

    public void PopulateIterationIdsAndActionPaths(
      IVssRequestContext tfsRequestContext,
      List<TestIterationDetailsModel> iterationDetails)
    {
      this.ExecuteAction(tfsRequestContext, "TeamFoundationTestManagementIterationResultService.PopulateIterationIdsAndActionPaths", (Action) (() =>
      {
        ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) iterationDetails, nameof (iterationDetails), "Test Results");
        int num = 1;
        foreach (TestIterationDetailsModel iterationDetail in iterationDetails)
        {
          if (iterationDetail != null)
          {
            iterationDetail.Id = num;
            this.GenerateActionPaths(tfsRequestContext, iterationDetail);
            ++num;
          }
        }
      }), 1015140, "TestResultsInsights");
    }

    private List<TestActionResult> CreateTestActionResults(
      IVssRequestContext requestContext,
      int runId,
      int resultId,
      TestIterationDetailsModel iterationDetail)
    {
      List<TestActionResult> testActionResults = new List<TestActionResult>();
      testActionResults.Add(this.CreateActionResultForIteration(requestContext, runId, resultId, iterationDetail));
      testActionResults.AddRange((IEnumerable<TestActionResult>) this.CreateActionResultsForTestSteps(requestContext, runId, resultId, iterationDetail.ActionResults));
      return testActionResults;
    }

    private TestActionResult CreateActionResultForIteration(
      IVssRequestContext requestContext,
      int runId,
      int resultId,
      TestIterationDetailsModel iterationDetail)
    {
      TestActionResult resultForIteration = new TestActionResult();
      resultForIteration.ActionPath = string.Empty;
      resultForIteration.TestRunId = runId;
      resultForIteration.TestResultId = resultId;
      resultForIteration.IterationId = iterationDetail.Id;
      resultForIteration.Comment = iterationDetail.Comment;
      resultForIteration.ErrorMessage = iterationDetail.ErrorMessage;
      TestActionResult testActionResult1 = resultForIteration;
      TimeSpan timeSpan;
      long num;
      if (iterationDetail.DurationInMs < 0.0)
      {
        num = 0L;
      }
      else
      {
        timeSpan = TimeSpan.FromMilliseconds(iterationDetail.DurationInMs);
        num = timeSpan.Ticks;
      }
      testActionResult1.Duration = num;
      DateTime serverStartDate;
      DateTime serverCompleteDate;
      this.GetServerDates(requestContext, iterationDetail.StartedDate, iterationDetail.CompletedDate, resultForIteration.Duration, out serverStartDate, out serverCompleteDate);
      resultForIteration.DateStarted = serverStartDate;
      resultForIteration.DateCompleted = serverCompleteDate;
      if (resultForIteration.DateCompleted < resultForIteration.DateStarted)
        throw new InvalidPropertyException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.TestIterationResultCompletedDateGreaterThanStartDate));
      if (resultForIteration.Duration == 0L)
      {
        TestActionResult testActionResult2 = resultForIteration;
        timeSpan = resultForIteration.DateCompleted - resultForIteration.DateStarted;
        long ticks = timeSpan.Ticks;
        testActionResult2.Duration = ticks;
      }
      if (!string.IsNullOrEmpty(iterationDetail.Outcome))
        resultForIteration.Outcome = this.ValidateAndGetEnumValue<Microsoft.TeamFoundation.TestManagement.Client.TestOutcome>(iterationDetail.Outcome, Microsoft.TeamFoundation.TestManagement.Client.TestOutcome.None);
      return resultForIteration;
    }

    private List<TestActionResult> CreateActionResultsForTestSteps(
      IVssRequestContext requestContext,
      int runId,
      int resultId,
      List<TestActionResultModel> actionresults)
    {
      List<TestActionResult> resultsForTestSteps = new List<TestActionResult>();
      if (actionresults == null)
        return resultsForTestSteps;
      foreach (TestActionResultModel actionresult in actionresults)
      {
        if (actionresult != null)
        {
          TestActionResult testActionResult1 = new TestActionResult();
          testActionResult1.ActionPath = actionresult.ActionPath;
          testActionResult1.IterationId = actionresult.IterationId;
          TestActionResult testActionResult2 = testActionResult1;
          SharedStepModel sharedStepModel1 = actionresult.SharedStepModel;
          int id = sharedStepModel1 != null ? sharedStepModel1.Id : 0;
          testActionResult2.SetId = id;
          TestActionResult testActionResult3 = testActionResult1;
          SharedStepModel sharedStepModel2 = actionresult.SharedStepModel;
          int revision = sharedStepModel2 != null ? sharedStepModel2.Revision : 0;
          testActionResult3.SetRevision = revision;
          testActionResult1.TestRunId = runId;
          testActionResult1.TestResultId = resultId;
          testActionResult1.Comment = actionresult.Comment;
          testActionResult1.ErrorMessage = actionresult.ErrorMessage;
          testActionResult1.Duration = actionresult.DurationInMs > 0.0 ? TimeSpan.FromMilliseconds(actionresult.DurationInMs).Ticks : 0L;
          DateTime serverStartDate;
          DateTime serverCompleteDate;
          this.GetServerDates(requestContext, actionresult.StartedDate, actionresult.CompletedDate, testActionResult1.Duration, out serverStartDate, out serverCompleteDate);
          testActionResult1.DateStarted = serverStartDate;
          testActionResult1.DateCompleted = serverCompleteDate;
          if (testActionResult1.DateCompleted < testActionResult1.DateStarted)
            throw new InvalidPropertyException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.TestActionResultCompletedDateGreaterThanStartDate));
          if (testActionResult1.Duration == 0L)
            testActionResult1.Duration = (testActionResult1.DateCompleted - testActionResult1.DateStarted).Ticks;
          if (!string.IsNullOrEmpty(actionresult.Outcome))
            testActionResult1.Outcome = this.ValidateAndGetEnumValue<Microsoft.TeamFoundation.TestManagement.Client.TestOutcome>(actionresult.Outcome, Microsoft.TeamFoundation.TestManagement.Client.TestOutcome.None);
          resultsForTestSteps.Add(testActionResult1);
        }
      }
      return resultsForTestSteps;
    }

    private void GetServerDates(
      IVssRequestContext requestContext,
      DateTime webApiStartDate,
      DateTime webApiCompleteDate,
      long duration,
      out DateTime serverStartDate,
      out DateTime serverCompleteDate)
    {
      if (!DateTime.Equals(webApiStartDate, new DateTime()) && !DateTime.Equals(webApiCompleteDate, new DateTime()))
      {
        serverStartDate = TestManagementServiceUtility.CheckAndGetDate(requestContext, webApiStartDate, "StartedDate");
        serverCompleteDate = TestManagementServiceUtility.CheckAndGetDate(requestContext, webApiCompleteDate, "CompletedDate");
      }
      else if (!DateTime.Equals(webApiStartDate, new DateTime()))
      {
        serverStartDate = TestManagementServiceUtility.CheckAndGetDate(requestContext, webApiStartDate, "StartedDate");
        serverCompleteDate = serverStartDate.AddTicks(duration);
      }
      else if (!DateTime.Equals(webApiCompleteDate, new DateTime()))
      {
        serverCompleteDate = TestManagementServiceUtility.CheckAndGetDate(requestContext, webApiCompleteDate, "CompletedDate");
        serverStartDate = serverCompleteDate.AddTicks(-duration);
      }
      else
      {
        serverStartDate = DateTime.UtcNow;
        serverCompleteDate = serverStartDate.AddTicks(duration);
      }
    }

    private List<TestResultParameter> CreateTestParameters(
      int runId,
      int testResultId,
      List<TestResultParameterModel> parameters)
    {
      List<TestResultParameter> testParameters = new List<TestResultParameter>();
      if (parameters != null)
      {
        testParameters.AddRange((IEnumerable<TestResultParameter>) this.CreateTestParametersForIteration(runId, testResultId, parameters));
        testParameters.AddRange((IEnumerable<TestResultParameter>) this.CreateTestParametersForTestSteps(runId, testResultId, parameters));
      }
      return testParameters;
    }

    private List<TestResultParameter> CreateTestParametersForIteration(
      int runId,
      int testResultId,
      List<TestResultParameterModel> parameters)
    {
      Dictionary<string, TestResultParameterModel> dict = new Dictionary<string, TestResultParameterModel>();
      parameters.ForEach((Action<TestResultParameterModel>) (param => dict[param.ParameterName] = param));
      return dict.Values.Select<TestResultParameterModel, TestResultParameter>((Func<TestResultParameterModel, TestResultParameter>) (webParam => new TestResultParameter()
      {
        TestRunId = runId,
        TestResultId = testResultId,
        IterationId = webParam.IterationId,
        ActionPath = string.Empty,
        ParameterName = webParam.ParameterName,
        DataType = webParam.DataType,
        Expected = TestResultParameterModel.ValueToBytes(webParam.Value)
      })).ToList<TestResultParameter>();
    }

    private List<TestResultParameter> CreateTestParametersForTestSteps(
      int runId,
      int testResultId,
      List<TestResultParameterModel> parameters)
    {
      return parameters.Select<TestResultParameterModel, TestResultParameter>((Func<TestResultParameterModel, TestResultParameter>) (webParam => new TestResultParameter()
      {
        TestRunId = runId,
        TestResultId = testResultId,
        IterationId = webParam.IterationId,
        ActionPath = webParam.ActionPath,
        ParameterName = webParam.ParameterName,
        DataType = webParam.DataType,
        Expected = TestResultParameterModel.ValueToBytes(webParam.Value)
      })).ToList<TestResultParameter>();
    }

    private void GenerateActionPaths(
      IVssRequestContext requestContext,
      TestIterationDetailsModel iterationDetail)
    {
      iterationDetail.ActionResults?.ForEach((Action<TestActionResultModel>) (actionResult =>
      {
        if (actionResult == null)
          return;
        actionResult.IterationId = iterationDetail.Id;
        actionResult.ActionPath = this.GenerateActionPath(requestContext, actionResult.ActionPath, actionResult.StepIdentifier);
      }));
      iterationDetail.Parameters?.ForEach((Action<TestResultParameterModel>) (param =>
      {
        if (param == null)
          return;
        param.IterationId = iterationDetail.Id;
        param.ActionPath = this.GenerateActionPath(requestContext, param.ActionPath, param.StepIdentifier);
      }));
    }

    private string GenerateActionPath(
      IVssRequestContext requestContext,
      string actionPath,
      string stepIdentifier)
    {
      if (TestIterationResultsHelper.IsValidActionPath(requestContext, actionPath))
        return actionPath;
      string[] strArray = !string.IsNullOrEmpty(stepIdentifier) ? stepIdentifier.Split(';') : throw new ArgumentException(string.Format(ServerResources.SpecifiedInvalidStepIdentifier, (object) stepIdentifier)).Expected("Test Results");
      string parentActionPath = string.Empty;
      foreach (string s in strArray)
      {
        int id = 0;
        ref int local = ref id;
        if (!int.TryParse(s, out local) || id <= 0)
          throw new ArgumentException(string.Format(ServerResources.SpecifiedInvalidStepIdentifier, (object) stepIdentifier)).Expected("Test Results");
        parentActionPath = TestIterationResultsHelper.GenerateActionPath(requestContext, id, parentActionPath);
      }
      return parentActionPath;
    }
  }
}
