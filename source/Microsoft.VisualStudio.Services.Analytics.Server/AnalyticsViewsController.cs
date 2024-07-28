// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.AnalyticsViewsController
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Analytics.Telemetry;
using Microsoft.VisualStudio.Services.Analytics.WebApi;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Analytics
{
  [VersionedApiControllerCustomName(Area = "AnalyticsViews", ResourceName = "Views")]
  public class AnalyticsViewsController : AnalyticsViewsControllerBase
  {
    protected override void InitializeExceptionMap(ApiExceptionMapping exceptionMap)
    {
      base.InitializeExceptionMap(exceptionMap);
      exceptionMap.AddStatusCode<AnalyticsViewCreationFailedException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<AnalyticsViewValidationException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<AnalyticsViewAccessDeniedException>(HttpStatusCode.Forbidden);
      exceptionMap.AddStatusCode<AnalyticsViewDoesNotExistException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<AnalyticsViewDataNotFoundException>(HttpStatusCode.InternalServerError);
      exceptionMap.AddStatusCode<AnalyticsViewSchemaGenerationFailedException>(HttpStatusCode.InternalServerError);
      exceptionMap.AddStatusCode<AnalyticsViewsODataMetadataReadException>(HttpStatusCode.InternalServerError);
    }

    [HttpGet]
    public IEnumerable<AnalyticsView> GetViews()
    {
      IAnalyticsViewsService service = this.TfsRequestContext.GetService<IAnalyticsViewsService>();
      ViewsEngagementPublisher.PublishGetViewsEvent(this.TfsRequestContext);
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      AnalyticsViewScope viewScope = this.GetViewScope();
      return service.GetViews(tfsRequestContext, viewScope);
    }

    [HttpGet]
    public AnalyticsView GetView(Guid viewId, [FromUri(Name = "$expand")] AnalyticsViewExpandFlags expand = AnalyticsViewExpandFlags.None)
    {
      ViewsEngagementPublisher.PublishGetViewEvent(this.TfsRequestContext, viewId, expand);
      ArgumentUtility.CheckForEmptyGuid(viewId, nameof (viewId));
      return this.TfsRequestContext.GetService<IAnalyticsViewsService>().GetView(this.TfsRequestContext, this.GetViewScope(), viewId, expand);
    }

    [FeatureEnabled("Analytics.Views.RestApi.Editable")]
    [HttpPost]
    public AnalyticsView CreateView(AnalyticsViewCreateParameters view, [FromUri(Name = "preview")] bool preview = false)
    {
      ViewsEngagementPublisher.PublishCreateViewEvent(this.TfsRequestContext, preview);
      ArgumentUtility.CheckForNull<AnalyticsViewCreateParameters>(view, nameof (view));
      return this.TfsRequestContext.GetService<IAnalyticsViewsService>().CreateView(this.TfsRequestContext, this.GetViewScope(), view, preview);
    }

    [FeatureEnabled("Analytics.Views.RestApi.Editable")]
    [HttpPut]
    public AnalyticsView ReplaceView(Guid viewId, AnalyticsViewReplaceParameters view)
    {
      ViewsEngagementPublisher.PublishReplaceViewEvent(this.TfsRequestContext, viewId);
      ArgumentUtility.CheckForEmptyGuid(viewId, nameof (viewId));
      ArgumentUtility.CheckForNull<AnalyticsViewReplaceParameters>(view, nameof (view));
      return this.TfsRequestContext.GetService<IAnalyticsViewsService>().ReplaceView(this.TfsRequestContext, this.GetViewScope(), viewId, view);
    }

    [FeatureEnabled("Analytics.Views.RestApi.Editable")]
    [HttpDelete]
    [ClientResponseType(typeof (void), null, null)]
    public void DeleteView(Guid viewId)
    {
      ViewsEngagementPublisher.PublishDeleteViewEvent(this.TfsRequestContext, viewId);
      ArgumentUtility.CheckForEmptyGuid(viewId, nameof (viewId));
      this.TfsRequestContext.GetService<IAnalyticsViewsService>().DeleteView(this.TfsRequestContext, this.GetViewScope(), viewId);
    }
  }
}
