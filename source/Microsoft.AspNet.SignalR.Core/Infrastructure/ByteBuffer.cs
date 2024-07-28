// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.Infrastructure.ByteBuffer
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.AspNet.SignalR.Infrastructure
{
  internal sealed class ByteBuffer
  {
    private int _currentLength;
    private readonly int? _maxLength;
    private readonly List<byte[]> _segments = new List<byte[]>();

    public ByteBuffer(int? maxLength) => this._maxLength = maxLength;

    public void Append(byte[] segment)
    {
      checked { this._currentLength += segment.Length; }
      if (this._maxLength.HasValue)
      {
        int currentLength = this._currentLength;
        int? maxLength = this._maxLength;
        int valueOrDefault = maxLength.GetValueOrDefault();
        if (currentLength > valueOrDefault & maxLength.HasValue)
          throw new InvalidOperationException("Buffer length exceeded");
      }
      this._segments.Add(segment);
    }

    public byte[] GetByteArray()
    {
      byte[] dst = new byte[this._currentLength];
      int dstOffset = 0;
      for (int index = 0; index < this._segments.Count; ++index)
      {
        byte[] segment = this._segments[index];
        Buffer.BlockCopy((Array) segment, 0, (Array) dst, dstOffset, segment.Length);
        dstOffset += segment.Length;
      }
      return dst;
    }

    public string GetString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      Decoder decoder = Encoding.UTF8.GetDecoder();
      for (int index = 0; index < this._segments.Count; ++index)
      {
        bool flush = index == this._segments.Count - 1;
        byte[] segment = this._segments[index];
        char[] chars1 = new char[decoder.GetCharCount(segment, 0, segment.Length, flush)];
        int chars2 = decoder.GetChars(segment, 0, segment.Length, chars1, 0, flush);
        stringBuilder.Append(chars1, 0, chars2);
      }
      return stringBuilder.ToString();
    }
  }
}
