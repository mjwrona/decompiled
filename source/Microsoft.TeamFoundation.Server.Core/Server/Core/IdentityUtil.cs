// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.IdentityUtil
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.Azure.Boards.CssNodes;
using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.TeamFoundation.Server.Core
{
  public static class IdentityUtil
  {
    private static string copyLocalPropertiesInIdentityConvertDisabledFF = "WorkItemTracking.Server.CopyLocalPropertiesInIdentityConvertDisabled";

    public static TeamFoundationIdentity[] Convert(IList<Microsoft.VisualStudio.Services.Identity.Identity> identities)
    {
      TeamFoundationIdentity[] foundationIdentityArray = new TeamFoundationIdentity[identities.Count];
      for (int index = 0; index < identities.Count; ++index)
        foundationIdentityArray[index] = IdentityUtil.Convert(identities[index]);
      return foundationIdentityArray;
    }

    public static TeamFoundationIdentity Convert(Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      TeamFoundationIdentity foundationIdentity = (TeamFoundationIdentity) null;
      if (identity != null)
      {
        foundationIdentity = new TeamFoundationIdentity(identity.Descriptor, identity.ProviderDisplayName, identity.CustomDisplayName, identity.IsActive, identity.MasterId == Guid.Empty || identity.MasterId == IdentityConstants.LinkedId ? identity.Id : identity.MasterId, identity.Id, identity.UniqueUserId, identity.Members, identity.MemberOf, identity.MemberIds, false, identity.SubjectDescriptor, identity.MetaType);
        if (identity.IsContainer)
          foundationIdentity.InitializeProperty(IdentityPropertyScope.Global, "SchemaClassName", (object) "Group");
        foreach (KeyValuePair<string, object> property in (IEnumerable<KeyValuePair<string, object>>) identity.Properties)
        {
          if (IdentityAttributeTags.ReadOnlyProperties.Contains(property.Key))
          {
            string str = property.Value is DateTime ? ((DateTime) property.Value).ToString((IFormatProvider) DateTimeFormatInfo.InvariantInfo) : property.Value.ToString();
            foundationIdentity.InitializeProperty(IdentityPropertyScope.Global, property.Key, (object) str);
          }
          else
            foundationIdentity.InitializeProperty(IdentityPropertyScope.Local, property.Key, property.Value);
        }
        foundationIdentity.ResetModifiedProperties();
        HashSet<string> modifiedProperties = identity.GetModifiedProperties();
        if (modifiedProperties != null)
        {
          foreach (string name in modifiedProperties)
          {
            object property = identity.GetProperty<object>(name, (object) null);
            foundationIdentity.SetProperty(IdentityPropertyScope.Global, name, property);
          }
        }
      }
      return foundationIdentity;
    }

    public static Microsoft.VisualStudio.Services.Identity.Identity[] Convert(
      IList<TeamFoundationIdentity> identities)
    {
      Microsoft.VisualStudio.Services.Identity.Identity[] identityArray = new Microsoft.VisualStudio.Services.Identity.Identity[identities.Count];
      for (int index = 0; index < identities.Count; ++index)
        identityArray[index] = IdentityUtil.Convert(identities[index]);
      return identityArray;
    }

    public static Microsoft.VisualStudio.Services.Identity.Identity Convert(
      IVssRequestContext requestContext,
      TeamFoundationIdentity identity)
    {
      return IdentityUtil.Convert(identity, !requestContext.IsFeatureEnabled(IdentityUtil.copyLocalPropertiesInIdentityConvertDisabledFF));
    }

    public static Microsoft.VisualStudio.Services.Identity.Identity Convert(
      TeamFoundationIdentity identity,
      bool copyLocalProps = false)
    {
      Microsoft.VisualStudio.Services.Identity.Identity identity1 = (Microsoft.VisualStudio.Services.Identity.Identity) null;
      if (identity != null)
      {
        Microsoft.VisualStudio.Services.Identity.Identity identity2 = new Microsoft.VisualStudio.Services.Identity.Identity();
        identity2.Id = identity.TeamFoundationId;
        identity2.Descriptor = new IdentityDescriptor(identity.Descriptor);
        identity2.SubjectDescriptor = identity.SubjectDescriptor;
        identity2.MetaType = identity.MetaType;
        identity2.ProviderDisplayName = identity.ProviderDisplayName;
        identity2.CustomDisplayName = identity.CustomDisplayName;
        identity2.IsActive = identity.IsActive;
        identity2.UniqueUserId = identity.UniqueUserId;
        identity2.IsContainer = identity.IsContainer;
        identity2.Members = identity.Members;
        identity2.MemberOf = identity.MemberOf;
        identity2.MasterId = identity.MasterId;
        identity1 = identity2;
        foreach (KeyValuePair<string, object> property in identity.GetProperties(copyLocalProps ? IdentityPropertyScope.Both : IdentityPropertyScope.Global))
          identity1.Properties.Add(property.Key, property.Value);
        HashSet<string> collection = identity.GetModifiedPropertiesLog(IdentityPropertyScope.Global).AsEmptyIfNull<HashSet<string>>();
        if (copyLocalProps)
          collection.AddRangeIfRangeNotNull<string, HashSet<string>>((IEnumerable<string>) identity.GetModifiedPropertiesLog(IdentityPropertyScope.Local));
        foreach (string name in collection)
        {
          object property = identity1.GetProperty<object>(name, (object) null);
          identity1.SetProperty(name, property);
        }
      }
      return identity1;
    }

    public static string GetDomainUserName(
      TeamFoundationIdentity identity,
      out string resolvableName)
    {
      string displayableName;
      UserNameUtil.GetIdentityName(identity.Descriptor.IdentityType, identity.DisplayName, identity.GetAttribute("Domain", string.Empty), identity.GetAttribute("Account", string.Empty), identity.UniqueUserId, out resolvableName, out displayableName);
      return displayableName;
    }

    public static string GetDomainUserName(TeamFoundationIdentity identity)
    {
      string displayableName;
      UserNameUtil.GetIdentityName(identity.Descriptor.IdentityType, identity.DisplayName, identity.GetAttribute("Domain", string.Empty), identity.GetAttribute("Account", string.Empty), identity.UniqueUserId, out string _, out displayableName);
      return displayableName;
    }

    public static string GetDomainName(TeamFoundationIdentity identity)
    {
      string domainName = identity.GetAttribute("Domain", string.Empty);
      if (identity.Descriptor.IdentityType == "Microsoft.TeamFoundation.Identity")
        domainName = string.IsNullOrEmpty(domainName) ? "[" + "SERVER" + "]" : UserNameUtil.GetDomainName(identity.DisplayName);
      return domainName;
    }

    internal static string CreateSecurityToken(TeamFoundationIdentity group) => group.GetAttribute("LocalScopeId", string.Empty) + (object) FrameworkSecurity.IdentitySecurityPathSeparator + group.TeamFoundationId.ToString();

    internal static GroupSpecialType GetGroupSpecialType(TeamFoundationIdentity identity)
    {
      GroupSpecialType groupSpecialType = GroupSpecialType.Generic;
      string attribute = identity.GetAttribute("SpecialType", (string) null);
      if (!string.IsNullOrEmpty(attribute))
        groupSpecialType = (GroupSpecialType) Enum.Parse(typeof (GroupSpecialType), attribute);
      return groupSpecialType;
    }

    public static bool IdentityHasName(TeamFoundationIdentity identity, string name) => VssStringComparer.UserName.Equals(identity.UniqueName, name) || VssStringComparer.UserName.Equals(identity.DisplayName, name) || VssStringComparer.UserName.Equals(identity.GetAttribute("Account", string.Empty), name) || VssStringComparer.UserName.Equals(IdentityUtil.GetDomainUserName(identity), name);

    public static bool IdentityHasProjectReadPermission(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      string projectUri)
    {
      IVssSecurityNamespace securityNamespace = requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, FrameworkSecurity.TeamProjectNamespaceId);
      string token = TeamProjectSecurityConstants.GetToken(projectUri);
      return token != null && (1 & securityNamespace.QueryEffectivePermissions(requestContext, token, new EvaluationPrincipal(identity.Descriptor))) == 1;
    }

    public static bool IdentityHasProjectReadPermission(
      IVssRequestContext requestContext,
      TeamFoundationIdentity identity,
      string projectUri)
    {
      return IdentityUtil.IdentityHasProjectReadPermission(requestContext, IdentityUtil.Convert(identity), projectUri);
    }

    public static bool IdentityHasProjectReadPermission(
      IVssRequestContext requestContext,
      TeamFoundationIdentity identity,
      Guid projectId)
    {
      return IdentityUtil.IdentityHasProjectReadPermission(requestContext, IdentityUtil.Convert(requestContext, identity), CommonStructureUtils.GetProjectUri(projectId));
    }

    public static bool IdentityHasProjectReadPermission(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      Guid projectId)
    {
      return IdentityUtil.IdentityHasProjectReadPermission(requestContext, identity, CommonStructureUtils.GetProjectUri(projectId));
    }
  }
}
