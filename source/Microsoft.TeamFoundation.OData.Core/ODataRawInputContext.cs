// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.ODataRawInputContext
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData
{
  internal class ODataRawInputContext : ODataInputContext
  {
    protected readonly Encoding Encoding;
    private const int BufferSize = 4096;
    private readonly ODataPayloadKind readerPayloadKind;
    private Stream stream;
    private TextReader textReader;

    public ODataRawInputContext(
      ODataFormat format,
      ODataMessageInfo messageInfo,
      ODataMessageReaderSettings messageReaderSettings)
      : base(format, messageInfo, messageReaderSettings)
    {
      try
      {
        this.stream = messageInfo.MessageStream;
        this.Encoding = messageInfo.Encoding;
        this.readerPayloadKind = messageInfo.PayloadKind;
      }
      catch (Exception ex)
      {
        if (ExceptionUtils.IsCatchableExceptionType(ex))
          messageInfo.MessageStream.Dispose();
        throw;
      }
    }

    public Stream Stream => this.stream;

    internal override ODataAsynchronousReader CreateAsynchronousReader() => this.CreateAsynchronousReaderImplementation();

    internal override Task<ODataAsynchronousReader> CreateAsynchronousReaderAsync() => TaskUtils.GetTaskForSynchronousOperation<ODataAsynchronousReader>((Func<ODataAsynchronousReader>) (() => this.CreateAsynchronousReaderImplementation()));

    internal override object ReadValue(
      IEdmPrimitiveTypeReference expectedPrimitiveTypeReference)
    {
      return this.ReadValueImplementation(expectedPrimitiveTypeReference);
    }

    internal override Task<object> ReadValueAsync(
      IEdmPrimitiveTypeReference expectedPrimitiveTypeReference)
    {
      return TaskUtils.GetTaskForSynchronousOperation<object>((Func<object>) (() => this.ReadValueImplementation(expectedPrimitiveTypeReference)));
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        try
        {
          if (this.textReader != null)
            this.textReader.Dispose();
          else if (this.stream != null)
            this.stream.Dispose();
        }
        finally
        {
          this.textReader = (TextReader) null;
          this.stream = (Stream) null;
        }
      }
      base.Dispose(disposing);
    }

    private ODataAsynchronousReader CreateAsynchronousReaderImplementation() => new ODataAsynchronousReader(this, this.Encoding);

    private object ReadValueImplementation(
      IEdmPrimitiveTypeReference expectedPrimitiveTypeReference)
    {
      if (expectedPrimitiveTypeReference != null ? ExtensionMethods.PrimitiveKind(expectedPrimitiveTypeReference) == EdmPrimitiveTypeKind.Binary || ExtensionMethods.PrimitiveKind(expectedPrimitiveTypeReference) == EdmPrimitiveTypeKind.Stream : this.readerPayloadKind == ODataPayloadKind.BinaryValue)
        return (object) this.ReadBinaryValue();
      this.textReader = this.Encoding == null ? (TextReader) new StreamReader(this.stream) : (TextReader) new StreamReader(this.stream, this.Encoding);
      return this.ReadRawValue(expectedPrimitiveTypeReference);
    }

    private byte[] ReadBinaryValue()
    {
      long length = 0;
      List<byte[]> numArrayList = new List<byte[]>();
      byte[] buffer;
      int count;
      do
      {
        buffer = new byte[4096];
        count = this.stream.Read(buffer, 0, buffer.Length);
        length += (long) count;
        numArrayList.Add(buffer);
      }
      while (count == buffer.Length);
      byte[] dst = new byte[length];
      for (int index = 0; index < numArrayList.Count - 1; ++index)
        Buffer.BlockCopy((Array) numArrayList[index], 0, (Array) dst, index * 4096, 4096);
      Buffer.BlockCopy((Array) numArrayList[numArrayList.Count - 1], 0, (Array) dst, (numArrayList.Count - 1) * 4096, count);
      return dst;
    }

    private object ReadRawValue(
      IEdmPrimitiveTypeReference expectedPrimitiveTypeReference)
    {
      string end = this.textReader.ReadToEnd();
      return expectedPrimitiveTypeReference == null || !this.MessageReaderSettings.EnablePrimitiveTypeConversion ? (object) end : ODataRawValueUtils.ConvertStringToPrimitive(end, expectedPrimitiveTypeReference);
    }
  }
}
