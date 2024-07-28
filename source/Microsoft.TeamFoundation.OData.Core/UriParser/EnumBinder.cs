// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.EnumBinder
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using System;
using System.Globalization;

namespace Microsoft.OData.UriParser
{
  internal sealed class EnumBinder
  {
    internal static bool TryBindDottedIdentifierAsEnum(
      DottedIdentifierToken dottedIdentifierToken,
      SingleValueNode parent,
      BindingState state,
      ODataUriResolver resolver,
      out QueryNode boundEnum)
    {
      return EnumBinder.TryBindIdentifier(dottedIdentifierToken.Identifier, (IEdmEnumTypeReference) null, state.Model, resolver, out boundEnum);
    }

    internal static bool TryBindIdentifier(
      string identifier,
      IEdmEnumTypeReference typeReference,
      IEdmModel modelWhenNoTypeReference,
      out QueryNode boundEnum)
    {
      return EnumBinder.TryBindIdentifier(identifier, typeReference, modelWhenNoTypeReference, (ODataUriResolver) null, out boundEnum);
    }

    internal static bool TryBindIdentifier(
      string identifier,
      IEdmEnumTypeReference typeReference,
      IEdmModel modelWhenNoTypeReference,
      ODataUriResolver resolver,
      out QueryNode boundEnum)
    {
      boundEnum = (QueryNode) null;
      string text = identifier;
      int length = text.IndexOf('\'');
      if (length < 0)
        return false;
      string str1 = text.Substring(0, length);
      if (typeReference != null && !string.Equals(str1, typeReference.FullName(), StringComparison.Ordinal))
        return false;
      IEdmEnumType enumType = typeReference != null ? (IEdmEnumType) typeReference.Definition : UriEdmHelpers.FindEnumTypeFromModel(modelWhenNoTypeReference, str1, resolver);
      if (enumType == null)
        return false;
      UriParserHelper.TryRemovePrefix(str1, ref text);
      UriParserHelper.TryRemoveQuotes(ref text);
      string str2 = text;
      ODataEnumValue enumValue;
      if (!EnumBinder.TryParseEnum(enumType, str2, out enumValue))
        return false;
      IEdmEnumTypeReference typeReference1 = typeReference ?? (IEdmEnumTypeReference) new EdmEnumTypeReference(enumType, false);
      boundEnum = (QueryNode) new ConstantNode((object) enumValue, identifier, (IEdmTypeReference) typeReference1);
      return true;
    }

    internal static bool TryParseEnum(
      IEdmEnumType enumType,
      string value,
      out ODataEnumValue enumValue)
    {
      long parseResult;
      bool flag = enumType.TryParseEnum(value, true, out parseResult);
      enumValue = (ODataEnumValue) null;
      if (flag)
        enumValue = new ODataEnumValue(parseResult.ToString((IFormatProvider) CultureInfo.InvariantCulture), enumType.FullTypeName());
      return flag;
    }
  }
}
