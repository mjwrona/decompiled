// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Graph.GraphMemberLookupController
// Assembly: Microsoft.VisualStudio.Services.Graph, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 00390AA0-D8BB-45EB-AEF5-70DC8BFC765D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Graph.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Graph.Client;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Graph
{
  [ControllerApiVersion(3.2)]
  [ClientInternalUseOnly(true)]
  [RestrictInternalGraphEndpoints]
  [VersionedApiControllerCustomName(Area = "Graph", ResourceName = "MemberLookup")]
  public class GraphMemberLookupController : GraphControllerBase
  {
    [HttpPost]
    [TraceFilter(6307511, 6307512)]
    [RequestContentTypeRestriction(AllowJsonPatch = false)]
    public IReadOnlyDictionary<SubjectDescriptor, GraphMember> LookupMembers(
      GraphSubjectLookup memberLookup)
    {
      ArgumentUtility.CheckForNull<GraphSubjectLookup>(memberLookup, nameof (memberLookup));
      ArgumentUtility.CheckForNull<IEnumerable<GraphSubjectLookupKey>>(memberLookup.LookupKeys, "LookupKeys");
      this.TfsRequestContext.TraceDataConditionally(10006076, TraceLevel.Info, this.TraceArea, nameof (GraphMemberLookupController), "Received input parameters", (Func<object>) (() => (object) new
      {
        memberLookup = memberLookup
      }), nameof (LookupMembers));
      int providedCount = memberLookup.LookupKeys.Count<GraphSubjectLookupKey>();
      if (providedCount > 500)
        throw new TooManyRequestedItemsException(providedCount, 500);
      List<SubjectDescriptor> list = memberLookup.LookupKeys.Select<GraphSubjectLookupKey, SubjectDescriptor>((Func<GraphSubjectLookupKey, SubjectDescriptor>) (x => x.Descriptor)).ToList<SubjectDescriptor>();
      Dictionary<SubjectDescriptor, GraphMember> dictionary = this.TfsRequestContext.GetService<IdentityService>().LookupIdentities(this.TfsRequestContext, (IEnumerable<SubjectDescriptor>) list, QueryMembership.None, (IEnumerable<string>) null).ToDictionary<KeyValuePair<SubjectDescriptor, Microsoft.VisualStudio.Services.Identity.Identity>, SubjectDescriptor, GraphMember>((Func<KeyValuePair<SubjectDescriptor, Microsoft.VisualStudio.Services.Identity.Identity>, SubjectDescriptor>) (x => x.Key), (Func<KeyValuePair<SubjectDescriptor, Microsoft.VisualStudio.Services.Identity.Identity>, GraphMember>) (x =>
      {
        Microsoft.VisualStudio.Services.Identity.Identity identity = x.Value;
        return (identity != null ? identity.ToGraphSubjectClient(this.TfsRequestContext) : (GraphSubject) null) as GraphMember;
      }));
      dictionary.Values.ForEach<GraphMember>((Action<GraphMember>) (x => x.FillSerializeInternalsField(this.TfsRequestContext)));
      return (IReadOnlyDictionary<SubjectDescriptor, GraphMember>) dictionary;
    }
  }
}
