// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.GroupWellKnownDescriptors
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

namespace Microsoft.TeamFoundation.Framework.Client
{
  public static class GroupWellKnownDescriptors
  {
    public static readonly IdentityDescriptor LicenseesGroup = IdentityHelper.CreateTeamFoundationDescriptor(GroupWellKnownSecurityIds.LicenseesGroup);
    public static readonly IdentityDescriptor EveryoneGroup = IdentityHelper.CreateTeamFoundationDescriptor(GroupWellKnownSecurityIds.EveryoneGroup);
    public static readonly IdentityDescriptor NamespaceAdministratorsGroup = IdentityHelper.CreateTeamFoundationDescriptor(GroupWellKnownSecurityIds.NamespaceAdministratorsGroup);
    public static readonly IdentityDescriptor ServiceUsersGroup = IdentityHelper.CreateTeamFoundationDescriptor(GroupWellKnownSecurityIds.ServiceUsersGroup);
  }
}
