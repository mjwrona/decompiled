// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.MultipartMixed.ODataMultipartMixedBatchOutputContext
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using System;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.MultipartMixed
{
  internal sealed class ODataMultipartMixedBatchOutputContext : ODataRawOutputContext
  {
    private readonly string batchBoundary;

    internal ODataMultipartMixedBatchOutputContext(
      ODataFormat format,
      ODataMessageInfo messageInfo,
      ODataMessageWriterSettings messageWriterSettings)
      : base(format, messageInfo, messageWriterSettings)
    {
      this.batchBoundary = ODataMultipartMixedBatchWriterUtils.GetBatchBoundaryFromMediaType(messageInfo.MediaType);
    }

    internal override ODataBatchWriter CreateODataBatchWriter() => this.CreateODataBatchWriterImplementation();

    internal override Task<ODataBatchWriter> CreateODataBatchWriterAsync() => TaskUtils.GetTaskForSynchronousOperation<ODataBatchWriter>((Func<ODataBatchWriter>) (() => this.CreateODataBatchWriterImplementation()));

    private ODataBatchWriter CreateODataBatchWriterImplementation()
    {
      this.encoding = this.encoding ?? (Encoding) MediaTypeUtils.EncodingUtf8NoPreamble;
      ODataBatchWriter writerImplementation = (ODataBatchWriter) new ODataMultipartMixedBatchWriter(this, this.batchBoundary);
      this.outputInStreamErrorListener = (IODataOutputInStreamErrorListener) writerImplementation;
      return writerImplementation;
    }
  }
}
