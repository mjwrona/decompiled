// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.ODataBinaryStreamReader
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using System;
using System.IO;

namespace Microsoft.OData
{
  internal sealed class ODataBinaryStreamReader : Stream
  {
    private readonly Func<char[], int, int, int> reader;
    private readonly int charLength = 1024;
    private char[] chars;
    private int bytesOffset;
    private byte[] bytes = new byte[0];

    internal ODataBinaryStreamReader(Func<char[], int, int, int> reader)
    {
      this.reader = reader;
      this.chars = new char[this.charLength];
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

    public override int Read(byte[] buffer, int offset, int count)
    {
      int index = 0;
      int num = this.bytes.Length - this.bytesOffset;
      while (index < count)
      {
        if (num == 0)
        {
          int length = this.reader(this.chars, offset, this.charLength);
          if (length >= 1)
          {
            this.bytes = Convert.FromBase64CharArray(this.chars, 0, length);
            num = this.bytes.Length;
            this.bytesOffset = 0;
            if (num < 1)
              break;
          }
          else
            break;
        }
        buffer[index] = this.bytes[this.bytesOffset];
        ++index;
        ++this.bytesOffset;
        --num;
      }
      return index;
    }

    public override void Flush() => throw new NotSupportedException();

    public override void SetLength(long value) => throw new NotSupportedException();

    public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();

    public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
  }
}
