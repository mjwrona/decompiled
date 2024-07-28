// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Expressions.EvaluationResult
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.TeamFoundation.DistributedTask.Expressions.CollectionAccessors;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;

namespace Microsoft.TeamFoundation.DistributedTask.Expressions
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class EvaluationResult
  {
    private static readonly NumberStyles s_numberStyles = NumberStyles.Integer | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands;
    private static readonly Lazy<JsonSerializer> s_serializer = new Lazy<JsonSerializer>((Func<JsonSerializer>) (() => JsonUtility.CreateJsonSerializer()));
    private readonly int m_level;
    private readonly bool m_omitTracing;

    internal EvaluationResult(
      EvaluationContext context,
      int level,
      object val,
      ValueKind kind,
      object raw)
      : this(context, level, val, kind, raw, false)
    {
    }

    internal EvaluationResult(
      EvaluationContext context,
      int level,
      object val,
      ValueKind kind,
      object raw,
      bool omitTracing)
    {
      this.m_level = level;
      this.Value = val;
      this.Kind = kind;
      this.Raw = raw;
      this.m_omitTracing = omitTracing;
      if (omitTracing)
        return;
      this.TraceValue(context);
    }

    public ValueKind Kind { get; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public object Raw { get; }

    public object Value { get; }

    public int CompareTo(EvaluationContext context, EvaluationResult right)
    {
      object number1;
      ValueKind valueKind;
      switch (this.Kind)
      {
        case ValueKind.Boolean:
        case ValueKind.DateTime:
        case ValueKind.Number:
        case ValueKind.String:
        case ValueKind.Version:
          number1 = this.Value;
          valueKind = this.Kind;
          break;
        default:
          number1 = (object) this.ConvertToNumber(context);
          valueKind = ValueKind.Number;
          break;
      }
      switch (valueKind)
      {
        case ValueKind.Boolean:
          bool boolean = right.ConvertToBoolean(context);
          return ((bool) number1).CompareTo(boolean);
        case ValueKind.DateTime:
          DateTimeOffset dateTime = right.ConvertToDateTime(context);
          return ((DateTimeOffset) number1).CompareTo(dateTime);
        case ValueKind.Number:
          Decimal number2 = right.ConvertToNumber(context);
          return ((Decimal) number1).CompareTo(number2);
        case ValueKind.String:
          string str = right.ConvertToString(context);
          if (!(number1 is string strA))
            strA = string.Empty;
          string strB = str ?? string.Empty;
          return string.Compare(strA, strB, StringComparison.OrdinalIgnoreCase);
        default:
          Version version = right.ConvertToVersion(context);
          return (number1 as Version).CompareTo(version);
      }
    }

    public bool ConvertToBoolean(EvaluationContext context)
    {
      switch (this.Kind)
      {
        case ValueKind.Array:
        case ValueKind.DateTime:
        case ValueKind.Object:
        case ValueKind.Version:
          bool val1 = true;
          this.TraceValue(context, (object) val1, ValueKind.Boolean);
          return val1;
        case ValueKind.Boolean:
          return (bool) this.Value;
        case ValueKind.Null:
          bool val2 = false;
          this.TraceValue(context, (object) val2, ValueKind.Boolean);
          return val2;
        case ValueKind.Number:
          bool val3 = (Decimal) this.Value != 0M;
          this.TraceValue(context, (object) val3, ValueKind.Boolean);
          return val3;
        case ValueKind.String:
          bool val4 = !string.IsNullOrEmpty(this.Value as string);
          this.TraceValue(context, (object) val4, ValueKind.Boolean);
          return val4;
        default:
          throw new NotSupportedException(string.Format("Unable to convert value to Boolean. Unexpected value kind '{0}'.", (object) this.Kind));
      }
    }

    public DateTimeOffset ConvertToDateTime(EvaluationContext context)
    {
      DateTimeOffset result;
      if (this.TryConvertToDateTime(context, out result))
        return result;
      throw new TypeCastException(context?.SecretMasker, this.Value, this.Kind, ValueKind.DateTime);
    }

    public object ConvertToNull(EvaluationContext context)
    {
      object result;
      if (this.TryConvertToNull(context, out result))
        return result;
      throw new TypeCastException(context?.SecretMasker, this.Value, this.Kind, ValueKind.Null);
    }

    public Decimal ConvertToNumber(EvaluationContext context)
    {
      Decimal result;
      if (this.TryConvertToNumber(context, out result))
        return result;
      throw new TypeCastException(context?.SecretMasker, this.Value, this.Kind, ValueKind.Number);
    }

    public string ConvertToString(EvaluationContext context)
    {
      string result;
      if (this.TryConvertToString(context, out result))
        return result;
      throw new TypeCastException(context?.SecretMasker, this.Value, this.Kind, ValueKind.String);
    }

    public Version ConvertToVersion(EvaluationContext context)
    {
      Version result;
      if (this.TryConvertToVersion(context, out result))
        return result;
      throw new TypeCastException(context?.SecretMasker, this.Value, this.Kind, ValueKind.Version);
    }

    public bool Equals(EvaluationContext context, EvaluationResult right)
    {
      if (this.Kind == ValueKind.Boolean)
        return (bool) this.Value == right.ConvertToBoolean(context);
      if (this.Kind == ValueKind.DateTime)
      {
        DateTimeOffset result;
        if (right.TryConvertToDateTime(context, out result))
          return (DateTimeOffset) this.Value == result;
      }
      else if (this.Kind == ValueKind.Number)
      {
        Decimal result;
        if (right.TryConvertToNumber(context, out result))
          return (Decimal) this.Value == result;
      }
      else if (this.Kind == ValueKind.Version)
      {
        Version result;
        if (right.TryConvertToVersion(context, out result))
          return (Version) this.Value == result;
      }
      else if (this.Kind == ValueKind.String)
      {
        string result;
        if (right.TryConvertToString(context, out result))
        {
          if (!(this.Value is string empty))
            empty = string.Empty;
          string b = result ?? string.Empty;
          return string.Equals(empty, b, StringComparison.OrdinalIgnoreCase);
        }
      }
      else
      {
        if (this.Kind == ValueKind.Array || this.Kind == ValueKind.Object)
          return this.Kind == right.Kind && this.Value == right.Value;
        if (this.Kind == ValueKind.Null && right.TryConvertToNull(context, out object _))
          return true;
      }
      return false;
    }

    public bool TryConvertToDateTime(EvaluationContext context, out DateTimeOffset result)
    {
      switch (this.Kind)
      {
        case ValueKind.Array:
        case ValueKind.Boolean:
        case ValueKind.Null:
        case ValueKind.Number:
        case ValueKind.Object:
        case ValueKind.Version:
          result = new DateTimeOffset();
          this.TraceCoercionFailed(context, ValueKind.DateTime);
          return false;
        case ValueKind.DateTime:
          result = (DateTimeOffset) this.Value;
          return true;
        case ValueKind.String:
          if (EvaluationResult.TryParseDateTime(context?.Options, this.Value as string, out result))
          {
            this.TraceValue(context, (object) result, ValueKind.DateTime);
            return true;
          }
          this.TraceCoercionFailed(context, ValueKind.DateTime);
          return false;
        default:
          throw new NotSupportedException(string.Format("Unable to determine whether value can be converted to Number. Unexpected value kind '{0}'.", (object) this.Kind));
      }
    }

    public bool TryConvertToNull(EvaluationContext context, out object result)
    {
      switch (this.Kind)
      {
        case ValueKind.Null:
          result = (object) null;
          return true;
        case ValueKind.String:
          if (string.IsNullOrEmpty(this.Value as string))
          {
            result = (object) null;
            this.TraceValue(context, result, ValueKind.Null);
            return true;
          }
          break;
      }
      result = (object) null;
      this.TraceCoercionFailed(context, ValueKind.Null);
      return false;
    }

    public bool TryConvertToNumber(EvaluationContext context, out Decimal result)
    {
      switch (this.Kind)
      {
        case ValueKind.Array:
        case ValueKind.DateTime:
        case ValueKind.Object:
        case ValueKind.Version:
          result = 0M;
          this.TraceCoercionFailed(context, ValueKind.Number);
          return false;
        case ValueKind.Boolean:
          result = (bool) this.Value ? 1M : 0M;
          this.TraceValue(context, (object) result, ValueKind.Number);
          return true;
        case ValueKind.Null:
          result = 0M;
          this.TraceValue(context, (object) result, ValueKind.Number);
          return true;
        case ValueKind.Number:
          result = (Decimal) this.Value;
          return true;
        case ValueKind.String:
          if (!(this.Value is string empty))
            empty = string.Empty;
          string s = empty;
          if (string.IsNullOrEmpty(s))
          {
            result = 0M;
            this.TraceValue(context, (object) result, ValueKind.Number);
            return true;
          }
          if (Decimal.TryParse(s, EvaluationResult.s_numberStyles, (IFormatProvider) CultureInfo.InvariantCulture, out result))
          {
            this.TraceValue(context, (object) result, ValueKind.Number);
            return true;
          }
          this.TraceCoercionFailed(context, ValueKind.Number);
          return false;
        default:
          throw new NotSupportedException(string.Format("Unable to determine whether value can be converted to Number. Unexpected value kind '{0}'.", (object) this.Kind));
      }
    }

    public bool TryConvertToString(EvaluationContext context, out string result)
    {
      switch (this.Kind)
      {
        case ValueKind.Array:
        case ValueKind.Object:
          result = (string) null;
          this.TraceCoercionFailed(context, ValueKind.String);
          return false;
        case ValueKind.Boolean:
          result = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}", this.Value);
          this.TraceValue(context, (object) result, ValueKind.String);
          return true;
        case ValueKind.DateTime:
          result = ((DateTimeOffset) this.Value).ToString(ExpressionConstants.DateTimeFormat, (IFormatProvider) CultureInfo.InvariantCulture);
          this.TraceValue(context, (object) result, ValueKind.String);
          return true;
        case ValueKind.Null:
          result = string.Empty;
          this.TraceValue(context, (object) result, ValueKind.Null);
          return true;
        case ValueKind.Number:
          result = ((Decimal) this.Value).ToString(ExpressionConstants.NumberFormat, (IFormatProvider) CultureInfo.InvariantCulture);
          this.TraceValue(context, (object) result, ValueKind.String);
          return true;
        case ValueKind.String:
          result = this.Value as string;
          return true;
        case ValueKind.Version:
          result = (this.Value as Version).ToString();
          this.TraceValue(context, (object) result, ValueKind.String);
          return true;
        default:
          throw new NotSupportedException(string.Format("Unable to convert to String. Unexpected value kind '{0}'.", (object) this.Kind));
      }
    }

    public bool TryConvertToVersion(EvaluationContext context, out Version result)
    {
      switch (this.Kind)
      {
        case ValueKind.Array:
        case ValueKind.DateTime:
        case ValueKind.Null:
        case ValueKind.Object:
          result = (Version) null;
          this.TraceCoercionFailed(context, ValueKind.Version);
          return false;
        case ValueKind.Boolean:
          result = (Version) null;
          this.TraceCoercionFailed(context, ValueKind.Version);
          return false;
        case ValueKind.Number:
          if (Version.TryParse(this.ConvertToString(context), out result))
          {
            this.TraceValue(context, (object) result, ValueKind.Version);
            return true;
          }
          this.TraceCoercionFailed(context, ValueKind.Version);
          return false;
        case ValueKind.String:
          if (!(this.Value is string empty))
            empty = string.Empty;
          if (Version.TryParse(empty, out result))
          {
            this.TraceValue(context, (object) result, ValueKind.Version);
            return true;
          }
          this.TraceCoercionFailed(context, ValueKind.Version);
          return false;
        case ValueKind.Version:
          result = this.Value as Version;
          return true;
        default:
          throw new NotSupportedException(string.Format("Unable to convert to Version. Unexpected value kind '{0}'.", (object) this.Kind));
      }
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool TryGetCollectionInterface(out object collection)
    {
      if (this.Kind == ValueKind.Object || this.Kind == ValueKind.Array)
      {
        object obj = this.Value;
        switch (obj)
        {
          case IReadOnlyObject _:
            collection = obj;
            return true;
          case IDictionary<string, string> dictionary1:
            collection = (object) new ReadOnlyDictionaryOfStringStringAccessor((IReadOnlyDictionary<string, string>) new ReadOnlyDictionary<string, string>(dictionary1));
            return true;
          case IDictionary<string, object> dictionary2:
            collection = (object) new ReadOnlyDictionaryOfStringObjectAccessor((IReadOnlyDictionary<string, object>) new ReadOnlyDictionary<string, object>(dictionary2));
            return true;
          case IReadOnlyDictionary<string, string> dictionary3:
            collection = (object) new ReadOnlyDictionaryOfStringStringAccessor(dictionary3);
            return true;
          case IReadOnlyDictionary<string, object> dictionary4:
            collection = (object) new ReadOnlyDictionaryOfStringObjectAccessor(dictionary4);
            return true;
          case JObject jobject:
            collection = (object) new JObjectAccessor(jobject);
            return true;
          case IReadOnlyArray _:
            collection = obj;
            return true;
          case IList<object> list1:
            collection = (object) new ListOfObjectAccessor(list1);
            return true;
          case IReadOnlyList<object> list2:
            collection = (object) new ReadOnlyListOfObjectAccessor(list2);
            return true;
          case JArray jarray:
            collection = (object) new JArrayAccessor(jarray);
            return true;
          default:
            switch (EvaluationResult.s_serializer.Value.ContractResolver.ResolveContract(obj.GetType()))
            {
              case JsonObjectContract contract1:
                collection = (object) new JsonObjectContractAccessor(contract1, obj);
                return true;
              case JsonDictionaryContract contract2:
                if (contract2.DictionaryKeyType == typeof (string))
                {
                  collection = (object) new JsonDictionaryContractAccessor(contract2, obj);
                  return true;
                }
                break;
            }
            break;
        }
      }
      collection = (object) null;
      return false;
    }

    public static EvaluationResult CreateIntermediateResult(
      EvaluationContext context,
      object obj,
      out ResultMemory conversionResultMemory)
    {
      ValueKind kind;
      object raw;
      object canonicalValue = ExpressionUtil.ConvertToCanonicalValue(context?.Options, obj, out kind, out raw, out conversionResultMemory);
      return new EvaluationResult(context, 0, canonicalValue, kind, raw, true);
    }

    private void TraceCoercionFailed(EvaluationContext context, ValueKind toKind)
    {
      if (this.m_omitTracing)
        return;
      this.TraceVerbose(context, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "=> Unable to coerce {0} to {1}.", (object) this.Kind, (object) toKind));
    }

    private void TraceValue(EvaluationContext context)
    {
      if (this.m_omitTracing)
        return;
      this.TraceValue(context, this.Value, this.Kind);
    }

    private void TraceValue(EvaluationContext context, object val, ValueKind kind)
    {
      if (this.m_omitTracing)
        return;
      this.TraceVerbose(context, "=> " + ExpressionUtil.FormatValue(context?.SecretMasker, val, kind));
    }

    private void TraceVerbose(EvaluationContext context, string message)
    {
      if (this.m_omitTracing || context == null)
        return;
      context.Trace.Verbose(string.Empty.PadLeft(this.m_level * 2, '.') + (message ?? string.Empty));
    }

    private static bool TryParseDateTime(
      EvaluationOptions options,
      string s,
      out DateTimeOffset result)
    {
      if (string.IsNullOrEmpty(s))
      {
        result = new DateTimeOffset();
        return false;
      }
      s = s.Trim();
      int index = 0;
      int result1;
      if (EvaluationResult.ReadInt32(s, 4, 4, ref index, out result1))
      {
        char separator;
        int result2;
        if (EvaluationResult.ReadSeparator(s, ref index, new char[2]
        {
          '-',
          '/'
        }, out separator) && EvaluationResult.ReadInt32(s, 1, 2, ref index, out result2))
        {
          int result3;
          if (EvaluationResult.ReadSeparator(s, ref index, separator) && EvaluationResult.ReadInt32(s, 1, 2, ref index, out result3))
          {
            int result4;
            if (EvaluationResult.ReadSeparator(s, ref index, ' ', 'T') && EvaluationResult.ReadInt32(s, 1, 2, ref index, out result4))
            {
              int result5;
              if (EvaluationResult.ReadSeparator(s, ref index, ':') && EvaluationResult.ReadInt32(s, 1, 2, ref index, out result5))
              {
                int result6;
                if (EvaluationResult.ReadSeparator(s, ref index, ':') && EvaluationResult.ReadInt32(s, 1, 2, ref index, out result6))
                {
                  int ticks;
                  if (ExpressionUtil.SafeCharAt(s, index) == '.')
                  {
                    ++index;
                    string result7;
                    if (!EvaluationResult.ReadDigits(s, 1, 7, ref index, out result7))
                    {
                      result = new DateTimeOffset();
                      return false;
                    }
                    if (result7.Length < 7)
                      result7 = result7.PadRight(7, '0');
                    ticks = int.Parse(result7, NumberStyles.None, (IFormatProvider) CultureInfo.InvariantCulture);
                  }
                  else
                    ticks = 0;
                  TimeSpan offset;
                  if (index >= s.Length)
                  {
                    TimeZoneInfo timeZoneInfo = options?.TimeZone ?? TimeZoneInfo.Local;
                    try
                    {
                      DateTime dateTime = new DateTime(result1, result2, result3, result4, result5, result6, DateTimeKind.Unspecified);
                      offset = timeZoneInfo.GetUtcOffset(dateTime);
                    }
                    catch
                    {
                      result = new DateTimeOffset();
                      return false;
                    }
                  }
                  else if (!EvaluationResult.ReadOffset(s, ref index, out offset) || index < s.Length)
                  {
                    result = new DateTimeOffset();
                    return false;
                  }
                  try
                  {
                    result = new DateTimeOffset(result1, result2, result3, result4, result5, result6, offset);
                  }
                  catch
                  {
                    result = new DateTimeOffset();
                    return false;
                  }
                  if (ticks > 0)
                    result = result.AddTicks((long) ticks);
                  return true;
                }
              }
            }
          }
        }
      }
      result = new DateTimeOffset();
      return false;
    }

    private static bool ReadDigits(
      string str,
      int minLength,
      int maxLength,
      ref int index,
      out string result)
    {
      int startIndex = index;
      while (char.IsDigit(ExpressionUtil.SafeCharAt(str, index)))
        ++index;
      int length = index - startIndex;
      if (length < minLength || length > maxLength)
      {
        result = (string) null;
        return false;
      }
      result = str.Substring(startIndex, length);
      return true;
    }

    private static bool ReadInt32(
      string str,
      int minLength,
      int maxLength,
      ref int index,
      out int result)
    {
      string result1;
      if (!EvaluationResult.ReadDigits(str, minLength, maxLength, ref index, out result1))
      {
        result = 0;
        return false;
      }
      result = int.Parse(result1, NumberStyles.None, (IFormatProvider) CultureInfo.InvariantCulture);
      return true;
    }

    private static bool ReadSeparator(string str, ref int index, params char[] allowed) => EvaluationResult.ReadSeparator(str, ref index, allowed, out char _);

    private static bool ReadSeparator(
      string str,
      ref int index,
      char[] allowed,
      out char separator)
    {
      separator = ExpressionUtil.SafeCharAt(str, index++);
      foreach (char ch in allowed)
      {
        if ((int) separator == (int) ch)
          return true;
      }
      separator = char.MinValue;
      return false;
    }

    private static bool ReadOffset(string str, ref int index, out TimeSpan offset)
    {
      if (ExpressionUtil.SafeCharAt(str, index) == 'Z')
      {
        ++index;
        offset = TimeSpan.Zero;
        return true;
      }
      bool flag;
      if (ExpressionUtil.SafeCharAt(str, index) == '-')
      {
        ++index;
        flag = true;
      }
      else if (ExpressionUtil.SafeCharAt(str, index) == '+')
      {
        ++index;
        flag = false;
      }
      else
      {
        offset = new TimeSpan();
        return false;
      }
      int result1;
      if (EvaluationResult.ReadInt32(str, 1, 2, ref index, out result1))
      {
        int result2;
        if (EvaluationResult.ReadSeparator(str, ref index, ':') && EvaluationResult.ReadInt32(str, 1, 2, ref index, out result2))
        {
          offset = !flag ? new TimeSpan(result1, result2, 0) : TimeSpan.Zero.Subtract(new TimeSpan(result1, result2, 0));
          return true;
        }
      }
      offset = new TimeSpan();
      return false;
    }
  }
}
