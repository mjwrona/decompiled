// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.Server.ExtensionRequestsController
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 57F50803-C5C4-41A9-A26F-AD293D563111
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ExtensionManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;
using System.Net;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.Server
{
  [VersionedApiControllerCustomName(Area = "ExtensionManagement", ResourceName = "RequestedExtensions")]
  [ClientInternalUseOnly(false)]
  public class ExtensionRequestsController : TfsApiController
  {
    public override string TraceArea => "ExtensionRequest";

    public override string ActivityLogArea => "WebApi";

    protected override void InitializeExceptionMap(ApiExceptionMapping exceptionMap)
    {
      base.InitializeExceptionMap(exceptionMap);
      exceptionMap.AddStatusCode<InstalledExtensionNotFoundException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<ExtensionDoesNotExistException>(HttpStatusCode.NotFound);
    }

    [HttpGet]
    [ClientLocationId("216b978f-b164-424e-ada2-b77561e842b7")]
    public List<RequestedExtension> GetRequests() => this.TfsRequestContext.GetService<IExtensionRequestService>().GetRequests(this.TfsRequestContext);

    [HttpPost]
    [ClientLocationId("f5afca1e-a728-4294-aa2d-4af0173431b5")]
    [ClientEnforceBodyParameterToComeBeforeQueryParameters(false)]
    public RequestedExtension RequestExtension(
      string publisherName,
      string extensionName,
      [FromBody] string requestMessage)
    {
      ArgumentUtility.CheckForNull<string>(publisherName, nameof (publisherName));
      ArgumentUtility.CheckForNull<string>(extensionName, nameof (extensionName));
      return this.TfsRequestContext.GetService<IExtensionRequestService>().RequestExtension(this.TfsRequestContext, publisherName, extensionName, requestMessage);
    }

    [HttpPatch]
    [ClientLocationId("AA93E1F3-511C-4364-8B9C-EB98818F2E0B")]
    public int ResolveRequest(
      string publisherName,
      string extensionName,
      string requesterId,
      ExtensionRequestState state,
      [FromBody] string rejectMessage)
    {
      ArgumentUtility.CheckForNull<string>(publisherName, nameof (publisherName));
      ArgumentUtility.CheckForNull<string>(extensionName, nameof (extensionName));
      IList<ExtensionRequest> extensionRequestList = this.TfsRequestContext.GetService<IExtensionRequestService>().ResolveRequests(this.TfsRequestContext, publisherName, extensionName, requesterId, rejectMessage, state);
      return extensionRequestList == null ? 0 : extensionRequestList.Count;
    }

    [HttpPatch]
    [ClientLocationId("BA93E1F3-511C-4364-8B9C-EB98818F2E0B")]
    public int ResolveAllRequests(
      string publisherName,
      string extensionName,
      ExtensionRequestState state,
      [FromBody] string rejectMessage)
    {
      return this.ResolveRequest(publisherName, extensionName, (string) null, state, rejectMessage);
    }

    [HttpDelete]
    [ClientLocationId("f5afca1e-a728-4294-aa2d-4af0173431b5")]
    public void DeleteRequest(string publisherName, string extensionName)
    {
      ArgumentUtility.CheckForNull<string>(publisherName, nameof (publisherName));
      ArgumentUtility.CheckForNull<string>(extensionName, nameof (extensionName));
      this.TfsRequestContext.GetService<IExtensionRequestService>().DeleteRequests(this.TfsRequestContext, publisherName, extensionName);
    }
  }
}
