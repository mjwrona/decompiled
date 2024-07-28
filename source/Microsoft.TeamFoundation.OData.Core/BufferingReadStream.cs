// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.BufferingReadStream
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.OData
{
  internal sealed class BufferingReadStream : Stream
  {
    private readonly LinkedList<byte[]> buffers;
    private Stream innerStream;
    private int positionInCurrentBuffer;
    private bool bufferingModeDisabled;
    private LinkedListNode<byte[]> currentReadNode;

    internal BufferingReadStream(Stream stream)
    {
      this.innerStream = stream;
      this.buffers = new LinkedList<byte[]>();
    }

    public override bool CanRead => true;

    public override bool CanSeek => false;

    public override bool CanWrite => false;

    public override long Length => throw new NotSupportedException();

    public override long Position
    {
      get => throw new NotSupportedException();
      set => throw new NotSupportedException();
    }

    internal bool IsBuffering => !this.bufferingModeDisabled;

    public override void Flush() => throw new NotSupportedException();

    public override int Read(byte[] userBuffer, int offset, int count)
    {
      ExceptionUtils.CheckArgumentNotNull<byte[]>(userBuffer, nameof (userBuffer));
      ExceptionUtils.CheckIntegerNotNegative(offset, nameof (offset));
      ExceptionUtils.CheckIntegerPositive(count, nameof (count));
      int num1 = 0;
      while (this.currentReadNode != null && count > 0)
      {
        byte[] src = this.currentReadNode.Value;
        int count1 = src.Length - this.positionInCurrentBuffer;
        if (count1 == count)
        {
          Buffer.BlockCopy((Array) src, this.positionInCurrentBuffer, (Array) userBuffer, offset, count);
          int num2 = num1 + count;
          this.MoveToNextBuffer();
          return num2;
        }
        if (count1 > count)
        {
          Buffer.BlockCopy((Array) src, this.positionInCurrentBuffer, (Array) userBuffer, offset, count);
          int num3 = num1 + count;
          this.positionInCurrentBuffer += count;
          return num3;
        }
        Buffer.BlockCopy((Array) src, this.positionInCurrentBuffer, (Array) userBuffer, offset, count1);
        num1 += count1;
        offset += count1;
        count -= count1;
        this.MoveToNextBuffer();
      }
      int count2 = this.innerStream.Read(userBuffer, offset, count);
      if (!this.bufferingModeDisabled && count2 > 0)
      {
        byte[] dst = new byte[count2];
        Buffer.BlockCopy((Array) userBuffer, offset, (Array) dst, 0, count2);
        this.buffers.AddLast(dst);
      }
      return num1 + count2;
    }

    public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();

    public override void SetLength(long value) => throw new NotSupportedException();

    public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();

    internal void ResetStream()
    {
      this.currentReadNode = this.buffers.First;
      this.positionInCurrentBuffer = 0;
    }

    internal void StopBuffering()
    {
      this.ResetStream();
      this.bufferingModeDisabled = true;
    }

    protected override void Dispose(bool disposing)
    {
      if (!this.bufferingModeDisabled)
        return;
      if (disposing && this.innerStream != null)
      {
        this.innerStream.Dispose();
        this.innerStream = (Stream) null;
      }
      base.Dispose(disposing);
    }

    private void MoveToNextBuffer()
    {
      if (this.bufferingModeDisabled)
      {
        this.buffers.RemoveFirst();
        this.currentReadNode = this.buffers.First;
      }
      else
        this.currentReadNode = this.currentReadNode.Next;
      this.positionInCurrentBuffer = 0;
    }
  }
}
