// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.GroupWellKnownIdentityDescriptors
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Security.Principal;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public static class GroupWellKnownIdentityDescriptors
  {
    public static readonly IdentityDescriptor LicenseesGroup = IdentityHelper.CreateReadOnlyTeamFoundationDescriptor(GroupWellKnownSecurityIds.LicenseesGroup);
    public static readonly IdentityDescriptor EveryoneGroup = IdentityHelper.CreateReadOnlyTeamFoundationDescriptor(GroupWellKnownSecurityIds.EveryoneGroup);
    public static readonly IdentityDescriptor NamespaceAdministratorsGroup = IdentityHelper.CreateReadOnlyTeamFoundationDescriptor(GroupWellKnownSecurityIds.NamespaceAdministratorsGroup);
    public static readonly IdentityDescriptor ServiceUsersGroup = IdentityHelper.CreateReadOnlyTeamFoundationDescriptor(GroupWellKnownSecurityIds.ServiceUsersGroup);
    public static readonly IdentityDescriptor SecurityServiceGroup = IdentityHelper.CreateReadOnlyTeamFoundationDescriptor(GroupWellKnownSecurityIds.SecurityServiceGroup);
    [Obsolete("The host-wide Contributors group is no longer created for project collections, and is removed from existing project collections on upgrade to TFS 2012.")]
    public static readonly IdentityDescriptor ContributorsGroup = IdentityHelper.CreateReadOnlyTeamFoundationDescriptor(GroupWellKnownSecurityIds.ContributorsGroup);
    public static readonly IdentityDescriptor AccountCreatorGroup = IdentityHelper.CreateReadOnlyTeamFoundationDescriptor(GroupWellKnownSecurityIds.AccountCreatorGroup);
    public static readonly IdentityDescriptor ServicePrincipalGroup = IdentityHelper.CreateReadOnlyTeamFoundationDescriptor(GroupWellKnownSecurityIds.ServicePrincipalGroup);
    public static readonly IdentityDescriptor LicensedUsersGroup = IdentityHelper.CreateReadOnlyTeamFoundationDescriptor(GroupWellKnownSecurityIds.LicensedUsersGroup);
    public static readonly IdentityDescriptor InvitedUsersGroup = IdentityHelper.CreateReadOnlyTeamFoundationDescriptor(GroupWellKnownSecurityIds.InvitedUsersGroup);
    public static readonly IdentityDescriptor InstanceAllocatorsGroup = IdentityHelper.CreateReadOnlyTeamFoundationDescriptor(GroupWellKnownSecurityIds.InstanceAllocatorsGroup);
    public static readonly IdentityDescriptor UsersGroup = IdentityHelper.CreateReadOnlyTeamFoundationDescriptor(GroupWellKnownSecurityIds.UsersGroup);
    public static readonly IdentityDescriptor ApplicationPrincipalsGroup = IdentityHelper.CreateReadOnlyTeamFoundationDescriptor(GroupWellKnownSecurityIds.ApplicationPrincipalsGroup);
    public static readonly IdentityDescriptor ProjectScopedUsersGroup = IdentityHelper.CreateReadOnlyTeamFoundationDescriptor(GroupWellKnownSecurityIds.ProjectScopedUsersGroup);

    public static class FeatureAvailability
    {
      public static readonly IdentityDescriptor AdminUsersGroup = IdentityHelper.CreateReadOnlyTeamFoundationDescriptor(new SecurityIdentifier(FeatureAvailabilityConstants.FeatureAvailabilityAdminUsersGroupIdentifier));
      public static readonly IdentityDescriptor AccountAdminUsersGroup = IdentityHelper.CreateReadOnlyTeamFoundationDescriptor(new SecurityIdentifier(FeatureAvailabilityConstants.FeatureAvailabilityAccountAdminUsersGroupIdentifier));
      public static readonly IdentityDescriptor ReadersUsersGroup = IdentityHelper.CreateReadOnlyTeamFoundationDescriptor(new SecurityIdentifier(FeatureAvailabilityConstants.FeatureAvailabilityReadersUsersGroupIdentifier));
    }

    public static class Proxy
    {
      public static readonly IdentityDescriptor ServiceAccounts = IdentityHelper.CreateReadOnlyTeamFoundationDescriptor(new SecurityIdentifier(SidIdentityHelper.ConstructWellKnownSid(5U, 1U)));
    }
  }
}
