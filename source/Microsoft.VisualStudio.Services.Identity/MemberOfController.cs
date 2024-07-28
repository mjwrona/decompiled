// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.MemberOfController
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Identity
{
  [ControllerApiVersion(1.0)]
  [VersionedApiControllerCustomName(Area = "IMS", ResourceName = "MembersOf")]
  public class MemberOfController : IdentitiesControllerBase
  {
    [HttpGet]
    [ClientLocationId("22865B02-9E4A-479E-9E18-E35B8803B8A0")]
    public IQueryable<IdentityDescriptor> ReadMembersOf(
      string memberId,
      QueryMembership queryMembership = QueryMembership.Direct)
    {
      this.TfsRequestContext.GetService<IdentityService>();
      return MembersController.ReadIdentity(this.TfsRequestContext, memberId, queryMembership).MemberOf.AsQueryable<IdentityDescriptor>();
    }

    [HttpGet]
    [ClientLocationId("22865B02-9E4A-479E-9E18-E35B8803B8A0")]
    [ClientResponseType(typeof (IdentityDescriptor), null, null)]
    public HttpResponseMessage ReadMemberOf(
      string memberId,
      string containerId,
      QueryMembership queryMembership = QueryMembership.Direct)
    {
      this.TfsRequestContext.GetService<IdentityService>();
      Microsoft.VisualStudio.Services.Identity.Identity identity = MembersController.ReadIdentity(this.TfsRequestContext, memberId, queryMembership);
      Microsoft.VisualStudio.Services.Identity.Identity container = MembersController.ReadIdentity(this.TfsRequestContext, containerId, QueryMembership.None);
      return this.Request.CreateResponse<IdentityDescriptor>(HttpStatusCode.OK, identity.MemberOf.Where<IdentityDescriptor>((Func<IdentityDescriptor, bool>) (containerDescriptor => IdentityDescriptorComparer.Instance.Equals(container.Descriptor, containerDescriptor))).FirstOrDefault<IdentityDescriptor>());
    }

    public override IDictionary<Type, HttpStatusCode> HttpExceptions => (IDictionary<Type, HttpStatusCode>) IdentitiesController.s_httpIdentityServiceExceptions;

    public override string TraceArea => "IdentityService";
  }
}
