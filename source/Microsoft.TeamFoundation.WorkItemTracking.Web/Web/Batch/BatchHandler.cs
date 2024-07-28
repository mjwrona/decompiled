// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Batch.BatchHandler
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers;
using Microsoft.VisualStudio.Services.Common;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Batch
{
  public abstract class BatchHandler : IBatchHandler
  {
    public BatchHandler(HttpServer server)
    {
      this.Server = server;
      this.ControllerSelector = new DefaultHttpControllerSelector(this.Server.Configuration);
    }

    public HttpServer Server { get; private set; }

    protected DefaultHttpControllerSelector ControllerSelector { get; }

    public abstract IEnumerable<HttpResponseMessage> ProcessBatch(
      IVssRequestContext requestContext,
      IEnumerable<BatchHttpRequestMessage> requests);

    protected virtual IHttpController GetControllerFromRequest(HttpRequestMessage request)
    {
      ArgumentUtility.CheckForNull<HttpRequestMessage>(request, nameof (request));
      return this.ControllerSelector.SelectController(request).CreateController(request);
    }

    internal virtual bool ShouldUseIdentityRefForWorkItemFieldValues(
      IVssRequestContext requestContext,
      IHttpController controller)
    {
      return ((WorkItemTrackingApiController) controller).ShouldUseIdentityRefForWorkItemFieldValues(requestContext);
    }

    internal virtual bool ShouldReturnProjectScopedUrl(
      IVssRequestContext requestContext,
      IHttpController controller)
    {
      return ((WorkItemTrackingApiController) controller).ShouldReturnProjectScopedUrls(requestContext);
    }

    internal virtual bool ShouldReturnProjectScopedUrl(
      IVssRequestContext requestContext,
      HttpRequestMessage request)
    {
      return ((WorkItemTrackingApiController) this.ControllerSelector.SelectController(request).CreateController(request)).ShouldReturnProjectScopedUrls(requestContext);
    }
  }
}
