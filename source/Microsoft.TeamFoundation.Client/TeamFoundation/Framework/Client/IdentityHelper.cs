// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.IdentityHelper
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.ComponentModel;
using System.Security.Principal;

namespace Microsoft.TeamFoundation.Framework.Client
{
  public static class IdentityHelper
  {
    public static IdentityDescriptor CreateDescriptorFromSid(string sid)
    {
      if (string.IsNullOrEmpty(sid))
        throw new ArgumentException(TFCommonResources.EmptyStringNotAllowed(), nameof (sid));
      return sid.StartsWith(SidIdentityHelper.TeamFoundationSidPrefix, StringComparison.OrdinalIgnoreCase) ? IdentityHelper.CreateTeamFoundationDescriptor(sid) : IdentityHelper.CreateWindowsDescriptor(sid);
    }

    public static IdentityDescriptor CreateDescriptorFromSid(SecurityIdentifier securityId) => IdentityHelper.CreateDescriptorFromSid(new SecurityIdentifierInfo(securityId));

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static IdentityDescriptor CreateDescriptorFromSid(SecurityIdentifierInfo securityIdInfo) => SidIdentityHelper.IsTeamFoundationIdentifier(securityIdInfo) ? IdentityHelper.CreateTeamFoundationDescriptor(securityIdInfo) : IdentityHelper.CreateWindowsDescriptor(securityIdInfo);

    public static IdentityDescriptor CreateWindowsDescriptor(string sid) => new IdentityDescriptor("System.Security.Principal.WindowsIdentity", sid);

    public static IdentityDescriptor CreateWindowsDescriptor(SecurityIdentifier securityId) => IdentityHelper.CreateWindowsDescriptor(new SecurityIdentifierInfo(securityId));

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static IdentityDescriptor CreateWindowsDescriptor(SecurityIdentifierInfo securityIdInfo) => SidDescriptor.Create("System.Security.Principal.WindowsIdentity", securityIdInfo);

    public static IdentityDescriptor CreateTeamFoundationDescriptor(string sid) => new IdentityDescriptor("Microsoft.TeamFoundation.Identity", sid);

    public static IdentityDescriptor CreateTeamFoundationDescriptor(SecurityIdentifier securityId) => IdentityHelper.CreateTeamFoundationDescriptor(new SecurityIdentifierInfo(securityId));

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static IdentityDescriptor CreateTeamFoundationDescriptor(
      SecurityIdentifierInfo securityIdInfo)
    {
      return SidDescriptor.Create("Microsoft.TeamFoundation.Identity", securityIdInfo);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static GroupSpecialType GetGroupSpecialType(TeamFoundationIdentity identity)
    {
      GroupSpecialType groupSpecialType = GroupSpecialType.Generic;
      string attribute = identity.GetAttribute("SpecialType", (string) null);
      if (!string.IsNullOrEmpty(attribute))
        groupSpecialType = (GroupSpecialType) Enum.Parse(typeof (GroupSpecialType), attribute);
      return groupSpecialType;
    }

    public static string GetDomainUserName(
      TeamFoundationIdentity identity,
      out string resolvableName)
    {
      ArgumentUtility.CheckForNull<TeamFoundationIdentity>(identity, nameof (identity));
      string displayableName;
      UserNameUtil.GetIdentityName(identity.Descriptor.IdentityType, identity.DisplayName, identity.GetAttribute("Domain", string.Empty), identity.GetAttribute("Account", string.Empty), identity.UniqueUserId, out resolvableName, out displayableName);
      return displayableName;
    }

    public static string GetDomainUserName(TeamFoundationIdentity identity)
    {
      ArgumentUtility.CheckForNull<TeamFoundationIdentity>(identity, nameof (identity));
      string displayableName;
      UserNameUtil.GetIdentityName(identity.Descriptor.IdentityType, identity.DisplayName, identity.GetAttribute("Domain", string.Empty), identity.GetAttribute("Account", string.Empty), identity.UniqueUserId, out string _, out displayableName);
      return displayableName;
    }

    public static string GetDomainName(TeamFoundationIdentity identity)
    {
      ArgumentUtility.CheckForNull<TeamFoundationIdentity>(identity, nameof (identity));
      string domainName = identity.GetAttribute("Domain", string.Empty);
      if (identity.Descriptor.IdentityType == "Microsoft.TeamFoundation.Identity")
        domainName = !string.IsNullOrEmpty(domainName) ? UserNameUtil.GetDomainName(identity.DisplayName) : "[" + "SERVER" + "]";
      return domainName;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static string CreateSecurityToken(TeamFoundationIdentity group) => group.GetAttribute("Domain", string.Empty) + (object) FrameworkSecurity.IdentitySecurityPathSeparator + group.TeamFoundationId.ToString();

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void CheckDescriptor(IdentityDescriptor descriptor, string parameterName)
    {
      ArgumentUtility.CheckForNull<IdentityDescriptor>(descriptor, parameterName);
      if (string.IsNullOrEmpty(descriptor.IdentityType))
        throw new ArgumentException(TFCommonResources.EmptyStringNotAllowed(), parameterName + ".IdentityType");
      if (string.IsNullOrEmpty(descriptor.Identifier))
        throw new ArgumentException(TFCommonResources.EmptyStringNotAllowed(), parameterName + ".Identifier");
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static bool IdentityHasName(TeamFoundationIdentity identity, string name)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(name, nameof (name));
      return VssStringComparer.UserName.Equals(identity.UniqueName, name) || VssStringComparer.UserName.Equals(identity.DisplayName, name) || VssStringComparer.UserName.Equals(identity.GetAttribute("Account", string.Empty), name) || VssStringComparer.UserName.Equals(IdentityHelper.GetDomainUserName(identity), name);
    }
  }
}
