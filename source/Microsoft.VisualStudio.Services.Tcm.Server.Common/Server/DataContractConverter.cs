// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.DataContractConverter
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.Client;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class DataContractConverter
  {
    protected TestManagementRequestContext m_requestContext;
    protected ITestManagementObjectHelper m_objectFactory;
    protected ITestEnvironmentsHelper m_testEnvironmentHelper;
    private StatisticsHelper m_statisticsHelper;
    private IReleaseServiceHelper m_releaseServiceHelper;
    private ITeamFoundationTestManagementRunService m_testManagementRunService;

    public DataContractConverter(TestManagementRequestContext requestContext)
    {
      this.m_requestContext = requestContext;
      this.m_testManagementRunService = this.m_requestContext.RequestContext.GetService<ITeamFoundationTestManagementRunService>();
      this.m_statisticsHelper = new StatisticsHelper(requestContext);
      this.m_releaseServiceHelper = (IReleaseServiceHelper) new ReleaseServiceHelper();
    }

    public DataContractConverter(
      TestManagementRequestContext requestContext,
      ITestEnvironmentsHelper testEnvironmentsHelper,
      StatisticsHelper statisticsHelper,
      IReleaseServiceHelper releaseServiceHelper)
    {
      this.m_requestContext = requestContext;
      this.m_testManagementRunService = this.m_requestContext.RequestContext.GetService<ITeamFoundationTestManagementRunService>();
      this.m_testEnvironmentHelper = testEnvironmentsHelper;
      this.m_statisticsHelper = statisticsHelper;
      this.m_releaseServiceHelper = releaseServiceHelper;
    }

    public void Populate(
      string projectId,
      RunCreateModel testRun,
      out TestRun run,
      out List<TestCaseResult> testCaseResults,
      out TeamProjectReference projectReference)
    {
      ArgumentUtility.CheckForNull<RunCreateModel>(testRun, nameof (testRun), "Test Results");
      projectReference = this.GetProjectReference(projectId);
      this.PopulateRunAndResultFromCreateModel(projectReference.Name, testRun, out run, out testCaseResults);
    }

    public TestRun UpdateRunDetailsFromModel(TestRun testRun, RunUpdateModel updateModel)
    {
      if (!string.IsNullOrEmpty(updateModel.State))
      {
        Microsoft.TeamFoundation.TestManagement.Client.TestRunState result;
        if (!Enum.TryParse<Microsoft.TeamFoundation.TestManagement.Client.TestRunState>(updateModel.State, true, out result))
          throw new InvalidPropertyException("State", ServerResources.InvalidPropertyMessage);
        testRun.State = (byte) result;
      }
      if (updateModel.Substate != TestRunSubstate.None)
        testRun.Substate = (byte) updateModel.Substate;
      if (!string.IsNullOrEmpty(updateModel.Name))
        testRun.Title = updateModel.Name;
      if (updateModel.Comment != null)
        testRun.Comment = updateModel.Comment;
      if (!string.IsNullOrEmpty(updateModel.CompletedDate))
        testRun.CompleteDate = TestManagementServiceUtility.CheckAndGetDate(this.m_requestContext.RequestContext, updateModel.CompletedDate, "CompletedDate");
      if (!string.IsNullOrEmpty(updateModel.StartedDate))
        testRun.StartDate = TestManagementServiceUtility.CheckAndGetDate(this.m_requestContext.RequestContext, updateModel.StartedDate, "StartedDate");
      this.AdjustStartAndCompleteDateForTestRunIfRequired(testRun);
      if (!string.IsNullOrEmpty(updateModel.DueDate))
        testRun.DueDate = TestManagementServiceUtility.CheckAndGetDate(this.m_requestContext.RequestContext, updateModel.DueDate, "DueDate");
      if (updateModel.DtlEnvironmentDetails != null)
        testRun.CsmParameters = updateModel.DtlEnvironmentDetails.CsmParameters;
      if (updateModel.DtlEnvironment != null && !string.IsNullOrEmpty(updateModel.DtlEnvironment.Url))
        testRun.DtlTestEnvironment = updateModel.DtlEnvironment;
      if (updateModel.DtlAutEnvironment != null && !string.IsNullOrEmpty(updateModel.DtlAutEnvironment.Url))
        testRun.DtlAutEnvironment = updateModel.DtlAutEnvironment;
      if (!string.IsNullOrEmpty(updateModel.Iteration))
        testRun.Iteration = updateModel.Iteration;
      if (updateModel.Build != null && !string.IsNullOrEmpty(updateModel.Build.Id))
        this.UpdateTestRunBuildUriAndNumberFromBuildId(updateModel.Build.Id, testRun);
      if (!string.IsNullOrEmpty(updateModel.Controller))
      {
        if (!testRun.IsAutomated)
          throw new InvalidPropertyException("Controller", ServerResources.ControllerSetForNonAutomatedRun);
        testRun.Controller = updateModel.Controller;
      }
      int num = this.ValidateTestSettingsFromUpdateModel(updateModel);
      if (num != 0)
        testRun.PublicTestSettingsId = num;
      if (!string.IsNullOrEmpty(updateModel.TestEnvironmentId))
      {
        Guid result;
        if (!Guid.TryParse(updateModel.TestEnvironmentId, out result))
          throw new InvalidPropertyException("TestEnvironmentId", ServerResources.InvalidPropertyMessage);
        testRun.TestEnvironmentId = result;
      }
      if (!string.IsNullOrEmpty(updateModel.State))
      {
        Microsoft.TeamFoundation.TestManagement.Client.TestRunState result;
        if (!Enum.TryParse<Microsoft.TeamFoundation.TestManagement.Client.TestRunState>(updateModel.State, true, out result))
          throw new InvalidPropertyException("State", ServerResources.InvalidPropertyMessage);
        testRun.State = (byte) result;
      }
      if (!string.IsNullOrEmpty(updateModel.ErrorMessage))
        testRun.ErrorMessage = updateModel.ErrorMessage;
      if (updateModel.LogEntries != null)
        testRun.TestMessageLogEntries = updateModel.LogEntries;
      if (!string.IsNullOrEmpty(updateModel.ReleaseUri) && !string.IsNullOrEmpty(updateModel.ReleaseEnvironmentUri))
      {
        testRun.ReleaseUri = updateModel.ReleaseUri;
        testRun.ReleaseEnvironmentUri = updateModel.ReleaseEnvironmentUri;
      }
      if (!string.IsNullOrEmpty(updateModel.BuildDropLocation))
        testRun.DropLocation = updateModel.BuildDropLocation;
      testRun.SourceWorkflow = string.IsNullOrEmpty(updateModel.SourceWorkflow) ? this.GetSourceWorkflowForRun(testRun) : updateModel.SourceWorkflow;
      if (updateModel.Tags != null)
      {
        testRun.Tags = new List<TestTag>();
        testRun.Tags.AddRange((IEnumerable<TestTag>) updateModel.Tags);
      }
      return testRun;
    }

    public void PopulateRunDetails(TestRun src, TestRun dest)
    {
      dest.State = dest.State != (byte) 0 ? dest.State : src.State;
      dest.Substate = dest.Substate != (byte) 0 ? dest.Substate : src.Substate;
      dest.Title = !string.IsNullOrEmpty(dest.Title) ? dest.Title : src.Title;
      dest.Comment = dest.Comment != null ? dest.Comment : src.Comment;
      dest.CompleteDate = dest.CompleteDate != new DateTime() ? dest.CompleteDate : src.CompleteDate;
      dest.StartDate = dest.StartDate != new DateTime() ? dest.StartDate : src.StartDate;
      dest.DueDate = dest.DueDate != new DateTime() ? dest.DueDate : src.DueDate;
      dest.CsmParameters = dest.CsmParameters != null ? dest.CsmParameters : src.CsmParameters;
      dest.DtlTestEnvironment = dest.DtlTestEnvironment == null || string.IsNullOrEmpty(dest.DtlTestEnvironment.Url) ? src.DtlTestEnvironment : dest.DtlTestEnvironment;
      dest.DtlAutEnvironment = dest.DtlAutEnvironment == null || string.IsNullOrEmpty(dest.DtlAutEnvironment.Url) ? src.DtlAutEnvironment : dest.DtlAutEnvironment;
      dest.Iteration = !string.IsNullOrEmpty(dest.Iteration) ? dest.Iteration : src.Iteration;
      dest.BuildReference = dest.BuildReference != null ? dest.BuildReference : src.BuildReference;
      dest.Controller = dest.Controller != null ? dest.Controller : src.Controller;
      dest.TestSettingsId = dest.TestSettingsId != 0 ? dest.TestSettingsId : src.TestSettingsId;
      dest.TestEnvironmentId = dest.TestEnvironmentId != new Guid() ? dest.TestEnvironmentId : src.TestEnvironmentId;
      dest.ErrorMessage = !string.IsNullOrEmpty(dest.ErrorMessage) ? dest.ErrorMessage : src.ErrorMessage;
      dest.TestMessageLogEntries = dest.TestMessageLogEntries != null ? dest.TestMessageLogEntries : src.TestMessageLogEntries;
      dest.ReleaseUri = string.IsNullOrEmpty(dest.ReleaseUri) || string.IsNullOrEmpty(dest.ReleaseEnvironmentUri) ? src.ReleaseUri : dest.ReleaseUri;
      dest.ReleaseEnvironmentUri = string.IsNullOrEmpty(dest.ReleaseUri) || string.IsNullOrEmpty(dest.ReleaseEnvironmentUri) ? src.ReleaseEnvironmentUri : dest.ReleaseEnvironmentUri;
      dest.DropLocation = !string.IsNullOrEmpty(dest.DropLocation) ? dest.DropLocation : src.DropLocation;
      dest.SourceWorkflow = !string.IsNullOrEmpty(dest.SourceWorkflow) ? dest.SourceWorkflow : this.GetSourceWorkflowForRun(src);
    }

    public Microsoft.TeamFoundation.TestManagement.WebApi.ShallowReference GetRunRepresentation(
      string projectName,
      TestRun run)
    {
      RestApiResourceDetails resourceMapping = this.m_requestContext.ResourceMappings[ResourceMappingConstants.TestRun];
      return new Microsoft.TeamFoundation.TestManagement.WebApi.ShallowReference()
      {
        Id = run.TestRunId.ToString(),
        Name = run.Title,
        Url = UrlBuildHelper.GetResourceUrl(this.m_requestContext.RequestContext, resourceMapping.ServiceInstanceType, resourceMapping.Area, resourceMapping.ResourceId, (object) new
        {
          runId = run.TestRunId,
          project = projectName
        })
      };
    }

    public RunStatistic ConvertTestStatisticsToDataContract(TestRunStatistic testRunStatistic)
    {
      RunStatistic dataContract = new RunStatistic();
      dataContract.Outcome = Enum.GetName(typeof (Microsoft.TeamFoundation.TestManagement.Client.TestOutcome), (object) (Microsoft.TeamFoundation.TestManagement.Client.TestOutcome) testRunStatistic.Outcome);
      dataContract.State = Enum.GetName(typeof (TestResultState), (object) (TestResultState) testRunStatistic.State);
      dataContract.Count = testRunStatistic.Count;
      if (testRunStatistic.ResolutionState != null)
        dataContract.ResolutionState = this.ConvertResolutionStateToDataContract(testRunStatistic.ResolutionState);
      if (testRunStatistic.ResultMetadata > (byte) 0)
        dataContract.ResultMetadata = (ResultMetadata) testRunStatistic.ResultMetadata;
      return dataContract;
    }

    private Microsoft.TeamFoundation.TestManagement.WebApi.TestResolutionState ConvertResolutionStateToDataContract(
      TestResolutionState resolutionState)
    {
      return new Microsoft.TeamFoundation.TestManagement.WebApi.TestResolutionState()
      {
        Id = resolutionState.Id,
        Name = resolutionState.Name,
        project = new Microsoft.TeamFoundation.TestManagement.WebApi.ShallowReference()
        {
          Name = resolutionState.TeamProject
        }
      };
    }

    public int ValidateTestSettingsFromUpdateModel(RunUpdateModel updateModel)
    {
      if (updateModel.TestSettings == null || string.IsNullOrEmpty(updateModel.TestSettings.Id))
        return 0;
      int result;
      if (int.TryParse(updateModel.TestSettings.Id, out result) && result > 0)
        return result;
      throw new InvalidPropertyException("TestSettings.Id", ServerResources.InvalidPropertyMessage);
    }

    internal void PopulateRunAndResultFromCreateModel(
      string projectName,
      RunCreateModel createModel,
      out TestRun testRun,
      out List<TestCaseResult> testCaseResults)
    {
      this.PopulateTestRun(createModel, projectName, out testRun);
      testCaseResults = new List<TestCaseResult>();
      if (testRun.TestPlanId <= 0 || createModel.PointIds == null || ((IEnumerable<int>) createModel.PointIds).Count<int>() <= 0)
        return;
      this.PopulateTestResults(projectName, createModel, testRun.TestPlanId, 100000, out testCaseResults);
    }

    internal void PopulateRunSummaryByRunSummaryByOutcomeModel(
      IList<RunSummaryModel> runSummaryModel,
      out List<RunSummaryByOutcome> runSummaryByOutcomes)
    {
      runSummaryByOutcomes = new List<RunSummaryByOutcome>();
      foreach (RunSummaryModel runSummaryModel1 in (IEnumerable<RunSummaryModel>) runSummaryModel)
        runSummaryByOutcomes.Add(new RunSummaryByOutcome()
        {
          TestOutcome = (Microsoft.TeamFoundation.TestManagement.Client.TestOutcome) runSummaryModel1.TestOutcome,
          ResultCount = runSummaryModel1.ResultCount,
          ResultDuration = runSummaryModel1.Duration
        });
    }

    internal void PopulatePlannedResultDetailsFromCreateModel(
      string projectName,
      RunCreateModel createModel,
      int planId,
      out List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> testCaseResults)
    {
      GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(this.m_requestContext, projectName);
      testCaseResults = new List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>();
      List<TestCaseResult> testCaseResults1;
      this.PopulateTestResults(projectName, createModel, planId, 100000, out testCaseResults1);
      HashSet<int> source = new HashSet<int>(testCaseResults1.Select<TestCaseResult, int>((Func<TestCaseResult, int>) (tr => tr.TestCaseId)));
      this.m_requestContext.WorkItemFieldDataHelper.PopulateResultsFromTestCases(this.m_requestContext, projectFromName, testCaseResults1.ToArray(), source.ToArray<int>(), false);
      foreach (TestCaseResult testCaseResult in testCaseResults1)
        testCaseResults.Add(this.ConvertTestResultToWebApiModel(testCaseResult, false));
    }

    internal Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult ConvertTestResultToWebApiModel(
      TestCaseResult testCaseResult,
      bool includedetails)
    {
      Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult webApiModel = new Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult();
      webApiModel.TestCase = new Microsoft.TeamFoundation.TestManagement.WebApi.ShallowReference()
      {
        Id = testCaseResult.TestCaseId.ToString()
      };
      webApiModel.TestPoint = new Microsoft.TeamFoundation.TestManagement.WebApi.ShallowReference()
      {
        Id = testCaseResult.TestPointId.ToString()
      };
      webApiModel.Configuration = new Microsoft.TeamFoundation.TestManagement.WebApi.ShallowReference()
      {
        Id = testCaseResult.ConfigurationId.ToString(),
        Name = testCaseResult.ConfigurationName
      };
      webApiModel.Owner = new IdentityRef()
      {
        Id = testCaseResult.Owner.ToString(),
        DisplayName = testCaseResult.OwnerName
      };
      webApiModel.RunBy = new IdentityRef()
      {
        Id = testCaseResult.RunBy.ToString(),
        DisplayName = testCaseResult.RunByName
      };
      webApiModel.Id = testCaseResult.TestResultId;
      webApiModel.AutomatedTestName = testCaseResult.AutomatedTestName;
      webApiModel.AutomatedTestStorage = testCaseResult.AutomatedTestStorage;
      webApiModel.AutomatedTestId = testCaseResult.AutomatedTestId;
      webApiModel.AutomatedTestType = testCaseResult.AutomatedTestType;
      webApiModel.AutomatedTestTypeId = testCaseResult.AutomatedTestTypeId;
      webApiModel.Priority = (int) testCaseResult.Priority;
      webApiModel.TestCaseTitle = testCaseResult.TestCaseTitle;
      webApiModel.TestCaseRevision = testCaseResult.TestCaseRevision;
      if (includedetails)
      {
        webApiModel.TestRun = new Microsoft.TeamFoundation.TestManagement.WebApi.ShallowReference()
        {
          Id = testCaseResult.TestRunId.ToString()
        };
        webApiModel.TestPlan = new Microsoft.TeamFoundation.TestManagement.WebApi.ShallowReference()
        {
          Id = testCaseResult.TestPlanId.ToString()
        };
        webApiModel.LastUpdatedBy = new IdentityRef()
        {
          Id = testCaseResult.LastUpdatedBy.ToString()
        };
        webApiModel.LastUpdatedDate = testCaseResult.LastUpdated;
        webApiModel.FailureType = testCaseResult.FailureType.ToString();
        webApiModel.State = testCaseResult.State.ToString();
        webApiModel.ResolutionStateId = testCaseResult.ResolutionStateId;
        webApiModel.Outcome = testCaseResult.Outcome.ToString();
      }
      return webApiModel;
    }

    protected virtual void PopulateTestResults(
      string projectName,
      RunCreateModel createModel,
      int planId,
      int firstTestCaseResultId,
      out List<TestCaseResult> testCaseResults)
    {
      testCaseResults = new List<TestCaseResult>();
    }

    protected virtual void PopulateTestRun(
      RunCreateModel createModel,
      string projectName,
      out TestRun testRun)
    {
      testRun = new TestRun();
      ArgumentUtility.CheckStringForNullOrEmpty(createModel.Name, "Name", "Test Results");
      testRun.Title = createModel.Name;
      TestRunType result1;
      testRun.Type = !Enum.TryParse<TestRunType>(createModel.Type, true, out result1) ? (byte) 4 : (byte) result1;
      bool? automated = createModel.Automated;
      if (automated.HasValue)
      {
        TestRun testRun1 = testRun;
        automated = createModel.Automated;
        int num = automated.Value ? 1 : 0;
        testRun1.IsAutomated = num != 0;
      }
      else
        testRun.IsAutomated = false;
      testRun.TestPlanId = 0;
      if (createModel.Plan != null)
      {
        int result2;
        if (string.IsNullOrEmpty(createModel.Plan.Id) || !int.TryParse(createModel.Plan.Id, out result2) || result2 <= 0)
          throw new InvalidPropertyException("plan.Id", ServerResources.InvalidPropertyMessage);
        testRun.TestPlanId = result2;
      }
      if (createModel.PointIds != null && testRun.TestPlanId == 0)
        throw new ArgumentNullException("plan.Id").Expected("Test Results");
      if (!string.IsNullOrEmpty(createModel.Iteration))
        testRun.Iteration = createModel.Iteration;
      else if (testRun.TestPlanId == 0)
        testRun.Iteration = projectName;
      if (createModel.Build != null && !string.IsNullOrEmpty(createModel.Build.Id))
      {
        this.UpdateTestRunBuildUriAndNumberFromBuildId(createModel.Build.Id, testRun);
      }
      else
      {
        if (createModel.BuildPlatform != null && !string.IsNullOrEmpty(createModel.BuildPlatform))
          throw new ArgumentException(ServerResources.BuildIdNotSpecifiedWithPlatformFlavor).Expected("Test Results");
        if (createModel.BuildFlavor != null && !string.IsNullOrEmpty(createModel.BuildFlavor))
          throw new ArgumentException(ServerResources.BuildIdNotSpecifiedWithPlatformFlavor).Expected("Test Results");
      }
      if (createModel.BuildPlatform != null && !string.IsNullOrEmpty(createModel.BuildPlatform))
        testRun.BuildPlatform = createModel.BuildPlatform;
      if (createModel.BuildFlavor != null && !string.IsNullOrEmpty(createModel.BuildFlavor))
        testRun.BuildFlavor = createModel.BuildFlavor;
      if (createModel.BuildReference != null && createModel.BuildReference.Id > 0 && !string.IsNullOrEmpty(createModel.BuildReference.Number) && !string.IsNullOrEmpty(createModel.BuildReference.BranchName) && createModel.BuildReference.BuildDefinitionId > 0 && createModel.BuildReference.SourceVersion != null && createModel.BuildReference.Flavor != null && createModel.BuildReference.Platform != null && createModel.BuildReference.BuildSystem != null && createModel.BuildReference.RepositoryType != null && !string.IsNullOrEmpty(createModel.BuildReference.Uri))
        testRun.BuildReference = new BuildConfiguration()
        {
          BuildId = createModel.BuildReference.Id,
          BuildNumber = createModel.BuildReference.Number,
          BranchName = createModel.BuildReference.BranchName,
          BuildDefinitionId = createModel.BuildReference.BuildDefinitionId,
          SourceVersion = createModel.BuildReference.SourceVersion,
          BuildFlavor = createModel.BuildReference.Flavor,
          BuildPlatform = createModel.BuildReference.Platform,
          CreatedDate = createModel.BuildReference.CreationDate,
          BuildSystem = createModel.BuildReference.BuildSystem,
          RepositoryType = createModel.BuildReference.RepositoryType,
          BuildUri = createModel.BuildReference.Uri,
          RepositoryId = createModel.BuildReference.RepositoryGuid
        };
      if (testRun.BuildReference != null && !string.IsNullOrEmpty(createModel.BuildReference?.TargetBranchName))
        testRun.BuildReference.TargetBranchName = createModel.BuildReference.TargetBranchName;
      if (createModel.PipelineReference != null)
        testRun.PipelineReference = createModel.PipelineReference;
      if (!string.IsNullOrEmpty(createModel.State))
      {
        Microsoft.TeamFoundation.TestManagement.Client.TestRunState result3;
        if (!Enum.TryParse<Microsoft.TeamFoundation.TestManagement.Client.TestRunState>(createModel.State, true, out result3) || result3 != Microsoft.TeamFoundation.TestManagement.Client.TestRunState.NotStarted && result3 != Microsoft.TeamFoundation.TestManagement.Client.TestRunState.Waiting && result3 != Microsoft.TeamFoundation.TestManagement.Client.TestRunState.InProgress)
          throw new InvalidPropertyException("state", string.Format(ServerResources.CreateTestRunStateInvalidMessage, (object) createModel.State));
        testRun.State = (byte) result3;
      }
      else
        testRun.State = (byte) 2;
      if (!string.IsNullOrEmpty(createModel.Controller))
      {
        if (!testRun.IsAutomated || ((int) testRun.Type & 16) != 0)
          throw new InvalidPropertyException("controller", ServerResources.ControllerSetForNonAutomatedRun);
        testRun.Controller = createModel.Controller;
      }
      if (!string.IsNullOrEmpty(createModel.DueDate))
        testRun.DueDate = TestManagementServiceUtility.CheckAndGetDate(this.m_requestContext.RequestContext, createModel.DueDate, "dueDate");
      if (!string.IsNullOrEmpty(createModel.ErrorMessage))
        testRun.ErrorMessage = createModel.ErrorMessage;
      if (!string.IsNullOrEmpty(createModel.Comment))
        testRun.Comment = createModel.Comment;
      if (createModel.ConfigurationIds != null && createModel.ConfigurationIds.Length != 0)
        testRun.ConfigurationIds = createModel.ConfigurationIds;
      if (createModel.TestSettings != null)
      {
        ArgumentUtility.CheckStringForNullOrEmpty(createModel.TestSettings.Id, "testSettings.Id", "Test Results");
        int result4;
        if (!int.TryParse(createModel.TestSettings.Id, out result4) || result4 <= 0)
          throw new InvalidPropertyException("testSettings.Id", ServerResources.InvalidPropertyMessage);
        testRun.PublicTestSettingsId = result4;
      }
      if (!string.IsNullOrEmpty(createModel.TestEnvironmentId))
      {
        Guid result5;
        if (!Guid.TryParse(createModel.TestEnvironmentId, out result5))
          throw new InvalidPropertyException("testEnvironmentId", ServerResources.InvalidPropertyMessage);
        testRun.TestEnvironmentId = result5;
      }
      if (testRun.IsAutomated && ((int) testRun.Type & 16) != 0)
      {
        ArgumentUtility.CheckForNull<Microsoft.TeamFoundation.TestManagement.WebApi.ShallowReference>(createModel.DtlTestEnvironment, "DtlTestEnvironment", "Test Results");
        testRun.DtlTestEnvironment = createModel.DtlTestEnvironment;
        testRun.DtlAutEnvironment = createModel.DtlAutEnvironment;
        if (createModel.EnvironmentDetails != null && createModel.EnvironmentDetails.CsmContent != null && createModel.EnvironmentDetails.CsmParameters != null && createModel.EnvironmentDetails.SubscriptionName != null)
        {
          testRun.CsmContent = createModel.EnvironmentDetails.CsmContent;
          testRun.CsmParameters = createModel.EnvironmentDetails.CsmParameters;
          testRun.SubscriptionName = createModel.EnvironmentDetails.SubscriptionName;
        }
        ArgumentUtility.CheckForNull<RunFilter>(createModel.Filter, "Filter", "Test Results");
        ArgumentUtility.CheckStringForNullOrEmpty(createModel.Filter.SourceFilter, "Filter", "Test Results");
        testRun.Filter = createModel.Filter;
      }
      if (!string.IsNullOrEmpty(createModel.BuildDropLocation))
        testRun.DropLocation = createModel.BuildDropLocation;
      if (!string.IsNullOrEmpty(createModel.StartDate))
        testRun.StartDate = TestManagementServiceUtility.CheckAndGetDate(this.m_requestContext.RequestContext, createModel.StartDate, "startDate");
      if (!string.IsNullOrEmpty(createModel.CompleteDate))
        testRun.CompleteDate = TestManagementServiceUtility.CheckAndGetDate(this.m_requestContext.RequestContext, createModel.CompleteDate, "completeDate");
      this.AdjustStartAndCompleteDateForTestRunIfRequired(testRun);
      if (createModel.Owner != null && !string.IsNullOrEmpty(createModel.Owner.DisplayName))
      {
        Microsoft.VisualStudio.Services.Identity.Identity identity = new TestManagementServiceUtility(this.m_requestContext).ReadIdentityByDisplayName(createModel.Owner.DisplayName);
        testRun.Owner = identity != null ? identity.Id : throw new InvalidPropertyException(string.Format(ServerResources.TfsIdentityNotFound, (object) createModel.Owner.DisplayName));
        testRun.OwnerName = identity.DisplayName;
      }
      else
      {
        testRun.Owner = this.m_requestContext.UserTeamFoundationId;
        testRun.OwnerName = this.m_requestContext.UserTeamFoundationName;
      }
      if (!string.IsNullOrEmpty(createModel.ReleaseUri) && !string.IsNullOrEmpty(createModel.ReleaseEnvironmentUri))
      {
        testRun.ReleaseUri = createModel.ReleaseUri;
        testRun.ReleaseEnvironmentUri = createModel.ReleaseEnvironmentUri;
      }
      else if (!string.IsNullOrEmpty(createModel.ReleaseUri) || !string.IsNullOrEmpty(createModel.ReleaseEnvironmentUri))
        throw new ArgumentException(ServerResources.ReleaseIdOrEnvironmentIdNotSpecified).Expected("Test Results");
      if (createModel.ReleaseReference != null && createModel.ReleaseReference.Id > 0 && createModel.ReleaseReference.EnvironmentId > 0 && createModel.ReleaseReference.DefinitionId > 0 && createModel.ReleaseReference.EnvironmentDefinitionId > 0 && !string.IsNullOrEmpty(createModel.ReleaseReference.Name) && !string.IsNullOrEmpty(createModel.ReleaseReference.EnvironmentName))
        testRun.ReleaseReference = new ReleaseReference()
        {
          ReleaseId = createModel.ReleaseReference.Id,
          ReleaseEnvId = createModel.ReleaseReference.EnvironmentId,
          ReleaseDefId = createModel.ReleaseReference.DefinitionId,
          ReleaseEnvDefId = createModel.ReleaseReference.EnvironmentDefinitionId,
          ReleaseName = createModel.ReleaseReference.Name,
          ReleaseEnvName = createModel.ReleaseReference.EnvironmentName,
          ReleaseCreationDate = createModel.ReleaseReference.CreationDate,
          EnvironmentCreationDate = createModel.ReleaseReference.EnvironmentCreationDate,
          Attempt = createModel.ReleaseReference.Attempt,
          ReleaseUri = createModel.ReleaseUri,
          ReleaseEnvUri = createModel.ReleaseEnvironmentUri
        };
      if (createModel.CustomTestFields != null)
      {
        testRun.CustomFields = new List<TestExtensionField>();
        foreach (CustomTestField customTestField in createModel.CustomTestFields)
          testRun.CustomFields.Add(new TestExtensionField()
          {
            Field = new TestExtensionFieldDetails()
            {
              Name = customTestField.FieldName
            },
            Value = customTestField.Value
          });
      }
      if (createModel.Tags != null)
      {
        testRun.Tags = new List<TestTag>();
        testRun.Tags.AddRange((IEnumerable<TestTag>) createModel.Tags);
      }
      testRun.RunTimeout = createModel.RunTimeout;
      testRun.TestConfigurationsMapping = createModel.TestConfigurationsMapping;
      testRun.SourceWorkflow = string.IsNullOrEmpty(createModel.SourceWorkflow) ? this.GetSourceWorkflowForRun(testRun) : createModel.SourceWorkflow;
    }

    private string GetSourceWorkflowForRun(TestRun run)
    {
      if (!run.IsAutomated)
        return SourceWorkflow.Manual;
      return !string.IsNullOrEmpty(run.ReleaseUri) && !string.IsNullOrEmpty(run.ReleaseEnvironmentUri) ? SourceWorkflow.ContinuousDelivery : SourceWorkflow.ContinuousIntegration;
    }

    private void AdjustStartAndCompleteDateForTestRunIfRequired(TestRun testRun)
    {
      if (this.IsDateTimeDefault(testRun.CompleteDate))
        return;
      if (this.IsDateTimeDefault(testRun.StartDate))
      {
        testRun.StartDate = testRun.CompleteDate;
      }
      else
      {
        if (this.DateTimeCompare(testRun.CompleteDate, testRun.StartDate) >= 0)
          return;
        testRun.CompleteDate = testRun.StartDate;
      }
    }

    private bool IsDateTimeDefault(DateTime date) => DateTime.Compare(date.Kind != DateTimeKind.Utc ? date.ToUniversalTime() : date, new DateTime().ToUniversalTime()) == 0;

    private int DateTimeCompare(DateTime date1, DateTime date2) => DateTime.Compare(date1.Kind != DateTimeKind.Utc ? date1.ToUniversalTime() : date1, date2.Kind != DateTimeKind.Utc ? date2.ToUniversalTime() : date2);

    private void UpdateTestRunBuildUriAndNumberFromBuildId(string buildId, TestRun testRun)
    {
      if (string.IsNullOrEmpty(buildId))
        return;
      int result = 0;
      if (!int.TryParse(buildId, out result))
        return;
      testRun.BuildReference = new BuildConfiguration()
      {
        BuildId = result
      };
    }

    private TeamProjectReference GetProjectReference(string projectIdentifier)
    {
      using (PerfManager.Measure(this.m_requestContext.RequestContext, "CrossService", TraceUtils.GetActionName(nameof (GetProjectReference), "CSS")))
      {
        Guid result;
        if (Guid.TryParse(projectIdentifier, out result))
        {
          ProjectInfo projectFromGuid = this.m_requestContext.ProjectServiceHelper.GetProjectFromGuid(result);
          return projectFromGuid != null ? this.ToTeamProjectReference(this.m_requestContext.RequestContext, projectFromGuid) : (TeamProjectReference) null;
        }
        ProjectInfo projectFromName = this.m_requestContext.ProjectServiceHelper.GetProjectFromName(projectIdentifier);
        return projectFromName != null ? this.ToTeamProjectReference(this.m_requestContext.RequestContext, projectFromName) : (TeamProjectReference) null;
      }
    }

    public Microsoft.TeamFoundation.TestManagement.WebApi.TestRun ConvertTestRunToDataContract(
      TestRun testRun,
      TeamProjectReference projectReference,
      bool includeAllMetadata,
      bool includeAllDetails,
      bool includeAdditionalDetails)
    {
      Microsoft.TeamFoundation.TestManagement.WebApi.TestRun dataContract = new Microsoft.TeamFoundation.TestManagement.WebApi.TestRun();
      dataContract.Id = testRun.TestRunId;
      dataContract.Revision = testRun.Revision;
      dataContract.Iteration = testRun.Iteration;
      dataContract.Name = testRun.Title;
      dataContract.IsAutomated = testRun.IsAutomated;
      dataContract.State = ((Microsoft.TeamFoundation.TestManagement.Client.TestRunState) testRun.State).ToString();
      dataContract.TotalTests = testRun.TotalTests;
      dataContract.IncompleteTests = testRun.IncompleteTests;
      dataContract.NotApplicableTests = testRun.NotApplicableTests;
      dataContract.PassedTests = testRun.PassedTests;
      dataContract.UnanalyzedTests = testRun.UnanalyzedTests;
      dataContract.Url = this.GetTestRunUrl(testRun.TestRunId, projectReference.Name);
      dataContract.WebAccessUrl = this.m_testManagementRunService.GetTestRunWebAccessUrl(this.m_requestContext, testRun.TestRunId, projectReference.Name);
      if (includeAllMetadata)
      {
        dataContract.Owner = IdentityHelper.ToIdentityRef(this.m_requestContext.RequestContext, testRun.Owner.ToString(), testRun.OwnerName);
        dataContract.StartedDate = testRun.StartDate;
        dataContract.CompletedDate = testRun.CompleteDate;
        if (testRun.TestPlanId > 0)
          dataContract.Plan = new Microsoft.TeamFoundation.TestManagement.WebApi.ShallowReference()
          {
            Id = testRun.TestPlanId.ToString()
          };
        if (testRun.PublicTestSettingsId > 0)
          dataContract.TestSettings = new Microsoft.TeamFoundation.TestManagement.WebApi.ShallowReference()
          {
            Id = testRun.PublicTestSettingsId.ToString()
          };
        if (!string.IsNullOrEmpty(testRun.ReleaseUri))
          dataContract.ReleaseUri = testRun.ReleaseUri;
        if (!string.IsNullOrEmpty(testRun.ReleaseEnvironmentUri))
          dataContract.ReleaseEnvironmentUri = testRun.ReleaseEnvironmentUri;
        if (testRun.BuildReference != null && testRun.BuildReference.BuildId != 0)
          dataContract.Build = new Microsoft.TeamFoundation.TestManagement.WebApi.ShallowReference()
          {
            Id = testRun.BuildReference.BuildId.ToString()
          };
        dataContract.Release = this.GetReleaseReference(testRun.ReleaseReference);
        dataContract.Project = this.GetProjectRepresentation(projectReference);
        if (testRun.PipelineReference != null)
          dataContract.PipelineReference = testRun.PipelineReference;
        if (includeAllDetails)
        {
          if (testRun.TestPlanId > 0)
            dataContract.Plan = this.GetPlanRepresentation(testRun.TeamProject, testRun.TestPlanId);
          dataContract.Build = this.GetBuildRepresentation(testRun.BuildReference);
          dataContract.BuildConfiguration = testRun.BuildReference == null || testRun.BuildReference.BuildId != 0 ? this.GetBuildConfiguration(testRun.TeamProject, testRun.BuildReference) : this.GetBuildConfiguration(testRun.TeamProject, testRun);
          Microsoft.VisualStudio.Services.Identity.Identity identity1 = this.ReadIdentityByAccountId(testRun.Owner);
          if (identity1 != null)
            dataContract.Owner = this.CreateTeamFoundationIdentityReference(identity1);
          if (testRun.TestEnvironmentId != Guid.Empty)
            dataContract.TestEnvironment = this.m_testEnvironmentHelper?.GetTestEnvironment(testRun.TeamProject, testRun.TestEnvironmentId);
          dataContract.PostProcessState = ((PostProcessState) testRun.PostProcessState).ToString();
          if (!string.IsNullOrEmpty(testRun.ErrorMessage))
            dataContract.ErrorMessage = testRun.ErrorMessage;
          dataContract.DueDate = testRun.DueDate;
          dataContract.CreatedDate = testRun.CreationDate;
          dataContract.LastUpdatedDate = testRun.LastUpdated;
          dataContract.RunStatistics = this.m_statisticsHelper.GetTestRunStatisticsForRun(projectReference, testRun).RunStatistics;
          Microsoft.VisualStudio.Services.Identity.Identity identity2 = this.ReadIdentityByAccountId(testRun.LastUpdatedBy);
          if (identity2 != null)
            dataContract.LastUpdatedBy = this.CreateTeamFoundationIdentityReference(identity2);
          dataContract.Controller = testRun.Controller;
          if (!string.IsNullOrEmpty(testRun.Comment))
            dataContract.Comment = testRun.Comment;
          dataContract.DropLocation = testRun.DropLocation;
          dataContract.TestMessageLogId = testRun.TestMessageLogId;
          if (testRun.PublicTestSettingsId > 0)
          {
            TestSettings testSettingdById = this.ObjectFactory.GetTestSettingdById(this.m_requestContext, testRun.PublicTestSettingsId, testRun.TeamProject);
            if (testSettingdById != null)
              dataContract.TestSettings = new Microsoft.TeamFoundation.TestManagement.WebApi.ShallowReference()
              {
                Id = testSettingdById.Id.ToString(),
                Name = testSettingdById.Name
              };
          }
          dataContract.Filter = testRun.Filter;
          dataContract.DtlEnvironment = testRun.DtlTestEnvironment;
          dataContract.DtlAutEnvironment = testRun.DtlAutEnvironment;
          dataContract.Substate = (TestRunSubstate) testRun.Substate;
          if (testRun.CustomFields != null)
          {
            dataContract.CustomFields = new List<CustomTestField>();
            foreach (TestExtensionField customField in testRun.CustomFields)
              dataContract.CustomFields.Add(new CustomTestField()
              {
                FieldName = customField.Field.Name,
                Value = customField.Value
              });
          }
          if (this.m_requestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.GetReleaseDetailsFromTcm"))
            this.PopulateReleaseDetailsFromRef(dataContract, projectReference);
          else
            this.PopulateReleaseDetails(dataContract, projectReference);
        }
        else if (includeAdditionalDetails)
        {
          dataContract.Build = this.GetBuildRepresentation(testRun.BuildReference);
          dataContract.BuildConfiguration = testRun.BuildReference == null || testRun.BuildReference.BuildId != 0 ? this.GetBuildConfiguration(testRun.TeamProject, testRun.BuildReference) : this.GetBuildConfiguration(testRun.TeamProject, testRun);
          dataContract.Filter = testRun.Filter;
          dataContract.DtlEnvironment = testRun.DtlTestEnvironment;
          dataContract.DtlAutEnvironment = testRun.DtlAutEnvironment;
          dataContract.DropLocation = testRun.DropLocation;
        }
        if (testRun.Tags != null)
          dataContract.Tags = (IList<TestTag>) testRun.Tags;
      }
      this.SecureTestRunWebApiObject(dataContract, projectReference.Id);
      return dataContract;
    }

    internal void SecureTestRunWebApiObject(Microsoft.TeamFoundation.TestManagement.WebApi.TestRun testRun, Guid projectId)
    {
      this.m_requestContext.RequestContext.CheckPermissionToReadPublicIdentityInfo();
      testRun.InitializeSecureObject((ISecuredObject) new Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult()
      {
        Project = new Microsoft.TeamFoundation.TestManagement.WebApi.ShallowReference()
        {
          Id = projectId.ToString()
        }
      });
    }

    internal IList<TestSettings2> ConvertTestSettingsListToDataContract(
      IList<TestSettings> testSettings)
    {
      List<TestSettings2> dataContract = new List<TestSettings2>(testSettings.Count);
      foreach (TestSettings testSetting in (IEnumerable<TestSettings>) testSettings)
        dataContract.Add(this.ConvertTestSettingToDataContract(testSetting));
      return (IList<TestSettings2>) dataContract;
    }

    internal TestSettings2 ConvertTestSettingToDataContract(TestSettings testSettings) => new TestSettings2()
    {
      TestSettingsId = testSettings.Id,
      TestSettingsName = testSettings.Name,
      TestSettingsContent = testSettings.Settings,
      AreaPath = testSettings.AreaPath,
      Description = testSettings.Description,
      IsPublic = testSettings.IsPublic,
      MachineRoles = TestSettingsMachineRole.ToXml(testSettings.MachineRoles),
      CreatedDate = testSettings.CreatedDate,
      LastUpdatedDate = testSettings.LastUpdated,
      CreatedBy = IdentityHelper.ToIdentityRef(this.m_requestContext.RequestContext, testSettings.CreatedBy.ToString(), testSettings.CreatedByName),
      LastUpdatedBy = IdentityHelper.ToIdentityRef(this.m_requestContext.RequestContext, testSettings.LastUpdatedBy.ToString(), testSettings.LastUpdatedByName)
    };

    internal IList<TestSettings> GetTestSettingsListFromDataContract(
      IList<TestSettings2> testSettingsDataContractList)
    {
      List<TestSettings> fromDataContract = new List<TestSettings>(testSettingsDataContractList.Count);
      foreach (TestSettings2 settingsDataContract in (IEnumerable<TestSettings2>) testSettingsDataContractList)
        fromDataContract.Add(this.GetTestSettingsFromDataContract(settingsDataContract));
      return (IList<TestSettings>) fromDataContract;
    }

    internal TestSettings GetTestSettingsFromDataContract(TestSettings2 testSettingsDataContract) => new TestSettings()
    {
      Id = testSettingsDataContract.TestSettingsId,
      Name = testSettingsDataContract.TestSettingsName,
      Description = testSettingsDataContract.Description,
      AreaPath = testSettingsDataContract.AreaPath,
      Settings = this.m_requestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableHtmlDecodeForTestSettings") ? HttpUtility.HtmlDecode(testSettingsDataContract.TestSettingsContent) : testSettingsDataContract.TestSettingsContent,
      MachineRoles = TestSettingsMachineRole.FromXml(this.m_requestContext.RequestContext, testSettingsDataContract.MachineRoles),
      IsPublic = testSettingsDataContract.IsPublic,
      LastUpdated = testSettingsDataContract.LastUpdatedDate,
      CreatedDate = testSettingsDataContract.CreatedDate,
      LastUpdatedBy = this.getId(testSettingsDataContract.LastUpdatedBy),
      CreatedBy = this.getId(testSettingsDataContract.CreatedBy)
    };

    internal Guid getId(IdentityRef identity)
    {
      Guid result = Guid.Empty;
      if (identity != null)
        Guid.TryParse(identity.Id, out result);
      return result;
    }

    internal Microsoft.TeamFoundation.TestManagement.WebApi.TestRun ConvertTestRunToDataContractForBulkApi(
      TestRun testRun,
      TeamProjectReference projectReference)
    {
      Microsoft.TeamFoundation.TestManagement.WebApi.TestRun contractForBulkApi = new Microsoft.TeamFoundation.TestManagement.WebApi.TestRun();
      contractForBulkApi.Id = testRun.TestRunId;
      contractForBulkApi.Name = testRun.Title;
      contractForBulkApi.IsAutomated = testRun.IsAutomated;
      contractForBulkApi.State = ((Microsoft.TeamFoundation.TestManagement.Client.TestRunState) testRun.State).ToString();
      contractForBulkApi.TotalTests = testRun.TotalTests;
      contractForBulkApi.Url = this.GetTestRunUrl(testRun.TestRunId, testRun.TeamProject != null ? testRun.TeamProject : projectReference.Name);
      contractForBulkApi.WebAccessUrl = this.m_testManagementRunService.GetTestRunWebAccessUrl(this.m_requestContext, testRun.TestRunId, projectReference.Name);
      contractForBulkApi.IncompleteTests = testRun.IncompleteTests;
      contractForBulkApi.NotApplicableTests = testRun.NotApplicableTests;
      contractForBulkApi.PassedTests = testRun.PassedTests;
      contractForBulkApi.UnanalyzedTests = testRun.UnanalyzedTests;
      contractForBulkApi.Owner = IdentityHelper.ToIdentityRef(this.m_requestContext.RequestContext, testRun.Owner.ToString(), testRun.OwnerName);
      contractForBulkApi.StartedDate = testRun.StartDate;
      contractForBulkApi.CompletedDate = testRun.CompleteDate;
      if (testRun.TestPlanId > 0)
        contractForBulkApi.Plan = new Microsoft.TeamFoundation.TestManagement.WebApi.ShallowReference()
        {
          Id = testRun.TestPlanId.ToString()
        };
      contractForBulkApi.Project = this.GetProjectRepresentation(projectReference);
      if (contractForBulkApi.Project != null)
        testRun.TeamProject = contractForBulkApi.Project.Name;
      if (testRun.TestPlanId > 0)
        contractForBulkApi.Plan = this.GetPlanRepresentation(testRun.TeamProject, testRun.TestPlanId);
      if (testRun.BuildReference != null)
        contractForBulkApi.BuildConfiguration = this.GetBuildConfiguration(testRun.TeamProject, testRun.BuildReference);
      if (testRun.PipelineReference != null)
        contractForBulkApi.PipelineReference = testRun.PipelineReference;
      Microsoft.VisualStudio.Services.Identity.Identity identity1 = this.ReadIdentityByAccountId(testRun.Owner);
      if (identity1 != null)
        contractForBulkApi.Owner = this.CreateTeamFoundationIdentityReference(identity1);
      contractForBulkApi.CreatedDate = testRun.CreationDate;
      contractForBulkApi.LastUpdatedDate = testRun.LastUpdated;
      Microsoft.VisualStudio.Services.Identity.Identity identity2 = this.ReadIdentityByAccountId(testRun.LastUpdatedBy);
      if (identity2 != null)
        contractForBulkApi.LastUpdatedBy = this.CreateTeamFoundationIdentityReference(identity2);
      if (testRun.CustomFields != null)
      {
        contractForBulkApi.CustomFields = new List<CustomTestField>();
        foreach (TestExtensionField customField in testRun.CustomFields)
          contractForBulkApi.CustomFields.Add(new CustomTestField()
          {
            FieldName = customField.Field.Name,
            Value = customField.Value
          });
      }
      if (testRun.ReleaseReference != null)
        contractForBulkApi.Release = new Microsoft.TeamFoundation.TestManagement.WebApi.ReleaseReference()
        {
          Id = testRun.ReleaseReference.ReleaseId,
          EnvironmentId = testRun.ReleaseReference.ReleaseEnvId
        };
      if (testRun.TestRunStatistics == null)
      {
        List<TestRunStatistic> testRunStatisticList = new List<TestRunStatistic>();
        if (testRun.PassedTests != 0)
          testRunStatisticList.Add(new TestRunStatistic()
          {
            Outcome = (byte) 2,
            Count = testRun.PassedTests
          });
        if (testRun.UnanalyzedTests != 0)
          testRunStatisticList.Add(new TestRunStatistic()
          {
            Outcome = (byte) 3,
            Count = testRun.UnanalyzedTests
          });
        testRun.TestRunStatistics = testRunStatisticList.ToArray();
      }
      contractForBulkApi.RunStatistics = new List<RunStatistic>();
      contractForBulkApi.RunStatistics.AddRange(((IEnumerable<TestRunStatistic>) testRun.TestRunStatistics).Select<TestRunStatistic, RunStatistic>((Func<TestRunStatistic, RunStatistic>) (testRunStatistic => this.ConvertTestRunStatisticsToDataContract(testRunStatistic))));
      return contractForBulkApi;
    }

    internal RunStatistic ConvertTestRunStatisticsToDataContract(TestRunStatistic testRunStatistic) => new RunStatistic()
    {
      Outcome = Enum.GetName(typeof (Microsoft.TeamFoundation.TestManagement.Client.TestOutcome), (object) (Microsoft.TeamFoundation.TestManagement.Client.TestOutcome) testRunStatistic.Outcome),
      Count = testRunStatistic.Count,
      State = Enum.GetName(typeof (Microsoft.TeamFoundation.TestManagement.Client.TestOutcome), (object) TestResultState.Unspecified)
    };

    protected IdentityRef CreateTeamFoundationIdentityReference(Microsoft.VisualStudio.Services.Identity.Identity identity) => identity.ToIdentityRef(this.m_requestContext.RequestContext);

    protected Microsoft.VisualStudio.Services.Identity.Identity ReadIdentityByAccountId(
      Guid accountId)
    {
      if (accountId != Guid.Empty)
      {
        Microsoft.VisualStudio.Services.Identity.Identity[] source = this.ReadIdentityByAccounts(new Guid[1]
        {
          accountId
        });
        if (source != null)
          return ((IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) source).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
      }
      return (Microsoft.VisualStudio.Services.Identity.Identity) null;
    }

    protected Microsoft.VisualStudio.Services.Identity.Identity[] ReadIdentityByAccounts(
      Guid[] accountIds)
    {
      if (!((IEnumerable<Guid>) accountIds).IsNullOrEmpty<Guid>())
      {
        IList<Microsoft.VisualStudio.Services.Identity.Identity> source = this.m_requestContext.IdentityService.ReadIdentities(this.m_requestContext.RequestContext, (IList<Guid>) accountIds, QueryMembership.None, (IEnumerable<string>) null);
        if (source != null)
          return source.ToArray<Microsoft.VisualStudio.Services.Identity.Identity>();
      }
      return (Microsoft.VisualStudio.Services.Identity.Identity[]) null;
    }

    protected Microsoft.TeamFoundation.TestManagement.WebApi.ShallowReference GetProjectRepresentation(
      TeamProjectReference projectReference)
    {
      return new Microsoft.TeamFoundation.TestManagement.WebApi.ShallowReference()
      {
        Name = projectReference.Name,
        Id = projectReference.Id != Guid.Empty ? projectReference.Id.ToString() : string.Empty,
        Url = projectReference.Url
      };
    }

    protected Microsoft.TeamFoundation.TestManagement.WebApi.BuildConfiguration GetBuildConfiguration(
      string projectName,
      BuildConfiguration buildConfig)
    {
      if (buildConfig == null)
        return (Microsoft.TeamFoundation.TestManagement.WebApi.BuildConfiguration) null;
      return new Microsoft.TeamFoundation.TestManagement.WebApi.BuildConfiguration()
      {
        Flavor = buildConfig.BuildFlavor,
        Id = buildConfig.BuildId,
        Platform = buildConfig.BuildPlatform,
        Project = new Microsoft.TeamFoundation.TestManagement.WebApi.ShallowReference()
        {
          Name = projectName
        },
        Uri = buildConfig.BuildUri,
        BuildDefinitionId = buildConfig.BuildDefinitionId,
        Number = buildConfig.BuildNumber,
        TargetBranchName = buildConfig.TargetBranchName,
        BranchName = buildConfig.BranchName
      };
    }

    protected virtual Microsoft.TeamFoundation.TestManagement.WebApi.ShallowReference GetPlanRepresentation(
      string projectName,
      int planId)
    {
      return new Microsoft.TeamFoundation.TestManagement.WebApi.ShallowReference()
      {
        Id = planId.ToString()
      };
    }

    public void PopulateReleaseDetails(
      Microsoft.TeamFoundation.TestManagement.WebApi.TestRun testRunDataContract,
      TeamProjectReference projectReference)
    {
      if (string.IsNullOrEmpty(testRunDataContract.ReleaseUri) || string.IsNullOrEmpty(testRunDataContract.ReleaseEnvironmentUri))
        return;
      ReleaseReference releaseReference = this.m_releaseServiceHelper.QueryReleaseReferenceByUri(this.m_requestContext.RequestContext, new GuidAndString(string.Empty, projectReference.Id), testRunDataContract.ReleaseUri, testRunDataContract.ReleaseEnvironmentUri);
      if (releaseReference == null)
        return;
      testRunDataContract.Release = new Microsoft.TeamFoundation.TestManagement.WebApi.ReleaseReference()
      {
        Id = releaseReference.ReleaseId,
        Name = releaseReference.ReleaseName,
        EnvironmentId = releaseReference.ReleaseEnvId,
        DefinitionId = releaseReference.ReleaseDefId,
        EnvironmentDefinitionId = releaseReference.ReleaseEnvDefId,
        EnvironmentName = releaseReference.ReleaseEnvName,
        CreationDate = releaseReference.ReleaseCreationDate,
        EnvironmentCreationDate = releaseReference.EnvironmentCreationDate,
        Attempt = releaseReference.Attempt
      };
    }

    public void PopulateReleaseDetailsFromRef(
      Microsoft.TeamFoundation.TestManagement.WebApi.TestRun testRunDataContract,
      TeamProjectReference projectReference)
    {
      if (string.IsNullOrEmpty(testRunDataContract.ReleaseUri) || string.IsNullOrEmpty(testRunDataContract.ReleaseEnvironmentUri))
        return;
      ReleaseReference releaseReference = (ReleaseReference) null;
      using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(this.m_requestContext))
      {
        int releaseArtifactId1 = this.m_releaseServiceHelper.GetReleaseArtifactId(testRunDataContract.ReleaseUri);
        int releaseArtifactId2 = this.m_releaseServiceHelper.GetReleaseArtifactId(testRunDataContract.ReleaseEnvironmentUri);
        releaseReference = managementDatabase.GetReleaseRef(projectReference.Id, releaseArtifactId1, releaseArtifactId2);
      }
      if (releaseReference == null)
        return;
      testRunDataContract.Release = new Microsoft.TeamFoundation.TestManagement.WebApi.ReleaseReference()
      {
        Id = releaseReference.ReleaseId,
        Name = releaseReference.ReleaseName,
        EnvironmentId = releaseReference.ReleaseEnvId,
        DefinitionId = releaseReference.ReleaseDefId,
        EnvironmentDefinitionId = releaseReference.ReleaseEnvDefId,
        EnvironmentName = !string.IsNullOrEmpty(releaseReference.ReleaseEnvName) ? releaseReference.ReleaseEnvName : testRunDataContract.ReleaseEnvironmentUri,
        CreationDate = releaseReference.ReleaseCreationDate,
        EnvironmentCreationDate = releaseReference.EnvironmentCreationDate,
        Attempt = releaseReference.Attempt
      };
    }

    protected Microsoft.TeamFoundation.TestManagement.WebApi.ShallowReference GetBuildRepresentation(
      BuildConfiguration buildRef)
    {
      if (buildRef == null || buildRef.BuildId <= 0)
        return (Microsoft.TeamFoundation.TestManagement.WebApi.ShallowReference) null;
      return new Microsoft.TeamFoundation.TestManagement.WebApi.ShallowReference()
      {
        Id = buildRef.BuildId.ToString((IFormatProvider) CultureInfo.InvariantCulture),
        Name = buildRef.BuildNumber,
        Url = UrlBuildHelper.GetResourceUrl(this.m_requestContext.RequestContext, ServiceInstanceTypes.TFS, "build", BuildResourceIds.Builds, (object) new
        {
          buildId = buildRef.BuildId
        })
      };
    }

    protected virtual string GetTestRunUrl(int testRunId, string projectName)
    {
      if (!this.m_requestContext.ResourceMappings.ContainsKey(ResourceMappingConstants.TestResult))
        return string.Empty;
      RestApiResourceDetails resourceMapping = this.m_requestContext.ResourceMappings[ResourceMappingConstants.TestRun];
      return UrlBuildHelper.GetResourceUrl(this.m_requestContext.RequestContext, resourceMapping.ServiceInstanceType, resourceMapping.Area, resourceMapping.ResourceId, (object) new
      {
        runId = testRunId,
        project = projectName
      });
    }

    protected Microsoft.TeamFoundation.TestManagement.WebApi.BuildConfiguration GetBuildConfiguration(
      string projectName,
      TestRun testRun)
    {
      if (testRun == null || string.IsNullOrWhiteSpace(testRun.BuildUri))
        return (Microsoft.TeamFoundation.TestManagement.WebApi.BuildConfiguration) null;
      int result = 0;
      int.TryParse(LinkingUtilities.DecodeUri(testRun.BuildUri).ToolSpecificId, out result);
      return new Microsoft.TeamFoundation.TestManagement.WebApi.BuildConfiguration()
      {
        Flavor = testRun.BuildFlavor,
        Id = result,
        Platform = testRun.BuildPlatform,
        Project = new Microsoft.TeamFoundation.TestManagement.WebApi.ShallowReference()
        {
          Name = projectName
        },
        Uri = testRun.BuildUri
      };
    }

    protected TeamProjectReference ToTeamProjectReference(
      IVssRequestContext requestContext,
      ProjectInfo projectInfo)
    {
      return new TeamProjectReference()
      {
        Id = projectInfo.Id,
        Abbreviation = projectInfo.Abbreviation,
        Name = projectInfo.Name,
        State = projectInfo.State,
        Description = projectInfo.Description,
        Revision = projectInfo.Revision,
        Visibility = projectInfo.Visibility
      };
    }

    protected Microsoft.TeamFoundation.TestManagement.WebApi.ReleaseReference GetReleaseReference(
      ReleaseReference releaseRef)
    {
      if (releaseRef == null || releaseRef.ReleaseId <= 0)
        return (Microsoft.TeamFoundation.TestManagement.WebApi.ReleaseReference) null;
      return new Microsoft.TeamFoundation.TestManagement.WebApi.ReleaseReference()
      {
        Id = releaseRef.ReleaseId,
        Name = releaseRef.ReleaseName,
        EnvironmentId = releaseRef.ReleaseEnvId,
        DefinitionId = releaseRef.ReleaseDefId,
        EnvironmentDefinitionId = releaseRef.ReleaseEnvDefId,
        EnvironmentName = releaseRef.ReleaseEnvName,
        CreationDate = releaseRef.ReleaseCreationDate,
        EnvironmentCreationDate = releaseRef.EnvironmentCreationDate,
        Attempt = releaseRef.Attempt
      };
    }

    protected internal virtual ITestManagementObjectHelper ObjectFactory
    {
      get
      {
        if (this.m_objectFactory == null)
          this.m_objectFactory = (ITestManagementObjectHelper) new TestManagementObjectHelper();
        return this.m_objectFactory;
      }
      set => this.m_objectFactory = value;
    }
  }
}
