// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Graph.RestServices.GraphMembershipTraversalController
// Assembly: Microsoft.VisualStudio.Services.Graph, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 00390AA0-D8BB-45EB-AEF5-70DC8BFC765D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Graph.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Graph.Client;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Graph.RestServices
{
  [ControllerApiVersion(3.2)]
  [ClientInternalUseOnly(true)]
  [RestrictInternalGraphEndpoints]
  [VersionedApiControllerCustomName(Area = "Graph", ResourceName = "MembershipTraversals")]
  public class GraphMembershipTraversalController : GraphControllerBase
  {
    [HttpGet]
    [TraceFilter(6308000, 6308009)]
    public GraphMembershipTraversal TraverseMemberships(
      [ClientParameterType(typeof (string), false)] SubjectDescriptor subjectDescriptor,
      GraphTraversalDirection direction = GraphTraversalDirection.Unknown,
      int depth = 1)
    {
      if (direction == GraphTraversalDirection.Down)
        return this.TfsRequestContext.GetService<PlatformGraphMembershipTraversalService>().TraverseDescendants(this.TfsRequestContext, subjectDescriptor, depth);
      throw new NotImplementedException(Resources.UnsupportedGraphTraversalDirection());
    }

    [HttpPost]
    [RequestContentTypeRestriction(AllowJsonPatch = false)]
    [TraceFilter(6309000, 6309009)]
    public IReadOnlyDictionary<SubjectDescriptor, GraphMembershipTraversal> LookupMembershipTraversals(
      GraphSubjectLookup membershipTraversalLookup,
      GraphTraversalDirection direction = GraphTraversalDirection.Unknown,
      int depth = 1)
    {
      ArgumentUtility.CheckForNull<GraphSubjectLookup>(membershipTraversalLookup, nameof (membershipTraversalLookup));
      ArgumentUtility.CheckForNull<IEnumerable<GraphSubjectLookupKey>>(membershipTraversalLookup.LookupKeys, "LookupKeys");
      if (direction != GraphTraversalDirection.Down)
        throw new NotImplementedException(Resources.UnsupportedGraphTraversalDirection());
      IEnumerable<SubjectDescriptor> subjectDescriptors = membershipTraversalLookup.ToSubjectDescriptors();
      return !subjectDescriptors.IsNullOrEmpty<SubjectDescriptor>() ? this.TfsRequestContext.GetService<PlatformGraphMembershipTraversalService>().LookupDescendantsTraversals(this.TfsRequestContext, subjectDescriptors, depth) : (IReadOnlyDictionary<SubjectDescriptor, GraphMembershipTraversal>) new Dictionary<SubjectDescriptor, GraphMembershipTraversal>();
    }
  }
}
