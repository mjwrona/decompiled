// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.IdentityServiceExtensions
// Assembly: Microsoft.VisualStudio.Services.Licensing.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F3070F25-7414-49A0-9C00-005379F04A49
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Licensing
{
  public static class IdentityServiceExtensions
  {
    private const string s_area = "VisualStudio.Services.Licensing.IdentityServiceExtensions";
    private const string s_layer = "BusinessLogic";

    public static Microsoft.VisualStudio.Services.Identity.Identity ReadIdentityWithFallback(
      this IdentityService identityService,
      IVssRequestContext requestContext,
      Guid storageKey,
      QueryMembership queryMembership = QueryMembership.None,
      IEnumerable<string> propertyNameFilters = null,
      bool includeRestrictedVisibility = false)
    {
      return identityService.ReadIdentitiesWithFallback(requestContext, (IEnumerable<Guid>) new Guid[1]
      {
        storageKey
      }, queryMembership, propertyNameFilters, (includeRestrictedVisibility ? 1 : 0) != 0).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
    }

    public static Microsoft.VisualStudio.Services.Identity.Identity ReadIdentityWithFallback(
      this IdentityService identityService,
      IVssRequestContext requestContext,
      SubjectDescriptor subjectDescriptor,
      QueryMembership queryMembership = QueryMembership.None,
      IEnumerable<string> propertyNameFilters = null,
      bool includeRestrictedVisibility = false)
    {
      return identityService.ReadIdentitiesWithFallback(requestContext, (IEnumerable<SubjectDescriptor>) new SubjectDescriptor[1]
      {
        subjectDescriptor
      }, queryMembership, propertyNameFilters, (includeRestrictedVisibility ? 1 : 0) != 0).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
    }

    public static IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentitiesWithFallback(
      this IdentityService identityService,
      IVssRequestContext requestContext,
      IEnumerable<Guid> storageKeys,
      QueryMembership queryMembership = QueryMembership.None,
      IEnumerable<string> propertyNameFilters = null,
      bool includeRestrictedVisibility = false)
    {
      List<Guid> list1 = storageKeys.ToList<Guid>();
      List<Microsoft.VisualStudio.Services.Identity.Identity> list2 = identityService.ReadIdentities(requestContext, (IList<Guid>) list1.ToList<Guid>(), queryMembership, propertyNameFilters, includeRestrictedVisibility).Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (x => x != null)).ToList<Microsoft.VisualStudio.Services.Identity.Identity>();
      List<Guid> usersNotFound = list1.Except<Guid>(list2.Select<Microsoft.VisualStudio.Services.Identity.Identity, Guid>((Func<Microsoft.VisualStudio.Services.Identity.Identity, Guid>) (identity => identity.Id))).ToList<Guid>();
      if (list1.Count > list2.Count && usersNotFound.Any<Guid>())
      {
        IVssRequestContext readContext = requestContext.To(TeamFoundationHostType.Application);
        requestContext.TraceDataConditionally(5002105, TraceLevel.Error, "VisualStudio.Services.Licensing.IdentityServiceExtensions", "BusinessLogic", "Identities not found at initial host level.", (Func<object>) (() => (object) new
        {
          UserId = usersNotFound,
          CurrentHostType = requestContext.ServiceHost.HostType,
          RereadHostType = TeamFoundationHostTypeHelper.NormalizeHostType(readContext.ServiceHost.HostType),
          StackTrace = Environment.StackTrace
        }), nameof (ReadIdentitiesWithFallback));
        IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> collection = readContext.GetService<IdentityService>().ReadIdentities(readContext, (IList<Guid>) usersNotFound, queryMembership, propertyNameFilters, includeRestrictedVisibility).Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (x => x != null));
        list2.AddRange(collection);
      }
      usersNotFound = list1.Except<Guid>(list2.Select<Microsoft.VisualStudio.Services.Identity.Identity, Guid>((Func<Microsoft.VisualStudio.Services.Identity.Identity, Guid>) (identity => identity.Id))).ToList<Guid>();
      if (list1.Count > list2.Count && usersNotFound.Any<Guid>())
      {
        IVssRequestContext readContext = requestContext.To(TeamFoundationHostType.Deployment);
        requestContext.TraceDataConditionally(5002107, TraceLevel.Error, "VisualStudio.Services.Licensing.IdentityServiceExtensions", "BusinessLogic", "Identities not found at initial host level.", (Func<object>) (() => (object) new
        {
          UserId = usersNotFound,
          CurrentHostType = requestContext.ServiceHost.HostType,
          RereadHostType = TeamFoundationHostTypeHelper.NormalizeHostType(readContext.ServiceHost.HostType),
          StackTrace = Environment.StackTrace
        }), nameof (ReadIdentitiesWithFallback));
        IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> collection = readContext.GetService<IdentityService>().ReadIdentities(readContext, (IList<Guid>) usersNotFound, queryMembership, propertyNameFilters, includeRestrictedVisibility).Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (x => x != null));
        list2.AddRange(collection);
      }
      return (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) list2;
    }

    public static IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentitiesWithFallback(
      this IdentityService identityService,
      IVssRequestContext requestContext,
      IEnumerable<SubjectDescriptor> subjectDescriptors,
      QueryMembership queryMembership = QueryMembership.None,
      IEnumerable<string> propertyNameFilters = null,
      bool includeRestrictedVisibility = false)
    {
      List<SubjectDescriptor> list1 = subjectDescriptors.ToList<SubjectDescriptor>();
      List<Microsoft.VisualStudio.Services.Identity.Identity> list2 = identityService.ReadIdentities(requestContext, (IList<SubjectDescriptor>) list1.ToList<SubjectDescriptor>(), queryMembership, propertyNameFilters, includeRestrictedVisibility).Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (x => x != null)).ToList<Microsoft.VisualStudio.Services.Identity.Identity>();
      List<SubjectDescriptor> usersNotFound = list1.Except<SubjectDescriptor>(list2.Select<Microsoft.VisualStudio.Services.Identity.Identity, SubjectDescriptor>((Func<Microsoft.VisualStudio.Services.Identity.Identity, SubjectDescriptor>) (identity => identity.SubjectDescriptor))).ToList<SubjectDescriptor>();
      if (list1.Count > list2.Count && usersNotFound.Any<SubjectDescriptor>())
      {
        IVssRequestContext readContext = requestContext.To(TeamFoundationHostType.Application);
        requestContext.TraceDataConditionally(5002106, TraceLevel.Error, "VisualStudio.Services.Licensing.IdentityServiceExtensions", "BusinessLogic", "Identities not found at initial host level.", (Func<object>) (() => (object) new
        {
          UserId = usersNotFound,
          RereadHostType = TeamFoundationHostTypeHelper.NormalizeHostType(readContext.ServiceHost.HostType),
          StackTrace = Environment.StackTrace
        }), nameof (ReadIdentitiesWithFallback));
        IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> collection = readContext.GetService<IdentityService>().ReadIdentities(readContext, (IList<SubjectDescriptor>) usersNotFound, queryMembership, propertyNameFilters, includeRestrictedVisibility).Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (x => x != null));
        list2.AddRange(collection);
      }
      usersNotFound = list1.Except<SubjectDescriptor>(list2.Select<Microsoft.VisualStudio.Services.Identity.Identity, SubjectDescriptor>((Func<Microsoft.VisualStudio.Services.Identity.Identity, SubjectDescriptor>) (identity => identity.SubjectDescriptor))).ToList<SubjectDescriptor>();
      if (list1.Count > list2.Count && usersNotFound.Any<SubjectDescriptor>())
      {
        IVssRequestContext readContext = requestContext.To(TeamFoundationHostType.Deployment);
        requestContext.TraceDataConditionally(5002108, TraceLevel.Error, "VisualStudio.Services.Licensing.IdentityServiceExtensions", "BusinessLogic", "Identities not found at initial host level.", (Func<object>) (() => (object) new
        {
          UserId = usersNotFound,
          RereadHostType = TeamFoundationHostTypeHelper.NormalizeHostType(readContext.ServiceHost.HostType),
          StackTrace = Environment.StackTrace
        }), nameof (ReadIdentitiesWithFallback));
        IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> collection = readContext.GetService<IdentityService>().ReadIdentities(readContext, (IList<SubjectDescriptor>) usersNotFound, queryMembership, propertyNameFilters, includeRestrictedVisibility).Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (x => x != null));
        list2.AddRange(collection);
      }
      return (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) list2;
    }

    public static IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentitiesBatched(
      this IdentityService identityService,
      IVssRequestContext requestContext,
      IEnumerable<SubjectDescriptor> subjectDescriptors,
      int batchSize,
      QueryMembership queryMembership = QueryMembership.None,
      IEnumerable<string> propertyNameFilters = null,
      bool includeRestrictedVisibility = false)
    {
      return subjectDescriptors.Batch<SubjectDescriptor>(batchSize).SelectMany<IList<SubjectDescriptor>, Microsoft.VisualStudio.Services.Identity.Identity>((Func<IList<SubjectDescriptor>, IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>>) (batch => (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) identityService.ReadIdentities(requestContext, (IList<SubjectDescriptor>) batch.ToList<SubjectDescriptor>(), queryMembership, propertyNameFilters, includeRestrictedVisibility)));
    }
  }
}
