// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.IdentityMruController
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Server.Core
{
  [VersionedApiControllerCustomName("core", "identityMru", 1)]
  [ClientInternalUseOnly(true)]
  public class IdentityMruController : ServerCoreApiController
  {
    [HttpGet]
    [ClientResponseType(typeof (IEnumerable<IdentityRef>), null, null)]
    public HttpResponseMessage GetIdentityMru(string mruName) => this.Request.CreateResponse<IEnumerable<IdentityRef>>(HttpStatusCode.OK, new IdentityMru(this.TfsRequestContext, mruName).Read(this.TfsRequestContext).OrderBy<Microsoft.VisualStudio.Services.Identity.Identity, string>((Func<Microsoft.VisualStudio.Services.Identity.Identity, string>) (identity => identity.DisplayName), (IComparer<string>) StringComparer.OrdinalIgnoreCase).Select<Microsoft.VisualStudio.Services.Identity.Identity, IdentityRef>((Func<Microsoft.VisualStudio.Services.Identity.Identity, IdentityRef>) (identity => identity.ToIdentityRef(this.TfsRequestContext))));

    [HttpPost]
    [ClientResponseType(typeof (void), null, null)]
    public HttpResponseMessage CreateIdentityMru(string mruName, Microsoft.TeamFoundation.Core.WebApi.IdentityData mruData)
    {
      ArgumentUtility.CheckForNull<Microsoft.TeamFoundation.Core.WebApi.IdentityData>(mruData, nameof (mruData));
      new IdentityMru(this.TfsRequestContext, mruName).AddItems(this.TfsRequestContext, mruData.IdentityIds);
      return this.Request.CreateResponse(HttpStatusCode.NoContent);
    }

    [HttpPatch]
    [ClientResponseType(typeof (void), null, null)]
    public HttpResponseMessage UpdateIdentityMru(string mruName, Microsoft.TeamFoundation.Core.WebApi.IdentityData mruData)
    {
      ArgumentUtility.CheckForNull<Microsoft.TeamFoundation.Core.WebApi.IdentityData>(mruData, nameof (mruData));
      new IdentityMru(this.TfsRequestContext, mruName).UpdateItems(this.TfsRequestContext, mruData.IdentityIds);
      return this.Request.CreateResponse(HttpStatusCode.NoContent);
    }

    [HttpDelete]
    [ClientResponseType(typeof (void), null, null)]
    public HttpResponseMessage DeleteIdentityMru(string mruName, Microsoft.TeamFoundation.Core.WebApi.IdentityData mruData)
    {
      ArgumentUtility.CheckForNull<Microsoft.TeamFoundation.Core.WebApi.IdentityData>(mruData, nameof (mruData));
      new IdentityMru(this.TfsRequestContext, mruName).DeleteItems(this.TfsRequestContext, mruData.IdentityIds);
      return this.Request.CreateResponse(HttpStatusCode.NoContent);
    }
  }
}
