// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Validation.ScriptTaskValidator
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Validation
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class ScriptTaskValidator
  {
    private readonly Regex[] m_regexesToMatch;
    private readonly string[] m_stringsToMatch;

    public ScriptTaskValidator(
      ScriptTaskValidator.IBadTokenProvider clientTokenPrv = null)
    {
      HashSet<Regex> source1 = new HashSet<Regex>(ScriptTaskValidator.RegexPatternComparer.Instance);
      HashSet<string> source2 = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      List<ScriptTaskValidator.IBadTokenProvider> badTokenProviderList = new List<ScriptTaskValidator.IBadTokenProvider>(2)
      {
        ScriptTaskValidator.BaseBadTokenProvider.Instance
      };
      if (clientTokenPrv != null)
        badTokenProviderList.Add(clientTokenPrv);
      foreach (ScriptTaskValidator.IBadTokenProvider badTokenProvider in badTokenProviderList)
      {
        foreach (string pattern in badTokenProvider.GetRegexPatternsToMatch())
          source1.Add(new Regex(pattern, RegexOptions.Compiled, TimeSpan.FromMilliseconds(100.0)));
        foreach (string str in badTokenProvider.GetStaticTokensToMatch())
          source2.Add(str);
      }
      this.m_regexesToMatch = source1.ToArray<Regex>();
      this.m_stringsToMatch = source2.ToArray<string>();
    }

    public bool HasBadParamOrArgument(
      string exeAndArgs,
      out string matchedPattern,
      out string matchedToken)
    {
      ArgumentUtility.CheckForNull<string>(exeAndArgs, nameof (exeAndArgs));
      foreach (string input in exeAndArgs.Split())
      {
        for (int index = 0; index < this.m_stringsToMatch.Length; ++index)
        {
          string str = this.m_stringsToMatch[index];
          if (input.IndexOf(str, StringComparison.OrdinalIgnoreCase) >= 0)
          {
            matchedPattern = str;
            matchedToken = input;
            return true;
          }
        }
        for (int index = 0; index < this.m_regexesToMatch.Length; ++index)
        {
          Regex regex = this.m_regexesToMatch[index];
          if (regex.IsMatch(input))
          {
            matchedPattern = regex.ToString();
            matchedToken = input;
            return true;
          }
        }
      }
      matchedPattern = (string) null;
      matchedToken = (string) null;
      return false;
    }

    public interface IBadTokenProvider
    {
      IEnumerable<string> GetRegexPatternsToMatch();

      IEnumerable<string> GetStaticTokensToMatch();
    }

    private sealed class BaseBadTokenProvider : ScriptTaskValidator.IBadTokenProvider
    {
      public static readonly ScriptTaskValidator.IBadTokenProvider Instance = (ScriptTaskValidator.IBadTokenProvider) new ScriptTaskValidator.BaseBadTokenProvider();

      private BaseBadTokenProvider()
      {
      }

      public IEnumerable<string> GetRegexPatternsToMatch()
      {
        return (IEnumerable<string>) new string[5]
        {
          wrapInDelimeters("4[1-9a-km-zA-HJ-NP-Z]{94}"),
          wrapInDelimeters("4[1-9a-km-zA-HJ-NP-Z]{105}"),
          wrapInDelimeters("[1-3][1-9a-km-zA-HJ-NP-Z]{32,34}"),
          wrapInDelimeters("bc[0-9]{1,2}([0-9a-zA-Z]){39}"),
          wrapInDelimeters("bc[0-9]{1,2}([0-9a-zA-Z]){59}")
        };

        static string wrapInDelimeters(string argument) => "(^|=)" + argument + "$";
      }

      public IEnumerable<string> GetStaticTokensToMatch() => (IEnumerable<string>) new string[17]
      {
        "xmr.suprnova.cc",
        "MoneroOcean.stream",
        "supportXMR.com",
        "xmr.nanopool.org",
        "monero.hashvault.pro",
        "MoriaXMR.com",
        "xmrpool.",
        "minergate.com",
        "viaxmr.com",
        "xmr.suprnova.cc",
        "--donate-level",
        "cpuminer",
        "cpuminer-opt",
        "cryptonight",
        "sgminer",
        "xmrig",
        "nheqminer"
      };
    }

    private sealed class RegexPatternComparer : IEqualityComparer<Regex>
    {
      public static readonly IEqualityComparer<Regex> Instance = (IEqualityComparer<Regex>) new ScriptTaskValidator.RegexPatternComparer();

      private RegexPatternComparer()
      {
      }

      public bool Equals(Regex x, Regex y) => x.ToString() == y.ToString();

      public int GetHashCode(Regex obj) => obj.GetHashCode();
    }
  }
}
