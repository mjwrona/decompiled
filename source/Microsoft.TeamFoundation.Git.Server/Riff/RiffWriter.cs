// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Riff.RiffWriter
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.TeamFoundation.Git.Server.Riff
{
  internal sealed class RiffWriter : IDisposable
  {
    private readonly Stream m_stream;
    private readonly bool m_leaveOpen;
    private readonly BinaryWriter m_binaryWriter;
    private readonly Stack<RiffWriter.ChunkInfo> m_startedChunks;
    private bool m_riffHeaderWritten;

    public RiffWriter(Stream stream, bool leaveOpen)
    {
      ArgumentUtility.CheckForNull<Stream>(stream, nameof (stream));
      GitServerUtils.CheckIsLittleEndian();
      this.m_stream = stream;
      this.m_leaveOpen = leaveOpen;
      this.m_binaryWriter = new BinaryWriter(stream, GitEncodingUtil.SafeAscii, true);
      this.m_startedChunks = new Stack<RiffWriter.ChunkInfo>();
    }

    public void BeginRiffChunk(uint formType, uint dataLength)
    {
      ArgumentUtility.CheckForOutOfRange((long) dataLength, nameof (dataLength), 0L);
      this.DoBeginChunk(1179011410U, checked (dataLength + 4U), true, true);
      this.m_binaryWriter.Write(formType);
      this.m_riffHeaderWritten = true;
    }

    public void BeginListChunk(uint listType, uint dataLength)
    {
      ArgumentUtility.CheckForOutOfRange((long) dataLength, nameof (dataLength), 0L);
      this.DoBeginChunk(1414744396U, checked (dataLength + 4U), false, true);
      this.m_binaryWriter.Write(listType);
    }

    public void BeginChunk(uint id, uint dataLength)
    {
      ArgumentUtility.CheckForOutOfRange((long) dataLength, nameof (dataLength), 0L);
      this.DoBeginChunk(id, dataLength, false, false);
    }

    private void DoBeginChunk(uint id, uint dataLength, bool isRiffChunk, bool allowSubChunks)
    {
      if (isRiffChunk && this.m_riffHeaderWritten)
        throw new InvalidOperationException("BeginRiffChunk already called.");
      if (!isRiffChunk && !this.m_riffHeaderWritten)
        throw new InvalidOperationException("BeginRiffChunk has not been called yet.");
      this.ThrowIfNoRoomForMoreChunk(dataLength);
      this.m_binaryWriter.Write(id);
      this.m_binaryWriter.Write(dataLength);
      this.m_startedChunks.Push(new RiffWriter.ChunkInfo(checked ((uint) this.m_stream.Position), dataLength, allowSubChunks));
    }

    public void EndChunk()
    {
      RiffWriter.ChunkInfo chunkInfo = this.m_startedChunks.Pop();
      uint offset = (uint) checked (this.m_stream.Position - (long) chunkInfo.DataPosition);
      if ((int) offset != (int) chunkInfo.DataLength)
      {
        this.m_stream.Position = (long) (chunkInfo.DataPosition - 4U);
        this.m_binaryWriter.Write(offset);
        this.m_stream.Seek((long) offset, SeekOrigin.Current);
      }
      if (offset % 2U == 0U)
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
      uint num2 = (uint) ((alignment + alignment - 4 - 4 - num1) % alignment);
      this.ThrowIfNoRoomForMoreChunk(num2);
      this.m_binaryWriter.Write(1263424842U);
      GitStreamUtil.SafeForwardSeek(this.m_stream, (long) num2, true);
    }

    private void ThrowIfNoRoomForMoreChunk(uint dataLength)
    {
      if (this.m_stream.Position > 4294967287L - (long) dataLength)
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
      public readonly uint DataPosition;
      public readonly uint DataLength;
      public readonly bool AllowSubChunks;

      public ChunkInfo(uint dataPosition, uint dataLength, bool allowSubChunks)
      {
        this.DataPosition = dataPosition;
        this.DataLength = dataLength;
        this.AllowSubChunks = allowSubChunks;
      }
    }
  }
}
