// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.Service.RestServices.LicensingTransferIdentitiesExtensionsController
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Licensing.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Licensing.Service.RestServices
{
  [ControllerApiVersion(1.0)]
  public class LicensingTransferIdentitiesExtensionsController : LicensingApiController
  {
    [HttpPost]
    [TraceFilterWithException(1040070, 1040078, 1040079)]
    [ClientLocationId("DA46FE26-DBB6-41D9-9D6B-86BF47E4E444")]
    [ClientResponseType(typeof (void), null, null)]
    public HttpResponseMessage TransferExtensionsForIdentities(
      [FromBody] IList<IdentityMapping> identityMapping)
    {
      List<Tuple<Microsoft.VisualStudio.Services.Identity.Identity, Microsoft.VisualStudio.Services.Identity.Identity>> list = identityMapping.Select<IdentityMapping, Tuple<Microsoft.VisualStudio.Services.Identity.Identity, Microsoft.VisualStudio.Services.Identity.Identity>>((Func<IdentityMapping, Tuple<Microsoft.VisualStudio.Services.Identity.Identity, Microsoft.VisualStudio.Services.Identity.Identity>>) (x => new Tuple<Microsoft.VisualStudio.Services.Identity.Identity, Microsoft.VisualStudio.Services.Identity.Identity>(x.SourceIdentity, x.TargetIdentity))).ToList<Tuple<Microsoft.VisualStudio.Services.Identity.Identity, Microsoft.VisualStudio.Services.Identity.Identity>>();
      this.TfsRequestContext.GetService<IExtensionEntitlementService>().TransferExtensionsForIdentities(this.TfsRequestContext, (IList<Tuple<Microsoft.VisualStudio.Services.Identity.Identity, Microsoft.VisualStudio.Services.Identity.Identity>>) list);
      return this.Request.CreateResponse(HttpStatusCode.OK);
    }
  }
}
