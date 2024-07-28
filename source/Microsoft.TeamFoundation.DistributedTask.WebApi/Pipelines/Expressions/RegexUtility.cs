// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Expressions.RegexUtility
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using System;
using System.Text.RegularExpressions;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Expressions
{
  internal static class RegexUtility
  {
    private static TimeSpan s_regexTimeout = TimeSpan.FromSeconds(2.0);

    public static TimeSpan GetRegexTimeOut() => RegexUtility.s_regexTimeout;

    public static bool IsMatch(string value, string regexPattern, string regexOptionsString) => RegexUtility.IsSafeMatch(value, regexPattern, RegexUtility.ConvertToRegexOptions(regexOptionsString));

    public static bool IsMatch(string value, string wellKnownRegexKey)
    {
      Lazy<Regex> regex1 = WellKnownRegularExpressions.GetRegex(wellKnownRegexKey);
      if (regex1 == null)
        return true;
      Regex regex = regex1.Value;
      return RegexUtility.IsSafeMatch(value, (Func<string, Match>) (x => regex.Match(value)));
    }

    public static RegexOptions ConvertToRegexOptions(string regexOptions)
    {
      RegexOptions result;
      if (RegexUtility.TryConvertToRegexOptions(regexOptions, out result))
        return result;
      throw new RegularExpressionInvalidOptionsException(PipelineStrings.InvalidRegexOptions((object) regexOptions, (object) string.Join(",", RegexUtility.WellKnownRegexOptions.All)));
    }

    private static bool TryConvertToRegexOptions(string regexOptions, out RegexOptions result)
    {
      result = RegexOptions.ECMAScript | RegexOptions.CultureInvariant;
      if (string.IsNullOrEmpty(regexOptions))
        return false;
      string str = regexOptions;
      char[] separator = new char[1]{ ',' };
      foreach (string a in str.Split(separator, StringSplitOptions.RemoveEmptyEntries))
      {
        if (string.Equals(a, RegexUtility.WellKnownRegexOptions.IgnoreCase, StringComparison.OrdinalIgnoreCase))
        {
          result |= RegexOptions.IgnoreCase;
        }
        else
        {
          if (!string.Equals(a, RegexUtility.WellKnownRegexOptions.Multiline, StringComparison.OrdinalIgnoreCase))
            return false;
          result |= RegexOptions.Multiline;
        }
      }
      return true;
    }

    private static bool IsSafeMatch(string value, Func<string, Match> getSafeMatch)
    {
      try
      {
        return getSafeMatch(value).Success;
      }
      catch (Exception ex) when (ex is RegexMatchTimeoutException || ex is ArgumentException)
      {
        throw new RegularExpressionValidationFailureException(PipelineStrings.RegexFailed((object) value, (object) ex.Message), ex);
      }
    }

    private static bool IsSafeMatch(string value, string regex, RegexOptions regexOptions) => RegexUtility.IsSafeMatch(value, (Func<string, Match>) (x => RegexUtility.GetSafeMatch(x, regex, regexOptions)));

    private static Match GetSafeMatch(string value, string regex, RegexOptions regexOptions) => Regex.Match(value, regex, regexOptions, RegexUtility.s_regexTimeout);

    private static class WellKnownRegexOptions
    {
      public static string IgnoreCase = nameof (IgnoreCase);
      public static string Multiline = nameof (Multiline);
      public static string[] All = new string[2]
      {
        RegexUtility.WellKnownRegexOptions.IgnoreCase,
        RegexUtility.WellKnownRegexOptions.Multiline
      };
    }
  }
}
