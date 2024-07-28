// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestExecution.Server.TestExecutionService
// Assembly: Microsoft.VisualStudio.Services.TE.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2BC2680F-A5FB-41BE-A4CF-F78BF7AC3E02
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.TE.Server.Common.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Test.WebApi;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Microsoft.TeamFoundation.TestExecution.Server
{
  [CLSCompliant(false)]
  public class TestExecutionService : ITestExecutionService, IVssFrameworkService
  {
    private ITestManagementRunHelper _testManagementRunHelper;
    private IPropertyServiceHelper _propertyServiceHelper;
    private ITcmLogger _tcmLogger;
    private ITestExecutionServiceJobHelper _testExecutionServiceJobHelper;
    private ITestManagementConfigurationHelper _configurationHelper;

    public void ServiceStart(IVssRequestContext context)
    {
    }

    public void ServiceEnd(IVssRequestContext context)
    {
    }

    public virtual int SubmitTestExecutionJob(
      TestExecutionRequestContext requestContext,
      ITestExecutionJob job)
    {
      DtaLogger logger = this.GetLogger(requestContext);
      try
      {
        return this.SubmitJobInternal(requestContext, job);
      }
      catch (Exception ex)
      {
        logger.Error(6200210, string.Format("SubmitTestExecutionJob: Unhandled Exception: {0}", (object) ex));
        throw;
      }
    }

    public IEnumerable<TestAutomationRunSlice> QuerySlicesByTestRunId(
      TestExecutionRequestContext requestContext,
      int testRunId)
    {
      using (DtaSliceDatabase component = requestContext.RequestContext.CreateComponent<DtaSliceDatabase>())
        return component.QuerySlicesByTestRunId(testRunId);
    }

    public TestAutomationRunSlice QuerySliceBySliceId(
      TestExecutionRequestContext requestContext,
      int sliceId)
    {
      using (DtaSliceDatabase component = requestContext.RequestContext.CreateComponent<DtaSliceDatabase>())
        return component.QuerySliceById(sliceId);
    }

    public TestAutomationRunSlice GetSlice(
      TestExecutionRequestContext requestContext,
      int testAgentId)
    {
      DtaLogger logger = this.GetLogger(requestContext);
      logger.Verbose(6200401, string.Format("Test Agent {0} has requested for a new slice.", (object) testAgentId));
      int testRunId = 0;
      TestAgent testAgent;
      try
      {
        TeamProjectReference referenceForTestagent = TestRunPropertiesService.GetProjectReferenceForTestagent(requestContext, testAgentId);
        Utilities.GetProjectUri(requestContext, referenceForTestagent.Name);
        using (DtaAgentDatabase component = requestContext.RequestContext.CreateComponent<DtaAgentDatabase>())
        {
          testAgent = component.QueryTestAgent(testAgentId);
          if (testAgent != null)
            testRunId = testAgent.TestRunId;
        }
      }
      catch (TestExecutionObjectNotFoundException ex)
      {
        logger.Error(6200402, string.Format("The requested test agent is not found. Either it never registered, or it unregistered itself. testAgentId : {0}. Exception: {1}", (object) testAgentId, (object) ex));
        return Utilities.GetSliceToStopAgent();
      }
      if (testRunId <= 0)
        return (TestAutomationRunSlice) null;
      try
      {
        TestAutomationRunSlice slice1;
        using (DtaSliceDatabase component = requestContext.RequestContext.CreateComponent<DtaSliceDatabase>())
        {
          slice1 = component.GetSlice(testAgentId, (string) null, testRunId);
          if (slice1 == null)
          {
            logger.Warning(6200403, string.Format("No slice found to return for testagent {0} for run {1}", (object) testAgentId, (object) testRunId));
            return (TestAutomationRunSlice) null;
          }
        }
        if (slice1.Status == AutomatedTestRunSliceStatus.Aborted)
        {
          logger.Warning(6200404, "Aborting the slice and invoking workflow as Maximum retryCount has been exhausted for sliceId {0}. TestRunId : {1}", (object) slice1.Id, (object) testRunId);
          Dictionary<string, object> eventData = new Dictionary<string, object>()
          {
            {
              "SliceAbortReason",
              (object) "RetryCountExhausted"
            },
            {
              "TcmRunId",
              (object) slice1.TestRunInformation.TcmRun.Id
            },
            {
              "DtlEnvironmentUrl",
              (object) testAgent.DtlEnvironment.Url
            },
            {
              "SliceId",
              (object) slice1.Id
            }
          };
          CILogger.Instance.PublishCI(requestContext, "SliceAborted", eventData);
          this.ScheduleWorkflowJobOnSlicesTermination(requestContext, testRunId);
          return (TestAutomationRunSlice) null;
        }
        TestAutomationRunSlice slice2 = this.ValidateAndPopulateSliceDetails(requestContext, slice1);
        Dictionary<string, object> eventData1 = new Dictionary<string, object>()
        {
          {
            "TcmRunId",
            (object) slice2.TestRunInformation.TcmRun.Id
          },
          {
            "SliceType",
            (object) slice2.Type.ToString()
          },
          {
            "SliceId",
            (object) slice2.Id
          },
          {
            "DtlEnvironmentUrl",
            (object) testAgent.DtlEnvironment.Url
          },
          {
            "AgentId",
            (object) testAgentId
          },
          {
            "IsTestRunComplete",
            (object) slice2.TestRunInformation.IsTestRunComplete
          }
        };
        CILogger.Instance.PublishCI(requestContext, "SliceQueued", eventData1);
        return slice2;
      }
      catch (TestExecutionObjectNotFoundException ex)
      {
        logger.Warning(6200405, string.Format("Throwing TestExecutionObjectNotFoundException as TestExecutionServiceInvalidOperation for Agent : {0} : TestRunId : {1}", (object) testAgentId, (object) testRunId));
        throw new TestExecutionServiceInvalidOperationException(ex.Message, (Exception) ex);
      }
    }

    public void CancelTestRun(
      TestExecutionRequestContext requestContext,
      int testRunId,
      Guid cancelledBy)
    {
      this.GetLogger(requestContext).Verbose(6200406, "AbortTestRun requested for TestRunId:{0}.", (object) testRunId);
      this.AbortTestWorkflowJob(requestContext, testRunId, cancelledBy);
    }

    public virtual void UpdateSlice(
      TestExecutionRequestContext requestContext,
      TestAutomationRunSlice sliceUpdatePackage)
    {
      DtaLogger logger = this.GetLogger(requestContext);
      TestRunInformation testRunInformation = sliceUpdatePackage.TestRunInformation;
      try
      {
        TeamProjectReference projectReference = TestRunPropertiesService.GetProjectReference(requestContext, testRunInformation.TcmRun.Id);
        this.AddSliceLogsToTcm(requestContext, sliceUpdatePackage, projectReference);
        logger.Info(6200407, "Got update for slice. SliceId : {0}. TestRunId : {1}", (object) sliceUpdatePackage.Id, (object) testRunInformation.TcmRun.Id);
        string projectUri = Utilities.GetProjectUri(requestContext, projectReference.Name);
        requestContext.SecurityManager.CheckPermission(requestContext, DTAPermissionType.SlicesUpdate, projectUri);
        this.ValidateSliceUpdatePackage(sliceUpdatePackage, requestContext);
        using (DtaSliceDatabase component = requestContext.RequestContext.CreateComponent<DtaSliceDatabase>())
          component.UpdateSlice(sliceUpdatePackage);
        int id = sliceUpdatePackage.TestRunInformation.TcmRun.Id;
        switch (sliceUpdatePackage.Status)
        {
          case AutomatedTestRunSliceStatus.Completed:
          case AutomatedTestRunSliceStatus.Aborted:
          case AutomatedTestRunSliceStatus.Cancelled:
            this.ScheduleWorkflowJobOnSlicesTermination(requestContext, id);
            break;
        }
      }
      catch (Exception ex)
      {
        TestRun testRun = this.TestManagementRunHelper.GetTestRun(requestContext, testRunInformation.TcmRun.Id);
        Dictionary<string, object> eventData = new Dictionary<string, object>()
        {
          {
            "SliceUpdateFailed",
            (object) ex
          },
          {
            "TestRunState",
            (object) testRun.State
          },
          {
            "TcmRunId",
            (object) testRunInformation.TcmRun.Id
          },
          {
            "SliceId",
            (object) sliceUpdatePackage
          }
        };
        CILogger.Instance.PublishCI(requestContext, "SliceUpdateFailed", eventData);
        logger.Error(6200427, "Update failed for slice. SliceId : {0}. TestRunId : {1}. Exception: {2}", (object) sliceUpdatePackage.Id, (object) testRunInformation.TcmRun.Id, (object) ex);
        if (string.Equals(testRun.State, "Aborted", StringComparison.OrdinalIgnoreCase))
          return;
        throw;
      }
    }

    public void QueueSlices(
      TestExecutionRequestContext requestContext,
      int testRunId,
      List<TestAutomationRunSlice> slices)
    {
      DtaLogger logger = this.GetLogger(requestContext);
      logger.Info(6200408, "QueueSlices requested for TestRunId:{0}.", (object) testRunId);
      using (DtaSliceDatabase component = requestContext.RequestContext.CreateComponent<DtaSliceDatabase>())
        component.QueueSlices(slices);
      logger.Info(6200409, "QueueSlices completed for TestRunId:{0}.", (object) testRunId);
    }

    private int SubmitJobInternal(TestExecutionRequestContext requestContext, ITestExecutionJob job)
    {
      int testRunId1 = -1;
      DtaLogger logger = this.GetLogger(requestContext);
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine(string.Format((IFormatProvider) CultureInfo.CurrentCulture, TestExecutionServiceResources.NewTestRunSubmitted));
      this.ValidateJob(job);
      Dictionary<string, object> eventData1 = new Dictionary<string, object>()
      {
        {
          "JobDetails",
          (object) job.ToString()
        }
      };
      CILogger.Instance.PublishCI(requestContext, "AutomationRunJobDetails", eventData1);
      if (!(job is TestRunDefinitionJob runDefinition))
      {
        logger.Error(6200413, string.Format("Received an invalid job. {0}", (object) job));
        throw new ArgumentException(TestExecutionServiceResources.InvalidJobDefinition, nameof (job));
      }
      logger.Info(6200410, "Received a new job with details : {0}", (object) job.ToString());
      TeamProjectReference projectReference = this.ValidateProjectInfo(requestContext, runDefinition.ProjectReference);
      string projectUri = Utilities.GetProjectUri(requestContext, projectReference.Name);
      try
      {
        requestContext.SecurityManager.CheckPermission(requestContext, DTAPermissionType.AutomationRunsCreate, projectUri);
      }
      catch (Exception ex)
      {
        stringBuilder.AppendLine(string.Format((IFormatProvider) CultureInfo.CurrentCulture, TestExecutionServiceResources.CheckPermissionFailed, (object) ex.Message));
        this.TcmLogger.AddLogToTcmRun(requestContext, testRunId1, projectReference, stringBuilder.ToString());
        throw;
      }
      int testRunId2 = runDefinition.TestRunId;
      try
      {
        this.StoreAutomationRunProperties(requestContext, runDefinition, projectReference);
        TestRun testRun = this.TestManagementRunHelper.GetTestRun(requestContext, testRunId2, projectReference, false);
        TestSettings testSettings = this.TestManagementRunHelper.GetTestSettings(requestContext, testRun);
        TestRunProperties testRunProperties = runDefinition.RunProperties != null ? new TestRunProperties(testRun, runDefinition.RunProperties) : new TestRunProperties(testRun, testSettings);
        this.StoreTestRunPropertiesForAutomationRun(requestContext, testRunId2, testRunProperties);
        string str1 = string.Empty;
        string str2 = string.Empty;
        if (testRun.Filter != null)
        {
          str1 = testRun.Filter.SourceFilter;
          str2 = testRun.Filter.TestCaseFilter;
        }
        Guid guid = this.JobHelper.QueueWorkflowJob(requestContext, testRunId2, projectReference);
        if (testRun.Owner != null)
          stringBuilder.AppendLine(string.Format((IFormatProvider) CultureInfo.CurrentCulture, TestExecutionServiceResources.TestRunQueued, (object) testRun.Owner.DisplayName));
        this.TcmLogger.AddLogToTcmRun(requestContext, testRunId2, projectReference, stringBuilder.ToString());
        logger.Verbose(6200411, "Queued the run corresponding to the job. job.TcmRunId : {0}", (object) testRun.Id);
        Dictionary<string, object> eventData2 = new Dictionary<string, object>()
        {
          {
            "TcmRunId",
            (object) testRunId2
          },
          {
            "RunTimeoutInSecs",
            (object) runDefinition.RunTimeout.TotalSeconds
          },
          {
            "SettingsType",
            (object) testRunProperties.SettingsType
          },
          {
            "SourceFilter",
            (object) str1
          },
          {
            "TestCaseFilter",
            (object) (!string.IsNullOrWhiteSpace(str2)).ToString()
          },
          {
            "CustomConfigProvided",
            (object) (!string.IsNullOrWhiteSpace(runDefinition.TestConfigurationsMapping)).ToString()
          },
          {
            "BuildUrl",
            testRun.Build == null ? (object) string.Empty : (object) testRun.Build.Url
          },
          {
            "CustomSlicingEnabled",
            (object) testRunProperties.IsCustomSlicingEnabled
          },
          {
            "CustomSlicing-MaxNumberOfAgents",
            (object) testRunProperties.MaxAgents
          },
          {
            "CustomSlicing-NumberOfTestCasesPerSlice",
            (object) testRunProperties.NumberOfTestCasesPerSlice
          },
          {
            "CustomSlicing-TimeBasedSlicingEnabled",
            (object) testRunProperties.IsTimeBasedSlicing
          },
          {
            "CustomSlicing-SliceTime",
            (object) testRunProperties.SliceTime
          },
          {
            "JobId",
            (object) guid.ToString()
          }
        };
        CILogger.Instance.PublishCI(requestContext, "AutomationRunQueued", eventData2);
      }
      catch (Exception ex)
      {
        logger.Error(6200412, string.Format("Failed to queue the test run workflow job for testrun : {0}: Exception : {1}", (object) testRunId2, (object) ex));
        Dictionary<string, object> eventData3 = new Dictionary<string, object>()
        {
          {
            "TcmRunId",
            (object) testRunId2
          },
          {
            "Exception",
            (object) ex.ToString()
          }
        };
        CILogger.Instance.PublishCI(requestContext, "AutomationRunFailedToQueue", eventData3);
      }
      return testRunId2;
    }

    private void StoreTestRunPropertiesForAutomationRun(
      TestExecutionRequestContext requestContext,
      int testRunId,
      TestRunProperties testRunProperties)
    {
      if (testRunProperties.IsCustomSlicingEnabled)
        this.StoreCustomSlicingFlagForAutomationRun(requestContext, testRunId, testRunProperties);
      if (testRunProperties.IsRerunEnabled)
        this.StoreRerunInfoForAutomationRun(requestContext, testRunId, testRunProperties);
      if (testRunProperties.IsTestImpactOn)
        this.StoreIsTestImpactOnAutomationRun(requestContext, testRunId, testRunProperties.IsTestImpactOn, testRunProperties.BaseLineBuildId);
      Dictionary<string, object> properties = new Dictionary<string, object>()
      {
        {
          TestPropertiesConstants.AutomationRunTestRunProperties,
          (object) testRunProperties.Serialize<TestRunProperties>()
        }
      };
      this.PropertyServiceHelper.AddOrUpdate(requestContext, requestContext.AutomationRunArtifactKindId, testRunId, (IDictionary<string, object>) properties);
    }

    private DtaLogger GetLogger(TestExecutionRequestContext requestContext) => new DtaLogger(requestContext, TestExecutionServiceConstants.TestExecutionServiceArea, TestExecutionServiceConstants.ServiceLayer);

    private void StoreAutomationRunProperties(
      TestExecutionRequestContext requestContext,
      TestRunDefinitionJob runDefinition,
      TeamProjectReference project)
    {
      DtaLogger logger = this.GetLogger(requestContext);
      this.StoreProjectReferenceForAutomationRun(requestContext, runDefinition.TestRunId, project);
      logger.Info(6200413, "Stored ProjectReference in PropertyStore for TestRunId : {0}", (object) runDefinition.TestRunId);
      this.StoreTimeoutForAutomationRun(requestContext, runDefinition.TestRunId, runDefinition.RunTimeout);
      logger.Info(6200414, "Stored RunTimeOut in PropertyStore for TestRunId : {0}", (object) runDefinition.TestRunId);
      if (runDefinition.ConfigurationIds != null && runDefinition.ConfigurationIds.Count > 0)
      {
        this.StoreConfigIdForAutomationRun(requestContext, runDefinition.TestRunId, runDefinition.ConfigurationIds.First<int>());
        logger.Info(6200417, "Configururation Id is stored in Property store for TestRunId : {0}", (object) runDefinition.TestRunId);
      }
      if (!string.IsNullOrEmpty(runDefinition.EnvironmentUrl))
        this.StoreEnvironmentUrlForAutomationRun(requestContext, runDefinition.TestRunId, runDefinition.EnvironmentUrl);
      if (runDefinition.Filter == null)
        return;
      this.StoreRunSourceFilterForAutomationRun(requestContext, runDefinition.TestRunId, runDefinition.Filter);
      this.StoreRunTestCaseFilterForAutomationRun(requestContext, runDefinition.TestRunId, runDefinition.Filter);
    }

    private void StoreRunSourceFilterForAutomationRun(
      TestExecutionRequestContext requestContext,
      int testRunId,
      Microsoft.TeamFoundation.TestManagement.WebApi.RunFilter filter)
    {
      Dictionary<string, object> properties = new Dictionary<string, object>()
      {
        {
          TestPropertiesConstants.AutomationRunSourceFilter,
          (object) filter.SourceFilter
        }
      };
      this.PropertyServiceHelper.AddOrUpdate(requestContext, requestContext.AutomationRunArtifactKindId, testRunId, (IDictionary<string, object>) properties);
    }

    private void StoreRunTestCaseFilterForAutomationRun(
      TestExecutionRequestContext requestContext,
      int testRunId,
      Microsoft.TeamFoundation.TestManagement.WebApi.RunFilter filter)
    {
      Dictionary<string, object> properties = new Dictionary<string, object>()
      {
        {
          TestPropertiesConstants.AutomationRunTestCaseFilter,
          (object) filter.TestCaseFilter
        }
      };
      this.PropertyServiceHelper.AddOrUpdate(requestContext, requestContext.AutomationRunArtifactKindId, testRunId, (IDictionary<string, object>) properties);
    }

    private void StoreEnvironmentUrlForAutomationRun(
      TestExecutionRequestContext requestContext,
      int testRunId,
      string environmentUrl)
    {
      Dictionary<string, object> properties = new Dictionary<string, object>()
      {
        {
          TestPropertiesConstants.AutomationRunEnvironmentUrl,
          (object) environmentUrl
        }
      };
      this.PropertyServiceHelper.AddOrUpdate(requestContext, requestContext.AutomationRunArtifactKindId, testRunId, (IDictionary<string, object>) properties);
    }

    private void StoreProjectReferenceForAutomationRun(
      TestExecutionRequestContext requestContext,
      int testRunId,
      TeamProjectReference project)
    {
      Dictionary<string, object> properties = new Dictionary<string, object>()
      {
        {
          TestPropertiesConstants.AutomationRunProjectReference,
          (object) project.Id.ToString()
        }
      };
      this.PropertyServiceHelper.AddOrUpdate(requestContext, requestContext.AutomationRunArtifactKindId, testRunId, (IDictionary<string, object>) properties);
    }

    private void StoreTimeoutForAutomationRun(
      TestExecutionRequestContext requestContext,
      int testRunId,
      TimeSpan runTimeout)
    {
      Dictionary<string, object> properties = new Dictionary<string, object>()
      {
        {
          TestPropertiesConstants.AutomationRunTimeout,
          (object) runTimeout.TotalSeconds.ToString()
        }
      };
      this.PropertyServiceHelper.AddOrUpdate(requestContext, requestContext.AutomationRunArtifactKindId, testRunId, (IDictionary<string, object>) properties);
    }

    private void StoreConfigIdForAutomationRun(
      TestExecutionRequestContext requestContext,
      int testRunId,
      int configurationId)
    {
      Dictionary<string, object> properties = new Dictionary<string, object>()
      {
        {
          TestPropertiesConstants.AutomationRunConfigurations,
          (object) configurationId
        }
      };
      this.PropertyServiceHelper.AddOrUpdate(requestContext, requestContext.AutomationRunArtifactKindId, testRunId, (IDictionary<string, object>) properties);
    }

    private void StoreCustomSlicingFlagForAutomationRun(
      TestExecutionRequestContext requestContext,
      int testRunId,
      TestRunProperties testRunProperties)
    {
      Dictionary<string, object> properties = new Dictionary<string, object>()
      {
        {
          TestPropertiesConstants.AutomationRunCustomSlicingEnabled,
          (object) testRunProperties.IsCustomSlicingEnabled
        },
        {
          TestPropertiesConstants.AutomationRunCustomSlicingMaxAgents,
          (object) testRunProperties.MaxAgents
        },
        {
          TestPropertiesConstants.AutomationRunCustomSlicingNumberOfTestCasesPerSlice,
          (object) testRunProperties.NumberOfTestCasesPerSlice
        },
        {
          TestPropertiesConstants.AutomationRunCustomSlicingIsTimeBasedSlicing,
          (object) testRunProperties.IsTimeBasedSlicing
        },
        {
          TestPropertiesConstants.AutomationRunCustomSlicingSliceTime,
          (object) testRunProperties.SliceTime
        }
      };
      this.PropertyServiceHelper.AddOrUpdate(requestContext, requestContext.AutomationRunArtifactKindId, testRunId, (IDictionary<string, object>) properties);
    }

    private void StoreRerunInfoForAutomationRun(
      TestExecutionRequestContext requestContext,
      int testRunId,
      TestRunProperties testRunProperties)
    {
      Dictionary<string, object> properties = new Dictionary<string, object>()
      {
        {
          TestPropertiesConstants.AutomationRerunCount,
          (object) testRunProperties.RerunMaxAttempts
        },
        {
          TestPropertiesConstants.AutomationRerunIteration,
          (object) 0
        },
        {
          TestPropertiesConstants.AutomationRerunTestCasesMaxThreshold,
          (object) testRunProperties.RerunFailedThreshold
        },
        {
          TestPropertiesConstants.AutomationRerunFailedTestCasesMaxLimit,
          (object) testRunProperties.RerunFailedTestCasesMaxLimit
        }
      };
      this.PropertyServiceHelper.AddOrUpdate(requestContext, requestContext.AutomationRunArtifactKindId, testRunId, (IDictionary<string, object>) properties);
    }

    private void StoreIsTestImpactOnAutomationRun(
      TestExecutionRequestContext requestContext,
      int testRunId,
      bool isTestImpactOn,
      int baseLineBuildId)
    {
      Dictionary<string, object> properties = new Dictionary<string, object>()
      {
        {
          TestPropertiesConstants.AutomationRunIsTestImpactOn,
          (object) isTestImpactOn
        },
        {
          TestPropertiesConstants.AutomationRunBaseLineDefinitionRunId,
          (object) baseLineBuildId
        }
      };
      this.PropertyServiceHelper.AddOrUpdate(requestContext, requestContext.AutomationRunArtifactKindId, testRunId, (IDictionary<string, object>) properties);
    }

    private void AbortTestWorkflowJob(
      TestExecutionRequestContext requestContext,
      int testRunId,
      Guid cancelledBy)
    {
      DtaLogger logger = this.GetLogger(requestContext);
      WorkFlowJobDetails workFlowJobDetails = (WorkFlowJobDetails) null;
      using (DtaWorkFlowJobDatabase component = requestContext.RequestContext.CreateComponent<DtaWorkFlowJobDatabase>())
      {
        workFlowJobDetails = component.QueryWorkflowJob(testRunId);
        if (workFlowJobDetails != null)
          component.AbortWorkFlowJob(testRunId, cancelledBy);
      }
      if (workFlowJobDetails != null)
        this.ScheduleTestWorkflowJob(requestContext, testRunId);
      else
        logger.Warning(6200420, "Warning : AbortTestRun requested for TestRunId:{0} But no associated workflow found.", (object) testRunId);
      Dictionary<string, object> eventData = new Dictionary<string, object>()
      {
        {
          "TcmRunId",
          (object) testRunId
        },
        {
          "RunCancelledBy",
          (object) cancelledBy
        }
      };
      CILogger.Instance.PublishCI(requestContext, "AutomationRunUserAbortReceived", eventData);
    }

    private void ScheduleWorkflowJobOnSlicesTermination(
      TestExecutionRequestContext requestContext,
      int testRunId)
    {
      this.GetLogger(requestContext);
      IDtaSliceService service = requestContext.RequestContext.GetService<IDtaSliceService>();
      using (new SimpleTimer(requestContext, string.Format("WakeUpWorkFlowJob:{0}", (object) testRunId), true, string.Format("{0}", (object) testRunId)))
      {
        foreach (TestAutomationRunSlice runSlice in service.QuerySlicesByTestRunId(requestContext, testRunId))
        {
          if (!runSlice.HasTerminated())
          {
            Dictionary<string, object> eventData = new Dictionary<string, object>()
            {
              {
                "TcmRunId",
                (object) testRunId
              },
              {
                "SliceId",
                (object) string.Format("{0}:{1}", (object) runSlice.Id, (object) runSlice.Status)
              }
            };
            CILogger.Instance.PublishCI(requestContext, "AutomationRunWorkflowNotScheduled", eventData);
            return;
          }
        }
        this.ScheduleTestWorkflowJob(requestContext, testRunId);
      }
    }

    private void ScheduleTestWorkflowJob(TestExecutionRequestContext requestContext, int testRunId)
    {
      DtaLogger logger = this.GetLogger(requestContext);
      Guid jobGuid = Guid.Empty;
      using (DtaWorkFlowJobDatabase component = requestContext.RequestContext.CreateComponent<DtaWorkFlowJobDatabase>())
        jobGuid = component.QueryWorkflowJob(testRunId).JobId;
      logger.Info(6200421, string.Format("Scheduling the workflow job {0} now for Tcm RunId : {1}", (object) jobGuid, (object) testRunId));
      this.JobHelper.WakeUpWorkflowJob(requestContext, jobGuid, testRunId);
    }

    private void ValidateJob(ITestExecutionJob job)
    {
      ArgumentValidator.CheckNull((object) job, nameof (job));
      if (job.Type == JobType.None)
        throw new ArgumentException(TestExecutionServiceResources.InvalidJobType, nameof (job));
    }

    private void ValidateSliceUpdatePackage(
      TestAutomationRunSlice sliceUpdatePackage,
      TestExecutionRequestContext requestContext)
    {
      ArgumentValidator.CheckNull((object) sliceUpdatePackage, nameof (sliceUpdatePackage));
      DtaLogger logger = this.GetLogger(requestContext);
      if (sliceUpdatePackage.Id <= 0)
      {
        ArgumentException argumentException = new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, TestExecutionServiceResources.InvalidSliceId, (object) sliceUpdatePackage.Id), nameof (sliceUpdatePackage));
        logger.Error(6200426, string.Format("Invalid Slice ID {0}", (object) sliceUpdatePackage.Id));
        throw argumentException;
      }
    }

    private TestAutomationRunSlice ValidateAndPopulateSliceDetails(
      TestExecutionRequestContext requestContext,
      TestAutomationRunSlice taskSlice)
    {
      DtaLogger logger = this.GetLogger(requestContext);
      logger.Verbose(6200422, "A slice is allocated. SliceId : {0}", (object) taskSlice.Id);
      try
      {
        this.TestManagementRunHelper.PopulateTestRunInformation(requestContext, taskSlice.TestRunInformation);
        taskSlice.TestConfigId = TestRunPropertiesService.GetTestConfigurationId(requestContext, taskSlice.TestRunInformation.TcmRun.Id);
        logger.Verbose(6200423, string.Format("Populated Slice with stored AutomationTestRunPropeties. SliceId : {0}. TestConfigIds : {1}. TestRunId : {2}", (object) taskSlice.Id, (object) taskSlice.TestConfigId, (object) taskSlice.TestRunInformation.TcmRun.Id));
        return taskSlice;
      }
      catch (TestObjectNotFoundException ex)
      {
        logger.Error(6200424, string.Format("ValidateAndPopulateSliceDetails: Handled TestObjectNotFoundException {0}", (object) ex));
        throw new TestExecutionServiceInvalidOperationException(TestExecutionServiceResources.QueryRunDetailsFailed, (Exception) ex);
      }
      catch (TeamFoundationServerException ex)
      {
        logger.Error(6200425, string.Format("ValidateAndPopulateSliceDetails: Handled TeamFoundationServerException {0}", (object) ex));
        throw new TestExecutionServiceException(TestExecutionServiceResources.QueryRunDetailsFailed, (Exception) ex);
      }
    }

    private TeamProjectReference ValidateProjectInfo(
      TestExecutionRequestContext context,
      TeamProjectReference project)
    {
      ProjectInfo projectInfo = (ProjectInfo) null;
      if (project.Id == Guid.Empty && string.IsNullOrEmpty(project.Name))
        throw new ArgumentException(TestExecutionServiceResources.InvalidProjectInfo, "job.ProjectReference");
      if (project.Id != Guid.Empty)
        projectInfo = context.ProjectServiceHelper.GetProjectFromGuid(project.Id);
      else if (!string.IsNullOrEmpty(project.Name))
        projectInfo = context.ProjectServiceHelper.GetProjectFromName(project.Name);
      if (project == null)
        return (TeamProjectReference) null;
      return new TeamProjectReference()
      {
        Id = projectInfo.Id,
        Name = projectInfo.Name
      };
    }

    private void AddSliceLogsToTcm(
      TestExecutionRequestContext requestContext,
      TestAutomationRunSlice sliceUpdatePackage,
      TeamProjectReference project)
    {
      int id = sliceUpdatePackage.TestRunInformation.TcmRun.Id;
      if (sliceUpdatePackage.Type != AutomatedTestRunSliceType.Discovery)
        return;
      this.TcmLogger.AddLogToTcmRun(requestContext, id, project, string.Format((IFormatProvider) CultureInfo.CurrentCulture, TestExecutionServiceResources.TestRunUpdate, (object) sliceUpdatePackage.Type, (object) sliceUpdatePackage.Status, (object) id));
      if (string.IsNullOrEmpty(sliceUpdatePackage.Results))
        return;
      int count = JsonConvert.DeserializeObject<List<TestMetadata>>(sliceUpdatePackage.Results).Count;
      this.TcmLogger.AddLogToTcmRun(requestContext, id, project, string.Format((IFormatProvider) CultureInfo.CurrentCulture, TestExecutionServiceResources.NumberOfTestCasesDiscovered, (object) count));
    }

    public ITestManagementRunHelper TestManagementRunHelper
    {
      get => this._testManagementRunHelper ?? (this._testManagementRunHelper = (ITestManagementRunHelper) new Microsoft.TeamFoundation.TestExecution.Server.TestManagementRunHelper());
      set => this._testManagementRunHelper = value;
    }

    public ITestExecutionServiceJobHelper JobHelper
    {
      get => this._testExecutionServiceJobHelper ?? (this._testExecutionServiceJobHelper = (ITestExecutionServiceJobHelper) new TestExecutionServiceJobHelper());
      set => this._testExecutionServiceJobHelper = value;
    }

    public IPropertyServiceHelper PropertyServiceHelper
    {
      get => this._propertyServiceHelper ?? (this._propertyServiceHelper = (IPropertyServiceHelper) new Microsoft.TeamFoundation.TestExecution.Server.PropertyServiceHelper());
      set => this._propertyServiceHelper = value;
    }

    public ITcmLogger TcmLogger
    {
      get => this._tcmLogger ?? (this._tcmLogger = (ITcmLogger) new Microsoft.TeamFoundation.TestExecution.Server.TcmLogger());
      set => this._tcmLogger = value;
    }

    public ITestManagementConfigurationHelper ConfigurationHelper
    {
      get => this._configurationHelper ?? (this._configurationHelper = (ITestManagementConfigurationHelper) new TestManagementConfigurationHelper());
      set => this._configurationHelper = value;
    }
  }
}
