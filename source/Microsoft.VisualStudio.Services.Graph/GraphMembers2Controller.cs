// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Graph.GraphMembers2Controller
// Assembly: Microsoft.VisualStudio.Services.Graph, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 00390AA0-D8BB-45EB-AEF5-70DC8BFC765D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Graph.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Graph.Client;
using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Graph
{
  [ControllerApiVersion(7.1)]
  [ClientInternalUseOnly(true)]
  [VersionedApiControllerCustomName(Area = "Graph", ResourceName = "Members", ResourceVersion = 2)]
  public class GraphMembers2Controller : GraphMembersControllerBase<AadGraphMember>
  {
    private readonly IAadServicePrincipalConfigurationHelper aadServicePrincipalConfigurationHelper;

    protected override IReadOnlyCollection<string> AllowedSubjectTypes => GraphMembersControllerBase<AadGraphMember>.MemberSubjectTypes;

    protected override string Layer => nameof (GraphMembers2Controller);

    public GraphMembers2Controller()
      : this(AadServicePrincipalConfigurationHelper.Instance)
    {
    }

    public GraphMembers2Controller(
      IAadServicePrincipalConfigurationHelper aadServicePrincipalConfigurationHelper)
    {
      this.aadServicePrincipalConfigurationHelper = aadServicePrincipalConfigurationHelper;
    }

    protected override void ValidateSubjectDescriptorType(SubjectDescriptor subjectDescriptor)
    {
    }

    [HttpGet]
    [ClientResponseType(typeof (PagedGraphMembers), null, null)]
    [TraceFilter(6307680, 6307689)]
    [ClientLocationId("8B9ECDB2-B752-485A-8418-CC15CF12EE07")]
    [ClientExample("GetAllMembers.json", null, null, null)]
    public HttpResponseMessage ListMembers(
      [FromUri] string continuationToken = null,
      [ClientQueryParameter, ClientParameterAsIEnumerable(typeof (string), ',')] string subjectTypes = null,
      [ClientQueryParameter, ClientParameterAsIEnumerable(typeof (string), ',')] string subjectKinds = null,
      [ClientQueryParameter, ClientParameterAsIEnumerable(typeof (string), ',')] string metaTypes = null,
      [ClientParameterType(typeof (string), false), FromUri] SubjectDescriptor? scopeDescriptor = null)
    {
      return this.ListMembersInternal(subjectKinds, subjectTypes, continuationToken, scopeDescriptor, metaTypes);
    }

    [HttpGet]
    [TraceFilter(6307601, 6307602)]
    [ClientLocationId("B9AF63A7-5DB6-4AF8-AAE7-387F775EA9C6")]
    public GraphMember GetMemberByDescriptor([ClientParameterType(typeof (string), false)] SubjectDescriptor memberDescriptor)
    {
      GraphMember graphSubject;
      if (this.aadServicePrincipalConfigurationHelper.IsAllowedSubjectTypesEnabled(this.TfsRequestContext))
      {
        graphSubject = (GraphMember) this.GetMember(this.TfsRequestContext, memberDescriptor);
      }
      else
      {
        Microsoft.VisualStudio.Services.Identity.Identity identity = GraphSubjectHelper.FetchSingleIdentityByDescriptor(this.TfsRequestContext, memberDescriptor);
        graphSubject = (identity != null ? identity.ToGraphSubjectClient(this.TfsRequestContext) : (GraphSubject) null) as GraphMember;
      }
      if (graphSubject == null)
        throw new GraphSubjectNotFoundException(memberDescriptor);
      graphSubject.FillSerializeInternalsField(this.TfsRequestContext);
      return graphSubject;
    }
  }
}
