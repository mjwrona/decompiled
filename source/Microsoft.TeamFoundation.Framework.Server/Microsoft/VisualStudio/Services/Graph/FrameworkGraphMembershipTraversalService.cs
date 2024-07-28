// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Graph.FrameworkGraphMembershipTraversalService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Graph.Client;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Graph
{
  internal class FrameworkGraphMembershipTraversalService : 
    IGraphMembershipTraversalService,
    IVssFrameworkService
  {
    private WellKnownIdentifierMapper m_wellKnownIdentifierMapper;
    private static readonly PerformanceTracer s_tracer = new PerformanceTracer(GraphMembershipPerfCounters.StandardSet, "Graph", nameof (FrameworkGraphMembershipTraversalService));
    internal static readonly TimedActionTracePoints TraceTraverseDescendants = new TimedActionTracePoints(10006100, 10006101, 10006102, 10006103);
    internal static readonly TimedActionTracePoints TraceLookupDescendantsTraversals = new TimedActionTracePoints(10006200, 10006201, 10006202, 10006203);
    private Guid m_serviceHostId;
    private const string c_area = "Graph";
    private const string c_layer = "FrameworkGraphMembershipTraversalService";

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      this.m_serviceHostId = systemRequestContext.ServiceHost.InstanceId;
      this.m_wellKnownIdentifierMapper = new WellKnownIdentifierMapper(systemRequestContext.ServiceHost.InstanceId);
      this.ValidateRequestContext(systemRequestContext);
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public GraphMembershipTraversal TraverseDescendants(
      IVssRequestContext context,
      SubjectDescriptor subjectDescriptor,
      int depth)
    {
      return FrameworkGraphMembershipTraversalService.s_tracer.TraceTimedAction<GraphMembershipTraversal>(context, FrameworkGraphMembershipTraversalService.TraceTraverseDescendants, (Func<GraphMembershipTraversal>) (() =>
      {
        this.ValidateRequestContext(context);
        GraphValidation.CheckDescriptor(subjectDescriptor, nameof (subjectDescriptor));
        context.TraceDataConditionally(10006104, TraceLevel.Verbose, "Graph", nameof (FrameworkGraphMembershipTraversalService), "Traversing Descendants for subject descriptor and depth", (Func<object>) (() => (object) new
        {
          subjectDescriptor = subjectDescriptor,
          depth = depth
        }), nameof (TraverseDescendants));
        subjectDescriptor = this.m_wellKnownIdentifierMapper.MapFromWellKnownIdentifier(subjectDescriptor);
        GraphMembershipTraversal graphMembershipTraversal = context.GetClient<GraphHttpClient>().TraverseMembershipsAsync((string) subjectDescriptor, new GraphTraversalDirection?(GraphTraversalDirection.Down), new int?(depth)).SyncResult<GraphMembershipTraversal>();
        context.TraceDataConditionally(10006107, TraceLevel.Verbose, "Graph", nameof (FrameworkGraphMembershipTraversalService), "Traversed Descendants for subject descriptor ", (Func<object>) (() => (object) new
        {
          subjectDescriptor = subjectDescriptor,
          graphMembershipTraversal = graphMembershipTraversal
        }), nameof (TraverseDescendants));
        return graphMembershipTraversal;
      }), actionName: nameof (TraverseDescendants));
    }

    public IReadOnlyDictionary<SubjectDescriptor, GraphMembershipTraversal> LookupDescendantsTraversals(
      IVssRequestContext context,
      IEnumerable<SubjectDescriptor> subjectDescriptors,
      int traversalDepth)
    {
      return FrameworkGraphMembershipTraversalService.s_tracer.TraceTimedAction<IReadOnlyDictionary<SubjectDescriptor, GraphMembershipTraversal>>(context, FrameworkGraphMembershipTraversalService.TraceLookupDescendantsTraversals, (Func<IReadOnlyDictionary<SubjectDescriptor, GraphMembershipTraversal>>) (() =>
      {
        this.ValidateRequestContext(context);
        context.TraceDataConditionally(10006204, TraceLevel.Verbose, "Graph", nameof (FrameworkGraphMembershipTraversalService), "Traversing Descendants for subject descriptor and depth", (Func<object>) (() => (object) new
        {
          subjectDescriptors = subjectDescriptors,
          traversalDepth = traversalDepth
        }), nameof (LookupDescendantsTraversals));
        foreach (SubjectDescriptor subjectDescriptor in subjectDescriptors)
          GraphValidation.CheckDescriptor(subjectDescriptor, nameof (subjectDescriptors));
        subjectDescriptors = (IEnumerable<SubjectDescriptor>) subjectDescriptors.Select<SubjectDescriptor, SubjectDescriptor>((Func<SubjectDescriptor, SubjectDescriptor>) (x => this.m_wellKnownIdentifierMapper.MapFromWellKnownIdentifier(x))).ToList<SubjectDescriptor>();
        GraphSubjectLookup membershipTraversalLookup = new GraphSubjectLookup((IEnumerable<GraphSubjectLookupKey>) subjectDescriptors.Select<SubjectDescriptor, GraphSubjectLookupKey>((Func<SubjectDescriptor, GraphSubjectLookupKey>) (x => new GraphSubjectLookupKey(x))).ToList<GraphSubjectLookupKey>());
        IReadOnlyDictionary<SubjectDescriptor, GraphMembershipTraversal> graphMembershipTraversals = context.GetClient<GraphHttpClient>().LookupMembershipTraversalsAsync(membershipTraversalLookup, new GraphTraversalDirection?(GraphTraversalDirection.Down), new int?(traversalDepth)).SyncResult<IReadOnlyDictionary<SubjectDescriptor, GraphMembershipTraversal>>();
        context.TraceDataConditionally(10006207, TraceLevel.Verbose, "Graph", nameof (FrameworkGraphMembershipTraversalService), "Traversed Descendants for subject descriptor ", (Func<object>) (() => (object) new
        {
          graphMembershipTraversals = graphMembershipTraversals
        }), nameof (LookupDescendantsTraversals));
        return graphMembershipTraversals;
      }), actionName: nameof (LookupDescendantsTraversals));
    }

    private void ValidateRequestContext(IVssRequestContext context)
    {
      context.CheckServiceHostType(TeamFoundationHostType.ProjectCollection);
      context.CheckServiceHostId(this.m_serviceHostId, (IVssFrameworkService) this);
    }
  }
}
