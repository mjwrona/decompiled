// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.VersionDiscoveryController
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3FE9DD3E-5758-4D1B-8056-ECAF8A4B7A77
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server
{
  [VersionedApiControllerCustomName(Area = "Extensions", ResourceName = "vdisc")]
  public class VersionDiscoveryController : TfsApiController
  {
    public HttpResponseMessage Options()
    {
      IVssRequestContext vssRequestContext = this.TfsRequestContext.To(TeamFoundationHostType.Deployment);
      List<SupportedExtension> source = vssRequestContext.GetService<IVersionDiscoveryService>().LoadSupportedVersions(vssRequestContext);
      HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK);
      MediaTypeHeaderValue mediaType = new MediaTypeHeaderValue("application/json");
      HttpConfiguration httpConfiguration = WebApiConfiguration.GetHttpConfiguration(this.TfsRequestContext);
      VssJsonCollectionWrapper<IEnumerable<SupportedExtension>> collectionWrapper = new VssJsonCollectionWrapper<IEnumerable<SupportedExtension>>((IEnumerable) source);
      Type type = typeof (VssJsonCollectionWrapper<IEnumerable<SupportedExtension>>);
      ContentNegotiationResult negotiationResult = httpConfiguration.Services.GetContentNegotiator().Negotiate(type, this.Request, (IEnumerable<MediaTypeFormatter>) httpConfiguration.Formatters);
      response.Content = (HttpContent) new ObjectContent(type, (object) collectionWrapper, negotiationResult.Formatter, mediaType);
      return response;
    }

    public override string TraceArea => "InstalledExtensionVersionController";

    public override string ActivityLogArea => "Extensions";
  }
}
