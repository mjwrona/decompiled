// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Diff.FileTypeUtil
// Assembly: Microsoft.TeamFoundation.Diff, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F647AACF-6EF1-4C0C-AB27-20317A054A39
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Diff.dll

using Microsoft.TeamFoundation.Common;
using System;
using System.ComponentModel;
using System.IO;
using System.Text;

namespace Microsoft.TeamFoundation.Diff
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class FileTypeUtil
  {
    private static object m_lock = new object();
    private static Encoding m_utf8;
    private static Encoding m_utf8NoBom;
    private static Encoding m_utf16Little;
    private static Encoding m_utf16Big;
    private static Encoding m_utf32Little;
    private static Encoding m_utf32Big;
    private static readonly byte[] PdfMagic = new byte[5]
    {
      (byte) 37,
      (byte) 80,
      (byte) 68,
      (byte) 70,
      (byte) 45
    };

    public static Encoding TryDetermineTextEncoding(string path)
    {
      Encoding textEncoding = Encoding.Default;
      try
      {
        Encoding encoding = FileTypeUtil.DetermineEncoding(path, false, Encoding.Default);
        if (encoding != null)
          textEncoding = encoding;
      }
      catch (FileNotFoundException ex)
      {
      }
      catch (Exception ex)
      {
      }
      return textEncoding;
    }

    public static Encoding DetermineEncoding(
      string path,
      bool checkForBinary,
      Encoding fallbackEncoding)
    {
      using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, 1024))
        return FileTypeUtil.DetermineEncoding((Stream) fileStream, checkForBinary, fallbackEncoding);
    }

    public static Encoding DetermineEncoding(
      Stream stream,
      bool checkForBinary,
      Encoding fallbackEncoding)
    {
      return FileTypeUtil.DetermineEncoding(stream, checkForBinary, fallbackEncoding, false, out bool _);
    }

    public static Encoding DetermineEncoding(
      Stream stream,
      bool checkForBinary,
      Encoding fallbackEncoding,
      bool scanFullFile,
      out bool containsPreamble)
    {
      long scanBytes = scanFullFile ? long.MaxValue : 1024L;
      return FileTypeUtil.DetermineEncoding(stream, checkForBinary, fallbackEncoding, scanBytes, out containsPreamble);
    }

    public static Encoding DetermineEncoding(
      Stream stream,
      bool checkForBinary,
      Encoding fallbackEncoding,
      long scanBytes,
      out bool containsPreamble)
    {
      containsPreamble = false;
      if (FileTypeUtil.m_utf8 == null)
      {
        lock (FileTypeUtil.m_lock)
        {
          if (FileTypeUtil.m_utf8 == null)
          {
            FileTypeUtil.m_utf8 = (Encoding) new UTF8Encoding(true);
            FileTypeUtil.m_utf8NoBom = (Encoding) new UTF8Encoding(false);
            FileTypeUtil.m_utf16Little = (Encoding) new UnicodeEncoding(false, true);
            FileTypeUtil.m_utf16Big = (Encoding) new UnicodeEncoding(true, true);
            FileTypeUtil.m_utf32Little = (Encoding) new UTF32Encoding(false, true);
            FileTypeUtil.m_utf32Big = (Encoding) new UTF32Encoding(true, true);
          }
        }
      }
      Encoding encoding = (Encoding) null;
      if (stream.CanSeek && stream.Length == 0L)
        return fallbackEncoding;
      byte[] buffer = new byte[1024];
      bool isPdf = false;
      int num = stream.Read(buffer, 0, buffer.Length);
      if (num > 2)
      {
        encoding = FileTypeUtil.FindMatchingEncoding(buffer, num, out isPdf);
        if (encoding != null)
        {
          containsPreamble = true;
          return encoding;
        }
      }
      UTF8Flags result = UTF8Flags.NONE;
      if (encoding == null)
      {
        FileTypeUtil.GetUTF8Info(buffer, num, scanBytes, stream, out result);
        UTF8Flags utF8Flags = (UTF8Flags) 8390401;
        if ((result & utF8Flags) == UTF8Flags.NONASCII)
          encoding = FileTypeUtil.m_utf8NoBom;
      }
      if (checkForBinary && encoding == null)
      {
        if (isPdf)
          return (Encoding) null;
        for (int index = 0; index < num; ++index)
        {
          if ((buffer[index] <= (byte) 31 || buffer[index] == (byte) 127) && buffer[index] != (byte) 9 && buffer[index] != (byte) 12 && buffer[index] != (byte) 13 && buffer[index] != (byte) 10 && buffer[index] != (byte) 26)
            return (Encoding) null;
        }
      }
      return encoding ?? fallbackEncoding;
    }

    private static Encoding FindMatchingEncoding(byte[] buffer, int length, out bool isPdf)
    {
      isPdf = false;
      if (length >= 2)
      {
        if (buffer[0] == (byte) 254 && buffer[1] == byte.MaxValue)
          return FileTypeUtil.m_utf16Big;
        if (buffer[0] == byte.MaxValue && buffer[1] == (byte) 254)
          return length >= 4 && buffer[2] == (byte) 0 && buffer[3] == (byte) 0 ? FileTypeUtil.m_utf32Little : FileTypeUtil.m_utf16Little;
        if (length >= 3 && buffer[0] == (byte) 239 && buffer[1] == (byte) 187 && buffer[2] == (byte) 191)
          return FileTypeUtil.m_utf8;
        if (length >= 4 && buffer[0] == (byte) 0 && buffer[1] == (byte) 0 && buffer[2] == (byte) 254 && buffer[3] == byte.MaxValue)
          return FileTypeUtil.m_utf32Big;
        if (length >= FileTypeUtil.PdfMagic.Length && ArrayUtil.Equals(FileTypeUtil.PdfMagic, buffer, FileTypeUtil.PdfMagic.Length))
        {
          isPdf = true;
          return (Encoding) null;
        }
      }
      return (Encoding) null;
    }

    private static void GetUTF8Info(
      byte[] buffer,
      int bufferLength,
      long scanBytes,
      Stream stream,
      out UTF8Flags result)
    {
      result = UTF8Flags.NONE;
      int num1 = 0;
      int num2 = 0;
      int num3 = 0;
      int num4 = 0;
      int num5 = 0;
      long num6 = scanBytes;
      byte[] numArray = buffer;
      do
      {
        while (num5 < bufferLength)
        {
          byte num7 = buffer[num5++];
          int num8;
          if (num7 <= (byte) 127)
          {
            ++num4;
            if (num2 != 0)
            {
              result |= UTF8Flags.COUNT_NO_TRAIL;
              return;
            }
          }
          else if (((int) num7 & 64) == 0)
          {
            result |= UTF8Flags.NONASCII;
            if (num2 != 0)
            {
              num1 = num1 << 6 | (int) num7 & 63;
              --num2;
              if (num2 == 0)
              {
                ++num4;
                int num9 = 2;
                if (num1 < 128)
                  num9 = 1;
                else if (num1 >= 2048)
                {
                  if (num1 < 65536)
                    num9 = 3;
                  else if (num1 <= 1114111)
                    num9 = 4;
                }
                if (num3 > num9)
                  result |= UTF8Flags.OVERLONG;
                else if (num1 > (int) ushort.MaxValue)
                {
                  result |= UTF8Flags.UCS4;
                  if (num1 > 1114111)
                  {
                    result |= UTF8Flags.UCS4OUTOFRANGE;
                    return;
                  }
                  ++num4;
                }
              }
            }
            else
            {
              num8 = num4 + 1;
              result |= UTF8Flags.TRAIL_NO_COUNT;
              return;
            }
          }
          else
          {
            result |= UTF8Flags.NONASCII;
            if (num2 != 0)
            {
              result |= UTF8Flags.COUNT_NO_TRAIL;
              num8 = num4 + 1;
              return;
            }
            while (((int) num7 & 128) != 0)
            {
              num7 <<= 1;
              ++num2;
            }
            num1 = (int) num7 >> num2;
            num3 = num2;
            --num2;
          }
        }
        num6 -= (long) num5;
        if (num6 > 0L && bufferLength == buffer.Length)
        {
          if (buffer == numArray)
            buffer = new byte[1024];
          bufferLength = stream.Read(buffer, 0, buffer.Length);
          num5 = 0;
        }
        else
          bufferLength = 0;
      }
      while (bufferLength > 0);
      if (num6 <= 0L || num2 == 0)
        return;
      result |= UTF8Flags.COUNT_NO_TRAIL;
    }
  }
}
