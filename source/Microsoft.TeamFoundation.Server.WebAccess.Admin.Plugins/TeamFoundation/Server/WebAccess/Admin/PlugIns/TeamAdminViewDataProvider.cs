// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Admin.PlugIns.TeamAdminViewDataProvider
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 362E2629-6AF5-42CD-95A4-09FE50F477E2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins.dll

using Microsoft.Azure.Devops.Teams.Service;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.Server.WebAccess.Admin.Utils;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Models;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.FeatureManagement;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.Contributions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Web.Script.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.Admin.PlugIns
{
  public class TeamAdminViewDataProvider : IExtensionDataProvider
  {
    private const string s_area = "WebAccessAdmin";
    private const string s_layer = "TeamAdminViewDataProvider";

    public string Name => "Admin.TeamAdminView";

    public object GetData(
      IVssRequestContext requestContext,
      DataProviderContext providerContext,
      Contribution contribution)
    {
      TeamAdminViewData data1 = new TeamAdminViewData();
      TfsWebContext webContext = (TfsWebContext) WebContextFactory.GetWebContext(requestContext);
      WebApiTeam team = (WebApiTeam) null;
      IWebTeamContext context;
      if (string.Equals(requestContext.GetService<IContributionNavigationService>().GetSelectedElementByType(requestContext, "ms.vss-web.hub")?.Id, "ms.vss-admin-web.project-admin-hub", StringComparison.OrdinalIgnoreCase) && requestContext.TryGetWebTeamContextWithoutGlobalContext(out context))
        team = context.Team;
      if (team == null)
        team = webContext.Team;
      if (team == null)
      {
        requestContext.Trace(10013735, TraceLevel.Error, "WebAccessAdmin", nameof (TeamAdminViewDataProvider), "Unable to resolve team");
        throw new ArgumentException("Unable to resolve team");
      }
      TeamFoundationIdentity readIdentity = requestContext.GetService<TeamFoundationIdentityService>().ReadIdentities(requestContext, new IdentityDescriptor[1]
      {
        team.Identity.Descriptor
      }, MembershipQuery.Direct, ReadIdentityOptions.ExtendedProperties, (IEnumerable<string>) new string[1]
      {
        TeamConstants.TeamPropertyName
      }, IdentityPropertyScope.Local)[0];
      ITeamSettings teamSettings = requestContext.GetService<ITeamConfigurationService>().GetTeamSettings(requestContext, team, false, true);
      ProjectProcessConfiguration processSettings1 = requestContext.GetService<IProjectConfigurationService>().GetProcessSettings(requestContext, webContext.Project.Uri, false);
      ITeamSettings settings = teamSettings;
      ProjectProcessConfiguration processSettings2 = processSettings1;
      TeamViewModel teamViewModel = new TeamViewModel(readIdentity, settings, processSettings2);
      teamViewModel.Identity.PopulateTeamAdmins(webContext);
      data1.TeamId = teamViewModel.Identity.TeamFoundationId;
      data1.TeamName = teamViewModel.Identity.FriendlyDisplayName;
      data1.ImageUrl = webContext.IdentityImage(teamViewModel.Identity.TeamFoundationId);
      if (requestContext.IsFeatureEnabled("VisualStudio.Services.EnableSafeDeserializer"))
      {
        data1.EditGroupOptionsJson = JsonConvert.SerializeObject((object) new
        {
          name = teamViewModel.Identity.FriendlyDisplayName,
          description = teamViewModel.Identity.Description,
          tfid = teamViewModel.Identity.TeamFoundationId
        });
        data1.TeamViewModelJson = JsonConvert.SerializeObject((object) new
        {
          currentIdentity = teamViewModel.Identity.ToJson(),
          admins = teamViewModel.Identity.Administrators,
          processSettings = teamViewModel.ProcessSettings.ToJson(requestContext, webContext.Project.Name),
          bugsBehaviorState = TeamSettingsControlHelpers.GetBugBehaviorState(webContext.TfsRequestContext, webContext.Project)
        });
      }
      else
      {
        JavaScriptSerializer scriptSerializer = new JavaScriptSerializer();
        data1.EditGroupOptionsJson = scriptSerializer.Serialize((object) new
        {
          name = teamViewModel.Identity.FriendlyDisplayName,
          description = teamViewModel.Identity.Description,
          tfid = teamViewModel.Identity.TeamFoundationId
        });
        data1.TeamViewModelJson = scriptSerializer.Serialize((object) new
        {
          currentIdentity = teamViewModel.Identity.ToJson(),
          admins = teamViewModel.Identity.Administrators,
          processSettings = teamViewModel.ProcessSettings.ToJson(requestContext, webContext.Project.Name),
          bugsBehaviorState = TeamSettingsControlHelpers.GetBugBehaviorState(webContext.TfsRequestContext, webContext.Project)
        });
      }
      TeamWITSettingsModel data2 = new TeamWITSettingsModel(requestContext, team, teamViewModel.ProcessSettings, teamViewModel.Settings, requestContext.GetCollectionTimeZone());
      data1.TeamSettingsDataJson = ConvertUtility.DataContractJson(data2.GetType(), (object) data2);
      data1.IsWorkAgileFeatureEnabled = requestContext.GetService<IContributedFeatureService>().IsFeatureEnabled(requestContext, "ms.vss-work.agile");
      return (object) data1;
    }
  }
}
