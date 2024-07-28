// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.StringAsEnumResolver
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.OData.UriParser
{
  public sealed class StringAsEnumResolver : ODataUriResolver
  {
    public override void PromoteBinaryOperandTypes(
      BinaryOperatorKind binaryOperatorKind,
      ref SingleValueNode leftNode,
      ref SingleValueNode rightNode,
      out IEdmTypeReference typeReference)
    {
      typeReference = (IEdmTypeReference) null;
      if (leftNode.TypeReference != null && rightNode.TypeReference != null)
      {
        if (leftNode.TypeReference.IsEnum() && rightNode.TypeReference.IsString() && rightNode is ConstantNode)
        {
          string literalText = ((ConstantNode) rightNode).Value as string;
          IEdmTypeReference typeReference1 = leftNode.TypeReference;
          ODataEnumValue enumValue;
          if (StringAsEnumResolver.TryParseEnum(typeReference1.Definition as IEdmEnumType, literalText, out enumValue))
          {
            rightNode = (SingleValueNode) new ConstantNode((object) enumValue, literalText, typeReference1);
            return;
          }
        }
        else if (rightNode.TypeReference.IsEnum() && leftNode.TypeReference.IsString() && leftNode is ConstantNode)
        {
          string literalText = ((ConstantNode) leftNode).Value as string;
          IEdmTypeReference typeReference2 = rightNode.TypeReference;
          ODataEnumValue enumValue;
          if (StringAsEnumResolver.TryParseEnum(typeReference2.Definition as IEdmEnumType, literalText, out enumValue))
          {
            leftNode = (SingleValueNode) new ConstantNode((object) enumValue, literalText, typeReference2);
            return;
          }
        }
      }
      base.PromoteBinaryOperandTypes(binaryOperatorKind, ref leftNode, ref rightNode, out typeReference);
    }

    public override IDictionary<IEdmOperationParameter, SingleValueNode> ResolveOperationParameters(
      IEdmOperation operation,
      IDictionary<string, SingleValueNode> input)
    {
      Dictionary<IEdmOperationParameter, SingleValueNode> dictionary = new Dictionary<IEdmOperationParameter, SingleValueNode>((IEqualityComparer<IEdmOperationParameter>) EqualityComparer<IEdmOperationParameter>.Default);
      foreach (KeyValuePair<string, SingleValueNode> keyValuePair in (IEnumerable<KeyValuePair<string, SingleValueNode>>) input)
      {
        IEdmOperationParameter key = !this.EnableCaseInsensitive ? operation.FindParameter(keyValuePair.Key) : ODataUriResolver.ResolveOperationParameterNameCaseInsensitive(operation, keyValuePair.Key);
        if (key == null)
          throw new ODataException(Microsoft.OData.Strings.ODataParameterWriterCore_ParameterNameNotFoundInOperation((object) keyValuePair.Key, (object) operation.Name));
        SingleValueNode singleValueNode = keyValuePair.Value;
        if (key.Type.IsEnum() && singleValueNode is ConstantNode && singleValueNode.TypeReference != null && singleValueNode.TypeReference.IsString())
        {
          string literalText = ((ConstantNode) keyValuePair.Value).Value as string;
          IEdmTypeReference type = key.Type;
          ODataEnumValue enumValue;
          if (StringAsEnumResolver.TryParseEnum(type.Definition as IEdmEnumType, literalText, out enumValue))
            singleValueNode = (SingleValueNode) new ConstantNode((object) enumValue, literalText, type);
        }
        dictionary.Add(key, singleValueNode);
      }
      return (IDictionary<IEdmOperationParameter, SingleValueNode>) dictionary;
    }

    public override IEnumerable<KeyValuePair<string, object>> ResolveKeys(
      IEdmEntityType type,
      IList<string> positionalValues,
      Func<IEdmTypeReference, string, object> convertFunc)
    {
      return base.ResolveKeys(type, positionalValues, (Func<IEdmTypeReference, string, object>) ((typeRef, valueText) =>
      {
        if (typeRef.IsEnum() && valueText.StartsWith("'", StringComparison.Ordinal) && valueText.EndsWith("'", StringComparison.Ordinal))
          valueText = typeRef.FullName() + valueText;
        return convertFunc(typeRef, valueText);
      }));
    }

    public override IEnumerable<KeyValuePair<string, object>> ResolveKeys(
      IEdmEntityType type,
      IDictionary<string, string> namedValues,
      Func<IEdmTypeReference, string, object> convertFunc)
    {
      return base.ResolveKeys(type, namedValues, (Func<IEdmTypeReference, string, object>) ((typeRef, valueText) =>
      {
        if (typeRef.IsEnum() && valueText.StartsWith("'", StringComparison.Ordinal) && valueText.EndsWith("'", StringComparison.Ordinal))
          valueText = typeRef.FullName() + valueText;
        return convertFunc(typeRef, valueText);
      }));
    }

    private static bool TryParseEnum(
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
