// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.CustomUriLiteralParsers
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.OData.UriParser
{
  public sealed class CustomUriLiteralParsers : IUriLiteralParser
  {
    private static readonly object Locker = new object();
    private static IUriLiteralParser[] customUriLiteralParsers = new IUriLiteralParser[0];
    private static CustomUriLiteralParsers.UriLiteralParserPerEdmType[] customUriLiteralParserPerEdmType = new CustomUriLiteralParsers.UriLiteralParserPerEdmType[0];
    private static CustomUriLiteralParsers singleInstance;

    private CustomUriLiteralParsers()
    {
    }

    internal static CustomUriLiteralParsers Instance
    {
      get
      {
        if (CustomUriLiteralParsers.singleInstance == null)
          CustomUriLiteralParsers.singleInstance = new CustomUriLiteralParsers();
        return CustomUriLiteralParsers.singleInstance;
      }
    }

    public object ParseUriStringToType(
      string text,
      IEdmTypeReference targetType,
      out UriLiteralParsingException parsingException)
    {
      IUriLiteralParser literalParserByEdmType = CustomUriLiteralParsers.GetUriLiteralParserByEdmType(targetType);
      if (literalParserByEdmType != null)
        return literalParserByEdmType.ParseUriStringToType(text, targetType, out parsingException);
      foreach (IUriLiteralParser uriLiteralParser in CustomUriLiteralParsers.customUriLiteralParsers)
      {
        object uriStringToType = uriLiteralParser.ParseUriStringToType(text, targetType, out parsingException);
        if (parsingException != null)
          return (object) null;
        if (uriStringToType != null)
          return uriStringToType;
      }
      parsingException = (UriLiteralParsingException) null;
      return (object) null;
    }

    public static void AddCustomUriLiteralParser(IUriLiteralParser customUriLiteralParser)
    {
      ExceptionUtils.CheckArgumentNotNull<IUriLiteralParser>(customUriLiteralParser, nameof (customUriLiteralParser));
      lock (CustomUriLiteralParsers.Locker)
        CustomUriLiteralParsers.customUriLiteralParsers = !((IEnumerable<IUriLiteralParser>) CustomUriLiteralParsers.customUriLiteralParsers).Contains<IUriLiteralParser>(customUriLiteralParser) ? ((IEnumerable<IUriLiteralParser>) CustomUriLiteralParsers.customUriLiteralParsers).Concat<IUriLiteralParser>((IEnumerable<IUriLiteralParser>) new IUriLiteralParser[1]
        {
          customUriLiteralParser
        }).ToArray<IUriLiteralParser>() : throw new ODataException(Microsoft.OData.Strings.UriCustomTypeParsers_AddCustomUriTypeParserAlreadyExists);
    }

    public static void AddCustomUriLiteralParser(
      IEdmTypeReference edmTypeReference,
      IUriLiteralParser customUriLiteralParser)
    {
      ExceptionUtils.CheckArgumentNotNull<IEdmTypeReference>(edmTypeReference, nameof (edmTypeReference));
      ExceptionUtils.CheckArgumentNotNull<IUriLiteralParser>(customUriLiteralParser, nameof (customUriLiteralParser));
      lock (CustomUriLiteralParsers.Locker)
      {
        if (CustomUriLiteralParsers.IsEdmTypeAlreadyRegistered(edmTypeReference))
          throw new ODataException(Microsoft.OData.Strings.UriCustomTypeParsers_AddCustomUriTypeParserEdmTypeExists((object) edmTypeReference.FullName()));
        CustomUriLiteralParsers.customUriLiteralParserPerEdmType = ((IEnumerable<CustomUriLiteralParsers.UriLiteralParserPerEdmType>) CustomUriLiteralParsers.customUriLiteralParserPerEdmType).Concat<CustomUriLiteralParsers.UriLiteralParserPerEdmType>((IEnumerable<CustomUriLiteralParsers.UriLiteralParserPerEdmType>) new CustomUriLiteralParsers.UriLiteralParserPerEdmType[1]
        {
          new CustomUriLiteralParsers.UriLiteralParserPerEdmType()
          {
            EdmTypeOfUriParser = edmTypeReference,
            UriLiteralParser = customUriLiteralParser
          }
        }).ToArray<CustomUriLiteralParsers.UriLiteralParserPerEdmType>();
      }
    }

    public static bool RemoveCustomUriLiteralParser(IUriLiteralParser customUriLiteralParser)
    {
      ExceptionUtils.CheckArgumentNotNull<IUriLiteralParser>(customUriLiteralParser, nameof (customUriLiteralParser));
      lock (CustomUriLiteralParsers.Locker)
      {
        CustomUriLiteralParsers.UriLiteralParserPerEdmType[] array1 = ((IEnumerable<CustomUriLiteralParsers.UriLiteralParserPerEdmType>) CustomUriLiteralParsers.customUriLiteralParserPerEdmType).Where<CustomUriLiteralParsers.UriLiteralParserPerEdmType>((Func<CustomUriLiteralParsers.UriLiteralParserPerEdmType, bool>) (parser => !parser.UriLiteralParser.Equals((object) customUriLiteralParser))).ToArray<CustomUriLiteralParsers.UriLiteralParserPerEdmType>();
        IUriLiteralParser[] array2 = ((IEnumerable<IUriLiteralParser>) CustomUriLiteralParsers.customUriLiteralParsers).Where<IUriLiteralParser>((Func<IUriLiteralParser, bool>) (parser => !parser.Equals((object) customUriLiteralParser))).ToArray<IUriLiteralParser>();
        bool flag = array1.Length < CustomUriLiteralParsers.customUriLiteralParserPerEdmType.Length || array2.Length < CustomUriLiteralParsers.customUriLiteralParsers.Length;
        CustomUriLiteralParsers.customUriLiteralParserPerEdmType = array1;
        CustomUriLiteralParsers.customUriLiteralParsers = array2;
        return flag;
      }
    }

    private static bool IsEdmTypeAlreadyRegistered(IEdmTypeReference edmTypeReference) => ((IEnumerable<CustomUriLiteralParsers.UriLiteralParserPerEdmType>) CustomUriLiteralParsers.customUriLiteralParserPerEdmType).Any<CustomUriLiteralParsers.UriLiteralParserPerEdmType>((Func<CustomUriLiteralParsers.UriLiteralParserPerEdmType, bool>) (uriParserOfEdmType => uriParserOfEdmType.EdmTypeOfUriParser.IsEquivalentTo(edmTypeReference)));

    private static IUriLiteralParser GetUriLiteralParserByEdmType(IEdmTypeReference edmTypeReference) => ((IEnumerable<CustomUriLiteralParsers.UriLiteralParserPerEdmType>) CustomUriLiteralParsers.customUriLiteralParserPerEdmType).FirstOrDefault<CustomUriLiteralParsers.UriLiteralParserPerEdmType>((Func<CustomUriLiteralParsers.UriLiteralParserPerEdmType, bool>) (uriParserOfEdmType => uriParserOfEdmType.EdmTypeOfUriParser.IsEquivalentTo(edmTypeReference)))?.UriLiteralParser;

    private sealed class UriLiteralParserPerEdmType
    {
      internal IEdmTypeReference EdmTypeOfUriParser { get; set; }

      internal IUriLiteralParser UriLiteralParser { get; set; }
    }
  }
}
