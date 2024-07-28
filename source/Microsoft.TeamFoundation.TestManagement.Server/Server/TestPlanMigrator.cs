// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestPlanMigrator
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.Client;
using Microsoft.TeamFoundation.TestManagement.Common.Internal;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal class TestPlanMigrator
  {
    internal static ProjectMigrationDetails GetMigrationStatus(
      TestManagementRequestContext context,
      string projectName)
    {
      context.TraceEnter("BusinessLayer", "TestPlan.GetMigrationStatus");
      GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, projectName);
      context.SecurityManager.CheckProjectMigrationPermissions(context);
      ProjectMigrationDetails migrationDetails;
      using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create(context))
        migrationDetails = planningDatabase.GetProjectMigrationDetails(context, projectFromName.GuidId);
      if (migrationDetails.MigrationState == UpgradeMigrationState.InProgress)
      {
        using (TeamFoundationLock teamFoundationLock = context.RequestContext.GetService<TeamFoundationLockingService>().AcquireLock(context.RequestContext, TeamFoundationLockMode.Exclusive, LockHelper.ConstructLockKeyForProject(projectName), 0))
        {
          if (teamFoundationLock != null)
            migrationDetails.MigrationState = UpgradeMigrationState.Aborted;
        }
      }
      context.TraceLeave("BusinessLayer", "TestPlan.GetMigrationStatus");
      return migrationDetails;
    }

    internal static void MigratePlans(
      TestManagementRequestContext context,
      string projectName,
      bool force = false)
    {
      context.TraceEnter("BusinessLayer", "TestPlan.MigratePlans");
      TestPlanMigrator.ValidatePlanMigrationPreRequisites(context, projectName, force);
      TestPlanMigrator.SchedulePlanMigrationJob(context, projectName);
      context.TraceLeave("BusinessLayer", "TestPlan.MigratePlans");
    }

    private static void ValidatePlanMigrationPreRequisites(
      TestManagementRequestContext context,
      string projectName,
      bool force)
    {
      context.TraceEnter("BusinessLayer", "TestPlan.ValidatePlanMigrationPreRequisites");
      GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, projectName);
      context.SecurityManager.CheckProjectMigrationPermissions(context);
      ProjectMigrationDetails migrationDetails;
      using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create(context))
        migrationDetails = planningDatabase.GetProjectMigrationDetails(context, projectFromName.GuidId);
      if (!force && migrationDetails.TotalPlansCount == 0)
        throw new TestManagementValidationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.NoPlansToMigrate, (object) projectName));
      if (!force && migrationDetails.MigrationState == UpgradeMigrationState.Completed)
        throw new TestManagementInvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.TestPlanMigrationAlreadyCompleted, (object) projectName));
      using (TeamFoundationLock teamFoundationLock = context.RequestContext.GetService<TeamFoundationLockingService>().AcquireLock(context.RequestContext, TeamFoundationLockMode.Exclusive, LockHelper.ConstructLockKeyForProject(projectName), 0))
      {
        if (teamFoundationLock == null)
          throw new TestManagementInvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.TestPlanMigrationAlreadyInProgress, (object) projectName));
      }
      context.TraceLeave("BusinessLayer", "TestPlan.ValidatePlanMigrationPreRequisites");
    }

    private static void SchedulePlanMigrationJob(
      TestManagementRequestContext context,
      string projectName)
    {
      context.TraceEnter("BusinessLayer", "TestPlan.SchedulePlanMigrationJob");
      TeamFoundationJobService service = context.RequestContext.GetService<TeamFoundationJobService>();
      TeamFoundationJobDefinition foundationJobDefinition = new TeamFoundationJobDefinition();
      foundationJobDefinition.JobId = Guid.NewGuid();
      foundationJobDefinition.ExtensionName = "Microsoft.TeamFoundation.TestManagement.Server.Jobs.MigratePlansJob";
      XmlDocument xmlDocument = XmlUtility.LoadXmlDocumentFromString(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "<MigratePlansJob teamProjectName=\"{0}\"/>", (object) projectName));
      foundationJobDefinition.Data = (XmlNode) xmlDocument.DocumentElement;
      foundationJobDefinition.Name = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "TestManagement Migration job for project {0}", (object) projectName);
      foundationJobDefinition.PriorityClass = JobPriorityClass.High;
      List<TeamFoundationJobDefinition> jobUpdates = new List<TeamFoundationJobDefinition>()
      {
        foundationJobDefinition
      };
      List<TeamFoundationJobReference> jobReferences = new List<TeamFoundationJobReference>()
      {
        foundationJobDefinition.ToJobReference()
      };
      service.UpdateJobDefinitions(context.RequestContext, (IEnumerable<Guid>) null, (IEnumerable<TeamFoundationJobDefinition>) jobUpdates);
      service.QueueJobsNow(context.RequestContext, (IEnumerable<TeamFoundationJobReference>) jobReferences);
      context.TraceLeave("BusinessLayer", "TestPlan.SchedulePlanMigrationJob");
    }
  }
}
