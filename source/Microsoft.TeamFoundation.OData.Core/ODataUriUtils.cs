// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.ODataUriUtils
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using Microsoft.OData.Metadata;
using Microsoft.OData.UriParser;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.OData
{
  public static class ODataUriUtils
  {
    public static object ConvertFromUriLiteral(string value, ODataVersion version) => ODataUriUtils.ConvertFromUriLiteral(value, version, (IEdmModel) null, (IEdmTypeReference) null);

    public static object ConvertFromUriLiteral(
      string value,
      ODataVersion version,
      IEdmModel model,
      IEdmTypeReference typeReference)
    {
      ExceptionUtils.CheckArgumentNotNull<string>(value, nameof (value));
      if (typeReference != null && model == null)
        throw new ODataException(Strings.ODataUriUtils_ConvertFromUriLiteralTypeRefWithoutModel);
      if (model == null)
        model = (IEdmModel) EdmCoreModel.Instance;
      ExpressionLexer expressionLexer = new ExpressionLexer(value, false, false);
      ExpressionToken resultToken;
      expressionLexer.TryPeekNextToken(out resultToken, out Exception _);
      if (resultToken.Kind == ExpressionTokenKind.BracedExpression && typeReference != null && typeReference.IsStructured())
        return ODataUriConversionUtils.ConvertFromResourceValue(value, model, typeReference);
      if (resultToken.Kind == ExpressionTokenKind.BracketedExpression)
        return ODataUriConversionUtils.ConvertFromCollectionValue(value, model, typeReference);
      QueryNode boundEnum;
      if (resultToken.Kind == ExpressionTokenKind.Identifier && EnumBinder.TryBindIdentifier(expressionLexer.ExpressionText, (IEdmEnumTypeReference) null, model, out boundEnum))
        return ((ConstantNode) boundEnum).Value;
      object primitiveValue = expressionLexer.ReadLiteralToken();
      if (typeReference != null)
        primitiveValue = ODataUriConversionUtils.VerifyAndCoerceUriPrimitiveLiteral(primitiveValue, value, model, typeReference);
      return primitiveValue;
    }

    [SuppressMessage("Microsoft.Design", "CA1055:UriReturnValuesShouldNotBeStrings", Justification = "designed to aid the creation on a URI, not create a full one")]
    public static string ConvertToUriLiteral(object value, ODataVersion version) => ODataUriUtils.ConvertToUriLiteral(value, version, (IEdmModel) null);

    [SuppressMessage("Microsoft.Design", "CA1055:UriReturnValuesShouldNotBeStrings", Justification = "designed to aid the creation on a URI, not create a full one")]
    public static string ConvertToUriLiteral(object value, ODataVersion version, IEdmModel model)
    {
      if (value == null)
        value = (object) new ODataNullValue();
      if (model == null)
        model = (IEdmModel) EdmCoreModel.Instance;
      switch (value)
      {
        case ODataNullValue _:
          return "null";
        case ODataResourceValue resourceValue:
          return ODataUriConversionUtils.ConvertToResourceLiteral(resourceValue, model, version);
        case ODataCollectionValue collectionValue:
          return ODataUriConversionUtils.ConvertToUriCollectionLiteral(collectionValue, model, version);
        case ODataEnumValue odataEnumValue:
          return ODataUriConversionUtils.ConvertToUriEnumLiteral(odataEnumValue, version);
        case ODataResourceBase resource:
          return ODataUriConversionUtils.ConvertToUriEntityLiteral(resource, model);
        case ODataEntityReferenceLink link:
          return ODataUriConversionUtils.ConvertToUriEntityReferenceLiteral(link, model);
        case ODataEntityReferenceLinks links:
          return ODataUriConversionUtils.ConvertToUriEntityReferencesLiteral(links, model);
        case IEnumerable<ODataResourceBase> entries:
          return ODataUriConversionUtils.ConvertToUriEntitiesLiteral(entries, model);
        default:
          value = model.ConvertToUnderlyingTypeIfUIntValue(value);
          return ODataUriConversionUtils.ConvertToUriPrimitiveLiteral(value, version);
      }
    }
  }
}
