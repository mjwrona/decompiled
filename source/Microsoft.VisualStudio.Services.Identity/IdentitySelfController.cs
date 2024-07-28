// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentitySelfController
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Identity
{
  [ControllerApiVersion(1.0)]
  [VersionedApiControllerCustomName(Area = "IMS", ResourceName = "me")]
  [ClientInclude(~RestClientLanguages.Swagger2)]
  public class IdentitySelfController : TfsApiController
  {
    [HttpGet]
    [ClientResponseType(typeof (IdentitySelf), null, null)]
    public HttpResponseMessage GetSelf()
    {
      IdentitySelfService identitySelfService = new IdentitySelfService();
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity = this.TfsRequestContext.GetUserIdentity();
      IdentitySelf identitySelf;
      if (!this.TfsRequestContext.ExecutionEnvironment.IsHostedDeployment)
      {
        if (userIdentity != null)
          identitySelf = identitySelfService.GetIdentitySelfResult(this.TfsRequestContext, userIdentity, (Microsoft.VisualStudio.Services.Identity.Identity) null, (IEnumerable<TenantInfo>) new TenantInfo[0]);
        else
          identitySelf = new IdentitySelf()
          {
            Id = Guid.Empty,
            DisplayName = (string) null,
            AccountName = (string) null,
            Tenants = (IEnumerable<TenantInfo>) new TenantInfo[0]
          };
      }
      else
        identitySelf = identitySelfService.Get(this.TfsRequestContext);
      return this.Request.CreateResponse<IdentitySelf>(HttpStatusCode.OK, identitySelf);
    }

    public override string ActivityLogArea => "Identities";
  }
}
