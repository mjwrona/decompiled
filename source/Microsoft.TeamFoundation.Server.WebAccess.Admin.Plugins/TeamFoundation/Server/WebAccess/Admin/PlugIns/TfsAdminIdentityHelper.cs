// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Admin.PlugIns.TfsAdminIdentityHelper
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 362E2629-6AF5-42CD-95A4-09FE50F477E2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins.Helpers;
using Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins.Models;
using Microsoft.TeamFoundation.WebPlatform.Server;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.Admin.PlugIns
{
  internal sealed class TfsAdminIdentityHelper
  {
    internal static TeamFoundationIdentity[] ListScopedApplicationGroupsForProject(
      IVssRequestContext requestContext,
      string scope = null,
      ReadIdentityOptions? readOptions = null,
      IEnumerable<string> propertyNameFilters = null,
      IdentityPropertyScope? propertyScope = null)
    {
      ProjectInfo projectInfo = (ProjectInfo) null;
      if (scope == null)
        projectInfo = requestContext.GetService<IRequestProjectService>().GetProject(requestContext);
      ITeamFoundationIdentityService service = requestContext.GetService<ITeamFoundationIdentityService>();
      ITeamFoundationIdentityService foundationIdentityService = service;
      IVssRequestContext requestContext1 = requestContext;
      string scope1 = scope ?? projectInfo?.Uri.ToString();
      int readOptions1 = (int) readOptions ?? 4;
      object propertyNameFilters1 = (object) propertyNameFilters;
      if (propertyNameFilters1 == null)
        propertyNameFilters1 = (object) new string[1]{ "*" };
      int propertyScope1 = (int) propertyScope ?? 2;
      TeamFoundationIdentity[] foundationIdentityArray = foundationIdentityService.ListApplicationGroups(requestContext1, scope1, (ReadIdentityOptions) readOptions1, (IEnumerable<string>) propertyNameFilters1, (IdentityPropertyScope) propertyScope1);
      if (requestContext.ExecutionEnvironment.IsHostedDeployment)
        foundationIdentityArray = ((IEnumerable<TeamFoundationIdentity>) foundationIdentityArray).Where<TeamFoundationIdentity>((Func<TeamFoundationIdentity, bool>) (x => x != null && !AadIdentityHelper.IsAadGroup(x.Descriptor))).ToArray<TeamFoundationIdentity>();
      return ((IEnumerable<TeamFoundationIdentity>) TfsAdminIdentityHelper.PopulateGroupSubjectDescriptors(requestContext, service, foundationIdentityArray)).OrderBy<TeamFoundationIdentity, string>((Func<TeamFoundationIdentity, string>) (x => x.DisplayName)).ToArray<TeamFoundationIdentity>();
    }

    private static TeamFoundationIdentity[] PopulateGroupSubjectDescriptors(
      IVssRequestContext requestContext,
      ITeamFoundationIdentityService identityService,
      TeamFoundationIdentity[] tfsGroups)
    {
      List<Guid> guidList = new List<Guid>();
      foreach (TeamFoundationIdentity tfsGroup in tfsGroups)
        guidList.Add(tfsGroup.TeamFoundationId);
      IEnumerable<TeamFoundationIdentity> source = ((IEnumerable<TeamFoundationIdentity>) identityService.ReadIdentities(requestContext, guidList.ToArray())).Where<TeamFoundationIdentity>((Func<TeamFoundationIdentity, bool>) (x => x != null));
      List<TeamFoundationIdentity> foundationIdentityList = new List<TeamFoundationIdentity>();
      foreach (TeamFoundationIdentity tfsGroup in tfsGroups)
      {
        TeamFoundationIdentity tfi = tfsGroup;
        TeamFoundationIdentity foundationIdentity = source.SingleOrDefault<TeamFoundationIdentity>((Func<TeamFoundationIdentity, bool>) (g => g.TeamFoundationId == tfi.TeamFoundationId));
        if (tfi.GetProperty<string>("RestrictedVisible", (string) null) != "RestrictedVisible" && foundationIdentity != null)
        {
          tfi.SubjectDescriptor = foundationIdentity.SubjectDescriptor;
          foundationIdentityList.Add(tfi);
        }
        else
          requestContext.Trace(10050061, TraceLevel.Info, "General", "Service", tfi.Descriptor.Identifier);
      }
      return foundationIdentityList.ToArray();
    }

    internal static bool IsAnonymousPrincipal(IdentityDescriptor descriptor) => IdentityDescriptorComparer.Instance.Equals(descriptor, UserWellKnownIdentityDescriptors.AnonymousPrincipal);

    internal static bool IsAnonymousUsersGroup(IdentityDescriptor descriptor) => string.Equals(descriptor.IdentityType, "Microsoft.TeamFoundation.Identity", StringComparison.OrdinalIgnoreCase) && descriptor.Identifier.StartsWith(SidIdentityHelper.TeamFoundationSidPrefix, StringComparison.OrdinalIgnoreCase) && descriptor.Identifier.EndsWith(GroupWellKnownSidConstants.AnonymousUsersGroupSid.Substring(SidIdentityHelper.WellKnownDomainSid.Length), StringComparison.OrdinalIgnoreCase);

    internal static IdentityViewData JsonFromFilteredIdentitiesList(
      IVssRequestContext requestContext,
      TeamFoundationFilteredIdentitiesList filteredIdentities,
      bool filterServiceIdentities = false)
    {
      IEnumerable<TeamFoundationIdentity> foundationIdentities = (IEnumerable<TeamFoundationIdentity>) filteredIdentities.Items;
      bool hasMoreItems = filteredIdentities.HasMoreItems;
      int totalItems = filteredIdentities.TotalItems;
      if (foundationIdentities != null)
      {
        if (!requestContext.IsFeatureEnabled("Identity.AnonymousPrincipal"))
          foundationIdentities = foundationIdentities.Where<TeamFoundationIdentity>((Func<TeamFoundationIdentity, bool>) (s => !TfsAdminIdentityHelper.IsAnonymousPrincipal(s.Descriptor) && !TfsAdminIdentityHelper.IsAnonymousUsersGroup(s.Descriptor)));
        foundationIdentities = foundationIdentities.Where<TeamFoundationIdentity>((Func<TeamFoundationIdentity, bool>) (s => !ServicePrincipals.IsServicePrincipal(requestContext, s.Descriptor) && !IdentityHelper.IsWellKnownGroup(s.Descriptor, GroupWellKnownIdentityDescriptors.ServicePrincipalGroup)));
        if (requestContext.ExecutionEnvironment.IsHostedDeployment && requestContext.ServiceHost.Is(TeamFoundationHostType.Application))
          foundationIdentities = foundationIdentities.Where<TeamFoundationIdentity>((Func<TeamFoundationIdentity, bool>) (s => !IdentityHelper.IsWellKnownGroup(s.Descriptor, GroupWellKnownIdentityDescriptors.InvitedUsersGroup)));
        if (filterServiceIdentities)
          foundationIdentities = foundationIdentities.Where<TeamFoundationIdentity>((Func<TeamFoundationIdentity, bool>) (s => !IdentityHelper.IsServiceIdentity(requestContext, (IReadOnlyVssIdentity) IdentityUtil.Convert(s))));
      }
      return TfsAdminIdentityHelper.BuildFilteredIdentitiesViewModel(foundationIdentities, hasMoreItems, totalItems, requestContext.ServiceHost.CollectionServiceHost.InstanceId);
    }

    public static IdentityViewData BuildFilteredIdentitiesViewModel(
      IEnumerable<TeamFoundationIdentity> filteredIdentities,
      bool hasMoreItems,
      int totalItems,
      Guid collectionHostId)
    {
      IEnumerable<GraphViewModel> graphViewModels = Enumerable.Empty<GraphViewModel>();
      if (filteredIdentities != null)
        graphViewModels = (IEnumerable<GraphViewModel>) filteredIdentities.Select<TeamFoundationIdentity, GraphViewModel>((Func<TeamFoundationIdentity, GraphViewModel>) (s => new GraphViewModel(s, collectionHostId))).ToList<GraphViewModel>();
      return new IdentityViewData()
      {
        Identities = graphViewModels,
        HasMore = hasMoreItems,
        TotalIdentityCount = totalItems,
        CollectionHostId = collectionHostId
      };
    }

    public static TeamFoundationFilteredIdentitiesList ReadGroupMembers(
      IVssRequestContext requestContext,
      Guid scope,
      MembershipQuery? scopedMembershipQuery)
    {
      return requestContext.GetService<TeamFoundationIdentityService>().ReadFilteredIdentitiesById(requestContext, new Guid[1]
      {
        scope
      }, IdentityManagementHelpers.GetPageSize(new int?()), (IEnumerable<IdentityFilter>) new List<IdentityFilter>(), (string) null, true, (MembershipQuery) ((int) scopedMembershipQuery ?? 1), MembershipQuery.None, true, (IEnumerable<string>) null);
    }
  }
}
