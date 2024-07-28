// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.MigrateTcmDataToWit
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.Client;
using Microsoft.TeamFoundation.TestManagement.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal class MigrateTcmDataToWit
  {
    private TfsTestManagementRequestContext m_RequestContext;
    private CachedRegistryService m_registryService;
    private MigrationLogger m_logger;
    private int m_fetchLimit;
    private int m_retryCount;
    private int m_retriesLeft;
    private int m_updateArtifactLimit;
    private bool m_byPassWitValidation;
    private string m_teamProjectName;

    public MigrateTcmDataToWit(
      TestManagementRequestContext context,
      IServicingContext servicingContext,
      string teamProjectName)
    {
      this.m_teamProjectName = teamProjectName;
      this.m_RequestContext = new TfsTestManagementRequestContext(context.RequestContext);
      this.m_logger = new MigrationLogger(context, servicingContext);
      this.m_registryService = context.RequestContext.GetService<CachedRegistryService>();
      this.InitializeMigrationParameters();
    }

    public void MigrateData()
    {
      GuidAndString projectFromName = Validator.CheckAndGetProjectFromName((TestManagementRequestContext) this.m_RequestContext, this.m_teamProjectName);
      try
      {
        this.m_logger.Log(TraceLevel.Info, "TestManagement::MigrateData Start");
        if (!this.IsProjectMigrationRequired(this.m_RequestContext, projectFromName.GuidId))
          return;
        this.CleanUpDeletedTestPlan();
        this.DeleteSessionsAssociatedWithDeletedPlans(projectFromName.GuidId);
        this.UpdateProjectMigrationState(this.m_RequestContext, projectFromName.GuidId, UpgradeMigrationState.InProgress, (string) null);
        this.m_retriesLeft = this.m_retryCount;
        while (true)
        {
          this.m_logger.Log(TraceLevel.Info, "Fetching plans which need to be migrated");
          List<TestPlan> beMigratedOnWit;
          using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create((TestManagementRequestContext) this.m_RequestContext))
            beMigratedOnWit = planningDatabase.FetchPlansToBeMigratedOnWit((TestManagementRequestContext) this.m_RequestContext, projectFromName.GuidId, this.m_fetchLimit);
          if (beMigratedOnWit.Count != 0)
          {
            this.m_logger.Log(TraceLevel.Info, string.Format("Fetched {0} plan(s)", (object) beMigratedOnWit.Count));
            this.ResolveIdentities(beMigratedOnWit);
            this.FixAreaPathAndIterationPathPostM80PartitionDBChanges(beMigratedOnWit, projectFromName.GuidId, this.m_teamProjectName);
            this.MigrateTestPlans(projectFromName, beMigratedOnWit);
          }
          else
            break;
        }
        this.m_logger.Log(TraceLevel.Info, "No more plans need to be migrated");
        this.m_logger.Log(TraceLevel.Info, "Updating clone operation information");
        using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create((TestManagementRequestContext) this.m_RequestContext))
          planningDatabase.UpdateCloneOperationInformationAfterMigrationOnWit((TestManagementRequestContext) this.m_RequestContext, projectFromName.GuidId);
        this.UpgradeBuildDefinitions(projectFromName.GuidId);
        this.UpdateProjectMigrationState(this.m_RequestContext, projectFromName.GuidId, UpgradeMigrationState.Completed, (string) null);
      }
      catch (Exception ex)
      {
        this.UpdateProjectMigrationState(this.m_RequestContext, projectFromName.GuidId, UpgradeMigrationState.Failed, ex.Message);
        throw;
      }
      finally
      {
        this.m_logger.Log(TraceLevel.Info, "TestManagement::MigrateData End");
      }
    }

    private void FixAreaPathAndIterationPathPostM80PartitionDBChanges(
      List<TestPlan> plansToBeMigrated,
      Guid projectGuid,
      string projectName)
    {
      this.m_logger.Log(TraceLevel.Info, "Start Convert of TCM AreaPath and IterationPath to WorkItem. Since from M80 we started storing ProjectGuid\\RelativeArea instead of ProjectName\\RelativeArea and same of Iteration");
      foreach (TestPlan testPlan in plansToBeMigrated)
      {
        testPlan.AreaPath = string.IsNullOrEmpty(testPlan.AreaPath) ? testPlan.AreaPath : this.m_RequestContext.CSSHelper.TCMToWorkItemPath(testPlan.AreaPath, projectGuid, projectName);
        testPlan.Iteration = string.IsNullOrEmpty(testPlan.Iteration) ? testPlan.Iteration : this.m_RequestContext.CSSHelper.TCMToWorkItemPath(testPlan.Iteration, projectGuid, projectName);
      }
      this.m_logger.Log(TraceLevel.Info, "Completed the Convert of TCM AreaPath and IterationPath to WorkItem");
    }

    private void DeleteSessionsAssociatedWithDeletedPlans(Guid projectGuid)
    {
      Guid teamFoundationId = this.m_RequestContext.UserTeamFoundationId;
      this.m_logger.Log(TraceLevel.Info, "Detaching sessions from deleted plans");
      List<int> values;
      using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create((TestManagementRequestContext) this.m_RequestContext))
        values = planningDatabase.DeleteSessionsAssociatedWithDeletedPlans(projectGuid, teamFoundationId);
      if (values.Count <= 0)
        return;
      this.m_logger.Log(TraceLevel.Info, string.Format("Exploratory Testing Sessions having id {0} were deleted during upgrade as they did not have an associated test plan", (object) string.Join<int>(",", (IEnumerable<int>) values)));
      this.m_RequestContext.TestManagementHost.SignalTfsJobService((TestManagementRequestContext) this.m_RequestContext, IdConstants.ProjectDeletionCleanupJobId);
    }

    private void UpgradeBuildDefinitions(Guid projectGuid)
    {
      this.m_logger.Log(TraceLevel.Info, "UpgradeBuildDefinitions Start");
      new BuildDefinitionUpgrader(this.m_RequestContext, this.m_logger, this.m_teamProjectName, projectGuid).Perform();
      this.m_logger.Log(TraceLevel.Info, "UpgradeBuildDefinitions End");
    }

    private void MigrateTestPlans(GuidAndString projectId, List<TestPlan> plansToBeMigrated)
    {
      foreach (TestPlan plan in plansToBeMigrated)
      {
        this.m_logger.Log(TraceLevel.Info, string.Format("Starting migration of test plan {0} to work item layer", (object) plan.PlanId));
        this.LogPlanDetails(plan);
        if (plan.MigrationState == UpgradeMigrationState.NotStarted)
        {
          this.m_logger.Log(TraceLevel.Info, "Creating Work items for plan and suites");
          try
          {
            TestPlan.MigrateTestPlan((TestManagementRequestContext) this.m_RequestContext, this.m_logger, plan, this.m_teamProjectName, projectId, this.m_byPassWitValidation);
          }
          catch (LegacyServerException ex)
          {
            this.m_logger.Log(TraceLevel.Info, string.Format("Error while creating plan/suite workitems ErrorCode:{0} Exception Details:{1}", (object) ex.ErrorCode, (object) ex.ToString()));
            throw;
          }
          catch (TestManagementValidationException ex)
          {
            this.m_logger.Log(TraceLevel.Info, string.Format("Error while creating plan/suite workitems ErrorCode:{0} Exception Details:{1}", (object) ex.ErrorCode, (object) ex.ToString()));
            if (this.m_retriesLeft > 0)
            {
              this.m_logger.Log(TraceLevel.Info, string.Format("Retrying the wit creation after updating the CSS cache. Remaining retries: {0}", (object) this.m_retriesLeft));
              this.m_RequestContext.TestManagementHost.Replicator.ForceUpdateCss((TestManagementRequestContext) this.m_RequestContext, string.Empty, new int?());
              this.m_logger.Log(TraceLevel.Info, "Updated the Css cache");
              --this.m_retriesLeft;
              break;
            }
            throw;
          }
          this.m_logger.Log(TraceLevel.Info, "Done Creating Work items for plan and suites");
          this.LogPlanDetails(plan);
          using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create((TestManagementRequestContext) this.m_RequestContext))
            planningDatabase.UpdateWitProperties(projectId.GuidId, plan);
          this.m_logger.Log(TraceLevel.Info, "Updated plan/suite work item identfiers in TCM database");
        }
        this.m_retriesLeft = this.m_retryCount;
        this.m_logger.Log(TraceLevel.Info, string.Format("Replacing testplan and test suite identifiers with work item identifiers in referencing TCM artifacts. TcmPlanId:{0} WitPlanId:{1}", (object) plan.SourcePlanIdPreUpgrade, (object) plan.PlanWorkItem.Id));
        using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create((TestManagementRequestContext) this.m_RequestContext))
          planningDatabase.UpdateTcmArtifactsAfterMigrationOnWit((TestManagementRequestContext) this.m_RequestContext, this.m_logger, projectId.GuidId, plan.SourcePlanIdPreUpgrade, plan.PlanWorkItem.Id, this.m_updateArtifactLimit);
        this.m_logger.Log(TraceLevel.Info, string.Format("Completed migration of test plan {0} to work item layer. New test plan id is {1}", (object) plan.SourcePlanIdPreUpgrade, (object) plan.PlanWorkItem.Id));
      }
    }

    private bool IsProjectMigrationRequired(
      TfsTestManagementRequestContext tcmRequestContext,
      Guid projectGuid)
    {
      ProjectMigrationDetails migrationDetails;
      using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create((TestManagementRequestContext) tcmRequestContext))
        migrationDetails = planningDatabase.GetProjectMigrationDetails((TestManagementRequestContext) tcmRequestContext, projectGuid);
      return migrationDetails.MigrationState != UpgradeMigrationState.Completed;
    }

    private void UpdateProjectMigrationState(
      TfsTestManagementRequestContext tcmRequestContext,
      Guid projectGuid,
      UpgradeMigrationState migrationState,
      string migrationError)
    {
      using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create((TestManagementRequestContext) tcmRequestContext))
        planningDatabase.UpdateProjectMigrationDetails((TestManagementRequestContext) tcmRequestContext, projectGuid, migrationState, migrationError);
    }

    private void ResolveIdentities(List<TestPlan> plans)
    {
      this.m_logger.Log(TraceLevel.Info, "Resolving identities for plans");
      HashSet<Guid> source = new HashSet<Guid>();
      foreach (TestPlan plan in plans)
      {
        if (plan.Owner != Guid.Empty)
          source.Add(plan.Owner);
      }
      Dictionary<Guid, string> dictionary;
      try
      {
        dictionary = IdentityHelper.ResolveIdentities((TestManagementRequestContext) this.m_RequestContext, source.ToArray<Guid>());
      }
      catch (Exception ex)
      {
        this.m_logger.Log(TraceLevel.Warning, string.Format("Error in resolving identities during test plan upgrade. Exception Details: {0}", (object) ex.ToString()));
        dictionary = new Dictionary<Guid, string>();
      }
      foreach (TestPlan plan in plans)
      {
        Guid teamFoundationId = plan.Owner;
        string teamFoundationName;
        if (!dictionary.TryGetValue(plan.Owner, out teamFoundationName))
        {
          this.m_logger.Log(TraceLevel.Info, string.Format("Could not resolve identity: {0}. Creating test plan '{1}' using {2}", (object) plan.Name, (object) plan.Owner, (object) this.m_RequestContext.UserTeamFoundationName));
          teamFoundationId = this.m_RequestContext.UserTeamFoundationId;
          teamFoundationName = this.m_RequestContext.UserTeamFoundationName;
        }
        string str = IdentityHelper.ResolveIdentityToName((TestManagementRequestContext) this.m_RequestContext, teamFoundationId, true);
        if (string.IsNullOrEmpty(str))
          str = this.m_RequestContext.UserDistinctTeamFoundationName;
        plan.OwnerName = teamFoundationName;
        plan.CreatedByName = teamFoundationName;
        plan.CreatedByDistinctName = str;
        plan.LastUpdatedBy = teamFoundationId;
        plan.LastUpdatedByName = teamFoundationName;
        foreach (ServerTestSuite serverTestSuite in plan.SuitesMetaData)
        {
          serverTestSuite.LastUpdatedBy = teamFoundationId;
          serverTestSuite.LastUpdatedByName = teamFoundationName;
          serverTestSuite.CreatedByName = teamFoundationName;
          serverTestSuite.CreatedByDistinctName = str;
        }
      }
      this.m_logger.Log(TraceLevel.Info, "Finished Resolving identities");
    }

    private void InitializeMigrationParameters()
    {
      this.m_fetchLimit = this.m_registryService.GetValue<int>(this.m_RequestContext.RequestContext, (RegistryQuery) "/Service/TestManagement/Settings/MigratePlanFetchLimit", 10);
      this.m_updateArtifactLimit = this.m_registryService.GetValue<int>(this.m_RequestContext.RequestContext, (RegistryQuery) "/Service/TestManagement/Settings/MigratePlanUpdateArtifactLimit", 100000);
      this.m_byPassWitValidation = this.m_registryService.GetValue<bool>(this.m_RequestContext.RequestContext, (RegistryQuery) "/Service/TestManagement/Settings/MigratePlanBypassWitChecks", true);
      this.m_retryCount = this.m_registryService.GetValue<int>(this.m_RequestContext.RequestContext, (RegistryQuery) "/Service/TestManagement/Settings/MigratePlanRetryCount", 3);
      this.m_logger.Log(TraceLevel.Info, string.Format(" Plan migration parameters: m_fetchLimit {0} m_updateArtifactLimit {1} m_byPassWitValidation {2} m_retryCount {3}", (object) this.m_fetchLimit, (object) this.m_updateArtifactLimit, (object) this.m_byPassWitValidation, (object) this.m_retryCount));
    }

    private void LogPlanDetails(TestPlan plan)
    {
      this.m_logger.Log(TraceLevel.Info, "Logging plan details");
      this.m_logger.Log(TraceLevel.Info, plan.ToVerboseString());
      foreach (object obj in plan.SuitesMetaData)
        this.m_logger.Log(TraceLevel.Info, obj.ToString());
    }

    private void CleanUpDeletedTestPlan()
    {
      this.m_logger.Log(TraceLevel.Info, "TestManagement: MigrateData: CleanUpDeletedTestPlan: TestPlans pending delete cleanup started.");
      using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create((TestManagementRequestContext) this.m_RequestContext))
      {
        bool flag = false;
        while (!flag)
        {
          flag = planningDatabase.CleanDeletedTestPlansBeforeWITMigration();
          this.m_logger.Log(TraceLevel.Info, string.Format("TestManagement: MigrateData: TestPlans pending delete cleanup completed: {0}", (object) flag));
        }
      }
      this.m_logger.Log(TraceLevel.Info, "TestManagement: MigrateData: CleanUpDeletedTestPlan: TestPlans pending delete cleanup finished.");
    }
  }
}
