// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.ReSearch.Core.Parsers.MD.MarkDownParsedContentWriter
// Assembly: Microsoft.VisualStudio.Services.Search.ReSearch.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 657A74AE-F2A6-4615-BB2F-7FA1F961B173
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.ReSearch.Core.dll

using Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Core.Parsing;
using Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Parsers;
using Microsoft.VisualStudio.Services.Search.ReSearch.Core.Parsers.ContentWriters;

namespace Microsoft.VisualStudio.Services.Search.ReSearch.Core.Parsers.MD
{
  internal class MarkDownParsedContentWriter : IParsedContentWriter
  {
    public ParsedData WriteParsedContent(IParsedContent parsedContent)
    {
      MarkDownParsedContent downParsedContent = parsedContent as MarkDownParsedContent;
      return new ParsedData()
      {
        Content = downParsedContent.SerializeToByteArray<MarkDownParsedContent>()
      };
    }
  }
}
