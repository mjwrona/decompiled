// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.ResultRetentionSettingsService
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal class ResultRetentionSettingsService : 
    TeamFoundationTestManagementService,
    IResultRetentionSettingsService,
    IVssFrameworkService
  {
    private Func<string> _getCurrentUserIdentityIdDelegate;
    private Action _checkHasRetentionSettingsUpdatePermissionsDelegate;

    public ResultRetentionSettingsService()
    {
    }

    public ResultRetentionSettingsService(
      Func<string> getCurrentUserIdentityIdDelegate = null,
      Action checkHasRetentionSettingsUpdatePermissions = null)
    {
      this._getCurrentUserIdentityIdDelegate = getCurrentUserIdentityIdDelegate;
      this._checkHasRetentionSettingsUpdatePermissionsDelegate = checkHasRetentionSettingsUpdatePermissions;
    }

    public ResultRetentionSettings Create(
      TestManagementRequestContext context,
      GuidAndString projectId,
      ResultRetentionSettings settings,
      bool skipPermissionsCheck = false)
    {
      this.CheckRetentionSettingsModifyPermissions(context, projectId.String, skipPermissionsCheck);
      settings.LastUpdatedBy = new IdentityRef();
      settings.LastUpdatedBy.Id = this.GetCurrentUserIdentityId(context);
      using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
      {
        if (managementDatabase.GetResultRetentionSettings(context.RequestContext, projectId.GuidId) == null)
          return managementDatabase.CreateResultRetentionSettings(context.RequestContext, projectId.GuidId, settings);
        throw new TestManagementInvalidOperationException(ServerResources.ResultRetentionSettingsAlreadyExistsExceptionText);
      }
    }

    public ResultRetentionSettings Get(
      TestManagementRequestContext context,
      GuidAndString projectId)
    {
      ResultRetentionSettings retentionSettings;
      using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
        retentionSettings = managementDatabase.GetResultRetentionSettings(context.RequestContext, projectId.GuidId);
      if (retentionSettings == null)
        retentionSettings = new ResultRetentionSettings()
        {
          AutomatedResultsRetentionDuration = -1,
          ManualResultsRetentionDuration = -1,
          LastUpdatedBy = (IdentityRef) null
        };
      return retentionSettings;
    }

    public ResultRetentionSettings Update(
      TestManagementRequestContext context,
      GuidAndString projectId,
      ResultRetentionSettings settings,
      bool skipPermissionsCheck = false)
    {
      this.CheckRetentionSettingsModifyPermissions(context, projectId.String, skipPermissionsCheck);
      settings.LastUpdatedBy = new IdentityRef();
      settings.LastUpdatedBy.Id = this.GetCurrentUserIdentityId(context);
      using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
        return managementDatabase.GetResultRetentionSettings(context.RequestContext, projectId.GuidId) == null ? managementDatabase.CreateResultRetentionSettings(context.RequestContext, projectId.GuidId, settings) : managementDatabase.UpdateResultRetentionSettings(context.RequestContext, projectId.GuidId, settings);
    }

    public void CreateResultRetentionSettingsForNewProject(
      TestManagementRequestContext context,
      GuidAndString projectId)
    {
      try
      {
        context.TraceEnter("BusinessLayer", "TeamProject.CreateResultRetentionSettingsForNewProject");
        CachedRegistryService service = context.RequestContext.GetService<CachedRegistryService>();
        int num1 = service.GetValue<int>(context.RequestContext, (RegistryQuery) "/Service/TestManagement/Settings/AutomatedResultRetentionDurationForNewProject", 30);
        int num2 = service.GetValue<int>(context.RequestContext, (RegistryQuery) "/Service/TestManagement/Settings/ManualResultRetentionDurationForNewProject", 365);
        this.Create(context, projectId, new ResultRetentionSettings()
        {
          AutomatedResultsRetentionDuration = num1,
          ManualResultsRetentionDuration = num2
        }, true);
      }
      finally
      {
        context.TraceLeave("BusinessLayer", "TeamProject.CreateResultRetentionSettingsForNewProject");
      }
    }

    public void CreateResultRetentionSettingsForExistingProjects(
      TestManagementRequestContext context)
    {
      try
      {
        context.TraceEnter("BusinessLayer", "TeamProject.CreateResultRetentionSettingsForExistingProjects");
        IEnumerable<ProjectInfo> projects = context.RequestContext.GetService<IProjectService>().GetProjects(context.RequestContext, ProjectState.WellFormed);
        if (projects == null || !projects.Any<ProjectInfo>())
        {
          context.TraceInfo("BusinessLayer", "No projects found having test data.");
        }
        else
        {
          ResultRetentionSettings settings = new ResultRetentionSettings()
          {
            AutomatedResultsRetentionDuration = -1,
            ManualResultsRetentionDuration = -1
          };
          foreach (ProjectInfo projectInfo in projects)
            this.Update(context, new GuidAndString(projectInfo.Uri, projectInfo.Id), settings, true);
        }
      }
      finally
      {
        context.TraceLeave("BusinessLayer", "TeamProject.CreateResultRetentionSettingsForExistingProjects");
      }
    }

    private string GetCurrentUserIdentityId(TestManagementRequestContext context) => this._getCurrentUserIdentityIdDelegate == null ? context.UserTeamFoundationId.ToString() : this._getCurrentUserIdentityIdDelegate();

    private void CheckRetentionSettingsModifyPermissions(
      TestManagementRequestContext context,
      string projectUri,
      bool skipPermissionsCheck)
    {
      if (this._checkHasRetentionSettingsUpdatePermissionsDelegate != null)
        this._checkHasRetentionSettingsUpdatePermissionsDelegate();
      else if (skipPermissionsCheck)
        context.TraceInfo("BusinessLayer", "CheckRetentionSettingsModifyPermissions::Skipping retention settings permissions check. This call seems to be made from servicing");
      else
        context.SecurityManager.CheckRetentionSettingsModifyPermissions(context.RequestContext, projectUri);
    }
  }
}
