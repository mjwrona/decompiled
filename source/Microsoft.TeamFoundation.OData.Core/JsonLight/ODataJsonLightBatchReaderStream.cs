// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.JsonLight.ODataJsonLightBatchReaderStream
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Json;
using System;

namespace Microsoft.OData.JsonLight
{
  internal sealed class ODataJsonLightBatchReaderStream : ODataBatchReaderStream
  {
    private readonly ODataJsonLightInputContext inputContext;

    internal ODataJsonLightBatchReaderStream(ODataJsonLightInputContext inputContext) => this.inputContext = inputContext;

    internal BufferingJsonReader JsonReader => this.inputContext.JsonReader;

    internal override int ReadWithDelimiter(byte[] userBuffer, int userBufferOffset, int count) => throw new NotImplementedException();

    internal override int ReadWithLength(byte[] userBuffer, int userBufferOffset, int count)
    {
      int count1 = count;
      while (count1 > 0)
      {
        if (this.BatchBuffer.NumberOfBytesInBuffer >= count1)
        {
          Buffer.BlockCopy((Array) this.BatchBuffer.Bytes, this.BatchBuffer.CurrentReadPosition, (Array) userBuffer, userBufferOffset, count1);
          this.BatchBuffer.SkipTo(this.BatchBuffer.CurrentReadPosition + count1);
          count1 = 0;
        }
        else
        {
          int numberOfBytesInBuffer = this.BatchBuffer.NumberOfBytesInBuffer;
          Buffer.BlockCopy((Array) this.BatchBuffer.Bytes, this.BatchBuffer.CurrentReadPosition, (Array) userBuffer, userBufferOffset, numberOfBytesInBuffer);
          count1 -= numberOfBytesInBuffer;
          userBufferOffset += numberOfBytesInBuffer;
          if (this.underlyingStreamExhausted)
            throw new ODataException(Strings.General_InternalError((object) InternalErrorCodes.ODataBatchReaderStreamBuffer_ReadWithLength));
          this.underlyingStreamExhausted = this.BatchBuffer.RefillFrom(this.inputContext.Stream, 8000);
        }
      }
      return count - count1;
    }
  }
}
