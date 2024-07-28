// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Common.JavaScriptObjectDeserializer
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using Microsoft.TeamFoundation.Common.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Microsoft.TeamFoundation.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class JavaScriptObjectDeserializer
  {
    private const int DefaultRecursionLimit = 100;
    private const string DateTimePrefix = "\"\\/Date(";
    private const int DateTimePrefixLength = 8;
    private const string DateTimeSuffix = "\\/\"";
    private const int DateTimeSuffixLength = 3;
    private const long DatetimeMinTimeTicks = 621355968000000000;
    private JavaScriptObjectDeserializer.JavaScriptString _s;
    private int _depthLimit;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static object BasicDeserialize(string input)
    {
      JavaScriptObjectDeserializer objectDeserializer = new JavaScriptObjectDeserializer(input, 100);
      object obj = objectDeserializer.DeserializeInternal(0);
      if (objectDeserializer._s.GetNextNonEmptyChar().HasValue)
        throw new ArgumentException(TFCommonResources.JavaScriptSerializer_JSON_IllegalPrimitive((object) objectDeserializer._s.ToString()));
      return obj;
    }

    private JavaScriptObjectDeserializer(string input, int depthLimit)
    {
      this._s = new JavaScriptObjectDeserializer.JavaScriptString(input);
      this._depthLimit = depthLimit;
    }

    private object DeserializeInternal(int depth)
    {
      if (++depth > this._depthLimit)
        throw new ArgumentException(this._s.GetDebugString(TFCommonResources.JavaScriptSerializer_JSON_DepthLimitExceeded()));
      char? nextNonEmptyChar = this._s.GetNextNonEmptyChar();
      if (!nextNonEmptyChar.HasValue)
        return (object) null;
      this._s.MovePrev();
      if (this.IsNextElementDateTime())
        return this.DeserializeStringIntoDateTime();
      if (JavaScriptObjectDeserializer.IsNextElementObject(nextNonEmptyChar))
        return (object) this.DeserializeDictionary(depth);
      if (JavaScriptObjectDeserializer.IsNextElementArray(nextNonEmptyChar))
        return (object) this.DeserializeList(depth);
      return JavaScriptObjectDeserializer.IsNextElementString(nextNonEmptyChar) ? (object) this.DeserializeString() : this.DeserializePrimitiveObject();
    }

    private IList DeserializeList(int depth)
    {
      IList list = (IList) new ArrayList();
      char? nullable1 = this._s.MoveNext();
      int? nullable2 = nullable1.HasValue ? new int?((int) nullable1.GetValueOrDefault()) : new int?();
      int num1 = 91;
      if (!(nullable2.GetValueOrDefault() == num1 & nullable2.HasValue))
        throw new ArgumentException(this._s.GetDebugString(TFCommonResources.JavaScriptSerializer_JSON_InvalidArrayStart()));
      bool flag = false;
      char? nextNonEmptyChar;
      char? nullable3;
      int num2;
      do
      {
        nullable3 = nextNonEmptyChar = this._s.GetNextNonEmptyChar();
        if (nullable3.HasValue)
        {
          nullable3 = nextNonEmptyChar;
          nullable2 = nullable3.HasValue ? new int?((int) nullable3.GetValueOrDefault()) : new int?();
          int num3 = 93;
          if (!(nullable2.GetValueOrDefault() == num3 & nullable2.HasValue))
          {
            this._s.MovePrev();
            object obj = this.DeserializeInternal(depth);
            list.Add(obj);
            flag = false;
            nextNonEmptyChar = this._s.GetNextNonEmptyChar();
            nullable3 = nextNonEmptyChar;
            nullable2 = nullable3.HasValue ? new int?((int) nullable3.GetValueOrDefault()) : new int?();
            int num4 = 93;
            if (!(nullable2.GetValueOrDefault() == num4 & nullable2.HasValue))
            {
              flag = true;
              nullable3 = nextNonEmptyChar;
              nullable2 = nullable3.HasValue ? new int?((int) nullable3.GetValueOrDefault()) : new int?();
              num2 = 44;
            }
            else
              goto label_8;
          }
          else
            goto label_8;
        }
        else
          goto label_8;
      }
      while (nullable2.GetValueOrDefault() == num2 & nullable2.HasValue);
      throw new ArgumentException(this._s.GetDebugString(TFCommonResources.JavaScriptSerializer_JSON_InvalidArrayExpectComma()));
label_8:
      if (flag)
        throw new ArgumentException(this._s.GetDebugString(TFCommonResources.JavaScriptSerializer_JSON_InvalidArrayExtraComma()));
      nullable3 = nextNonEmptyChar;
      nullable2 = nullable3.HasValue ? new int?((int) nullable3.GetValueOrDefault()) : new int?();
      int num5 = 93;
      if (!(nullable2.GetValueOrDefault() == num5 & nullable2.HasValue))
        throw new ArgumentException(this._s.GetDebugString(TFCommonResources.JavaScriptSerializer_JSON_InvalidArrayEnd()));
      return list;
    }

    private IDictionary<string, object> DeserializeDictionary(int depth)
    {
      IDictionary<string, object> dictionary = (IDictionary<string, object>) null;
      char? nullable1 = this._s.MoveNext();
      int? nullable2 = nullable1.HasValue ? new int?((int) nullable1.GetValueOrDefault()) : new int?();
      int num1 = 123;
      if (!(nullable2.GetValueOrDefault() == num1 & nullable2.HasValue))
        throw new ArgumentException(this._s.GetDebugString(TFCommonResources.JavaScriptSerializer_JSON_ExpectedOpenBrace()));
      char? nextNonEmptyChar;
      char? nullable3;
      int num2;
      do
      {
        nullable3 = nextNonEmptyChar = this._s.GetNextNonEmptyChar();
        if (nullable3.HasValue)
        {
          this._s.MovePrev();
          nullable3 = nextNonEmptyChar;
          nullable2 = nullable3.HasValue ? new int?((int) nullable3.GetValueOrDefault()) : new int?();
          int num3 = 58;
          if (!(nullable2.GetValueOrDefault() == num3 & nullable2.HasValue))
          {
            string key = (string) null;
            nullable3 = nextNonEmptyChar;
            nullable2 = nullable3.HasValue ? new int?((int) nullable3.GetValueOrDefault()) : new int?();
            int num4 = 125;
            if (!(nullable2.GetValueOrDefault() == num4 & nullable2.HasValue))
            {
              key = this.DeserializeMemberName();
              nullable3 = this._s.GetNextNonEmptyChar();
              nullable2 = nullable3.HasValue ? new int?((int) nullable3.GetValueOrDefault()) : new int?();
              int num5 = 58;
              if (!(nullable2.GetValueOrDefault() == num5 & nullable2.HasValue))
                goto label_6;
            }
            if (dictionary == null)
            {
              dictionary = (IDictionary<string, object>) new Dictionary<string, object>();
              if (key == null)
                goto label_9;
            }
            this.ThrowIfMaxJsonDeserializerMembersExceeded(dictionary.Count);
            object obj = this.DeserializeInternal(depth);
            dictionary[key] = obj;
            nextNonEmptyChar = this._s.GetNextNonEmptyChar();
            nullable3 = nextNonEmptyChar;
            nullable2 = nullable3.HasValue ? new int?((int) nullable3.GetValueOrDefault()) : new int?();
            int num6 = 125;
            if (!(nullable2.GetValueOrDefault() == num6 & nullable2.HasValue))
            {
              nullable3 = nextNonEmptyChar;
              nullable2 = nullable3.HasValue ? new int?((int) nullable3.GetValueOrDefault()) : new int?();
              num2 = 44;
            }
            else
              goto label_14;
          }
          else
            goto label_3;
        }
        else
          goto label_14;
      }
      while (nullable2.GetValueOrDefault() == num2 & nullable2.HasValue);
      goto label_12;
label_3:
      throw new ArgumentException(this._s.GetDebugString(TFCommonResources.JavaScriptSerializer_JSON_InvalidMemberName()));
label_6:
      throw new ArgumentException(this._s.GetDebugString(TFCommonResources.JavaScriptSerializer_JSON_InvalidObject()));
label_9:
      nextNonEmptyChar = this._s.GetNextNonEmptyChar();
      goto label_14;
label_12:
      throw new ArgumentException(this._s.GetDebugString(TFCommonResources.JavaScriptSerializer_JSON_InvalidObject()));
label_14:
      nullable3 = nextNonEmptyChar;
      nullable2 = nullable3.HasValue ? new int?((int) nullable3.GetValueOrDefault()) : new int?();
      int num7 = 125;
      if (!(nullable2.GetValueOrDefault() == num7 & nullable2.HasValue))
        throw new ArgumentException(this._s.GetDebugString(TFCommonResources.JavaScriptSerializer_JSON_InvalidObject()));
      return dictionary;
    }

    private void ThrowIfMaxJsonDeserializerMembersExceeded(int count)
    {
      if (count >= 1000)
        throw new InvalidOperationException();
    }

    private string DeserializeMemberName()
    {
      char? nextNonEmptyChar = this._s.GetNextNonEmptyChar();
      if (!nextNonEmptyChar.HasValue)
        return (string) null;
      this._s.MovePrev();
      return JavaScriptObjectDeserializer.IsNextElementString(nextNonEmptyChar) ? this.DeserializeString() : this.DeserializePrimitiveToken();
    }

    private object DeserializePrimitiveObject()
    {
      string s = this.DeserializePrimitiveToken();
      switch (s)
      {
        case "null":
          return (object) null;
        case "true":
          return (object) true;
        case "false":
          return (object) false;
        default:
          bool flag = s.IndexOf('.') >= 0;
          if (s.LastIndexOf("e", StringComparison.OrdinalIgnoreCase) < 0)
          {
            if (!flag)
            {
              int result1;
              if (int.TryParse(s, NumberStyles.Integer, (IFormatProvider) CultureInfo.InvariantCulture, out result1))
                return (object) result1;
              long result2;
              if (long.TryParse(s, NumberStyles.Integer, (IFormatProvider) CultureInfo.InvariantCulture, out result2))
                return (object) result2;
            }
            Decimal result;
            if (Decimal.TryParse(s, NumberStyles.Number, (IFormatProvider) CultureInfo.InvariantCulture, out result))
              return (object) result;
          }
          double result3;
          if (double.TryParse(s, NumberStyles.Float, (IFormatProvider) CultureInfo.InvariantCulture, out result3))
            return (object) result3;
          throw new ArgumentException(TFCommonResources.JavaScriptSerializer_JSON_IllegalPrimitive((object) s));
      }
    }

    private string DeserializePrimitiveToken()
    {
      StringBuilder stringBuilder = new StringBuilder();
      char? nullable1 = new char?();
      char? nullable2;
      while ((nullable2 = this._s.MoveNext()).HasValue)
      {
        if (char.IsLetterOrDigit(nullable2.Value) || nullable2.Value == '.' || nullable2.Value == '-' || nullable2.Value == '_' || nullable2.Value == '+')
        {
          stringBuilder.Append(nullable2.Value);
        }
        else
        {
          this._s.MovePrev();
          break;
        }
      }
      return stringBuilder.ToString();
    }

    private string DeserializeString()
    {
      StringBuilder sb = new StringBuilder();
      bool flag = false;
      char ch = this.CheckQuoteChar(this._s.MoveNext());
      char? c;
      while ((c = this._s.MoveNext()).HasValue)
      {
        char? nullable1 = c;
        int? nullable2 = nullable1.HasValue ? new int?((int) nullable1.GetValueOrDefault()) : new int?();
        int num1 = 92;
        if (nullable2.GetValueOrDefault() == num1 & nullable2.HasValue)
        {
          if (flag)
          {
            sb.Append('\\');
            flag = false;
          }
          else
            flag = true;
        }
        else if (flag)
        {
          this.AppendCharToBuilder(c, sb);
          flag = false;
        }
        else
        {
          char? nullable3 = c;
          nullable2 = nullable3.HasValue ? new int?((int) nullable3.GetValueOrDefault()) : new int?();
          int num2 = (int) ch;
          if (nullable2.GetValueOrDefault() == num2 & nullable2.HasValue)
            return sb.ToString();
          sb.Append(c.Value);
        }
      }
      throw new ArgumentException(this._s.GetDebugString(TFCommonResources.JavaScriptSerializer_JSON_UnterminatedString()));
    }

    private void AppendCharToBuilder(char? c, StringBuilder sb)
    {
      char? nullable1 = c;
      int? nullable2 = nullable1.HasValue ? new int?((int) nullable1.GetValueOrDefault()) : new int?();
      int num1 = 34;
      if (!(nullable2.GetValueOrDefault() == num1 & nullable2.HasValue))
      {
        char? nullable3 = c;
        nullable2 = nullable3.HasValue ? new int?((int) nullable3.GetValueOrDefault()) : new int?();
        int num2 = 39;
        if (!(nullable2.GetValueOrDefault() == num2 & nullable2.HasValue))
        {
          char? nullable4 = c;
          nullable2 = nullable4.HasValue ? new int?((int) nullable4.GetValueOrDefault()) : new int?();
          int num3 = 47;
          if (!(nullable2.GetValueOrDefault() == num3 & nullable2.HasValue))
          {
            char? nullable5 = c;
            nullable2 = nullable5.HasValue ? new int?((int) nullable5.GetValueOrDefault()) : new int?();
            int num4 = 98;
            if (nullable2.GetValueOrDefault() == num4 & nullable2.HasValue)
            {
              sb.Append('\b');
              return;
            }
            char? nullable6 = c;
            nullable2 = nullable6.HasValue ? new int?((int) nullable6.GetValueOrDefault()) : new int?();
            int num5 = 102;
            if (nullable2.GetValueOrDefault() == num5 & nullable2.HasValue)
            {
              sb.Append('\f');
              return;
            }
            char? nullable7 = c;
            nullable2 = nullable7.HasValue ? new int?((int) nullable7.GetValueOrDefault()) : new int?();
            int num6 = 110;
            if (nullable2.GetValueOrDefault() == num6 & nullable2.HasValue)
            {
              sb.Append('\n');
              return;
            }
            char? nullable8 = c;
            nullable2 = nullable8.HasValue ? new int?((int) nullable8.GetValueOrDefault()) : new int?();
            int num7 = 114;
            if (nullable2.GetValueOrDefault() == num7 & nullable2.HasValue)
            {
              sb.Append('\r');
              return;
            }
            char? nullable9 = c;
            nullable2 = nullable9.HasValue ? new int?((int) nullable9.GetValueOrDefault()) : new int?();
            int num8 = 116;
            if (nullable2.GetValueOrDefault() == num8 & nullable2.HasValue)
            {
              sb.Append('\t');
              return;
            }
            char? nullable10 = c;
            nullable2 = nullable10.HasValue ? new int?((int) nullable10.GetValueOrDefault()) : new int?();
            int num9 = 117;
            if (!(nullable2.GetValueOrDefault() == num9 & nullable2.HasValue))
              throw new ArgumentException(this._s.GetDebugString(TFCommonResources.JavaScriptSerializer_JSON_BadEscape()));
            sb.Append((char) int.Parse(this._s.MoveNext(4), NumberStyles.HexNumber, (IFormatProvider) CultureInfo.InvariantCulture));
            return;
          }
        }
      }
      sb.Append(c.Value);
    }

    private char CheckQuoteChar(char? c)
    {
      char ch = '"';
      char? nullable1 = c;
      int? nullable2 = nullable1.HasValue ? new int?((int) nullable1.GetValueOrDefault()) : new int?();
      int num1 = 39;
      if (nullable2.GetValueOrDefault() == num1 & nullable2.HasValue)
      {
        ch = c.Value;
      }
      else
      {
        char? nullable3 = c;
        nullable2 = nullable3.HasValue ? new int?((int) nullable3.GetValueOrDefault()) : new int?();
        int num2 = 34;
        if (!(nullable2.GetValueOrDefault() == num2 & nullable2.HasValue))
          throw new ArgumentException(this._s.GetDebugString(TFCommonResources.JavaScriptSerializer_JSON_StringNotQuoted()));
      }
      return ch;
    }

    private object DeserializeStringIntoDateTime()
    {
      Match match = Regex.Match(this._s.Substring(this._s.IndexOf("\\/\"") + 3), "^\"\\\\/Date\\((?<ticks>-?[0-9]+)(?:[a-zA-Z]|(?:\\+|-)[0-9]{4})?\\)\\\\/\"");
      long result;
      if (!long.TryParse(match.Groups["ticks"].Value, out result))
        return (object) this.DeserializeString();
      this._s.MoveNext(match.Length);
      return (object) new DateTime(result * 10000L + 621355968000000000L, DateTimeKind.Utc);
    }

    private static bool IsNextElementArray(char? c)
    {
      char? nullable1 = c;
      int? nullable2 = nullable1.HasValue ? new int?((int) nullable1.GetValueOrDefault()) : new int?();
      int num = 91;
      return nullable2.GetValueOrDefault() == num & nullable2.HasValue;
    }

    private bool IsNextElementDateTime()
    {
      string a = this._s.MoveNext(8);
      if (a == null)
        return false;
      this._s.MovePrev(8);
      return string.Equals(a, "\"\\/Date(", StringComparison.Ordinal);
    }

    private static bool IsNextElementObject(char? c)
    {
      char? nullable1 = c;
      int? nullable2 = nullable1.HasValue ? new int?((int) nullable1.GetValueOrDefault()) : new int?();
      int num = 123;
      return nullable2.GetValueOrDefault() == num & nullable2.HasValue;
    }

    private static bool IsNextElementString(char? c)
    {
      char? nullable1 = c;
      int? nullable2 = nullable1.HasValue ? new int?((int) nullable1.GetValueOrDefault()) : new int?();
      int num1 = 34;
      if (nullable2.GetValueOrDefault() == num1 & nullable2.HasValue)
        return true;
      char? nullable3 = c;
      nullable2 = nullable3.HasValue ? new int?((int) nullable3.GetValueOrDefault()) : new int?();
      int num2 = 39;
      return nullable2.GetValueOrDefault() == num2 & nullable2.HasValue;
    }

    private class JavaScriptString
    {
      private string _s;
      private int _index;

      internal JavaScriptString(string s) => this._s = s;

      internal char? GetNextNonEmptyChar()
      {
        while (this._s.Length > this._index)
        {
          char c = this._s[this._index++];
          if (!char.IsWhiteSpace(c))
            return new char?(c);
        }
        return new char?();
      }

      internal char? MoveNext() => this._s.Length > this._index ? new char?(this._s[this._index++]) : new char?();

      internal string MoveNext(int count)
      {
        if (this._s.Length < this._index + count)
          return (string) null;
        string str = this._s.Substring(this._index, count);
        this._index += count;
        return str;
      }

      internal void MovePrev()
      {
        if (this._index <= 0)
          return;
        --this._index;
      }

      internal void MovePrev(int count)
      {
        for (; this._index > 0 && count > 0; --count)
          --this._index;
      }

      public override string ToString() => this._s.Length > this._index ? this._s.Substring(this._index) : string.Empty;

      internal string GetDebugString(string message) => message + " (" + this._index.ToString() + "): " + this._s;

      internal int IndexOf(string substr) => this._s.Length > this._index ? this._s.IndexOf(substr, this._index, StringComparison.CurrentCulture) - this._index : -1;

      internal string Substring(int length) => this._s.Length > this._index + length ? this._s.Substring(this._index, length) : this.ToString();
    }
  }
}
