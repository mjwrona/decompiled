// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentitiesDebugController_BackCompatController
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Identity
{
  [BackCompatJsonFormatter]
  [ControllerApiVersion(0.0)]
  [VersionedApiControllerCustomName(Area = "IMS", ResourceName = "IdentitiesDebug")]
  public class IdentitiesDebugController_BackCompatController : IdentitiesControllerBase
  {
    [HttpGet]
    public HttpResponseMessage GetResource(string resource) => (HttpResponseMessage) null;

    public override string TraceArea => "IdentityDebugService";
  }
}
