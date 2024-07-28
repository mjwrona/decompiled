// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Graph.GraphMembershipStatesController
// Assembly: Microsoft.VisualStudio.Services.Graph, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 00390AA0-D8BB-45EB-AEF5-70DC8BFC765D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Graph.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Graph.Client;
using Microsoft.VisualStudio.Services.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Graph
{
  [ControllerApiVersion(3.2)]
  [VersionedApiControllerCustomName(Area = "Graph", ResourceName = "MembershipStates")]
  public class GraphMembershipStatesController : GraphControllerBase
  {
    [HttpGet]
    [TraceFilter(6307140, 6307149)]
    [ClientExample("GetMembershipStateBySubjectDescriptor.json", null, null, null)]
    public GraphMembershipState GetMembershipState([ClientParameterType(typeof (string), false)] SubjectDescriptor subjectDescriptor) => GraphResultExtensions.GetGraphMembershipState(this.TfsRequestContext, subjectDescriptor, (this.TfsRequestContext.GetService<IdentityService>().ReadIdentities(this.TfsRequestContext, (IList<SubjectDescriptor>) new SubjectDescriptor[1]
    {
      subjectDescriptor
    }, QueryMembership.None, (IEnumerable<string>) null).Single<Microsoft.VisualStudio.Services.Identity.Identity>()?.IsActive ?? throw new GraphSubjectNotFoundException(subjectDescriptor)).Value) ?? throw new GraphSubjectNotFoundException(subjectDescriptor);
  }
}
