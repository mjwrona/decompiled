// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Logging.RegexSecret
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Microsoft.TeamFoundation.DistributedTask.Logging
{
  internal sealed class RegexSecret : ISecret
  {
    private readonly string m_pattern;
    private readonly Regex m_regex;

    public RegexSecret(string pattern)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(pattern, nameof (pattern));
      this.m_pattern = pattern;
      this.m_regex = new Regex(pattern);
    }

    public override bool Equals(object obj) => obj is RegexSecret regexSecret && string.Equals(this.m_pattern, regexSecret.m_pattern, StringComparison.Ordinal);

    public override int GetHashCode() => this.m_pattern.GetHashCode();

    public IEnumerable<ReplacementPosition> GetPositions(string input)
    {
      int startIndex = 0;
      while (startIndex < input.Length)
      {
        Match match = this.m_regex.Match(input, startIndex);
        if (!match.Success)
          break;
        startIndex = match.Index + 1;
        yield return new ReplacementPosition(match.Index, match.Length);
      }
    }

    public string Pattern => this.m_pattern;
  }
}
