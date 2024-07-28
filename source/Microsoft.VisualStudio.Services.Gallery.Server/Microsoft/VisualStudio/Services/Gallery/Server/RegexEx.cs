// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.RegexEx
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using System;
using System.Text.RegularExpressions;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  public static class RegexEx
  {
    private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(15.0);

    public static Regex CreateWithTimeout(string pattern, RegexOptions options) => new Regex(pattern, options, RegexEx.Timeout);

    public static string ReplaceWithTimeout(
      string input,
      string pattern,
      string replacement,
      RegexOptions options)
    {
      return Regex.Replace(input, pattern, replacement, options, RegexEx.Timeout);
    }

    public static string ReplaceWithTimeoutOrOriginal(
      string input,
      string pattern,
      MatchEvaluator evaluator,
      RegexOptions options)
    {
      try
      {
        return Regex.Replace(input, pattern, evaluator, options, RegexEx.Timeout);
      }
      catch (RegexMatchTimeoutException ex)
      {
        return input;
      }
    }

    public static Match MatchWithTimeoutOrNull(string input, string pattern, RegexOptions options)
    {
      try
      {
        return Regex.Match(input, pattern, options, RegexEx.Timeout);
      }
      catch (RegexMatchTimeoutException ex)
      {
        return (Match) null;
      }
    }

    public static MatchCollection MatchesWithTimeoutOrNull(
      string input,
      string pattern,
      RegexOptions options)
    {
      try
      {
        return Regex.Matches(input, pattern, options, RegexEx.Timeout);
      }
      catch (RegexMatchTimeoutException ex)
      {
        return (MatchCollection) null;
      }
    }
  }
}
