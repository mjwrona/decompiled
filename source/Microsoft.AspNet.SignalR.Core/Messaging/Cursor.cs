// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.Messaging.Cursor
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace Microsoft.AspNet.SignalR.Messaging
{
  internal class Cursor
  {
    private static char[] _escapeChars = new char[3]
    {
      '\\',
      '|',
      ','
    };
    private string _escapedKey;

    public string Key { get; private set; }

    public ulong Id { get; set; }

    public static Cursor Clone(Cursor cursor) => new Cursor(cursor.Key, cursor.Id, cursor._escapedKey);

    public Cursor(string key, ulong id)
      : this(key, id, Cursor.Escape(key))
    {
    }

    public Cursor(string key, ulong id, string minifiedKey)
    {
      this.Key = key;
      this.Id = id;
      this._escapedKey = minifiedKey;
    }

    public static void WriteCursors(TextWriter textWriter, IList<Cursor> cursors, string prefix)
    {
      textWriter.Write(prefix);
      for (int index = 0; index < cursors.Count; ++index)
      {
        if (index > 0)
          textWriter.Write('|');
        Cursor cursor = cursors[index];
        textWriter.Write(cursor._escapedKey);
        textWriter.Write(',');
        Cursor.WriteUlongAsHexToBuffer(cursor.Id, textWriter);
      }
    }

    internal static void WriteUlongAsHexToBuffer(ulong value, TextWriter textWriter)
    {
      int num = 0;
      for (int index = 0; index < 16; ++index)
      {
        char hex = Cursor.Int32ToHex((int) (value >> 60));
        value <<= 4;
        if (num != 0 || hex != '0')
        {
          textWriter.Write(hex);
          ++num;
        }
      }
      if (num != 0)
        return;
      textWriter.Write('0');
    }

    private static char Int32ToHex(int value) => value >= 10 ? (char) (value - 10 + 65) : (char) (value + 48);

    private static string Escape(string value)
    {
      if (value.IndexOfAny(Cursor._escapeChars) == -1)
        return value;
      StringBuilder stringBuilder = new StringBuilder();
      foreach (char ch in value)
      {
        switch (ch)
        {
          case ',':
            stringBuilder.Append('\\').Append(ch);
            break;
          case '\\':
            stringBuilder.Append('\\').Append(ch);
            break;
          case '|':
            stringBuilder.Append('\\').Append(ch);
            break;
          default:
            stringBuilder.Append(ch);
            break;
        }
      }
      return stringBuilder.ToString();
    }

    public static List<Cursor> GetCursors(string cursor, string prefix) => Cursor.GetCursors(cursor, prefix, (Func<string, string>) (s => s));

    public static List<Cursor> GetCursors(
      string cursor,
      string prefix,
      Func<string, string> keyMaximizer)
    {
      return Cursor.GetCursors(cursor, prefix, (Func<string, object, string>) ((key, state) => ((Func<string, string>) state)(key)), (object) keyMaximizer);
    }

    public static List<Cursor> GetCursors(
      string cursor,
      string prefix,
      Func<string, object, string> keyMaximizer,
      object state)
    {
      if (string.IsNullOrEmpty(cursor))
        throw new FormatException(Resources.Error_InvalidCursorFormat);
      if (!cursor.StartsWith(prefix, StringComparison.Ordinal))
        return (List<Cursor>) null;
      HashSet<string> stringSet = new HashSet<string>();
      List<Cursor> cursors = new List<Cursor>();
      string key = (string) null;
      string minifiedKey = (string) null;
      bool flag1 = false;
      bool flag2 = true;
      StringBuilder sb = new StringBuilder();
      StringBuilder stringBuilder = new StringBuilder();
      ulong id;
      for (int length = prefix.Length; length < cursor.Length; ++length)
      {
        char ch = cursor[length];
        if (flag1)
        {
          if (ch != '\\' && ch != ',' && ch != '|')
            throw new FormatException(Resources.Error_InvalidCursorFormat);
          sb.Append(ch);
          stringBuilder.Append(ch);
          flag1 = false;
        }
        else
        {
          switch (ch)
          {
            case ',':
              if (!flag2)
                throw new FormatException(Resources.Error_InvalidCursorFormat);
              key = keyMaximizer(sb.ToString(), state);
              if (key == null)
                return (List<Cursor>) null;
              if (!stringSet.Add(key))
                throw new FormatException(Resources.Error_InvalidCursorFormat);
              minifiedKey = stringBuilder.ToString();
              sb.Clear();
              stringBuilder.Clear();
              flag2 = false;
              continue;
            case '\\':
              if (!flag2)
                throw new FormatException(Resources.Error_InvalidCursorFormat);
              stringBuilder.Append('\\');
              flag1 = true;
              continue;
            case '|':
              if (flag2)
                throw new FormatException(Resources.Error_InvalidCursorFormat);
              Cursor.ParseCursorId(sb, out id);
              Cursor cursor1 = new Cursor(key, id, minifiedKey);
              cursors.Add(cursor1);
              sb.Clear();
              flag2 = true;
              continue;
            default:
              sb.Append(ch);
              if (flag2)
              {
                stringBuilder.Append(ch);
                continue;
              }
              continue;
          }
        }
      }
      if (flag2)
        throw new FormatException(Resources.Error_InvalidCursorFormat);
      Cursor.ParseCursorId(sb, out id);
      Cursor cursor2 = new Cursor(key, id, minifiedKey);
      cursors.Add(cursor2);
      return cursors;
    }

    private static void ParseCursorId(StringBuilder sb, out ulong id)
    {
      string s = sb.ToString();
      id = ulong.Parse(s, NumberStyles.HexNumber, (IFormatProvider) CultureInfo.InvariantCulture);
    }

    public override string ToString() => this.Key;
  }
}
