// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.MembersController_BackCompatController
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
  [BackCompatJsonFormatter]
  [ControllerApiVersion(0.0)]
  [VersionedApiControllerCustomName(Area = "IMS", ResourceName = "Members")]
  public class MembersController_BackCompatController : IdentitiesControllerBase
  {
    private const string s_area = "Identity";
    private const string s_layer = "MembersController_BackCompatController";
    private static readonly Dictionary<Type, HttpStatusCode> s_httpExceptions = new Dictionary<Type, HttpStatusCode>();

    [HttpGet]
    public IQueryable<IdentityDescriptor> ReadMemberships(
      string containerId,
      QueryMembership queryMembership = QueryMembership.Direct)
    {
      this.TfsRequestContext.GetService<IdentityService>();
      return MembersController_BackCompatController.ReadIdentity(this.TfsRequestContext, containerId, queryMembership).Members.AsQueryable<IdentityDescriptor>();
    }

    [HttpGet]
    public HttpResponseMessage ReadMembership(
      string containerId,
      string memberId,
      QueryMembership queryMembership = QueryMembership.Direct)
    {
      this.TfsRequestContext.GetService<IdentityService>();
      Microsoft.VisualStudio.Services.Identity.Identity identity = MembersController_BackCompatController.ReadIdentity(this.TfsRequestContext, containerId, queryMembership);
      Microsoft.VisualStudio.Services.Identity.Identity member = MembersController_BackCompatController.ReadIdentity(this.TfsRequestContext, memberId, QueryMembership.None);
      return this.Request.CreateResponse<IdentityDescriptor>(HttpStatusCode.OK, identity.Members.Where<IdentityDescriptor>((Func<IdentityDescriptor, bool>) (memberDescriptor => IdentityDescriptorComparer.Instance.Equals(member.Descriptor, memberDescriptor))).FirstOrDefault<IdentityDescriptor>());
    }

    [HttpPut]
    public HttpResponseMessage AddMember(string containerId, string memberId)
    {
      IdentityService service = this.TfsRequestContext.GetService<IdentityService>();
      Microsoft.VisualStudio.Services.Identity.Identity identity1 = MembersController_BackCompatController.ReadIdentity(this.TfsRequestContext.Elevate(), containerId, QueryMembership.None);
      Microsoft.VisualStudio.Services.Identity.Identity identity2 = MembersController_BackCompatController.ReadIdentity(this.TfsRequestContext.Elevate(), memberId, QueryMembership.None);
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      IdentityDescriptor descriptor = identity1.Descriptor;
      Microsoft.VisualStudio.Services.Identity.Identity member = identity2;
      return this.Request.CreateResponse<bool>(HttpStatusCode.OK, service.AddMemberToGroup(tfsRequestContext, descriptor, member));
    }

    [HttpDelete]
    public HttpResponseMessage RemoveMember(string containerId, string memberId)
    {
      IdentityService service = this.TfsRequestContext.GetService<IdentityService>();
      Microsoft.VisualStudio.Services.Identity.Identity identity1 = MembersController_BackCompatController.ReadIdentity(this.TfsRequestContext.Elevate(), containerId, QueryMembership.None);
      Microsoft.VisualStudio.Services.Identity.Identity identity2 = MembersController_BackCompatController.ReadIdentity(this.TfsRequestContext.Elevate(), memberId, QueryMembership.None);
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      IdentityDescriptor descriptor1 = identity1.Descriptor;
      IdentityDescriptor descriptor2 = identity2.Descriptor;
      return this.Request.CreateResponse<bool>(HttpStatusCode.OK, service.RemoveMemberFromGroup(tfsRequestContext, descriptor1, descriptor2));
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
        requestContext.TraceConditionally(IdentityTracing.ConvertToReadIdentityTracePoint(IdentityTracing.ReadIdentityTraceKind.ById, queryMembership, 0, 1), TraceLevel.Verbose, "Identity", nameof (MembersController_BackCompatController), (Func<string>) (() => string.Format("MembersController_BackCompatController.ReadIdentity where identityId : {0}, queryMembership : {1}", (object) identityGuid, (object) queryMembership)));
        return service.ReadIdentities(requestContext, (IList<Guid>) new Guid[1]
        {
          identityGuid
        }, queryMembership, (IEnumerable<string>) null)[0] ?? throw new IdentityNotFoundException(identityGuid);
      }
      IdentityDescriptor descriptor = IdentityParser.GetDescriptorFromString(identityId);
      requestContext.TraceConditionally(IdentityTracing.ConvertToReadIdentityTracePoint(IdentityTracing.ReadIdentityTraceKind.ByDescriptor, queryMembership, 0, 1), TraceLevel.Verbose, "Identity", nameof (MembersController_BackCompatController), (Func<string>) (() => string.Format("MembersController_BackCompatController.ReadIdentity where descriptor : {0}, queryMembership : {1}", (object) descriptor, (object) queryMembership)));
      return service.ReadIdentities(requestContext, (IList<IdentityDescriptor>) new IdentityDescriptor[1]
      {
        descriptor
      }, queryMembership, (IEnumerable<string>) null)[0] ?? throw new IdentityNotFoundException(descriptor);
    }

    public override IDictionary<Type, HttpStatusCode> HttpExceptions => (IDictionary<Type, HttpStatusCode>) MembersController_BackCompatController.s_httpExceptions;

    public override string TraceArea => "IdentityService";
  }
}
