// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.FrameworkReportingEventsService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Microsoft.VisualStudio.Services.Commerce
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class FrameworkReportingEventsService : IReportingEventsService, IVssFrameworkService
  {
    private const string s_area = "Commerce";
    private const string s_layer = "FrameworkReportingEventsService";

    public void ServiceStart(IVssRequestContext requestContext) => requestContext.CheckHostedDeployment();

    public void ServiceEnd(IVssRequestContext requestContext)
    {
    }

    public IEnumerable<ICommerceEvent> GetCommerceEvents(
      IVssRequestContext requestContext,
      string viewName,
      string resourceName,
      DateTime startTime,
      DateTime endTime,
      string filter = null)
    {
      requestContext.CheckDeploymentRequestContext();
      ArgumentUtility.CheckStringForNullOrEmpty(viewName, nameof (viewName));
      ArgumentUtility.CheckStringForNullOrEmpty(resourceName, nameof (resourceName));
      requestContext.TraceEnter(5104319, "Commerce", nameof (FrameworkReportingEventsService), nameof (GetCommerceEvents));
      try
      {
        return CommerceDeploymentHelper.ExecuteFrameworkServiceWithFallback<IEnumerable<ICommerceEvent>>(requestContext, (Func<IEnumerable<ICommerceEvent>>) (() => this.GetCommerceReportingHttpClient(requestContext).GetCommerceEvents(viewName, resourceName, startTime, endTime, filter).SyncResult<IEnumerable<ICommerceEvent>>()), (Func<IEnumerable<ICommerceEvent>>) (() => this.GetReportingHttpClient(requestContext).GetCommerceEvents(viewName, resourceName, startTime, endTime, filter).SyncResult<IEnumerable<ICommerceEvent>>()), "Commerce", nameof (FrameworkReportingEventsService), 5104319, CommerceDeploymentHelper.IsCommerceServiceRoutingEnabled(requestContext) || CommerceDeploymentHelper.IsCommerceForwardingEnabled(requestContext, Guid.Empty, layer: nameof (FrameworkReportingEventsService)));
      }
      catch (Exception ex)
      {
        requestContext.TraceException(5104321, "Commerce", nameof (FrameworkReportingEventsService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(5104322, "Commerce", nameof (FrameworkReportingEventsService), nameof (GetCommerceEvents));
      }
    }

    internal virtual Microsoft.VisualStudio.Services.Commerce.Client.ReportingHttpClient GetReportingHttpClient(
      IVssRequestContext requestContext)
    {
      return requestContext.GetClient<Microsoft.VisualStudio.Services.Commerce.Client.ReportingHttpClient>();
    }

    internal virtual Microsoft.VisualStudio.Services.Commerce.WebApi.ReportingHttpClient GetCommerceReportingHttpClient(
      IVssRequestContext requestContext)
    {
      return requestContext.GetClient<Microsoft.VisualStudio.Services.Commerce.WebApi.ReportingHttpClient>();
    }
  }
}
