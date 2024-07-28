// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.IdentityHelper
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal static class IdentityHelper
  {
    internal static IList<Guid> SearchUserByDisplayName(
      TestManagementRequestContext context,
      string displayName)
    {
      if (!string.IsNullOrEmpty(displayName))
      {
        Microsoft.VisualStudio.Services.Identity.Identity identity = IdentityHelper.SearchIdentityByDisplayName(context, displayName);
        if (identity != null)
          return (IList<Guid>) new List<Guid>()
          {
            identity.Id
          };
      }
      return (IList<Guid>) new List<Guid>();
    }

    internal static Dictionary<string, Microsoft.VisualStudio.Services.Identity.Identity> SearchUsersByNames(
      TestManagementRequestContext context,
      List<string> names)
    {
      if (names == null || !names.Any<string>())
        return new Dictionary<string, Microsoft.VisualStudio.Services.Identity.Identity>();
      Dictionary<string, Microsoft.VisualStudio.Services.Identity.Identity> dictionary = new Dictionary<string, Microsoft.VisualStudio.Services.Identity.Identity>();
      names = names.Distinct<string>().ToList<string>();
      foreach (string name in names)
      {
        Microsoft.VisualStudio.Services.Identity.Identity identity = IdentityHelper.SearchIdentityByDisplayName(context, name);
        if (identity != null)
          dictionary[name] = identity;
      }
      return dictionary;
    }

    internal static List<Microsoft.VisualStudio.Services.Identity.Identity> SearchUsersByDirectoryAlias(
      TestManagementRequestContext context,
      List<string> directoryAliases)
    {
      if (directoryAliases == null || !directoryAliases.Any<string>())
        return new List<Microsoft.VisualStudio.Services.Identity.Identity>();
      directoryAliases = directoryAliases.Distinct<string>().ToList<string>();
      return directoryAliases.Select<string, Microsoft.VisualStudio.Services.Identity.Identity>((Func<string, Microsoft.VisualStudio.Services.Identity.Identity>) (n => IdentityHelper.SearchIdentityByDirectoryAlias(context, n))).Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (n => n != null)).ToList<Microsoft.VisualStudio.Services.Identity.Identity>();
    }

    internal static Microsoft.VisualStudio.Services.Identity.Identity SearchUserByIdentityDescriptor(
      TestManagementRequestContext context,
      IdentityDescriptor identityDescriptor)
    {
      return context.IdentityService.ReadIdentities(context.RequestContext, (IList<IdentityDescriptor>) new List<IdentityDescriptor>()
      {
        identityDescriptor
      }, QueryMembership.None, (IEnumerable<string>) null).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
    }

    internal static Dictionary<Guid, string> ResolveIdentities(
      TestManagementRequestContext context,
      Guid[] teamFoundationIds)
    {
      using (PerfManager.Measure(context.RequestContext, "BusinessLayer", "IdentityHelper.ResolveIdentities"))
      {
        ArgumentUtility.CheckForNull<TestManagementRequestContext>(context, nameof (context));
        Dictionary<Guid, string> dictionary = new Dictionary<Guid, string>();
        if (teamFoundationIds != null && teamFoundationIds.Length != 0)
        {
          IList<Microsoft.VisualStudio.Services.Identity.Identity> source = context.IdentityService.ReadIdentities(context.RequestContext, (IList<Guid>) teamFoundationIds, QueryMembership.None, (IEnumerable<string>) null);
          if (source != null && source.Any<Microsoft.VisualStudio.Services.Identity.Identity>())
          {
            foreach (Microsoft.VisualStudio.Services.Identity.Identity identity in (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) source)
            {
              if (identity != null)
              {
                string str = dictionary.ContainsValue(identity.DisplayName) ? IdentityHelper.MakeDisplayNameUnique(identity) : identity.DisplayName;
                if (!dictionary.ContainsKey(identity.Id))
                  dictionary.Add(identity.Id, str);
              }
            }
            if (context.UserTeamFoundationId != Guid.Empty && !dictionary.ContainsKey(context.UserTeamFoundationId))
              dictionary.Add(context.UserTeamFoundationId, context.UserTeamFoundationName);
            if (!dictionary.ContainsKey(Guid.Empty))
              dictionary.Add(Guid.Empty, (string) null);
          }
        }
        return dictionary;
      }
    }

    internal static Dictionary<Guid, Tuple<string, string>> ResolveIdentitiesEx(
      TestManagementRequestContext context,
      IList<Guid> teamFoundationIds)
    {
      using (PerfManager.Measure(context.RequestContext, "BusinessLayer", "IdentityHelper.ResolveIdentitiesEx"))
      {
        ArgumentUtility.CheckForNull<TestManagementRequestContext>(context, nameof (context));
        Dictionary<Guid, Tuple<string, string>> dictionary = new Dictionary<Guid, Tuple<string, string>>();
        if (teamFoundationIds != null && teamFoundationIds.Count > 0)
        {
          IList<Microsoft.VisualStudio.Services.Identity.Identity> source = context.IdentityService.ReadIdentities(context.RequestContext, teamFoundationIds, QueryMembership.None, (IEnumerable<string>) null);
          if (source != null && source.Any<Microsoft.VisualStudio.Services.Identity.Identity>())
          {
            foreach (Microsoft.VisualStudio.Services.Identity.Identity identity in (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) source)
            {
              if (identity != null && !dictionary.ContainsKey(identity.Id))
                dictionary.Add(identity.Id, new Tuple<string, string>(identity.DisplayName, IdentityHelper.GetDomainUserName(identity)));
            }
            if (!dictionary.ContainsKey(Guid.Empty))
              dictionary.Add(Guid.Empty, (Tuple<string, string>) null);
          }
        }
        return dictionary;
      }
    }

    internal static Dictionary<Guid, string> ResolveIdentities(
      TestManagementRequestContext context,
      IEnumerable<Guid> teamFoundationIds)
    {
      HashSet<Guid> source = new HashSet<Guid>(teamFoundationIds);
      return IdentityHelper.ResolveIdentities(context, source.ToArray<Guid>());
    }

    internal static string ResolveIdentityToName(
      TestManagementRequestContext context,
      Guid teamFoundationId,
      bool returnsDistinctName = false)
    {
      ArgumentUtility.CheckForNull<TestManagementRequestContext>(context, nameof (context));
      if (teamFoundationId != Guid.Empty)
      {
        if (teamFoundationId == context.UserTeamFoundationId)
          return !returnsDistinctName ? context.UserTeamFoundationName : context.UserDistinctTeamFoundationName;
        try
        {
          IList<Microsoft.VisualStudio.Services.Identity.Identity> source1 = context.IdentityService.ReadIdentities(context.RequestContext, (IList<Guid>) new Guid[1]
          {
            teamFoundationId
          }, QueryMembership.None, (IEnumerable<string>) null);
          if (source1 != null)
          {
            if (source1.Any<Microsoft.VisualStudio.Services.Identity.Identity>())
            {
              if (source1.Count > 1)
              {
                IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> source2 = source1.Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (i => i != null && i.IsActive));
                if (source2 != null && source2.Count<Microsoft.VisualStudio.Services.Identity.Identity>() > 0)
                {
                  Microsoft.VisualStudio.Services.Identity.Identity identity = source2.First<Microsoft.VisualStudio.Services.Identity.Identity>();
                  return returnsDistinctName ? IdentityHelper.GetDistinctDisplayName(identity.DisplayName, IdentityHelper.GetDisambiguationPart(identity)) : identity.DisplayName;
                }
              }
              return returnsDistinctName ? IdentityHelper.GetDistinctDisplayName(source1[0].DisplayName, IdentityHelper.GetDisambiguationPart(source1[0])) : source1[0].DisplayName;
            }
          }
        }
        catch
        {
        }
      }
      return string.Empty;
    }

    internal static UpdatedProperties ResolveIdentity(
      this UpdatedProperties property,
      TestManagementRequestContext context)
    {
      if (property != null)
        property.LastUpdatedByName = IdentityHelper.ResolveIdentityToName(context, property.LastUpdatedBy);
      return property;
    }

    internal static string GetDistinctDisplayName(string displayName, string uniqueName) => string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0} <{1}>", (object) displayName, (object) uniqueName);

    internal static string GetDistinctDisplayName(Microsoft.VisualStudio.Services.Identity.Identity identity) => string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0} <{1}>", (object) identity.DisplayName, (object) IdentityHelper.GetDisambiguationPart(identity));

    private static Microsoft.VisualStudio.Services.Identity.Identity SearchIdentityByDisplayName(
      TestManagementRequestContext context,
      string displayName)
    {
      return IdentityHelper.SearchIdentity(context, IdentitySearchFilter.DisplayName, displayName);
    }

    private static Microsoft.VisualStudio.Services.Identity.Identity SearchIdentityByDirectoryAlias(
      TestManagementRequestContext context,
      string directoryAlias)
    {
      return IdentityHelper.SearchIdentity(context, IdentitySearchFilter.DirectoryAlias, directoryAlias);
    }

    private static Microsoft.VisualStudio.Services.Identity.Identity SearchIdentity(
      TestManagementRequestContext context,
      IdentitySearchFilter searchFactor,
      string factorValue)
    {
      if (!string.IsNullOrEmpty(factorValue))
      {
        IList<Microsoft.VisualStudio.Services.Identity.Identity> source = context.IdentityService.ReadIdentities(context.RequestContext, searchFactor, factorValue, QueryMembership.None, (IEnumerable<string>) null, ReadIdentitiesOptions.None);
        if (source != null && source.Any<Microsoft.VisualStudio.Services.Identity.Identity>())
          return source[0];
      }
      return (Microsoft.VisualStudio.Services.Identity.Identity) null;
    }

    private static string GetDomainUserName(Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      ArgumentUtility.CheckForNull<Microsoft.VisualStudio.Services.Identity.Identity>(identity, nameof (identity));
      string displayableName;
      UserNameUtil.GetIdentityName(identity.Descriptor.IdentityType, identity.DisplayName, IdentityHelper.GetAttribute(identity, "Domain", string.Empty), IdentityHelper.GetAttribute(identity, "Account", string.Empty), identity.UniqueUserId, out string _, out displayableName);
      return displayableName;
    }

    private static string MakeDisplayNameUnique(Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      string domainUserName = IdentityHelper.GetDomainUserName(identity);
      return IdentityHelper.GetDistinctDisplayName(identity.DisplayName, domainUserName);
    }

    private static string GetDisambiguationPart(Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      string attribute1 = IdentityHelper.GetAttribute(identity, "Account", string.Empty);
      if (!identity.IsContainer && (identity.IsExternalUser || IdentityHelper.IsMsaDomain(identity)))
        return attribute1;
      string attribute2 = IdentityHelper.GetAttribute(identity, "LocalScopeId", string.Empty);
      string attribute3 = IdentityHelper.GetAttribute(identity, "Domain", string.Empty);
      if (!string.IsNullOrEmpty(attribute2))
        return string.Format("{0}{1}", (object) "id:", (object) identity.Id);
      return string.IsNullOrEmpty(attribute3) ? attribute1 : string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}\\{1}", (object) attribute3, (object) attribute1);
    }

    private static bool IsMsaDomain(Microsoft.VisualStudio.Services.Identity.Identity identity) => IdentityHelper.GetAttribute(identity, "Domain", string.Empty).Equals("Windows Live ID", StringComparison.OrdinalIgnoreCase);

    internal static string GetAttribute(Microsoft.VisualStudio.Services.Identity.Identity identity, string name, string defaultValue)
    {
      object obj;
      return identity.TryGetProperty(name, out obj) && obj != null ? obj.ToString() : defaultValue;
    }

    public static IdentityRef ToIdentityRef(
      IVssRequestContext requestContext,
      string id,
      string name)
    {
      IdentityRef identityRef = new IdentityRef();
      if (!string.IsNullOrEmpty(id))
        identityRef.Id = id;
      if (!string.IsNullOrEmpty(name))
        identityRef.DisplayName = name;
      requestContext.CheckPermissionToReadPublicIdentityInfo();
      return identityRef;
    }
  }
}
