// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Batch.ODataBatchResponseItem
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.OData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.AspNet.OData.Batch
{
  public abstract class ODataBatchResponseItem : IDisposable
  {
    public static Task WriteMessageAsync(ODataBatchWriter writer, HttpResponseMessage response) => ODataBatchResponseItem.WriteMessageAsync(writer, response, CancellationToken.None);

    public static async Task WriteMessageAsync(
      ODataBatchWriter writer,
      HttpResponseMessage response,
      CancellationToken cancellationToken)
    {
      if (writer == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (writer));
      HttpRequestMessage request = response != null ? response.RequestMessage : throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (response));
      ODataBatchOperationResponseMessage operationResponseMessage = writer.CreateOperationResponseMessage(request != null ? request.GetODataContentId() : string.Empty);
      operationResponseMessage.StatusCode = (int) response.StatusCode;
      foreach (KeyValuePair<string, IEnumerable<string>> header in (HttpHeaders) response.Headers)
        operationResponseMessage.SetHeader(header.Key, string.Join(",", header.Value));
      if (response.Content == null)
        return;
      foreach (KeyValuePair<string, IEnumerable<string>> header in (HttpHeaders) response.Content.Headers)
        operationResponseMessage.SetHeader(header.Key, string.Join(",", header.Value));
      using (Stream stream = operationResponseMessage.GetStream())
      {
        cancellationToken.ThrowIfCancellationRequested();
        await response.Content.CopyToAsync(stream);
      }
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    public abstract Task WriteResponseAsync(
      ODataBatchWriter writer,
      CancellationToken cancellationToken);

    internal Task WriteResponseAsync(ODataBatchWriter writer) => this.WriteResponseAsync(writer, CancellationToken.None);

    internal virtual bool IsResponseSuccessful() => false;

    protected abstract void Dispose(bool disposing);
  }
}
