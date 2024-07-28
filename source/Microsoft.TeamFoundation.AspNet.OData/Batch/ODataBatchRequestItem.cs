// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Batch.ODataBatchRequestItem
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
  public abstract class ODataBatchRequestItem : IDisposable
  {
    public static async Task<HttpResponseMessage> SendMessageAsync(
      HttpMessageInvoker invoker,
      HttpRequestMessage request,
      CancellationToken cancellationToken,
      Dictionary<string, string> contentIdToLocationMapping)
    {
      if (invoker == null)
        throw Error.ArgumentNull(nameof (invoker));
      if (request == null)
        throw Error.ArgumentNull(nameof (request));
      if (contentIdToLocationMapping != null)
      {
        request.RequestUri = new Uri(ContentIdHelpers.ResolveContentId(request.RequestUri.OriginalString, (IDictionary<string, string>) contentIdToLocationMapping));
        request.SetODataContentIdMapping((IDictionary<string, string>) contentIdToLocationMapping);
      }
      HttpResponseMessage response = await invoker.SendAsync(request, cancellationToken);
      string odataContentId = request.GetODataContentId();
      if (contentIdToLocationMapping != null && odataContentId != null)
        ODataBatchRequestItem.AddLocationHeaderToMapping(response, (IDictionary<string, string>) contentIdToLocationMapping, odataContentId);
      return response;
    }

    private static void AddLocationHeaderToMapping(
      HttpResponseMessage response,
      IDictionary<string, string> contentIdToLocationMapping,
      string contentId)
    {
      if (!(response.Headers.Location != (Uri) null))
        return;
      contentIdToLocationMapping.Add(contentId, response.Headers.Location.AbsoluteUri);
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    public abstract IEnumerable<IDisposable> GetResourcesForDisposal();

    public abstract Task<ODataBatchResponseItem> SendRequestAsync(
      HttpMessageInvoker invoker,
      CancellationToken cancellationToken);

    protected abstract void Dispose(bool disposing);
  }
}
