// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Riff.RiffUtil
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.IO;
using System.Linq;

namespace Microsoft.TeamFoundation.Git.Server.Riff
{
  internal static class RiffUtil
  {
    public static byte[] IdToBytes(string id)
    {
      ArgumentUtility.CheckForOutOfRange(id.Length, "id.Length", 4);
      return GitEncodingUtil.SafeAscii.GetBytes(id);
    }

    public static uint IdToUInt32(byte[] id)
    {
      ArgumentUtility.CheckForOutOfRange(id.Length, "id.Length", 4);
      GitServerUtils.CheckIsLittleEndian();
      return BitConverter.ToUInt32(id, 0);
    }

    public static uint IdToUInt32(string id) => RiffUtil.IdToUInt32(RiffUtil.IdToBytes(id));

    public static RiffChunk GetChunk(ILookup<uint, RiffChunk> chunks, uint chunkId)
    {
      RiffChunk result;
      if (!chunks.TryGetChunk(chunkId, out result))
        throw new InvalidDataException(string.Format("Missing chunk: 0x{0:x8}.", (object) chunkId));
      return result;
    }

    public static bool TryGetChunk(
      this ILookup<uint, RiffChunk> chunks,
      uint chunkId,
      out RiffChunk result)
    {
      result = chunks[chunkId].FirstOrDefault<RiffChunk>();
      return result != null;
    }

    internal static Sha1Id[] ReadSha1Ids(Stream stream)
    {
      if (stream.Length % 20L != 0L)
        throw new InvalidDataException(string.Format("stream.Length % 20 = {0}.", (object) (stream.Length % 20L)));
      int length = checked ((int) unchecked (stream.Length / 20L));
      stream.Position = 0L;
      Sha1Id[] sha1IdArray = new Sha1Id[length];
      for (int index = 0; index < length; ++index)
      {
        try
        {
          sha1IdArray[index] = Sha1Id.FromStream(stream);
        }
        catch (Sha1IdStreamReadException ex)
        {
          throw new InvalidDataException(ex.Message);
        }
      }
      return sha1IdArray;
    }

    internal static unsafe Sha1Id[] ReadSha1Ids(Stream stream, byte[] buf)
    {
      long num = stream.Length % 20L == 0L ? stream.Length : throw new InvalidDataException(string.Format("stream.Length % 20 = {0}.", (object) (stream.Length % 20L)));
      int length = checked ((int) unchecked (num / 20L));
      stream.Position = 0L;
      Sha1Id[] sha1IdArray = new Sha1Id[length];
      fixed (Sha1Id* sha1IdPtr = sha1IdArray)
        fixed (byte* source = buf)
        {
          byte* numPtr = (byte*) sha1IdPtr;
          int sourceBytesToCopy;
          for (long index = 0; index < num; index += (long) sourceBytesToCopy)
          {
            int count = checked ((int) Math.Min((long) buf.Length, num - index));
            sourceBytesToCopy = stream.Read(buf, 0, count);
            Buffer.MemoryCopy((void*) source, (void*) (numPtr + index), num - index, (long) sourceBytesToCopy);
          }
        }
      return sha1IdArray;
    }

    public static unsafe void WriteSha1Ids(Stream stream, Sha1Id[] ids, int count, byte[] buf)
    {
      ArgumentUtility.CheckForOutOfRange(count, nameof (count), 0, ids.Length);
      fixed (Sha1Id* sha1IdPtr = ids)
        fixed (byte* destination = buf)
        {
          byte* numPtr = (byte*) sha1IdPtr;
          long num1 = 0;
          long num2 = (long) checked (20 * count);
          while (num1 < num2)
          {
            int num3 = checked ((int) Math.Min((long) buf.Length, num2 - num1));
            Buffer.MemoryCopy((void*) checked (unchecked ((UIntPtr) numPtr) + unchecked ((UIntPtr) num1)), (void*) destination, (long) buf.Length, (long) num3);
            stream.Write(buf, 0, num3);
            checked { num1 += (long) num3; }
          }
        }
    }

    internal static byte[] ReadBytes(Stream stream, int maxCount)
    {
      if (stream.Length > (long) maxCount)
        throw new InvalidGitIndexException((Exception) new InvalidDataException(string.Format("stream.Length ({0}) > {1} ({2}).", (object) stream.Length, (object) nameof (maxCount), (object) maxCount)));
      stream.Position = 0L;
      byte[] buf = new byte[stream.Length];
      GitStreamUtil.ReadGreedy(stream, buf, 0, buf.Length);
      return buf;
    }

    internal static T[] ReadArray<T>(Stream stream, int sizeOfValue, byte[] buf) where T : struct
    {
      if (stream.Length % (long) sizeOfValue != 0L)
        throw new InvalidDataException(string.Format("stream.Length ({0}) is not a multiple of {1}.", (object) stream.Length, (object) sizeOfValue));
      stream.Position = 0L;
      int numValues = checked ((int) unchecked (stream.Length / (long) sizeOfValue));
      return GitStreamUtil.ReadArray<T>(stream, sizeOfValue, numValues, buf);
    }

    public static unsafe void FillByteArray(byte[] buffer, int intOffset, int value)
    {
      ArgumentUtility.CheckForOutOfRange(intOffset * 4, nameof (intOffset), 0, buffer.Length);
      fixed (byte* numPtr = buffer)
        *(int*) (numPtr + ((IntPtr) intOffset * 4).ToInt64()) = value;
    }

    public static class FieldLengths
    {
      public const int Id = 4;
      public const int DataLength = 4;
      public const int LongDataLength = 8;
      public const int Header = 8;
      public const int LongHeader = 12;
    }

    public static class ChunkIds
    {
      public const uint Riff = 1179011410;
      public const uint Riff2 = 843467090;
      public const uint List = 1414744396;
      public const uint Junk = 1263424842;
    }
  }
}
