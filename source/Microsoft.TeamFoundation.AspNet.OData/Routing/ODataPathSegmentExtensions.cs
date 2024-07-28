// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Routing.ODataPathSegmentExtensions
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using Microsoft.OData;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.AspNet.OData.Routing
{
  internal static class ODataPathSegmentExtensions
  {
    public static string TranslatePathTemplateSegment(
      this PathTemplateSegment pathTemplatesegment,
      out string value)
    {
      string str = pathTemplatesegment != null ? pathTemplatesegment.LiteralText : throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (pathTemplatesegment));
      if (str == null)
        throw new ODataException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.InvalidAttributeRoutingTemplateSegment, (object) string.Empty));
      if (str.StartsWith("{", StringComparison.Ordinal) && str.EndsWith("}", StringComparison.Ordinal))
      {
        string[] strArray = str.Substring(1, str.Length - 2).Split(':');
        if (strArray.Length != 2)
          throw new ODataException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.InvalidAttributeRoutingTemplateSegment, (object) str));
        value = "{" + strArray[0] + "}";
        return strArray[1];
      }
      value = string.Empty;
      return string.Empty;
    }

    public static string ToUriLiteral(this MetadataSegment segment)
    {
      if (segment == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (segment));
      return "$metadata";
    }

    public static string ToUriLiteral(this ValueSegment segment)
    {
      if (segment == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (segment));
      return "$value";
    }

    public static string ToUriLiteral(this BatchSegment segment)
    {
      if (segment == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (segment));
      return "$batch";
    }

    public static string ToUriLiteral(this CountSegment segment)
    {
      if (segment == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (segment));
      return "$count";
    }

    public static string ToUriLiteral(this TypeSegment segment)
    {
      IEdmType type = segment != null ? segment.EdmType : throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (segment));
      if (segment.EdmType.TypeKind == EdmTypeKind.Collection)
        type = ((IEdmCollectionType) segment.EdmType).ElementType.Definition;
      return type.FullTypeName();
    }

    public static string ToUriLiteral(this SingletonSegment segment)
    {
      if (segment == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (segment));
      return segment.Singleton.Name;
    }

    public static string ToUriLiteral(this PropertySegment segment)
    {
      if (segment == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (segment));
      return segment.Property.Name;
    }

    public static string ToUriLiteral(this PathTemplateSegment segment) => segment != null ? segment.LiteralText : throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (segment));

    public static string ToUriLiteral(this DynamicPathSegment segment) => segment != null ? segment.Identifier : throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (segment));

    public static string ToUriLiteral(this NavigationPropertySegment segment)
    {
      if (segment == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (segment));
      return segment.NavigationProperty.Name;
    }

    public static string ToUriLiteral(this NavigationPropertyLinkSegment segment)
    {
      if (segment == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (segment));
      return segment.NavigationProperty.Name + "/$ref";
    }

    public static string ToUriLiteral(this KeySegment segment)
    {
      if (segment == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (segment));
      return ODataPathSegmentExtensions.ConvertKeysToUriLiteral(segment.Keys, segment.EdmType);
    }

    public static string ToUriLiteral(this EntitySetSegment segment)
    {
      if (segment == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (segment));
      return segment.EntitySet.Name;
    }

    public static string ToUriLiteral(this OperationSegment segment)
    {
      if (segment == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (segment));
      if (segment.Operations.Single<IEdmOperation>() is IEdmAction element1)
        return element1.FullName();
      Dictionary<string, string> dictionary = segment.Parameters.ToDictionary<OperationSegmentParameter, string, string>((Func<OperationSegmentParameter, string>) (parameterValue => parameterValue.Name), (Func<OperationSegmentParameter, string>) (parameterValue => ODataPathSegmentExtensions.TranslateNode(parameterValue.Value)));
      IEdmFunction element2 = (IEdmFunction) segment.Operations.Single<IEdmOperation>();
      IEnumerable<string> values = dictionary.Select<KeyValuePair<string, string>, string>((Func<KeyValuePair<string, string>, string>) (v => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}={1}", new object[2]
      {
        (object) v.Key,
        (object) v.Value
      })));
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}({1})", new object[2]
      {
        (object) element2.FullName(),
        (object) string.Join(",", values)
      });
    }

    public static string ToUriLiteral(this OperationImportSegment segment)
    {
      if (segment == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (segment));
      if (segment.OperationImports.Single<IEdmOperationImport>() is IEdmActionImport edmActionImport)
        return edmActionImport.Name;
      Dictionary<string, string> dictionary = segment.Parameters.ToDictionary<OperationSegmentParameter, string, string>((Func<OperationSegmentParameter, string>) (parameterValue => parameterValue.Name), (Func<OperationSegmentParameter, string>) (parameterValue => ODataPathSegmentExtensions.TranslateNode(parameterValue.Value)));
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}({1})", new object[2]
      {
        (object) segment.OperationImports.Single<IEdmOperationImport>().Name,
        (object) string.Join(",", dictionary.Select<KeyValuePair<string, string>, string>((Func<KeyValuePair<string, string>, string>) (v => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}={1}", new object[2]
        {
          (object) v.Key,
          (object) v.Value
        }))))
      });
    }

    public static string ToUriLiteral(this UnresolvedPathSegment segment) => segment != null ? segment.SegmentValue : throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (segment));

    private static string ConvertKeysToUriLiteral(
      IEnumerable<KeyValuePair<string, object>> keys,
      IEdmType edmType)
    {
      IEdmEntityType type = edmType as IEdmEntityType;
      if (keys.Count<KeyValuePair<string, object>>() < 2)
      {
        KeyValuePair<string, object> keyValue = keys.First<KeyValuePair<string, object>>();
        if (type.Key().Any<IEdmStructuralProperty>((Func<IEdmStructuralProperty, bool>) (k => k.Name == keyValue.Key)))
          return string.Join(",", keys.Select<KeyValuePair<string, object>, string>((Func<KeyValuePair<string, object>, string>) (keyValuePair => ODataPathSegmentExtensions.TranslateKeySegmentValue(keyValuePair.Value))).ToArray<string>());
      }
      return string.Join(",", keys.Select<KeyValuePair<string, object>, string>((Func<KeyValuePair<string, object>, string>) (keyValuePair => keyValuePair.Key + "=" + ODataPathSegmentExtensions.TranslateKeySegmentValue(keyValuePair.Value))).ToArray<string>());
    }

    private static string TranslateKeySegmentValue(object value)
    {
      switch (value)
      {
        case null:
          throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (value));
        case UriTemplateExpression templateExpression:
          return templateExpression.LiteralText;
        case ConstantNode constantNode:
          if (constantNode.Value is ODataEnumValue odataEnumValue)
            return ODataUriUtils.ConvertToUriLiteral((object) odataEnumValue, ODataVersion.V4);
          break;
      }
      return ODataUriUtils.ConvertToUriLiteral(value, ODataVersion.V4);
    }

    private static string TranslateNode(object node)
    {
      switch (node)
      {
        case null:
          throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (node));
        case ConstantNode constantNode:
          if (constantNode.Value is UriTemplateExpression templateExpression)
            return templateExpression.LiteralText;
          return constantNode.Value is ODataEnumValue odataEnumValue ? ODataUriUtils.ConvertToUriLiteral((object) odataEnumValue, ODataVersion.V4) : constantNode.LiteralText;
        case ConvertNode convertNode:
          return ODataPathSegmentExtensions.TranslateNode((object) convertNode.Source);
        default:
          throw Microsoft.AspNet.OData.Common.Error.NotSupported(SRResources.CannotRecognizeNodeType, (object) typeof (ODataPathSegmentTranslator), (object) node.GetType().FullName);
      }
    }
  }
}
