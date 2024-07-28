// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.ODataBatchReaderStreamBuffer
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.OData
{
  internal sealed class ODataBatchReaderStreamBuffer
  {
    internal const int BufferLength = 8000;
    private const int MaxLineFeedLength = 2;
    private const int TwoDashesLength = 2;
    private readonly byte[] bytes = new byte[8000];
    private int currentReadPosition;
    private int numberOfBytesInBuffer;

    internal byte[] Bytes => this.bytes;

    internal int CurrentReadPosition => this.currentReadPosition;

    internal int NumberOfBytesInBuffer => this.numberOfBytesInBuffer;

    internal byte this[int index] => this.bytes[index];

    internal void SkipTo(int newPosition)
    {
      int num = newPosition - this.currentReadPosition;
      this.currentReadPosition = newPosition;
      this.numberOfBytesInBuffer -= num;
    }

    internal bool RefillFrom(Stream stream, int preserveFrom)
    {
      this.ShiftToBeginning(preserveFrom);
      int count = 8000 - this.numberOfBytesInBuffer;
      int num = stream.Read(this.bytes, this.numberOfBytesInBuffer, count);
      this.numberOfBytesInBuffer += num;
      return num == 0;
    }

    internal ODataBatchReaderStreamScanResult ScanForLineEnd(
      out int lineEndStartPosition,
      out int lineEndEndPosition)
    {
      return this.ScanForLineEnd(this.currentReadPosition, 8000, false, out lineEndStartPosition, out lineEndEndPosition, out bool _);
    }

    internal ODataBatchReaderStreamScanResult ScanForBoundary(
      IEnumerable<string> boundaries,
      int maxDataBytesToScan,
      out int boundaryStartPosition,
      out int boundaryEndPosition,
      out bool isEndBoundary,
      out bool isParentBoundary)
    {
      boundaryStartPosition = -1;
      boundaryEndPosition = -1;
      isEndBoundary = false;
      isParentBoundary = false;
      int scanStartIx = this.currentReadPosition;
      int lineEndStartPosition;
      int boundaryDelimiterStartPosition;
      while (true)
      {
        switch (this.ScanForBoundaryStart(scanStartIx, maxDataBytesToScan, out lineEndStartPosition, out boundaryDelimiterStartPosition))
        {
          case ODataBatchReaderStreamScanResult.NoMatch:
            goto label_2;
          case ODataBatchReaderStreamScanResult.PartialMatch:
            goto label_3;
          case ODataBatchReaderStreamScanResult.Match:
            isParentBoundary = false;
            foreach (string boundary in boundaries)
            {
              switch (this.MatchBoundary(lineEndStartPosition, boundaryDelimiterStartPosition, boundary, out boundaryStartPosition, out boundaryEndPosition, out isEndBoundary))
              {
                case ODataBatchReaderStreamScanResult.NoMatch:
                  boundaryStartPosition = -1;
                  boundaryEndPosition = -1;
                  isEndBoundary = false;
                  isParentBoundary = true;
                  continue;
                case ODataBatchReaderStreamScanResult.PartialMatch:
                  boundaryEndPosition = -1;
                  isEndBoundary = false;
                  return ODataBatchReaderStreamScanResult.PartialMatch;
                case ODataBatchReaderStreamScanResult.Match:
                  return ODataBatchReaderStreamScanResult.Match;
                default:
                  throw new ODataException(Strings.General_InternalError((object) InternalErrorCodes.ODataBatchReaderStreamBuffer_ScanForBoundary));
              }
            }
            scanStartIx = scanStartIx == boundaryDelimiterStartPosition ? boundaryDelimiterStartPosition + 1 : boundaryDelimiterStartPosition;
            continue;
          default:
            goto label_16;
        }
      }
label_2:
      return ODataBatchReaderStreamScanResult.NoMatch;
label_3:
      boundaryStartPosition = lineEndStartPosition < 0 ? boundaryDelimiterStartPosition : lineEndStartPosition;
      return ODataBatchReaderStreamScanResult.PartialMatch;
label_16:
      throw new ODataException(Strings.General_InternalError((object) InternalErrorCodes.ODataBatchReaderStreamBuffer_ScanForBoundary));
    }

    private ODataBatchReaderStreamScanResult ScanForBoundaryStart(
      int scanStartIx,
      int maxDataBytesToScan,
      out int lineEndStartPosition,
      out int boundaryDelimiterStartPosition)
    {
      int num = this.currentReadPosition + Math.Min(maxDataBytesToScan, this.numberOfBytesInBuffer) - 1;
      for (int index = scanStartIx; index <= num; ++index)
      {
        char ch = (char) this.bytes[index];
        switch (ch)
        {
          case '\n':
          case '\r':
            lineEndStartPosition = index;
            if (ch == '\r' && index == num && maxDataBytesToScan >= this.numberOfBytesInBuffer)
            {
              boundaryDelimiterStartPosition = index;
              return ODataBatchReaderStreamScanResult.PartialMatch;
            }
            boundaryDelimiterStartPosition = ch != '\r' || this.bytes[index + 1] != (byte) 10 ? index + 1 : index + 2;
            return ODataBatchReaderStreamScanResult.Match;
          case '-':
            lineEndStartPosition = -1;
            if (index == num && maxDataBytesToScan >= this.numberOfBytesInBuffer)
            {
              boundaryDelimiterStartPosition = index;
              return ODataBatchReaderStreamScanResult.PartialMatch;
            }
            if (this.bytes[index + 1] == (byte) 45)
            {
              boundaryDelimiterStartPosition = index;
              return ODataBatchReaderStreamScanResult.Match;
            }
            break;
        }
      }
      lineEndStartPosition = -1;
      boundaryDelimiterStartPosition = -1;
      return ODataBatchReaderStreamScanResult.NoMatch;
    }

    private ODataBatchReaderStreamScanResult ScanForLineEnd(
      int scanStartIx,
      int maxDataBytesToScan,
      bool allowLeadingWhitespaceOnly,
      out int lineEndStartPosition,
      out int lineEndEndPosition,
      out bool endOfBufferReached)
    {
      endOfBufferReached = false;
      int num = this.currentReadPosition + Math.Min(maxDataBytesToScan, this.numberOfBytesInBuffer) - 1;
      for (int index = scanStartIx; index <= num; ++index)
      {
        char c = (char) this.bytes[index];
        switch (c)
        {
          case '\n':
          case '\r':
            lineEndStartPosition = index;
            if (c == '\r' && index == num && maxDataBytesToScan >= this.numberOfBytesInBuffer)
            {
              lineEndEndPosition = -1;
              return ODataBatchReaderStreamScanResult.PartialMatch;
            }
            lineEndEndPosition = lineEndStartPosition;
            if (c == '\r' && this.bytes[index + 1] == (byte) 10)
              ++lineEndEndPosition;
            return ODataBatchReaderStreamScanResult.Match;
          default:
            if (allowLeadingWhitespaceOnly && !char.IsWhiteSpace(c))
            {
              lineEndStartPosition = -1;
              lineEndEndPosition = -1;
              return ODataBatchReaderStreamScanResult.NoMatch;
            }
            continue;
        }
      }
      endOfBufferReached = true;
      lineEndStartPosition = -1;
      lineEndEndPosition = -1;
      return ODataBatchReaderStreamScanResult.NoMatch;
    }

    private ODataBatchReaderStreamScanResult MatchBoundary(
      int lineEndStartPosition,
      int boundaryDelimiterStartPosition,
      string boundary,
      out int boundaryStartPosition,
      out int boundaryEndPosition,
      out bool isEndBoundary)
    {
      boundaryStartPosition = -1;
      boundaryEndPosition = -1;
      int val1 = this.currentReadPosition + this.numberOfBytesInBuffer - 1;
      int val2 = boundaryDelimiterStartPosition + 2 + boundary.Length + 2 - 1;
      bool flag;
      int matchLength;
      if (val1 < val2 + 2)
      {
        flag = true;
        matchLength = Math.Min(val1, val2) - boundaryDelimiterStartPosition + 1;
      }
      else
      {
        flag = false;
        matchLength = val2 - boundaryDelimiterStartPosition + 1;
      }
      if (this.MatchBoundary(boundary, boundaryDelimiterStartPosition, matchLength, out isEndBoundary))
      {
        boundaryStartPosition = lineEndStartPosition < 0 ? boundaryDelimiterStartPosition : lineEndStartPosition;
        if (flag)
        {
          isEndBoundary = false;
          return ODataBatchReaderStreamScanResult.PartialMatch;
        }
        boundaryEndPosition = boundaryDelimiterStartPosition + 2 + boundary.Length - 1;
        if (isEndBoundary)
          boundaryEndPosition += 2;
        int lineEndEndPosition;
        bool endOfBufferReached;
        switch (this.ScanForLineEnd(boundaryEndPosition + 1, int.MaxValue, true, out int _, out lineEndEndPosition, out endOfBufferReached))
        {
          case ODataBatchReaderStreamScanResult.NoMatch:
            if (endOfBufferReached)
            {
              if (boundaryStartPosition == 0)
                throw new ODataException(Strings.ODataBatchReaderStreamBuffer_BoundaryLineSecurityLimitReached((object) 8000));
              isEndBoundary = false;
              return ODataBatchReaderStreamScanResult.PartialMatch;
            }
            break;
          case ODataBatchReaderStreamScanResult.PartialMatch:
            if (boundaryStartPosition == 0)
              throw new ODataException(Strings.ODataBatchReaderStreamBuffer_BoundaryLineSecurityLimitReached((object) 8000));
            isEndBoundary = false;
            return ODataBatchReaderStreamScanResult.PartialMatch;
          case ODataBatchReaderStreamScanResult.Match:
            boundaryEndPosition = lineEndEndPosition;
            return ODataBatchReaderStreamScanResult.Match;
          default:
            throw new ODataException(Strings.General_InternalError((object) InternalErrorCodes.ODataBatchReaderStreamBuffer_ScanForBoundary));
        }
      }
      return ODataBatchReaderStreamScanResult.NoMatch;
    }

    private bool MatchBoundary(
      string boundary,
      int startIx,
      int matchLength,
      out bool isEndBoundary)
    {
      isEndBoundary = false;
      if (matchLength == 0)
        return true;
      int num = 0;
      int index1 = startIx;
      for (int index2 = -2; index2 < matchLength - 2; ++index2)
      {
        if (index2 < 0)
        {
          if (this.bytes[index1] != (byte) 45)
            return false;
        }
        else if (index2 < boundary.Length)
        {
          if ((int) this.bytes[index1] != (int) boundary[index2])
            return false;
        }
        else
        {
          if (this.bytes[index1] != (byte) 45)
            return true;
          ++num;
        }
        ++index1;
      }
      isEndBoundary = num == 2;
      return true;
    }

    private void ShiftToBeginning(int startIndex)
    {
      int count = this.currentReadPosition + this.numberOfBytesInBuffer - startIndex;
      this.currentReadPosition = 0;
      if (count <= 0)
      {
        this.numberOfBytesInBuffer = 0;
      }
      else
      {
        this.numberOfBytesInBuffer = count;
        Buffer.BlockCopy((Array) this.bytes, startIndex, (Array) this.bytes, 0, count);
      }
    }
  }
}
