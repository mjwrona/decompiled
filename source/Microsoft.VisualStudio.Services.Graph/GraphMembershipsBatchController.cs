// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Graph.GraphMembershipsBatchController
// Assembly: Microsoft.VisualStudio.Services.Graph, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 00390AA0-D8BB-45EB-AEF5-70DC8BFC765D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Graph.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Graph.Client;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Graph
{
  [ControllerApiVersion(3.2)]
  [ClientGroupByResource("Memberships")]
  [VersionedApiControllerCustomName(Area = "Graph", ResourceName = "MembershipsBatch")]
  public class GraphMembershipsBatchController : GraphControllerBase
  {
    [HttpGet]
    [TraceFilter(6307200, 6307209)]
    [ClientExample("BatchGetMembershipsGroupDown.json", "All members of a group", null, null)]
    [ClientExample("BatchGetMembershipsUserUp.json", "All groups for a user", null, null)]
    [ClientExample("BatchGetMembershipsVSTSGroupUp.json", "All groups for a group", null, null)]
    public IEnumerable<GraphMembership> ListMemberships(
      [ClientParameterType(typeof (string), false)] SubjectDescriptor subjectDescriptor,
      GraphTraversalDirection direction = GraphTraversalDirection.Unknown,
      int depth = 1)
    {
      if (subjectDescriptor.IsGroupScopeType())
        throw new NotImplementedException();
      BusinessRulesValidator.ValidateGraphTraversalDepth(this.TfsRequestContext, depth);
      return GraphMembershipsBatchController.GetMembershipsUsingIdentityService(this.TfsRequestContext, subjectDescriptor, direction, depth);
    }

    private static IEnumerable<GraphMembership> GetMembershipsUsingIdentityService(
      IVssRequestContext requestContext,
      SubjectDescriptor subjectDescriptor,
      GraphTraversalDirection direction,
      int depth)
    {
      List<GraphMembership> usingIdentityService = new List<GraphMembership>();
      QueryMembership queryMembership = GraphMembershipsBatchController.GetQueryMembership(depth);
      Microsoft.VisualStudio.Services.Identity.Identity identity = GraphSubjectHelper.FetchSingleIdentityByDescriptor(requestContext, subjectDescriptor, queryMembership, true);
      GraphTraversalDirection traversalDirection = GraphTraversalDirection.Up;
      if (direction != GraphTraversalDirection.Unknown)
        traversalDirection = direction;
      subjectDescriptor = depth == 1 ? identity.SubjectDescriptor : new SubjectDescriptor();
      if (traversalDirection == GraphTraversalDirection.Up)
      {
        if (identity.MemberOf != null)
        {
          foreach (IdentityDescriptor identityDescriptor in (IEnumerable<IdentityDescriptor>) identity.MemberOf)
          {
            GraphMembership graphMembership = GraphResultExtensions.GetGraphMembership(requestContext, subjectDescriptor, identityDescriptor.ToSubjectDescriptor(requestContext));
            if (graphMembership != null)
              usingIdentityService.Add(graphMembership);
          }
        }
      }
      else if (identity.Members != null)
      {
        foreach (IdentityDescriptor member in (IEnumerable<IdentityDescriptor>) identity.Members)
        {
          GraphMembership graphMembership = GraphResultExtensions.GetGraphMembership(requestContext, member.ToSubjectDescriptor(requestContext), subjectDescriptor);
          if (graphMembership != null)
            usingIdentityService.Add(graphMembership);
        }
      }
      return (IEnumerable<GraphMembership>) usingIdentityService;
    }

    private static QueryMembership GetQueryMembership(int depth)
    {
      if (depth == -1)
        return QueryMembership.Expanded;
      if (depth == 1)
        return QueryMembership.Direct;
      throw new GraphBadRequestException(Resources.TraversalDepthNotSupported((object) depth, (object) nameof (depth)));
    }
  }
}
