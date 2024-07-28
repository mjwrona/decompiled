// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.ExtensionEventsController
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  [ControllerApiVersion(3.1)]
  [VersionedApiControllerCustomName(Area = "gallery", ResourceName = "events")]
  public class ExtensionEventsController : TfsApiController
  {
    private const string c_traceArea = "WebApi";
    private const string c_activityLogArea = "ExtensionEventsController";

    [HttpPost]
    [ClientLocationId("0BF2BD3A-70E0-4D5D-8BF7-BD4A9C2AB6E7")]
    [ClientResponseType(typeof (void), null, null)]
    public HttpResponseMessage PublishExtensionEvents([FromBody] IEnumerable<ExtensionEvents> extensionEvents)
    {
      if (!this.CanPublishExtensionEvents(this.TfsRequestContext))
        return this.Request.CreateResponse(HttpStatusCode.MethodNotAllowed);
      this.TfsRequestContext.GetService<IExtensionDailyStatsService>().AddExtensionEvents(this.TfsRequestContext, extensionEvents);
      return this.Request.CreateResponse(HttpStatusCode.Created);
    }

    [HttpGet]
    [ClientLocationId("3D13C499-2168-4D06-BEF4-14ABA185DCD5")]
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "These are optional parameter in REST, they need to have default values if not specified by client")]
    [SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", Justification = "The name queryFlags is consistent with other places in Marketplace")]
    public ExtensionEvents GetExtensionEvents(
      string publisherName,
      string extensionName,
      [FromUri] int? count = null,
      [FromUri] DateTime? afterDate = null,
      [FromUri] string include = null,
      [FromUri] string includeProperty = null)
    {
      this.TfsRequestContext.Trace(12061090, TraceLevel.Info, "gallery", this.LayerName, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Controller params, publisherName:{0}, extensionName:{1}, count:{2}, afterDate:{3}, queryFlags={4}", (object) publisherName, (object) extensionName, (object) count, (object) afterDate, (object) include));
      ExtensionEvents extensionEvents = this.TfsRequestContext.GetService<IExtensionDailyStatsService>().GetExtensionEvents(this.TfsRequestContext, publisherName, extensionName, count, afterDate, include, includeProperty);
      extensionEvents.ExtensionId = new Guid();
      return extensionEvents;
    }

    protected internal virtual bool CanPublishExtensionEvents(IVssRequestContext requestContext)
    {
      bool flag = false;
      if (this.TfsRequestContext.ExecutionEnvironment.IsHostedDeployment && this.IsServicePrincipal(this.TfsRequestContext))
        flag = true;
      return flag;
    }

    [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Request context is always available")]
    protected internal virtual bool IsServicePrincipal(IVssRequestContext requestContext) => ServicePrincipals.IsServicePrincipal(requestContext, requestContext?.UserContext);

    public override string ActivityLogArea => nameof (ExtensionEventsController);

    public override string TraceArea => "WebApi";

    public string LayerName => nameof (ExtensionEventsController);
  }
}
