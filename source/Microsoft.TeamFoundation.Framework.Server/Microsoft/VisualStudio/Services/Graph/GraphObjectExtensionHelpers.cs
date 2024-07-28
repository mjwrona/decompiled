// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Graph.GraphObjectExtensionHelpers
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.GraphProfile.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Graph
{
  internal static class GraphObjectExtensionHelpers
  {
    private static readonly IReadOnlyDictionary<string, IdentityMetaType> ToIdentityMetaTypes = (IReadOnlyDictionary<string, IdentityMetaType>) new Dictionary<string, IdentityMetaType>()
    {
      {
        "member".ToLowerInvariant(),
        IdentityMetaType.Member
      },
      {
        "guest".ToLowerInvariant(),
        IdentityMetaType.Guest
      },
      {
        "companyAdministrator".ToLowerInvariant(),
        IdentityMetaType.CompanyAdministrator
      },
      {
        "helpdeskAdministrator".ToLowerInvariant(),
        IdentityMetaType.HelpdeskAdministrator
      },
      {
        "application".ToLowerInvariant(),
        IdentityMetaType.Application
      },
      {
        "managedIdentity".ToLowerInvariant(),
        IdentityMetaType.ManagedIdentity
      },
      {
        "unknown".ToLowerInvariant(),
        IdentityMetaType.Unknown
      }
    };
    private static readonly IReadOnlyDictionary<IdentityMetaType, string> ToGraphUserMetaTypes = (IReadOnlyDictionary<IdentityMetaType, string>) new Dictionary<IdentityMetaType, string>()
    {
      {
        IdentityMetaType.Member,
        "member"
      },
      {
        IdentityMetaType.Guest,
        "guest"
      },
      {
        IdentityMetaType.CompanyAdministrator,
        "companyAdministrator"
      },
      {
        IdentityMetaType.HelpdeskAdministrator,
        "helpdeskAdministrator"
      },
      {
        IdentityMetaType.Application,
        "application"
      },
      {
        IdentityMetaType.ManagedIdentity,
        "managedIdentity"
      },
      {
        IdentityMetaType.Unknown,
        (string) null
      }
    };

    internal static void GetUserUrlAndLinks(
      IVssRequestContext context,
      SubjectDescriptor subjectDescriptor,
      Guid storageKey,
      out string url,
      out ReferenceLinks links)
    {
      GraphObjectExtensionHelpers.GetMemberUrlAndLinks(context, subjectDescriptor, storageKey, out url, out links);
    }

    internal static void GetServicePrincipalUrlAndLinks(
      IVssRequestContext context,
      SubjectDescriptor subjectDescriptor,
      Guid storageKey,
      out string url,
      out ReferenceLinks links)
    {
      GraphObjectExtensionHelpers.GetMemberUrlAndLinks(context, subjectDescriptor, storageKey, out url, out links);
    }

    internal static string ConvertToGraphUserMetaType(IdentityMetaType metaType)
    {
      string graphUserMetaType;
      if (GraphObjectExtensionHelpers.ToGraphUserMetaTypes.TryGetValue(metaType, out graphUserMetaType))
        return graphUserMetaType;
      throw new ArgumentException(FrameworkResources.InvalidMetaType((object) metaType));
    }

    internal static IdentityMetaType ConvertToIdentityMetaType(string metaType)
    {
      if (metaType == null)
        return IdentityMetaType.Unknown;
      IdentityMetaType identityMetaType;
      if (GraphObjectExtensionHelpers.ToIdentityMetaTypes.TryGetValue(metaType.ToLowerInvariant(), out identityMetaType))
        return identityMetaType;
      throw new ArgumentException(FrameworkResources.InvalidMetaType((object) metaType));
    }

    internal static IdentityMetaType? ConvertToIdentityMetaTypeIgnoreNull(string metaType)
    {
      IdentityMetaType identityMetaType;
      return metaType != null && GraphObjectExtensionHelpers.ToIdentityMetaTypes.TryGetValue(metaType.ToLowerInvariant(), out identityMetaType) ? new IdentityMetaType?(identityMetaType) : new IdentityMetaType?();
    }

    private static void GetMemberUrlAndLinks(
      IVssRequestContext context,
      SubjectDescriptor subjectDescriptor,
      Guid storageKey,
      out string url,
      out ReferenceLinks links)
    {
      string href = subjectDescriptor.IsAadServicePrincipalType() ? GraphUrlHelper.GetGraphServicePrincipalUrl(context, subjectDescriptor) : GraphUrlHelper.GetGraphUserUrl(context, subjectDescriptor);
      string graphMembershipsUrl = GraphUrlHelper.GetGraphMembershipsUrl(context, subjectDescriptor);
      string membershipStateUrl = GraphUrlHelper.GetGraphMembershipStateUrl(context, subjectDescriptor);
      string graphStorageKeyUrl = GraphUrlHelper.GetGraphStorageKeyUrl(context, subjectDescriptor);
      string graphMemberAvatarUrl = GraphProfileUrlHelper.GetGraphMemberAvatarUrl(context, subjectDescriptor, storageKey);
      links = new ReferenceLinks();
      links.AddLink("self", href);
      links.AddLink("memberships", graphMembershipsUrl);
      links.AddLink("membershipState", membershipStateUrl);
      links.AddLink(nameof (storageKey), graphStorageKeyUrl);
      if (!string.IsNullOrEmpty(graphMemberAvatarUrl))
        links.AddLink("avatar", graphMemberAvatarUrl);
      url = href;
    }
  }
}
