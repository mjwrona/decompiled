// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Riff.Riff2Writer
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.TeamFoundation.Git.Server.Riff
{
  internal sealed class Riff2Writer : IDisposable
  {
    private readonly Stream m_stream;
    private readonly bool m_leaveOpen;
    private readonly BinaryWriter m_binaryWriter;
    private readonly Stack<Riff2Writer.ChunkInfo> m_startedChunks;
    private bool m_riffHeaderWritten;

    public Riff2Writer(Stream stream, bool leaveOpen)
    {
      ArgumentUtility.CheckForNull<Stream>(stream, nameof (stream));
      GitServerUtils.CheckIsLittleEndian();
      this.m_stream = stream;
      this.m_leaveOpen = leaveOpen;
      this.m_binaryWriter = new BinaryWriter(stream, GitEncodingUtil.SafeAscii, true);
      this.m_startedChunks = new Stack<Riff2Writer.ChunkInfo>();
    }

    public void BeginRiffChunk(uint formType, long dataLength)
    {
      ArgumentUtility.CheckForOutOfRange(dataLength, nameof (dataLength), 0L);
      this.DoBeginChunk(843467090U, checked (dataLength + 4L), true, true);
      this.m_binaryWriter.Write(formType);
      this.m_riffHeaderWritten = true;
    }

    public void BeginChunk(uint id, long dataLength)
    {
      ArgumentUtility.CheckForOutOfRange(dataLength, nameof (dataLength), 0L);
      this.DoBeginChunk(id, dataLength, false, false);
    }

    private void DoBeginChunk(uint id, long dataLength, bool isRiffChunk, bool allowSubChunks)
    {
      if (isRiffChunk && this.m_riffHeaderWritten)
        throw new InvalidOperationException("BeginRiffChunk already called.");
      if (!isRiffChunk && !this.m_riffHeaderWritten)
        throw new InvalidOperationException("BeginRiffChunk has not been called yet.");
      this.ThrowIfNoRoomForMoreChunk(dataLength);
      this.m_binaryWriter.Write(id);
      this.m_binaryWriter.Write(dataLength);
      this.m_startedChunks.Push(new Riff2Writer.ChunkInfo(this.m_stream.Position, dataLength, allowSubChunks));
    }

    public void EndChunk()
    {
      Riff2Writer.ChunkInfo chunkInfo = this.m_startedChunks.Pop();
      long offset = checked (this.m_stream.Position - chunkInfo.DataPosition);
      if (offset != chunkInfo.DataLength)
      {
        this.m_stream.Position = chunkInfo.DataPosition - 8L;
        this.m_binaryWriter.Write(offset);
        this.m_stream.Seek(offset, SeekOrigin.Current);
      }
      if (offset % 2L == 0L)
        return;
      this.m_stream.WriteByte((byte) 0);
    }

    public void WriteJunkChunkIfNeeded(int alignment)
    {
      ArgumentUtility.CheckForOutOfRange(alignment, nameof (alignment), 4);
      if (alignment % 2 != 0)
        throw new ArgumentOutOfRangeException(nameof (alignment), (object) alignment, string.Format("{0} is an odd number.", (object) alignment));
      int num1 = (int) ((this.m_stream.Position + 4L + 4L) % (long) alignment);
      if (num1 == 0)
        return;
      long num2 = (long) ((alignment + alignment - 4 - 4 - num1) % alignment);
      this.ThrowIfNoRoomForMoreChunk(num2);
      this.m_binaryWriter.Write(1263424842U);
      GitStreamUtil.SafeForwardSeek(this.m_stream, num2, true);
    }

    private void ThrowIfNoRoomForMoreChunk(long dataLength)
    {
      if (this.m_stream.Position > 9223372036854775799L - dataLength)
        throw new InvalidOperationException(string.Format("No more room for chunk of length {0}.", (object) dataLength));
    }

    public void Dispose()
    {
      this.m_binaryWriter.Dispose();
      if (this.m_leaveOpen)
        return;
      this.m_stream.Dispose();
    }

    private struct ChunkInfo
    {
      public readonly long DataPosition;
      public readonly long DataLength;
      public readonly bool AllowSubChunks;

      public ChunkInfo(long dataPosition, long dataLength, bool allowSubChunks)
      {
        this.DataPosition = dataPosition;
        this.DataLength = dataLength;
        this.AllowSubChunks = allowSubChunks;
      }
    }
  }
}
