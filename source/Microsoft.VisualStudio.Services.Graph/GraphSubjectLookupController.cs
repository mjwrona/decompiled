// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Graph.GraphSubjectLookupController
// Assembly: Microsoft.VisualStudio.Services.Graph, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 00390AA0-D8BB-45EB-AEF5-70DC8BFC765D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Graph.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Graph.Client;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Graph
{
  [ControllerApiVersion(3.2)]
  [VersionedApiControllerCustomName(Area = "Graph", ResourceName = "SubjectLookup")]
  public class GraphSubjectLookupController : GraphControllerBase
  {
    [HttpPost]
    [TraceFilter(6307510, 6307519)]
    [RequestContentTypeRestriction(AllowJsonPatch = false)]
    [ClientExample("LookupSubjects.json", null, null, null)]
    public IReadOnlyDictionary<SubjectDescriptor, GraphSubject> LookupSubjects(
      GraphSubjectLookup subjectLookup)
    {
      ArgumentUtility.CheckForNull<GraphSubjectLookup>(subjectLookup, nameof (subjectLookup));
      ArgumentUtility.CheckForNull<IEnumerable<GraphSubjectLookupKey>>(subjectLookup.LookupKeys, "LookupKeys");
      this.TfsRequestContext.TraceDataConditionally(10006075, TraceLevel.Info, this.TraceArea, nameof (GraphSubjectLookupController), "Received input parameters", (Func<object>) (() => (object) new
      {
        subjectLookup = subjectLookup
      }), nameof (LookupSubjects));
      int providedCount = subjectLookup.LookupKeys.Count<GraphSubjectLookupKey>();
      if (providedCount > 500)
        throw new TooManyRequestedItemsException(providedCount, 500);
      Dictionary<SubjectDescriptor, GraphSubject> dictionary = new Dictionary<SubjectDescriptor, GraphSubject>(subjectLookup.LookupKeys.Count<GraphSubjectLookupKey>());
      foreach (GraphSubjectLookupKey lookupKey in subjectLookup.LookupKeys)
        dictionary[lookupKey.Descriptor] = (GraphSubject) null;
      IdentityService service = this.TfsRequestContext.GetService<IdentityService>();
      PartitionResults<SubjectDescriptor> partitionResults = subjectLookup.LookupKeys.Select<GraphSubjectLookupKey, SubjectDescriptor>((Func<GraphSubjectLookupKey, SubjectDescriptor>) (x => x.Descriptor)).Partition<SubjectDescriptor>((Predicate<SubjectDescriptor>) (x => x.IsGroupScopeType()));
      if (partitionResults.MatchingPartition.Any<SubjectDescriptor>())
      {
        List<GraphScope> graphScopeList = new List<GraphScope>();
        foreach (SubjectDescriptor subjectDescriptor in partitionResults.MatchingPartition)
        {
          IdentityScope scope = GraphSubjectHelper.FetchIdentityScope(this.TfsRequestContext, subjectDescriptor);
          GraphScope graphScopeClient = scope != null ? scope.ToGraphScopeClient(this.TfsRequestContext) : (GraphScope) null;
          if (graphScopeClient != null)
            dictionary[subjectDescriptor] = (GraphSubject) graphScopeClient;
        }
      }
      if (partitionResults.NonMatchingPartition.Any<SubjectDescriptor>())
      {
        foreach (KeyValuePair<SubjectDescriptor, GraphSubject> keyValuePair in service.LookupIdentities(this.TfsRequestContext, (IEnumerable<SubjectDescriptor>) partitionResults.NonMatchingPartition, QueryMembership.None, (IEnumerable<string>) null).ToDictionary<KeyValuePair<SubjectDescriptor, Microsoft.VisualStudio.Services.Identity.Identity>, SubjectDescriptor, GraphSubject>((Func<KeyValuePair<SubjectDescriptor, Microsoft.VisualStudio.Services.Identity.Identity>, SubjectDescriptor>) (x => x.Key), (Func<KeyValuePair<SubjectDescriptor, Microsoft.VisualStudio.Services.Identity.Identity>, GraphSubject>) (x =>
        {
          Microsoft.VisualStudio.Services.Identity.Identity identity = x.Value;
          return identity == null ? (GraphSubject) null : identity.ToGraphSubjectClient(this.TfsRequestContext);
        })))
        {
          if (keyValuePair.Value != null)
            dictionary[keyValuePair.Key] = keyValuePair.Value;
        }
      }
      dictionary.Values.ForEach<GraphSubject>((Action<GraphSubject>) (x => x.FillSerializeInternalsField(this.TfsRequestContext)));
      return (IReadOnlyDictionary<SubjectDescriptor, GraphSubject>) dictionary;
    }
  }
}
