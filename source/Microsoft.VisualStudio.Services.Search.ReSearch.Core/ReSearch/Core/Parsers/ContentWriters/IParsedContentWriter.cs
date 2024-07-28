// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.ReSearch.Core.Parsers.ContentWriters.IParsedContentWriter
// Assembly: Microsoft.VisualStudio.Services.Search.ReSearch.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 657A74AE-F2A6-4615-BB2F-7FA1F961B173
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.ReSearch.Core.dll

using Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Core.Parsing;

namespace Microsoft.VisualStudio.Services.Search.ReSearch.Core.Parsers.ContentWriters
{
  public interface IParsedContentWriter
  {
    ParsedData WriteParsedContent(IParsedContent parsedContent);
  }
}
