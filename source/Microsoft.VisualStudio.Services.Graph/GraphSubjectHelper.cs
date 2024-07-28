// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Graph.GraphSubjectHelper
// Assembly: Microsoft.VisualStudio.Services.Graph, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 00390AA0-D8BB-45EB-AEF5-70DC8BFC765D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Graph.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Graph.Client;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Microsoft.VisualStudio.Services.Graph
{
  public static class GraphSubjectHelper
  {
    public static IList<SubjectDescriptor> CreateDedupedListOfSubjectDescriptorsFromString(
      string subjectDescriptors)
    {
      if (string.IsNullOrWhiteSpace(subjectDescriptors))
        return (IList<SubjectDescriptor>) Array.Empty<SubjectDescriptor>();
      try
      {
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        return (IList<SubjectDescriptor>) ((IEnumerable<string>) subjectDescriptors.Split(',')).Where<string>((Func<string, bool>) (x => !string.IsNullOrEmpty(x))).Distinct<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase).Select<string, string>(GraphSubjectHelper.\u003C\u003EO.\u003C0\u003E__UrlDecode ?? (GraphSubjectHelper.\u003C\u003EO.\u003C0\u003E__UrlDecode = new Func<string, string>(WebUtility.UrlDecode))).Select<string, SubjectDescriptor>(GraphSubjectHelper.\u003C\u003EO.\u003C1\u003E__FromString ?? (GraphSubjectHelper.\u003C\u003EO.\u003C1\u003E__FromString = new Func<string, SubjectDescriptor>(SubjectDescriptor.FromString))).ToList<SubjectDescriptor>();
      }
      catch (Exception ex)
      {
        TeamFoundationEventLog.Default.LogException(ex.Message, ex);
        throw;
      }
    }

    public static IList<Guid> CreateDedupedListOfIdsFromString(string ids)
    {
      if (string.IsNullOrWhiteSpace(ids))
        return (IList<Guid>) Array.Empty<Guid>();
      try
      {
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        return (IList<Guid>) ((IEnumerable<string>) ids.Split(',')).Where<string>((Func<string, bool>) (x => x != string.Empty)).Distinct<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase).Select<string, Guid>(GraphSubjectHelper.\u003C\u003EO.\u003C2\u003E__Parse ?? (GraphSubjectHelper.\u003C\u003EO.\u003C2\u003E__Parse = new Func<string, Guid>(Guid.Parse))).ToList<Guid>();
      }
      catch (Exception ex)
      {
        TeamFoundationEventLog.Default.LogException(ex.Message, ex);
        throw;
      }
    }

    internal static Microsoft.VisualStudio.Services.Identity.Identity FetchSingleIdentityByDescriptor(
      IVssRequestContext requestContext,
      SubjectDescriptor descriptor,
      QueryMembership queryMembership = QueryMembership.None,
      bool throwIfNotFound = false)
    {
      Microsoft.VisualStudio.Services.Identity.Identity identity = requestContext.GetService<IdentityService>().ReadIdentities(requestContext, (IList<SubjectDescriptor>) new SubjectDescriptor[1]
      {
        descriptor
      }, queryMembership, (IEnumerable<string>) null).Single<Microsoft.VisualStudio.Services.Identity.Identity>();
      return !throwIfNotFound || identity != null ? identity : throw new GraphSubjectNotFoundException(descriptor);
    }

    internal static IList<Microsoft.VisualStudio.Services.Identity.Identity> FetchIdentitiesByDescriptors(
      IVssRequestContext requestContext,
      IEnumerable<SubjectDescriptor> descriptors,
      QueryMembership queryMembership = QueryMembership.None,
      bool throwIfNotFound = false)
    {
      IList<Microsoft.VisualStudio.Services.Identity.Identity> source1 = requestContext.GetService<IdentityService>().ReadIdentities(requestContext, (IList<SubjectDescriptor>) descriptors.ToList<SubjectDescriptor>(), queryMembership, (IEnumerable<string>) null);
      if (throwIfNotFound)
      {
        IEnumerable<SubjectDescriptor> source2 = descriptors.Except<SubjectDescriptor>(source1.Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (x => x != null)).Select<Microsoft.VisualStudio.Services.Identity.Identity, SubjectDescriptor>((Func<Microsoft.VisualStudio.Services.Identity.Identity, SubjectDescriptor>) (x => x.SubjectDescriptor)));
        if (source2.Count<SubjectDescriptor>() > 0)
          throw new GraphSubjectNotFoundException(string.Join<SubjectDescriptor>(",", (IEnumerable<SubjectDescriptor>) source2.ToArray<SubjectDescriptor>()));
      }
      return source1;
    }

    public static IdentityScope FetchIdentityScope(
      IVssRequestContext requestContext,
      SubjectDescriptor descriptor)
    {
      return requestContext.GetService<IPlatformIdentityServiceInternal>().GetScopeByMasterId(requestContext, descriptor.GetMasterScopeId());
    }

    public static Guid GetLocalScopeId(
      this SubjectDescriptor scopeDescriptor,
      IVssRequestContext requestContext)
    {
      return (GraphSubjectHelper.FetchIdentityScope(requestContext, scopeDescriptor) ?? throw new GraphSubjectNotFoundException(scopeDescriptor)).LocalScopeId;
    }

    public static GraphUser FetchGraphUser(
      IVssRequestContext requestContext,
      SubjectDescriptor userDescriptor)
    {
      return GraphSubjectHelper.FetchGraphMember<GraphUser>(requestContext, userDescriptor);
    }

    public static GraphServicePrincipal FetchGraphServicePrincipal(
      IVssRequestContext requestContext,
      SubjectDescriptor servicePrincipalDescriptor)
    {
      return GraphSubjectHelper.FetchGraphMember<GraphServicePrincipal>(requestContext, servicePrincipalDescriptor);
    }

    public static T FetchGraphMember<T>(
      IVssRequestContext requestContext,
      SubjectDescriptor memberDescriptor)
      where T : AadGraphMember
    {
      Microsoft.VisualStudio.Services.Identity.Identity identity = GraphSubjectHelper.FetchSingleIdentityByDescriptor(requestContext, memberDescriptor);
      GraphSubject graphSubjectClient = identity != null ? identity.ToGraphSubjectClient(requestContext) : (GraphSubject) null;
      return graphSubjectClient != null && graphSubjectClient is T obj ? obj : throw new GraphSubjectNotFoundException(memberDescriptor);
    }
  }
}
