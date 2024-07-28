// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Admin.PlugIns.GroupsViewDataProvider
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 362E2629-6AF5-42CD-95A4-09FE50F477E2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.Server.Types.Team;
using Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins.Helpers;
using Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins.Models;
using Microsoft.TeamFoundation.WebPlatform.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.Admin.PlugIns
{
  public class GroupsViewDataProvider : IExtensionDataProvider
  {
    public string Name => "Admin.GroupsView";

    public object GetData(
      IVssRequestContext requestContext,
      DataProviderContext providerContext,
      Contribution contribution)
    {
      bool flag = false;
      if (providerContext.Properties.ContainsKey("teamsFlag") && providerContext.Properties["teamsFlag"] != null)
        flag = bool.Parse(providerContext.Properties["teamsFlag"].ToString());
      ProjectInfo project = requestContext.GetService<IRequestProjectService>().GetProject(requestContext);
      string scope = project?.Uri.ToString();
      TeamFoundationIdentity[] foundationIdentityArray = TfsAdminIdentityHelper.ListScopedApplicationGroupsForProject(requestContext, scope);
      IdentityViewData identityViewData = TfsAdminIdentityHelper.JsonFromFilteredIdentitiesList(requestContext, new TeamFoundationFilteredIdentitiesList()
      {
        HasMoreItems = false,
        Items = foundationIdentityArray
      });
      IEnumerable<GraphViewModel> source = flag ? identityViewData.Identities.Where<GraphViewModel>((Func<GraphViewModel, bool>) (x => x.SubjectKind.ToLower() == "team")) : identityViewData.Identities;
      Guid DefaultTeamId = Guid.Empty;
      if (project != null && source != null && source.Any<GraphViewModel>())
      {
        DefaultTeamId = requestContext.GetService<ITeamService>().GetDefaultTeamId(requestContext, project.Id);
        source.Where<GraphViewModel>((Func<GraphViewModel, bool>) (x => x.IdentityId.ToString() == DefaultTeamId.ToString())).FirstOrDefault<GraphViewModel>((Func<GraphViewModel, bool>) (y => y.IsDefaultTeam = true));
      }
      foreach (GraphViewModel graphViewModel in source)
        graphViewModel.IsWellKnownGroup = SidIdentityHelper.IsWellKnownSid(SubjectDescriptor.FromString(graphViewModel.Descriptor).ToIdentityDescriptor(requestContext).Identifier);
      return (object) new GroupsViewData()
      {
        Identities = source,
        HasMore = false,
        TotalIdentityCount = identityViewData.Identities.Count<GraphViewModel>(),
        CollectionHostId = identityViewData.CollectionHostId,
        IsProjectScope = (scope != null)
      };
    }
  }
}
