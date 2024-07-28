// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.Infrastructure.UrlDecoder
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using System.Text;

namespace Microsoft.AspNet.SignalR.Infrastructure
{
  internal static class UrlDecoder
  {
    public static string UrlDecode(string str) => str == null ? (string) null : UrlDecoder.UrlDecodeInternal(str, Encoding.UTF8);

    private static string UrlDecodeInternal(string value, Encoding encoding)
    {
      if (value == null)
        return (string) null;
      int length = value.Length;
      UrlDecoder.DecoderHelper decoderHelper = new UrlDecoder.DecoderHelper(length, encoding);
      for (int index = 0; index < length; ++index)
      {
        char ch = value[index];
        switch (ch)
        {
          case '%':
            if (index < length - 2)
            {
              int num1 = UrlDecoder.HexToInt(value[index + 1]);
              int num2 = UrlDecoder.HexToInt(value[index + 2]);
              if (num1 >= 0 && num2 >= 0)
              {
                byte b = (byte) (num1 << 4 | num2);
                index += 2;
                decoderHelper.AddByte(b);
                break;
              }
              goto default;
            }
            else
              goto default;
          case '+':
            ch = ' ';
            goto default;
          default:
            if (((int) ch & 65408) == 0)
            {
              decoderHelper.AddByte((byte) ch);
              break;
            }
            decoderHelper.AddChar(ch);
            break;
        }
      }
      return decoderHelper.GetString();
    }

    private static int HexToInt(char h)
    {
      if (h >= '0' && h <= '9')
        return (int) h - 48;
      if (h >= 'a' && h <= 'f')
        return (int) h - 97 + 10;
      return h < 'A' || h > 'F' ? -1 : (int) h - 65 + 10;
    }

    private class DecoderHelper
    {
      private int _bufferSize;
      private int _numChars;
      private char[] _charBuffer;
      private int _numBytes;
      private byte[] _byteBuffer;
      private Encoding _encoding;

      private void FlushBytes()
      {
        if (this._numBytes <= 0)
          return;
        this._numChars += this._encoding.GetChars(this._byteBuffer, 0, this._numBytes, this._charBuffer, this._numChars);
        this._numBytes = 0;
      }

      internal DecoderHelper(int bufferSize, Encoding encoding)
      {
        this._bufferSize = bufferSize;
        this._encoding = encoding;
        this._charBuffer = new char[bufferSize];
      }

      internal void AddChar(char ch)
      {
        if (this._numBytes > 0)
          this.FlushBytes();
        this._charBuffer[this._numChars++] = ch;
      }

      internal void AddByte(byte b)
      {
        if (this._byteBuffer == null)
          this._byteBuffer = new byte[this._bufferSize];
        this._byteBuffer[this._numBytes++] = b;
      }

      internal string GetString()
      {
        if (this._numBytes > 0)
          this.FlushBytes();
        return this._numChars > 0 ? new string(this._charBuffer, 0, this._numChars) : string.Empty;
      }
    }
  }
}
