// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestExecution.Server.DistributedTestRunService
// Assembly: Microsoft.VisualStudio.Services.TE.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2BC2680F-A5FB-41BE-A4CF-F78BF7AC3E02
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.TE.Server.Common.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Test.WebApi;
using Microsoft.TeamFoundation.TestExecution.Server.Database.Model;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.TestExecution.Server
{
  [CLSCompliant(false)]
  public class DistributedTestRunService : IDistributedTestRunService, IVssFrameworkService
  {
    private const string TestRunSystem = "TestRunSystem";
    private const string TestRunSystemValue = "VSTS - vstest";

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public virtual DistributedTestRun CreateTestRun(
      TestExecutionRequestContext testRequestContext,
      Guid projectId,
      DistributedTestRun createRun)
    {
      ArgumentUtility.CheckForEmptyGuid(projectId, "Project Id");
      ArgumentUtility.CheckForNull<DistributedTestRun>(createRun, "Distributed Test Run");
      ArgumentUtility.CheckForNull<DistributedTestRunCreateModel>(createRun.DistributedTestRunCreateModel, "Run Create Model");
      ArgumentUtility.CheckForNull<string>(createRun.DistributedTestRunCreateModel.EnvironmentUrl, "Distributed Test Run Environment");
      IVssRequestContext requestContext = testRequestContext.RequestContext;
      DtaLogger logger = DistributedTestRunService.GetLogger(testRequestContext);
      logger.Verbose(6200316, string.Format("DistributedTestRunService:CreateTestRun() called with project {0} for environment {1}", (object) projectId, (object) createRun.DistributedTestRunCreateModel.EnvironmentUrl));
      RunCreateModel runCreateModel = DistributedTestRunService.GetRunCreateModel(createRun);
      try
      {
        using (DistributedTestRunDatabase component = requestContext.CreateComponent<DistributedTestRunDatabase>())
        {
          string runProperties = JsonUtility.ToString((object) createRun.DistributedTestRunCreateModel.RunProperties);
          DistributedTestRunDbModel distributedTestRunDbModel = component.QueryDistributedTestRun(projectId, runCreateModel.DtlTestEnvironment.Url);
          if (distributedTestRunDbModel != null && distributedTestRunDbModel.TestRunId == DtaConstants.FailedTestRun && DateTime.UtcNow.Subtract(distributedTestRunDbModel.CreationDate) >= TimeSpan.FromSeconds(100.0))
          {
            logger.Error(6200319, "DistributedTestRunService:CreateTestRun(): Deleting stale distributed test run for environment " + createRun.DistributedTestRunCreateModel.EnvironmentUrl);
            component.DeleteDistributedTestRun(projectId, runCreateModel.DtlTestEnvironment.Url);
          }
          component.CreateDistributedTestRun(projectId, runCreateModel.DtlTestEnvironment.Url, runProperties);
        }
      }
      catch (TestEnvironmentAlreadyExistsException ex)
      {
        logger.Verbose(6200317, "DistributedTestRunService:CreateTestRun() completed. Run is not created for environment " + createRun.DistributedTestRunCreateModel.EnvironmentUrl);
        return new DistributedTestRun()
        {
          EnvironmentUri = createRun.DistributedTestRunCreateModel.EnvironmentUrl,
          TestRunId = DtaConstants.FailedTestRun
        };
      }
      DistributedTestRun testRun = this.UpdateDistributedTestRunWithTraTestRun(testRequestContext, projectId, runCreateModel, createRun);
      logger.Verbose(6200318, string.Format("DistributedTestRunService:CreateTestRun() completed with test run id {0} for environment {1}", (object) testRun.TestRunId, (object) testRun.EnvironmentUri));
      return testRun;
    }

    public virtual DistributedTestRun GetTestRun(
      TestExecutionRequestContext testRequestContext,
      Guid projectId,
      string environmentUri)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(environmentUri, "Distributed Test Run Environment");
      IVssRequestContext requestContext = testRequestContext.RequestContext;
      DtaLogger logger = DistributedTestRunService.GetLogger(testRequestContext);
      logger.Verbose(6200314, "DistributedTestRunService:GetTestRun() called for environment {0}", (object) environmentUri);
      DistributedTestRun testRun = new DistributedTestRun()
      {
        EnvironmentUri = environmentUri,
        TestRunId = -1
      };
      using (DistributedTestRunDatabase component = requestContext.CreateComponent<DistributedTestRunDatabase>())
        testRun.TestRunId = component.QueryDistributedTestRun(projectId, environmentUri, out string _);
      logger.Verbose(6200315, "DistributedTestRunService:GetTestRun() completed for environment {0}", (object) environmentUri);
      return testRun;
    }

    public virtual DistributedTestRun UpdateTestRun(
      TestExecutionRequestContext testRequestContext,
      Guid projectId,
      DistributedTestRun distributedTestRun)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(distributedTestRun.EnvironmentUri, "Distributed Test Run Environment");
      IVssRequestContext requestContext = testRequestContext.RequestContext;
      DtaLogger logger = DistributedTestRunService.GetLogger(testRequestContext);
      logger.Verbose(6200312, "DistributedTestRunService:UpdateTestRun() called for environment {0}", (object) distributedTestRun.EnvironmentUri);
      using (DistributedTestRunDatabase component = requestContext.CreateComponent<DistributedTestRunDatabase>())
        component.UpdateDistributedTestRun(projectId, distributedTestRun.EnvironmentUri, distributedTestRun.TestRunId);
      logger.Verbose(6200313, "DistributedTestRunService:UpdateTestRun() completed for environment {0}", (object) distributedTestRun.EnvironmentUri);
      return distributedTestRun;
    }

    public DistributedTestRun GetTestRun(
      TestExecutionRequestContext testRequestContext,
      Guid projectId,
      int testRunId)
    {
      ArgumentUtility.CheckForNonnegativeInt(testRunId, "Test Run Id");
      IVssRequestContext requestContext = testRequestContext.RequestContext;
      DtaLogger logger = DistributedTestRunService.GetLogger(testRequestContext);
      logger.Verbose(6200310, string.Format("DistributedTestRunService:GetTestRun() called for testRunId {0}", (object) testRunId));
      DistributedTestRunDbModel distributedTestRunDbModel;
      using (DistributedTestRunDatabase component = requestContext.CreateComponent<DistributedTestRunDatabase>())
        distributedTestRunDbModel = component.QueryDistributedTestRun(projectId, testRunId);
      RunProperties runProperties = (RunProperties) null;
      if (!string.IsNullOrEmpty(distributedTestRunDbModel.RunProperties))
        runProperties = JsonUtilities.Deserialize<RunProperties>(distributedTestRunDbModel.RunProperties);
      DistributedTestRun testRun = new DistributedTestRun();
      testRun.TestRunId = distributedTestRunDbModel.TestRunId;
      testRun.EnvironmentUri = distributedTestRunDbModel.EnvironmentUri;
      testRun.DistributedTestRunCreateModel = new DistributedTestRunCreateModel()
      {
        RunProperties = runProperties
      };
      logger.Verbose(6200311, string.Format("DistributedTestRunService:GetTestRun() completed for testRunId {0}", (object) testRunId));
      return testRun;
    }

    public virtual void DeleteTestRun(
      TestExecutionRequestContext testRequestContext,
      Guid projectId,
      DistributedTestRun distributedTestRun)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(distributedTestRun.EnvironmentUri, "Distributed Test Run Environment");
      IVssRequestContext requestContext = testRequestContext.RequestContext;
      DtaLogger logger = DistributedTestRunService.GetLogger(testRequestContext);
      logger.Verbose(6200308, "DistributedTestRunService:QueueDeleteTestRun() called for environment " + distributedTestRun.EnvironmentUri);
      using (DistributedTestRunDatabase component = requestContext.CreateComponent<DistributedTestRunDatabase>())
        component.DeleteDistributedTestRun(projectId, distributedTestRun.EnvironmentUri);
      logger.Verbose(6200309, "DistributedTestRunService:QueueDeleteTestRun() completed for environment " + distributedTestRun.EnvironmentUri);
    }

    public virtual void DeleteTestRuns(
      TestExecutionRequestContext teamFoundationRequestContext,
      int numberOfDaysOlder)
    {
      IVssRequestContext requestContext = teamFoundationRequestContext.RequestContext;
      DtaLogger logger = DistributedTestRunService.GetLogger(teamFoundationRequestContext);
      logger.Verbose(6200306, "DistributedTestRunService:DeleteTestRuns()");
      using (DistributedTestRunDatabase component = requestContext.CreateComponent<DistributedTestRunDatabase>())
        component.DeleteDistributedTestRuns(numberOfDaysOlder);
      logger.Verbose(6200307, "DistributedTestRunService:DeleteTestRuns()");
    }

    private int CreateTraTestRun(
      TestExecutionRequestContext tfsRequestContext,
      RunCreateModel runCreateModel,
      Guid projectId)
    {
      DtaLogger logger = DistributedTestRunService.GetLogger(tfsRequestContext);
      logger.Verbose(6200304, "Calling TestManagement Create TestRun for project {0}", (object) projectId);
      int id = tfsRequestContext.TestResultsHttpClient.CreateTestRunAsync(runCreateModel, projectId).SyncResult<TestRun>().Id;
      logger.Verbose(6200305, "Completed TestManagement Create TestRun for project {0} with test run id {1}", (object) projectId, (object) id);
      return id;
    }

    private void StartTraTestRun(
      TestExecutionRequestContext tfsRequestContext,
      RunCreateModel runCreateModel,
      Guid projectId,
      int runId)
    {
      DtaLogger logger = DistributedTestRunService.GetLogger(tfsRequestContext);
      logger.Verbose(6200301, "Calling TestManagement Update TestRun for project {0} with test run id {1}", (object) projectId, (object) runId);
      TestRun testRun = tfsRequestContext.TestResultsHttpClient.GetTestRunByIdAsync(projectId, runId).SyncResult<TestRun>();
      if (testRun.State != "NotStarted")
        throw new TestExecutionServiceInvalidOperationException(string.Format(TestExecutionServiceResources.CannotStartTheRun, (object) testRun.Id, (object) testRun.State), (Exception) null);
      string name = runCreateModel.Name;
      Microsoft.TeamFoundation.TestManagement.WebApi.ShallowReference testSettings1 = runCreateModel.TestSettings;
      Microsoft.TeamFoundation.TestManagement.WebApi.ShallowReference dtlAutEnvironment1 = runCreateModel.DtlAutEnvironment;
      Microsoft.TeamFoundation.TestManagement.WebApi.ShallowReference dtlTestEnvironment = runCreateModel.DtlTestEnvironment;
      Microsoft.TeamFoundation.TestManagement.WebApi.ShallowReference build = runCreateModel.Build;
      Microsoft.TeamFoundation.TestManagement.WebApi.ShallowReference testSettings2 = testSettings1;
      bool? deleteUnexecutedResults = new bool?();
      Microsoft.TeamFoundation.TestManagement.WebApi.ShallowReference dtlEnvironment = dtlTestEnvironment;
      Microsoft.TeamFoundation.TestManagement.WebApi.ShallowReference dtlAutEnvironment2 = dtlAutEnvironment1;
      RunUpdateModel runUpdateModel = new RunUpdateModel(name, state: "InProgress", build: build, testSettings: testSettings2, deleteUnexecutedResults: deleteUnexecutedResults, dtlEnvironment: dtlEnvironment, dtlAutEnvironment: dtlAutEnvironment2)
      {
        BuildDropLocation = runCreateModel.BuildDropLocation,
        BuildFlavor = runCreateModel.BuildFlavor,
        BuildPlatform = runCreateModel.BuildPlatform,
        ReleaseEnvironmentUri = runCreateModel.ReleaseEnvironmentUri,
        ReleaseUri = runCreateModel.ReleaseUri,
        SourceWorkflow = SourceWorkflow.ContinuousDelivery
      };
      tfsRequestContext.TestResultsHttpClient.UpdateTestRunAsync(runUpdateModel, projectId, runId).SyncResult<TestRun>();
      logger.Verbose(6200302, "Completed TestManagement Update TestRun for project {0} with test run id {1}", (object) projectId, (object) runId);
    }

    private DistributedTestRun UpdateDistributedTestRunWithTraTestRun(
      TestExecutionRequestContext testRequestContext,
      Guid projectId,
      RunCreateModel runCreateModel,
      DistributedTestRun distributedRunModel)
    {
      IVssRequestContext requestContext1 = testRequestContext.RequestContext;
      DistributedTestRun distributedTestRun = new DistributedTestRun()
      {
        EnvironmentUri = runCreateModel.DtlTestEnvironment.Url,
        TestRunId = DtaConstants.FailedTestRun
      };
      Dictionary<string, object> eventData = new Dictionary<string, object>()
      {
        {
          "DtlEnvironmentUrl",
          (object) runCreateModel.DtlTestEnvironment.Url
        }
      };
      CILogger.Instance.PublishCI(testRequestContext, "AutomationRunQueued", eventData);
      try
      {
        if (distributedRunModel.TestRunId == 0)
        {
          distributedTestRun.TestRunId = this.CreateTraTestRun(testRequestContext, runCreateModel, projectId);
        }
        else
        {
          this.StartTraTestRun(testRequestContext, runCreateModel, projectId, distributedRunModel.TestRunId);
          distributedTestRun.TestRunId = distributedRunModel.TestRunId;
        }
        if (!string.Equals(runCreateModel.Type, "RunWithDtlEnv", StringComparison.OrdinalIgnoreCase))
        {
          ITestExecutionService service = requestContext1.GetService<ITestExecutionService>();
          TeamProjectReference projectReference = new TeamProjectReference()
          {
            Id = projectId
          };
          List<int> configIds = new List<int>();
          if (runCreateModel.ConfigurationIds != null && runCreateModel.ConfigurationIds.Length != 0)
            configIds = ((IEnumerable<int>) runCreateModel.ConfigurationIds).ToList<int>();
          TestRunDefinitionJob runDefinitionJob = new TestRunDefinitionJob(distributedTestRun.TestRunId, configIds, runCreateModel.RunTimeout, projectReference, runCreateModel.TestConfigurationsMapping, runCreateModel.DtlTestEnvironment.Url, runCreateModel.Filter, distributedRunModel.DistributedTestRunCreateModel.RunProperties);
          TestExecutionRequestContext requestContext2 = testRequestContext;
          TestRunDefinitionJob job = runDefinitionJob;
          service.SubmitTestExecutionJob(requestContext2, (ITestExecutionJob) job);
        }
        this.UpdateTestRun(testRequestContext, projectId, distributedTestRun);
      }
      catch (AggregateException ex)
      {
        this.HandledRunCreationFailure(testRequestContext, projectId, distributedTestRun, runCreateModel, ex.InnerException);
      }
      catch (Exception ex)
      {
        this.HandledRunCreationFailure(testRequestContext, projectId, distributedTestRun, runCreateModel, ex);
      }
      return distributedTestRun;
    }

    private void HandledRunCreationFailure(
      TestExecutionRequestContext testRequestContext,
      Guid projectId,
      DistributedTestRun distributedTestRun,
      RunCreateModel runCreateModel,
      Exception ex)
    {
      DistributedTestRunService.GetLogger(testRequestContext).Error(6200303, string.Format("DistributedTestRunService:CreateTestRun() failed to create Test Run: {0} for environment {1}", (object) ex, (object) distributedTestRun.EnvironmentUri));
      bool flag = true;
      switch (ex)
      {
        case InvalidOperationException _:
        case TestExecutionServiceInvalidOperationException _:
        case TestObjectNotFoundException _:
        case InvalidPropertyException _:
          flag = false;
          break;
      }
      Dictionary<string, object> eventData = new Dictionary<string, object>()
      {
        {
          "DtlEnvironmentUrl",
          (object) runCreateModel.DtlTestEnvironment.Url
        },
        {
          "Exception",
          (object) ex
        },
        {
          "ValidRunCreationInput",
          (object) flag
        },
        {
          "E2EId",
          (object) testRequestContext.E2EId
        },
        {
          "TestPointsCount",
          (object) (runCreateModel.PointIds == null ? 0 : runCreateModel.PointIds.Length)
        },
        {
          "BuildUrl",
          runCreateModel.Build == null ? (object) string.Empty : (object) runCreateModel.Build.Url
        }
      };
      CILogger.Instance.PublishCI(testRequestContext, "AutomationRunQueueFailure", eventData);
      if (!flag)
        throw new TestExecutionServiceInvalidOperationException(ex.Message, ex);
      this.DeleteTestRun(testRequestContext, projectId, distributedTestRun);
    }

    private static RunCreateModel GetRunCreateModel(DistributedTestRun distributedTestRun)
    {
      DistributedTestRunCreateModel testRunCreateModel = distributedTestRun.DistributedTestRunCreateModel;
      Microsoft.TeamFoundation.TestManagement.WebApi.RunFilter runFilter1 = new Microsoft.TeamFoundation.TestManagement.WebApi.RunFilter()
      {
        SourceFilter = testRunCreateModel.SourceFilter,
        TestCaseFilter = testRunCreateModel.TestCaseFilter
      };
      string name = testRunCreateModel.Name;
      string type1 = testRunCreateModel.Type;
      bool? automated = testRunCreateModel.Automated;
      Microsoft.TeamFoundation.TestManagement.WebApi.RunFilter runFilter2 = runFilter1;
      Microsoft.TeamFoundation.TestManagement.WebApi.ShallowReference shallowReference1 = new Microsoft.TeamFoundation.TestManagement.WebApi.ShallowReference()
      {
        Url = testRunCreateModel.EnvironmentUrl
      };
      Microsoft.TeamFoundation.TestManagement.WebApi.ShallowReference shallowReference2;
      if (testRunCreateModel.AutEnvironmentUrl == null)
      {
        shallowReference2 = (Microsoft.TeamFoundation.TestManagement.WebApi.ShallowReference) null;
      }
      else
      {
        shallowReference2 = new Microsoft.TeamFoundation.TestManagement.WebApi.ShallowReference();
        shallowReference2.Url = testRunCreateModel.AutEnvironmentUrl;
      }
      Microsoft.TeamFoundation.TestManagement.WebApi.ShallowReference shallowReference3 = shallowReference2;
      string buildDropLocation1 = testRunCreateModel.BuildDropLocation;
      Microsoft.TeamFoundation.TestManagement.WebApi.ShallowReference shallowReference4;
      if (testRunCreateModel.TestSettings == null)
      {
        shallowReference4 = (Microsoft.TeamFoundation.TestManagement.WebApi.ShallowReference) null;
      }
      else
      {
        shallowReference4 = new Microsoft.TeamFoundation.TestManagement.WebApi.ShallowReference();
        shallowReference4.Id = testRunCreateModel.TestSettings.Id.ToString();
      }
      Microsoft.TeamFoundation.TestManagement.WebApi.ShallowReference shallowReference5 = shallowReference4;
      string buildFlavor1 = testRunCreateModel.BuildFlavor;
      string buildPlatform1 = testRunCreateModel.BuildPlatform;
      Microsoft.TeamFoundation.Test.WebApi.ShallowReference build1 = testRunCreateModel.Build;
      int id = build1 != null ? build1.Id : 0;
      string configurationsMapping = testRunCreateModel.TestConfigurationsMapping;
      string releaseEnvironmentUri1 = testRunCreateModel.ReleaseEnvironmentUri;
      string releaseUri1 = testRunCreateModel.ReleaseUri;
      int[] pointIds = testRunCreateModel.PointIds;
      int[] configurationIds = testRunCreateModel.ConfigurationIds;
      Microsoft.TeamFoundation.TestManagement.WebApi.ShallowReference plan;
      if (testRunCreateModel.Plan == null)
      {
        plan = (Microsoft.TeamFoundation.TestManagement.WebApi.ShallowReference) null;
      }
      else
      {
        plan = new Microsoft.TeamFoundation.TestManagement.WebApi.ShallowReference();
        plan.Id = testRunCreateModel.Plan.Id.ToString();
      }
      Microsoft.TeamFoundation.TestManagement.WebApi.ShallowReference testSettings = shallowReference5;
      int buildId = id;
      bool? isAutomated = automated;
      string type2 = type1;
      string buildDropLocation2 = buildDropLocation1;
      int[] configIds = configurationIds;
      Microsoft.TeamFoundation.TestManagement.WebApi.RunFilter filter = runFilter2;
      Microsoft.TeamFoundation.TestManagement.WebApi.ShallowReference dtlTestEnvironment = shallowReference1;
      string buildPlatform2 = buildPlatform1;
      string buildFlavor2 = buildFlavor1;
      string releaseUri2 = releaseUri1;
      string releaseEnvironmentUri2 = releaseEnvironmentUri1;
      TimeSpan? runTimeout = new TimeSpan?(testRunCreateModel.RunTimeout);
      string testconfigurationsMapping = configurationsMapping;
      Microsoft.TeamFoundation.TestManagement.WebApi.ShallowReference dtlAutEnvironment = shallowReference3;
      RunCreateModel runCreateModel1 = new RunCreateModel(name, pointIds: pointIds, plan: plan, testSettings: testSettings, buildId: buildId, isAutomated: isAutomated, type: type2, buildDropLocation: buildDropLocation2, configIds: configIds, filter: filter, dtlTestEnvironment: dtlTestEnvironment, buildPlatform: buildPlatform2, buildFlavor: buildFlavor2, releaseUri: releaseUri2, releaseEnvironmentUri: releaseEnvironmentUri2, runTimeout: runTimeout, testconfigurationsMapping: testconfigurationsMapping, dtlAutEnvironment: dtlAutEnvironment);
      runCreateModel1.AddCustomTestField("TestRunSystem", (object) "VSTS - vstest");
      if (testRunCreateModel.TestReportingSettings != null && !string.IsNullOrEmpty(testRunCreateModel.TestReportingSettings.PullRequestTargetBranchName))
        runCreateModel1.BuildReference = new BuildConfiguration()
        {
          TargetBranchName = testRunCreateModel.TestReportingSettings.PullRequestTargetBranchName
        };
      if (testRunCreateModel.TfsSpecificProperties != null && testRunCreateModel.ReleaseUri == null)
      {
        ArgumentUtility.CheckForNonnegativeInt(testRunCreateModel.TfsSpecificProperties.StageAttempt, "Stage Attempt");
        ArgumentUtility.CheckForNonnegativeInt(testRunCreateModel.TfsSpecificProperties.PhaseAttempt, "Phase Attempt");
        ArgumentUtility.CheckForNonnegativeInt(testRunCreateModel.TfsSpecificProperties.JobAttempt, "Job Attempt");
        RunCreateModel runCreateModel2 = runCreateModel1;
        PipelineReference pipelineReference = new PipelineReference();
        Microsoft.TeamFoundation.Test.WebApi.ShallowReference build2 = testRunCreateModel.Build;
        pipelineReference.PipelineId = build2 != null ? build2.Id : 0;
        runCreateModel2.PipelineReference = pipelineReference;
        runCreateModel1.PipelineReference.StageReference = new StageReference()
        {
          StageName = testRunCreateModel.TfsSpecificProperties.StageName,
          Attempt = testRunCreateModel.TfsSpecificProperties.StageAttempt
        };
        runCreateModel1.PipelineReference.PhaseReference = new PhaseReference()
        {
          PhaseName = testRunCreateModel.TfsSpecificProperties.PhaseName,
          Attempt = testRunCreateModel.TfsSpecificProperties.PhaseAttempt
        };
        runCreateModel1.PipelineReference.JobReference = new JobReference()
        {
          JobName = testRunCreateModel.TfsSpecificProperties.JobName,
          Attempt = testRunCreateModel.TfsSpecificProperties.JobAttempt
        };
      }
      return runCreateModel1;
    }

    private static DtaLogger GetLogger(TestExecutionRequestContext tfsRequestContext) => new DtaLogger(tfsRequestContext, TestExecutionServiceConstants.TestExecutionServiceArea, TestExecutionServiceConstants.ServiceLayer);
  }
}
