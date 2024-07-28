// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestExecution.Server.TestRunPropertiesService
// Assembly: Microsoft.VisualStudio.Services.TE.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2BC2680F-A5FB-41BE-A4CF-F78BF7AC3E02
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.TE.Server.Common.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Test.WebApi;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;

namespace Microsoft.TeamFoundation.TestExecution.Server
{
  [CLSCompliant(false)]
  public class TestRunPropertiesService
  {
    public static TimeSpan GetTestRunTimeout(
      TestExecutionRequestContext requestContext,
      int testRunId)
    {
      requestContext.RequestContext.Trace(6200450, TraceLevel.Verbose, TestExecutionServiceConstants.TestExecutionServiceArea, TestExecutionServiceConstants.HelpersLayer, string.Format("Getting test run time out for testRunId {0}", (object) testRunId));
      IDictionary<string, object> dictionary = TestRunPropertiesService.PropertyServiceHelper.Get(requestContext, requestContext.AutomationRunArtifactKindId, testRunId, (IEnumerable<string>) null);
      TimeSpan testRunTimeout = TimeSpan.Zero;
      string automationRunTimeout = TestPropertiesConstants.AutomationRunTimeout;
      object s;
      ref object local = ref s;
      if (dictionary.TryGetValue(automationRunTimeout, out local))
      {
        int result;
        if (int.TryParse(s as string, out result))
          testRunTimeout = TimeSpan.FromSeconds((double) result);
      }
      else
        requestContext.RequestContext.Trace(6200451, TraceLevel.Warning, TestExecutionServiceConstants.TestExecutionServiceArea, TestExecutionServiceConstants.HelpersLayer, "Could not find the timeout stored for testRunId {0}.", (object) testRunId);
      requestContext.RequestContext.Trace(6200452, TraceLevel.Verbose, TestExecutionServiceConstants.TestExecutionServiceArea, TestExecutionServiceConstants.HelpersLayer, string.Format("Timeout stored for testRunId {0}.\nTimeout : {1}", (object) testRunId, (object) testRunTimeout));
      return testRunTimeout;
    }

    public static TeamProjectReference GetProjectReference(
      TestExecutionRequestContext requestContext,
      int testRunId)
    {
      requestContext.RequestContext.Trace(6200453, TraceLevel.Verbose, TestExecutionServiceConstants.TestExecutionServiceArea, TestExecutionServiceConstants.HelpersLayer, string.Format("Getting project name for testRunId {0}.", (object) testRunId));
      IDictionary<string, object> dictionary = TestRunPropertiesService.PropertyServiceHelper.Get(requestContext, requestContext.AutomationRunArtifactKindId, testRunId, (IEnumerable<string>) null);
      TeamProjectReference projectReference1 = (TeamProjectReference) null;
      string projectReference2 = TestPropertiesConstants.AutomationRunProjectReference;
      object input;
      ref object local = ref input;
      Guid result;
      if (dictionary.TryGetValue(projectReference2, out local) && Guid.TryParse(input as string, out result))
      {
        TeamProjectReference project = new TeamProjectReference()
        {
          Id = result
        };
        projectReference1 = Utilities.HydrateProjectReference(requestContext, project);
      }
      if (projectReference1 == null)
      {
        requestContext.RequestContext.Trace(6200454, TraceLevel.Error, TestExecutionServiceConstants.TestExecutionServiceArea, TestExecutionServiceConstants.HelpersLayer, string.Format("Could not find the Project Reference stored for testRunId {0}.", (object) testRunId));
        throw new TestExecutionObjectNotFoundException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, TestExecutionServiceResources.InvalidRunId, (object) testRunId), (Exception) null);
      }
      requestContext.RequestContext.Trace(6200455, TraceLevel.Verbose, TestExecutionServiceConstants.TestExecutionServiceArea, TestExecutionServiceConstants.HelpersLayer, string.Format("Found the project Guid stored for testRunId {0}.\nProjectName : {1}\nProjectGuid : {2}", (object) testRunId, (object) projectReference1.Name, (object) projectReference1.Id));
      return projectReference1;
    }

    public static TeamProjectReference GetProjectReferenceForTestagent(
      TestExecutionRequestContext requestContext,
      int agentId)
    {
      IDictionary<string, object> dictionary = TestRunPropertiesService.PropertyServiceHelper.Get(requestContext, requestContext.TestAgentArtifactKindId, agentId, (IEnumerable<string>) null);
      TeamProjectReference referenceForTestagent = (TeamProjectReference) null;
      string projectReference = TestPropertiesConstants.AutomationRunProjectReference;
      object input;
      ref object local = ref input;
      Guid result;
      if (dictionary.TryGetValue(projectReference, out local) && Guid.TryParse(input as string, out result))
      {
        TeamProjectReference project = new TeamProjectReference()
        {
          Id = result
        };
        referenceForTestagent = Utilities.HydrateProjectReference(requestContext, project);
      }
      if (referenceForTestagent == null)
      {
        requestContext.RequestContext.Trace(6200492, TraceLevel.Error, TestExecutionServiceConstants.TestExecutionServiceArea, TestExecutionServiceConstants.HelpersLayer, "Could not find the Project Reference stored for agentId {0}.", (object) agentId);
        throw new TestExecutionObjectNotFoundException(TestExecutionServiceResources.InvalidAgentId, (Exception) null);
      }
      requestContext.RequestContext.Trace(6200493, TraceLevel.Verbose, TestExecutionServiceConstants.TestExecutionServiceArea, TestExecutionServiceConstants.HelpersLayer, "Found the project Guid stored for agentId {0}.\nProjectName : {1}\nProjectGuid : {2}", (object) agentId, (object) referenceForTestagent.Name, (object) referenceForTestagent.Id);
      return referenceForTestagent;
    }

    public static TestRunProperties GetTestRunProperties(
      TestExecutionRequestContext requestContext,
      int testRunId)
    {
      DtaLogger logger = TestRunPropertiesService.GetLogger(requestContext);
      logger.Verbose(6200453, string.Format("Getting project name for testRunId {0}.", (object) testRunId));
      IDictionary<string, object> dictionary = TestRunPropertiesService.PropertyServiceHelper.Get(requestContext, requestContext.AutomationRunArtifactKindId, testRunId, (IEnumerable<string>) null);
      TestRunProperties testRunProperties1 = (TestRunProperties) null;
      string testRunProperties2 = TestPropertiesConstants.AutomationRunTestRunProperties;
      object json;
      ref object local = ref json;
      if (dictionary.TryGetValue(testRunProperties2, out local) && !string.IsNullOrWhiteSpace(json as string))
        testRunProperties1 = JsonUtilities.Deserialize<TestRunProperties>(json as string);
      if (testRunProperties1 == null)
      {
        logger.Error(6200454, string.Format("Could not find the run properties stored for testRunId {0}.", (object) testRunId));
        throw new TestExecutionObjectNotFoundException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, TestExecutionServiceResources.InvalidRunId, (object) testRunId), (Exception) null);
      }
      logger.Verbose(6200455, string.Format("Found the run properties stored for testRunId {0}", (object) testRunId));
      return testRunProperties1;
    }

    public static bool IsTestImpactOn(TestExecutionRequestContext requestContext, int testRunId)
    {
      requestContext.RequestContext.Trace(6200456, TraceLevel.Verbose, TestExecutionServiceConstants.TestExecutionServiceArea, TestExecutionServiceConstants.HelpersLayer, string.Format("Getting test impact property for testRunId {0}.", (object) testRunId));
      IDictionary<string, object> dictionary = TestRunPropertiesService.PropertyServiceHelper.Get(requestContext, requestContext.AutomationRunArtifactKindId, testRunId, (IEnumerable<string>) null);
      bool result = false;
      string runIsTestImpactOn = TestPropertiesConstants.AutomationRunIsTestImpactOn;
      object obj;
      ref object local = ref obj;
      if (dictionary.TryGetValue(runIsTestImpactOn, out local))
        bool.TryParse(obj as string, out result);
      requestContext.RequestContext.Trace(6200457, TraceLevel.Verbose, TestExecutionServiceConstants.TestExecutionServiceArea, TestExecutionServiceConstants.HelpersLayer, string.Format("Test impact enabled: {0} for testRunId {1} ", (object) result, (object) testRunId));
      return result;
    }

    public static string GetTestRunInformationSource(TestExecutionRequestContext context) => context.RequestContext.GetService<IVssRegistryService>().GetValue(context.RequestContext, (RegistryQuery) DtaConstants.TfsRegistryPathForTestRunInformation, true, string.Empty);

    public static int GetBaseLineDefinitionRunId(
      TestExecutionRequestContext requestContext,
      int testRunId)
    {
      requestContext.RequestContext.Trace(6200458, TraceLevel.Verbose, TestExecutionServiceConstants.TestExecutionServiceArea, TestExecutionServiceConstants.HelpersLayer, string.Format("Getting base line definition run id property for testRunId {0}.", (object) testRunId));
      IDictionary<string, object> dictionary = TestRunPropertiesService.PropertyServiceHelper.Get(requestContext, requestContext.AutomationRunArtifactKindId, testRunId, (IEnumerable<string>) null);
      int result = 0;
      string lineDefinitionRunId = TestPropertiesConstants.AutomationRunBaseLineDefinitionRunId;
      object obj;
      ref object local = ref obj;
      if (!dictionary.TryGetValue(lineDefinitionRunId, out local))
        return result;
      if (obj != null)
        int.TryParse(obj.ToString(), out result);
      requestContext.RequestContext.Trace(6200459, TraceLevel.Verbose, TestExecutionServiceConstants.TestExecutionServiceArea, TestExecutionServiceConstants.HelpersLayer, string.Format("Base line definition run id {0} for testRunId {1}.", (object) result, (object) testRunId));
      return result;
    }

    public static int GetRerunIterationCount(
      TestExecutionRequestContext requestContext,
      int testRunId)
    {
      requestContext.RequestContext.Trace(6200466, TraceLevel.Verbose, TestExecutionServiceConstants.TestExecutionServiceArea, TestExecutionServiceConstants.HelpersLayer, string.Format("Getting rerun iteration count for testRunId {0}.", (object) testRunId));
      IDictionary<string, object> dictionary = TestRunPropertiesService.PropertyServiceHelper.Get(requestContext, requestContext.AutomationRunArtifactKindId, testRunId, (IEnumerable<string>) null);
      int rerunIterationCount = 0;
      string automationRerunCount = TestPropertiesConstants.AutomationRerunCount;
      object obj;
      ref object local = ref obj;
      if (dictionary.TryGetValue(automationRerunCount, out local))
        rerunIterationCount = Convert.ToInt32(obj);
      requestContext.RequestContext.Trace(6200467, TraceLevel.Verbose, TestExecutionServiceConstants.TestExecutionServiceArea, TestExecutionServiceConstants.HelpersLayer, string.Format("Rerun iteration count: {0} for testRunId {1} ", (object) rerunIterationCount, (object) testRunId));
      return rerunIterationCount;
    }

    public static bool IsTestPlanScenario(TestExecutionRequestContext requestContext, int testRunId)
    {
      requestContext.RequestContext.Trace(6200468, TraceLevel.Verbose, TestExecutionServiceConstants.TestExecutionServiceArea, TestExecutionServiceConstants.HelpersLayer, string.Format("Getting IsTestPlanScenario check for testRunId {0}.", (object) testRunId));
      TeamProjectReference projectReference = TestRunPropertiesService.GetProjectReference(requestContext, testRunId);
      TestRun testRun = new TestManagementRunHelper().GetTestRun(requestContext, testRunId, projectReference, false);
      if (string.IsNullOrEmpty(testRun.Plan?.Id))
        return false;
      int result;
      int.TryParse(testRun.Plan.Id, out result);
      bool flag = result > 0;
      requestContext.RequestContext.Trace(6200469, TraceLevel.Verbose, TestExecutionServiceConstants.TestExecutionServiceArea, TestExecutionServiceConstants.HelpersLayer, string.Format("IsTestPlanScenario check for testRunId {0} returned {1}.", (object) testRunId, (object) flag));
      return flag;
    }

    public static bool IsOnDemandRun(TestExecutionRequestContext requestContext, int testRunId)
    {
      requestContext.Logger.Verbose(6200468, string.Format("Getting IsOnDemadRun check for testRunId {0}.", (object) testRunId));
      TeamProjectReference projectReference = TestRunPropertiesService.GetProjectReference(requestContext, testRunId);
      bool flag = string.Equals(requestContext.RequestContext.GetService<IDistributedTestRunService>().GetTestRun(requestContext, projectReference.Id, testRunId).DistributedTestRunCreateModel?.RunProperties?.TestRunType, "TestRun", StringComparison.OrdinalIgnoreCase);
      requestContext.Logger.Verbose(6200469, string.Format("IsOnDemadRun check for testRunId {0} returned {1}.", (object) testRunId, (object) flag));
      return flag;
    }

    public static bool IsCustomSlicingEnabled(
      TestExecutionRequestContext requestContext,
      int testRunId,
      out int maxAgents,
      out int numberOfTestCasesPerSlice)
    {
      requestContext.RequestContext.Trace(6200469, TraceLevel.Verbose, TestExecutionServiceConstants.TestExecutionServiceArea, TestExecutionServiceConstants.HelpersLayer, string.Format("Getting IsCustomSlicingEnabled check for testRunId {0}.", (object) testRunId));
      IDictionary<string, object> dictionary = TestRunPropertiesService.PropertyServiceHelper.Get(requestContext, requestContext.AutomationRunArtifactKindId, testRunId, (IEnumerable<string>) null);
      bool result = false;
      maxAgents = 0;
      numberOfTestCasesPerSlice = 0;
      object obj;
      if (dictionary.TryGetValue(TestPropertiesConstants.AutomationRunCustomSlicingEnabled, out obj))
        bool.TryParse(obj as string, out result);
      if (dictionary.TryGetValue(TestPropertiesConstants.AutomationRunCustomSlicingMaxAgents, out obj) && obj is int num1)
        maxAgents = num1;
      if (dictionary.TryGetValue(TestPropertiesConstants.AutomationRunCustomSlicingNumberOfTestCasesPerSlice, out obj) && obj is int num2)
        numberOfTestCasesPerSlice = num2;
      requestContext.RequestContext.Trace(6200470, TraceLevel.Verbose, TestExecutionServiceConstants.TestExecutionServiceArea, TestExecutionServiceConstants.HelpersLayer, string.Format("Getting IsCustomSlicingEnabled check for testRunId {0}: MaxAgents: {1} TestCasesPerSlice: {2}.", (object) testRunId, (object) maxAgents, (object) numberOfTestCasesPerSlice));
      return result;
    }

    public static int GetRerunTestCasesMaxThreshold(
      TestExecutionRequestContext requestContext,
      int testRunId)
    {
      requestContext.RequestContext.Trace(6200471, TraceLevel.Verbose, TestExecutionServiceConstants.TestExecutionServiceArea, TestExecutionServiceConstants.HelpersLayer, string.Format("Getting rerun threshold limit for testRunId {0}.", (object) testRunId));
      IDictionary<string, object> dictionary = TestRunPropertiesService.PropertyServiceHelper.Get(requestContext, requestContext.AutomationRunArtifactKindId, testRunId, (IEnumerable<string>) null);
      int casesMaxThreshold1 = 0;
      string casesMaxThreshold2 = TestPropertiesConstants.AutomationRerunTestCasesMaxThreshold;
      object obj;
      ref object local = ref obj;
      if (dictionary.TryGetValue(casesMaxThreshold2, out local))
        casesMaxThreshold1 = Convert.ToInt32(obj);
      requestContext.RequestContext.Trace(6200472, TraceLevel.Verbose, TestExecutionServiceConstants.TestExecutionServiceArea, TestExecutionServiceConstants.HelpersLayer, string.Format("Rerun threshold limit: {0} for testRunId {1}", (object) casesMaxThreshold1, (object) testRunId));
      return casesMaxThreshold1;
    }

    public static int GetRerunFailedTestCasesMaxLimit(
      TestExecutionRequestContext requestContext,
      int testRunId)
    {
      DtaLogger dtaLogger = new DtaLogger(requestContext);
      dtaLogger.Verbose(6200471, TestExecutionServiceConstants.TestExecutionServiceArea, (object) TestExecutionServiceConstants.HelpersLayer, (object) string.Format("Getting rerun failed test cases max limit for testRunId {0}.", (object) testRunId));
      IDictionary<string, object> dictionary = TestRunPropertiesService.PropertyServiceHelper.Get(requestContext, requestContext.AutomationRunArtifactKindId, testRunId, (IEnumerable<string>) null);
      int testCasesMaxLimit1 = 0;
      string testCasesMaxLimit2 = TestPropertiesConstants.AutomationRerunFailedTestCasesMaxLimit;
      object obj;
      ref object local = ref obj;
      if (dictionary.TryGetValue(testCasesMaxLimit2, out local))
        testCasesMaxLimit1 = Convert.ToInt32(obj);
      dtaLogger.Verbose(6200472, TestExecutionServiceConstants.TestExecutionServiceArea, (object) TestExecutionServiceConstants.HelpersLayer, (object) string.Format("Rerun failed test cases max limit: {0} for testRunId {1}", (object) testCasesMaxLimit1, (object) testRunId));
      return testCasesMaxLimit1;
    }

    public static int GetRerunCurrentIteration(
      TestExecutionRequestContext requestContext,
      int testRunId)
    {
      requestContext.RequestContext.Trace(6200473, TraceLevel.Verbose, TestExecutionServiceConstants.TestExecutionServiceArea, TestExecutionServiceConstants.HelpersLayer, string.Format("Getting rerun iteration count for testRunId {0}.", (object) testRunId));
      IDictionary<string, object> dictionary = TestRunPropertiesService.PropertyServiceHelper.Get(requestContext, requestContext.AutomationRunArtifactKindId, testRunId, (IEnumerable<string>) null);
      int currentIteration = 0;
      string automationRerunIteration = TestPropertiesConstants.AutomationRerunIteration;
      object obj;
      ref object local = ref obj;
      if (dictionary.TryGetValue(automationRerunIteration, out local))
        currentIteration = Convert.ToInt32(obj);
      requestContext.RequestContext.Trace(6200474, TraceLevel.Verbose, TestExecutionServiceConstants.TestExecutionServiceArea, TestExecutionServiceConstants.HelpersLayer, string.Format("Rerun current iteration: {0} for testRunId {1} ", (object) currentIteration, (object) testRunId));
      return currentIteration;
    }

    public static int GetTestConfigurationId(
      TestExecutionRequestContext requestContext,
      int testRunId)
    {
      requestContext.Logger.Verbose(6200474, string.Format("GetConfigurationIdsFromPropertyStore for testRunId {0}.", (object) testRunId));
      object obj;
      if (TestRunPropertiesService.PropertyServiceHelper.Get(requestContext, requestContext.AutomationRunArtifactKindId, testRunId, (IEnumerable<string>) null).TryGetValue(TestPropertiesConstants.AutomationRunConfigurations, out obj))
      {
        requestContext.Logger.Verbose(6200475, string.Format("Found the config id stored for testRunId {0}.Config Id : {1}", (object) testRunId, obj));
        return Convert.ToInt32(obj);
      }
      requestContext.Logger.Verbose(6200476, string.Format("No config id stored for testRunId {0}", (object) testRunId));
      return -1;
    }

    public static bool IsTimeBasedSlicingEnabled(
      TestExecutionRequestContext requestContext,
      int testRunId,
      out int sliceTime)
    {
      requestContext.RequestContext.Trace(6200475, TraceLevel.Verbose, TestExecutionServiceConstants.TestExecutionServiceArea, TestExecutionServiceConstants.HelpersLayer, string.Format("Getting IsTimeBasedSlicingEnabled for testRunId {0}.", (object) testRunId));
      IDictionary<string, object> dictionary = TestRunPropertiesService.PropertyServiceHelper.Get(requestContext, requestContext.AutomationRunArtifactKindId, testRunId, (IEnumerable<string>) null);
      bool result = false;
      sliceTime = 0;
      object obj;
      if (dictionary.TryGetValue(TestPropertiesConstants.AutomationRunCustomSlicingIsTimeBasedSlicing, out obj))
        bool.TryParse(obj as string, out result);
      if (dictionary.TryGetValue(TestPropertiesConstants.AutomationRunCustomSlicingSliceTime, out obj) && obj is int num)
        sliceTime = num;
      requestContext.RequestContext.Trace(62004746, TraceLevel.Verbose, TestExecutionServiceConstants.TestExecutionServiceArea, TestExecutionServiceConstants.HelpersLayer, string.Format("IsTimeBasedSlicingEnabled {0} for testRunId {1}.", (object) result, (object) testRunId));
      return result;
    }

    public static void UpdateRerunIterationCount(
      TestExecutionRequestContext requestContext,
      int testRunId,
      int iterationCount)
    {
      requestContext.RequestContext.Trace(6200476, TraceLevel.Verbose, TestExecutionServiceConstants.TestExecutionServiceArea, TestExecutionServiceConstants.HelpersLayer, string.Format("Getting UpdateRerunIterationCount for testRunId {0} count {1}.", (object) testRunId, (object) iterationCount));
      Dictionary<string, object> properties = new Dictionary<string, object>()
      {
        {
          TestPropertiesConstants.AutomationRerunIteration,
          (object) iterationCount
        }
      };
      TestRunPropertiesService.PropertyServiceHelper.AddOrUpdate(requestContext, requestContext.AutomationRunArtifactKindId, testRunId, (IDictionary<string, object>) properties);
      requestContext.RequestContext.Trace(62004747, TraceLevel.Verbose, TestExecutionServiceConstants.TestExecutionServiceArea, TestExecutionServiceConstants.HelpersLayer, string.Format("UpdateRerunIterationCount completed for testRunId {0}.", (object) testRunId));
    }

    public static string GetEnvironmentUrl(
      TestExecutionRequestContext requestContext,
      int testRunId)
    {
      requestContext.RequestContext.Trace(6200478, TraceLevel.Verbose, TestExecutionServiceConstants.TestExecutionServiceArea, TestExecutionServiceConstants.HelpersLayer, string.Format("Getting EnvironmentUrl for testRunId {0}.", (object) testRunId));
      IDictionary<string, object> dictionary = TestRunPropertiesService.PropertyServiceHelper.Get(requestContext, requestContext.AutomationRunArtifactKindId, testRunId, (IEnumerable<string>) null);
      string environmentUrl = (string) null;
      string runEnvironmentUrl = TestPropertiesConstants.AutomationRunEnvironmentUrl;
      object obj;
      ref object local = ref obj;
      if (dictionary.TryGetValue(runEnvironmentUrl, out local))
        environmentUrl = obj.ToString();
      requestContext.RequestContext.Trace(6200479, TraceLevel.Verbose, TestExecutionServiceConstants.TestExecutionServiceArea, TestExecutionServiceConstants.HelpersLayer, string.Format("Getting Environment Url completed for testRunId {0}.", (object) testRunId));
      return environmentUrl;
    }

    public static string GetSourceFilter(TestExecutionRequestContext requestContext, int testRunId)
    {
      requestContext.RequestContext.Trace(6200480, TraceLevel.Verbose, TestExecutionServiceConstants.TestExecutionServiceArea, TestExecutionServiceConstants.HelpersLayer, string.Format("Getting Source Filter for testRunId {0}.", (object) testRunId));
      IDictionary<string, object> dictionary = TestRunPropertiesService.PropertyServiceHelper.Get(requestContext, requestContext.AutomationRunArtifactKindId, testRunId, (IEnumerable<string>) null);
      string sourceFilter = (string) null;
      string automationRunSourceFilter = TestPropertiesConstants.AutomationRunSourceFilter;
      object obj;
      ref object local = ref obj;
      if (dictionary.TryGetValue(automationRunSourceFilter, out local))
        sourceFilter = obj.ToString();
      requestContext.RequestContext.Trace(6200481, TraceLevel.Verbose, TestExecutionServiceConstants.TestExecutionServiceArea, TestExecutionServiceConstants.HelpersLayer, string.Format("Getting Source Filter completed for testRunId {0}.", (object) testRunId));
      return sourceFilter;
    }

    public static string GetTestCaseFilter(
      TestExecutionRequestContext requestContext,
      int testRunId)
    {
      requestContext.RequestContext.Trace(6200482, TraceLevel.Verbose, TestExecutionServiceConstants.TestExecutionServiceArea, TestExecutionServiceConstants.HelpersLayer, string.Format("Getting Test Caser Filter for testRunId {0}.", (object) testRunId));
      IDictionary<string, object> dictionary = TestRunPropertiesService.PropertyServiceHelper.Get(requestContext, requestContext.AutomationRunArtifactKindId, testRunId, (IEnumerable<string>) null);
      string testCaseFilter = (string) null;
      string runTestCaseFilter = TestPropertiesConstants.AutomationRunTestCaseFilter;
      object obj;
      ref object local = ref obj;
      if (dictionary.TryGetValue(runTestCaseFilter, out local))
        testCaseFilter = obj.ToString();
      requestContext.RequestContext.Trace(6200483, TraceLevel.Verbose, TestExecutionServiceConstants.TestExecutionServiceArea, TestExecutionServiceConstants.HelpersLayer, string.Format("Getting Test Case Filter completed for testRunId {0}.", (object) testRunId));
      return testCaseFilter;
    }

    public static void CleanupTestRunInformation(
      TestExecutionRequestContext requestContext,
      int testRunId)
    {
      requestContext.RequestContext.Trace(6200477, TraceLevel.Verbose, TestExecutionServiceConstants.TestExecutionServiceArea, TestExecutionServiceConstants.HelpersLayer, string.Format("CleanupTestRunInformation for testRunId {0}", (object) testRunId));
      TestRunPropertiesService.PropertyServiceHelper.Delete(requestContext, requestContext.AutomationRunArtifactKindId, TestRunPropertiesService.GetEnvironmentUrl(requestContext, testRunId));
      TestRunPropertiesService.PropertyServiceHelper.Delete(requestContext, requestContext.AutomationRunArtifactKindId, testRunId);
      requestContext.RequestContext.Trace(62004748, TraceLevel.Verbose, TestExecutionServiceConstants.TestExecutionServiceArea, TestExecutionServiceConstants.HelpersLayer, string.Format("CleanupTestRunInformation completed for testRunId {0}.", (object) testRunId));
    }

    private static DtaLogger GetLogger(TestExecutionRequestContext tfsRequestContext) => new DtaLogger(tfsRequestContext, TestExecutionServiceConstants.TestExecutionServiceArea, TestExecutionServiceConstants.HelpersLayer);

    public static IPropertyServiceHelper PropertyServiceHelper { get; set; } = (IPropertyServiceHelper) new Microsoft.TeamFoundation.TestExecution.Server.PropertyServiceHelper();
  }
}
