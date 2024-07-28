// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Graph.GraphMembershipsController
// Assembly: Microsoft.VisualStudio.Services.Graph, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 00390AA0-D8BB-45EB-AEF5-70DC8BFC765D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Graph.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Graph.Client;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.WebApi;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Graph
{
  [ControllerApiVersion(3.2)]
  [VersionedApiControllerCustomName(Area = "Graph", ResourceName = "Memberships")]
  public class GraphMembershipsController : GraphControllerBase
  {
    [HttpGet]
    [TraceFilter(6307100, 6307109)]
    [ClientResponseType(typeof (GraphMembership), null, null)]
    [ClientExample("GetMembershipUser.json", "For a User", null, null)]
    [ClientExample("GetMembershipVSTSGroup.json", "For a Group", null, null)]
    public HttpResponseMessage GetMembership(
      [ClientParameterType(typeof (string), false)] SubjectDescriptor subjectDescriptor,
      [ClientParameterType(typeof (string), false)] SubjectDescriptor containerDescriptor)
    {
      ArgumentValidator.CheckDescriptorIsMemberSubjectKind(subjectDescriptor);
      if (containerDescriptor.SubjectType == "Microsoft.VisualStudio.Services.Graph.GraphScope")
        throw new GraphBadRequestException(Resources.GetScopeMembershipUnsupported());
      ArgumentValidator.CheckDescriptorIsGroupSubjectKind(containerDescriptor);
      if (!GraphMembershipsController.IsMember(this.TfsRequestContext, subjectDescriptor, containerDescriptor))
        throw new GraphMembershipNotFoundException(subjectDescriptor, containerDescriptor);
      return this.Request.CreateResponse<GraphMembership>(HttpStatusCode.OK, GraphResultExtensions.GetGraphMembership(this.TfsRequestContext, subjectDescriptor, containerDescriptor) ?? throw new GraphMembershipNotFoundException(subjectDescriptor, containerDescriptor));
    }

    [HttpHead]
    [TraceFilter(6307110, 6307119)]
    [ClientResponseType(typeof (void), null, null)]
    [ClientResponseCode(HttpStatusCode.OK, "The operation succeeded. Membership exists.", false)]
    [ClientResponseCode(HttpStatusCode.NotFound, "Membership does not exist.", true)]
    [ClientExample("CheckMembershipExistenceUser.json", null, null, null)]
    public HttpResponseMessage CheckMembershipExistence(
      [ClientParameterType(typeof (string), false)] SubjectDescriptor subjectDescriptor,
      [ClientParameterType(typeof (string), false)] SubjectDescriptor containerDescriptor)
    {
      ArgumentUtility.CheckForDefault<SubjectDescriptor>(subjectDescriptor, nameof (subjectDescriptor), "Graph");
      ArgumentUtility.CheckForDefault<SubjectDescriptor>(containerDescriptor, nameof (containerDescriptor), "Graph");
      ArgumentValidator.CheckDescriptorIsMemberSubjectKind(subjectDescriptor);
      ArgumentValidator.CheckDescriptorIsGroupSubjectKind(containerDescriptor);
      if (containerDescriptor.SubjectType == "Microsoft.VisualStudio.Services.Graph.GraphScope")
        throw new GraphBadRequestException(Resources.CheckScopeMembershipUnsupported());
      IdentityDescriptor identityDescriptor1 = subjectDescriptor.ToIdentityDescriptor(this.TfsRequestContext);
      IdentityDescriptor identityDescriptor2 = containerDescriptor.ToIdentityDescriptor(this.TfsRequestContext);
      if (identityDescriptor1 == (IdentityDescriptor) null)
        throw new GraphBadRequestException(IdentityResources.IdentityNotFoundWithDescriptor((object) subjectDescriptor.SubjectType, (object) subjectDescriptor.Identifier));
      if (!this.TfsRequestContext.GetService<IdentityService>().IsMember(this.TfsRequestContext, identityDescriptor2, identityDescriptor1))
        return new HttpResponseMessage(HttpStatusCode.NotFound);
      return new HttpResponseMessage()
      {
        StatusCode = HttpStatusCode.OK,
        RequestMessage = this.Request
      };
    }

    [HttpPut]
    [TraceFilter(6307120, 6307129)]
    [ClientResponseType(typeof (GraphMembership), null, null)]
    [ClientExample("CreateMembershipVSTSGroup.json", null, null, null)]
    [RequestContentTypeRestriction(AllowJsonPatch = false)]
    public HttpResponseMessage AddMembership(
      [ClientParameterType(typeof (string), false)] SubjectDescriptor subjectDescriptor,
      [ClientParameterType(typeof (string), false)] SubjectDescriptor containerDescriptor)
    {
      ArgumentValidator.CheckDescriptorIsGroupSubjectKind(containerDescriptor);
      ArgumentValidator.CheckDescriptorIsMemberSubjectKind(subjectDescriptor);
      if (subjectDescriptor.IsImportedIdentityType())
        throw new GraphBadRequestException(GraphResources.InvalidGraphSubjectDescriptor((object) subjectDescriptor));
      this.TfsRequestContext.GetService<IdentityService>().AddMemberToGroup(this.TfsRequestContext, containerDescriptor, subjectDescriptor);
      return this.Request.CreateResponse<GraphMembership>(HttpStatusCode.Created, GraphResultExtensions.CreateGraphMembership(this.TfsRequestContext, subjectDescriptor, containerDescriptor));
    }

    [HttpDelete]
    [TraceFilter(6307130, 6307139)]
    [ClientResponseType(typeof (void), null, null)]
    [ClientExample("DeleteMembershipUser.json", null, null, null)]
    [RequestContentTypeRestriction(AllowJsonPatch = false)]
    public HttpResponseMessage RemoveMembership(
      [ClientParameterType(typeof (string), false)] SubjectDescriptor subjectDescriptor,
      [ClientParameterType(typeof (string), false)] SubjectDescriptor containerDescriptor)
    {
      ArgumentValidator.CheckDescriptorIsGroupSubjectKind(containerDescriptor);
      ArgumentValidator.CheckDescriptorIsMemberSubjectKind(subjectDescriptor);
      this.TfsRequestContext.GetService<IdentityService>().RemoveMemberFromGroup(this.TfsRequestContext, containerDescriptor, subjectDescriptor);
      return new HttpResponseMessage(HttpStatusCode.OK);
    }

    private static bool IsMember(
      IVssRequestContext requestContext,
      SubjectDescriptor subjectDescriptor,
      SubjectDescriptor containerDescriptor)
    {
      return requestContext.GetService<IdentityService>().IsMember(requestContext, containerDescriptor, subjectDescriptor);
    }
  }
}
