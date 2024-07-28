// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.ScopesController
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Identity
{
  [ControllerApiVersion(1.0)]
  [VersionedApiControllerCustomName(Area = "IMS", ResourceName = "Scopes")]
  public class ScopesController : ScopesBaseController
  {
    [HttpPatch]
    [ClientResponseType(typeof (void), null, null)]
    public HttpResponseMessage RenameScope(Guid scopeId, IdentityScope renameScope)
    {
      string name = renameScope.Name;
      this.TfsRequestContext.GetService<IdentityService>().RenameScope(this.TfsRequestContext, scopeId, name);
      return new HttpResponseMessage()
      {
        StatusCode = HttpStatusCode.OK,
        RequestMessage = this.Request
      };
    }
  }
}
