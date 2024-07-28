// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestPlanning.RevisedTestPlansHelper
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.Client;
using Microsoft.TeamFoundation.TestManagement.Common;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.TestManagement.Server.TestPlanning
{
  internal class RevisedTestPlansHelper : TestPlansHelper
  {
    public RevisedTestPlansHelper(TestManagementRequestContext requestContext)
      : base(requestContext)
    {
    }

    public virtual Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestPlan GetTestPlanById(
      string projectId,
      int planId)
    {
      return this.ExecuteAction<Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestPlan>("RevisedTestPlansHelper.GetTestPlanById", (Func<Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestPlan>) (() =>
      {
        this.ValidateProjectArgument(projectId);
        TeamProjectReference projectReference = this.GetProjectReference(projectId);
        return this.ConvertToTestPlanningDataContract(this.GetPlanByIdHelper(projectId, planId, projectReference), projectReference);
      }), 1015053, "TestManagement");
    }

    public bool UserHasTestPlans(string projectId) => this.HasTestPlans(projectId);

    public List<Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestPlan> GetPlans(
      string projectId,
      out bool generateContinuationToken,
      string owner = "",
      int skip = 0,
      int top = 2147483647,
      int watermark = 0,
      bool includePlanDetails = true,
      bool filterActivePlans = false)
    {
      bool _generateContinuationToken = false;
      List<Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestPlan> plans1 = this.ExecuteAction<List<Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestPlan>>("RevisedTestPlansHelper.GetPlans", (Func<List<Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestPlan>>) (() =>
      {
        this.ValidateProjectArgument(projectId);
        TeamProjectReference projectReference = this.GetProjectReference(projectId);
        List<Microsoft.TeamFoundation.TestManagement.Server.TestPlan> testPlanList = !this.RequestContext.IsFeatureEnabled("TestManagement.Server.ImproveGetTestPlansApiPerformance") ? this.GetServerPlans(projectId, owner, skip, top, watermark, includePlanDetails, filterActivePlans, projectReference) : this.GetServerPlansImproved(projectId, owner, skip, top, watermark, includePlanDetails, filterActivePlans, projectReference);
        Dictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity> identityIdVsIdentityMap = this.GetIdentityIdVsIdentityMap(this.ReadIdentitesByAccountId(this.GetUniqueIdentityIdsFromTestPlanList(testPlanList)));
        if (testPlanList == null)
          testPlanList = new List<Microsoft.TeamFoundation.TestManagement.Server.TestPlan>();
        List<Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestPlan> plans2 = new List<Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestPlan>(testPlanList.Count);
        if (!testPlanList.IsNullOrEmpty<Microsoft.TeamFoundation.TestManagement.Server.TestPlan>() && testPlanList.Count >= top)
          _generateContinuationToken = true;
        foreach (Microsoft.TeamFoundation.TestManagement.Server.TestPlan testPlan in testPlanList)
        {
          Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestPlan planningDataContract;
          try
          {
            planningDataContract = this.ConvertToTestPlanningDataContract(testPlan, projectReference, includePlanDetails, identityIdVsIdentityMap);
          }
          catch (Exception ex)
          {
            int planId = testPlan == null ? 0 : testPlan.PlanId;
            this.RequestContext.TraceError("RestLayer", "RevisedTestPlansHelper.GetPlans: ConvertToTestPlanningDataContract threw exception. projectId = {0}, testPlanId = {1}, Exception = {2}", (object) projectId, (object) planId, (object) ex.Message);
            continue;
          }
          if (planningDataContract != null)
            plans2.Add(planningDataContract);
        }
        return plans2;
      }), 1015053, "TestManagement");
      generateContinuationToken = _generateContinuationToken;
      return plans1;
    }

    internal virtual CloneTestPlanOperationInformation GetPlanCloneWrapper(
      string projectName,
      int cloneOperationId)
    {
      return ServerTestSuite.GetPlanCloneOperationInformation((TestManagementRequestContext) this.TfsTestManagementRequestContext, projectName, cloneOperationId);
    }

    internal virtual List<int> GetSuiteIdsWrapper(int cloneOperationId) => ServerTestSuite.GetSuiteIds((TestManagementRequestContext) this.TfsTestManagementRequestContext, cloneOperationId);

    public CloneTestPlanOperationInformation GetCloneInformation(
      int cloneOperationId,
      string projectName)
    {
      return this.ExecuteAction<CloneTestPlanOperationInformation>("RestHelper.GetCloneInformation", (Func<CloneTestPlanOperationInformation>) (() =>
      {
        CloneTestPlanOperationInformation planCloneWrapper = this.GetPlanCloneWrapper(projectName, cloneOperationId);
        List<int> suiteIdsWrapper = this.GetSuiteIdsWrapper(cloneOperationId);
        if (planCloneWrapper == null)
          throw new Microsoft.TeamFoundation.TestManagement.Server.TestObjectNotFoundException(this.RequestContext, cloneOperationId, ObjectTypes.TestPlanClone);
        this.ValidateProjectArgument(projectName);
        SourceTestplanResponse sourceTestPlan = planCloneWrapper.sourceTestPlan;
        Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestPlan testPlan = new Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestPlan();
        if (sourceTestPlan != null)
        {
          Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestPlan testPlanById = this.GetTestPlanById(projectName, sourceTestPlan.Id);
          if (testPlanById != null)
            sourceTestPlan.Name = testPlanById.Name;
          sourceTestPlan.Project = this.GetProjectReference(projectName);
          sourceTestPlan.suiteIds = suiteIdsWrapper;
          planCloneWrapper.sourceTestPlan = sourceTestPlan;
        }
        Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestPlan destinationTestPlan = planCloneWrapper.destinationTestPlan;
        if (destinationTestPlan != null && destinationTestPlan.Project != null)
          planCloneWrapper.destinationTestPlan = this.GetTestPlanById(destinationTestPlan.Project.Name, destinationTestPlan.Id);
        if (planCloneWrapper.cloneOperationResponse != null)
        {
          planCloneWrapper.cloneOperationResponse.links = new ReferenceLinks();
          planCloneWrapper.cloneOperationResponse.links.AddLink("_self", UrlBuildHelper.GetResourceUrl(this.RequestContext, ServiceInstanceTypes.TFS, "testplan", TestPlanResourceIds.TestPlanClone, (object) new
          {
            project = projectName,
            cloneOperationId = cloneOperationId
          }));
          if (planCloneWrapper.sourceTestPlan != null)
            planCloneWrapper.cloneOperationResponse.links.AddLink("sourcePlan", UrlBuildHelper.GetResourceUrl(this.RequestContext, ServiceInstanceTypes.TFS, "testplan", TestPlanResourceIds.TestPlan, (object) new
            {
              project = projectName,
              planId = planCloneWrapper.sourceTestPlan.Id
            }));
          if (destinationTestPlan != null && destinationTestPlan.Project != null && planCloneWrapper.destinationTestPlan != null)
            planCloneWrapper.cloneOperationResponse.links.AddLink("destinationPlan", UrlBuildHelper.GetResourceUrl(this.RequestContext, ServiceInstanceTypes.TFS, "testplan", TestPlanResourceIds.TestPlan, (object) new
            {
              project = destinationTestPlan.Project.Name,
              planId = planCloneWrapper.destinationTestPlan.Id
            }));
          planCloneWrapper.cloneOperationResponse.links.AddLink("sourceProject", this.ProjectServiceHelper.GetProjectRepresentation(projectName).Url);
          if (destinationTestPlan != null && destinationTestPlan.Project != null)
            planCloneWrapper.cloneOperationResponse.links.AddLink("destinationProject", this.ProjectServiceHelper.GetProjectRepresentation(destinationTestPlan.Project.Name).Url);
        }
        return planCloneWrapper;
      }), 1015050, "TestManagement");
    }

    public CloneTestPlanOperationInformation CreateAndBeginNewCloneOfTestPlan(
      string sourceProjectName,
      int sourcePlanId,
      DestinationTestPlanCloneParams plan,
      List<int> sourceSuiteIds,
      Microsoft.TeamFoundation.TestManagement.WebApi.CloneOptions cloneOptions,
      bool deepClone)
    {
      ArgumentUtility.CheckForNull<DestinationTestPlanCloneParams>(plan, "destinationPlan", this.RequestContext.ServiceName);
      ArgumentUtility.CheckForNull<Microsoft.TeamFoundation.TestManagement.WebApi.CloneOptions>(cloneOptions, nameof (cloneOptions), this.RequestContext.ServiceName);
      return this.ExecuteAction<CloneTestPlanOperationInformation>("PlansHelper.CreateAndBeginCloneOfTestPlan", (Func<CloneTestPlanOperationInformation>) (() =>
      {
        new Microsoft.TeamFoundation.TestManagement.WebApi.ShallowReference().Name = plan.AreaPath;
        string str = string.IsNullOrEmpty(plan.Project) ? sourceProjectName : plan.Project;
        Microsoft.TeamFoundation.TestManagement.Server.CloneOptions cloneOptions1 = Microsoft.TeamFoundation.TestManagement.Server.CloneOptions.ConvertToCloneOptions(cloneOptions);
        TeamProjectReference projectReference = this.GetProjectReference(str);
        Microsoft.TeamFoundation.TestManagement.Server.TestPlan serverTestPlan = this.CreateServerTestPlan(str, (TestPlanCreateParams) plan, projectReference);
        if (serverTestPlan == null)
          throw new ArgumentException("TestPlanCreateParams");
        int num = this.BeginCloneOperationWrapper(sourceProjectName, str, sourcePlanId, serverTestPlan, sourceSuiteIds, cloneOptions1, deepClone, plan.AreaPath);
        CloneTestPlanOperationInformation newCloneOfTestPlan = new CloneTestPlanOperationInformation();
        newCloneOfTestPlan.cloneOptions = cloneOptions;
        newCloneOfTestPlan.sourceTestPlan = new SourceTestplanResponse()
        {
          Id = sourcePlanId,
          Name = this.GetPlanName(sourcePlanId, sourceProjectName),
          suiteIds = sourceSuiteIds,
          Project = this.GetProjectReference(sourceProjectName)
        };
        Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestPlan testPlan = new Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestPlan();
        newCloneOfTestPlan.destinationTestPlan = this.ConvertToTestPlanningDataContract(serverTestPlan, projectReference);
        CloneOperationCommonResponse operationCommonResponse = new CloneOperationCommonResponse();
        operationCommonResponse.opId = num;
        operationCommonResponse.state = Microsoft.TeamFoundation.TestManagement.WebApi.CloneOperationState.Queued;
        operationCommonResponse.links = new ReferenceLinks();
        operationCommonResponse.links.AddLink("_self", UrlBuildHelper.GetResourceUrl(this.RequestContext, ServiceInstanceTypes.TFS, "testplan", TestPlanResourceIds.TestPlanClone, (object) new
        {
          project = sourceProjectName,
          cloneOperationId = num
        }));
        newCloneOfTestPlan.cloneOperationResponse = operationCommonResponse;
        return newCloneOfTestPlan;
      }), 1015053, "TestManagement");
    }

    public Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestPlan CreateTestPlan(
      string projectId,
      TestPlanCreateParams testPlanCreateParams)
    {
      return this.ExecuteAction<Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestPlan>("RevisedTestPlansHelper.CreateTestPlan", (Func<Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestPlan>) (() =>
      {
        this.ValidateProjectArgument(projectId);
        TeamProjectReference projectReference = this.GetProjectReference(projectId);
        return this.ConvertToTestPlanningDataContract(this.CreateServerTestPlan(projectId, testPlanCreateParams, projectReference), projectReference);
      }), 1015053, "TestManagement");
    }

    public Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestPlan PatchTestPlan(
      string projectId,
      int planId,
      TestPlanUpdateParams testPlanUpdateParams)
    {
      return this.ExecuteAction<Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestPlan>("RevisedTestPlansHelper.PatchTestPlan", (Func<Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestPlan>) (() =>
      {
        this.ValidateProjectArgument(projectId);
        ArgumentUtility.CheckForNull<TestPlanUpdateParams>(testPlanUpdateParams, nameof (testPlanUpdateParams), this.RequestContext.ServiceName);
        TeamProjectReference projectReference = this.GetProjectReference(projectId);
        return this.ConvertToTestPlanningDataContract(this.PatchServerTestPlan(this.ConvertUpdateParamsToServerContract(this.ValidateTestPlanPatch(projectReference, planId, testPlanUpdateParams.StartDate, testPlanUpdateParams.EndDate), testPlanUpdateParams, projectReference), projectReference, planId, testPlanUpdateParams.TestOutcomeSettings), projectReference);
      }), 1015053, "TestManagement");
    }

    internal virtual Microsoft.TeamFoundation.TestManagement.Server.TestPlan CreateServerTestPlan(
      string projectId,
      TestPlanCreateParams testPlanCreateParams,
      TeamProjectReference projectReference)
    {
      return this.ExecuteAction<Microsoft.TeamFoundation.TestManagement.Server.TestPlan>("RevisedTestPlansHelper.CreateServerTestPlan", (Func<Microsoft.TeamFoundation.TestManagement.Server.TestPlan>) (() =>
      {
        this.ValidateProjectArgument(projectId);
        ArgumentUtility.CheckForNull<TestPlanCreateParams>(testPlanCreateParams, nameof (testPlanCreateParams), this.RequestContext.ServiceName);
        this.ValidateTestPlanCreate(testPlanCreateParams.StartDate, testPlanCreateParams.EndDate);
        Microsoft.TeamFoundation.TestManagement.Server.TestPlan testPlan = new Microsoft.TeamFoundation.TestManagement.Server.TestPlan();
        this.ConvertCreatePramasToServerContract(testPlan, testPlanCreateParams, projectReference);
        return this.CreateServerTestPlan(testPlan, projectReference, testPlanCreateParams.TestOutcomeSettings);
      }), 1015053, "TestManagement");
    }

    internal virtual int GetBuildArtifactIdWrapper(string BuildUri) => this.BuildServiceHelper.GetBuildArtifactId(BuildUri);

    internal virtual Microsoft.TeamFoundation.TestManagement.WebApi.TestEnvironment GetTestEnvironmentWrapper(
      string projectName,
      Guid guid)
    {
      return this.EnvironmentsHelper.GetTestEnvironment(projectName, guid);
    }

    internal virtual Microsoft.TeamFoundation.TestManagement.WebApi.TestSettings GetTestSettingWrapper(
      string projectName,
      int settingsId)
    {
      return this.SettingsHelper.GetTestSetting(projectName, settingsId);
    }

    internal virtual Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestPlan ConvertToTestPlanningDataContract(
      Microsoft.TeamFoundation.TestManagement.Server.TestPlan testPlan,
      TeamProjectReference projectReference,
      bool includePlanDetails = true,
      Dictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity> identityIdVsIdentityMap = null)
    {
      if (testPlan == null)
        throw new Microsoft.TeamFoundation.TestManagement.Server.TestObjectNotFoundException(this.RequestContext, 0, ObjectTypes.TestPlan);
      string projectName = projectReference != null ? projectReference.Name : throw new ArgumentNullException(nameof (projectReference));
      Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestPlan planningDataContract = new Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestPlan();
      planningDataContract.Id = testPlan.PlanId;
      planningDataContract.Name = testPlan.Name;
      planningDataContract.AreaPath = testPlan.AreaPath;
      planningDataContract.Iteration = testPlan.Iteration;
      planningDataContract.Project = projectReference;
      planningDataContract.RootSuite = this.GetSuiteReference(projectName, testPlan.RootSuiteId, testPlan: testPlan);
      planningDataContract.State = testPlan.Status;
      planningDataContract.Links = new ReferenceLinks();
      planningDataContract.Links.AddLink("_self", UrlBuildHelper.GetResourceUrl(this.RequestContext, ServiceInstanceTypes.TFS, "testplan", TestPlanResourceIds.TestPlan, (object) new
      {
        planId = testPlan.PlanId,
        project = projectName
      }));
      planningDataContract.Links.AddLink("clientUrl", this.GetTestPlanClientUrl(testPlan.PlanId, projectName));
      planningDataContract.Links.AddLink("rootSuite", UrlBuildHelper.GetResourceUrl(this.RequestContext, ServiceInstanceTypes.TFS, "testplan", TestPlanResourceIds.TestSuiteProject, (object) new
      {
        suiteId = testPlan.RootSuiteId,
        planId = testPlan.PlanId,
        project = projectName
      }));
      if (!string.IsNullOrEmpty(testPlan.BuildUri))
      {
        int artifactIdWrapper = this.GetBuildArtifactIdWrapper(testPlan.BuildUri);
        planningDataContract.BuildId = artifactIdWrapper;
        planningDataContract.Links.AddLink("build", UrlBuildHelper.GetResourceUrl(this.RequestContext, ServiceInstanceTypes.TFS, "build", BuildResourceIds.Builds, (object) new
        {
          buildId = artifactIdWrapper,
          project = projectName
        }));
      }
      if (includePlanDetails)
      {
        planningDataContract.Description = testPlan.Description;
        planningDataContract.StartDate = new DateTime?(testPlan.StartDate);
        planningDataContract.EndDate = new DateTime?(testPlan.EndDate);
        planningDataContract.UpdatedDate = testPlan.LastUpdated;
        Microsoft.VisualStudio.Services.Identity.Identity identity1 = identityIdVsIdentityMap == null ? this.ReadIdentityByAccountId(testPlan.LastUpdatedBy) : (identityIdVsIdentityMap.ContainsKey(testPlan.LastUpdatedBy) ? identityIdVsIdentityMap[testPlan.LastUpdatedBy] : (Microsoft.VisualStudio.Services.Identity.Identity) null);
        if (testPlan.LastUpdatedBy != Guid.Empty)
          planningDataContract.UpdatedBy = identity1.ToIdentityRef(this.RequestContext);
        Microsoft.VisualStudio.Services.Identity.Identity identity2 = identityIdVsIdentityMap == null ? this.ReadIdentityByAccountId(testPlan.Owner) : (identityIdVsIdentityMap.ContainsKey(testPlan.Owner) ? identityIdVsIdentityMap[testPlan.Owner] : (Microsoft.VisualStudio.Services.Identity.Identity) null);
        if (identity2 != null)
          planningDataContract.Owner = identity2.ToIdentityRef(this.RequestContext);
        if (testPlan.PreviousBuildUri != null && !string.IsNullOrEmpty(testPlan.PreviousBuildUri))
        {
          int artifactIdWrapper = this.GetBuildArtifactIdWrapper(testPlan.PreviousBuildUri);
          planningDataContract.PreviousBuildId = artifactIdWrapper;
          planningDataContract.Links.AddLink("previousBuild", UrlBuildHelper.GetResourceUrl(this.RequestContext, ServiceInstanceTypes.TFS, "build", BuildResourceIds.Builds, (object) new
          {
            buildId = artifactIdWrapper,
            project = projectName
          }));
        }
        planningDataContract.Revision = testPlan.Revision;
        if (testPlan.AutomatedTestSettingsId != 0)
          planningDataContract.AutomatedTestSettings = this.GetTestSettingWrapper(projectName, testPlan.AutomatedTestSettingsId);
        if (testPlan.TestSettingsId != 0)
          planningDataContract.ManualTestSettings = this.GetTestSettingWrapper(projectName, testPlan.TestSettingsId);
        if (testPlan.BuildDefinitionId != 0)
        {
          planningDataContract.BuildDefinition = new Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.BuildDefinitionReference(testPlan.BuildDefinitionId);
          planningDataContract.Links.AddLink("buildDefinition", UrlBuildHelper.GetResourceUrl(this.RequestContext, ServiceInstanceTypes.TFS, "build", BuildResourceIds.Definitions, (object) new
          {
            definitionId = testPlan.BuildDefinitionId,
            project = projectName
          }));
        }
        if (testPlan.ReleaseEnvDef != null)
        {
          planningDataContract.ReleaseEnvironmentDefinition = new ReleaseEnvironmentDefinitionReference();
          planningDataContract.ReleaseEnvironmentDefinition.DefinitionId = testPlan.ReleaseEnvDef.ReleaseDefinitionId;
          planningDataContract.ReleaseEnvironmentDefinition.EnvironmentDefinitionId = testPlan.ReleaseEnvDef.ReleaseEnvDefinitionId;
        }
        bool acrossSuitesValue = this.GetSyncOutcomeAcrossSuitesValue(projectReference.Id, testPlan.PlanId.ToString());
        planningDataContract.TestOutcomeSettings = new TestOutcomeSettings();
        planningDataContract.TestOutcomeSettings.SyncOutcomeAcrossSuites = acrossSuitesValue;
      }
      return planningDataContract;
    }

    internal virtual Microsoft.TeamFoundation.TestManagement.Server.TestPlan ConvertUpdateParamsToServerContract(
      Microsoft.TeamFoundation.TestManagement.Server.TestPlan testPlan,
      TestPlanUpdateParams testPlanUpdateParams,
      TeamProjectReference projectReference)
    {
      ArgumentUtility.CheckForNull<TestPlanUpdateParams>(testPlanUpdateParams, nameof (testPlanUpdateParams), this.RequestContext.ServiceName);
      testPlan = this.ConvertCreatePramasToServerContract(testPlan, (TestPlanCreateParams) testPlanUpdateParams, projectReference, false);
      if (testPlanUpdateParams.Revision != 0)
        testPlan.Revision = testPlanUpdateParams.Revision;
      return testPlan;
    }

    internal virtual ProjectInfo GetProjectFromNameWrapper(string projectName) => this.TfsTestManagementRequestContext.ProjectServiceHelper.GetProjectFromName(projectName);

    internal virtual Microsoft.TeamFoundation.TestManagement.Server.BuildConfiguration QueryBuildConfigurationByIdWrapper(
      Guid projectId,
      int buildId)
    {
      return new Microsoft.TeamFoundation.TestManagement.Server.BuildServiceHelper().QueryBuildConfigurationById(this.RequestContext, projectId, buildId);
    }

    internal virtual string ConvertTcmStateToWorkItemStateWrapper(
      string teamProjectName,
      TestPlanState testPlanState,
      string categoryRefName)
    {
      return Compat2013UpdateHelper.ConvertTcmStateToWorkItemState((TestManagementRequestContext) this.TfsTestManagementRequestContext, teamProjectName, (byte) testPlanState, categoryRefName);
    }

    internal virtual string ConvertTcmStateToWorkItemStateWrapper(
      string teamProjectName,
      int workItemId,
      TestPlanState testPlanState,
      string categoryRefName)
    {
      return Compat2013UpdateHelper.ConvertTcmStateToWorkItemState((TestManagementRequestContext) this.TfsTestManagementRequestContext, teamProjectName, workItemId, (byte) testPlanState, categoryRefName);
    }

    internal virtual Microsoft.TeamFoundation.TestManagement.Server.TestPlan ConvertCreatePramasToServerContract(
      Microsoft.TeamFoundation.TestManagement.Server.TestPlan testPlan,
      TestPlanCreateParams testPlanCreateParams,
      TeamProjectReference projectReference,
      bool isCreate = true)
    {
      ArgumentUtility.CheckForNull<TestPlanCreateParams>(testPlanCreateParams, nameof (testPlanCreateParams), this.RequestContext.ServiceName);
      ArgumentUtility.CheckForNull<Microsoft.TeamFoundation.TestManagement.Server.TestPlan>(testPlan, nameof (testPlan), this.RequestContext.ServiceName);
      ArgumentUtility.CheckForNull<TeamProjectReference>(projectReference, nameof (projectReference), this.RequestContext.ServiceName);
      if (isCreate)
        ArgumentUtility.CheckStringForNullOrEmpty(testPlanCreateParams.Name, "planName", this.RequestContext.ServiceName);
      ProjectInfo projectFromNameWrapper = this.GetProjectFromNameWrapper(projectReference.Name);
      if (!string.IsNullOrEmpty(testPlanCreateParams.Name))
        testPlan.Name = testPlanCreateParams.Name.Trim();
      if (!string.IsNullOrEmpty(testPlanCreateParams.Description))
        testPlan.Description = testPlanCreateParams.Description.Trim();
      testPlan.AreaPath = string.IsNullOrEmpty(testPlanCreateParams.AreaPath) ? this.GetAreaPathForProject(projectFromNameWrapper.Uri) : testPlanCreateParams.AreaPath;
      DateTime dateTime1 = isCreate ? DateTime.UtcNow : DateTime.MinValue;
      DateTime dateTime2 = isCreate ? dateTime1.AddDays(7.0) : DateTime.MinValue;
      DateTime? nullable;
      if (!string.IsNullOrEmpty(testPlanCreateParams.Iteration))
      {
        testPlan.Iteration = testPlanCreateParams.Iteration;
        TcmCommonStructureNodeInfo fromIterationPath = this.TfsTestManagementRequestContext.CSSHelper.GetNodeFromIterationPath(testPlan.Iteration);
        nullable = fromIterationPath.StartDate;
        dateTime1 = nullable ?? dateTime1;
        nullable = fromIterationPath.FinishDate;
        dateTime2 = nullable ?? dateTime2;
      }
      else
        testPlan.Iteration = this.GetIterationPathForProject(projectFromNameWrapper.Uri);
      Microsoft.TeamFoundation.TestManagement.Server.TestPlan testPlan1 = testPlan;
      nullable = testPlanCreateParams.StartDate;
      DateTime dateTime3 = nullable ?? dateTime1;
      testPlan1.StartDate = dateTime3;
      Microsoft.TeamFoundation.TestManagement.Server.TestPlan testPlan2 = testPlan;
      nullable = testPlanCreateParams.EndDate;
      DateTime dateTime4 = nullable ?? dateTime2;
      testPlan2.EndDate = dateTime4;
      if (testPlanCreateParams.ReleaseEnvironmentDefinition != null)
      {
        if (testPlanCreateParams.ReleaseEnvironmentDefinition.DefinitionId < 0 || testPlanCreateParams.ReleaseEnvironmentDefinition.EnvironmentDefinitionId < 0)
          throw new InvalidPropertyException("ReleaseEnvironmentDefinition", Microsoft.TeamFoundation.TestManagement.Server.ServerResources.InvalidPropertyMessage);
        testPlan.ReleaseEnvDef = new ReleaseEnvironmentDefinition()
        {
          ReleaseDefinitionId = testPlanCreateParams.ReleaseEnvironmentDefinition.DefinitionId,
          ReleaseEnvDefinitionId = testPlanCreateParams.ReleaseEnvironmentDefinition.EnvironmentDefinitionId
        };
      }
      if (testPlanCreateParams.BuildId != 0)
      {
        string str = LinkingUtilities.EncodeUri(new ArtifactId("Build", "Build", testPlanCreateParams.BuildId.ToString()));
        int buildId = testPlanCreateParams.BuildId;
        if (buildId == 0)
        {
          testPlan.BuildUri = string.Empty;
        }
        else
        {
          Microsoft.TeamFoundation.TestManagement.Server.BuildConfiguration buildConfiguration = this.QueryBuildConfigurationByIdWrapper(projectReference.Id, buildId);
          if (buildConfiguration != null)
          {
            testPlan.BuildDefinition = buildConfiguration.BuildDefinitionName;
            testPlan.BuildDefinitionId = buildConfiguration.BuildDefinitionId;
            testPlan.BuildQuality = buildConfiguration.BuildQuality;
            testPlan.BuildUri = buildConfiguration.BuildUri;
          }
          else
            this.TfsTestManagementRequestContext.TraceWarning("RestLayer", Microsoft.TeamFoundation.TestManagement.Server.ServerResources.BuildsNotFound, (object) str);
        }
      }
      if (testPlanCreateParams.BuildDefinition != null)
      {
        int id = testPlanCreateParams.BuildDefinition.Id;
        if (testPlanCreateParams.BuildId != 0 && !string.IsNullOrEmpty(testPlan.BuildUri) && testPlan.BuildDefinitionId != id)
          throw new InvalidPropertyException("Build.Id", Microsoft.TeamFoundation.TestManagement.Server.ServerResources.InvalidPropertyMessage);
        testPlan.BuildDefinitionId = id;
      }
      if (testPlanCreateParams.State != null)
      {
        TestPlanState result;
        if (!Enum.TryParse<TestPlanState>(testPlanCreateParams.State, true, out result))
          throw new InvalidPropertyException("State", Microsoft.TeamFoundation.TestManagement.Server.ServerResources.InvalidPropertyMessage);
        testPlan.State = (byte) result;
        testPlan.Status = !isCreate ? this.ConvertTcmStateToWorkItemStateWrapper(projectReference.Name, testPlan.PlanId, result, WitCategoryRefName.TestPlan) : this.ConvertTcmStateToWorkItemStateWrapper(projectReference.Name, result, WitCategoryRefName.TestPlan);
      }
      if (testPlanCreateParams.Owner != null)
      {
        this.PopulateTestPlanOwnerInfo(testPlan, testPlanCreateParams.Owner);
      }
      else
      {
        Microsoft.VisualStudio.Services.Identity.Identity authenticatedIdentity = this.TestManagementRequestContext.RequestContext.GetAuthenticatedIdentity();
        if (authenticatedIdentity != null)
        {
          testPlan.Owner = authenticatedIdentity.Id;
          testPlan.OwnerName = authenticatedIdentity.DisplayName;
        }
      }
      return testPlan;
    }
  }
}
