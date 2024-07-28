// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.Infrastructure.ArraySegmentTextReader
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using System;
using System.IO;
using System.Text;

namespace Microsoft.AspNet.SignalR.Infrastructure
{
  public class ArraySegmentTextReader : TextReader
  {
    private readonly ArraySegment<byte> _buffer;
    private readonly Encoding _encoding;
    private int _offset;

    public ArraySegmentTextReader(ArraySegment<byte> buffer, Encoding encoding)
    {
      this._buffer = buffer;
      this._encoding = encoding;
      this._offset = this._buffer.Offset;
    }

    public override int Read(char[] buffer, int index, int count)
    {
      int byteCount = Math.Min(this._buffer.Count - this._offset, this._encoding.GetByteCount(buffer, index, count));
      int chars = this._encoding.GetChars(this._buffer.Array, this._offset, byteCount, buffer, index);
      this._offset += byteCount;
      return chars;
    }
  }
}
