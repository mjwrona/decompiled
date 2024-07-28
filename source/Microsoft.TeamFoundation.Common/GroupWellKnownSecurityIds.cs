// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.GroupWellKnownSecurityIds
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using System;
using System.Security.Principal;

namespace Microsoft.TeamFoundation
{
  public static class GroupWellKnownSecurityIds
  {
    public static readonly SecurityIdentifier NamespaceAdministratorsGroup = new SecurityIdentifier(GroupWellKnownSidConstants.NamespaceAdministratorsGroupSid);
    public static readonly SecurityIdentifier ServiceUsersGroup = new SecurityIdentifier(GroupWellKnownSidConstants.ServiceUsersGroupSid);
    public static readonly SecurityIdentifier EveryoneGroup = new SecurityIdentifier(GroupWellKnownSidConstants.EveryoneGroupSid);
    public static readonly SecurityIdentifier LicenseesGroup = new SecurityIdentifier(GroupWellKnownSidConstants.LicenseesGroupSid);
    public static readonly SecurityIdentifier SecurityServiceGroup = new SecurityIdentifier(GroupWellKnownSidConstants.SecurityServiceGroupSid);
    public static readonly SecurityIdentifier AnonymousUsersGroup = new SecurityIdentifier(GroupWellKnownSidConstants.AnonymousUsersGroupSid);
    [Obsolete("The host-wide Contributors group is no longer created for project collections, and is removed from existing project collections on upgrade to TFS 2012.")]
    public static readonly SecurityIdentifier ContributorsGroup = new SecurityIdentifier(GroupWellKnownSidConstants.ContributorsGroupSid);
    public static readonly SecurityIdentifier AccountCreatorGroup = new SecurityIdentifier(GroupWellKnownSidConstants.AccountCreatorGroupSid);
    public static readonly SecurityIdentifier ServicePrincipalGroup = new SecurityIdentifier(GroupWellKnownSidConstants.ServicePrincipalGroupSid);
    public static readonly SecurityIdentifier LicensedUsersGroup = new SecurityIdentifier(GroupWellKnownSidConstants.LicensedUsersGroupSid);
    public static readonly SecurityIdentifier InvitedUsersGroup = new SecurityIdentifier(GroupWellKnownSidConstants.InvitedUsersGroupSid);
    public static readonly SecurityIdentifier InstanceAllocatorsGroup = new SecurityIdentifier(GroupWellKnownSidConstants.InstanceAllocatorsGroup);
    public static readonly SecurityIdentifier UsersGroup = new SecurityIdentifier(GroupWellKnownSidConstants.UsersGroupSid);
    public static readonly SecurityIdentifier ApplicationPrincipalsGroup = new SecurityIdentifier(GroupWellKnownSidConstants.ApplicationPrincipalsGroupSid);
    public static readonly SecurityIdentifier GroupLicenseRulesGroup = new SecurityIdentifier(GroupWellKnownSidConstants.GroupLicenseRulesGroupSid);
    public static readonly SecurityIdentifier ProjectScopedUsersGroup = new SecurityIdentifier(GroupWellKnownSidConstants.ProjectScopedUsersGroupSid);
  }
}
