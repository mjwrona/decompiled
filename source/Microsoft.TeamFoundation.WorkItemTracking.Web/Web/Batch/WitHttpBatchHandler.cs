// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Batch.WitHttpBatchHandler
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Web.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Batch;
using System.Web.Http.Controllers;
using System.Web.Http.Hosting;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Batch
{
  public class WitHttpBatchHandler : HttpBatchHandler
  {
    private readonly HttpServer server;
    protected readonly IList<IBatchHandler> handlers = (IList<IBatchHandler>) new List<IBatchHandler>();

    public WitHttpBatchHandler(HttpServer server, bool IsPartialUpdateEnabled = false)
      : base(server)
    {
      this.server = server;
      this.handlers.Add((IBatchHandler) new InvalidRouteBatchHandler(server));
      this.handlers.Add((IBatchHandler) new WorkItemRestoreBatchHandler(server));
      this.handlers.Add((IBatchHandler) new WorkItemDeleteBatchHandler(server));
      this.handlers.Add((IBatchHandler) new WorkItemDestroyBatchHandler(server));
      this.handlers.Add((IBatchHandler) new QueryGetBatchHandler(server));
      if (IsPartialUpdateEnabled)
        this.handlers.Add((IBatchHandler) new WorkItemPartialUpdateBatchHandler(server));
      else
        this.handlers.Add((IBatchHandler) new WorkItemUpdateBatchHandler(server));
      this.handlers.Add((IBatchHandler) new UnhandledBatchHandler(server));
    }

    public override Task<HttpResponseMessage> ProcessBatchAsync(
      HttpRequestMessage request,
      CancellationToken cancellationToken)
    {
      IVssRequestContext property = (IVssRequestContext) request.Properties[TfsApiPropertyKeys.TfsRequestContext];
      property.ValidateIdentity();
      IList<BatchHttpRequestMessage> batchRequests;
      try
      {
        batchRequests = this.ParseBatchRequests(property, request);
      }
      catch (Exception ex)
      {
        return Task.FromResult<HttpResponseMessage>(request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.Message));
      }
      List<HttpResponseMessage> source = new List<HttpResponseMessage>();
      foreach (IBatchHandler handler in (IEnumerable<IBatchHandler>) this.handlers)
      {
        cancellationToken.ThrowIfCancellationRequested();
        source.AddRange(handler.ProcessBatch(property, batchRequests.Where<BatchHttpRequestMessage>((Func<BatchHttpRequestMessage, bool>) (r => !r.Handled))));
      }
      List<HttpResponseMessage> list = source.OrderBy<HttpResponseMessage, int>((Func<HttpResponseMessage, int>) (r => ((BatchHttpRequestMessage) r.RequestMessage).RequestId)).ToList<HttpResponseMessage>();
      TelemetryHelper.PublishBatchHandlerResults(property, batchRequests);
      try
      {
        return Task.FromResult<HttpResponseMessage>(this.CreateResponseMessage((IList<HttpResponseMessage>) list, request));
      }
      finally
      {
        foreach (HttpRequestMessage httpRequestMessage in (IEnumerable<BatchHttpRequestMessage>) batchRequests)
        {
          request.RegisterForDispose(httpRequestMessage.GetResourcesForDisposal());
          request.RegisterForDispose((IDisposable) httpRequestMessage);
        }
      }
    }

    internal virtual IList<BatchHttpRequestMessage> ParseBatchRequests(
      IVssRequestContext tfsRequestContext,
      HttpRequestMessage request)
    {
      IList<BatchHttpRequestMessage> batchRequests = (IList<BatchHttpRequestMessage>) new List<BatchHttpRequestMessage>();
      if (request.Content == null)
        return batchRequests;
      request.Properties.Add(HttpPropertyKeys.IsBatchRequest, (object) true);
      JsonBatchHttpRequest result = request.Content.ReadAsAsync<JsonBatchHttpRequest>().Result;
      if (result != null)
      {
        int num = 1;
        List<ValidationResult> source = new List<ValidationResult>();
        string absoluteUri = request.RequestUri.AbsoluteUri;
        string str1 = tfsRequestContext.RelativeUrl();
        string str2 = absoluteUri.Substring(0, absoluteUri.LastIndexOf(str1));
        foreach (JsonHttpRequest instance in (List<JsonHttpRequest>) result)
        {
          ValidationContext validationContext = new ValidationContext((object) instance);
          if (!System.ComponentModel.DataAnnotations.Validator.TryValidateObject((object) instance, validationContext, (ICollection<ValidationResult>) source) && source.Any<ValidationResult>())
            throw new ValidationException(string.Join(",", source.Select<ValidationResult, string>((Func<ValidationResult, string>) (v => v.ErrorMessage))));
          Uri requestUri = new Uri(str2 + instance.Uri, UriKind.Absolute);
          BatchHttpRequestMessage httpRequestMessage = new BatchHttpRequestMessage(new HttpMethod(instance.Method), requestUri);
          httpRequestMessage.CopyBatchRequestProperties(request);
          httpRequestMessage.RequestId = num++;
          if (instance.Body != null)
          {
            StringContent stringContent = new StringContent(instance.Body.ToString());
            if (instance.Headers != null)
            {
              foreach (KeyValuePair<string, string> header in instance.Headers)
              {
                stringContent.Headers.Remove(header.Key);
                stringContent.Headers.Add(header.Key, header.Value);
              }
            }
            httpRequestMessage.Content = (HttpContent) stringContent;
          }
          HttpRequestContext context = new HttpRequestContext()
          {
            Configuration = this.server.Configuration,
            RouteData = this.server.Configuration.Routes.GetRouteData((HttpRequestMessage) httpRequestMessage),
            IncludeErrorDetail = request.ShouldIncludeErrorDetail()
          };
          httpRequestMessage.SetRequestContext(context);
          batchRequests.Add(httpRequestMessage);
        }
      }
      return batchRequests;
    }

    internal virtual HttpResponseMessage CreateResponseMessage(
      IList<HttpResponseMessage> responses,
      HttpRequestMessage request)
    {
      JsonBatchHttpResponse batchHttpResponse = new JsonBatchHttpResponse();
      foreach (HttpResponseMessage response in (IEnumerable<HttpResponseMessage>) responses)
      {
        JsonHttpResponse jsonHttpResponse = new JsonHttpResponse()
        {
          Code = (int) response.StatusCode
        };
        if (response.Headers != null)
        {
          foreach (KeyValuePair<string, IEnumerable<string>> header in (HttpHeaders) response.Headers)
            jsonHttpResponse.Headers.Add(header.Key, string.Join(",", header.Value));
        }
        if (response.Content != null)
        {
          jsonHttpResponse.Body = response.Content.ReadAsStringAsync().Result;
          if (response.Content.Headers.Any<KeyValuePair<string, IEnumerable<string>>>())
            jsonHttpResponse.Headers = response.Content.Headers.ToDictionary<KeyValuePair<string, IEnumerable<string>>, string, string>((Func<KeyValuePair<string, IEnumerable<string>>, string>) (h => h.Key), (Func<KeyValuePair<string, IEnumerable<string>>, string>) (h => string.Join(",", h.Value)));
        }
        batchHttpResponse.Add(jsonHttpResponse);
      }
      return request.CreateResponse<JsonBatchHttpResponse>(HttpStatusCode.OK, batchHttpResponse);
    }
  }
}
