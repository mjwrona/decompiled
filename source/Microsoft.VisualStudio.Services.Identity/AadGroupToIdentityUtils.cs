// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.AadGroupToIdentityUtils
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Aad;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Organization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Identity
{
  public static class AadGroupToIdentityUtils
  {
    private const string Area = "IdentityServiceExtensions";
    private const string Layer = "AadGroupToIdentityUtils";

    internal static IList<Microsoft.VisualStudio.Services.Identity.Identity> AddAggregateIdentitiesForAadGroups(
      IVssRequestContext applicationContext,
      IList<AadGroup> aadGroups)
    {
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) aadGroups, nameof (aadGroups), "IdentityServiceExtensions");
      IdentityService service = applicationContext.GetService<IdentityService>();
      IPlatformIdentityServiceInternal platformIdentityService = applicationContext.GetService<IPlatformIdentityServiceInternal>();
      string role = "AzureActiveDirectoryApplicationGroup";
      FrameworkIdentityType identityType = FrameworkIdentityType.AggregateIdentity;
      List<Microsoft.VisualStudio.Services.Identity.Identity> list = aadGroups.Where<AadGroup>((Func<AadGroup, bool>) (group => !string.IsNullOrEmpty(group.Mail) && ArgumentUtility.IsValidEmailAddress(group.Mail))).Select<AadGroup, Microsoft.VisualStudio.Services.Identity.Identity>((Func<AadGroup, Microsoft.VisualStudio.Services.Identity.Identity>) (group =>
      {
        IPlatformIdentityServiceInternal identityServiceInternal = platformIdentityService;
        IVssRequestContext requestContext = applicationContext;
        int identityType1 = (int) identityType;
        string role1 = role;
        Guid guid = group.ObjectId;
        string identifier = guid.ToString();
        string displayName = group.DisplayName;
        string mail = group.Mail;
        string mailNickname = group.MailNickname;
        guid = applicationContext.GetOrganizationAadTenantId();
        string domain = guid.ToString();
        return identityServiceInternal.BuildFrameworkIdentity(requestContext, (FrameworkIdentityType) identityType1, role1, identifier, displayName, mail, mailNickname, domain);
      })).ToList<Microsoft.VisualStudio.Services.Identity.Identity>();
      IVssRequestContext requestContext1 = applicationContext;
      List<Microsoft.VisualStudio.Services.Identity.Identity> identities = list;
      service.UpdateIdentities(requestContext1, (IList<Microsoft.VisualStudio.Services.Identity.Identity>) identities, true);
      return (IList<Microsoft.VisualStudio.Services.Identity.Identity>) list;
    }

    internal static IDictionary<IdentityDescriptor, AadGroup> GetGroupsFromAad(
      IVssRequestContext requestContext,
      IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> groups)
    {
      return AadGroupToIdentityUtils.GetGroupsFromAad(requestContext, groups.Select<Microsoft.VisualStudio.Services.Identity.Identity, IdentityDescriptor>((Func<Microsoft.VisualStudio.Services.Identity.Identity, IdentityDescriptor>) (g => g.Descriptor)));
    }

    internal static IDictionary<IdentityDescriptor, AadGroup> GetGroupsFromAad(
      IVssRequestContext requestContext,
      IEnumerable<IdentityDescriptor> descriptors)
    {
      GetGroupsWithIdsRequest<IdentityDescriptor> request = new GetGroupsWithIdsRequest<IdentityDescriptor>()
      {
        Identifiers = descriptors
      };
      return requestContext.GetService<AadService>().GetGroupsWithIds<IdentityDescriptor>(requestContext, request).Groups;
    }

    internal static IList<AadGroup> FilterAndTraceOrphanedGroups(
      IVssRequestContext applicationContext,
      IDictionary<IdentityDescriptor, AadGroup> groups)
    {
      return (IList<AadGroup>) groups.Where<KeyValuePair<IdentityDescriptor, AadGroup>>((Func<KeyValuePair<IdentityDescriptor, AadGroup>, bool>) (group =>
      {
        if (group.Value != null)
          return true;
        applicationContext.Trace(850623, TraceLevel.Verbose, "IdentityServiceExtensions", nameof (AadGroupToIdentityUtils), "This {0} group is returning from AadService but is not existing in Aad Service anymore", (object) group.Key);
        return false;
      })).Select<KeyValuePair<IdentityDescriptor, AadGroup>, AadGroup>((Func<KeyValuePair<IdentityDescriptor, AadGroup>, AadGroup>) (group => group.Value)).ToList<AadGroup>();
    }
  }
}
