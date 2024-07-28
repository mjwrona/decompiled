// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.Infrastructure.MemoryPoolTextWriter
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace Microsoft.AspNet.SignalR.Infrastructure
{
  internal class MemoryPoolTextWriter : TextWriter
  {
    private readonly IMemoryPool _memory;
    private char[] _textArray;
    private int _textBegin;
    private int _textEnd;
    private const int _textLength = 128;
    private byte[] _dataArray;
    private int _dataEnd;
    private readonly Encoder _encoder;

    public ArraySegment<byte> Buffer => new ArraySegment<byte>(this._dataArray, 0, this._dataEnd);

    public MemoryPoolTextWriter(IMemoryPool memory)
      : base((IFormatProvider) CultureInfo.InvariantCulture)
    {
      this._memory = memory;
      this._textArray = this._memory.AllocChar(128);
      this._dataArray = MemoryPool.EmptyArray;
      this._encoder = Encoding.UTF8.GetEncoder();
    }

    public override Encoding Encoding => Encoding.UTF8;

    protected override void Dispose(bool disposing)
    {
      try
      {
        if (!disposing)
          return;
        if (this._textArray != null)
        {
          this._memory.FreeChar(this._textArray);
          this._textArray = (char[]) null;
        }
        if (this._dataArray == null)
          return;
        this._memory.FreeByte(this._dataArray);
        this._dataArray = (byte[]) null;
      }
      finally
      {
        base.Dispose(disposing);
      }
    }

    private void Encode(bool flush)
    {
      this.Grow(this._encoder.GetByteCount(this._textArray, this._textBegin, this._textEnd - this._textBegin, flush));
      int bytes = this._encoder.GetBytes(this._textArray, this._textBegin, this._textEnd - this._textBegin, this._dataArray, this._dataEnd, flush);
      this._textBegin = this._textEnd = 0;
      this._dataEnd += bytes;
    }

    protected void Grow(int minimumAvailable)
    {
      if (this._dataArray.Length - this._dataEnd >= minimumAvailable)
        return;
      byte[] destinationArray = this._memory.AllocByte(this._dataArray.Length + Math.Max(this._dataArray.Length, minimumAvailable));
      Array.Copy((Array) this._dataArray, 0, (Array) destinationArray, 0, this._dataEnd);
      this._memory.FreeByte(this._dataArray);
      this._dataArray = destinationArray;
    }

    public override void Write(char value)
    {
      if (128 == this._textEnd)
      {
        this.Encode(false);
        if (128 == this._textEnd)
          throw new InvalidOperationException("Unexplainable failure to encode text");
      }
      this._textArray[this._textEnd++] = value;
    }

    public override void Write(char[] value, int index, int length)
    {
      int sourceIndex = index;
      int num = index + length;
      while (sourceIndex < num)
      {
        if (128 == this._textEnd)
          this.Encode(false);
        int length1 = num - sourceIndex;
        if (length1 > 128 - this._textEnd)
          length1 = 128 - this._textEnd;
        Array.Copy((Array) value, sourceIndex, (Array) this._textArray, this._textEnd, length1);
        sourceIndex += length1;
        this._textEnd += length1;
      }
    }

    public override void Write(string value)
    {
      int sourceIndex = 0;
      int length = value.Length;
      while (sourceIndex < length)
      {
        if (128 == this._textEnd)
          this.Encode(false);
        int count = length - sourceIndex;
        if (count > 128 - this._textEnd)
          count = 128 - this._textEnd;
        value.CopyTo(sourceIndex, this._textArray, this._textEnd, count);
        sourceIndex += count;
        this._textEnd += count;
      }
    }

    public override void Flush()
    {
      while (this._textBegin != this._textEnd)
        this.Encode(true);
    }

    public void Write(ArraySegment<byte> data)
    {
      this.Flush();
      this.Grow(data.Count);
      System.Buffer.BlockCopy((Array) data.Array, data.Offset, (Array) this._dataArray, this._dataEnd, data.Count);
      this._dataEnd += data.Count;
    }
  }
}
