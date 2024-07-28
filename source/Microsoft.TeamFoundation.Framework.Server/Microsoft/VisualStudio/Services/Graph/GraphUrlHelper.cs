// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Graph.GraphUrlHelper
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Location.Server;
using System;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Graph
{
  internal static class GraphUrlHelper
  {
    private const string Area = "Graph";
    private const string Layer = "GraphUrlHelper";

    public static string GetGraphGroupUrl(
      IVssRequestContext requestContext,
      SubjectDescriptor groupDescriptor)
    {
      return GraphUrlHelper.GetGraphMemberlUrl(requestContext, GraphResourceIds.Groups.GroupsResourceLocationId, (object) new
      {
        groupDescriptor = groupDescriptor
      }, 10008219);
    }

    public static string GetGraphScopeUrl(
      IVssRequestContext requestContext,
      SubjectDescriptor scopeDescriptor)
    {
      try
      {
        return GraphUrlHelper.GetGraphLocationDataProvider(requestContext).GetResourceUri(requestContext, "Graph", GraphResourceIds.Scopes.ScopesResourceLocationId, (object) new
        {
          scopeDescriptor = scopeDescriptor
        }).AbsoluteUri;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(10008218, "Graph", nameof (GraphUrlHelper), ex);
        return string.Empty;
      }
    }

    public static string GetGraphUserUrl(
      IVssRequestContext requestContext,
      SubjectDescriptor userDescriptor)
    {
      return GraphUrlHelper.GetGraphMemberlUrl(requestContext, GraphResourceIds.Users.UsersResourceLocationId, (object) new
      {
        userDescriptor = userDescriptor
      }, 10008219);
    }

    public static string GetGraphServicePrincipalUrl(
      IVssRequestContext requestContext,
      SubjectDescriptor servicePrincipalDescriptor)
    {
      return GraphUrlHelper.GetGraphMemberlUrl(requestContext, GraphResourceIds.ServicePrincipals.ServicePrincipalsResourceLocationId, (object) new
      {
        servicePrincipalDescriptor = servicePrincipalDescriptor
      }, 10008219);
    }

    public static string GetGraphMembershipUrl(
      IVssRequestContext requestContext,
      SubjectDescriptor subjectDescriptor,
      SubjectDescriptor containerDescriptor)
    {
      try
      {
        return GraphUrlHelper.GetGraphLocationDataProvider(requestContext).GetResourceUri(requestContext, "Graph", GraphResourceIds.Memberships.MembershipsResourceLocationId, (object) new
        {
          subjectDescriptor = subjectDescriptor,
          containerDescriptor = containerDescriptor
        }).AbsoluteUri;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(10008228, "Graph", nameof (GraphUrlHelper), ex);
        return string.Empty;
      }
    }

    public static string GetGraphMembershipsUrl(
      IVssRequestContext requestContext,
      SubjectDescriptor subjectDescriptor)
    {
      try
      {
        return GraphUrlHelper.GetGraphLocationDataProvider(requestContext).GetResourceUri(requestContext, "Graph", GraphResourceIds.Memberships.MembershipsBatchResourceLocationId, (object) new
        {
          subjectDescriptor = subjectDescriptor
        }).AbsoluteUri;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(10008220, "Graph", nameof (GraphUrlHelper), ex);
        return string.Empty;
      }
    }

    public static string GetGraphMembershipStateUrl(
      IVssRequestContext requestContext,
      SubjectDescriptor subjectDescriptor)
    {
      try
      {
        return GraphUrlHelper.GetGraphLocationDataProvider(requestContext).GetResourceUri(requestContext, "Graph", GraphResourceIds.Memberships.MembershipStatesResourceLocationId, (object) new
        {
          subjectDescriptor = subjectDescriptor
        }).AbsoluteUri;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(10008221, "Graph", nameof (GraphUrlHelper), ex);
        return string.Empty;
      }
    }

    public static string GetGraphStorageKeyUrl(
      IVssRequestContext requestContext,
      SubjectDescriptor subjectDescriptor)
    {
      try
      {
        return GraphUrlHelper.GetGraphLocationDataProvider(requestContext).GetResourceUri(requestContext, "Graph", GraphResourceIds.StorageKeys.StorageKeysResourceLocationId, (object) new
        {
          subjectDescriptor = subjectDescriptor
        }).AbsoluteUri;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(10008222, "Graph", nameof (GraphUrlHelper), ex);
        return string.Empty;
      }
    }

    public static string GetGraphGroupsUrl(
      IVssRequestContext requestContext,
      SubjectDescriptor scopeDescriptor)
    {
      try
      {
        return GraphUrlHelper.GetGraphLocationDataProvider(requestContext).GetResourceUri(requestContext, "Graph", GraphResourceIds.Groups.GroupsResourceLocationId, (object) new
        {
          scopeDescriptor = scopeDescriptor
        }).AbsoluteUri;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(10008224, "Graph", nameof (GraphUrlHelper), ex);
        return string.Empty;
      }
    }

    public static string GetGraphDescriptorUrl(IVssRequestContext requestContext, Guid storageKey)
    {
      try
      {
        return GraphUrlHelper.GetGraphLocationDataProvider(requestContext).GetResourceUri(requestContext, "Graph", GraphResourceIds.Descriptors.DescriptorsResourceLocationId, (object) new
        {
          storageKey = storageKey
        }).AbsoluteUri;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(10008225, "Graph", nameof (GraphUrlHelper), ex);
        return string.Empty;
      }
    }

    public static string GetGraphSubjectUrl(
      IVssRequestContext requestContext,
      SubjectDescriptor subjectDescriptor)
    {
      try
      {
        if (subjectDescriptor == new SubjectDescriptor())
          return string.Empty;
        if (subjectDescriptor.IsGroupScopeType())
          return GraphUrlHelper.GetGraphScopeUrl(requestContext, subjectDescriptor);
        if (subjectDescriptor.IsGroupType())
          return GraphUrlHelper.GetGraphGroupUrl(requestContext, subjectDescriptor);
        if (subjectDescriptor.IsAadServicePrincipalType())
          return GraphUrlHelper.GetGraphServicePrincipalUrl(requestContext, subjectDescriptor);
        if (subjectDescriptor.IsUserType())
          return GraphUrlHelper.GetGraphUserUrl(requestContext, subjectDescriptor);
        if (Microsoft.TeamFoundation.Framework.Server.ServicePrincipals.IsServicePrincipal(requestContext, requestContext.GetAuthenticatedDescriptor()))
          return GraphUrlHelper.GetGraphLocationDataProvider(requestContext).GetResourceUri(requestContext, "Graph", GraphResourceIds.Subjects.SubjectsResourceLocationId, (object) new
          {
            subjectDescriptor = subjectDescriptor
          }).AbsoluteUri;
        requestContext.Trace(10008223, TraceLevel.Error, "Graph", nameof (GraphUrlHelper), "Unexpected call to GetGraphSubjectUrl from non S2S call. Subject Descriptor : {0}", (object) subjectDescriptor);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(10008226, "Graph", nameof (GraphUrlHelper), ex);
      }
      return string.Empty;
    }

    private static ILocationDataProvider GetGraphLocationDataProvider(
      IVssRequestContext requestContext)
    {
      return requestContext.GetService<ILocationService>().GetLocationData(requestContext, GraphResourceIds.AreaIdGuid);
    }

    private static string GetGraphMemberlUrl(
      IVssRequestContext requestContext,
      Guid identifier,
      object routeValues,
      int tracepoint)
    {
      try
      {
        return GraphUrlHelper.GetGraphLocationDataProvider(requestContext).GetResourceUri(requestContext, "Graph", identifier, routeValues).AbsoluteUri;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(tracepoint, "Graph", nameof (GraphUrlHelper), ex);
        return string.Empty;
      }
    }
  }
}
