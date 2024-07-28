// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.GroupWellKnownSidConstants
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using Microsoft.TeamFoundation.Framework.Common;
using System;

namespace Microsoft.TeamFoundation
{
  public static class GroupWellKnownSidConstants
  {
    public static readonly string NamespaceAdministratorsGroupSid = SidIdentityHelper.ConstructWellKnownSid(0U, 1U);
    public static readonly string ServiceUsersGroupSid = SidIdentityHelper.ConstructWellKnownSid(0U, 2U);
    public static readonly string EveryoneGroupSid = SidIdentityHelper.ConstructWellKnownSid(0U, 3U);
    public static readonly string LicenseesGroupSid = SidIdentityHelper.ConstructWellKnownSid(0U, 4U);
    public static readonly string SecurityServiceGroupSid = SidIdentityHelper.ConstructWellKnownSid(0U, 5U);
    public static readonly string AnonymousUsersGroupSid = SidIdentityHelper.ConstructWellKnownSid(0U, 6U);
    [Obsolete("The host-wide Contributors group is no longer created for project collections, and is removed from existing project collections on upgrade to TFS 2012.")]
    public static readonly string ContributorsGroupSid = SidIdentityHelper.ConstructWellKnownSid(0U, 7U);
    public static readonly string ServicePrincipalGroupSid = SidIdentityHelper.ConstructWellKnownSid(0U, 8U);
    public static readonly string InstanceAllocatorsGroup = SidIdentityHelper.ConstructWellKnownSid(0U, 9U);
    public static readonly string UsersGroupSid = SidIdentityHelper.ConstructWellKnownSid(0U, 11U);
    public static readonly string GroupLicenseRulesGroupSid = SidIdentityHelper.ConstructWellKnownSid(0U, 12U);
    public static readonly string ApplicationPrincipalsGroupSid = SidIdentityHelper.ConstructWellKnownSid(0U, 13U);
    public static readonly string ProjectScopedUsersGroupSid = SidIdentityHelper.ConstructWellKnownSid(0U, 14U);
    public static readonly string AccountCreatorGroupSid = SidIdentityHelper.ConstructWellKnownSid(0U, 1024U);
    public static readonly string LicensedUsersGroupSid = SidIdentityHelper.ConstructWellKnownSid(0U, 2048U);
    public static readonly string InvitedUsersGroupSid = SidIdentityHelper.ConstructWellKnownSid(0U, 4096U);
  }
}
