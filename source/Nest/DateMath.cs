// Decompiled with JetBrains decompiler
// Type: Nest.DateMath
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Nest
{
  [JsonFormatter(typeof (DateMathFormatter))]
  public abstract class DateMath : IDateMath
  {
    private static readonly Regex DateMathRegex = new Regex("^(?<anchor>now|.+(?:\\|\\||$))(?<ranges>(?:(?:\\+|\\-)[^\\/]*))?(?<rounding>\\/(?:y|M|w|d|h|m|s))?$");
    protected Union<DateTime, string> Anchor;
    protected DateMathTimeUnit? Round;

    public static DateMathExpression Now => new DateMathExpression("now");

    protected IDateMath Self => (IDateMath) this;

    Union<DateTime, string> IDateMath.Anchor => this.Anchor;

    IList<Tuple<DateMathOperation, DateMathTime>> IDateMath.Ranges { get; } = (IList<Tuple<DateMathOperation, DateMathTime>>) new List<Tuple<DateMathOperation, DateMathTime>>();

    DateMathTimeUnit? IDateMath.Round => this.Round;

    public static DateMathExpression Anchored(DateTime anchor) => new DateMathExpression(anchor);

    public static DateMathExpression Anchored(string dateAnchor) => new DateMathExpression(dateAnchor);

    public static implicit operator DateMath(DateTime dateTime) => (DateMath) DateMath.Anchored(dateTime);

    public static implicit operator DateMath(string dateMath) => DateMath.FromString(dateMath);

    public static DateMath FromString(string dateMath)
    {
      if (dateMath == null)
        return (DateMath) null;
      Match match = DateMath.DateMathRegex.Match(dateMath);
      if (!match.Success)
        throw new ArgumentException("Cannot create a DateMathExpression out of '" + dateMath + "'");
      DateMathExpression dateMathExpression = new DateMathExpression(match.Groups["anchor"].Value);
      if (match.Groups["ranges"].Success)
      {
        string str = match.Groups["ranges"].Value;
        do
        {
          int length = str.Substring(1).IndexOfAny(new char[3]
          {
            '+',
            '-',
            '/'
          });
          if (length == -1)
            length = str.Length - 1;
          string expression = str.Substring(1, length);
          if (str.StartsWith("+", StringComparison.Ordinal))
          {
            dateMathExpression = dateMathExpression.Add((DateMathTime) expression);
            str = str.Substring(length + 1);
          }
          else if (str.StartsWith("-", StringComparison.Ordinal))
          {
            dateMathExpression = dateMathExpression.Subtract((DateMathTime) expression);
            str = str.Substring(length + 1);
          }
          else
            str = (string) null;
        }
        while (!str.IsNullOrEmpty());
      }
      if (match.Groups["rounding"].Success)
      {
        DateMathTimeUnit? nullable = match.Groups["rounding"].Value.Substring(1).ToEnum<DateMathTimeUnit>(StringComparison.Ordinal);
        if (nullable.HasValue)
          return dateMathExpression.RoundTo(nullable.Value);
      }
      return (DateMath) dateMathExpression;
    }

    internal static bool IsValidDateMathString(string dateMath) => dateMath != null && DateMath.DateMathRegex.IsMatch(dateMath);

    internal bool IsValid => this.Self.Anchor.Match<bool>((Func<DateTime, bool>) (_ => true), (Func<string, bool>) (s => !s.IsNullOrEmpty()));

    public override string ToString()
    {
      if (!this.IsValid)
        return string.Empty;
      string separator = this.Self.Round.HasValue || this.Self.Ranges.HasAny<Tuple<DateMathOperation, DateMathTime>>() ? "||" : string.Empty;
      StringBuilder stringBuilder1 = new StringBuilder();
      string str1 = this.Self.Anchor.Match<string>((Func<DateTime, string>) (d => DateMath.ToMinThreeDecimalPlaces(d) + separator), (Func<string, string>) (s => !(s == "now") && !s.EndsWith("||", StringComparison.Ordinal) ? s + separator : s));
      stringBuilder1.Append(str1);
      foreach (Tuple<DateMathOperation, DateMathTime> range in (IEnumerable<Tuple<DateMathOperation, DateMathTime>>) this.Self.Ranges)
      {
        stringBuilder1.Append(range.Item1.GetStringValue());
        stringBuilder1.Append((object) range.Item2);
      }
      DateMathTimeUnit? round = this.Self.Round;
      if (round.HasValue)
      {
        StringBuilder stringBuilder2 = stringBuilder1;
        round = this.Self.Round;
        string str2 = "/" + round.Value.GetStringValue();
        stringBuilder2.Append(str2);
      }
      return stringBuilder1.ToString();
    }

    private static string ToMinThreeDecimalPlaces(DateTime dateTime)
    {
      StringBuilder stringBuilder = Elasticsearch.Net.Extensions.StringBuilderCache.Acquire(33);
      string str = dateTime.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFF", (IFormatProvider) CultureInfo.InvariantCulture);
      stringBuilder.Append(str);
      if (str.Length > 20 && str.Length < 23)
      {
        int num = 23 - str.Length;
        for (int index = 0; index < num; ++index)
          stringBuilder.Append('0');
      }
      switch (dateTime.Kind)
      {
        case DateTimeKind.Utc:
          stringBuilder.Append('Z');
          break;
        case DateTimeKind.Local:
          TimeSpan timeSpan = TimeZoneInfo.Local.GetUtcOffset(dateTime);
          if (timeSpan >= TimeSpan.Zero)
          {
            stringBuilder.Append('+');
          }
          else
          {
            stringBuilder.Append('-');
            timeSpan = timeSpan.Negate();
          }
          DateMath.AppendTwoDigitNumber(stringBuilder, timeSpan.Hours);
          stringBuilder.Append(':');
          DateMath.AppendTwoDigitNumber(stringBuilder, timeSpan.Minutes);
          break;
      }
      return Elasticsearch.Net.Extensions.StringBuilderCache.GetStringAndRelease(stringBuilder);
    }

    private static void AppendTwoDigitNumber(StringBuilder result, int val)
    {
      result.Append((char) (48 + val / 10));
      result.Append((char) (48 + val % 10));
    }
  }
}
