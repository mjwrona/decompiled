// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Batch.ODataBatchContent
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Microsoft.AspNet.OData.Batch
{
  public class ODataBatchContent : HttpContent
  {
    private IServiceProvider _requestContainer;
    private ODataMessageWriterSettings _writerSettings;

    public ODataBatchContent(
      IEnumerable<ODataBatchResponseItem> responses,
      IServiceProvider requestContainer)
      : this(responses, requestContainer, (MediaTypeHeaderValue) null)
    {
    }

    public ODataBatchContent(
      IEnumerable<ODataBatchResponseItem> responses,
      IServiceProvider requestContainer,
      MediaTypeHeaderValue contentType)
    {
      this.Initialize(responses, requestContainer);
      if (contentType == null)
        contentType = MediaTypeHeaderValue.Parse(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "multipart/mixed;boundary=batchresponse_{0}", new object[1]
        {
          (object) Guid.NewGuid()
        }));
      this.Headers.ContentType = contentType;
      this.Headers.TryAddWithoutValidation("OData-Version", ODataUtils.ODataVersionToString(this._writerSettings.Version.GetValueOrDefault()));
    }

    protected override Task SerializeToStreamAsync(Stream stream, TransportContext context) => this.WriteToResponseMessageAsync((IODataResponseMessage) ODataMessageWrapperHelper.Create(stream, this.Headers, this._requestContainer));

    protected override bool TryComputeLength(out long length)
    {
      length = -1L;
      return false;
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        foreach (ODataBatchResponseItem response in this.Responses)
          response?.Dispose();
      }
      base.Dispose(disposing);
    }

    private void Initialize(
      IEnumerable<ODataBatchResponseItem> responses,
      IServiceProvider requestContainer)
    {
      this.Responses = responses != null ? responses : throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (responses));
      this._requestContainer = requestContainer;
      this._writerSettings = ServiceProviderServiceExtensions.GetRequiredService<ODataMessageWriterSettings>(requestContainer);
    }

    public IEnumerable<ODataBatchResponseItem> Responses { get; private set; }

    private async Task WriteToResponseMessageAsync(IODataResponseMessage responseMessage)
    {
      ODataBatchWriter writer = new ODataMessageWriter(responseMessage, this._writerSettings).CreateODataBatchWriter();
      writer.WriteStartBatch();
      foreach (ODataBatchResponseItem response in this.Responses)
        await response.WriteResponseAsync(writer);
      writer.WriteEndBatch();
    }
  }
}
