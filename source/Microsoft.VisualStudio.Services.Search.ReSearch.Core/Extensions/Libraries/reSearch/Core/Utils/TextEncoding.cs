// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Core.Utils.TextEncoding
// Assembly: Microsoft.VisualStudio.Services.Search.ReSearch.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 657A74AE-F2A6-4615-BB2F-7FA1F961B173
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.ReSearch.Core.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.ReSearch.Core.Core.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Core.Utils
{
  public class TextEncoding
  {
    [StaticSafe]
    private static List<TextEncodingEntry> s_encodings = new List<TextEncodingEntry>();

    [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
    static TextEncoding()
    {
      TextEncoding.AddEncoding(Encoding.UTF8);
      TextEncoding.AddEncoding(Encoding.UTF7);
      TextEncoding.AddEncoding(Encoding.Unicode);
      TextEncoding.AddEncoding(Encoding.BigEndianUnicode);
      TextEncoding.AddEncoding(Encoding.UTF32);
      TextEncoding.AddEncoding(65006);
      TextEncoding.PrepareEncodings();
    }

    private static void AddEncoding(int codepage)
    {
      try
      {
        TextEncoding.AddEncoding(Encoding.GetEncoding(codepage));
      }
      catch (ArgumentException ex)
      {
      }
      catch (NotSupportedException ex)
      {
      }
    }

    private static void AddEncoding(Encoding encoding)
    {
      byte[] preamble = encoding.GetPreamble();
      if (preamble == null || preamble.Length == 0)
        return;
      TextEncoding.s_encodings.Add(new TextEncodingEntry(preamble, encoding));
    }

    private static void PrepareEncodings() => TextEncoding.s_encodings.Sort((Comparison<TextEncodingEntry>) ((enc1, enc2) => enc2.Preamble.Length - enc1.Preamble.Length));

    public Encoding GetEncodingWithBom(byte[] preamble)
    {
      if (preamble == null)
        return (Encoding) null;
      foreach (TextEncodingEntry encoding in TextEncoding.s_encodings)
      {
        byte[] preamble1 = encoding.Preamble;
        if (preamble.Length >= preamble1.Length)
        {
          bool flag = true;
          for (int index = 0; index < preamble1.Length; ++index)
          {
            if ((int) preamble[index] != (int) preamble1[index])
            {
              flag = false;
              break;
            }
          }
          if (flag)
            return encoding.Encoding;
        }
      }
      return (Encoding) null;
    }

    public virtual string GetString(byte[] bytes)
    {
      Encoding encoding = bytes != null ? this.GetEncoding(bytes, Encoding.Default) : throw new ArgumentNullException(nameof (bytes));
      int index = 0;
      byte[] preamble = encoding.GetPreamble();
      if (preamble != null)
        index = preamble.Length;
      int count = bytes.Length - index;
      return encoding.GetString(bytes, index, count);
    }

    public Encoding GetEncoding(byte[] bytes, Encoding defaultEncoding)
    {
      if (bytes == null)
        throw new ArgumentNullException(nameof (bytes));
      if (defaultEncoding == null)
        throw new ArgumentNullException(nameof (defaultEncoding));
      return this.GetEncodingWithBom(bytes) ?? ((TextEncoding.GetUTF8Info(bytes) & (UTF8Flags) 8390401) != UTF8Flags.NONASCII ? defaultEncoding : (Encoding) new UTF8Encoding(false));
    }

    private static UTF8Flags GetUTF8Info(byte[] content)
    {
      UTF8Flags utF8Info1 = UTF8Flags.NONE;
      int num1 = 0;
      int num2 = 0;
      int num3 = 0;
      int num4 = 0;
      long num5 = 0;
      while (num5 < (long) content.Length)
      {
        byte num6 = content[(IntPtr) num5++];
        int num7;
        if (num6 <= (byte) 127)
        {
          ++num4;
          if (num2 != 0)
            return utF8Info1 | UTF8Flags.COUNT_NO_TRAIL;
        }
        else if (((int) num6 & 64) == 0)
        {
          utF8Info1 |= UTF8Flags.NONASCII;
          if (num2 != 0)
          {
            num1 = num1 << 6 | (int) num6 & 63;
            --num2;
            if (num2 == 0)
            {
              ++num4;
              int num8 = 2;
              if (num1 < 128)
                num8 = 1;
              else if (num1 >= 2048)
              {
                if (num1 < 65536)
                  num8 = 3;
                else if (num1 <= 1114111)
                  num8 = 4;
              }
              if (num3 > num8)
                utF8Info1 |= UTF8Flags.OVERLONG;
              else if (num1 > (int) ushort.MaxValue)
              {
                utF8Info1 |= UTF8Flags.UCS4;
                if (num1 > 1114111)
                  return utF8Info1 | UTF8Flags.UCS4OUTOFRANGE;
                ++num4;
              }
            }
          }
          else
          {
            num7 = num4 + 1;
            return utF8Info1 | UTF8Flags.TRAIL_NO_COUNT;
          }
        }
        else
        {
          utF8Info1 |= UTF8Flags.NONASCII;
          if (num2 != 0)
          {
            UTF8Flags utF8Info2 = utF8Info1 | UTF8Flags.COUNT_NO_TRAIL;
            num7 = num4 + 1;
            return utF8Info2;
          }
          while (((int) num6 & 128) != 0)
          {
            num6 <<= 1;
            ++num2;
          }
          num1 = (int) num6 >> num2;
          num3 = num2;
          --num2;
        }
      }
      if (num2 != 0)
        utF8Info1 |= UTF8Flags.COUNT_NO_TRAIL;
      return utF8Info1;
    }
  }
}
