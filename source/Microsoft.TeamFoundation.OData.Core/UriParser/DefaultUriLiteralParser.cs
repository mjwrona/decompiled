// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.DefaultUriLiteralParser
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using System.Collections.Generic;

namespace Microsoft.OData.UriParser
{
  internal sealed class DefaultUriLiteralParser : IUriLiteralParser
  {
    private List<IUriLiteralParser> uriTypeParsers;
    private static DefaultUriLiteralParser singleInstance = new DefaultUriLiteralParser();

    private DefaultUriLiteralParser() => this.uriTypeParsers = new List<IUriLiteralParser>()
    {
      (IUriLiteralParser) CustomUriLiteralParsers.Instance,
      (IUriLiteralParser) UriPrimitiveTypeParser.Instance
    };

    internal static DefaultUriLiteralParser Instance => DefaultUriLiteralParser.singleInstance;

    public object ParseUriStringToType(
      string text,
      IEdmTypeReference targetType,
      out UriLiteralParsingException parsingException)
    {
      parsingException = (UriLiteralParsingException) null;
      foreach (IUriLiteralParser uriTypeParser in this.uriTypeParsers)
      {
        object uriStringToType = uriTypeParser.ParseUriStringToType(text, targetType, out parsingException);
        if (parsingException != null)
          return (object) null;
        if (uriStringToType != null)
          return uriStringToType;
      }
      return (object) null;
    }
  }
}
