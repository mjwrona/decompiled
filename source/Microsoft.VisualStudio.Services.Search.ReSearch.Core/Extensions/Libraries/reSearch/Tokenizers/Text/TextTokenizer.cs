// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Tokenizers.Text.TextTokenizer
// Assembly: Microsoft.VisualStudio.Services.Search.ReSearch.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 657A74AE-F2A6-4615-BB2F-7FA1F961B173
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.ReSearch.Core.dll

using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Tokenizers.Text
{
  internal class TextTokenizer
  {
    public bool AllowUnderscore { get; set; }

    public bool AllowWildcards { get; set; }

    public uint CharacterOffsetBase { get; set; }

    public IDocumentFieldWriter<TextToken> Target { get; set; }

    public uint TokenOffsetBase { get; set; }

    private bool IsTokenCharacter(char c)
    {
      if (char.IsLetterOrDigit(c) || c == '_' && this.AllowUnderscore)
        return true;
      if (!this.AllowWildcards)
        return false;
      return c == '*' || c == '?';
    }

    public virtual IEnumerable<TextToken> Tokenize(string text)
    {
      if (text != null)
      {
        uint tokens = 0;
        for (int index = 0; index < text.Length; ++index)
        {
          if (this.IsTokenCharacter(text[index]))
          {
            int j = index + 1;
            while (j < text.Length && this.IsTokenCharacter(text[j]))
              ++j;
            string str = text.Substring(index, j - index);
            yield return new TextToken()
            {
              CharacterOffset = (uint) index + this.CharacterOffsetBase,
              TokenOffset = tokens + this.TokenOffsetBase,
              Value = str
            };
            ++tokens;
            index = j;
          }
        }
      }
    }
  }
}
