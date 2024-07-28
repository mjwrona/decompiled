// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.BinaryOperatorBinder
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using System;

namespace Microsoft.OData.UriParser
{
  internal sealed class BinaryOperatorBinder
  {
    private readonly Func<QueryToken, QueryNode> bindMethod;
    private readonly ODataUriResolver resolver;

    internal BinaryOperatorBinder(Func<QueryToken, QueryNode> bindMethod, ODataUriResolver resolver)
    {
      this.bindMethod = bindMethod;
      this.resolver = resolver;
    }

    internal QueryNode BindBinaryOperator(BinaryOperatorToken binaryOperatorToken)
    {
      ExceptionUtils.CheckArgumentNotNull<BinaryOperatorToken>(binaryOperatorToken, nameof (binaryOperatorToken));
      SingleValueNode operandFromToken1 = this.GetOperandFromToken(binaryOperatorToken.OperatorKind, binaryOperatorToken.Left);
      SingleValueNode operandFromToken2 = this.GetOperandFromToken(binaryOperatorToken.OperatorKind, binaryOperatorToken.Right);
      IEdmTypeReference typeReference;
      this.resolver.PromoteBinaryOperandTypes(binaryOperatorToken.OperatorKind, ref operandFromToken1, ref operandFromToken2, out typeReference);
      return (QueryNode) new BinaryOperatorNode(binaryOperatorToken.OperatorKind, operandFromToken1, operandFromToken2, typeReference);
    }

    internal static void PromoteOperandTypes(
      BinaryOperatorKind binaryOperatorKind,
      ref SingleValueNode left,
      ref SingleValueNode right,
      TypeFacetsPromotionRules facetsPromotionRules)
    {
      IEdmTypeReference left1;
      IEdmTypeReference right1;
      if (!TypePromotionUtils.PromoteOperandTypes(binaryOperatorKind, left, right, out left1, out right1, facetsPromotionRules))
        throw new ODataException(Microsoft.OData.Strings.MetadataBinder_IncompatibleOperandsError(left.TypeReference == null ? (object) "<null>" : (object) left.TypeReference.FullName(), right.TypeReference == null ? (object) "<null>" : (object) right.TypeReference.FullName(), (object) binaryOperatorKind));
      left = MetadataBindingUtils.ConvertToTypeIfNeeded(left, left1);
      right = MetadataBindingUtils.ConvertToTypeIfNeeded(right, right1);
    }

    private SingleValueNode GetOperandFromToken(
      BinaryOperatorKind operatorKind,
      QueryToken queryToken)
    {
      if (!(this.bindMethod(queryToken) is SingleValueNode operandFromToken))
        throw new ODataException(Microsoft.OData.Strings.MetadataBinder_BinaryOperatorOperandNotSingleValue((object) operatorKind.ToString()));
      return operandFromToken;
    }
  }
}
