// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.ClaimsController
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Identity
{
  [ControllerApiVersion(1.0)]
  [VersionedApiControllerCustomName(Area = "IMS", ResourceName = "Claims")]
  public class ClaimsController : IdentitiesControllerBase
  {
    public const string TraceLayer = "ClaimsController";

    [HttpPut]
    [ClientResponseType(typeof (Microsoft.VisualStudio.Services.Identity.Identity), null, null)]
    public HttpResponseMessage CreateOrBindWithClaims(Microsoft.VisualStudio.Services.Identity.Identity sourceIdentity)
    {
      this.TfsRequestContext.TraceAlways(15109093, TraceLevel.Info, this.TraceArea, nameof (ClaimsController), string.Format("Framework requested to create or bind identity {0}", (object) sourceIdentity?.Descriptor));
      Microsoft.VisualStudio.Services.Identity.Identity orBindWithClaims = this.TfsRequestContext.GetService<PlatformClaimsUpdateService>().CreateOrBindWithClaims(this.TfsRequestContext, sourceIdentity);
      if (orBindWithClaims != null)
        this.ScrubIdentityPropertiesAndMasterId((IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) new Microsoft.VisualStudio.Services.Identity.Identity[1]
        {
          orBindWithClaims
        });
      return this.Request.CreateResponse<Microsoft.VisualStudio.Services.Identity.Identity>(HttpStatusCode.OK, orBindWithClaims);
    }

    public override IDictionary<Type, HttpStatusCode> HttpExceptions => (IDictionary<Type, HttpStatusCode>) IdentitiesController.s_httpIdentityServiceExceptions;

    public override string TraceArea => "IdentityService";
  }
}
