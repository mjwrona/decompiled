// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure.AnalyticsODataBatchHandler
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.AspNet.OData.Batch;
using Microsoft.OData;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure
{
  public class AnalyticsODataBatchHandler : DefaultODataBatchHandler
  {
    private static readonly string s_layer = "ODataBatchHandler";

    public AnalyticsODataBatchHandler(HttpServer server)
      : base(server)
    {
      this.MessageQuotas.MaxPartsPerBatch = 1;
    }

    public override async Task<HttpResponseMessage> ProcessBatchAsync(
      HttpRequestMessage request,
      CancellationToken cancellationToken)
    {
      IVssRequestContext vssRequestContext = HttpContext.Current.GetVssRequestContext();
      vssRequestContext.TraceEnter(12013027, this.Area, this.Layer, nameof (ProcessBatchAsync));
      HttpResponseMessage httpResponseMessage1;
      try
      {
        HttpResponseMessage httpResponseMessage2 = await base.ProcessBatchAsync(request, cancellationToken);
        OperationResponseItem operationResponseItem = httpResponseMessage2.Content is ODataBatchContent content ? content.Responses.OfType<OperationResponseItem>().FirstOrDefault<OperationResponseItem>((Func<OperationResponseItem, bool>) (x => x.Response.StatusCode == HttpStatusCode.Unauthorized)) : (OperationResponseItem) null;
        httpResponseMessage1 = operationResponseItem == null ? httpResponseMessage2 : operationResponseItem.Response;
      }
      finally
      {
        vssRequestContext.TraceLeave(12013028, this.Area, this.Layer, nameof (ProcessBatchAsync));
      }
      vssRequestContext = (IVssRequestContext) null;
      return httpResponseMessage1;
    }

    public override async Task<IList<ODataBatchRequestItem>> ParseBatchRequestsAsync(
      HttpRequestMessage request,
      CancellationToken cancellationToken)
    {
      AnalyticsODataBatchHandler odataBatchHandler = this;
      HttpRequestMessage httpRequestMessage = request;
      httpRequestMessage.Content = await AnalyticsODataBatchPayloadRewriter.RewriteSubRequests(request);
      httpRequestMessage = (HttpRequestMessage) null;
      IList<ODataBatchRequestItem> batchItems = (IList<ODataBatchRequestItem>) null;
      try
      {
        // ISSUE: reference to a compiler-generated method
        batchItems = await odataBatchHandler.\u003C\u003En__1(request, cancellationToken);
      }
      catch (ODataException ex)
      {
        request.ThrowWrappedException((Exception) ex);
      }
      if (batchItems.Count > odataBatchHandler.MessageQuotas.MaxPartsPerBatch || batchItems.Any<ODataBatchRequestItem>((Func<ODataBatchRequestItem, bool>) (bi => bi.GetType() == typeof (ChangeSetRequestItem))))
        request.ThrowWrappedException((Exception) new ODataException(AnalyticsResources.ODATA_BATCH_QUERY_SIZE_INVALID()));
      else if (batchItems.Any<ODataBatchRequestItem>((Func<ODataBatchRequestItem, bool>) (bi => bi.GetType() == typeof (ChangeSetRequestItem))))
        request.ThrowWrappedException((Exception) new ODataException(AnalyticsResources.ODATA_BATCH_CHANGESET_INVALID()));
      IList<ODataBatchRequestItem> batchRequestsAsync = batchItems;
      batchItems = (IList<ODataBatchRequestItem>) null;
      return batchRequestsAsync;
    }

    protected string Area => "AnalyticsModel";

    protected string Layer => AnalyticsODataBatchHandler.s_layer;
  }
}
