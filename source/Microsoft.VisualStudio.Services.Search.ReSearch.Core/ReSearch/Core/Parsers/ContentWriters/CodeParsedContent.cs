// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.ReSearch.Core.Parsers.ContentWriters.CodeParsedContent
// Assembly: Microsoft.VisualStudio.Services.Search.ReSearch.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 657A74AE-F2A6-4615-BB2F-7FA1F961B173
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.ReSearch.Core.dll

using Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Tokenizers.Text;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.ReSearch.Core.Parsers.ContentWriters
{
  public class CodeParsedContent : IParsedContent
  {
    public CodeParsedContent(IEnumerable<TextToken> tokens, bool hasCodeSymbols, string content)
    {
      this.Symbols = tokens;
      this.HasCodeSymbols = hasCodeSymbols;
      this.Content = content;
    }

    public IEnumerable<TextToken> Symbols { get; set; }

    public bool HasCodeSymbols { get; private set; }

    public string Content { get; private set; }
  }
}
