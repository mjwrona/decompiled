// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Graph.GraphSubjectsController
// Assembly: Microsoft.VisualStudio.Services.Graph, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 00390AA0-D8BB-45EB-AEF5-70DC8BFC765D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Graph.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Graph.Client;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.WebApi;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Graph
{
  [ControllerApiVersion(3.2)]
  [ClientInternalUseOnly(true)]
  [RestrictInternalGraphEndpoints]
  [VersionedApiControllerCustomName(Area = "Graph", ResourceName = "Subjects")]
  public class GraphSubjectsController : GraphControllerBase
  {
    [HttpGet]
    [TraceFilter(6307607, 6307608)]
    public GraphSubject GetSubject([ClientParameterType(typeof (string), false)] SubjectDescriptor subjectDescriptor)
    {
      GraphSubject graphSubjectClient;
      if (subjectDescriptor.IsGroupScopeType())
      {
        this.TfsRequestContext.GetService<IdentityService>();
        IdentityScope identityScope = GraphSubjectHelper.FetchIdentityScope(this.TfsRequestContext, subjectDescriptor);
        graphSubjectClient = identityScope != null ? identityScope.ToGraphSubjectClient(this.TfsRequestContext) : (GraphSubject) null;
      }
      else
      {
        Microsoft.VisualStudio.Services.Identity.Identity identity = GraphSubjectHelper.FetchSingleIdentityByDescriptor(this.TfsRequestContext, subjectDescriptor);
        graphSubjectClient = identity != null ? identity.ToGraphSubjectClient(this.TfsRequestContext) : (GraphSubject) null;
      }
      if (graphSubjectClient == null)
        throw new GraphSubjectNotFoundException(subjectDescriptor);
      graphSubjectClient.FillSerializeInternalsField(this.TfsRequestContext);
      return graphSubjectClient;
    }
  }
}
