// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.MultipartMixed.ODataMultipartMixedBatchReaderStream
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Microsoft.OData.MultipartMixed
{
  internal sealed class ODataMultipartMixedBatchReaderStream : ODataBatchReaderStream
  {
    private const int LineBufferLength = 2000;
    private readonly byte[] lineBuffer;
    private readonly string batchBoundary;
    private readonly ODataMediaTypeResolver mediaTypeResolver;
    private Encoding batchEncoding;
    private Encoding changesetEncoding;
    private string changesetBoundary;
    private ODataMultipartMixedBatchInputContext multipartMixedBatchInputContext;

    internal ODataMultipartMixedBatchReaderStream(
      ODataMultipartMixedBatchInputContext inputContext,
      string batchBoundary,
      Encoding batchEncoding)
    {
      this.batchEncoding = batchEncoding;
      this.multipartMixedBatchInputContext = inputContext;
      this.batchBoundary = batchBoundary;
      this.lineBuffer = new byte[2000];
      this.mediaTypeResolver = ODataMediaTypeResolver.GetMediaTypeResolver(inputContext.Container);
    }

    internal string BatchBoundary => this.batchBoundary;

    internal string ChangeSetBoundary => this.changesetBoundary;

    private Encoding CurrentEncoding => this.changesetEncoding ?? this.batchEncoding;

    private IEnumerable<string> CurrentBoundaries
    {
      get
      {
        if (this.changesetBoundary != null)
          yield return this.changesetBoundary;
        yield return this.batchBoundary;
      }
    }

    internal void ResetChangeSetBoundary()
    {
      this.changesetBoundary = (string) null;
      this.changesetEncoding = (Encoding) null;
    }

    internal bool SkipToBoundary(out bool isEndBoundary, out bool isParentBoundary)
    {
      this.EnsureBatchEncoding(this.multipartMixedBatchInputContext.Stream);
      ODataBatchReaderStreamScanResult streamScanResult = ODataBatchReaderStreamScanResult.NoMatch;
      while (streamScanResult != ODataBatchReaderStreamScanResult.Match)
      {
        int boundaryStartPosition;
        int boundaryEndPosition;
        streamScanResult = this.BatchBuffer.ScanForBoundary(this.CurrentBoundaries, int.MaxValue, out boundaryStartPosition, out boundaryEndPosition, out isEndBoundary, out isParentBoundary);
        switch (streamScanResult)
        {
          case ODataBatchReaderStreamScanResult.NoMatch:
            if (this.underlyingStreamExhausted)
            {
              this.BatchBuffer.SkipTo(this.BatchBuffer.CurrentReadPosition + this.BatchBuffer.NumberOfBytesInBuffer);
              return false;
            }
            this.underlyingStreamExhausted = this.BatchBuffer.RefillFrom(this.multipartMixedBatchInputContext.Stream, 8000);
            continue;
          case ODataBatchReaderStreamScanResult.PartialMatch:
            if (this.underlyingStreamExhausted)
            {
              this.BatchBuffer.SkipTo(this.BatchBuffer.CurrentReadPosition + this.BatchBuffer.NumberOfBytesInBuffer);
              return false;
            }
            this.underlyingStreamExhausted = this.BatchBuffer.RefillFrom(this.multipartMixedBatchInputContext.Stream, boundaryStartPosition);
            continue;
          case ODataBatchReaderStreamScanResult.Match:
            this.BatchBuffer.SkipTo(isParentBoundary ? boundaryStartPosition : boundaryEndPosition + 1);
            return true;
          default:
            throw new ODataException(Strings.General_InternalError((object) InternalErrorCodes.ODataBatchReaderStream_SkipToBoundary));
        }
      }
      throw new ODataException(Strings.General_InternalError((object) InternalErrorCodes.ODataBatchReaderStream_SkipToBoundary));
    }

    internal override int ReadWithDelimiter(byte[] userBuffer, int userBufferOffset, int count)
    {
      if (count == 0)
        return 0;
      int num1 = count;
      ODataBatchReaderStreamScanResult streamScanResult = ODataBatchReaderStreamScanResult.NoMatch;
      while (num1 > 0 && streamScanResult != ODataBatchReaderStreamScanResult.Match)
      {
        int boundaryStartPosition;
        streamScanResult = this.BatchBuffer.ScanForBoundary(this.CurrentBoundaries, num1, out boundaryStartPosition, out int _, out bool _, out bool _);
        switch (streamScanResult)
        {
          case ODataBatchReaderStreamScanResult.NoMatch:
            if (this.BatchBuffer.NumberOfBytesInBuffer >= num1)
            {
              Buffer.BlockCopy((Array) this.BatchBuffer.Bytes, this.BatchBuffer.CurrentReadPosition, (Array) userBuffer, userBufferOffset, num1);
              this.BatchBuffer.SkipTo(this.BatchBuffer.CurrentReadPosition + num1);
              return count;
            }
            int numberOfBytesInBuffer = this.BatchBuffer.NumberOfBytesInBuffer;
            Buffer.BlockCopy((Array) this.BatchBuffer.Bytes, this.BatchBuffer.CurrentReadPosition, (Array) userBuffer, userBufferOffset, numberOfBytesInBuffer);
            num1 -= numberOfBytesInBuffer;
            userBufferOffset += numberOfBytesInBuffer;
            if (this.underlyingStreamExhausted)
            {
              this.BatchBuffer.SkipTo(this.BatchBuffer.CurrentReadPosition + numberOfBytesInBuffer);
              return count - num1;
            }
            this.underlyingStreamExhausted = this.BatchBuffer.RefillFrom(this.multipartMixedBatchInputContext.Stream, 8000);
            continue;
          case ODataBatchReaderStreamScanResult.PartialMatch:
            if (this.underlyingStreamExhausted)
            {
              int count1 = Math.Min(this.BatchBuffer.NumberOfBytesInBuffer, num1);
              Buffer.BlockCopy((Array) this.BatchBuffer.Bytes, this.BatchBuffer.CurrentReadPosition, (Array) userBuffer, userBufferOffset, count1);
              this.BatchBuffer.SkipTo(this.BatchBuffer.CurrentReadPosition + count1);
              int num2 = num1 - count1;
              return count - num2;
            }
            int count2 = boundaryStartPosition - this.BatchBuffer.CurrentReadPosition;
            Buffer.BlockCopy((Array) this.BatchBuffer.Bytes, this.BatchBuffer.CurrentReadPosition, (Array) userBuffer, userBufferOffset, count2);
            num1 -= count2;
            userBufferOffset += count2;
            this.underlyingStreamExhausted = this.BatchBuffer.RefillFrom(this.multipartMixedBatchInputContext.Stream, boundaryStartPosition);
            continue;
          case ODataBatchReaderStreamScanResult.Match:
            int count3 = boundaryStartPosition - this.BatchBuffer.CurrentReadPosition;
            Buffer.BlockCopy((Array) this.BatchBuffer.Bytes, this.BatchBuffer.CurrentReadPosition, (Array) userBuffer, userBufferOffset, count3);
            int num3 = num1 - count3;
            userBufferOffset += count3;
            this.BatchBuffer.SkipTo(boundaryStartPosition);
            return count - num3;
          default:
            continue;
        }
      }
      throw new ODataException(Strings.General_InternalError((object) InternalErrorCodes.ODataBatchReaderStream_ReadWithDelimiter));
    }

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
          this.underlyingStreamExhausted = this.BatchBuffer.RefillFrom(this.multipartMixedBatchInputContext.Stream, 8000);
        }
      }
      return count - count1;
    }

    internal bool ProcessPartHeader(out string contentId)
    {
      bool isChangeSetPart;
      ODataBatchOperationHeaders operationHeaders = this.ReadPartHeaders(out isChangeSetPart);
      contentId = (string) null;
      if (isChangeSetPart)
      {
        this.DetermineChangesetBoundaryAndEncoding(operationHeaders["Content-Type"]);
        if (this.changesetEncoding == null)
          this.changesetEncoding = this.DetectEncoding(this.multipartMixedBatchInputContext.Stream);
        ReaderValidationUtils.ValidateEncodingSupportedInBatch(this.changesetEncoding);
      }
      else
        operationHeaders.TryGetValue("Content-ID", out contentId);
      return isChangeSetPart;
    }

    internal ODataBatchOperationHeaders ReadHeaders()
    {
      ODataBatchOperationHeaders operationHeaders = new ODataBatchOperationHeaders();
      for (string headerLine = this.ReadLine(); headerLine != null && headerLine.Length > 0; headerLine = this.ReadLine())
      {
        string headerName;
        string headerValue;
        ODataMultipartMixedBatchReaderStream.ValidateHeaderLine(headerLine, out headerName, out headerValue);
        if (operationHeaders.ContainsKeyOrdinal(headerName))
          throw new ODataException(Strings.ODataBatchReaderStream_DuplicateHeaderFound((object) headerName));
        operationHeaders.Add(headerName, headerValue);
      }
      return operationHeaders;
    }

    internal string ReadFirstNonEmptyLine()
    {
      string str;
      do
      {
        str = this.ReadLine();
        if (str == null)
          throw new ODataException(Strings.ODataBatchReaderStream_UnexpectedEndOfInput);
      }
      while (str.Length == 0);
      return str;
    }

    private static void ValidateHeaderLine(
      string headerLine,
      out string headerName,
      out string headerValue)
    {
      int length = headerLine.IndexOf(':');
      headerName = length > 0 ? headerLine.Substring(0, length).Trim() : throw new ODataException(Strings.ODataBatchReaderStream_InvalidHeaderSpecified((object) headerLine));
      headerValue = headerLine.Substring(length + 1).Trim();
    }

    private string ReadLine()
    {
      int num1 = 0;
      byte[] lineBuffer = this.lineBuffer;
      ODataBatchReaderStreamScanResult streamScanResult = ODataBatchReaderStreamScanResult.NoMatch;
      while (streamScanResult != ODataBatchReaderStreamScanResult.Match)
      {
        int lineEndStartPosition;
        int lineEndEndPosition;
        streamScanResult = this.BatchBuffer.ScanForLineEnd(out lineEndStartPosition, out lineEndEndPosition);
        switch (streamScanResult)
        {
          case ODataBatchReaderStreamScanResult.NoMatch:
            int numberOfBytesInBuffer = this.BatchBuffer.NumberOfBytesInBuffer;
            if (numberOfBytesInBuffer > 0)
            {
              ODataBatchUtils.EnsureArraySize(ref lineBuffer, num1, numberOfBytesInBuffer);
              Buffer.BlockCopy((Array) this.BatchBuffer.Bytes, this.BatchBuffer.CurrentReadPosition, (Array) lineBuffer, num1, numberOfBytesInBuffer);
              num1 += numberOfBytesInBuffer;
            }
            if (this.underlyingStreamExhausted)
            {
              if (num1 == 0)
                return (string) null;
              streamScanResult = ODataBatchReaderStreamScanResult.Match;
              this.BatchBuffer.SkipTo(this.BatchBuffer.CurrentReadPosition + numberOfBytesInBuffer);
              continue;
            }
            this.underlyingStreamExhausted = this.BatchBuffer.RefillFrom(this.multipartMixedBatchInputContext.Stream, 8000);
            continue;
          case ODataBatchReaderStreamScanResult.PartialMatch:
            int num2 = lineEndStartPosition - this.BatchBuffer.CurrentReadPosition;
            if (num2 > 0)
            {
              ODataBatchUtils.EnsureArraySize(ref lineBuffer, num1, num2);
              Buffer.BlockCopy((Array) this.BatchBuffer.Bytes, this.BatchBuffer.CurrentReadPosition, (Array) lineBuffer, num1, num2);
              num1 += num2;
            }
            if (this.underlyingStreamExhausted)
            {
              streamScanResult = ODataBatchReaderStreamScanResult.Match;
              this.BatchBuffer.SkipTo(lineEndStartPosition + 1);
              continue;
            }
            this.underlyingStreamExhausted = this.BatchBuffer.RefillFrom(this.multipartMixedBatchInputContext.Stream, lineEndStartPosition);
            continue;
          case ODataBatchReaderStreamScanResult.Match:
            int num3 = lineEndStartPosition - this.BatchBuffer.CurrentReadPosition;
            if (num3 > 0)
            {
              ODataBatchUtils.EnsureArraySize(ref lineBuffer, num1, num3);
              Buffer.BlockCopy((Array) this.BatchBuffer.Bytes, this.BatchBuffer.CurrentReadPosition, (Array) lineBuffer, num1, num3);
              num1 += num3;
            }
            this.BatchBuffer.SkipTo(lineEndEndPosition + 1);
            continue;
          default:
            throw new ODataException(Strings.General_InternalError((object) InternalErrorCodes.ODataBatchReaderStream_ReadLine));
        }
      }
      return this.CurrentEncoding.GetString(lineBuffer, 0, num1);
    }

    private ODataBatchOperationHeaders ReadPartHeaders(out bool isChangeSetPart) => this.ValidatePartHeaders(this.ReadHeaders(), out isChangeSetPart);

    private ODataBatchOperationHeaders ValidatePartHeaders(
      ODataBatchOperationHeaders headers,
      out bool isChangeSetPart)
    {
      string str;
      if (!headers.TryGetValue("Content-Type", out str))
        throw new ODataException(Strings.ODataBatchReaderStream_MissingContentTypeHeader);
      if (MediaTypeUtils.MediaTypeAndSubtypeAreEqual(str, "application/http"))
      {
        isChangeSetPart = false;
        string strA;
        if (!headers.TryGetValue("Content-Transfer-Encoding", out strA) || string.Compare(strA, "binary", StringComparison.OrdinalIgnoreCase) != 0)
          throw new ODataException(Strings.ODataBatchReaderStream_MissingOrInvalidContentEncodingHeader((object) "Content-Transfer-Encoding", (object) "binary"));
      }
      else
      {
        if (!MediaTypeUtils.MediaTypeStartsWithTypeAndSubtype(str, "multipart/mixed"))
          throw new ODataException(Strings.ODataBatchReaderStream_InvalidContentTypeSpecified((object) "Content-Type", (object) str, (object) "multipart/mixed", (object) "application/http"));
        isChangeSetPart = true;
        if (this.changesetBoundary != null)
          throw new ODataException(Strings.ODataBatchReaderStream_NestedChangesetsAreNotSupported);
      }
      return headers;
    }

    private void DetermineChangesetBoundaryAndEncoding(string contentType)
    {
      ODataMediaType mediaType;
      MediaTypeUtils.GetFormatFromContentType(contentType, new ODataPayloadKind[1]
      {
        ODataPayloadKind.Batch
      }, this.mediaTypeResolver, out mediaType, out this.changesetEncoding, out ODataPayloadKind _);
      this.changesetBoundary = ODataMultipartMixedBatchWriterUtils.GetBatchBoundaryFromMediaType(mediaType);
    }

    private void EnsureBatchEncoding(Stream stream)
    {
      if (this.batchEncoding == null)
        this.batchEncoding = this.DetectEncoding(stream);
      ReaderValidationUtils.ValidateEncodingSupportedInBatch(this.batchEncoding);
    }

    private Encoding DetectEncoding(Stream stream)
    {
      while (!this.underlyingStreamExhausted && this.BatchBuffer.NumberOfBytesInBuffer < 4)
        this.underlyingStreamExhausted = this.BatchBuffer.RefillFrom(stream, this.BatchBuffer.CurrentReadPosition);
      int numberOfBytesInBuffer = this.BatchBuffer.NumberOfBytesInBuffer;
      if (numberOfBytesInBuffer < 2)
        return MediaTypeUtils.FallbackEncoding;
      if (this.BatchBuffer[this.BatchBuffer.CurrentReadPosition] == (byte) 254 && this.BatchBuffer[this.BatchBuffer.CurrentReadPosition + 1] == byte.MaxValue)
        return (Encoding) new UnicodeEncoding(true, true);
      if (this.BatchBuffer[this.BatchBuffer.CurrentReadPosition] == byte.MaxValue && this.BatchBuffer[this.BatchBuffer.CurrentReadPosition + 1] == (byte) 254)
      {
        if (numberOfBytesInBuffer >= 4 && this.BatchBuffer[this.BatchBuffer.CurrentReadPosition + 2] == (byte) 0 && this.BatchBuffer[this.BatchBuffer.CurrentReadPosition + 3] == (byte) 0)
          throw Error.NotSupported();
        return (Encoding) new UnicodeEncoding(false, true);
      }
      if (numberOfBytesInBuffer >= 3 && this.BatchBuffer[this.BatchBuffer.CurrentReadPosition] == (byte) 239 && this.BatchBuffer[this.BatchBuffer.CurrentReadPosition + 1] == (byte) 187 && this.BatchBuffer[this.BatchBuffer.CurrentReadPosition + 2] == (byte) 191)
        return Encoding.UTF8;
      if (numberOfBytesInBuffer >= 4 && this.BatchBuffer[this.BatchBuffer.CurrentReadPosition] == (byte) 0 && this.BatchBuffer[this.BatchBuffer.CurrentReadPosition + 1] == (byte) 0 && this.BatchBuffer[this.BatchBuffer.CurrentReadPosition + 2] == (byte) 254 && this.BatchBuffer[this.BatchBuffer.CurrentReadPosition + 3] == byte.MaxValue)
        throw Error.NotSupported();
      return MediaTypeUtils.FallbackEncoding;
    }
  }
}
