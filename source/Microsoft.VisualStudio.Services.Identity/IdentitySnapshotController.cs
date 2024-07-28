// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentitySnapshotController
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Identity
{
  [ControllerApiVersion(1.0)]
  [VersionedApiControllerCustomName(Area = "IMS", ResourceName = "IdentitySnapshot")]
  public class IdentitySnapshotController : IdentitiesControllerBase
  {
    [HttpGet]
    [ClientResponseType(typeof (IdentitySnapshot), null, null)]
    public HttpResponseMessage GetIdentitySnapshot(string scopeId)
    {
      PlatformIdentityService service = this.TfsRequestContext.GetService<PlatformIdentityService>();
      service.CheckPermission(this.TfsRequestContext, (string) null, 1, true);
      Guid.Parse(scopeId);
      HashSet<Guid> filterList = this.GetFilterList();
      return this.Request.CreateResponse<IdentitySnapshot>(HttpStatusCode.OK, service.ReadIdentitySnapshot(this.TfsRequestContext, scopeId, filterList));
    }

    private HashSet<Guid> GetFilterList()
    {
      HashSet<Guid> filterList = new HashSet<Guid>();
      Microsoft.VisualStudio.Services.Identity.Identity readIdentity = this.TfsRequestContext.GetService<IdentityService>().ReadIdentities(this.TfsRequestContext, (IList<IdentityDescriptor>) new IdentityDescriptor[1]
      {
        GroupWellKnownIdentityDescriptors.InvitedUsersGroup
      }, QueryMembership.None, (IEnumerable<string>) null)[0];
      if (readIdentity != null)
        filterList.Add(readIdentity.Id);
      return filterList;
    }

    public override IDictionary<Type, HttpStatusCode> HttpExceptions => (IDictionary<Type, HttpStatusCode>) IdentitiesController.s_httpIdentityServiceExceptions;

    public override string TraceArea => "IdentityService";
  }
}
