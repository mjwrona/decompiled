// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Graph.GraphMembersController
// Assembly: Microsoft.VisualStudio.Services.Graph, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 00390AA0-D8BB-45EB-AEF5-70DC8BFC765D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Graph.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Graph.Client;
using Microsoft.VisualStudio.Services.WebApi;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Graph
{
  [ControllerApiVersion(3.2)]
  [ClientInternalUseOnly(true)]
  [RestrictInternalGraphEndpoints]
  [VersionedApiControllerCustomName(Area = "Graph", ResourceName = "Members")]
  public class GraphMembersController : GraphControllerBase
  {
    [HttpGet]
    [TraceFilter(6307601, 6307602)]
    [ClientLocationId("B9AF63A7-5DB6-4AF8-AAE7-387F775EA9C6")]
    public GraphMember GetMemberByDescriptor([ClientParameterType(typeof (string), false)] SubjectDescriptor memberDescriptor)
    {
      Microsoft.VisualStudio.Services.Identity.Identity identity = GraphSubjectHelper.FetchSingleIdentityByDescriptor(this.TfsRequestContext, memberDescriptor);
      if (!((identity != null ? identity.ToGraphSubjectClient(this.TfsRequestContext) : (GraphSubject) null) is GraphMember graphSubjectClient))
        throw new GraphSubjectNotFoundException(memberDescriptor);
      graphSubjectClient.FillSerializeInternalsField(this.TfsRequestContext);
      return graphSubjectClient;
    }
  }
}
