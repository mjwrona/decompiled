// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.CustomUriLiteralPrefixes
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using System;
using System.Collections.Generic;

namespace Microsoft.OData.UriParser
{
  public static class CustomUriLiteralPrefixes
  {
    private static readonly object Locker = new object();
    private static Dictionary<string, IEdmTypeReference> CustomLiteralPrefixesOfEdmTypes = new Dictionary<string, IEdmTypeReference>((IEqualityComparer<string>) StringComparer.Ordinal);

    public static void AddCustomLiteralPrefix(
      string literalPrefix,
      IEdmTypeReference literalEdmTypeReference)
    {
      ExceptionUtils.CheckArgumentNotNull<IEdmTypeReference>(literalEdmTypeReference, nameof (literalEdmTypeReference));
      ExceptionUtils.CheckArgumentStringNotNullOrEmpty(literalPrefix, nameof (literalPrefix));
      UriParserHelper.ValidatePrefixLiteral(literalPrefix);
      lock (CustomUriLiteralPrefixes.Locker)
      {
        if (CustomUriLiteralPrefixes.CustomLiteralPrefixesOfEdmTypes.ContainsKey(literalPrefix))
          throw new ODataException(Microsoft.OData.Strings.CustomUriTypePrefixLiterals_AddCustomUriTypePrefixLiteralAlreadyExists((object) literalPrefix));
        CustomUriLiteralPrefixes.CustomLiteralPrefixesOfEdmTypes.Add(literalPrefix, literalEdmTypeReference);
      }
    }

    public static bool RemoveCustomLiteralPrefix(string literalPrefix)
    {
      ExceptionUtils.CheckArgumentStringNotNullOrEmpty(literalPrefix, nameof (literalPrefix));
      UriParserHelper.ValidatePrefixLiteral(literalPrefix);
      lock (CustomUriLiteralPrefixes.Locker)
        return CustomUriLiteralPrefixes.CustomLiteralPrefixesOfEdmTypes.Remove(literalPrefix);
    }

    internal static IEdmTypeReference GetEdmTypeByCustomLiteralPrefix(string literalPrefix)
    {
      lock (CustomUriLiteralPrefixes.Locker)
      {
        IEdmTypeReference customLiteralPrefix;
        if (CustomUriLiteralPrefixes.CustomLiteralPrefixesOfEdmTypes.TryGetValue(literalPrefix, out customLiteralPrefix))
          return customLiteralPrefix;
      }
      return (IEdmTypeReference) null;
    }
  }
}
