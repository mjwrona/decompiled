// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Graph.PlatformGraphMembershipTraversalService
// Assembly: Microsoft.VisualStudio.Services.Graph, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 00390AA0-D8BB-45EB-AEF5-70DC8BFC765D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Graph.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Graph.Client;
using Microsoft.VisualStudio.Services.Graph.Store;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Graph
{
  public class PlatformGraphMembershipTraversalService : 
    IGraphMembershipTraversalService,
    IVssFrameworkService
  {
    private int m_maxSubjectsToTraverseInBatch;
    private int m_maxAadGroupExpansions;
    private int m_maxRemoteDescendantsToTraverse;
    private int m_maxDescendantsToTraverse;
    private int m_readIdentitiesBatchSize;
    private Guid m_serviceHostId;
    private const string c_area = "Graph";
    private const string c_layer = "PlatformGraphMembershipTraversalService";
    private static readonly PerformanceTracer s_tracer = new PerformanceTracer(GraphMembershipPerfCounters.StandardSet, "Graph", nameof (PlatformGraphMembershipTraversalService));

    public void ServiceStart(IVssRequestContext context)
    {
      context.CheckProjectCollectionRequestContext();
      this.m_serviceHostId = context.ServiceHost.InstanceId;
      IVssRequestContext vssRequestContext = context.To(TeamFoundationHostType.Deployment);
      vssRequestContext.GetService<IVssRegistryService>().RegisterNotification(vssRequestContext, new RegistrySettingsChangedCallback(this.OnRegistryChanged), "/Service/Sps/Graph/...");
      this.UpdateSettings(context);
      this.WarmUpGraphMembershipTraversalCacheForMegaTenanat(context);
    }

    public void ServiceEnd(IVssRequestContext context)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(context, nameof (context));
      IVssRequestContext vssRequestContext = context.To(TeamFoundationHostType.Deployment);
      vssRequestContext.GetService<IVssRegistryService>().UnregisterNotification(vssRequestContext, new RegistrySettingsChangedCallback(this.OnRegistryChanged));
    }

    public GraphMembershipTraversal TraverseDescendants(
      IVssRequestContext context,
      SubjectDescriptor subjectDescriptor,
      int traversalDepth)
    {
      if (!context.IsFeatureEnabled("VisualStudio.Services.Graph.EnableTraverseDescendants"))
        throw new FeatureFlagNotEnabledGraphException(Resources.TraverseDescendantsFeatureIsUnavailable());
      return PlatformGraphMembershipTraversalService.s_tracer.TraceTimedAction<GraphMembershipTraversal>(context, new TimedActionTracePoints(15280150, 15280151, 15280152, 15280153), (Func<GraphMembershipTraversal>) (() =>
      {
        context.CheckServiceHostId(this.m_serviceHostId, (IVssFrameworkService) this);
        context.TraceDataConditionally(15280251, TraceLevel.Verbose, "Graph", nameof (PlatformGraphMembershipTraversalService), string.Format("Received input parameters: subjectDescriptor: {0}, depth: {1}", (object) subjectDescriptor, (object) traversalDepth), methodName: nameof (TraverseDescendants));
        PlatformGraphMembershipTraversalService.ValidateDepth(traversalDepth);
        PlatformGraphMembershipTraversalService.ValidateGroupDescriptors((IEnumerable<SubjectDescriptor>) new SubjectDescriptor[1]
        {
          subjectDescriptor
        });
        GraphMembershipPermissionChecker.CheckReadMembershipsPermission(context, subjectDescriptor);
        GraphMembershipTraversal membershipTraversal;
        IVssRequestContext requestContext = this.LookupDescendantsTraversalsInternal(context, (IEnumerable<SubjectDescriptor>) new SubjectDescriptor[1]
        {
          subjectDescriptor
        }).TryGetValue(subjectDescriptor, out membershipTraversal) ? context : throw new GraphException(string.Format("No GraphMembershipTraversal result constructed for SubjectDescriptor: {0}", (object) subjectDescriptor));
        int? traversedSubjectIdsCount;
        if (membershipTraversal == null)
        {
          traversedSubjectIdsCount = new int?();
        }
        else
        {
          IEnumerable<Guid> traversedSubjectIds = membershipTraversal.TraversedSubjectIds;
          traversedSubjectIdsCount = traversedSubjectIds != null ? new int?(traversedSubjectIds.Count<Guid>()) : new int?();
        }
        this.PublishCustomerIntelligenceEvent(requestContext, traversedSubjectIdsCount);
        return membershipTraversal;
      }), actionName: nameof (TraverseDescendants));
    }

    public IReadOnlyDictionary<SubjectDescriptor, GraphMembershipTraversal> LookupDescendantsTraversals(
      IVssRequestContext context,
      IEnumerable<SubjectDescriptor> groupDescriptors,
      int traversalDepth)
    {
      return PlatformGraphMembershipTraversalService.s_tracer.TraceTimedAction<IReadOnlyDictionary<SubjectDescriptor, GraphMembershipTraversal>>(context, new TimedActionTracePoints(15280200, 15280201, 15280202, 15280203), (Func<IReadOnlyDictionary<SubjectDescriptor, GraphMembershipTraversal>>) (() =>
      {
        context.CheckServiceHostId(this.m_serviceHostId, (IVssFrameworkService) this);
        ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) groupDescriptors, nameof (groupDescriptors));
        IEnumerable<SubjectDescriptor> subjectDescriptors = groupDescriptors.Distinct<SubjectDescriptor>();
        if (subjectDescriptors.Count<SubjectDescriptor>() > this.m_maxSubjectsToTraverseInBatch)
          throw new TooManyRequestedItemsException(subjectDescriptors.Count<SubjectDescriptor>(), this.m_maxSubjectsToTraverseInBatch);
        context.TraceDataConditionally(15280301, TraceLevel.Verbose, "Graph", nameof (PlatformGraphMembershipTraversalService), string.Format("Received input parameters: subjectDescriptor: {0}, depth: {1}", (object) subjectDescriptors, (object) traversalDepth), methodName: nameof (LookupDescendantsTraversals));
        PlatformGraphMembershipTraversalService.ValidateDepth(traversalDepth);
        PlatformGraphMembershipTraversalService.ValidateGroupDescriptors(subjectDescriptors);
        GraphMembershipPermissionChecker.CheckMembershipTraversalsPermission(context, traversalDepth, GraphTraversalDirection.Down);
        IReadOnlyDictionary<SubjectDescriptor, GraphMembershipTraversal> readOnlyDictionary = this.LookupDescendantsTraversalsInternal(context, subjectDescriptors);
        IVssRequestContext requestContext = context;
        IEnumerable<GraphMembershipTraversal> values = readOnlyDictionary.Values;
        int? traversedSubjectIdsCount = values != null ? values.Sum<GraphMembershipTraversal>((Func<GraphMembershipTraversal, int?>) (x =>
        {
          IEnumerable<Guid> traversedSubjectIds = x.TraversedSubjectIds;
          return traversedSubjectIds == null ? new int?() : new int?(traversedSubjectIds.Count<Guid>());
        })) : new int?();
        this.PublishCustomerIntelligenceEvent(requestContext, traversedSubjectIdsCount);
        return readOnlyDictionary;
      }), actionName: nameof (LookupDescendantsTraversals));
    }

    private void PublishCustomerIntelligenceEvent(
      IVssRequestContext requestContext,
      int? traversedSubjectIdsCount = 0)
    {
      CustomerIntelligenceService service = requestContext.GetService<CustomerIntelligenceService>();
      Dictionary<string, object> data = new Dictionary<string, object>()
      {
        ["TraversedSubjectIdsCount"] = (object) traversedSubjectIdsCount
      };
      IVssRequestContext requestContext1 = requestContext;
      string graph = CustomerIntelligenceArea.Graph;
      string traverseDescendants = CustomerIntelligenceFeature.TraverseDescendants;
      CustomerIntelligenceData properties = new CustomerIntelligenceData((IDictionary<string, object>) data);
      service.Publish(requestContext1, graph, traverseDescendants, properties);
    }

    private static void ValidateDepth(int traversalDepth)
    {
      if (traversalDepth != -1)
        throw new GraphBadRequestException(Resources.UnsupportedTraversedDescendantsDepth((object) traversalDepth));
    }

    private static void ValidateGroupDescriptors(IEnumerable<SubjectDescriptor> descriptors)
    {
      List<SubjectDescriptor> subjectDescriptorList = new List<SubjectDescriptor>();
      foreach (SubjectDescriptor descriptor in descriptors)
      {
        GraphValidation.CheckDescriptor(descriptor, "descriptor");
        if (!descriptor.IsGroupType())
          subjectDescriptorList.Add(descriptor);
      }
      if (!subjectDescriptorList.IsNullOrEmpty<SubjectDescriptor>())
        throw new UnsupportedSubjectTypeForMembershipTraversal(Resources.UnsupportedSubjectTypeForMembershipTraversal((object) subjectDescriptorList.Select<SubjectDescriptor, string>((Func<SubjectDescriptor, string>) (x => x.SubjectType))));
    }

    private IReadOnlyDictionary<SubjectDescriptor, GraphMembershipTraversal> LookupDescendantsTraversalsInternal(
      IVssRequestContext context,
      IEnumerable<SubjectDescriptor> groupDescriptors)
    {
      Dictionary<SubjectDescriptor, GraphMembershipTraversal> dictionary = new Dictionary<SubjectDescriptor, GraphMembershipTraversal>();
      List<SubjectDescriptor> first = new List<SubjectDescriptor>(groupDescriptors);
      foreach (KeyValuePair<SubjectDescriptor, bool?> keyValuePair in (IEnumerable<KeyValuePair<SubjectDescriptor, bool?>>) this.AreSubjectsToTraverseActive(context, (IEnumerable<SubjectDescriptor>) first))
      {
        bool? nullable = keyValuePair.Value;
        if (!nullable.HasValue)
        {
          IVssRequestContext requestContext = context;
          SubjectDescriptor key1 = keyValuePair.Key;
          string format = GraphResources.GraphSubjectNotFound((object) key1.ToString());
          object[] objArray = Array.Empty<object>();
          requestContext.TraceAlways(15280204, TraceLevel.Verbose, "Graph", nameof (PlatformGraphMembershipTraversalService), format, objArray);
          SubjectDescriptor key2 = keyValuePair.Key;
          key1 = keyValuePair.Key;
          string traversalIncompletenessReason = GraphResources.GraphSubjectNotFound((object) key1.ToString());
          GraphMembershipTraversal membershipTraversal = this.ConstructEmptyTraversalResult(key2, false, traversalIncompletenessReason);
          dictionary[keyValuePair.Key] = membershipTraversal;
        }
        else
        {
          nullable = keyValuePair.Value;
          if (!nullable.Value && (PlatformGraphMembershipTraversalService.IsVstsGroup(context, keyValuePair.Key) || PlatformGraphMembershipTraversalService.IsAadGroupDeleted(context, keyValuePair.Key)))
          {
            context.Trace(15280259, TraceLevel.Verbose, "Graph", nameof (PlatformGraphMembershipTraversalService), string.Format("Constructed empty result for descriptor: {0} because the descriptor is an inactive VSD group or is a deleted AAD group.", (object) keyValuePair.Key));
            GraphMembershipTraversal membershipTraversal = this.ConstructEmptyTraversalResult(keyValuePair.Key);
            dictionary[keyValuePair.Key] = membershipTraversal;
          }
        }
      }
      if (dictionary.Count == groupDescriptors.Count<SubjectDescriptor>())
        return (IReadOnlyDictionary<SubjectDescriptor, GraphMembershipTraversal>) dictionary;
      if (!dictionary.Keys.IsNullOrEmpty<SubjectDescriptor>())
        first = first.Except<SubjectDescriptor>((IEnumerable<SubjectDescriptor>) dictionary.Keys).ToList<SubjectDescriptor>();
      foreach (KeyValuePair<SubjectDescriptor, List<SubjectDescriptor>> descendantsByDescriptor in (IEnumerable<KeyValuePair<SubjectDescriptor, List<SubjectDescriptor>>>) context.GetService<IGraphMembershipStore>().GetDescendantsByDescriptors(context, (IEnumerable<SubjectDescriptor>) first))
      {
        SubjectDescriptor key = descendantsByDescriptor.Key;
        List<SubjectDescriptor> source = descendantsByDescriptor.Value;
        List<SubjectDescriptor> localDescendants = source != null ? source.Where<SubjectDescriptor>((Func<SubjectDescriptor, bool>) (x => x != new SubjectDescriptor())).ToList<SubjectDescriptor>() : (List<SubjectDescriptor>) null;
        if (localDescendants == null)
        {
          context.TraceAlways(15280205, TraceLevel.Verbose, "Graph", nameof (PlatformGraphMembershipTraversalService), Resources.FailedToRetrieveLocalDescendants((object) key));
          GraphMembershipTraversal membershipTraversal = this.ConstructEmptyTraversalResult(key, false, Resources.FailedToRetrieveLocalDescendants((object) key));
          dictionary[key] = membershipTraversal;
        }
        else
        {
          context.TraceDataConditionally(15280159, TraceLevel.Verbose, "Graph", nameof (PlatformGraphMembershipTraversalService), string.Format("Find local descendants for SubjectDescriptor: {0}.", (object) key), (Func<object>) (() => (object) new
          {
            localDescendants = localDescendants
          }), nameof (LookupDescendantsTraversalsInternal));
          bool flag = AadIdentityHelper.IsAadGroup(key.ToIdentityDescriptor(context));
          if (!localDescendants.Any<SubjectDescriptor>() && !flag)
          {
            context.Trace(15280252, TraceLevel.Verbose, "Graph", nameof (PlatformGraphMembershipTraversalService), "Construct empty set of traversed descendants because the subject doesn't contain any descendants");
            GraphMembershipTraversal membershipTraversal = this.ConstructEmptyTraversalResult(key);
            dictionary[key] = membershipTraversal;
          }
          else if (localDescendants.Count<SubjectDescriptor>() > this.m_maxDescendantsToTraverse)
          {
            context.Trace(15280253, TraceLevel.Verbose, "Graph", nameof (PlatformGraphMembershipTraversalService), Resources.TraversedDecendantsLimitReached((object) this.m_maxDescendantsToTraverse));
            GraphMembershipTraversal membershipTraversal = this.ConstructTraversalResult(context, key, this.m_maxDescendantsToTraverse, false, Resources.TraversedDecendantsLimitReached((object) this.m_maxDescendantsToTraverse), (IList<SubjectDescriptor>) localDescendants);
            dictionary.Add(key, membershipTraversal);
          }
          else
          {
            HashSet<SubjectDescriptor> subjectDescriptorSet;
            if (flag)
              subjectDescriptorSet = new HashSet<SubjectDescriptor>()
              {
                key
              };
            else
              subjectDescriptorSet = new HashSet<SubjectDescriptor>(localDescendants.Where<SubjectDescriptor>((Func<SubjectDescriptor, bool>) (x => AadIdentityHelper.IsAadGroup(x.ToIdentityDescriptor(context)))));
            if (!context.ExecutionEnvironment.IsHostedDeployment || !subjectDescriptorSet.Any<SubjectDescriptor>())
            {
              context.Trace(15280254, TraceLevel.Verbose, "Graph", nameof (PlatformGraphMembershipTraversalService), "Finished traversing local descendants. There were no remote groups to expand.");
              GraphMembershipTraversal membershipTraversal = this.ConstructTraversalResult(context, key, this.m_maxDescendantsToTraverse, true, (string) null, (IList<SubjectDescriptor>) localDescendants);
              dictionary[key] = membershipTraversal;
            }
            else
            {
              Dictionary<Guid, SubjectDescriptor> traversedRemoteDescriptors;
              bool isTraversalComplete;
              string traversalIncompletenessReason;
              this.TraverseRemoteDescendants(context, (ISet<SubjectDescriptor>) subjectDescriptorSet, this.m_maxRemoteDescendantsToTraverse, out traversedRemoteDescriptors, out isTraversalComplete, out traversalIncompletenessReason);
              if (traversedRemoteDescriptors.IsNullOrEmpty<KeyValuePair<Guid, SubjectDescriptor>>())
              {
                context.Trace(15280255, TraceLevel.Verbose, "Graph", nameof (PlatformGraphMembershipTraversalService), "Found no remote descendants to filter.");
                GraphMembershipTraversal membershipTraversal = this.ConstructTraversalResult(context, key, this.m_maxDescendantsToTraverse, isTraversalComplete, traversalIncompletenessReason, (IList<SubjectDescriptor>) localDescendants);
                dictionary[key] = membershipTraversal;
              }
              else
              {
                IList<SubjectDescriptor> storageKeysDescriptorsInScope;
                if (!this.TryFilterAndResolveRemoteDescendants(context, traversedRemoteDescriptors.Keys.ToHashSet<Guid>(), out storageKeysDescriptorsInScope))
                {
                  if (context.IsFeatureEnabled("VisualStudio.Services.Graph.EnableTraverseDescendantsMegaTenantAlternativeFlow"))
                  {
                    storageKeysDescriptorsInScope = this.GetActiveDescendants(context, traversedRemoteDescriptors);
                  }
                  else
                  {
                    GraphMembershipTraversal membershipTraversal = this.ConstructTraversalResult(context, key, 0, false, Resources.FailedToRetrieveDescendants(), (IList<SubjectDescriptor>) new List<SubjectDescriptor>());
                    dictionary[key] = membershipTraversal;
                    continue;
                  }
                }
                context.TraceDataConditionally(15280260, TraceLevel.Verbose, "Graph", nameof (PlatformGraphMembershipTraversalService), "Find Active RemoteDescendants for SubjectDescriptor: " + string.Join<SubjectDescriptor>(", ", (IEnumerable<SubjectDescriptor>) subjectDescriptorSet.ToArray<SubjectDescriptor>()) + ".", (Func<object>) (() => (object) new
                {
                  localDescendants = localDescendants
                }), nameof (LookupDescendantsTraversalsInternal));
                IList<SubjectDescriptor> list = (IList<SubjectDescriptor>) localDescendants.Concat<SubjectDescriptor>((IEnumerable<SubjectDescriptor>) storageKeysDescriptorsInScope).ToList<SubjectDescriptor>();
                GraphMembershipTraversal membershipTraversal1 = this.ConstructTraversalResult(context, key, this.m_maxRemoteDescendantsToTraverse, isTraversalComplete, traversalIncompletenessReason, list);
                dictionary[key] = membershipTraversal1;
              }
            }
          }
        }
      }
      return (IReadOnlyDictionary<SubjectDescriptor, GraphMembershipTraversal>) dictionary;
    }

    private IList<SubjectDescriptor> GetActiveDescendants(
      IVssRequestContext context,
      Dictionary<Guid, SubjectDescriptor> traversedRemoteDescriptors)
    {
      List<SubjectDescriptor> activeDescendants = new List<SubjectDescriptor>();
      IEnumerable<SubjectDescriptor> source1 = traversedRemoteDescriptors.Values.Distinct<SubjectDescriptor>();
      IVssRequestContext vssRequestContext = context.Elevate();
      IdentityService service = vssRequestContext.GetService<IdentityService>();
      context.TraceSerializedConditionally(15280258, TraceLevel.Verbose, "Graph", nameof (PlatformGraphMembershipTraversalService), "traversedRemoteDescriptors = {0}", (object) traversedRemoteDescriptors);
      foreach (IList<SubjectDescriptor> source2 in source1.Batch<SubjectDescriptor>(this.m_readIdentitiesBatchSize))
      {
        foreach (Microsoft.VisualStudio.Services.Identity.Identity readIdentity in (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) service.ReadIdentities(vssRequestContext, (IList<SubjectDescriptor>) source2.ToList<SubjectDescriptor>(), QueryMembership.None, (IEnumerable<string>) null))
        {
          if (readIdentity != null && readIdentity.IsActive)
            activeDescendants.Add(readIdentity.SubjectDescriptor);
        }
      }
      return (IList<SubjectDescriptor>) activeDescendants;
    }

    private GraphMembershipTraversal ConstructTraversalResult(
      IVssRequestContext context,
      SubjectDescriptor subjectDescriptor,
      int maxDescendantsToReturnLimit,
      bool isTraversalComplete,
      string traversalIncompletenessReason,
      IList<SubjectDescriptor> traversedDescendantsDescriptors)
    {
      traversedDescendantsDescriptors = (IList<SubjectDescriptor>) traversedDescendantsDescriptors.Distinct<SubjectDescriptor>().ToList<SubjectDescriptor>();
      if (traversedDescendantsDescriptors.Count > maxDescendantsToReturnLimit)
      {
        context.Trace(15280256, TraceLevel.Verbose, "Graph", nameof (PlatformGraphMembershipTraversalService), Resources.TraversedDecendantsLimitReached((object) this.m_maxDescendantsToTraverse));
        isTraversalComplete = false;
        traversalIncompletenessReason = Resources.TraversedDecendantsLimitReached((object) this.m_maxDescendantsToTraverse);
        traversedDescendantsDescriptors = (IList<SubjectDescriptor>) traversedDescendantsDescriptors.OrderBy<SubjectDescriptor, SubjectDescriptor>((Func<SubjectDescriptor, SubjectDescriptor>) (g => g), (IComparer<SubjectDescriptor>) SubjectDescriptorComparer.Instance).Take<SubjectDescriptor>(this.m_maxDescendantsToTraverse).ToList<SubjectDescriptor>();
      }
      context.TraceDataConditionally(15280257, TraceLevel.Verbose, "Graph", nameof (PlatformGraphMembershipTraversalService), "Outputting the following traversed descendants", (Func<object>) (() => (object) new
      {
        subjectDescriptor = subjectDescriptor,
        isTraversalComplete = isTraversalComplete,
        traversalIncompletenessReason = traversalIncompletenessReason,
        traversedDescendantsDescriptors = traversedDescendantsDescriptors
      }), nameof (ConstructTraversalResult));
      return new GraphMembershipTraversal()
      {
        SubjectDescriptor = subjectDescriptor,
        IsComplete = isTraversalComplete,
        IncompletenessReason = traversalIncompletenessReason,
        TraversedSubjects = (IEnumerable<SubjectDescriptor>) traversedDescendantsDescriptors,
        TraversedSubjectIds = (IEnumerable<Guid>) this.GetIdentityLocalIds(context, traversedDescendantsDescriptors)
      };
    }

    private static bool IsVstsGroup(IVssRequestContext context, SubjectDescriptor subjectDescriptor)
    {
      Microsoft.VisualStudio.Services.Identity.Identity identity = GraphSubjectHelper.FetchSingleIdentityByDescriptor(context, subjectDescriptor);
      return AadIdentityHelper.IsTfsGroup(identity.Descriptor) && !AadIdentityHelper.IsAadGroup(identity.Descriptor);
    }

    private static bool IsAadGroupDeleted(
      IVssRequestContext context,
      SubjectDescriptor subjectDescriptor)
    {
      return !AadIdentityHelper.IsAadGroupNotDeleted((IReadOnlyVssIdentity) GraphSubjectHelper.FetchSingleIdentityByDescriptor(context, subjectDescriptor));
    }

    private IDictionary<SubjectDescriptor, bool?> AreSubjectsToTraverseActive(
      IVssRequestContext context,
      IEnumerable<SubjectDescriptor> subjectDescriptors)
    {
      return context.GetService<IGraphMembershipStore>().AreActive(context, subjectDescriptors);
    }

    private GraphMembershipTraversal ConstructEmptyTraversalResult(
      SubjectDescriptor subjectDescriptor,
      bool isTraversalComplete = true,
      string traversalIncompletenessReason = null)
    {
      return new GraphMembershipTraversal()
      {
        SubjectDescriptor = subjectDescriptor,
        IsComplete = isTraversalComplete,
        IncompletenessReason = traversalIncompletenessReason,
        TraversedSubjects = Enumerable.Empty<SubjectDescriptor>(),
        TraversedSubjectIds = Enumerable.Empty<Guid>()
      };
    }

    private void TraverseRemoteDescendants(
      IVssRequestContext context,
      ISet<SubjectDescriptor> remoteDescendantsToTraverse,
      int maxRemoteDescendantsToTraverse,
      out Dictionary<Guid, SubjectDescriptor> traversedRemoteDescriptors,
      out bool isTraversalComplete,
      out string traversalIncompletenessReason)
    {
      context.Trace(15280450, TraceLevel.Verbose, "Graph", nameof (PlatformGraphMembershipTraversalService), "Traversing remote descendants");
      traversedRemoteDescriptors = new Dictionary<Guid, SubjectDescriptor>();
      IRemoteGraphMembershipTraversalExtension traversalExtension = this.GetTraversalExtension(context);
      int num = 0;
      foreach (SubjectDescriptor subjectDescriptor in (IEnumerable<SubjectDescriptor>) remoteDescendantsToTraverse)
      {
        RemoteGraphMembershipTraversal membershipTraversal = traversalExtension.TraverseDescendants(context, subjectDescriptor);
        context.TraceSerializedConditionally(15280451, TraceLevel.Verbose, "Graph", nameof (PlatformGraphMembershipTraversalService), "RemoteDescendantsToTraverse = {0}, maxRemoteDescendantsToTraverse = {1}, TraversedRemoteDescriptors = {2}", (object) remoteDescendantsToTraverse, (object) maxRemoteDescendantsToTraverse, (object) membershipTraversal.TraversedRemoteIdToDescriptorDict);
        ++num;
        foreach (KeyValuePair<Guid, SubjectDescriptor> keyValuePair in (IEnumerable<KeyValuePair<Guid, SubjectDescriptor>>) membershipTraversal.TraversedRemoteIdToDescriptorDict)
        {
          if (!traversedRemoteDescriptors.ContainsKey(keyValuePair.Key))
            traversedRemoteDescriptors.Add(keyValuePair.Key, keyValuePair.Value);
        }
        if (traversedRemoteDescriptors.Count > maxRemoteDescendantsToTraverse)
        {
          isTraversalComplete = false;
          traversalIncompletenessReason = Resources.TraversedDecendantsLimitReached((object) maxRemoteDescendantsToTraverse);
          traversedRemoteDescriptors = traversedRemoteDescriptors.OrderBy<KeyValuePair<Guid, SubjectDescriptor>, Guid>((Func<KeyValuePair<Guid, SubjectDescriptor>, Guid>) (x => x.Key)).Take<KeyValuePair<Guid, SubjectDescriptor>>(maxRemoteDescendantsToTraverse).ToDictionary<KeyValuePair<Guid, SubjectDescriptor>, Guid, SubjectDescriptor>((Func<KeyValuePair<Guid, SubjectDescriptor>, Guid>) (pair => pair.Key), (Func<KeyValuePair<Guid, SubjectDescriptor>, SubjectDescriptor>) (pair => pair.Value));
          context.Trace(15280452, TraceLevel.Verbose, "Graph", nameof (PlatformGraphMembershipTraversalService), Resources.TraversedDecendantsLimitReached((object) maxRemoteDescendantsToTraverse));
          return;
        }
        if (!membershipTraversal.IsComplete)
        {
          isTraversalComplete = false;
          traversalIncompletenessReason = membershipTraversal.IncompletenessReason;
          traversedRemoteDescriptors = traversedRemoteDescriptors.OrderBy<KeyValuePair<Guid, SubjectDescriptor>, Guid>((Func<KeyValuePair<Guid, SubjectDescriptor>, Guid>) (key => key.Key)).ToDictionary<KeyValuePair<Guid, SubjectDescriptor>, Guid, SubjectDescriptor>((Func<KeyValuePair<Guid, SubjectDescriptor>, Guid>) (pair => pair.Key), (Func<KeyValuePair<Guid, SubjectDescriptor>, SubjectDescriptor>) (pair => pair.Value));
          context.Trace(15280453, TraceLevel.Verbose, "Graph", nameof (PlatformGraphMembershipTraversalService), traversalIncompletenessReason);
          return;
        }
        if (num >= this.m_maxAadGroupExpansions)
        {
          isTraversalComplete = false;
          traversalIncompletenessReason = Resources.AadGroupExpansionLimitReached();
          traversedRemoteDescriptors = traversedRemoteDescriptors.OrderBy<KeyValuePair<Guid, SubjectDescriptor>, Guid>((Func<KeyValuePair<Guid, SubjectDescriptor>, Guid>) (key => key.Key)).ToDictionary<KeyValuePair<Guid, SubjectDescriptor>, Guid, SubjectDescriptor>((Func<KeyValuePair<Guid, SubjectDescriptor>, Guid>) (pair => pair.Key), (Func<KeyValuePair<Guid, SubjectDescriptor>, SubjectDescriptor>) (pair => pair.Value));
          context.Trace(15280454, TraceLevel.Verbose, "Graph", nameof (PlatformGraphMembershipTraversalService), Resources.AadGroupExpansionLimitReached());
          return;
        }
      }
      isTraversalComplete = true;
      traversalIncompletenessReason = (string) null;
      traversedRemoteDescriptors = traversedRemoteDescriptors.OrderBy<KeyValuePair<Guid, SubjectDescriptor>, Guid>((Func<KeyValuePair<Guid, SubjectDescriptor>, Guid>) (key => key.Key)).ToDictionary<KeyValuePair<Guid, SubjectDescriptor>, Guid, SubjectDescriptor>((Func<KeyValuePair<Guid, SubjectDescriptor>, Guid>) (pair => pair.Key), (Func<KeyValuePair<Guid, SubjectDescriptor>, SubjectDescriptor>) (pair => pair.Value));
      context.Trace(15280455, TraceLevel.Verbose, "Graph", nameof (PlatformGraphMembershipTraversalService), "Finished traversing remote descendants");
    }

    private IRemoteGraphMembershipTraversalExtension GetTraversalExtension(
      IVssRequestContext context)
    {
      using (IDisposableReadOnlyList<IRemoteGraphMembershipTraversalExtension> extensions = context.GetExtensions<IRemoteGraphMembershipTraversalExtension>())
      {
        if (extensions.IsNullOrEmpty<IRemoteGraphMembershipTraversalExtension>())
        {
          context.Trace(15280456, TraceLevel.Error, "Graph", nameof (PlatformGraphMembershipTraversalService), "Could not find a traversal extension");
          throw new RemoteDirectoryTraversalExtensionNotFoundGraphException(Resources.AadTraversalExtensionNotFound());
        }
        if (extensions.Count > 1)
        {
          context.Trace(15280457, TraceLevel.Error, "Graph", nameof (PlatformGraphMembershipTraversalService), "Found multiple traversal extensions");
          throw new MultipleTraversalExtensionsFoundGraphException(Resources.MultipleAadTraversalExtensionsFound());
        }
        IRemoteGraphMembershipTraversalExtension traversalExtension = extensions.Single<IRemoteGraphMembershipTraversalExtension>();
        return traversalExtension.RemoteDirectoryName == RemoteDirectoryName.AzureActiveDirectory ? traversalExtension : throw new UnsupportedRemoteDirectoryGraphException(Resources.UnsupportedRemoteDirectoryTraversal());
      }
    }

    private bool TryFilterAndResolveRemoteDescendants(
      IVssRequestContext context,
      HashSet<Guid> traversedObjectIds,
      out IList<SubjectDescriptor> storageKeysDescriptorsInScope)
    {
      context.CheckProjectCollectionRequestContext();
      context.TraceDataConditionally(15280550, TraceLevel.Verbose, "Graph", nameof (PlatformGraphMembershipTraversalService), "Starting to filter traversed object ids", (Func<object>) (() => (object) new
      {
        traversedObjectIds = traversedObjectIds
      }), nameof (TryFilterAndResolveRemoteDescendants));
      if (this.TryFilterAndResolveRemoteDescendantsUsingCache(context, traversedObjectIds, out storageKeysDescriptorsInScope))
        return true;
      if (!this.TryPopulateGraphMembershipTraversalCache(context))
        return false;
      if (this.TryFilterAndResolveRemoteDescendantsUsingCache(context, traversedObjectIds, out storageKeysDescriptorsInScope))
        return true;
      throw new FailedToRetrieveStorageKeysInScopeGraphException(Resources.FailedToRetrieveStorageKeysInScope((object) context.ServiceHost.InstanceId));
    }

    private bool TryPopulateGraphMembershipTraversalCache(IVssRequestContext context)
    {
      if (context.GetService<IGraphMembershipStore>().IsMegaTenant(context))
      {
        context.Trace(15280560, TraceLevel.Info, "Graph", nameof (PlatformGraphMembershipTraversalService), "PublishGraphMembershipTraversalCacheExpiryEvent for current scope since it is a mega tenant");
        PlatformGraphMembershipTraversalService.PublishGraphMembershipTraversalCacheWarmupEvent(context, context.ServiceHost.InstanceId);
        return false;
      }
      PlatformGraphMembershipTraversalService.PopulateGraphMembershipTraversalCache(context);
      return true;
    }

    private bool TryFilterAndResolveRemoteDescendantsUsingCache(
      IVssRequestContext context,
      HashSet<Guid> traversedObjectIds,
      out IList<SubjectDescriptor> descriptorsInScope)
    {
      descriptorsInScope = (IList<SubjectDescriptor>) new List<SubjectDescriptor>();
      IVssRequestContext vssRequestContext = context.To(TeamFoundationHostType.Application);
      IGraphMembershipTraversalCache service = vssRequestContext.GetService<IGraphMembershipTraversalCache>();
      Guid scopeId = context.ServiceHost.InstanceId;
      IVssRequestContext requestContext = vssRequestContext;
      Guid key = scopeId;
      IDictionary<Guid, SubjectDescriptor> cacheEntries;
      ref IDictionary<Guid, SubjectDescriptor> local = ref cacheEntries;
      if (service.TryGetValue(requestContext, key, out local) && !cacheEntries.IsNullOrEmpty<KeyValuePair<Guid, SubjectDescriptor>>())
      {
        vssRequestContext.TraceDataConditionally(15280551, TraceLevel.Verbose, "Graph", nameof (PlatformGraphMembershipTraversalService), "Retrieved from cache", (Func<object>) (() => (object) new
        {
          cacheEntries = cacheEntries
        }), nameof (TryFilterAndResolveRemoteDescendantsUsingCache));
        foreach (Guid traversedObjectId in traversedObjectIds)
        {
          SubjectDescriptor subjectDescriptor;
          if (cacheEntries.TryGetValue(traversedObjectId, out subjectDescriptor))
            descriptorsInScope.Add(subjectDescriptor);
        }
        return true;
      }
      vssRequestContext.TraceDataConditionally(15280552, TraceLevel.Verbose, "Graph", nameof (PlatformGraphMembershipTraversalService), "Cache miss", (Func<object>) (() => (object) new
      {
        scopeId = scopeId
      }), nameof (TryFilterAndResolveRemoteDescendantsUsingCache));
      return false;
    }

    private static void PopulateGraphMembershipTraversalCache(IVssRequestContext collectionContext)
    {
      collectionContext.CheckProjectCollectionRequestContext();
      IEnumerable<Guid> organizationStorageKeys = collectionContext.GetService<IGraphMembershipStore>().GetOrganizationStorageKeysForMembersInScope(collectionContext);
      if (organizationStorageKeys.IsNullOrEmpty<Guid>())
        throw new FailedToRetrieveStorageKeysInScopeGraphException(Resources.FailedToRetrieveStorageKeysInScope((object) collectionContext.ServiceHost.InstanceId));
      collectionContext.TraceConditionally(15280155, TraceLevel.Verbose, "Graph", nameof (PlatformGraphMembershipTraversalService), (Func<string>) (() => "Storage keys in scope are " + organizationStorageKeys.Serialize<IEnumerable<Guid>>()));
      IDictionary<Guid, SubjectDescriptor> dictionary = (IDictionary<Guid, SubjectDescriptor>) new Dictionary<Guid, SubjectDescriptor>();
      IVssRequestContext vssRequestContext1 = collectionContext.To(TeamFoundationHostType.Application);
      Guid[] array = organizationStorageKeys.ToArray<Guid>();
      IVssRequestContext vssRequestContext2 = collectionContext.Elevate();
      IList<Microsoft.VisualStudio.Services.Identity.Identity> identities = vssRequestContext2.GetService<IdentityService>().ReadIdentities(vssRequestContext2, (IList<Guid>) array, QueryMembership.None, (IEnumerable<string>) null, true);
      collectionContext.TraceConditionally(15280156, TraceLevel.Verbose, "Graph", nameof (PlatformGraphMembershipTraversalService), (Func<string>) (() => "Identities found for given storage keys are " + identities.Serialize<IList<Microsoft.VisualStudio.Services.Identity.Identity>>()));
      GraphMembershipHelper.CheckForNullIdentities<Guid>(collectionContext, identities, (IList<Guid>) array, false);
      foreach (Microsoft.VisualStudio.Services.Identity.Identity identity in identities.Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (x => x != null)))
      {
        try
        {
          if (!AadIdentityHelper.IsAadUser((IReadOnlyVssIdentity) identity))
          {
            if (!AadIdentityHelper.IsAadGroup(identity.Descriptor))
              continue;
          }
          if (identity.GetAadObjectId() == Guid.Empty)
            collectionContext.TraceAlways(15280562, TraceLevel.Warning, "Graph", nameof (PlatformGraphMembershipTraversalService), string.Format("Failed to get the remote object Id of identity: {0}.", (object) identity));
          else
            dictionary[identity.GetAadObjectId()] = identity.SubjectDescriptor;
        }
        catch (Exception ex)
        {
          collectionContext.TraceAlways(15280568, TraceLevel.Verbose, "Graph", nameof (PlatformGraphMembershipTraversalService), "Failed to get the remote object Id of identity: " + identity.Serialize<Microsoft.VisualStudio.Services.Identity.Identity>() + ".");
          throw;
        }
      }
      vssRequestContext1.GetService<IGraphMembershipTraversalCache>().Set(vssRequestContext1, collectionContext.ServiceHost.InstanceId, dictionary);
    }

    private void WarmUpGraphMembershipTraversalCacheForMegaTenanat(IVssRequestContext context)
    {
      if (!context.GetService<IGraphMembershipStore>().IsMegaTenant(context))
        return;
      context.Trace(15280566, TraceLevel.Info, "Graph", nameof (PlatformGraphMembershipTraversalService), "PublishGraphMembershipTraversalCacheWarmupEvent for current scope since it is a mega tenant");
      PlatformGraphMembershipTraversalService.PublishGraphMembershipTraversalCacheWarmupEvent(context, context.ServiceHost.InstanceId);
    }

    private void OnRegistryChanged(
      IVssRequestContext context,
      RegistryEntryCollection changedEntries)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(context, nameof (context));
      this.UpdateSettings(context);
    }

    private void UpdateSettings(IVssRequestContext context)
    {
      IVssRequestContext vssRequestContext = context.To(TeamFoundationHostType.Deployment);
      IVssRegistryService service = vssRequestContext.GetService<IVssRegistryService>();
      this.m_maxSubjectsToTraverseInBatch = service.GetValue<int>(vssRequestContext, (RegistryQuery) "/Service/Sps/Graph/TraverseDescendants/MaxSubjectsToTraverseInBatch", 25);
      this.m_maxAadGroupExpansions = service.GetValue<int>(vssRequestContext, (RegistryQuery) "/Service/Sps/Graph/TraverseDescendants/MaxRemoteGroupExpansions", 10);
      this.m_maxRemoteDescendantsToTraverse = service.GetValue<int>(vssRequestContext, (RegistryQuery) "/Service/Sps/Graph/TraverseDescendants/MaxRemoteDescendantsToTraverse", 10000);
      this.m_maxDescendantsToTraverse = service.GetValue<int>(vssRequestContext, (RegistryQuery) "/Service/Sps/Graph/TraverseDescendants/MaxDescendantsToTraverse", 1000);
      this.m_readIdentitiesBatchSize = service.GetValue<int>(vssRequestContext, (RegistryQuery) "/Service/Sps/Graph/TraverseDescendants/ReadIdentitiesBatchSize", 10000);
    }

    private static void PublishGraphMembershipTraversalCacheWarmupEvent(
      IVssRequestContext context,
      Guid scopeId)
    {
      GraphMembershipTraversalCacheWarmupData taskArgs = new GraphMembershipTraversalCacheWarmupData()
      {
        HostId = context.ServiceHost.InstanceId,
        ScopeId = scopeId
      };
      try
      {
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        context.GetService<LongRunningTaskService>().ScheduleLongRunningTask(context, context.ServiceHost.InstanceId, PlatformGraphMembershipTraversalService.\u003C\u003EO.\u003C0\u003E__WarmUpGraphMembershipTraversalCache ?? (PlatformGraphMembershipTraversalService.\u003C\u003EO.\u003C0\u003E__WarmUpGraphMembershipTraversalCache = new TeamFoundationTaskCallback(PlatformGraphMembershipTraversalService.WarmUpGraphMembershipTraversalCache)), (object) taskArgs);
      }
      catch (TaskScheduleExistsException ex)
      {
        context.TraceException(15280567, TraceLevel.Info, "Graph", nameof (PlatformGraphMembershipTraversalService), (Exception) ex);
      }
    }

    private static void WarmUpGraphMembershipTraversalCache(
      IVssRequestContext requestContext,
      object taskArgs)
    {
      ArgumentUtility.CheckForNull<GraphMembershipTraversalCacheWarmupData>(taskArgs as GraphMembershipTraversalCacheWarmupData, "graphMembershipTraversalCacheWarmupData");
      PlatformGraphMembershipTraversalService.PopulateGraphMembershipTraversalCache(requestContext);
    }

    private IList<Guid> GetIdentityLocalIds(
      IVssRequestContext context,
      IList<SubjectDescriptor> subjectDescriptors)
    {
      List<Guid> identityLocalIds = new List<Guid>();
      IVssRequestContext vssRequestContext = context.Elevate();
      IdentityService service = vssRequestContext.GetService<IdentityService>();
      foreach (IList<SubjectDescriptor> subjectDescriptorList in subjectDescriptors.Batch<SubjectDescriptor>(this.m_readIdentitiesBatchSize))
      {
        IList<Microsoft.VisualStudio.Services.Identity.Identity> identityList = service.ReadIdentities(vssRequestContext, (IList<SubjectDescriptor>) subjectDescriptorList.ToList<SubjectDescriptor>(), QueryMembership.None, (IEnumerable<string>) null);
        GraphMembershipHelper.CheckForNullIdentities<SubjectDescriptor>(context, identityList, subjectDescriptorList);
        identityLocalIds.AddRange(identityList.Select<Microsoft.VisualStudio.Services.Identity.Identity, Guid>((Func<Microsoft.VisualStudio.Services.Identity.Identity, Guid>) (x => x.Id)));
      }
      return (IList<Guid>) identityLocalIds;
    }
  }
}
