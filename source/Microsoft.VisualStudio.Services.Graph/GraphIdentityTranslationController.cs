// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Graph.GraphIdentityTranslationController
// Assembly: Microsoft.VisualStudio.Services.Graph, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 00390AA0-D8BB-45EB-AEF5-70DC8BFC765D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Graph.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Graph
{
  [ControllerApiVersion(6.0)]
  [VersionedApiControllerCustomName(Area = "Graph", ResourceName = "IdentityTranslation")]
  [ClientInternalUseOnly(true)]
  [RestrictInternalGraphEndpoints]
  public class GraphIdentityTranslationController : GraphControllerBase
  {
    [HttpGet]
    [ClientResponseType(typeof (Guid), null, null)]
    public HttpResponseMessage Translate([ClientQueryParameter] string masterId = null, [ClientQueryParameter] string localId = null)
    {
      IdentityIdTranslationService service = this.TfsRequestContext.GetService<IdentityIdTranslationService>();
      return string.IsNullOrEmpty(masterId) ? this.Request.CreateResponse<Guid>(HttpStatusCode.OK, service.TranslateToMasterId(this.TfsRequestContext, Guid.Parse(localId))) : this.Request.CreateResponse<Guid>(HttpStatusCode.OK, service.TranslateFromMasterId(this.TfsRequestContext, Guid.Parse(masterId)));
    }
  }
}
