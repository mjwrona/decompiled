// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Core.Parsing.ParserUtils
// Assembly: Microsoft.VisualStudio.Services.Search.ReSearch.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 657A74AE-F2A6-4615-BB2F-7FA1F961B173
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.ReSearch.Core.dll

using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Core.Stores.TreeStore;
using System;

namespace Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Core.Parsing
{
  public static class ParserUtils
  {
    public static byte[] AppendParserTypeToContentId(ParserType parserType, byte[] contentId)
    {
      int length = contentId.Length;
      byte[] destinationArray = new byte[length + 4];
      Array.Copy((Array) contentId, 0, (Array) destinationArray, 0, length);
      destinationArray[length] = (byte) 31;
      destinationArray[length + 1] = (byte) 47;
      destinationArray[length + 2] = (byte) 63;
      destinationArray[length + 3] = (byte) parserType;
      return destinationArray;
    }

    public static ParserType GetParserTypeFromId(byte[] id)
    {
      int index = id.Length - 4;
      return id[index] == (byte) 31 && id[index + 1] == (byte) 47 && id[index + 2] == (byte) 63 ? (ParserType) id[index + 3] : throw new InvalidOperationException("id does not have a parser prefix");
    }

    public static ParserType GetParserType(this IMetaDataStoreItem metaDataStoreItem)
    {
      switch (ProgrammingLanguages.GetProgrammingLanguage(metaDataStoreItem.Extension.ToLowerInvariant()))
      {
        case ProgrammingLanguages.ProgrammingLanguage.VisualBasic:
          return ParserType.VB;
        case ProgrammingLanguages.ProgrammingLanguage.C:
        case ProgrammingLanguages.ProgrammingLanguage.Cpp:
        case ProgrammingLanguages.ProgrammingLanguage.Lex:
        case ProgrammingLanguages.ProgrammingLanguage.CWeb:
          return ParserType.Cpp;
        case ProgrammingLanguages.ProgrammingLanguage.CSharp:
          return ParserType.Cs;
        case ProgrammingLanguages.ProgrammingLanguage.Java:
          return ParserType.Java;
        default:
          return ParserType.Text;
      }
    }
  }
}
