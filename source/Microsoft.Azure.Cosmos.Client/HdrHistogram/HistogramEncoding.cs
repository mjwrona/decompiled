// Decompiled with JetBrains decompiler
// Type: HdrHistogram.HistogramEncoding
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using HdrHistogram.Encoding;
using HdrHistogram.Utilities;
using System;
using System.IO;
using System.IO.Compression;
using System.Reflection;

namespace HdrHistogram
{
  internal static class HistogramEncoding
  {
    private const int UncompressedDoubleHistogramEncodingCookie = 208802382;
    private const int CompressedDoubleHistogramEncodingCookie = 208802383;
    private const int EncodingCookieBaseV2 = 478450435;
    private const int EncodingCookieBaseV1 = 478450433;
    private const int EncodingCookieBaseV0 = 478450440;
    private const int CompressedEncodingCookieBaseV0 = 478450441;
    private const int CompressedEncodingCookieBaseV1 = 478450434;
    private const int CompressedEncodingCookieBaseV2 = 478450436;
    private const int EncodingHeaderSizeV0 = 32;
    private const int EncodingHeaderSizeV1 = 40;
    private const int EncodingHeaderSizeV2 = 40;
    private const int V2MaxWordSizeInBytes = 9;
    private const int Rfc1950HeaderLength = 2;
    private static readonly Type[] HistogramClassConstructorArgsTypes = new Type[3]
    {
      typeof (long),
      typeof (long),
      typeof (int)
    };

    public static HistogramBase DecodeFromCompressedByteBuffer(
      ByteBuffer buffer,
      long minBarForHighestTrackableValue)
    {
      int headerSize = HistogramEncoding.GetHeaderSize(buffer.GetInt());
      int num = buffer.GetInt();
      using (MemoryStream memoryStream = new MemoryStream(buffer.ToArray(), buffer.Position + 2, num - 2))
      {
        using (DeflateStream deflateStream = new DeflateStream((Stream) memoryStream, CompressionMode.Decompress, true))
        {
          ByteBuffer buffer1 = ByteBuffer.Allocate(headerSize);
          buffer1.ReadFrom((Stream) deflateStream, headerSize);
          return HistogramEncoding.DecodeFromByteBuffer(buffer1, minBarForHighestTrackableValue, deflateStream);
        }
      }
    }

    public static HistogramBase DecodeFromByteBuffer(
      ByteBuffer buffer,
      long minBarForHighestTrackableValue,
      DeflateStream decompressor = null)
    {
      IHeader header = HistogramEncoding.ReadHeader(buffer);
      int inBytesFromCookie = HistogramEncoding.GetWordSizeInBytesFromCookie(header.Cookie);
      HistogramBase histogramBase = HistogramEncoding.Create(HistogramEncoding.GetBestTypeForWordSize(inBytesFromCookie), header, minBarForHighestTrackableValue);
      int num = Math.Min(histogramBase.GetNeededByteBufferCapacity() - header.CapacityEstimateExcess, header.PayloadLengthInBytes);
      histogramBase.EstablishInternalTackingValues(histogramBase.FillCountsFromBuffer(HistogramEncoding.PayLoadSourceBuffer(buffer, decompressor, num, header), num, inBytesFromCookie));
      return histogramBase;
    }

    public static int EncodeIntoCompressedByteBuffer(
      this HistogramBase source,
      ByteBuffer targetBuffer)
    {
      ByteBuffer byteBuffer = ByteBuffer.Allocate(source.GetNeededByteBufferCapacity());
      source.Encode(byteBuffer, (IEncoder) HistogramEncoderV2.Instance);
      int position1 = targetBuffer.Position;
      targetBuffer.PutInt(HistogramEncoding.GetCompressedEncodingCookie());
      int position2 = targetBuffer.Position;
      targetBuffer.PutInt(0);
      int num1 = targetBuffer.CompressedCopy(byteBuffer, targetBuffer.Position);
      targetBuffer.PutInt(position2, num1);
      int num2 = num1 + 8;
      targetBuffer.Position = position1 + num2;
      return num2;
    }

    public static int GetEncodingCookie(this HistogramBase histogram) => 478450451;

    private static IHeader ReadHeader(ByteBuffer buffer)
    {
      int cookie = buffer.GetInt();
      int cookieBase = HistogramEncoding.GetCookieBase(cookie);
      int inBytesFromCookie = HistogramEncoding.GetWordSizeInBytesFromCookie(cookie);
      if (cookieBase == 478450435 || cookieBase == 478450433)
      {
        if (cookieBase == 478450435 && inBytesFromCookie != 9)
          throw new ArgumentException("The buffer does not contain a Histogram (no valid cookie found)");
        return (IHeader) new V1Header(cookie, buffer);
      }
      if (cookieBase == 478450440)
        return (IHeader) new V0Header(cookie, buffer);
      throw new NotSupportedException("The buffer does not contain a Histogram (no valid cookie found)");
    }

    private static HistogramBase Create(
      Type histogramType,
      IHeader header,
      long minBarForHighestTrackableValue)
    {
      ConstructorInfo constructor = TypeHelper.GetConstructor(histogramType, HistogramEncoding.HistogramClassConstructorArgsTypes);
      if (constructor == (ConstructorInfo) null)
        throw new ArgumentException("The target type does not have a supported constructor", nameof (histogramType));
      long num = Math.Max(header.HighestTrackableValue, minBarForHighestTrackableValue);
      try
      {
        return (HistogramBase) constructor.Invoke(new object[3]
        {
          (object) header.LowestTrackableUnitValue,
          (object) num,
          (object) header.NumberOfSignificantValueDigits
        });
      }
      catch (Exception ex)
      {
        throw new ArgumentException("Unable to create histogram of Type " + histogramType.Name + ": " + ex.Message, ex);
      }
    }

    private static ByteBuffer PayLoadSourceBuffer(
      ByteBuffer buffer,
      DeflateStream decompressor,
      int expectedCapacity,
      IHeader header)
    {
      ByteBuffer byteBuffer;
      if (decompressor == null)
      {
        byteBuffer = expectedCapacity <= buffer.Remaining() ? buffer : throw new ArgumentException("The buffer does not contain the full Histogram payload");
      }
      else
      {
        byteBuffer = ByteBuffer.Allocate(expectedCapacity);
        int num = byteBuffer.ReadFrom((Stream) decompressor, expectedCapacity);
        if (header.PayloadLengthInBytes != int.MaxValue && num < header.PayloadLengthInBytes)
          throw new ArgumentException("The buffer does not contain the indicated payload amount");
      }
      return byteBuffer;
    }

    private static int GetHeaderSize(int cookie)
    {
      switch (HistogramEncoding.GetCookieBase(cookie))
      {
        case 478450434:
          return 40;
        case 478450436:
          return 40;
        case 478450441:
          return 32;
        default:
          throw new ArgumentException("The buffer does not contain a compressed Histogram");
      }
    }

    private static int GetCookieBase(int cookie) => cookie & -241;

    private static int GetCompressedEncodingCookie() => 478450452;

    private static int GetWordSizeInBytesFromCookie(int cookie)
    {
      switch (HistogramEncoding.GetCookieBase(cookie))
      {
        case 478450435:
        case 478450436:
          return 9;
        default:
          return (cookie & 240) >> 4 & 14;
      }
    }

    private static Type GetBestTypeForWordSize(int wordSizeInBytes)
    {
      switch (wordSizeInBytes)
      {
        case 2:
          return typeof (ShortHistogram);
        case 4:
          return typeof (IntHistogram);
        case 8:
          return typeof (LongHistogram);
        case 9:
          return typeof (LongHistogram);
        default:
          throw new IndexOutOfRangeException();
      }
    }
  }
}
