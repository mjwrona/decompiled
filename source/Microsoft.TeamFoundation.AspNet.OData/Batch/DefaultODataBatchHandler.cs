// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Batch.DefaultODataBatchHandler
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Adapters;
using Microsoft.AspNet.OData.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Batch;

namespace Microsoft.AspNet.OData.Batch
{
  public class DefaultODataBatchHandler : ODataBatchHandler
  {
    public DefaultODataBatchHandler(HttpServer httpServer)
      : base(httpServer)
    {
    }

    public override async Task<HttpResponseMessage> ProcessBatchAsync(
      HttpRequestMessage request,
      CancellationToken cancellationToken)
    {
      DefaultODataBatchHandler odataBatchHandler = this;
      if (request == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (request));
      odataBatchHandler.ValidateRequest(request);
      IList<ODataBatchRequestItem> subRequests = await odataBatchHandler.ParseBatchRequestsAsync(request, cancellationToken);
      HttpConfiguration configuration = request.GetConfiguration();
      bool enableContinueOnErrorHeader = configuration != null && configuration.HasEnabledContinueOnErrorHeader();
      odataBatchHandler.SetContinueOnError((IWebApiHeaders) new WebApiRequestHeaders(request.Headers), enableContinueOnErrorHeader);
      HttpResponseMessage responseMessageAsync;
      try
      {
        IList<ODataBatchResponseItem> responses = await odataBatchHandler.ExecuteRequestMessagesAsync((IEnumerable<ODataBatchRequestItem>) subRequests, cancellationToken);
        responseMessageAsync = await odataBatchHandler.CreateResponseMessageAsync((IEnumerable<ODataBatchResponseItem>) responses, request, cancellationToken);
      }
      finally
      {
        foreach (ODataBatchRequestItem resource in (IEnumerable<ODataBatchRequestItem>) subRequests)
        {
          request.RegisterForDispose(resource.GetResourcesForDisposal());
          request.RegisterForDispose((IDisposable) resource);
        }
      }
      return responseMessageAsync;
    }

    public virtual async Task<IList<ODataBatchResponseItem>> ExecuteRequestMessagesAsync(
      IEnumerable<ODataBatchRequestItem> requests,
      CancellationToken cancellationToken)
    {
      DefaultODataBatchHandler odataBatchHandler = this;
      if (requests == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (requests));
      IList<ODataBatchResponseItem> responses = (IList<ODataBatchResponseItem>) new List<ODataBatchResponseItem>();
      try
      {
        foreach (ODataBatchRequestItem request in requests)
        {
          ODataBatchResponseItem batchResponseItem = await request.SendRequestAsync(odataBatchHandler.Invoker, cancellationToken);
          responses.Add(batchResponseItem);
          if (batchResponseItem != null && !batchResponseItem.IsResponseSuccessful())
          {
            if (!odataBatchHandler.ContinueOnError)
              break;
          }
        }
      }
      catch
      {
        foreach (ODataBatchResponseItem batchResponseItem in (IEnumerable<ODataBatchResponseItem>) responses)
          batchResponseItem?.Dispose();
        throw;
      }
      return responses;
    }

    public virtual async Task<IList<ODataBatchRequestItem>> ParseBatchRequestsAsync(
      HttpRequestMessage request,
      CancellationToken cancellationToken)
    {
      DefaultODataBatchHandler odataBatchHandler = this;
      if (request == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (request));
      IServiceProvider requestContainer = request.CreateRequestContainer(odataBatchHandler.ODataRouteName);
      ServiceProviderServiceExtensions.GetRequiredService<ODataMessageReaderSettings>(requestContainer).BaseUri = odataBatchHandler.GetBaseUri(request);
      ODataMessageReader messageReaderAsync = await request.Content.GetODataMessageReaderAsync(requestContainer, cancellationToken);
      request.RegisterForDispose((IDisposable) messageReaderAsync);
      List<ODataBatchRequestItem> requests = new List<ODataBatchRequestItem>();
      ODataBatchReader batchReader = messageReaderAsync.CreateODataBatchReader();
      Guid batchId = Guid.NewGuid();
      while (batchReader.Read())
      {
        if (batchReader.State == ODataBatchReaderState.ChangesetStart)
        {
          IList<HttpRequestMessage> requests1 = await batchReader.ReadChangeSetRequestAsync(batchId, cancellationToken);
          foreach (HttpRequestMessage httpRequestMessage in (IEnumerable<HttpRequestMessage>) requests1)
          {
            httpRequestMessage.CopyBatchRequestProperties(request);
            httpRequestMessage.DeleteRequestContainer(false);
          }
          requests.Add((ODataBatchRequestItem) new ChangeSetRequestItem((IEnumerable<HttpRequestMessage>) requests1));
        }
        else if (batchReader.State == ODataBatchReaderState.Operation)
        {
          HttpRequestMessage httpRequestMessage = await batchReader.ReadOperationRequestAsync(batchId, true, cancellationToken);
          httpRequestMessage.CopyBatchRequestProperties(request);
          httpRequestMessage.DeleteRequestContainer(false);
          requests.Add((ODataBatchRequestItem) new OperationRequestItem(httpRequestMessage));
        }
      }
      return (IList<ODataBatchRequestItem>) requests;
    }
  }
}
