// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestExecution.Server.TestRunExecutionConfigurationService
// Assembly: Microsoft.VisualStudio.Services.TE.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2BC2680F-A5FB-41BE-A4CF-F78BF7AC3E02
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.TE.Server.Common.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Test.WebApi;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.TestExecution.Server
{
  [CLSCompliant(false)]
  public class TestRunExecutionConfigurationService : 
    ITestRunExecutionConfigurationService,
    IVssFrameworkService
  {
    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public TestRunExecutionConfiguration GetRerunConfiguration(
      TestExecutionRequestContext requestContext,
      TestRunExecutionConfiguration testRunExecutionConfiguration)
    {
      if (requestContext.RequestContext.GetService<ITeamFoundationFeatureAvailabilityService>().GetFeatureInformation(requestContext.RequestContext, "TestExecution.RerunFailedTests").EffectiveState == FeatureAvailabilityState.Off)
        throw new FeatureDisabledException("Rerunning failed tests is disabled currently.");
      ArgumentUtility.CheckForNull<TestRunExecutionConfiguration>(testRunExecutionConfiguration, "Test Run Execution Configuration Model");
      ArgumentUtility.CheckStringForNullOrWhiteSpace(testRunExecutionConfiguration.ProjectName, "Project Name");
      ArgumentUtility.CheckGreaterThanZero((float) testRunExecutionConfiguration.TestRunId, "Test Run Id");
      ArgumentUtility.CheckForNull<RerunProperties>(testRunExecutionConfiguration.RerunProperties, "Rerun properties");
      if (testRunExecutionConfiguration.RerunProperties.RerunFailedThreshold <= 0 && testRunExecutionConfiguration.RerunProperties.RerunFailedTestCasesMaxLimit <= 0)
        throw new ArgumentException(TestExecutionServiceResources.InvalidRerunProperties);
      DtaLogger logger = this.GetLogger(requestContext);
      string projectName = testRunExecutionConfiguration.ProjectName;
      int testRunId = testRunExecutionConfiguration.TestRunId;
      logger.Verbose(6200352, TestExecutionServiceConstants.TestExecutionServiceArea, (object) TestExecutionServiceConstants.ServiceLayer, (object) string.Format("TestRunExecutionConfigurationService:GetRerunConfiguration() called with project {0} for test run {1}", (object) projectName, (object) testRunId));
      this.ValidateRerunTests(requestContext, testRunExecutionConfiguration);
      logger.Verbose(6200353, TestExecutionServiceConstants.TestExecutionServiceArea, (object) TestExecutionServiceConstants.ServiceLayer, (object) (string.Format("TestRunExecutionConfigurationService:GetRerunConfiguration() completed with project {0} for test run {1}. ", (object) projectName, (object) testRunId) + string.Format("Result: {0}", (object) testRunExecutionConfiguration.RerunProperties.AllowRerunFailedTests)));
      return testRunExecutionConfiguration;
    }

    private void ValidateRerunTests(
      TestExecutionRequestContext requestContext,
      TestRunExecutionConfiguration model)
    {
      DtaLogger logger = this.GetLogger(requestContext);
      string projectName = model.ProjectName;
      int testRunId1 = model.TestRunId;
      RerunProperties rerunProperties = model.RerunProperties;
      int currentIteration = rerunProperties.CurrentIteration;
      int rerunIterationCount = rerunProperties.RerunIterationCount;
      Dictionary<string, object> eventData1 = new Dictionary<string, object>()
      {
        {
          "TcmRunId",
          (object) testRunId1
        },
        {
          "RerunThresholdLimit",
          (object) rerunProperties.RerunFailedThreshold
        },
        {
          "RerunFailedTestCasesMaxLimit",
          (object) rerunProperties.RerunFailedTestCasesMaxLimit
        },
        {
          "RerunCurrentIteration",
          (object) rerunProperties.CurrentIteration
        }
      };
      CILogger.Instance.PublishCI(requestContext, "TestRunExecutionConfiguration", eventData1);
      if (rerunIterationCount == 0)
        model.RerunProperties.AllowRerunFailedTests = false;
      else if (currentIteration >= rerunIterationCount)
      {
        Dictionary<string, object> eventData2 = new Dictionary<string, object>()
        {
          {
            "TcmRunId",
            (object) testRunId1
          },
          {
            "RerunStatus.ReachedThreshold",
            (object) true
          },
          {
            "RerunAllowed",
            (object) false
          }
        };
        CILogger.Instance.PublishCI(requestContext, "TestRunExecutionConfiguration", eventData2);
        model.RerunProperties.AllowRerunFailedTests = false;
      }
      else
      {
        ITestManagementResultHelper managementResultHelper = this.TestManagementResultHelper;
        TestExecutionRequestContext requestContext1 = requestContext;
        TeamProjectReference projectReference = new TeamProjectReference();
        projectReference.Name = projectName;
        int testRunId2 = testRunId1;
        List<TestCaseResult> source = managementResultHelper.QueryTestResultsByRun(requestContext1, projectReference, testRunId2);
        if (source == null)
        {
          model.RerunProperties.AllowRerunFailedTests = false;
        }
        else
        {
          int count = source.Count;
          List<TestCaseResult> list = source.Where<TestCaseResult>((Func<TestCaseResult, bool>) (testCase =>
          {
            TestOutcome result;
            if (!Enum.TryParse<TestOutcome>(testCase.Outcome, out result))
              return false;
            return result == TestOutcome.Failed || result == TestOutcome.Aborted;
          })).ToList<TestCaseResult>();
          int rerunFailedThreshold = rerunProperties.RerunFailedThreshold;
          int testCasesMaxLimit = rerunProperties.RerunFailedTestCasesMaxLimit;
          model.RerunProperties.FailedTestCasesCount = list.Count;
          model.RerunProperties.TotalTestCasesCount = count;
          model.RerunProperties.RerunFailedTestCasesMaxLimit = testCasesMaxLimit;
          logger.Verbose(6200352, TestExecutionServiceConstants.TestExecutionServiceArea, (object) TestExecutionServiceConstants.ServiceLayer, (object) (string.Format("Rerun validation phase result for TestRunId:{0} ", (object) testRunId1) + string.Format("Total Testcases: {0} Failed Testcases: {1} ", (object) count, (object) list) + string.Format("rerun Iteration: {0} rerunThreshold: {1}", (object) currentIteration, (object) rerunFailedThreshold) + string.Format("rerunFailedTestCasesMaxLimit: {0}", (object) testCasesMaxLimit)));
          if (list.Count == 0)
          {
            Dictionary<string, object> eventData3 = new Dictionary<string, object>()
            {
              {
                "TcmRunId",
                (object) testRunId1
              },
              {
                "RerunStatus.NoFailTestCases",
                (object) true
              },
              {
                "RerunAllowed",
                (object) false
              }
            };
            CILogger.Instance.PublishCI(requestContext, "TestRunExecutionConfiguration", eventData3);
            model.RerunProperties.AllowRerunFailedTests = false;
          }
          else
          {
            if (rerunFailedThreshold > 0)
              model.RerunProperties.AllowRerunFailedTests = list.Count * 100 / count <= rerunFailedThreshold;
            else if (testCasesMaxLimit > 0)
              model.RerunProperties.AllowRerunFailedTests = list.Count <= testCasesMaxLimit;
            Dictionary<string, object> eventData4 = new Dictionary<string, object>()
            {
              {
                "TcmRunId",
                (object) testRunId1
              },
              {
                "FailedTestCasesCount",
                (object) list.Count
              },
              {
                "TotalTestCasesCount",
                (object) count
              },
              {
                "RerunAllowed",
                (object) model.RerunProperties.AllowRerunFailedTests
              }
            };
            CILogger.Instance.PublishCI(requestContext, "TestRunExecutionConfiguration", eventData4);
          }
        }
      }
    }

    private DtaLogger GetLogger(TestExecutionRequestContext tfsRequestContext) => new DtaLogger(tfsRequestContext, TestExecutionServiceConstants.TestExecutionServiceArea, TestExecutionServiceConstants.ServiceLayer);

    public ITestManagementResultHelper TestManagementResultHelper { get; set; } = (ITestManagementResultHelper) new Microsoft.TeamFoundation.TestExecution.Server.TestManagementResultHelper();
  }
}
