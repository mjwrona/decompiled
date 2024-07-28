// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Batch.ODataBatchReaderExtensions
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using Microsoft.OData;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.AspNet.OData.Batch
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class ODataBatchReaderExtensions
  {
    public static Task<IList<HttpRequestMessage>> ReadChangeSetRequestAsync(
      this ODataBatchReader reader,
      Guid batchId)
    {
      return reader.ReadChangeSetRequestAsync(batchId, CancellationToken.None);
    }

    public static async Task<IList<HttpRequestMessage>> ReadChangeSetRequestAsync(
      this ODataBatchReader reader,
      Guid batchId,
      CancellationToken cancellationToken)
    {
      if (reader == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (reader));
      if (reader.State != ODataBatchReaderState.ChangesetStart)
        throw Microsoft.AspNet.OData.Common.Error.InvalidOperation(SRResources.InvalidBatchReaderState, (object) reader.State.ToString(), (object) ODataBatchReaderState.ChangesetStart.ToString());
      Guid changeSetId = Guid.NewGuid();
      List<HttpRequestMessage> requests = new List<HttpRequestMessage>();
      while (reader.Read() && reader.State != ODataBatchReaderState.ChangesetEnd)
      {
        if (reader.State == ODataBatchReaderState.Operation)
        {
          List<HttpRequestMessage> httpRequestMessageList = requests;
          httpRequestMessageList.Add(await ODataBatchReaderExtensions.ReadOperationInternalAsync(reader, batchId, new Guid?(changeSetId), cancellationToken));
          httpRequestMessageList = (List<HttpRequestMessage>) null;
        }
      }
      return (IList<HttpRequestMessage>) requests;
    }

    public static Task<HttpRequestMessage> ReadOperationRequestAsync(
      this ODataBatchReader reader,
      Guid batchId,
      bool bufferContentStream)
    {
      return reader.ReadOperationRequestAsync(batchId, bufferContentStream, CancellationToken.None);
    }

    public static Task<HttpRequestMessage> ReadOperationRequestAsync(
      this ODataBatchReader reader,
      Guid batchId,
      bool bufferContentStream,
      CancellationToken cancellationToken)
    {
      if (reader == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (reader));
      if (reader.State != ODataBatchReaderState.Operation)
      {
        string batchReaderState1 = SRResources.InvalidBatchReaderState;
        object[] objArray = new object[2];
        ODataBatchReaderState batchReaderState2 = reader.State;
        objArray[0] = (object) batchReaderState2.ToString();
        batchReaderState2 = ODataBatchReaderState.Operation;
        objArray[1] = (object) batchReaderState2.ToString();
        throw Microsoft.AspNet.OData.Common.Error.InvalidOperation(batchReaderState1, objArray);
      }
      return ODataBatchReaderExtensions.ReadOperationInternalAsync(reader, batchId, new Guid?(), cancellationToken, bufferContentStream);
    }

    public static Task<HttpRequestMessage> ReadChangeSetOperationRequestAsync(
      this ODataBatchReader reader,
      Guid batchId,
      Guid changeSetId,
      bool bufferContentStream)
    {
      return reader.ReadChangeSetOperationRequestAsync(batchId, changeSetId, bufferContentStream, CancellationToken.None);
    }

    public static Task<HttpRequestMessage> ReadChangeSetOperationRequestAsync(
      this ODataBatchReader reader,
      Guid batchId,
      Guid changeSetId,
      bool bufferContentStream,
      CancellationToken cancellationToken)
    {
      if (reader == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (reader));
      if (reader.State != ODataBatchReaderState.Operation)
      {
        string batchReaderState1 = SRResources.InvalidBatchReaderState;
        object[] objArray = new object[2];
        ODataBatchReaderState batchReaderState2 = reader.State;
        objArray[0] = (object) batchReaderState2.ToString();
        batchReaderState2 = ODataBatchReaderState.Operation;
        objArray[1] = (object) batchReaderState2.ToString();
        throw Microsoft.AspNet.OData.Common.Error.InvalidOperation(batchReaderState1, objArray);
      }
      return ODataBatchReaderExtensions.ReadOperationInternalAsync(reader, batchId, new Guid?(changeSetId), cancellationToken, bufferContentStream);
    }

    private static async Task<HttpRequestMessage> ReadOperationInternalAsync(
      ODataBatchReader reader,
      Guid batchId,
      Guid? changeSetId,
      CancellationToken cancellationToken,
      bool bufferContentStream = true)
    {
      ODataBatchOperationRequestMessage batchRequest = reader.CreateOperationRequestMessage();
      HttpRequestMessage request = new HttpRequestMessage();
      request.Method = new HttpMethod(batchRequest.Method);
      request.RequestUri = batchRequest.Url;
      if (bufferContentStream)
      {
        using (Stream stream = batchRequest.GetStream())
        {
          MemoryStream bufferedStream = new MemoryStream();
          await stream.CopyToAsync((Stream) bufferedStream, 81920, cancellationToken);
          bufferedStream.Position = 0L;
          request.Content = (HttpContent) new StreamContent((Stream) bufferedStream);
          bufferedStream = (MemoryStream) null;
        }
      }
      else
        request.Content = (HttpContent) new LazyStreamContent((Func<Stream>) (() => batchRequest.GetStream()));
      foreach (KeyValuePair<string, string> header in batchRequest.Headers)
      {
        string key = header.Key;
        string str = header.Value;
        if (!request.Headers.TryAddWithoutValidation(key, str))
          request.Content.Headers.TryAddWithoutValidation(key, str);
      }
      request.SetODataBatchId(batchId);
      request.SetODataContentId(batchRequest.ContentId);
      if (changeSetId.HasValue && changeSetId.HasValue)
        request.SetODataChangeSetId(changeSetId.Value);
      return request;
    }
  }
}
