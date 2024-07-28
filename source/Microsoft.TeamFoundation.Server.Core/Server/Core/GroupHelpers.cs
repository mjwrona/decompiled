// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.GroupHelpers
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.Core
{
  internal static class GroupHelpers
  {
    public static bool IsApplicationGroup(TeamFoundationIdentity identity) => identity.IsContainer && GroupHelpers.IsTeamFoundationType(identity);

    public static bool IsTeamFoundationType(TeamFoundationIdentity identity) => identity.Descriptor.IdentityType.Equals("Microsoft.TeamFoundation.Identity", StringComparison.OrdinalIgnoreCase);

    public static bool IsCurrentHostWellKnownGroup(TeamFoundationIdentity identity) => GroupHelpers.IsApplicationGroup(identity) && VssStringComparer.SID.StartsWith(identity.Descriptor.Identifier, SidIdentityHelper.WellKnownSidPrefix);

    public static bool IsReadOnlyTfsGroup(
      IVssRequestContext requestContext,
      TeamFoundationIdentity groupIdentity)
    {
      if (GroupHelpers.IsCurrentHostWellKnownGroup(groupIdentity))
        return true;
      return GroupHelpers.IsApplicationGroup(groupIdentity) && !GroupHelpers.DoesUserHaveWritePermission(requestContext, groupIdentity);
    }

    public static bool DoesUserHaveWritePermission(
      IVssRequestContext requestContext,
      TeamFoundationIdentity groupIdentity)
    {
      return requestContext.GetService<TeamFoundationSecurityService>().GetSecurityNamespace(requestContext, FrameworkSecurity.IdentitiesNamespaceId).HasPermission(requestContext, IdentityUtil.CreateSecurityToken(groupIdentity), 2, false);
    }

    public static bool HasManageGroupMembershipPermission(
      IVssRequestContext requestContext,
      TeamFoundationIdentity groupIdentity,
      bool alwaysAllowAdministrators = false)
    {
      return groupIdentity != null && requestContext.GetService<TeamFoundationSecurityService>().GetSecurityNamespace(requestContext, FrameworkSecurity.IdentitiesNamespaceId).HasPermission(requestContext, IdentityUtil.CreateSecurityToken(groupIdentity), 8, alwaysAllowAdministrators);
    }

    public static void CheckManageGroupMembershipPermission(
      IVssRequestContext requestContext,
      TeamFoundationIdentity groupIdentity)
    {
      requestContext.GetService<TeamFoundationSecurityService>().GetSecurityNamespace(requestContext, FrameworkSecurity.IdentitiesNamespaceId).CheckPermission(requestContext, IdentityUtil.CreateSecurityToken(groupIdentity), 8, false);
    }

    internal static HashSet<IdentityDescriptor> ExpandMembersRecursively(
      IVssRequestContext requestContext,
      IdentityDescriptor group,
      int maxLevels = 100)
    {
      if (requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        throw new InvalidRequestContextHostException("This method is not supported on deployment level");
      HashSet<IdentityDescriptor> identityDescriptorSet1 = new HashSet<IdentityDescriptor>((IEqualityComparer<IdentityDescriptor>) IdentityDescriptorComparer.Instance);
      HashSet<IdentityDescriptor> first = new HashSet<IdentityDescriptor>((IEnumerable<IdentityDescriptor>) new IdentityDescriptor[1]
      {
        group
      }, (IEqualityComparer<IdentityDescriptor>) IdentityDescriptorComparer.Instance);
      HashSet<IdentityDescriptor> second = new HashSet<IdentityDescriptor>((IEqualityComparer<IdentityDescriptor>) IdentityDescriptorComparer.Instance);
      List<IdentityDescriptor> list = first.Except<IdentityDescriptor>((IEnumerable<IdentityDescriptor>) second).ToList<IdentityDescriptor>();
      int num;
      for (num = 0; list.Any<IdentityDescriptor>() && num < maxLevels; ++num)
      {
        if (requestContext.IsFeatureEnabled("Agile.Server.TeamService.AADGroupsBlockExpansion"))
        {
          IEnumerable<IdentityDescriptor> identityDescriptors = list.Where<IdentityDescriptor>((Func<IdentityDescriptor, bool>) (desc => AadIdentityHelper.IsAadGroup(desc)));
          identityDescriptorSet1.UnionWith(identityDescriptors);
          list = list.Except<IdentityDescriptor>(identityDescriptors).ToList<IdentityDescriptor>();
        }
        HashSet<IdentityDescriptor> identityDescriptorSet2 = GroupHelpers.ExpandMembers(requestContext, (IEnumerable<IdentityDescriptor>) list);
        if (!identityDescriptorSet2.IsNullOrEmpty<IdentityDescriptor>())
        {
          identityDescriptorSet1.UnionWith((IEnumerable<IdentityDescriptor>) identityDescriptorSet2);
          first.UnionWith(identityDescriptorSet2.Where<IdentityDescriptor>((Func<IdentityDescriptor, bool>) (x => GroupHelpers.IsTfsGroup(x))));
        }
        second.UnionWith((IEnumerable<IdentityDescriptor>) list);
        list = first.Except<IdentityDescriptor>((IEnumerable<IdentityDescriptor>) second).ToList<IdentityDescriptor>();
      }
      if (num > 10)
        requestContext.Trace(2300105, TraceLevel.Warning, "GroupHelper", nameof (ExpandMembersRecursively), string.Format("Expanded group {0} and found {1} levels", (object) group, (object) num));
      if (num > 99)
        requestContext.Trace(2300106, TraceLevel.Error, "GroupHelper", nameof (ExpandMembersRecursively), string.Format("Expanded group {0} and found {1} levels", (object) group, (object) num));
      if (identityDescriptorSet1.Count > 10000)
        requestContext.Trace(2300107, TraceLevel.Warning, "GroupHelper", nameof (ExpandMembersRecursively), string.Format("Expanded group {0} and found {1} users", (object) group, (object) identityDescriptorSet1.Count));
      if (identityDescriptorSet1.Count > 100000)
        requestContext.Trace(2300108, TraceLevel.Error, "GroupHelper", nameof (ExpandMembersRecursively), string.Format("Expanded group {0} and found {1} users", (object) group, (object) identityDescriptorSet1.Count));
      return identityDescriptorSet1;
    }

    private static HashSet<IdentityDescriptor> ExpandMembers(
      IVssRequestContext requestContext,
      IEnumerable<IdentityDescriptor> groups)
    {
      TeamFoundationIdentity[] source = requestContext.GetService<TeamFoundationIdentityService>().ReadIdentities(requestContext, groups.ToArray<IdentityDescriptor>(), MembershipQuery.Direct, ReadIdentityOptions.None, (IEnumerable<string>) null);
      HashSet<IdentityDescriptor> identityDescriptorSet = new HashSet<IdentityDescriptor>((IEqualityComparer<IdentityDescriptor>) IdentityDescriptorComparer.Instance);
      foreach (TeamFoundationIdentity foundationIdentity in ((IEnumerable<TeamFoundationIdentity>) source).Where<TeamFoundationIdentity>((Func<TeamFoundationIdentity, bool>) (x => x != null && !x.Members.IsNullOrEmpty<IdentityDescriptor>())))
        identityDescriptorSet.UnionWith((IEnumerable<IdentityDescriptor>) foundationIdentity.Members);
      return identityDescriptorSet;
    }

    internal static bool IsTfsGroup(IdentityDescriptor descriptor) => string.Equals(descriptor.IdentityType, "Microsoft.TeamFoundation.Identity", StringComparison.OrdinalIgnoreCase) && descriptor.Identifier.StartsWith(SidIdentityHelper.TeamFoundationSidPrefix, StringComparison.OrdinalIgnoreCase);
  }
}
