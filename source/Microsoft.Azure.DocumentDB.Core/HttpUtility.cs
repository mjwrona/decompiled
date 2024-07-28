// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.HttpUtility
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Microsoft.Azure.Documents.Collections;
using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace Microsoft.Azure.Documents
{
  internal sealed class HttpUtility
  {
    private static char[] s_entityEndingChars = new char[2]
    {
      ';',
      '&'
    };

    internal static string AspCompatUrlEncode(string s)
    {
      s = HttpUtility.UrlEncode(s);
      s = s.Replace("!", "%21");
      s = s.Replace("*", "%2A");
      s = s.Replace("(", "%28");
      s = s.Replace(")", "%29");
      s = s.Replace("-", "%2D");
      s = s.Replace(".", "%2E");
      s = s.Replace("_", "%5F");
      s = s.Replace("\\", "%5C");
      return s;
    }

    internal static string CollapsePercentUFromStringInternal(string s, Encoding e)
    {
      int length = s.Length;
      HttpUtility.UrlDecoder urlDecoder = new HttpUtility.UrlDecoder(length, e);
      if (s.IndexOf("%u", StringComparison.Ordinal) == -1)
        return s;
      for (int index = 0; index < length; ++index)
      {
        char ch1 = s[index];
        if (ch1 == '%' && index < length - 5 && s[index + 1] == 'u')
        {
          int num1 = HttpUtility.HexToInt(s[index + 2]);
          int num2 = HttpUtility.HexToInt(s[index + 3]);
          int num3 = HttpUtility.HexToInt(s[index + 4]);
          int num4 = HttpUtility.HexToInt(s[index + 5]);
          if (num1 >= 0 && num2 >= 0 && num3 >= 0 && num4 >= 0)
          {
            char ch2 = (char) (num1 << 12 | num2 << 8 | num3 << 4 | num4);
            index += 5;
            urlDecoder.AddChar(ch2);
            continue;
          }
        }
        if (((int) ch1 & 65408) == 0)
          urlDecoder.AddByte((byte) ch1);
        else
          urlDecoder.AddChar(ch1);
      }
      return urlDecoder.GetString();
    }

    internal static string FormatHttpCookieDateTime(DateTime dt)
    {
      if (dt < DateTime.MaxValue.AddDays(-1.0) && dt > DateTime.MinValue.AddDays(1.0))
        dt = dt.ToUniversalTime();
      return dt.ToString("ddd, dd-MMM-yyyy HH':'mm':'ss 'GMT'", (IFormatProvider) DateTimeFormatInfo.InvariantInfo);
    }

    internal static string FormatHttpDateTime(DateTime dt)
    {
      if (dt < DateTime.MaxValue.AddDays(-1.0) && dt > DateTime.MinValue.AddDays(1.0))
        dt = dt.ToUniversalTime();
      return dt.ToString("R", (IFormatProvider) DateTimeFormatInfo.InvariantInfo);
    }

    internal static string FormatHttpDateTimeUtc(DateTime dt) => dt.ToString("R", (IFormatProvider) DateTimeFormatInfo.InvariantInfo);

    internal static string FormatPlainTextAsHtml(string s)
    {
      if (s == null)
        return (string) null;
      StringBuilder sb = new StringBuilder();
      using (StringWriter output = new StringWriter(sb, (IFormatProvider) CultureInfo.InvariantCulture))
        HttpUtility.FormatPlainTextAsHtml(s, (TextWriter) output);
      return sb.ToString();
    }

    internal static INameValueCollection ParseQueryString(string queryString)
    {
      INameValueCollection queryString1 = (INameValueCollection) new DictionaryNameValueCollection();
      string str1 = queryString;
      char[] chArray1 = new char[1]{ '&' };
      foreach (string str2 in str1.Split(chArray1))
      {
        char[] chArray2 = new char[1]{ '=' };
        string[] strArray = str2.Split(chArray2);
        if (strArray.Length > 1)
        {
          string str3 = HttpUtility.UrlDecode(strArray[0].Trim('?', ' '));
          string str4 = HttpUtility.UrlDecode(strArray[1].Trim());
          queryString1.Add(CultureInfo.InvariantCulture.TextInfo.ToLower(str3), str4);
        }
      }
      return queryString1;
    }

    internal static void FormatPlainTextAsHtml(string s, TextWriter output)
    {
      if (s == null)
        return;
      int length = s.Length;
      char ch1 = char.MinValue;
      for (int index = 0; index < length; ++index)
      {
        char ch2 = s[index];
        switch (ch2)
        {
          case '\n':
            output.Write("<br>");
            goto case '\r';
          case '\r':
            ch1 = ch2;
            continue;
          case ' ':
            if (ch1 == ' ')
            {
              output.Write("&nbsp;");
              goto case '\r';
            }
            else
            {
              output.Write(ch2);
              goto case '\r';
            }
          case '"':
            output.Write("&quot;");
            goto case '\r';
          case '&':
            output.Write("&amp;");
            goto case '\r';
          case '<':
            output.Write("&lt;");
            goto case '\r';
          case '>':
            output.Write("&gt;");
            goto case '\r';
          default:
            if (ch2 >= ' ' && ch2 < 'Ā')
            {
              output.Write("&#");
              output.Write(((int) ch2).ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
              output.Write(';');
              goto case '\r';
            }
            else
            {
              output.Write(ch2);
              goto case '\r';
            }
        }
      }
    }

    internal static string FormatPlainTextSpacesAsHtml(string s)
    {
      if (s == null)
        return (string) null;
      StringBuilder sb = new StringBuilder();
      using (StringWriter stringWriter = new StringWriter(sb, (IFormatProvider) CultureInfo.InvariantCulture))
      {
        int length = s.Length;
        for (int index = 0; index < length; ++index)
        {
          char ch = s[index];
          if (ch == ' ')
            stringWriter.Write("&nbsp;");
          else
            stringWriter.Write(ch);
        }
      }
      return sb.ToString();
    }

    private static int HexToInt(char h)
    {
      if (h >= '0' && h <= '9')
        return (int) h - 48;
      if (h >= 'a' && h <= 'f')
        return (int) h - 97 + 10;
      return h >= 'A' && h <= 'F' ? (int) h - 65 + 10 : -1;
    }

    internal static char IntToHex(int n) => n <= 9 ? (char) (n + 48) : (char) (n - 10 + 97);

    private static bool IsNonAsciiByte(byte b) => b >= (byte) 127 || b < (byte) 32;

    internal static bool IsSafe(char ch)
    {
      if (ch >= 'a' && ch <= 'z' || ch >= 'A' && ch <= 'Z' || ch >= '0' && ch <= '9' || ch == '!')
        return true;
      switch (ch)
      {
        case '\'':
        case '(':
        case ')':
        case '*':
        case '-':
        case '.':
        case '_':
          return true;
        default:
          return false;
      }
    }

    public static string UrlDecode(string str) => str == null ? (string) null : HttpUtility.UrlDecode(str, Encoding.UTF8);

    public static string UrlDecode(byte[] bytes, Encoding e) => bytes == null ? (string) null : HttpUtility.UrlDecode(bytes, 0, bytes.Length, e);

    public static string UrlDecode(string str, Encoding e) => str == null ? (string) null : HttpUtility.UrlDecodeStringFromStringInternal(str, e);

    public static string UrlDecode(byte[] bytes, int offset, int count, Encoding e)
    {
      if (bytes == null && count == 0)
        return (string) null;
      if (bytes == null)
        throw new ArgumentNullException(nameof (bytes));
      if (offset < 0 || offset > bytes.Length)
        throw new ArgumentOutOfRangeException(nameof (offset));
      if (count < 0 || offset + count > bytes.Length)
        throw new ArgumentOutOfRangeException(nameof (count));
      return HttpUtility.UrlDecodeStringFromBytesInternal(bytes, offset, count, e);
    }

    private static byte[] UrlDecodeBytesFromBytesInternal(byte[] buf, int offset, int count)
    {
      int length = 0;
      byte[] sourceArray = new byte[count];
      for (int index1 = 0; index1 < count; ++index1)
      {
        int index2 = offset + index1;
        byte num1 = buf[index2];
        switch (num1)
        {
          case 37:
            if (index1 < count - 2)
            {
              int num2 = HttpUtility.HexToInt((char) buf[index2 + 1]);
              int num3 = HttpUtility.HexToInt((char) buf[index2 + 2]);
              if (num2 >= 0 && num3 >= 0)
              {
                num1 = (byte) (num2 << 4 | num3);
                index1 += 2;
                break;
              }
              break;
            }
            break;
          case 43:
            num1 = (byte) 32;
            break;
        }
        sourceArray[length++] = num1;
      }
      if (length < sourceArray.Length)
      {
        byte[] destinationArray = new byte[length];
        Array.Copy((Array) sourceArray, (Array) destinationArray, length);
        sourceArray = destinationArray;
      }
      return sourceArray;
    }

    private static string UrlDecodeStringFromBytesInternal(
      byte[] buf,
      int offset,
      int count,
      Encoding e)
    {
      HttpUtility.UrlDecoder urlDecoder = new HttpUtility.UrlDecoder(count, e);
      for (int index1 = 0; index1 < count; ++index1)
      {
        int index2 = offset + index1;
        byte b = buf[index2];
        switch (b)
        {
          case 37:
            if (index1 < count - 2)
            {
              if (buf[index2 + 1] == (byte) 117 && index1 < count - 5)
              {
                int num1 = HttpUtility.HexToInt((char) buf[index2 + 2]);
                int num2 = HttpUtility.HexToInt((char) buf[index2 + 3]);
                int num3 = HttpUtility.HexToInt((char) buf[index2 + 4]);
                int num4 = HttpUtility.HexToInt((char) buf[index2 + 5]);
                if (num1 >= 0 && num2 >= 0 && num3 >= 0 && num4 >= 0)
                {
                  char ch = (char) (num1 << 12 | num2 << 8 | num3 << 4 | num4);
                  index1 += 5;
                  urlDecoder.AddChar(ch);
                  break;
                }
                goto default;
              }
              else
              {
                int num5 = HttpUtility.HexToInt((char) buf[index2 + 1]);
                int num6 = HttpUtility.HexToInt((char) buf[index2 + 2]);
                if (num5 >= 0 && num6 >= 0)
                {
                  b = (byte) (num5 << 4 | num6);
                  index1 += 2;
                  goto default;
                }
                else
                  goto default;
              }
            }
            else
              goto default;
          case 43:
            b = (byte) 32;
            goto default;
          default:
            urlDecoder.AddByte(b);
            break;
        }
      }
      return urlDecoder.GetString();
    }

    private static string UrlDecodeStringFromStringInternal(string s, Encoding e)
    {
      int length = s.Length;
      HttpUtility.UrlDecoder urlDecoder = new HttpUtility.UrlDecoder(length, e);
      for (int index = 0; index < length; ++index)
      {
        char ch1 = s[index];
        switch (ch1)
        {
          case '%':
            if (index < length - 2)
            {
              if (s[index + 1] == 'u' && index < length - 5)
              {
                int num1 = HttpUtility.HexToInt(s[index + 2]);
                int num2 = HttpUtility.HexToInt(s[index + 3]);
                int num3 = HttpUtility.HexToInt(s[index + 4]);
                int num4 = HttpUtility.HexToInt(s[index + 5]);
                if (num1 >= 0 && num2 >= 0 && num3 >= 0 && num4 >= 0)
                {
                  char ch2 = (char) (num1 << 12 | num2 << 8 | num3 << 4 | num4);
                  index += 5;
                  urlDecoder.AddChar(ch2);
                  break;
                }
                goto default;
              }
              else
              {
                int num5 = HttpUtility.HexToInt(s[index + 1]);
                int num6 = HttpUtility.HexToInt(s[index + 2]);
                if (num5 >= 0 && num6 >= 0)
                {
                  byte b = (byte) (num5 << 4 | num6);
                  index += 2;
                  urlDecoder.AddByte(b);
                  break;
                }
                goto default;
              }
            }
            else
              goto default;
          case '+':
            ch1 = ' ';
            goto default;
          default:
            if (((int) ch1 & 65408) == 0)
            {
              urlDecoder.AddByte((byte) ch1);
              break;
            }
            urlDecoder.AddChar(ch1);
            break;
        }
      }
      return urlDecoder.GetString();
    }

    public static byte[] UrlDecodeToBytes(byte[] bytes) => bytes == null ? (byte[]) null : HttpUtility.UrlDecodeToBytes(bytes, 0, bytes != null ? bytes.Length : 0);

    public static byte[] UrlDecodeToBytes(string str) => str == null ? (byte[]) null : HttpUtility.UrlDecodeToBytes(str, Encoding.UTF8);

    public static byte[] UrlDecodeToBytes(string str, Encoding e) => str == null ? (byte[]) null : HttpUtility.UrlDecodeToBytes(e.GetBytes(str));

    public static byte[] UrlDecodeToBytes(byte[] bytes, int offset, int count)
    {
      if (bytes == null && count == 0)
        return (byte[]) null;
      if (bytes == null)
        throw new ArgumentNullException(nameof (bytes));
      if (offset < 0 || offset > bytes.Length)
        throw new ArgumentOutOfRangeException(nameof (offset));
      if (count < 0 || offset + count > bytes.Length)
        throw new ArgumentOutOfRangeException(nameof (count));
      return HttpUtility.UrlDecodeBytesFromBytesInternal(bytes, offset, count);
    }

    public static string UrlEncode(byte[] bytes)
    {
      if (bytes == null)
        return (string) null;
      byte[] bytes1 = HttpUtility.UrlEncodeToBytes(bytes);
      return Encoding.UTF8.GetString(bytes1, 0, bytes1.Length);
    }

    public static string UrlEncode(string str) => str == null ? (string) null : HttpUtility.UrlEncode(str, Encoding.UTF8);

    public static string UrlEncode(string str, Encoding e)
    {
      if (str == null)
        return (string) null;
      byte[] bytes = HttpUtility.UrlEncodeToBytes(str, e);
      return Encoding.UTF8.GetString(bytes, 0, bytes.Length);
    }

    public static string UrlEncode(byte[] bytes, int offset, int count)
    {
      if (bytes == null)
        return (string) null;
      byte[] bytes1 = HttpUtility.UrlEncodeToBytes(bytes, offset, count);
      return Encoding.UTF8.GetString(bytes1, 0, bytes1.Length);
    }

    private static byte[] UrlEncodeBytesToBytesInternal(
      byte[] bytes,
      int offset,
      int count,
      bool alwaysCreateReturnValue)
    {
      int num1 = 0;
      int num2 = 0;
      for (int index = 0; index < count; ++index)
      {
        char ch = (char) bytes[offset + index];
        if (ch == ' ')
          ++num1;
        else if (!HttpUtility.IsSafe(ch))
          ++num2;
      }
      if (!alwaysCreateReturnValue && num1 == 0 && num2 == 0)
        return bytes;
      byte[] bytesInternal = new byte[count + num2 * 2];
      int num3 = 0;
      for (int index1 = 0; index1 < count; ++index1)
      {
        byte num4 = bytes[offset + index1];
        char ch = (char) num4;
        if (HttpUtility.IsSafe(ch))
          bytesInternal[num3++] = num4;
        else if (ch == ' ')
        {
          bytesInternal[num3++] = (byte) 43;
        }
        else
        {
          byte[] numArray1 = bytesInternal;
          int index2 = num3;
          int num5 = index2 + 1;
          numArray1[index2] = (byte) 37;
          byte[] numArray2 = bytesInternal;
          int index3 = num5;
          int num6 = index3 + 1;
          int hex1 = (int) (byte) HttpUtility.IntToHex((int) num4 >> 4 & 15);
          numArray2[index3] = (byte) hex1;
          byte[] numArray3 = bytesInternal;
          int index4 = num6;
          num3 = index4 + 1;
          int hex2 = (int) (byte) HttpUtility.IntToHex((int) num4 & 15);
          numArray3[index4] = (byte) hex2;
        }
      }
      return bytesInternal;
    }

    private static byte[] UrlEncodeBytesToBytesInternalNonAscii(
      byte[] bytes,
      int offset,
      int count,
      bool alwaysCreateReturnValue)
    {
      int num1 = 0;
      for (int index = 0; index < count; ++index)
      {
        if (HttpUtility.IsNonAsciiByte(bytes[offset + index]))
          ++num1;
      }
      if (!alwaysCreateReturnValue && num1 == 0)
        return bytes;
      byte[] internalNonAscii = new byte[count + num1 * 2];
      int num2 = 0;
      for (int index1 = 0; index1 < count; ++index1)
      {
        byte b = bytes[offset + index1];
        if (HttpUtility.IsNonAsciiByte(b))
        {
          byte[] numArray1 = internalNonAscii;
          int index2 = num2;
          int num3 = index2 + 1;
          numArray1[index2] = (byte) 37;
          byte[] numArray2 = internalNonAscii;
          int index3 = num3;
          int num4 = index3 + 1;
          int hex1 = (int) (byte) HttpUtility.IntToHex((int) b >> 4 & 15);
          numArray2[index3] = (byte) hex1;
          byte[] numArray3 = internalNonAscii;
          int index4 = num4;
          num2 = index4 + 1;
          int hex2 = (int) (byte) HttpUtility.IntToHex((int) b & 15);
          numArray3[index4] = (byte) hex2;
        }
        else
          internalNonAscii[num2++] = b;
      }
      return internalNonAscii;
    }

    internal static string UrlEncodeNonAscii(string str, Encoding e)
    {
      if (string.IsNullOrEmpty(str))
        return str;
      if (e == null)
        e = Encoding.UTF8;
      byte[] bytes = e.GetBytes(str);
      byte[] internalNonAscii = HttpUtility.UrlEncodeBytesToBytesInternalNonAscii(bytes, 0, bytes.Length, false);
      return Encoding.UTF8.GetString(internalNonAscii, 0, internalNonAscii.Length);
    }

    internal static string UrlEncodeSpaces(string str)
    {
      if (str != null && str.IndexOf(' ') >= 0)
        str = str.Replace(" ", "%20");
      return str;
    }

    public static byte[] UrlEncodeToBytes(string str) => str == null ? (byte[]) null : HttpUtility.UrlEncodeToBytes(str, Encoding.UTF8);

    public static byte[] UrlEncodeToBytes(byte[] bytes) => bytes == null ? (byte[]) null : HttpUtility.UrlEncodeToBytes(bytes, 0, bytes.Length);

    public static byte[] UrlEncodeToBytes(string str, Encoding e)
    {
      if (str == null)
        return (byte[]) null;
      byte[] bytes = e.GetBytes(str);
      return HttpUtility.UrlEncodeBytesToBytesInternal(bytes, 0, bytes.Length, false);
    }

    public static byte[] UrlEncodeToBytes(byte[] bytes, int offset, int count)
    {
      if (bytes == null && count == 0)
        return (byte[]) null;
      if (bytes == null)
        throw new ArgumentNullException(nameof (bytes));
      if (offset < 0 || offset > bytes.Length)
        throw new ArgumentOutOfRangeException(nameof (offset));
      if (count < 0 || offset + count > bytes.Length)
        throw new ArgumentOutOfRangeException(nameof (count));
      return HttpUtility.UrlEncodeBytesToBytesInternal(bytes, offset, count, true);
    }

    public static string UrlEncodeUnicode(string str) => str == null ? (string) null : HttpUtility.UrlEncodeUnicodeStringToStringInternal(str, false);

    private static string UrlEncodeUnicodeStringToStringInternal(string s, bool ignoreAscii)
    {
      int length = s.Length;
      StringBuilder stringBuilder = new StringBuilder(length);
      for (int index = 0; index < length; ++index)
      {
        char ch = s[index];
        if (((int) ch & 65408) == 0)
        {
          if (ignoreAscii || HttpUtility.IsSafe(ch))
            stringBuilder.Append(ch);
          else if (ch == ' ')
          {
            stringBuilder.Append('+');
          }
          else
          {
            stringBuilder.Append('%');
            stringBuilder.Append(HttpUtility.IntToHex((int) ch >> 4 & 15));
            stringBuilder.Append(HttpUtility.IntToHex((int) ch & 15));
          }
        }
        else
        {
          stringBuilder.Append("%u");
          stringBuilder.Append(HttpUtility.IntToHex((int) ch >> 12 & 15));
          stringBuilder.Append(HttpUtility.IntToHex((int) ch >> 8 & 15));
          stringBuilder.Append(HttpUtility.IntToHex((int) ch >> 4 & 15));
          stringBuilder.Append(HttpUtility.IntToHex((int) ch & 15));
        }
      }
      return stringBuilder.ToString();
    }

    public static byte[] UrlEncodeUnicodeToBytes(string str) => str == null ? (byte[]) null : Encoding.UTF8.GetBytes(HttpUtility.UrlEncodeUnicode(str));

    public static string UrlPathEncode(string str)
    {
      if (str == null)
        return (string) null;
      int num = str.IndexOf('?');
      return num >= 0 ? HttpUtility.UrlPathEncode(str.Substring(0, num)) + str.Substring(num) : HttpUtility.UrlEncodeSpaces(HttpUtility.UrlEncodeNonAscii(str, Encoding.UTF8));
    }

    private class UrlDecoder
    {
      private int _bufferSize;
      private byte[] _byteBuffer;
      private char[] _charBuffer;
      private Encoding _encoding;
      private int _numBytes;
      private int _numChars;

      internal UrlDecoder(int bufferSize, Encoding encoding)
      {
        this._bufferSize = bufferSize;
        this._encoding = encoding;
        this._charBuffer = new char[bufferSize];
      }

      internal void AddByte(byte b)
      {
        if (this._byteBuffer == null)
          this._byteBuffer = new byte[this._bufferSize];
        this._byteBuffer[this._numBytes++] = b;
      }

      internal void AddChar(char ch)
      {
        if (this._numBytes > 0)
          this.FlushBytes();
        this._charBuffer[this._numChars++] = ch;
      }

      private void FlushBytes()
      {
        if (this._numBytes <= 0)
          return;
        this._numChars += this._encoding.GetChars(this._byteBuffer, 0, this._numBytes, this._charBuffer, this._numChars);
        this._numBytes = 0;
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
