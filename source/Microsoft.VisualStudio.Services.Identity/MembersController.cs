// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.MembersController
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Identity
{
  [ControllerApiVersion(1.0)]
  [VersionedApiControllerCustomName(Area = "IMS", ResourceName = "Members")]
  public class MembersController : IdentitiesControllerBase
  {
    private const string s_area = "Identity";
    private const string s_layer = "MembersController";

    [HttpGet]
    [ClientLocationId("8BA35978-138E-41F8-8963-7B1EA2C5F775")]
    public IQueryable<IdentityDescriptor> ReadMembers(
      string containerId,
      QueryMembership queryMembership = QueryMembership.Direct)
    {
      this.TfsRequestContext.GetService<IdentityService>();
      return MembersController.ReadIdentity(this.TfsRequestContext, containerId, queryMembership).Members.AsQueryable<IdentityDescriptor>();
    }

    [HttpGet]
    [ClientLocationId("8BA35978-138E-41F8-8963-7B1EA2C5F775")]
    [ClientResponseType(typeof (IdentityDescriptor), null, null)]
    public HttpResponseMessage ReadMember(
      string containerId,
      string memberId,
      QueryMembership queryMembership = QueryMembership.Direct)
    {
      this.TfsRequestContext.GetService<IdentityService>();
      Microsoft.VisualStudio.Services.Identity.Identity identity = MembersController.ReadIdentity(this.TfsRequestContext, containerId, queryMembership);
      Microsoft.VisualStudio.Services.Identity.Identity member = MembersController.ReadIdentity(this.TfsRequestContext, memberId, QueryMembership.None);
      return this.Request.CreateResponse<IdentityDescriptor>(HttpStatusCode.OK, identity.Members.Where<IdentityDescriptor>((Func<IdentityDescriptor, bool>) (memberDescriptor => IdentityDescriptorComparer.Instance.Equals(member.Descriptor, memberDescriptor))).FirstOrDefault<IdentityDescriptor>());
    }

    [HttpPut]
    [ClientResponseType(typeof (bool), null, null)]
    public HttpResponseMessage AddMember(string containerId, string memberId)
    {
      IdentityService service = this.TfsRequestContext.GetService<IdentityService>();
      Microsoft.VisualStudio.Services.Identity.Identity identity1 = MembersController.ReadIdentity(this.TfsRequestContext.Elevate(), containerId, QueryMembership.None);
      IdentityDescriptor memberDescriptor;
      if (Guid.TryParse(memberId, out Guid _))
      {
        Microsoft.VisualStudio.Services.Identity.Identity identity2;
        try
        {
          identity2 = MembersController.ReadIdentity(this.TfsRequestContext.Elevate(), memberId, QueryMembership.None);
        }
        catch (IdentityNotFoundException ex)
        {
          if (this.TfsRequestContext.IsDeploymentFallbackIdentityReadAllowed() && !this.TfsRequestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
            identity2 = MembersController.ReadIdentity(this.TfsRequestContext.To(TeamFoundationHostType.Deployment).Elevate(), memberId, QueryMembership.None);
          else if (this.TfsRequestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
            identity2 = MembersController.ReadIdentity(this.TfsRequestContext.To(TeamFoundationHostType.Application), memberId, QueryMembership.None);
          else
            throw;
        }
        memberDescriptor = identity2.Descriptor;
      }
      else
        memberDescriptor = IdentityParser.GetDescriptorFromString(memberId);
      return this.Request.CreateResponse<bool>(HttpStatusCode.OK, service.AddMemberToGroup(this.TfsRequestContext, identity1.Descriptor, memberDescriptor));
    }

    [HttpDelete]
    [ClientResponseType(typeof (bool), null, null)]
    public HttpResponseMessage RemoveMember(string containerId, string memberId)
    {
      IdentityService service = this.TfsRequestContext.GetService<IdentityService>();
      Microsoft.VisualStudio.Services.Identity.Identity identity1 = MembersController.ReadIdentity(this.TfsRequestContext.Elevate(), containerId, QueryMembership.None);
      Microsoft.VisualStudio.Services.Identity.Identity identity2 = MembersController.ReadIdentity(this.TfsRequestContext.Elevate(), memberId, QueryMembership.None);
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      IdentityDescriptor descriptor1 = identity1.Descriptor;
      IdentityDescriptor descriptor2 = identity2.Descriptor;
      return this.Request.CreateResponse<bool>(HttpStatusCode.OK, service.RemoveMemberFromGroup(tfsRequestContext, descriptor1, descriptor2));
    }

    [HttpDelete]
    [ClientResponseType(typeof (bool), null, null)]
    public HttpResponseMessage ForceRemoveMember(
      string containerId,
      string memberId,
      bool forceRemove)
    {
      if (!forceRemove)
        return this.Request.CreateResponse<bool>(HttpStatusCode.Forbidden, false);
      IdentityService service = this.TfsRequestContext.GetService<IdentityService>();
      Microsoft.VisualStudio.Services.Identity.Identity identity1 = MembersController.ReadIdentity(this.TfsRequestContext.Elevate(), containerId, QueryMembership.None);
      Microsoft.VisualStudio.Services.Identity.Identity identity2 = MembersController.ReadIdentity(this.TfsRequestContext.Elevate(), memberId, QueryMembership.None);
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      IdentityDescriptor descriptor1 = identity1.Descriptor;
      IdentityDescriptor descriptor2 = identity2.Descriptor;
      return this.Request.CreateResponse<bool>(HttpStatusCode.OK, service.ForceRemoveMemberFromGroup(tfsRequestContext, descriptor1, descriptor2));
    }

    internal static Microsoft.VisualStudio.Services.Identity.Identity ReadIdentity(
      IVssRequestContext requestContext,
      string identityId,
      QueryMembership queryMembership)
    {
      IdentityService service = requestContext.GetService<IdentityService>();
      Guid identityGuid;
      if (Guid.TryParse(identityId, out identityGuid))
      {
        requestContext.TraceConditionally(IdentityTracing.ConvertToReadIdentityTracePoint(IdentityTracing.ReadIdentityTraceKind.ById, queryMembership, 0, 1), TraceLevel.Verbose, "Identity", nameof (MembersController), (Func<string>) (() => string.Format("MembersController.ReadIdentity where identityId : {0}, queryMembership : {1}", (object) identityGuid, (object) queryMembership)));
        return service.ReadIdentities(requestContext, (IList<Guid>) new Guid[1]
        {
          identityGuid
        }, queryMembership, (IEnumerable<string>) null)[0] ?? throw new IdentityNotFoundException(identityGuid);
      }
      IdentityDescriptor descriptor = IdentityParser.GetDescriptorFromString(identityId);
      requestContext.TraceConditionally(IdentityTracing.ConvertToReadIdentityTracePoint(IdentityTracing.ReadIdentityTraceKind.ByDescriptor, queryMembership, 0, 1), TraceLevel.Verbose, "Identity", nameof (MembersController), (Func<string>) (() => string.Format("MembersController.ReadIdentity where descriptor : {0}, queryMembership : {1}", (object) descriptor, (object) queryMembership)));
      return service.ReadIdentities(requestContext, (IList<IdentityDescriptor>) new IdentityDescriptor[1]
      {
        descriptor
      }, queryMembership, (IEnumerable<string>) null)[0] ?? throw new IdentityNotFoundException(descriptor);
    }

    public override IDictionary<Type, HttpStatusCode> HttpExceptions => (IDictionary<Type, HttpStatusCode>) IdentitiesController.s_httpIdentityServiceExceptions;

    public override string TraceArea => "IdentityService";
  }
}
