// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.MultipartMixed.ODataMultipartMixedBatchInputContext
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using System;
using System.Threading.Tasks;

namespace Microsoft.OData.MultipartMixed
{
  internal sealed class ODataMultipartMixedBatchInputContext : ODataRawInputContext
  {
    private string batchBoundary;

    public ODataMultipartMixedBatchInputContext(
      ODataFormat format,
      ODataMessageInfo messageInfo,
      ODataMessageReaderSettings messageReaderSettings)
      : base(format, messageInfo, messageReaderSettings)
    {
      try
      {
        this.batchBoundary = ODataMultipartMixedBatchWriterUtils.GetBatchBoundaryFromMediaType(messageInfo.MediaType);
      }
      catch (Exception ex)
      {
        if (ExceptionUtils.IsCatchableExceptionType(ex))
          messageInfo.MessageStream.Dispose();
        throw;
      }
    }

    internal override ODataBatchReader CreateBatchReader() => this.CreateBatchReaderImplementation(true);

    internal override Task<ODataBatchReader> CreateBatchReaderAsync() => TaskUtils.GetTaskForSynchronousOperation<ODataBatchReader>((Func<ODataBatchReader>) (() => this.CreateBatchReaderImplementation(false)));

    private ODataBatchReader CreateBatchReaderImplementation(bool synchronous) => (ODataBatchReader) new ODataMultipartMixedBatchReader(this, this.batchBoundary, this.Encoding, synchronous);
  }
}
