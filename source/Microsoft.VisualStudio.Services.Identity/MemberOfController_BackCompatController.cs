// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.MemberOfController_BackCompatController
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
  [BackCompatJsonFormatter]
  [ControllerApiVersion(0.0)]
  [VersionedApiControllerCustomName(Area = "IMS", ResourceName = "MembersOf")]
  public class MemberOfController_BackCompatController : IdentitiesControllerBase
  {
    private static readonly Dictionary<Type, HttpStatusCode> s_httpExceptions = new Dictionary<Type, HttpStatusCode>();

    [HttpGet]
    public IQueryable<IdentityDescriptor> ReadMemberships(
      string memberId,
      QueryMembership queryMembership = QueryMembership.Direct)
    {
      this.TfsRequestContext.GetService<IdentityService>();
      return MembersController.ReadIdentity(this.TfsRequestContext, memberId, queryMembership).MemberOf.AsQueryable<IdentityDescriptor>();
    }

    [HttpGet]
    public HttpResponseMessage ReadMembership(
      string memberId,
      string containerId,
      QueryMembership queryMembership = QueryMembership.Direct)
    {
      this.TfsRequestContext.GetService<IdentityService>();
      Microsoft.VisualStudio.Services.Identity.Identity identity = MembersController.ReadIdentity(this.TfsRequestContext, memberId, queryMembership);
      Microsoft.VisualStudio.Services.Identity.Identity container = MembersController.ReadIdentity(this.TfsRequestContext, containerId, QueryMembership.None);
      return this.Request.CreateResponse<IdentityDescriptor>(HttpStatusCode.OK, identity.MemberOf.Where<IdentityDescriptor>((Func<IdentityDescriptor, bool>) (containerDescriptor => IdentityDescriptorComparer.Instance.Equals(container.Descriptor, containerDescriptor))).FirstOrDefault<IdentityDescriptor>());
    }

    public override IDictionary<Type, HttpStatusCode> HttpExceptions => (IDictionary<Type, HttpStatusCode>) MemberOfController_BackCompatController.s_httpExceptions;

    public override string TraceArea => "IdentityService";
  }
}
