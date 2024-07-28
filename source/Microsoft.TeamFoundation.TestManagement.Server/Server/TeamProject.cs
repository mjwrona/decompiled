// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TeamProject
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.Azure.Boards.CssNodes;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.TestManagement.Common;
using Microsoft.VisualStudio.Services.Identity;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  public static class TeamProject
  {
    internal static void QueueDelete(TestManagementRequestContext context, string projectUri)
    {
      context.SecurityManager.CheckTeamProjectDeletePermission(context, projectUri);
      bool flag;
      using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
        flag = managementDatabase.QueueDeleteProject(projectUri);
      if (flag)
        context.TestManagementHost.SignalTfsJobService(context, IdConstants.ProjectDeletionCleanupJobId);
      else
        context.TraceError("BusinessLayer", "TeamProject.QueueDelete failed for {0}", (object) projectUri);
    }

    internal static void CreateDefaultFailureTypesForExistingProjects(
      TestManagementRequestContext context)
    {
      using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
      {
        foreach (GuidAndString queryProject in managementDatabase.QueryProjects(context, false))
          managementDatabase.CreateDefaultTestFailureTypes(queryProject.GuidId);
      }
    }

    internal static void SetTestGroupPermissions(
      TestManagementRequestContext context,
      string projectName)
    {
      try
      {
        context.TraceEnter("BusinessLayer", nameof (SetTestGroupPermissions));
        context.SecurityManager.CheckTeamProjectCreatePermission(context);
        ProjectInfo project = context.RequestContext.GetService<IProjectService>().GetProject(context.RequestContext, projectName);
        context.TestManagementHost.Replicator.ForceUpdateCss(context, context.IsFeatureEnabled("TestManagement.Server.ProjectScopedLockInUpdateCss") ? project.Id.ToString() : string.Empty, new int?());
        string projectUriFromName = Validator.CheckAndGetProjectUriFromName(context, projectName);
        TeamFoundationSecurityService service = context.RequestContext.GetService<TeamFoundationSecurityService>();
        IdentityDescriptor foundationDescriptor = Microsoft.TeamFoundation.Framework.Server.IdentityHelper.CreateTeamFoundationDescriptor(TestWellKnownGroups.TestServiceAccountsIdentifier);
        IVssSecurityNamespace securityNamespace1 = service.GetSecurityNamespace(context.RequestContext, FrameworkSecurity.TeamProjectNamespaceId);
        string token = securityNamespace1.NamespaceExtension.HandleIncomingToken(context.RequestContext, securityNamespace1, projectUriFromName);
        securityNamespace1.SetPermissions(context.RequestContext, token, foundationDescriptor, TeamProjectPermissions.GenericRead | TeamProjectPermissions.PublishTestResults | TeamProjectPermissions.ViewTestResults, 0, true);
        context.TraceInfo("BusinessLayer", "Setting permissions using CSS");
        IVssSecurityNamespace securityNamespace2 = service.GetSecurityNamespace(context.RequestContext, AuthorizationSecurityConstants.CommonStructureNodeSecurityGuid);
        foreach (TcmCommonStructureNodeInfo rootNode in context.CSSHelper.GetRootNodes(projectUriFromName))
        {
          if (TFStringComparer.CssStructureType.Equals(rootNode.StructureType, "ProjectModelHierarchy"))
            securityNamespace2.SetPermissions(context.RequestContext, rootNode.Uri, foundationDescriptor, 17, 0, true);
        }
      }
      finally
      {
        context.TraceLeave("BusinessLayer", nameof (SetTestGroupPermissions));
      }
    }
  }
}
