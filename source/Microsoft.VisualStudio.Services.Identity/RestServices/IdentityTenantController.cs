// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.RestServices.IdentityTenantController
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Aad;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Identity.RestServices
{
  [ControllerApiVersion(1.0)]
  [VersionedApiControllerCustomName(Area = "IMS", ResourceName = "tenant")]
  [ClientInclude(~RestClientLanguages.Swagger2)]
  public class IdentityTenantController : TfsApiController
  {
    private const string TraceLayer = "Identities";

    public override string TraceArea => "IdentityTenants";

    [HttpGet]
    [ClientResponseType(typeof (TenantInfo), null, null)]
    public HttpResponseMessage GetTenant(string tenantId)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(tenantId, nameof (tenantId));
      if (!this.TfsRequestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        throw new UnexpectedHostTypeException(this.TfsRequestContext.ServiceHost.HostType);
      Guid result;
      if (!Guid.TryParse(tenantId, out result))
        throw new ArgumentException("Invalid tenantId.", tenantId);
      try
      {
        IVssRequestContext vssRequestContext = this.TfsRequestContext.Elevate();
        IAadTenantDetailProvider extension = vssRequestContext.GetExtension<IAadTenantDetailProvider>(ExtensionLifetime.Service);
        extension.CanHandleRequest(vssRequestContext);
        string displayName = extension.GetDisplayName(vssRequestContext, tenantId);
        IEnumerable<string> verifiedDomains = extension.GetVerifiedDomains(vssRequestContext, tenantId);
        return this.Request.CreateResponse<TenantInfo>(HttpStatusCode.OK, new TenantInfo()
        {
          TenantId = result,
          TenantName = displayName,
          VerifiedDomains = verifiedDomains
        });
      }
      catch (Exception ex)
      {
        this.TfsRequestContext.TraceException(0, this.TraceArea, "Identities", ex);
        return new HttpResponseMessage()
        {
          StatusCode = HttpStatusCode.NotFound,
          RequestMessage = this.Request
        };
      }
    }
  }
}
