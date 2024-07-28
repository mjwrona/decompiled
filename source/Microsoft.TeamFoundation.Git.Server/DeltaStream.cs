// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.DeltaStream
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.IO;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal class DeltaStream : Stream
  {
    private readonly Stream m_deltaStream;
    private readonly Stream m_baseStream;
    private readonly int m_outputSize;
    private readonly bool m_leaveDeltaOpen;
    private readonly bool m_leaveBaseOpen;
    private readonly byte[] m_oneByte = new byte[1];
    private bool m_closed;
    private DeltaStream.DeltaInstruction m_instruction;
    private long m_basePosition;
    private int m_position;

    public DeltaStream(
      Stream deltaStream,
      Stream baseStream,
      bool leaveDeltaOpen = false,
      bool leaveBaseOpen = false)
    {
      ArgumentUtility.CheckForNull<Stream>(deltaStream, nameof (deltaStream));
      ArgumentUtility.CheckForNull<Stream>(baseStream, nameof (baseStream));
      if (!baseStream.CanSeek)
        throw new InvalidOperationException();
      this.m_deltaStream = (Stream) new BufferedStream(deltaStream);
      this.m_baseStream = baseStream;
      this.m_basePosition = -1L;
      int num = this.ReadSize();
      if (baseStream.Length > (long) int.MaxValue || (long) num != baseStream.Length)
        throw new InvalidGitDeltaDataException();
      this.m_outputSize = this.ReadSize();
      this.m_leaveDeltaOpen = leaveDeltaOpen;
      this.m_leaveBaseOpen = leaveBaseOpen;
    }

    public override void Close()
    {
      if (!this.m_closed)
      {
        this.m_closed = true;
        if (!this.m_leaveDeltaOpen)
          this.m_deltaStream.Close();
        if (!this.m_leaveBaseOpen)
          this.m_baseStream.Close();
      }
      base.Close();
    }

    public override long Length => (long) this.m_outputSize;

    public override bool CanRead => true;

    public override int Read(byte[] buffer, int offset, int count)
    {
      int num1 = 0;
      while (count > 0 && (this.m_instruction.Size != 0 || this.ReadInstruction()))
      {
        int num2;
        if (this.m_instruction.Mode == DeltaStream.DeltaMode.Copy)
        {
          if (this.m_basePosition != (long) this.m_instruction.Offset)
          {
            this.m_baseStream.Seek((long) this.m_instruction.Offset, SeekOrigin.Begin);
            this.m_basePosition = (long) this.m_instruction.Offset;
          }
          num2 = this.m_baseStream.Read(buffer, offset, Math.Min(count, this.m_instruction.Size));
          this.m_basePosition += (long) num2;
        }
        else
        {
          if (this.m_instruction.Mode != DeltaStream.DeltaMode.Add)
            throw new InvalidGitDeltaDataException();
          num2 = this.m_deltaStream.Read(buffer, offset, Math.Min(count, this.m_instruction.Size));
        }
        if (num2 == 0)
          throw new InvalidOperationException();
        this.m_instruction.Offset += (uint) num2;
        this.m_instruction.Size -= num2;
        offset += num2;
        count -= num2;
        num1 += num2;
      }
      this.m_position += num1;
      return num1;
    }

    private int ReadSize()
    {
      int num1 = 0;
      int num2 = 0;
      while (true)
      {
        int num3 = this.ReadDeltaByte();
        if (num3 >= 0)
        {
          num1 |= (num3 & (int) sbyte.MaxValue) << 7 * num2;
          if ((num3 & 128) != 0)
            ++num2;
          else
            goto label_5;
        }
        else
          break;
      }
      throw new InvalidGitDeltaDataException();
label_5:
      return num1;
    }

    private uint ReadCopyOffsetOrSize(int instruction)
    {
      uint num1 = 0;
      if ((instruction & 1) != 0)
      {
        int num2 = this.ReadDeltaByte();
        if (num2 < 0)
          throw new InvalidGitDeltaDataException();
        num1 |= (uint) num2;
      }
      if ((instruction & 2) != 0)
      {
        int num3 = this.ReadDeltaByte();
        if (num3 < 0)
          throw new InvalidGitDeltaDataException();
        num1 |= (uint) (num3 << 8);
      }
      if ((instruction & 4) != 0)
      {
        int num4 = this.ReadDeltaByte();
        if (num4 < 0)
          throw new InvalidGitDeltaDataException();
        num1 |= (uint) (num4 << 16);
      }
      if ((instruction & 8) != 0)
      {
        int num5 = this.ReadDeltaByte();
        if (num5 < 0)
          throw new InvalidGitDeltaDataException();
        num1 |= (uint) (num5 << 24);
      }
      return num1;
    }

    private bool ReadInstruction()
    {
      int size1 = this.ReadDeltaByte();
      if (size1 < 0)
        return false;
      if ((size1 & 128) != 0)
      {
        uint offset = this.ReadCopyOffsetOrSize(size1 & 15);
        uint size2 = this.ReadCopyOffsetOrSize((size1 & 112) >> 4);
        if (size2 == 0U)
          size2 = 65536U;
        this.m_instruction = new DeltaStream.DeltaInstruction(DeltaStream.DeltaMode.Copy, (int) size2, offset);
      }
      else
        this.m_instruction = size1 > 0 ? new DeltaStream.DeltaInstruction(DeltaStream.DeltaMode.Add, size1) : throw new InvalidGitDeltaDataException();
      return true;
    }

    private int ReadDeltaByte() => this.m_deltaStream.Read(this.m_oneByte, 0, 1) == 0 ? -1 : (int) this.m_oneByte[0];

    public override bool CanSeek => false;

    public override bool CanWrite => false;

    public override void SetLength(long value) => throw new NotSupportedException();

    public override long Position
    {
      get => (long) this.m_position;
      set => throw new NotImplementedException();
    }

    public override void Flush() => throw new NotImplementedException();

    public override void Write(byte[] buffer, int offset, int count) => throw new NotImplementedException();

    public override long Seek(long offset, SeekOrigin origin) => throw new NotImplementedException();

    private enum DeltaMode
    {
      Add,
      Copy,
    }

    private struct DeltaInstruction
    {
      public DeltaStream.DeltaMode Mode;
      public int Size;
      public uint Offset;

      public DeltaInstruction(DeltaStream.DeltaMode mode, int size)
      {
        this.Mode = mode == DeltaStream.DeltaMode.Add ? mode : throw new InvalidGitDeltaDataException();
        this.Size = size;
        this.Offset = 0U;
      }

      public DeltaInstruction(DeltaStream.DeltaMode mode, int size, uint offset)
      {
        this.Mode = mode;
        this.Size = size;
        this.Offset = offset;
      }
    }
  }
}
