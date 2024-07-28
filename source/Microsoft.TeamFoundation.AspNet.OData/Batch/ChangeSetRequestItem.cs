// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Batch.ChangeSetRequestItem
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.AspNet.OData.Batch
{
  public class ChangeSetRequestItem : ODataBatchRequestItem
  {
    public ChangeSetRequestItem(IEnumerable<HttpRequestMessage> requests) => this.Requests = requests != null ? requests : throw Error.ArgumentNull(nameof (requests));

    public IEnumerable<HttpRequestMessage> Requests { get; private set; }

    public override async Task<ODataBatchResponseItem> SendRequestAsync(
      HttpMessageInvoker invoker,
      CancellationToken cancellationToken)
    {
      if (invoker == null)
        throw Error.ArgumentNull(nameof (invoker));
      Dictionary<string, string> contentIdToLocationMapping = new Dictionary<string, string>();
      List<HttpResponseMessage> responses = new List<HttpResponseMessage>();
      try
      {
        foreach (HttpRequestMessage request in this.Requests)
        {
          HttpResponseMessage httpResponseMessage = await ODataBatchRequestItem.SendMessageAsync(invoker, request, cancellationToken, contentIdToLocationMapping);
          if (httpResponseMessage.IsSuccessStatusCode)
          {
            responses.Add(httpResponseMessage);
          }
          else
          {
            ChangeSetRequestItem.DisposeResponses(responses);
            responses.Clear();
            responses.Add(httpResponseMessage);
            return (ODataBatchResponseItem) new ChangeSetResponseItem((IEnumerable<HttpResponseMessage>) responses);
          }
        }
      }
      catch
      {
        ChangeSetRequestItem.DisposeResponses(responses);
        throw;
      }
      return (ODataBatchResponseItem) new ChangeSetResponseItem((IEnumerable<HttpResponseMessage>) responses);
    }

    public override IEnumerable<IDisposable> GetResourcesForDisposal()
    {
      List<IDisposable> resourcesForDisposal = new List<IDisposable>();
      foreach (HttpRequestMessage request in this.Requests)
      {
        if (request != null)
          resourcesForDisposal.AddRange(request.GetResourcesForDisposal());
      }
      return (IEnumerable<IDisposable>) resourcesForDisposal;
    }

    protected override void Dispose(bool disposing)
    {
      if (!disposing)
        return;
      foreach (HttpRequestMessage request in this.Requests)
        request?.Dispose();
    }

    internal static void DisposeResponses(List<HttpResponseMessage> responses)
    {
      foreach (HttpResponseMessage response in responses)
        response?.Dispose();
    }
  }
}
