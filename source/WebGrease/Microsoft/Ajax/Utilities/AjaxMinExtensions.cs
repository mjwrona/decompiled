// Decompiled with JetBrains decompiler
// Type: Microsoft.Ajax.Utilities.AjaxMinExtensions
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.Ajax.Utilities
{
  public static class AjaxMinExtensions
  {
    public static string FormatInvariant(this string format, params object[] args)
    {
      try
      {
        return format == null ? string.Empty : string.Format((IFormatProvider) CultureInfo.InvariantCulture, format, args);
      }
      catch (FormatException ex)
      {
        return format;
      }
    }

    public static bool TryParseSingleInvariant(this string text, out float number)
    {
      try
      {
        number = Convert.ToSingle(text, (IFormatProvider) CultureInfo.InvariantCulture);
        return true;
      }
      catch (FormatException ex)
      {
        number = float.NaN;
        return false;
      }
      catch (OverflowException ex)
      {
        number = float.NaN;
        return false;
      }
    }

    public static bool TryParseIntInvariant(
      this string text,
      NumberStyles numberStyles,
      out int number)
    {
      number = 0;
      return text != null && int.TryParse(text, numberStyles, (IFormatProvider) CultureInfo.InvariantCulture, out number);
    }

    public static bool TryParseLongInvariant(
      this string text,
      NumberStyles numberStyles,
      out long number)
    {
      number = 0L;
      return text != null && long.TryParse(text, numberStyles, (IFormatProvider) CultureInfo.InvariantCulture, out number);
    }

    public static bool IsNullOrWhiteSpace(this string text) => string.IsNullOrWhiteSpace(text);

    public static string IfNullOrWhiteSpace(this string text, string defaultValue) => !string.IsNullOrWhiteSpace(text) ? text : defaultValue;

    public static string SubstringUpToFirst(this string text, char delimiter)
    {
      if (text == null)
        return (string) null;
      int length = text.IndexOf(delimiter);
      return length >= 0 ? text.Substring(0, length) : text;
    }

    public static string ToStringInvariant(this int number, string format) => format != null ? number.ToString(format, (IFormatProvider) CultureInfo.InvariantCulture) : number.ToString((IFormatProvider) CultureInfo.InvariantCulture);

    public static string ToStringInvariant(this double number, string format) => format != null ? number.ToString(format, (IFormatProvider) CultureInfo.InvariantCulture) : number.ToString((IFormatProvider) CultureInfo.InvariantCulture);

    public static string ToStringInvariant(this int number) => number.ToStringInvariant((string) null);

    public static IEnumerable<TSource> DistinctBy<TSource, TKey>(
      this IEnumerable<TSource> source,
      Func<TSource, TKey> keySelector)
    {
      HashSet<TKey> hash = new HashSet<TKey>();
      return source.Where<TSource>((Func<TSource, bool>) (p => hash.Add(keySelector(p))));
    }

    public static void ForEach<TObject>(
      this IEnumerable<TObject> collection,
      Action<TObject> action)
    {
      if (action == null)
        throw new ArgumentNullException(nameof (action));
      if (collection == null)
        return;
      foreach (TObject @object in collection)
        @object.IfNotNull<TObject>((Action<TObject>) (i => action(i)));
    }

    public static TResult IfNotNull<TObject, TResult>(
      this TObject obj,
      Func<TObject, TResult> action)
    {
      if (action == null)
        throw new ArgumentNullException(nameof (action));
      return (object) obj != null ? action(obj) : default (TResult);
    }

    public static TResult IfNotNull<TObject, TResult>(
      this TObject obj,
      Func<TObject, TResult> action,
      TResult defaultValue)
    {
      if (action == null)
        throw new ArgumentNullException(nameof (action));
      return (object) obj != null ? action(obj) : defaultValue;
    }

    public static void IfNotNull<TObject>(this TObject obj, Action<TObject> action)
    {
      if (action == null)
        throw new ArgumentNullException(nameof (action));
      if ((object) obj == null)
        return;
      action(obj);
    }

    public static void CopyItemsTo<TSource>(
      this ICollection<TSource> fromSet,
      ICollection<TSource> toSet)
    {
      if (toSet == null)
        throw new ArgumentNullException(nameof (toSet));
      if (fromSet == null)
        return;
      foreach (TSource from in (IEnumerable<TSource>) fromSet)
        toSet.Add(from);
    }
  }
}
