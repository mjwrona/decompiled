// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.UnaryOperatorBinder
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using System;

namespace Microsoft.OData.UriParser
{
  internal sealed class UnaryOperatorBinder
  {
    private readonly Func<QueryToken, QueryNode> bindMethod;

    internal UnaryOperatorBinder(Func<QueryToken, QueryNode> bindMethod) => this.bindMethod = bindMethod;

    internal QueryNode BindUnaryOperator(UnaryOperatorToken unaryOperatorToken)
    {
      ExceptionUtils.CheckArgumentNotNull<UnaryOperatorToken>(unaryOperatorToken, nameof (unaryOperatorToken));
      SingleValueNode operandFromToken = this.GetOperandFromToken(unaryOperatorToken);
      IEdmTypeReference targetTypeReference = UnaryOperatorBinder.PromoteOperandType(operandFromToken, unaryOperatorToken.OperatorKind);
      SingleValueNode typeIfNeeded = MetadataBindingUtils.ConvertToTypeIfNeeded(operandFromToken, targetTypeReference);
      return (QueryNode) new UnaryOperatorNode(unaryOperatorToken.OperatorKind, typeIfNeeded);
    }

    private static IEdmTypeReference PromoteOperandType(
      SingleValueNode operand,
      UnaryOperatorKind unaryOperatorKind)
    {
      IEdmTypeReference typeReference = operand.TypeReference;
      return TypePromotionUtils.PromoteOperandType(unaryOperatorKind, ref typeReference) ? typeReference : throw new ODataException(Microsoft.OData.Strings.MetadataBinder_IncompatibleOperandError(operand.TypeReference == null ? (object) "<null>" : (object) operand.TypeReference.FullName(), (object) unaryOperatorKind));
    }

    private SingleValueNode GetOperandFromToken(UnaryOperatorToken unaryOperatorToken)
    {
      if (!(this.bindMethod(unaryOperatorToken.Operand) is SingleValueNode operandFromToken))
        throw new ODataException(Microsoft.OData.Strings.MetadataBinder_UnaryOperatorOperandNotSingleValue((object) unaryOperatorToken.OperatorKind.ToString()));
      return operandFromToken;
    }
  }
}
