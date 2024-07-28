// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestRun
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.TestManagement.Client;
using Microsoft.TeamFoundation.TestManagement.Server.TCMServiceMigration;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [ClassVisibility(ClientVisibility.Internal)]
  [SoapInclude(typeof (TestRunBase))]
  [CLSCompliant(false)]
  public class TestRun : TestRunBase
  {
    private TestRunStatistic[] m_testRunStatistics;
    private int m_testRunId;
    private byte m_state;
    private string m_dropLocation;
    private byte m_postProcessState;
    private DateTime m_dueDate;
    private string m_iteration;
    private int m_testMessageLogId;
    private ShallowReference m_dtlTestEnvironment;
    private ShallowReference m_dtlAutEnvironment;
    private RunFilter m_runFilter;
    private List<TestMessageLogDetails> m_testMessageLogEntries;
    private ITelemetryLogger m_telemetryLogger;
    private static IBuildServiceHelper m_buildServiceHelper;
    private static IReleaseServiceHelper m_releaseServiceHelper;

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private, UseClientDefinedProperty = true, PropertyName = "Id")]
    [QueryMapping]
    public int TestRunId
    {
      get => this.m_testRunId;
      set => this.m_testRunId = value;
    }

    [XmlArray]
    [XmlArrayItem(typeof (TestRunStatistic))]
    [ClientProperty(ClientVisibility.Private)]
    public TestRunStatistic[] TestRunStatistics
    {
      get => this.m_testRunStatistics;
      set => this.m_testRunStatistics = value;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping(EnumType = typeof (Microsoft.TeamFoundation.TestManagement.Client.TestRunState))]
    public byte State
    {
      get => this.m_state;
      set => this.m_state = value;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping]
    public string DropLocation
    {
      get => this.m_dropLocation;
      set => this.m_dropLocation = value;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping(EnumType = typeof (Microsoft.TeamFoundation.TestManagement.Client.PostProcessState))]
    public byte PostProcessState
    {
      get => this.m_postProcessState;
      set => this.m_postProcessState = value;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping]
    public DateTime DueDate
    {
      get => this.m_dueDate;
      set => this.m_dueDate = value;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping]
    public string Iteration
    {
      get => this.m_iteration;
      set => this.m_iteration = value;
    }

    [XmlIgnore]
    internal int IterationId { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping]
    public int TestMessageLogId
    {
      get => this.m_testMessageLogId;
      set => this.m_testMessageLogId = value;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping]
    public string LegacySharePath { get; set; }

    [XmlElement]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping]
    public string ErrorMessage { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping(EnumType = typeof (TestRunType))]
    public byte Type { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping]
    public bool IsAutomated { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping]
    public int Version { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping]
    public bool IsBvt { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping]
    public int TotalTests { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping]
    public int IncompleteTests { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping]
    public int NotApplicableTests { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping]
    public int PassedTests { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping]
    public int UnanalyzedTests { get; set; }

    [XmlIgnore]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping]
    public string ReleaseUri { get; set; }

    [XmlIgnore]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping]
    public string ReleaseEnvironmentUri { get; set; }

    public int FailedTests { get; set; }

    public BuildConfiguration BuildReference { get; set; }

    public ReleaseReference ReleaseReference { get; set; }

    public RunFilter Filter
    {
      get => this.m_runFilter;
      set => this.m_runFilter = value;
    }

    public int[] ConfigurationIds { get; set; }

    public ShallowReference DtlTestEnvironment
    {
      get => this.m_dtlTestEnvironment;
      set => this.m_dtlTestEnvironment = value;
    }

    public ShallowReference DtlAutEnvironment
    {
      get => this.m_dtlAutEnvironment;
      set => this.m_dtlAutEnvironment = value;
    }

    public List<TestMessageLogDetails> TestMessageLogEntries
    {
      get => this.m_testMessageLogEntries;
      set => this.m_testMessageLogEntries = value;
    }

    public List<TestExtensionField> CustomFields { get; set; }

    public string SourceWorkflow { get; set; }

    public byte Substate { get; set; }

    public TimeSpan RunTimeout { get; set; }

    public string CsmParameters { get; set; }

    public string CsmContent { get; set; }

    public string SubscriptionName { get; set; }

    public string TestConfigurationsMapping { get; set; }

    public List<TestTag> Tags { get; set; }

    public PipelineReference PipelineReference { get; set; }

    internal bool RunHasDtlEnvironment => ((uint) this.Type & 16U) > 0U;

    internal ITelemetryLogger TelemetryLogger
    {
      get
      {
        if (this.m_telemetryLogger == null)
          this.m_telemetryLogger = (ITelemetryLogger) new Microsoft.TeamFoundation.TestManagement.Server.TelemetryLogger();
        return this.m_telemetryLogger;
      }
      set => this.m_telemetryLogger = value;
    }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "TestRun Id={0} Title={1} State = {2}, SubState = {3}, Type = {4} Build={5} Filter:{6} TestEnvName:{7} AutEnvName:{8}", (object) this.TestRunId, (object) this.Title, (object) this.State, (object) this.Substate, (object) this.Type, (object) this.BuildUri, (object) this.Filter, (object) (this.DtlTestEnvironment != null ? this.DtlTestEnvironment.Name : string.Empty), (object) (this.DtlAutEnvironment != null ? this.DtlAutEnvironment.Name : string.Empty));

    internal virtual TestRun Create(
      TestManagementRequestContext context,
      TestSettings settings,
      TestCaseResult[] results,
      bool populateDataRowCount,
      string teamProjectName,
      bool byRestApi = false)
    {
      using (PerfManager.Measure(context.RequestContext, "BusinessLayer", "TestRun.Create"))
      {
        IVssRegistryService service = context.RequestContext.GetService<IVssRegistryService>();
        TestRun testRun = (TestRun) null;
        byte state = this.State;
        if (this.Owner == Guid.Empty)
          throw new TestManagementValidationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.InvalidFieldValue, (object) "Owner"));
        GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, teamProjectName);
        context.SecurityManager.CheckPublishTestResultsPermission(context, projectFromName.String);
        if (!this.IsAutomated)
          this.IterationId = context.IterationsCache.GetIdAndThrow(context, this.Iteration).Id;
        TestRun.CheckBuildExists(context, projectFromName.GuidId, this.BuildUri);
        this.State = (byte) 1;
        if (this.Type == (byte) 0)
          this.Type = (byte) 1;
        if (this.ReleaseReference == null || this.ReleaseReference.ReleaseDefId == 0)
          this.ReleaseReference = this.GetReleaseDetails(context.RequestContext, projectFromName);
        if (this.BuildReference == null || this.BuildReference.BuildDefinitionId == 0)
          this.BuildReference = this.GetBuildConfigurationDetails(context.RequestContext, projectFromName);
        if (this.BuildReference != null)
          this.BuildReference.BranchName = GitHelper.GetModifiedBranchName(this.BuildReference.BranchName);
        Guid teamFoundationId = context.UserTeamFoundationId;
        string teamFoundationName = context.UserTeamFoundationName;
        this.SetRunSettings(context, teamFoundationId, projectFromName.GuidId, settings);
        this.Comment = TestManagementServiceUtility.GetTrimmedTestRunField(context.RequestContext, this, "Comment", this.Comment);
        this.ErrorMessage = TestManagementServiceUtility.GetTrimmedTestRunField(context.RequestContext, this, "ErrorMessage", this.ErrorMessage);
        int length = 16;
        List<TestExtensionField> customFields = this.CustomFields;
        TestExtensionField testExtensionField = customFields != null ? customFields.FirstOrDefault<TestExtensionField>((Func<TestExtensionField, bool>) (t => string.Equals(t.Field.Name, "TestRunSystem", StringComparison.OrdinalIgnoreCase))) : (TestExtensionField) null;
        if (!string.IsNullOrEmpty(testExtensionField?.Value?.ToString()))
          testExtensionField.Value = testExtensionField.Value.ToString().Length > length ? (object) testExtensionField.Value.ToString().Substring(0, length) : testExtensionField.Value;
        bool changeCounterInterval = ServiceMigrationHelper.ShouldChangeCounterInterval(context.RequestContext);
        bool isTcmService = context.IsTcmService;
        ProjectInfo projectFromGuid = context.ProjectServiceHelper.GetProjectFromGuid(projectFromName.GuidId);
        this.SetRunExtensionLogStoreCustomField(context, projectFromGuid);
        if (!string.IsNullOrEmpty(this.Title) && this.Title.Length > 256)
          this.Title = this.Title.Substring(0, 256);
        bool reuseDeletedTestRunId = context.IsFeatureEnabled("TestManagement.Server.ReuseDeletedTestRunId");
        IVssRequestContext requestContext = context.RequestContext;
        // ISSUE: explicit reference operation
        ref RegistryQuery local = @(RegistryQuery) "/Service/TestManagement/Settings/ReuseTestRunIdDays";
        int reuseTestRunIdDays = service.GetValue<int>(requestContext, in local, 2);
        using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
          testRun = managementDatabase.CreateTestRun(projectFromName.GuidId, this, teamFoundationId, changeCounterInterval, isTcmService, reuseDeletedTestRunId, reuseTestRunIdDays);
        testRun.TestSettingsId = this.TestSettingsId;
        testRun.LastUpdatedBy = teamFoundationId;
        testRun.LastUpdatedByName = teamFoundationName;
        testRun.OwnerName = IdentityHelper.ResolveIdentityToName(context, testRun.Owner);
        testRun.TeamProject = teamProjectName;
        testRun.ConfigurationIds = this.ConfigurationIds;
        testRun.Type = this.Type;
        testRun.Filter = this.Filter;
        testRun.CsmContent = this.CsmContent;
        testRun.CsmParameters = this.CsmParameters;
        testRun.SubscriptionName = this.SubscriptionName;
        testRun.DtlTestEnvironment = this.DtlTestEnvironment;
        testRun.DtlAutEnvironment = this.DtlAutEnvironment;
        testRun.RunTimeout = this.RunTimeout;
        testRun.TestConfigurationsMapping = this.TestConfigurationsMapping;
        testRun.Title = this.Title;
        testRun.BuildReference = this.BuildReference;
        testRun.Iteration = !this.IsAutomated ? this.Iteration : (string) null;
        testRun.TestPlanId = this.TestPlanId;
        testRun.ReleaseReference = this.ReleaseReference;
        testRun.IsAutomated = this.IsAutomated;
        testRun.PipelineReference = this.PipelineReference;
        if (results != null && results.Length != 0)
        {
          foreach (TestResult result in results)
            result.TestRunId = testRun.TestRunId;
          TestCaseResult.Create(context, results, populateDataRowCount, teamProjectName);
        }
        if ((int) state != (int) this.State)
        {
          using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
          {
            this.State = state;
            this.TestRunId = testRun.TestRunId;
            this.Revision = testRun.Revision;
            UpdatedProperties updatedProperties = (UpdatedProperties) managementDatabase.UpdateTestRun(projectFromName.GuidId, this, teamFoundationId);
            testRun.Revision = updatedProperties.Revision;
            testRun.LastUpdated = updatedProperties.LastUpdated;
          }
        }
        TestRun.FireNotification(context, testRun.TestRunId, teamProjectName);
        TestRun.FireRunCreatedNotification(context, testRun);
        if (context.IsFeatureEnabled("TestManagement.Server.OnDemandTesting") && this.State == (byte) 2)
          TestRun.FireRunStartedNotification(context, testRun);
        this.AddTelemetryForTestRun(context, projectFromName.GuidId, results != null ? results.Length : 0, byRestApi, ((TestRunType) this.Type).ToString());
        TestRunBase.PopulateVersion((TestRunBase) testRun);
        context.TraceInfo("BusinessLayer", "Test run successfully created. TestrunId = {0}, ProjectName = {1}", (object) testRun.TestRunId, (object) teamProjectName);
        return testRun;
      }
    }

    private void SetRunSettings(
      TestManagementRequestContext requestContext,
      Guid userId,
      Guid projectId,
      TestSettings settings)
    {
      if (settings == null)
        return;
      settings.IsPublic = false;
      settings.AreaId = requestContext.AreaPathsCache.GetIdAndThrow(requestContext, settings.AreaPath).Id;
      if (requestContext.IsFeatureEnabled("TestManagement.Server.CreateIfNotExistsTestSettings"))
      {
        using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(requestContext))
          this.TestSettingsId = managementDatabase.CreateIfNotExistsTestSettings(requestContext, projectId, settings, userId).Id;
      }
      else
      {
        using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(requestContext))
          this.TestSettingsId = managementDatabase.CreateTestSettings(requestContext, projectId, settings, userId).Id;
      }
    }

    private void SetRunExtensionLogStoreCustomField(
      TestManagementRequestContext context,
      ProjectInfo projectInfo)
    {
      bool flag = context.IsFeatureEnabled("TestManagement.Server.TestLogStoreOnTCMService");
      if (((projectInfo == null ? 0 : (projectInfo.Visibility == ProjectVisibility.Public ? 1 : 0)) & (flag ? 1 : 0)) != 0)
      {
        IList<TestExtensionFieldDetails> runExtensionFields = this.GetTestRunExtensionFields(context, projectInfo, new List<string>()
        {
          "LogStoreContainerState",
          "LogStoreContainerSize"
        });
        TestExtensionFieldDetails extensionFieldDetails1 = runExtensionFields.Where<TestExtensionFieldDetails>((Func<TestExtensionFieldDetails, bool>) (f => f.Name.Equals("LogStoreContainerState", StringComparison.OrdinalIgnoreCase))).FirstOrDefault<TestExtensionFieldDetails>();
        TestExtensionFieldDetails extensionFieldDetails2 = runExtensionFields.Where<TestExtensionFieldDetails>((Func<TestExtensionFieldDetails, bool>) (f => f.Name.Equals("LogStoreContainerSize", StringComparison.OrdinalIgnoreCase))).FirstOrDefault<TestExtensionFieldDetails>();
        if (this.CustomFields == null)
          this.CustomFields = new List<TestExtensionField>();
        if (extensionFieldDetails1 != null)
          this.CustomFields.Add(new TestExtensionField()
          {
            Field = extensionFieldDetails1,
            Value = !context.IsFeatureEnabled("TestManagement.Server.TestLogStoreGeoReplicationEnabled") ? (object) 41 : (object) 105
          });
        if (extensionFieldDetails2 == null)
          return;
        this.CustomFields.Add(new TestExtensionField()
        {
          Field = extensionFieldDetails2,
          Value = (object) 0
        });
      }
      else
      {
        if (!flag || !context.IsFeatureEnabled("TestManagement.Server.TestLogStoreGeoReplicationEnabled"))
          return;
        TestExtensionFieldDetails extensionFieldDetails = this.GetTestRunExtensionFields(context, projectInfo, new List<string>()
        {
          "LogStoreContainerState"
        }).FirstOrDefault<TestExtensionFieldDetails>();
        if (this.CustomFields == null)
          this.CustomFields = new List<TestExtensionField>();
        if (extensionFieldDetails == null)
          return;
        this.CustomFields.Add(new TestExtensionField()
        {
          Field = extensionFieldDetails,
          Value = (object) LogStoreContainerState.PendingGeoReplication
        });
      }
    }

    private IList<TestExtensionFieldDetails> GetTestRunExtensionFields(
      TestManagementRequestContext context,
      ProjectInfo projectInfo,
      List<string> fieldNames)
    {
      return context.RequestContext.GetService<ITeamFoundationTestExtensionFieldsService>().QueryFields(context, projectInfo.Id, (IList<string>) fieldNames, CustomTestFieldScope.System);
    }

    private BuildConfiguration GetBuildConfigurationDetails(
      IVssRequestContext context,
      GuidAndString projectId)
    {
      using (PerfManager.Measure(context, "BusinessLayer", "TestRun.GetBuildConfigurationDetails"))
      {
        BuildConfiguration configurationDetails = (BuildConfiguration) null;
        string buildUri = this.BuildUri;
        int buildId = 0;
        string buildNumber = this.BuildNumber;
        if (this.BuildReference != null)
        {
          if (this.BuildReference.BuildId > 0)
            buildId = this.BuildReference.BuildId;
          if (!string.IsNullOrEmpty(this.BuildReference.BuildUri))
            buildUri = this.BuildReference.BuildUri;
          if (!string.IsNullOrEmpty(this.BuildReference.BuildNumber))
            buildNumber = this.BuildReference.BuildNumber;
        }
        if (buildId > 0)
          configurationDetails = TestRun.BuildServiceHelper.QueryBuildConfigurationById(context, projectId.GuidId, buildId);
        if (!string.IsNullOrEmpty(buildUri))
          configurationDetails = TestRun.BuildServiceHelper.QueryBuildConfigurationByBuildUri(context, projectId.GuidId, buildUri);
        else if (!string.IsNullOrEmpty(buildNumber))
          configurationDetails = TestRun.BuildServiceHelper.QueryBuildConfigurationByBuildNumber(context, projectId.GuidId, buildNumber);
        if (configurationDetails != null)
        {
          configurationDetails.BuildPlatform = this.BuildPlatform != null ? this.BuildPlatform : string.Empty;
          configurationDetails.BuildFlavor = this.BuildFlavor != null ? this.BuildFlavor : string.Empty;
        }
        return configurationDetails;
      }
    }

    private ReleaseReference GetReleaseDetails(IVssRequestContext context, GuidAndString projectId)
    {
      using (PerfManager.Measure(context, "BusinessLayer", "TestRun.GetReleaseDetails"))
      {
        ReleaseReference releaseDetails = (ReleaseReference) null;
        if (!string.IsNullOrEmpty(this.ReleaseUri) && !string.IsNullOrEmpty(this.ReleaseEnvironmentUri))
          releaseDetails = TestRun.ReleaseServiceHelper.QueryReleaseReferenceByUri(context, projectId, this.ReleaseUri, this.ReleaseEnvironmentUri);
        return releaseDetails;
      }
    }

    private void AddTelemetryForTestRun(
      TestManagementRequestContext context,
      Guid projectId,
      int pointsCount,
      bool byRestApi,
      string testRunType)
    {
      try
      {
        CustomerIntelligenceData intelligenceData = new CustomerIntelligenceData();
        intelligenceData.Add("ProjectId", (object) projectId);
        intelligenceData.Add("TestRunSource", this.GetTestRunSource(pointsCount));
        intelligenceData.Add("TestRunId", this.TestRunId.ToString());
        intelligenceData.Add("TestRunAutomatedOrManual", this.IsAutomated ? (byRestApi ? "Automated_REST" : "Automated") : "Manual");
        intelligenceData.Add("TestRunType", testRunType);
        intelligenceData.Add("TestRunPoints", pointsCount.ToString());
        intelligenceData.Add("RunState", (double) this.State);
        intelligenceData.Add("TestRunSettings", this.PublicTestSettingsId == 0 ? "Default" : "Custom");
        intelligenceData.Add("Workflow", this.SourceWorkflow);
        if (this.BuildReference != null && !string.IsNullOrEmpty(this.BuildReference.BuildUri))
          intelligenceData.Add("BuildUri", this.BuildReference.BuildUri);
        if (this.ReleaseReference != null && !string.IsNullOrEmpty(this.ReleaseReference.ReleaseUri) && !string.IsNullOrEmpty(this.ReleaseReference.ReleaseEnvUri))
        {
          intelligenceData.Add("ReleaseUri", this.ReleaseReference.ReleaseUri);
          intelligenceData.Add("ReleaseEnvUri", this.ReleaseReference.ReleaseEnvUri);
          intelligenceData.Add("Attempt", (double) this.ReleaseReference.Attempt);
        }
        List<TestExtensionField> customFields = this.CustomFields;
        object obj = (customFields != null ? customFields.FirstOrDefault<TestExtensionField>((Func<TestExtensionField, bool>) (t => string.Equals(t.Field.Name, "TestRunSystem", StringComparison.OrdinalIgnoreCase))) : (TestExtensionField) null)?.Value ?? (object) TestResultsConstants.DefaultTestRunSystem;
        intelligenceData.Add("TestRunSystem", obj);
        ProjectInfo project = context.RequestContext.GetService<IProjectService>().GetProject(context.RequestContext, projectId);
        intelligenceData.AddDataspaceInformation(CustomerIntelligenceDataspaceType.Project, projectId.ToString(), ((int) project.Visibility).ToString((IFormatProvider) CultureInfo.InvariantCulture));
        this.TelemetryLogger.PublishData(context.RequestContext, nameof (TestRun), intelligenceData);
      }
      catch (Exception ex)
      {
        context.RequestContext.Trace(0, TraceLevel.Error, "TestManagement", "BusinessLayer", string.Format("Error in publishing run telmetry for Create or Run: {0}", (object) ex));
      }
    }

    private string GetTestRunSource(int pointsCount)
    {
      string empty = string.Empty;
      return this.Type == (byte) 4 || this.Type == (byte) 32 ? "Web" : (this.Type != (byte) 16 ? (!this.IsBvt ? "MTM" : "BVT") : (pointsCount > 0 ? "DTA-MTM" : "DTA"));
    }

    internal UpdatedProperties Update(
      TestManagementRequestContext context,
      string projectName,
      bool bSkipRunStateTransitionCheck = false,
      TestSettings settings = null,
      ReleaseReference releaseRef = null,
      BuildConfiguration buildRef = null,
      bool invokvedViaRestApi = false,
      bool enableCustomFields = false)
    {
      context.TraceEnter("BusinessLayer", "TestRun.Update");
      context.TraceVerbose("BusinessLayer", "Run details: {0}", (object) this.ToString());
      GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, projectName);
      context.SecurityManager.CheckPublishTestResultsPermission(context, projectFromName.String);
      if (this.Iteration != null)
        this.IterationId = context.IterationsCache.GetIdAndThrow(context, this.Iteration).Id;
      context.TraceVerbose("BusinessLayer", "TestRun.Update:: Updating test run. ProjectName = {0}", (object) projectName);
      Guid teamFoundationId = context.UserTeamFoundationId;
      this.SetRunSettings(context, teamFoundationId, projectFromName.GuidId, settings);
      this.Comment = TestManagementServiceUtility.GetTrimmedTestRunField(context.RequestContext, this, "Comment", this.Comment);
      this.ErrorMessage = TestManagementServiceUtility.GetTrimmedTestRunField(context.RequestContext, this, "ErrorMessage", this.ErrorMessage);
      UpdatedRunProperties updatedRunProperties;
      using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
      {
        try
        {
          updatedRunProperties = !enableCustomFields ? managementDatabase.UpdateTestRun(projectFromName.GuidId, this, teamFoundationId, releaseRef, buildRef, bSkipRunStateTransitionCheck) : managementDatabase.UpdateTestRunWithCustomFields(projectFromName.GuidId, this, teamFoundationId, releaseRef, buildRef, bSkipRunStateTransitionCheck);
        }
        catch (TestObjectUpdatedException ex)
        {
          context.TraceWarning("BusinessLayer", "TestRun.Update:: Error occurred while updating test run. Error = {0}. TestRunId = {1}", (object) ex.Message, (object) this.TestRunId);
          return new UpdatedProperties() { Revision = -1 };
        }
      }
      TestRun.FireNotification(context, this.TestRunId, projectName);
      this.LastUpdated = updatedRunProperties.LastUpdated;
      this.Revision = updatedRunProperties.Revision;
      this.LastUpdatedBy = updatedRunProperties.LastUpdatedBy;
      this.TeamProject = projectName;
      this.TotalTests = updatedRunProperties.TotalTests;
      this.PassedTests = updatedRunProperties.PassedTests;
      this.FailedTests = updatedRunProperties.FailedTests;
      if (updatedRunProperties.IsRunStarted)
        TestRun.FireRunStartedNotification(context, this);
      if (updatedRunProperties.IsRunCompleted)
      {
        TestRun.FireRunCompletedNotification(context, this);
        TestRun.LogRunCompletedTelemetry(context, this);
      }
      this.AddTelemetryForTestRun(context, projectFromName.GuidId, 0, invokvedViaRestApi, ((TestRunType) this.Type).ToString());
      return new UpdatedProperties()
      {
        Id = updatedRunProperties.Id,
        LastUpdated = updatedRunProperties.LastUpdated,
        LastUpdatedBy = updatedRunProperties.LastUpdatedBy,
        LastUpdatedByName = context.UserTeamFoundationName,
        Revision = updatedRunProperties.Revision
      };
    }

    internal UpdatedProperties Update(
      TestManagementRequestContext context,
      string projectName,
      bool shouldHydrate,
      bool bSkipRunStateTransitionCheck = false,
      TestSettings settings = null,
      ReleaseReference releaseRef = null,
      BuildConfiguration buildRef = null)
    {
      if (!shouldHydrate)
        return this.Update(context, projectName, bSkipRunStateTransitionCheck, settings, releaseRef, buildRef, false, false);
      TestRun src = TestRun.Query(context, this.TestRunId, Guid.Empty, (string) null, projectName).First<TestRun>();
      new DataContractConverter(context).PopulateRunDetails(src, this);
      return this.Update(context, projectName, bSkipRunStateTransitionCheck, settings, releaseRef, buildRef, false, false);
    }

    internal static UpdatedProperties Abort(
      TestManagementRequestContext context,
      int testRunId,
      int revision,
      string projectName,
      TestRunAbortOptions options,
      TestRunSubstate substate = TestRunSubstate.None,
      Guid? abortedBy = null)
    {
      context.TraceEnter("BusinessLayer", "TestRun.Abort");
      GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, projectName);
      context.SecurityManager.CheckPublishTestResultsPermission(context, projectFromName.String);
      context.TraceVerbose("BusinessLayer", "TestRun.Abort:: Aborting test run. TestRunId = {0}, ProjectName = {1}, SubState = {2}", (object) testRunId, (object) projectName, (object) substate);
      Guid teamFoundationId = context.UserTeamFoundationId;
      Guid updatedBy = abortedBy.Equals((object) null) ? teamFoundationId : abortedBy.Value;
      bool isTcmService = context.IsTcmService;
      string iterationUri;
      Guid runProjGuid;
      TestRun run;
      UpdatedProperties property;
      using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
      {
        try
        {
          property = managementDatabase.AbortTestRun(projectFromName.GuidId, testRunId, revision, options, (byte) substate, updatedBy, out iterationUri, out runProjGuid, out run, isTcmService);
        }
        catch (TestObjectUpdatedException ex)
        {
          context.TraceWarning("BusinessLayer", "TestRun.Abort:: Error occurred while aborting test run. Error = {0}. TestRunId = {1}", (object) ex.Message, (object) testRunId);
          return new UpdatedProperties() { Revision = -1 };
        }
      }
      TestRun.UpdateProjectDataForRun(context, runProjGuid, run);
      TestRun.UpdateIterationPathForRun(context, iterationUri, run);
      TestRun.FireNotification(context, testRunId, projectName);
      if (run != null)
      {
        TestRun.FireRunCompletedNotification(context, run);
        TestRun.LogRunCompletedTelemetry(context, run);
      }
      context.TraceInfo("BusinessLayer", "TestRun.Abort:: Test run aborted. TestRunId = {0}, abortedBy = {1}", (object) testRunId, (object) updatedBy);
      return property.ResolveIdentity(context);
    }

    public static UpdatedProperties Cancel(
      TestManagementRequestContext context,
      int testRunId,
      string projectName,
      Guid? canceledBy = null)
    {
      context.TraceEnter("BusinessLayer", "TestRun.Cancel");
      GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, projectName);
      context.SecurityManager.CheckPublishTestResultsPermission(context, projectFromName.String);
      context.TraceVerbose("BusinessLayer", "TestRun.Cancel:: Cancelling test run. TestRunId = {0}", (object) testRunId);
      Guid teamFoundationId = context.UserTeamFoundationId;
      Guid canceledBy1 = canceledBy.Equals((object) null) ? teamFoundationId : canceledBy.Value;
      string iterationUri;
      Guid runProjGuid;
      TestRun run;
      UpdatedProperties property;
      using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
        property = managementDatabase.CancelTestRun(projectFromName.GuidId, testRunId, canceledBy1, out iterationUri, out runProjGuid, out run);
      TestRun.UpdateProjectDataForRun(context, runProjGuid, run);
      TestRun.UpdateIterationPathForRun(context, iterationUri, run);
      TestRun.FireNotification(context, testRunId, projectName);
      TestRun.FireRunCancelRequestedNotification(context, run);
      context.TraceInfo("BusinessLayer", "TestRun.Cancel:: Test run cancelled. TestRunId = {0}, cancelledBy = {1}", (object) testRunId, (object) canceledBy1);
      return property.ResolveIdentity(context);
    }

    internal static void Delete(
      TestManagementRequestContext context,
      int[] testRunIds,
      string projectName)
    {
      if (testRunIds == null)
        return;
      GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, projectName);
      context.SecurityManager.CheckDeleteTestResultsPermission(context, projectFromName.String);
      Guid teamFoundationId = context.UserTeamFoundationId;
      bool isTcmService = context.IsTcmService;
      using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
      {
        foreach (int testRunId in testRunIds)
          managementDatabase.QueueDeleteTestRun(projectFromName.GuidId, testRunId, teamFoundationId, isTcmService);
      }
      foreach (int testRunId in testRunIds)
        TestRun.FireNotification(context, testRunId, projectName);
      context.TestManagementHost.SignalTfsJobService(context, context.JobMappings["TestManagement.Jobs.CleanupJob"].ToString());
    }

    internal static List<TestRun> Query(
      TestManagementRequestContext context,
      int testRunId,
      Guid owner,
      string buildUri,
      string teamProjectName,
      int planId = -1,
      int skip = 0,
      int top = 2147483647)
    {
      using (PerfManager.Measure(context.RequestContext, "BusinessLayer", "TestRun.Query(WithRunIdAndProjctName)"))
      {
        GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, teamProjectName);
        context.TraceVerbose("BusinessLayer", "TestRun.Query:: Querying test run. testRunId = {0}. ProjectName = {1}", (object) testRunId, (object) teamProjectName);
        context.SecurityManager.CheckViewTestResultsPermission(context, projectFromName.String);
        context.TestManagementHost.Replicator.UpdateCss(context);
        Dictionary<int, string> iterationMap = new Dictionary<int, string>();
        Dictionary<Guid, List<TestRun>> projectsRunsMap = new Dictionary<Guid, List<TestRun>>();
        bool isTcmService = context.IsTcmService;
        if (!context.RequestContext.IsFeatureEnabled("TestManagement.Server.DisableRunContextOnpremQuery"))
          isTcmService = !context.RequestContext.ExecutionEnvironment.IsOnPremisesDeployment;
        List<TestRun> testRunList;
        using (TestManagementDatabase replicaAwareComponent = TestManagementDatabase.CreateReadReplicaAwareComponent(context.RequestContext))
          testRunList = replicaAwareComponent.QueryTestRuns(testRunId, owner, buildUri, projectFromName.GuidId, out iterationMap, out projectsRunsMap, planId, skip, top, isTcmService);
        TestRun.UpdateProjectDataForRuns(context, projectsRunsMap);
        TestRun.UpdateIterationPathsForRuns(context, iterationMap, testRunList);
        TestRunBase.ResolveUserNames<TestRun>(context, testRunList);
        TestRunBase.PopulateVersion<TestRun>((IList<TestRun>) testRunList);
        TestRun.TrimFieldsForRuns(context, testRunList);
        context.TraceVerbose("BusinessLayer", "TestRun.Query:: Queried test runs successfully. TestRunsCount = {0}. TestRunId = {1} ProjectName = {2}", (object) testRunList.Count, (object) testRunId, (object) teamProjectName);
        return testRunList;
      }
    }

    private static void TrimFieldsForRuns(TestManagementRequestContext context, List<TestRun> runs)
    {
      runs.ForEach((Action<TestRun>) (r => r.Comment = TestManagementServiceUtility.GetTrimmedTestRunField(context.RequestContext, r, "Comment", r.Comment)));
      runs.ForEach((Action<TestRun>) (r => r.ErrorMessage = TestManagementServiceUtility.GetTrimmedTestRunField(context.RequestContext, r, "ErrorMessage", r.ErrorMessage)));
    }

    internal static List<TestRun> Query(
      TestManagementRequestContext context,
      ResultsStoreQuery query,
      bool includeStatistics = false)
    {
      using (PerfManager.Measure(context.RequestContext, "BusinessLayer", "TestRun.Query(With query)"))
      {
        context.TraceInfo("BusinessLayer", "TestRun.Query:: Querying test run. query = {0}.", (object) query);
        ArgumentUtility.CheckForNull<ResultsStoreQuery>(query, nameof (query), "Test Results");
        GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, query.TeamProjectName);
        if (!context.SecurityManager.HasViewTestResultsPermission(context, projectFromName.String))
          return new List<TestRun>();
        query.QueryText = TestRun.UpdateQueryTextToIncludeWebRunsIfNeeded(context.RequestContext, query.QueryText);
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        IterationPathQueryTranslator pathQueryTranslator = new IterationPathQueryTranslator(context, query, TestRun.\u003C\u003EO.\u003C0\u003E__TestRunQueryBuildUriTranslator ?? (TestRun.\u003C\u003EO.\u003C0\u003E__TestRunQueryBuildUriTranslator = new Func<string, string, string>(TestRun.TestRunQueryBuildUriTranslator)));
        query.QueryText = pathQueryTranslator.TranslateQuery();
        if (context.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableQueryRunThrowException"))
        {
          if (pathQueryTranslator.Root.Where == null)
            throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.WIQLNoFilterError));
        }
        else if (pathQueryTranslator.Root.Where == null)
          context.RequestContext.Trace(1015762, TraceLevel.Warning, "RestLayer", nameof (TestRun), nameof (Query));
        int lazyInitialization;
        using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
          lazyInitialization = managementDatabase.GetDataspaceIdWithLazyInitialization(projectFromName.GuidId);
        string whereClause = pathQueryTranslator.GenerateWhereClause(lazyInitialization);
        string orderClause = pathQueryTranslator.GenerateOrderClause();
        List<KeyValuePair<int, string>> valueLists = pathQueryTranslator.GenerateValueLists();
        Dictionary<int, string> iterationMap = new Dictionary<int, string>();
        Dictionary<Guid, List<TestRun>> projectsRunsMap = new Dictionary<Guid, List<TestRun>>();
        List<TestRun> testRunList;
        using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
          testRunList = managementDatabase.QueryTestRuns2(whereClause, orderClause, valueLists, out iterationMap, out projectsRunsMap);
        TestRun.UpdateProjectDataForRuns(context, projectsRunsMap);
        TestRun.UpdateIterationPathsForRuns(context, iterationMap, testRunList);
        TestRun.TrimFieldsForRuns(context, testRunList);
        TestRunBase.ResolveUserNames<TestRun>(context, testRunList);
        if (includeStatistics)
        {
          List<int> testRunIds = new List<int>(testRunList.Count);
          foreach (TestRun testRun in testRunList)
            testRunIds.Add(testRun.TestRunId);
          Dictionary<int, List<TestRunStatistic>> dictionary;
          using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
            dictionary = managementDatabase.QueryTestRunStatistics(testRunIds, projectFromName.GuidId, context.IsTcmService);
          foreach (TestRun testRun in testRunList)
          {
            if (dictionary.ContainsKey(testRun.TestRunId))
              testRun.TestRunStatistics = dictionary[testRun.TestRunId].ToArray();
          }
        }
        TestRunBase.PopulateVersion<TestRun>((IList<TestRun>) testRunList);
        return testRunList;
      }
    }

    internal static List<TestRun> Query2(
      TestManagementRequestContext context,
      int testRunId,
      Guid owner,
      string buildUri,
      GuidAndString projectId,
      int planId = -1,
      int skip = 0,
      int top = 2147483647)
    {
      return TestRun.Query2(context, testRunId, owner, buildUri, projectId.GuidId, planId, skip, top);
    }

    internal static List<TestRun> Query2(
      TestManagementRequestContext context,
      int testRunId,
      Guid owner,
      string buildUri,
      Guid projectId,
      int planId = -1,
      int skip = 0,
      int top = 2147483647)
    {
      using (PerfManager.Measure(context.RequestContext, "BusinessLayer", "TestRun.Query2(WithRunIdAndProjctName)"))
      {
        context.TraceVerbose("BusinessLayer", "TestRun.Query2:: Querying test run. testRunId = {0}. ProjectId = {1}", (object) testRunId, (object) projectId);
        List<TestRun> runs = new List<TestRun>();
        using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
          runs = managementDatabase.QueryTestRuns3(projectId, testRunId, owner, buildUri, planId, skip, top, context.IsTcmService);
        TestRunBase.ResolveUserNames<TestRun>(context, runs);
        TestRun.TrimFieldsForRuns(context, runs);
        context.TraceVerbose("BusinessLayer", "TestRun.Query2:: Queried test runs successfully. TestRunsCount = {0}. TestRunId = {1} ProjectId = {2}", (object) runs.Count, (object) testRunId, (object) projectId);
        return runs;
      }
    }

    internal static List<int> QueryTestRunIds(
      TestManagementRequestContext context,
      ResultsStoreQuery query)
    {
      using (PerfManager.Measure(context.RequestContext, "BusinessLayer", "TestRun.QueryTestRunIds"))
      {
        context.TraceInfo("BusinessLayer", "TestRun.QueryTestRunIds:: Querying test run. query = {0}.", (object) query);
        ArgumentUtility.CheckForNull<ResultsStoreQuery>(query, nameof (query), "Test Results");
        GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, query.TeamProjectName);
        if (!context.SecurityManager.HasViewTestResultsPermission(context, projectFromName.String))
          return new List<int>();
        context.TraceVerbose("BusinessLayer", "TestRun.QueryTestRunIds: Fetching test run ids.");
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        IterationPathQueryTranslator pathQueryTranslator = new IterationPathQueryTranslator(context, query, TestRun.\u003C\u003EO.\u003C0\u003E__TestRunQueryBuildUriTranslator ?? (TestRun.\u003C\u003EO.\u003C0\u003E__TestRunQueryBuildUriTranslator = new Func<string, string, string>(TestRun.TestRunQueryBuildUriTranslator)));
        if (context.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableQueryRunThrowException"))
        {
          if (pathQueryTranslator.Root.Where == null)
            throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.WIQLNoFilterError));
        }
        else if (pathQueryTranslator.Root.Where == null)
          context.RequestContext.Trace(1015762, TraceLevel.Warning, "RestLayer", nameof (TestRun), nameof (QueryTestRunIds));
        int lazyInitialization;
        using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
          lazyInitialization = managementDatabase.GetDataspaceIdWithLazyInitialization(projectFromName.GuidId);
        string whereClause = pathQueryTranslator.GenerateWhereClause(lazyInitialization);
        string orderClause = pathQueryTranslator.GenerateOrderClause();
        List<KeyValuePair<int, string>> valueLists = pathQueryTranslator.GenerateValueLists();
        using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
          return managementDatabase.QueryTestRunIds(whereClause, orderClause, valueLists);
      }
    }

    internal static List<TestRun> QueryInMultipleProjects(
      TestManagementRequestContext context,
      ResultsStoreQuery query)
    {
      context.TraceVerbose("BusinessLayer", "TestRun.QueryInMultipleProjects:: Querying test runs. query = {0}", (object) query.QueryText);
      List<TestRun> testRunList = (List<TestRun>) null;
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      IterationPathQueryTranslator pathQueryTranslator = new IterationPathQueryTranslator(context, query, TestRun.\u003C\u003EO.\u003C0\u003E__TestRunQueryBuildUriTranslator ?? (TestRun.\u003C\u003EO.\u003C0\u003E__TestRunQueryBuildUriTranslator = new Func<string, string, string>(TestRun.TestRunQueryBuildUriTranslator)));
      query.QueryText = pathQueryTranslator.TranslateQuery();
      List<KeyValuePair<int, string>> valueLists = pathQueryTranslator.GenerateValueLists();
      string multipleProjects = pathQueryTranslator.GenerateWhereClauseInMultipleProjects();
      string orderClause = pathQueryTranslator.GenerateOrderClause();
      Dictionary<int, string> iterationMap = new Dictionary<int, string>();
      Dictionary<Guid, List<TestRun>> projectsRunsMap = new Dictionary<Guid, List<TestRun>>();
      using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
        testRunList = managementDatabase.QueryTestRuns2(multipleProjects, orderClause, valueLists, out iterationMap, out projectsRunsMap);
      TestRun.UpdateProjectDataForRuns(context, projectsRunsMap);
      TestRun.UpdateIterationPathsForRuns(context, iterationMap, testRunList);
      context.TraceVerbose("BusinessLayer", "TestRun.QueryInMultipleProjects:: Queried test runs successfully. TestRunsCount = {0}. query = {1}", (object) testRunList.Count, (object) query.QueryText);
      TestRun.ProcessQueryResults(context, testRunList);
      TestRunBase.ResolveUserNames<TestRun>(context, testRunList);
      TestRunBase.PopulateVersion<TestRun>((IList<TestRun>) testRunList);
      return testRunList;
    }

    internal static int QueryCount(TestManagementRequestContext context, ResultsStoreQuery query)
    {
      ArgumentUtility.CheckForNull<ResultsStoreQuery>(query, nameof (query), "Test Results");
      GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, query.TeamProjectName);
      if (!context.SecurityManager.HasViewTestResultsPermission(context, projectFromName.String))
        return 0;
      IterationPathQueryTranslator pathQueryTranslator = new IterationPathQueryTranslator(context, query);
      pathQueryTranslator.TranslateQuery();
      int lazyInitialization;
      using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
        lazyInitialization = managementDatabase.GetDataspaceIdWithLazyInitialization(projectFromName.GuidId);
      string whereClause = pathQueryTranslator.GenerateWhereClause(lazyInitialization);
      List<KeyValuePair<int, string>> valueLists = pathQueryTranslator.GenerateValueLists();
      using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
        return managementDatabase.QueryObjectsCount(whereClause, valueLists, "vw_TestRun");
    }

    internal static List<TestMessageLogEntry> QueryLogEntriesForRun(
      TestManagementRequestContext context,
      int testRunId,
      int testMessageLogId,
      string projectName)
    {
      GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, projectName);
      if (!context.SecurityManager.HasViewTestResultsPermission(context, projectFromName.String))
        return new List<TestMessageLogEntry>();
      List<TestMessageLogEntry> source;
      using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
        source = managementDatabase.QueryLogEntriesForRun(projectFromName.GuidId, testRunId, testMessageLogId);
      Dictionary<Guid, string> dictionary = IdentityHelper.ResolveIdentities(context, source.Select<TestMessageLogEntry, Guid>((Func<TestMessageLogEntry, Guid>) (e => e.LogUser)));
      foreach (TestMessageLogEntry testMessageLogEntry in source)
      {
        if (dictionary.ContainsKey(testMessageLogEntry.LogUser))
          testMessageLogEntry.LogUserName = dictionary[testMessageLogEntry.LogUser];
      }
      return source;
    }

    internal static List<int> CreateLogEntriesForRun(
      TestManagementRequestContext context,
      int testRunId,
      TestMessageLogEntry[] entries,
      string projectName)
    {
      ArgumentUtility.CheckForNull<TestMessageLogEntry[]>(entries, nameof (entries), "Test Results");
      GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, projectName);
      context.SecurityManager.CheckPublishTestResultsPermission(context, projectFromName.String);
      ArgumentUtility.CheckEnumerableForEmpty((IEnumerable) entries, nameof (entries), "Test Results");
      int testMessageLogId = entries[0].TestMessageLogId;
      for (int index = 1; index < entries.Length; ++index)
      {
        if (entries[index].TestMessageLogId != testMessageLogId)
          throw new TestManagementValidationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.InvalidFieldValue, (object) "TestMessageLogEntry.TestMessageLogId"));
      }
      using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
        return managementDatabase.CreateLogEntriesForRun(projectFromName.GuidId, testRunId, testMessageLogId, entries);
    }

    internal static TestRun QueryTestRunByTmiRunId(
      TestManagementRequestContext context,
      Guid tmiRunId)
    {
      string iterationUri;
      Guid runProjGuid;
      TestRun testRun;
      using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
        testRun = managementDatabase.QueryTestRunByTmiRunId(tmiRunId, out iterationUri, out runProjGuid);
      TestRun.UpdateProjectDataForRun(context, runProjGuid, testRun);
      TestRun.UpdateIterationPathForRun(context, iterationUri, testRun);
      if (testRun == null || !context.SecurityManager.HasViewTestResultsPermission(context, testRun.TeamProjectUri))
        return (TestRun) null;
      testRun.OwnerName = IdentityHelper.ResolveIdentityToName(context, testRun.Owner);
      testRun.LastUpdatedByName = IdentityHelper.ResolveIdentityToName(context, testRun.LastUpdatedBy);
      TestRunBase.PopulateVersion((TestRunBase) testRun);
      return testRun;
    }

    internal static List<TestRun> QueryTestRuns(
      TestManagementRequestContext context,
      Guid projectId,
      QueryTestRunsFilter testRunFilter,
      int top,
      int batchSize,
      out DateTime continuationTokenLastUpdated)
    {
      using (PerfManager.Measure(context.RequestContext, "BusinessLayer", "TestRun.QueryTestRuns(WithRunFilterAndProjctGuid)"))
      {
        context.TraceVerbose("BusinessLayer", "TestRun.QueryTestRuns:: Querying test runs. ProjectGuid = {0}", (object) projectId.ToString());
        continuationTokenLastUpdated = DateTime.MaxValue;
        List<TestRun> testRunList = new List<TestRun>();
        using (TestManagementDatabase replicaAwareComponent = TestManagementDatabase.CreateReadReplicaAwareComponent(context.RequestContext))
        {
          DateTime minNextBatchLastUpdated;
          testRunList = replicaAwareComponent.QueryTestRunsbyFilters(projectId, testRunFilter, top, batchSize, out int _, out minNextBatchLastUpdated);
          if (minNextBatchLastUpdated != DateTime.MaxValue)
            continuationTokenLastUpdated = minNextBatchLastUpdated;
        }
        TestRunBase.ResolveUserNames<TestRun>(context, testRunList);
        TestRunBase.PopulateVersion<TestRun>((IList<TestRun>) testRunList);
        Dictionary<int, List<RunSummaryByOutcome>> dictionary;
        using (TestManagementDatabase replicaAwareComponent = TestManagementDatabase.CreateReadReplicaAwareComponent(context.RequestContext))
          dictionary = replicaAwareComponent.QueryTestRunsOutComeSummary(projectId, (IList<int>) testRunList.Select<TestRun, int>((Func<TestRun, int>) (r => r.TestRunId)).ToList<int>());
        foreach (TestRun testRun in testRunList)
        {
          List<RunSummaryByOutcome> valueOrDefault = dictionary.GetValueOrDefault<int, List<RunSummaryByOutcome>>(testRun.TestRunId, (List<RunSummaryByOutcome>) null);
          if (valueOrDefault != null)
            testRun.TestRunStatistics = valueOrDefault.Select<RunSummaryByOutcome, TestRunStatistic>((Func<RunSummaryByOutcome, TestRunStatistic>) (r => new TestRunStatistic()
            {
              Outcome = (byte) r.TestOutcome,
              Count = r.ResultCount
            })).ToArray<TestRunStatistic>();
        }
        TestRun.TrimFieldsForRuns(context, testRunList);
        context.TraceVerbose("BusinessLayer", "TestRun.QueryTestRuns:: Queried test runs successfully. TestRunsCount = {0}. ProjectGuid = {1}", (object) testRunList.Count, (object) projectId.ToString());
        return testRunList;
      }
    }

    internal static void FireNotification(
      TestManagementRequestContext context,
      int testRunId,
      string projectName)
    {
      context.EventService.PublishNotification(context.RequestContext, (object) new TestRunChangedNotification(testRunId, projectName));
      context.TraceInfo("BusinessLayer", "TestRunChangedNotification is fired. TestRunId = {0}, ProjectName = {1}", (object) testRunId, (object) projectName);
    }

    internal static void FireRunStartedNotification(
      TestManagementRequestContext context,
      TestRun testRun)
    {
      context.EventService.PublishNotification(context.RequestContext, (object) new TestRunStartedNotification(testRun));
      context.TraceInfo("BusinessLayer", "TestRunStartedNotification is fired. TestRunId = {0}", (object) testRun.TestRunId);
    }

    internal static void FireRunCompletedNotification(
      TestManagementRequestContext context,
      TestRun testRun)
    {
      context.EventService.PublishNotification(context.RequestContext, (object) new TestRunCompletedNotification(testRun));
      context.TraceInfo("BusinessLayer", "TestRunCompletedNotification is fired. TestRunId = {0}", (object) testRun.TestRunId);
    }

    internal static void FireRunCancelRequestedNotification(
      TestManagementRequestContext context,
      TestRun testRun)
    {
      context.EventService.PublishNotification(context.RequestContext, (object) new TestRunCanceledNotification(testRun));
      context.TraceInfo("BusinessLayer", "TestRunCanceledNotification is fired. TestRunId = {0}", (object) testRun.TestRunId);
    }

    internal static void FireRunCreatedNotification(
      TestManagementRequestContext context,
      TestRun testRun)
    {
      context.EventService.PublishNotification(context.RequestContext, (object) new TestRunCreatedNotification(testRun));
      context.TraceInfo("BusinessLayer", "TestRunCreatedNotification is fired. TestRunId = {0}", (object) testRun.TestRunId);
    }

    internal static List<TestRun> FilterNotOfType(IEnumerable<TestRun> runs, TestRunType type) => runs.Where<TestRun>((Func<TestRun, bool>) (r => ((int) r.Type & (int) (byte) type) == 0)).ToList<TestRun>();

    internal void ThrowInvalidOperationIfRunHasDtlEnvironment()
    {
      if (this.RunHasDtlEnvironment)
        throw new TestManagementInvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.InvalidPropertyMessage, (object) "Type"));
    }

    internal static void LogRunCompletedTelemetry(
      TestManagementRequestContext context,
      TestRun testRun)
    {
      double num1 = 0.0;
      if (!testRun.StartDate.Equals(DateTime.MinValue) && !testRun.CompleteDate.Equals(DateTime.MinValue) && testRun.CompleteDate > testRun.StartDate)
        num1 = testRun.CompleteDate.Subtract(testRun.StartDate).TotalMilliseconds;
      CustomerIntelligenceData cid = new CustomerIntelligenceData();
      cid.Add("TestRunId", testRun.TestRunId.ToString());
      cid.Add("TestRunState", Enum.GetName(typeof (Microsoft.TeamFoundation.TestManagement.Client.TestRunState), (object) testRun.State));
      CustomerIntelligenceData intelligenceData1 = cid;
      int num2 = testRun.TotalTests;
      string str1 = num2.ToString();
      intelligenceData1.Add("TotalTests", str1);
      CustomerIntelligenceData intelligenceData2 = cid;
      num2 = testRun.PassedTests;
      string str2 = num2.ToString();
      intelligenceData2.Add("PassedTests", str2);
      CustomerIntelligenceData intelligenceData3 = cid;
      num2 = testRun.FailedTests;
      string str3 = num2.ToString();
      intelligenceData3.Add("FailedTests", str3);
      cid.Add("RunDurationMs", num1);
      testRun.TelemetryLogger.PublishData(context.RequestContext, "TestRunCompleted", cid);
    }

    internal static void UpdateIterationPathForRun(
      TestManagementRequestContext context,
      string iterationUri,
      TestRun run)
    {
      if (run == null || string.IsNullOrEmpty(iterationUri))
        return;
      List<TcmCommonStructureNodeInfo> nodes = context.CSSHelper.GetNodes(new List<string>()
      {
        iterationUri
      });
      if (nodes == null || !nodes.Any<TcmCommonStructureNodeInfo>())
        return;
      run.Iteration = nodes.First<TcmCommonStructureNodeInfo>().Path;
    }

    internal static void UpdateIterationPathsForRuns(
      TestManagementRequestContext context,
      Dictionary<int, string> runIterationMap,
      List<TestRun> runs)
    {
      if (runIterationMap == null || !runIterationMap.Any<KeyValuePair<int, string>>())
        return;
      Dictionary<string, string> dictionary = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      foreach (KeyValuePair<int, string> runIteration in runIterationMap)
        dictionary[runIteration.Value] = string.Empty;
      List<TcmCommonStructureNodeInfo> nodes = context.CSSHelper.GetNodes(dictionary.Keys.ToList<string>());
      if (nodes == null || !nodes.Any<TcmCommonStructureNodeInfo>())
        return;
      foreach (TcmCommonStructureNodeInfo structureNodeInfo in nodes)
        dictionary[structureNodeInfo.Uri] = structureNodeInfo.Path;
      foreach (TestRun run in runs)
      {
        if (runIterationMap.ContainsKey(run.TestRunId))
        {
          string runIteration = runIterationMap[run.TestRunId];
          run.Iteration = dictionary[runIteration];
        }
      }
    }

    internal static void UpdateProjectDataForRun(
      TestManagementRequestContext context,
      Guid projectId,
      TestRun run)
    {
      if (!(projectId != Guid.Empty))
        return;
      ProjectInfo projectFromGuid = context.ProjectServiceHelper.GetProjectFromGuid(projectId);
      run.TeamProject = projectFromGuid.Name;
      run.TeamProjectUri = projectFromGuid.Uri;
    }

    internal static void UpdateProjectDataForRuns(
      TestManagementRequestContext context,
      Dictionary<Guid, List<TestRun>> projectsRunsMap)
    {
      if (projectsRunsMap == null || !projectsRunsMap.Any<KeyValuePair<Guid, List<TestRun>>>())
        return;
      foreach (Guid key in projectsRunsMap.Keys)
      {
        ProjectInfo projectFromGuid = context.ProjectServiceHelper.GetProjectFromGuid(key);
        foreach (TestRun testRun in projectsRunsMap[key])
        {
          testRun.TeamProject = projectFromGuid.Name;
          testRun.TeamProjectUri = projectFromGuid.Uri;
        }
      }
    }

    private static List<TestRun> GetNonDtlEnvRuns(List<TestRun> runs)
    {
      List<TestRun> nonDtlEnvRuns = new List<TestRun>();
      foreach (TestRun run in runs)
      {
        if (!run.RunHasDtlEnvironment)
          nonDtlEnvRuns.Add(run);
      }
      return nonDtlEnvRuns;
    }

    private static string TestRunQueryBuildUriTranslator(string nodeName, string nodeValue) => StringComparer.OrdinalIgnoreCase.Equals("BuildUri", nodeName) ? Validator.TranslateBuildUri(nodeValue) : nodeValue;

    private static void ProcessQueryResults(
      TestManagementRequestContext context,
      List<TestRun> runs)
    {
      int count = runs.Count;
      Dictionary<string, bool> permissionTable = new Dictionary<string, bool>(count);
      runs.RemoveAll((Predicate<TestRun>) (run =>
      {
        if (!permissionTable.ContainsKey(run.TeamProjectUri))
          permissionTable[run.TeamProjectUri] = !context.SecurityManager.HasViewTestResultsPermission(context, run.TeamProjectUri);
        return permissionTable[run.TeamProjectUri];
      }));
      context.TraceInfo("BusinessLayer", "Filtered out {0} runs. Returning {1} runs.", (object) (count - runs.Count), (object) runs.Count);
    }

    private static void CheckBuildExists(
      TestManagementRequestContext context,
      Guid projectId,
      string buildUri)
    {
      using (PerfManager.Measure(context.RequestContext, "BusinessLayer", "TestRun.CheckBuildExists"))
      {
        if (string.IsNullOrEmpty(buildUri))
          return;
        context.TraceInfo("BusinessLayer", "TestRun.CheckBuildExists for {0} - start", (object) buildUri);
        if (TestRun.BuildServiceHelper.QueryBuildByUri(context.RequestContext, projectId, buildUri, false) == null)
          throw new TestObjectNotFoundException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.BuildNotFound, (object) buildUri), ObjectTypes.AssociatedBuild);
        context.TraceInfo("BusinessLayer", "TestRun.CheckBuildExists end");
      }
    }

    internal static IBuildServiceHelper BuildServiceHelper
    {
      get => TestRun.m_buildServiceHelper ?? (IBuildServiceHelper) new Microsoft.TeamFoundation.TestManagement.Server.BuildServiceHelper();
      set => TestRun.m_buildServiceHelper = value;
    }

    internal static IReleaseServiceHelper ReleaseServiceHelper
    {
      get => TestRun.m_releaseServiceHelper ?? (IReleaseServiceHelper) new Microsoft.TeamFoundation.TestManagement.Server.ReleaseServiceHelper();
      set => TestRun.m_releaseServiceHelper = value;
    }

    private static string UpdateQueryTextToIncludeWebRunsIfNeeded(
      IVssRequestContext context,
      string queryText)
    {
      return context != null && !string.IsNullOrWhiteSpace(context.UserAgent) && (context.UserAgent.IndexOf("mtm.exe", StringComparison.InvariantCultureIgnoreCase) >= 0 || context.IsImpersonating) && !string.IsNullOrWhiteSpace(queryText) && queryText.Contains("[System.Type] = 'Normal'") ? queryText.Replace("[System.Type] = 'Normal'", "([System.Type] = 'Normal' OR [System.Type] = 'Web' OR [System.Type] = 'MtrRunInitiatedFromWeb')") : queryText;
    }
  }
}
