// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.LicensingLicensingRightsController
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Licensing;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Licensing
{
  [ControllerApiVersion(1.0)]
  public class LicensingLicensingRightsController : LicensingApiController
  {
    [HttpPost]
    [ClientLocationId("8671B016-FA74-4C88-B693-83BBB88C2264")]
    [ClientResponseType(typeof (void), null, null)]
    [ClientIgnore]
    public HttpResponseMessage TransferIdentityRights(
      IEnumerable<KeyValuePair<Guid, Guid>> userIdTransferMap,
      bool validateOnly = false)
    {
      ILicensingRightsService service = this.TfsRequestContext.GetService<ILicensingRightsService>();
      if (validateOnly)
        service.PreValidateTransferIdentityRights(this.TfsRequestContext, userIdTransferMap);
      else
        service.TransferIdentityRights(this.TfsRequestContext, userIdTransferMap);
      return this.Request.CreateResponse(HttpStatusCode.OK);
    }
  }
}
