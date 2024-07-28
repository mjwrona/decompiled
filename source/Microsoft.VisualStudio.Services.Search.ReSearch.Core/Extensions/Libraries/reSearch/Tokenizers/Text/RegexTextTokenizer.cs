// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Tokenizers.Text.RegexTextTokenizer
// Assembly: Microsoft.VisualStudio.Services.Search.ReSearch.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 657A74AE-F2A6-4615-BB2F-7FA1F961B173
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.ReSearch.Core.dll

using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Tokenizers.Text
{
  internal class RegexTextTokenizer
  {
    public uint CharacterOffsetBase { get; set; }

    public IEnumerable<TextToken> Tokenize(string text)
    {
      for (Match match = new Regex("(\\w+)|([^\\w\\s]?)").Match(text); match.Success; match = match.NextMatch())
      {
        if (!string.IsNullOrWhiteSpace(match.Groups[0].Value))
          yield return new TextToken()
          {
            CharacterOffset = (uint) match.Groups[0].Index + this.CharacterOffsetBase,
            Value = match.Groups[0].Value
          };
      }
    }
  }
}
