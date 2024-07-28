// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.Scopes2Controller
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi.Patch;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Identity
{
  [ControllerApiVersion(5.0)]
  [VersionedApiControllerCustomName(Area = "IMS", ResourceName = "Scopes", ResourceVersion = 2)]
  public class Scopes2Controller : ScopesBaseController
  {
    private const string s_enableRestoreScopeFeatureFlag = "VisualStudio.Services.Identity.EnableRestoreScope";

    [HttpPatch]
    [ClientRequestContentMediaType("application/json-patch+json")]
    [RequestContentTypeRestriction(AllowJson = false)]
    [ClientResponseType(typeof (void), null, null)]
    public HttpResponseMessage UpdateScope(Guid scopeId, [ClientParameterType(typeof (JsonPatchDocument), false)] PatchDocument<IdentityScope> patchDocument)
    {
      IPatchOperation<IdentityScope> patchOperation = patchDocument.Operations.SingleOrDefault<IPatchOperation<IdentityScope>>();
      if (patchOperation == null)
        throw new ScopeBadRequestException(FrameworkResources.NullOperationDetected());
      if (patchOperation.Operation != Operation.Replace)
        throw new ScopeBadRequestException(FrameworkResources.JsonPatchOperationNotSupported((object) patchOperation.Operation));
      string str = patchOperation.Path == null ? string.Empty : patchOperation.Path;
      if (str.StartsWith("/"))
        str = patchOperation.Path.Substring(1);
      PlatformIdentityService service = this.TfsRequestContext.GetService<PlatformIdentityService>();
      if ("IsActive".Equals(str, StringComparison.InvariantCultureIgnoreCase))
      {
        if (!this.TfsRequestContext.IsFeatureEnabled("VisualStudio.Services.Identity.EnableRestoreScope"))
          throw new FeatureDisabledException("EnableRestoreScope Feature Flag not enabled");
        service.RestoreScope(this.TfsRequestContext, scopeId);
      }
      else
      {
        if (!"Name".Equals(str, StringComparison.InvariantCultureIgnoreCase))
          throw new ScopeBadRequestException(FrameworkResources.NotSupportedJsonPatchOperation((object) patchOperation.Operation, (object) patchOperation.Path, patchOperation.Value));
        string newName = (string) patchOperation.Value;
        service.RenameScope(this.TfsRequestContext, scopeId, newName);
      }
      return new HttpResponseMessage()
      {
        StatusCode = HttpStatusCode.OK,
        RequestMessage = this.Request
      };
    }
  }
}
