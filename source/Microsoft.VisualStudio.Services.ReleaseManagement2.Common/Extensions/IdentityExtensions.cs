// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Common.Extensions.IdentityExtensions
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C3F75541-7C8A-4AF6-A47E-709CEEE7550D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Common.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Diagnostics;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Common.Extensions
{
  public static class IdentityExtensions
  {
    private const string MissingIdentitiesItemKey = "MS.VS.Services.ReleaseManagement2.IdentityMisses";

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", Justification = "This is not shown to the user and is for tracing")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "This is not shown to the user and is for tracing")]
    [SuppressMessage("Microsoft.Design", "CA1026:Default parameters should not be used", Justification = "This rule is deprecated")]
    public static IDictionary<string, IdentityRef> QueryIdentities(
      this ICollection<string> identityIds,
      IVssRequestContext context,
      bool throwExceptionOnFailure = true,
      bool includeUrls = true)
    {
      if (identityIds == null)
        throw new ArgumentNullException(nameof (identityIds));
      if (context == null)
        throw new ArgumentNullException(nameof (context));
      Dictionary<string, IdentityRef> dictionary = new Dictionary<string, IdentityRef>();
      List<Guid> list1 = identityIds.Select<string, Guid>((Func<string, Guid>) (x => new Guid(x))).Distinct<Guid>().Where<Guid>((Func<Guid, bool>) (g => g != Guid.Empty)).ToList<Guid>();
      try
      {
        if (list1.Count<Guid>() > 0)
        {
          IList<Microsoft.VisualStudio.Services.Identity.Identity> identities;
          using (ReleaseManagementTimer.Create(context, "Service", "IdentityExtensions.QueryIdentities", 1961101))
          {
            IdentityService service = context.GetService<IdentityService>();
            identities = IdentityExtensions.GetIdentities(context, service, (IList<Guid>) list1, throwExceptionOnFailure);
          }
          foreach (string identityId1 in (IEnumerable<string>) identityIds)
          {
            string identityId = identityId1;
            List<Microsoft.VisualStudio.Services.Identity.Identity> list2 = identities.Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (m => m != null && m.Id == new Guid(identityId))).ToList<Microsoft.VisualStudio.Services.Identity.Identity>();
            bool includeUrls1 = includeUrls;
            if (list2.Count > 1)
            {
              IEnumerable<string> values = list2.Select<Microsoft.VisualStudio.Services.Identity.Identity, string>((Func<Microsoft.VisualStudio.Services.Identity.Identity, string>) (id => id.ToString()));
              context.Trace(1900025, TraceLevel.Info, "ReleaseManagementService", nameof (IdentityExtensions), "QueryIdentities -- Found multiple identities for {0}. Details are: {1}", (object) identityId, (object) string.Join(",", values));
            }
            Microsoft.VisualStudio.Services.Identity.Identity identity = list2.FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
            if (identityId == Guid.Empty.ToString())
              dictionary[identityId] = (IdentityRef) null;
            else if (identity == null)
            {
              dictionary[identityId] = new IdentityRef()
              {
                Id = identityId
              };
            }
            else
            {
              if (string.Equals(identityId, "0000000D-0000-8888-8000-000000000000", StringComparison.OrdinalIgnoreCase))
                includeUrls1 = false;
              dictionary[identityId] = identity.ToIdentityRef(context, includeUrls1, true);
            }
          }
        }
      }
      catch (Exception ex)
      {
        context.Trace(1961051, TraceLevel.Warning, "ReleaseManagementService", nameof (IdentityExtensions), "QueryIdentities for identities: {0}. Exception: {1}", (object) string.Join(",", (IEnumerable<string>) identityIds), (object) ex.ToString());
        if (throwExceptionOnFailure)
          throw;
      }
      return (IDictionary<string, IdentityRef>) dictionary;
    }

    public static string GetIdentityDisplayName(
      IVssRequestContext requestContext,
      string identityId)
    {
      IDictionary<string, IdentityRef> dictionary = ((ICollection<string>) new string[1]
      {
        identityId
      }).QueryIdentities(requestContext, false);
      return !dictionary.ContainsKey(identityId) || dictionary[identityId] == null ? string.Empty : dictionary[identityId].DisplayName;
    }

    public static IdentityRef GetIdentityReference(
      IVssRequestContext requestContext,
      string identityId)
    {
      IdentityRef valueOrDefault = ((ICollection<string>) new string[1]
      {
        identityId
      }).QueryIdentities(requestContext, false).GetValueOrDefault<string, IdentityRef>(identityId);
      if (valueOrDefault != null)
        return valueOrDefault;
      return new IdentityRef() { Id = identityId };
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", Justification = "This is not shown to the user and is for tracing")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "This is not shown to the user and is for tracing")]
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Code not shipped externally.")]
    [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Framework internals might change in future")]
    public static IList<Microsoft.VisualStudio.Services.Identity.Identity> GetIdentities(
      IVssRequestContext requestContext,
      IdentityService identityService,
      IList<Guid> identityIds,
      bool throwExceptionOnFailure = true)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      if (identityService == null)
        throw new ArgumentNullException(nameof (identityService));
      if (identityIds == null)
        throw new ArgumentNullException(nameof (identityIds));
      IList<Microsoft.VisualStudio.Services.Identity.Identity> source = (IList<Microsoft.VisualStudio.Services.Identity.Identity>) new List<Microsoft.VisualStudio.Services.Identity.Identity>();
      HashSet<Guid> guidSet1 = new HashSet<Guid>();
      HashSet<Guid> guidSet2 = (HashSet<Guid>) null;
      try
      {
        foreach (Guid guid in identityIds.ToList<Guid>())
        {
          if (!(guid == Guid.Empty) && (!requestContext.Items.TryGetValue<HashSet<Guid>>("MS.VS.Services.ReleaseManagement2.IdentityMisses", out guidSet2) || !guidSet2.Contains(guid)))
            guidSet1.Add(guid);
        }
        if (guidSet1.Count > 0)
          source = IdentityExtensions.ReadIdentities(requestContext, identityService, guidSet1);
        List<Guid> list = guidSet1.Except<Guid>(source.Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (x => x != null)).Select<Microsoft.VisualStudio.Services.Identity.Identity, Guid>((Func<Microsoft.VisualStudio.Services.Identity.Identity, Guid>) (y => y.Id))).ToList<Guid>();
        if (list.Any<Guid>())
        {
          if (guidSet2 == null && !requestContext.TryGetItem<HashSet<Guid>>("MS.VS.Services.ReleaseManagement2.IdentityMisses", out guidSet2))
          {
            guidSet2 = new HashSet<Guid>();
            requestContext.Items["MS.VS.Services.ReleaseManagement2.IdentityMisses"] = (object) guidSet2;
          }
          foreach (Guid guid in list)
            guidSet2.Add(guid);
        }
      }
      catch (Exception ex)
      {
        requestContext.Trace(1961101, TraceLevel.Warning, "ReleaseManagementService", nameof (IdentityExtensions), "GetIdentities for identities: {0}. Exception: {1}", (object) string.Join(",", (IEnumerable<string>) identityIds.Select<Guid, string>((Func<Guid, string>) (x => x.ToString("D"))).ToList<string>()), (object) ex.ToString());
        if (throwExceptionOnFailure)
          throw;
      }
      return source;
    }

    private static IList<Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentities(
      IVssRequestContext requestContext,
      IdentityService identityService,
      HashSet<Guid> identitiesToFetch)
    {
      IList<Microsoft.VisualStudio.Services.Identity.Identity> identityList = (IList<Microsoft.VisualStudio.Services.Identity.Identity>) null;
      int num1 = 0;
      int num2 = 3;
      while (num1 < num2)
      {
        ++num1;
        try
        {
          identityList = identityService.ReadIdentities(requestContext, (IList<Guid>) identitiesToFetch.ToList<Guid>(), QueryMembership.None, (IEnumerable<string>) null);
          break;
        }
        catch (Exception ex)
        {
          if (num1 >= num2)
            throw;
        }
      }
      return identityList;
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", Justification = "This is not shown to the user and is for tracing")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "This is not shown to the user and is for tracing")]
    public static IEnumerable<Guid> GetGroupIds(
      this Guid identity,
      IVssRequestContext requestContext,
      IEnumerable<Guid> userOrGroupIds,
      bool throwExceptionOnFailure = true)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      if (userOrGroupIds == null)
        throw new ArgumentNullException(nameof (userOrGroupIds));
      requestContext.Trace(1961052, TraceLevel.Info, "ReleaseManagementService", nameof (IdentityExtensions), "GetGroupIds for identity: {0}", (object) identity.ToString());
      List<Guid> groupIds = new List<Guid>();
      List<Guid> list1 = userOrGroupIds.ToList<Guid>();
      list1.Add(identity);
      List<Guid> list2 = list1.Distinct<Guid>().ToList<Guid>();
      try
      {
        IdentityService service = requestContext.GetService<IdentityService>();
        IList<Microsoft.VisualStudio.Services.Identity.Identity> identities1 = IdentityExtensions.GetIdentities(requestContext, service, (IList<Guid>) list2, throwExceptionOnFailure);
        if (identities1.IsNullOrEmpty<Microsoft.VisualStudio.Services.Identity.Identity>())
          return (IEnumerable<Guid>) groupIds;
        IList<Microsoft.VisualStudio.Services.Identity.Identity> list3 = (IList<Microsoft.VisualStudio.Services.Identity.Identity>) identities1.Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (i => i != null)).ToList<Microsoft.VisualStudio.Services.Identity.Identity>();
        if (!list3.Select<Microsoft.VisualStudio.Services.Identity.Identity, Guid>((Func<Microsoft.VisualStudio.Services.Identity.Identity, Guid>) (i => i.Id)).Contains<Guid>(identity))
          return (IEnumerable<Guid>) groupIds;
        Microsoft.VisualStudio.Services.Identity.Identity identity1 = list3.First<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (i => i.Id == identity));
        IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> identities2 = list3.Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (i => i.Id != identity && i.IsContainer));
        requestContext.Trace(1961052, TraceLevel.Info, "ReleaseManagementService", nameof (IdentityExtensions), "GetGroupIds -- Filter groupids {0} is member of.", (object) identity.ToString());
        foreach (Microsoft.VisualStudio.Services.Identity.Identity identity2 in identities2)
        {
          requestContext.Trace(1961052, TraceLevel.Verbose, "ReleaseManagementService", nameof (IdentityExtensions), "GetGroupIds -- Check if {0} is member of groupid {1}.", (object) identity.ToString(), (object) identity2.Id);
          if (service.IsMember(requestContext, identity2.Descriptor, identity1.Descriptor))
          {
            requestContext.Trace(1961052, TraceLevel.Verbose, "ReleaseManagementService", nameof (IdentityExtensions), "GetGroupIds -- {0} is member of groupid {1}.", (object) identity.ToString(), (object) identity2.Id);
            groupIds.Add(identity2.Id);
          }
        }
      }
      catch (Exception ex)
      {
        requestContext.Trace(1961052, TraceLevel.Warning, "ReleaseManagementService", nameof (IdentityExtensions), "GetGroupIds for identity: {0}, and groupIds: {1}. Exception: {2}", (object) identity.ToString(), (object) string.Join(",", (IEnumerable<string>) userOrGroupIds.Select<Guid, string>((Func<Guid, string>) (x => x.ToString("D"))).ToList<string>()), (object) ex.ToString());
        if (throwExceptionOnFailure)
          throw;
      }
      return (IEnumerable<Guid>) groupIds;
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", Justification = "This is not shown to the user and is for tracing")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "This is not shown to the user and is for tracing")]
    public static IList<Microsoft.VisualStudio.Services.Identity.Identity> GetIdentitiesIdFromName(
      IVssRequestContext context,
      string displayName,
      bool throwExceptionOnFailure = true)
    {
      if (context == null)
        throw new ArgumentNullException(nameof (context));
      if (string.IsNullOrWhiteSpace(displayName))
        return (IList<Microsoft.VisualStudio.Services.Identity.Identity>) null;
      context.Trace(1961053, TraceLevel.Verbose, "ReleaseManagementService", nameof (IdentityExtensions), "GetIdentityIdFromName for nameFilter: {0}", (object) displayName);
      IList<Microsoft.VisualStudio.Services.Identity.Identity> identitiesIdFromName = (IList<Microsoft.VisualStudio.Services.Identity.Identity>) new List<Microsoft.VisualStudio.Services.Identity.Identity>();
      try
      {
        identitiesIdFromName = context.GetService<IdentityService>().ReadIdentities(context, IdentitySearchFilter.General, displayName, QueryMembership.None, (IEnumerable<string>) null);
        context.Trace(1961053, TraceLevel.Verbose, "ReleaseManagementService", nameof (IdentityExtensions), "GetIdentityIdFromName -- GotIdentityIdFromName for nameFilter: {0}", (object) displayName);
      }
      catch (Exception ex)
      {
        context.Trace(1961053, TraceLevel.Warning, "ReleaseManagementService", nameof (IdentityExtensions), "GetIdentitiesIdFromName for nameFilter: {0}. Exception: {1}", (object) displayName, (object) ex.ToString());
        if (throwExceptionOnFailure)
          throw;
      }
      return identitiesIdFromName;
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", Justification = "This is not shown to the user and is for tracing")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "This is not shown to the user and is for tracing")]
    [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Framework internals might change in future")]
    public static IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> GetGroups(
      this IdentityService identityService,
      IVssRequestContext requestContext,
      IEnumerable<IdentityDescriptor> descriptors,
      Guid[] scopeIds,
      bool throwExceptionOnFailure = true)
    {
      if (identityService == null)
        throw new ArgumentNullException(nameof (identityService));
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      if (descriptors == null)
        throw new ArgumentNullException(nameof (descriptors));
      requestContext.TraceEnter(1960105, "ReleaseManagementService", "JobLayer", nameof (GetGroups));
      List<Microsoft.VisualStudio.Services.Identity.Identity> groups = new List<Microsoft.VisualStudio.Services.Identity.Identity>();
      try
      {
        IList<Microsoft.VisualStudio.Services.Identity.Identity> source = identityService.ListGroups(requestContext, scopeIds, false, (IEnumerable<string>) null);
        foreach (IdentityDescriptor descriptor1 in descriptors)
        {
          IdentityDescriptor descriptor = descriptor1;
          Microsoft.VisualStudio.Services.Identity.Identity identity = source.FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (x => IdentityHelper.IsWellKnownGroup(x.Descriptor, descriptor)));
          if (identity != null)
            groups.Add(identity);
        }
      }
      catch (Exception ex)
      {
        requestContext.Trace(1961054, TraceLevel.Warning, "ReleaseManagementService", nameof (IdentityExtensions), "GetGroups for descriptions: {0}, and scopeIds: {1}. Exception: {2}", (object) string.Join(",", (IEnumerable<string>) descriptors.Select<IdentityDescriptor, string>((Func<IdentityDescriptor, string>) (x => x.ToString())).ToList<string>()), scopeIds != null ? (object) string.Join(",", (IEnumerable<string>) ((IEnumerable<Guid>) scopeIds).Select<Guid, string>((Func<Guid, string>) (x => x.ToString("D"))).ToList<string>()) : (object) string.Empty, (object) ex.ToString());
        if (throwExceptionOnFailure)
          throw;
      }
      finally
      {
        requestContext.TraceLeave(1960106, "ReleaseManagementService", "JobLayer", nameof (GetGroups));
      }
      return (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) groups;
    }

    public static Microsoft.VisualStudio.Services.Identity.Identity GetServiceUsersGroup(
      this IdentityService identityService,
      IVssRequestContext requestContext)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      IdentityDescriptor[] descriptors = new IdentityDescriptor[1]
      {
        GroupWellKnownIdentityDescriptors.ServiceUsersGroup
      };
      return identityService.GetGroups(requestContext, (IEnumerable<IdentityDescriptor>) descriptors, (Guid[]) null, false).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
    }

    public static IEnumerable<Guid> GetIdentityIds(
      this IVssRequestContext requestContext,
      string nameOrId)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      if (string.IsNullOrWhiteSpace(nameOrId))
        return Enumerable.Empty<Guid>();
      Guid result;
      if (Guid.TryParse(nameOrId, out result))
        return (IEnumerable<Guid>) new List<Guid>()
        {
          result
        };
      IList<Microsoft.VisualStudio.Services.Identity.Identity> identitiesIdFromName = IdentityExtensions.GetIdentitiesIdFromName(requestContext, nameOrId, false);
      return identitiesIdFromName == null ? Enumerable.Empty<Guid>() : identitiesIdFromName.Select<Microsoft.VisualStudio.Services.Identity.Identity, Guid>((Func<Microsoft.VisualStudio.Services.Identity.Identity, Guid>) (identity => identity.Id));
    }

    public static Microsoft.VisualStudio.Services.Identity.Identity ToIdentity(
      this IdentityRef identityRef,
      IVssRequestContext requestContext)
    {
      return !requestContext.IsFeatureEnabled("AzureDevops.ReleaseManagement.SearchIdentityById") ? identityRef.SearchIdentityName(requestContext) : identityRef.SearchIdentityById(requestContext);
    }

    private static Microsoft.VisualStudio.Services.Identity.Identity SearchIdentityName(
      this IdentityRef identityRef,
      IVssRequestContext requestContext)
    {
      IList<Microsoft.VisualStudio.Services.Identity.Identity> list = (IList<Microsoft.VisualStudio.Services.Identity.Identity>) requestContext.GetService<IUserIdentityService>().ReadIdentities(requestContext, IdentitySearchFilter.DisplayName, identityRef.DisplayName, QueryMembership.None, (IEnumerable<string>) null).Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (i => i.Id.Equals(new Guid(identityRef.Id)))).ToList<Microsoft.VisualStudio.Services.Identity.Identity>();
      return list.Count <= 0 ? (Microsoft.VisualStudio.Services.Identity.Identity) null : list[0];
    }

    private static Microsoft.VisualStudio.Services.Identity.Identity SearchIdentityById(
      this IdentityRef identityRef,
      IVssRequestContext requestContext)
    {
      IdentityService service = requestContext.GetService<IdentityService>();
      Guid result;
      if (!Guid.TryParse(identityRef.Id, out result))
        return (Microsoft.VisualStudio.Services.Identity.Identity) null;
      return service.ReadIdentities(requestContext, (IList<Guid>) new Guid[1]
      {
        result
      }, QueryMembership.None, (IEnumerable<string>) null).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
    }
  }
}
