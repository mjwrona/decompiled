// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Batch.UnbufferedODataBatchHandler
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
  public class UnbufferedODataBatchHandler : ODataBatchHandler
  {
    public UnbufferedODataBatchHandler(HttpServer httpServer)
      : base(httpServer)
    {
    }

    public override async Task<HttpResponseMessage> ProcessBatchAsync(
      HttpRequestMessage request,
      CancellationToken cancellationToken)
    {
      UnbufferedODataBatchHandler odataBatchHandler = this;
      if (request == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (request));
      odataBatchHandler.ValidateRequest(request);
      IServiceProvider requestContainer = request.CreateRequestContainer(odataBatchHandler.ODataRouteName);
      ServiceProviderServiceExtensions.GetRequiredService<ODataMessageReaderSettings>(requestContainer).BaseUri = odataBatchHandler.GetBaseUri(request);
      ODataMessageReader messageReaderAsync = await request.Content.GetODataMessageReaderAsync(requestContainer, cancellationToken);
      request.RegisterForDispose((IDisposable) messageReaderAsync);
      ODataBatchReader batchReader = messageReaderAsync.CreateODataBatchReader();
      List<ODataBatchResponseItem> responses = new List<ODataBatchResponseItem>();
      Guid batchId = Guid.NewGuid();
      HttpConfiguration configuration = request.GetConfiguration();
      bool enableContinueOnErrorHeader = configuration != null && configuration.HasEnabledContinueOnErrorHeader();
      odataBatchHandler.SetContinueOnError((IWebApiHeaders) new WebApiRequestHeaders(request.Headers), enableContinueOnErrorHeader);
      try
      {
        while (batchReader.Read())
        {
          ODataBatchResponseItem batchResponseItem = (ODataBatchResponseItem) null;
          if (batchReader.State == ODataBatchReaderState.ChangesetStart)
            batchResponseItem = await odataBatchHandler.ExecuteChangeSetAsync(batchReader, batchId, request, cancellationToken);
          else if (batchReader.State == ODataBatchReaderState.Operation)
            batchResponseItem = await odataBatchHandler.ExecuteOperationAsync(batchReader, batchId, request, cancellationToken);
          if (batchResponseItem != null)
          {
            responses.Add(batchResponseItem);
            if (!batchResponseItem.IsResponseSuccessful())
            {
              if (!odataBatchHandler.ContinueOnError)
                break;
            }
          }
        }
      }
      catch
      {
        foreach (ODataBatchResponseItem batchResponseItem in responses)
          batchResponseItem?.Dispose();
        throw;
      }
      return await odataBatchHandler.CreateResponseMessageAsync((IEnumerable<ODataBatchResponseItem>) responses, request, cancellationToken);
    }

    public virtual async Task<ODataBatchResponseItem> ExecuteOperationAsync(
      ODataBatchReader batchReader,
      Guid batchId,
      HttpRequestMessage originalRequest,
      CancellationToken cancellationToken)
    {
      UnbufferedODataBatchHandler odataBatchHandler = this;
      if (batchReader == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (batchReader));
      if (originalRequest == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (originalRequest));
      cancellationToken.ThrowIfCancellationRequested();
      HttpRequestMessage httpRequestMessage = await batchReader.ReadOperationRequestAsync(batchId, false);
      httpRequestMessage.CopyBatchRequestProperties(originalRequest);
      httpRequestMessage.DeleteRequestContainer(false);
      OperationRequestItem operation = new OperationRequestItem(httpRequestMessage);
      ODataBatchResponseItem batchResponseItem;
      try
      {
        batchResponseItem = await operation.SendRequestAsync(odataBatchHandler.Invoker, cancellationToken);
      }
      finally
      {
        originalRequest.RegisterForDispose(operation.GetResourcesForDisposal());
        originalRequest.RegisterForDispose((IDisposable) operation);
      }
      return batchResponseItem;
    }

    public virtual async Task<ODataBatchResponseItem> ExecuteChangeSetAsync(
      ODataBatchReader batchReader,
      Guid batchId,
      HttpRequestMessage originalRequest,
      CancellationToken cancellationToken)
    {
      UnbufferedODataBatchHandler odataBatchHandler = this;
      if (batchReader == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (batchReader));
      if (originalRequest == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (originalRequest));
      Guid changeSetId = Guid.NewGuid();
      List<HttpResponseMessage> changeSetResponse = new List<HttpResponseMessage>();
      Dictionary<string, string> contentIdToLocationMapping = new Dictionary<string, string>();
      try
      {
        while (batchReader.Read())
        {
          if (batchReader.State != ODataBatchReaderState.ChangesetEnd)
          {
            if (batchReader.State == ODataBatchReaderState.Operation)
            {
              HttpRequestMessage changeSetOperationRequest = await batchReader.ReadChangeSetOperationRequestAsync(batchId, changeSetId, false);
              changeSetOperationRequest.CopyBatchRequestProperties(originalRequest);
              changeSetOperationRequest.DeleteRequestContainer(false);
              try
              {
                HttpResponseMessage httpResponseMessage = await ODataBatchRequestItem.SendMessageAsync(odataBatchHandler.Invoker, changeSetOperationRequest, cancellationToken, contentIdToLocationMapping);
                if (httpResponseMessage.IsSuccessStatusCode)
                {
                  changeSetResponse.Add(httpResponseMessage);
                }
                else
                {
                  ChangeSetRequestItem.DisposeResponses(changeSetResponse);
                  changeSetResponse.Clear();
                  changeSetResponse.Add(httpResponseMessage);
                  return (ODataBatchResponseItem) new ChangeSetResponseItem((IEnumerable<HttpResponseMessage>) changeSetResponse);
                }
              }
              finally
              {
                originalRequest.RegisterForDispose(changeSetOperationRequest.GetResourcesForDisposal());
                originalRequest.RegisterForDispose((IDisposable) changeSetOperationRequest);
              }
              changeSetOperationRequest = (HttpRequestMessage) null;
            }
          }
          else
            break;
        }
      }
      catch
      {
        ChangeSetRequestItem.DisposeResponses(changeSetResponse);
        throw;
      }
      return (ODataBatchResponseItem) new ChangeSetResponseItem((IEnumerable<HttpResponseMessage>) changeSetResponse);
    }
  }
}
